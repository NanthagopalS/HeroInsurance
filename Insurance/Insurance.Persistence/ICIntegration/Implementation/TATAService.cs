using Insurance.Core.Contracts.Common;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using ThirdPartyUtilities.Helpers;
using Insurance.Domain.Bajaj;
using Microsoft.AspNetCore.Mvc;
using Insurance.Core.Features.TATA.Command.CKYC;
using Insurance.Core.Features.GoDigit.Command.CKYC;
using Insurance.Domain.HDFC;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Net.NetworkInformation;
using Irony.Parsing;
using DocumentFormat.OpenXml.ExtendedProperties;

namespace Insurance.Persistence.ICIntegration.Implementation;
public class TATAService : ITATAService
{
    private readonly HttpClient _client;
    private readonly ILogger<TATAService> _logger;
    private readonly TATAConfig _tataConfig;
    private readonly IApplicationClaims _applicationClaims;
    private readonly ICommonService _commonService;
    private readonly VehicleTypeConfig _vehicleTypeConfig;
    private readonly PolicyTypeConfig _policyTypeConfig;
    private const string AuthAPIErrorMessage = "Authentication API Issue";
    private const string FWPADeclaration = "Have standalone CPA >= 15 L";
    private const string TWPADeclaration = "Insured has standalone PA cover >= 15 lakhs";
    private const string KYC_SUCCESS = "KYC_SUCCESS";
    private const string KYC_FAILED = "KYC_FAILED";
    private const string POI_REQUIRED = "POI_REQUIRED";
    private const string POA_REQUIRED = "POA_REQUIRED";
    private const string OTP_SENT = "OTP_SENT";
    private const string DOC_REQUIRED = "DOC_REQUIRED";
    private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";
    private const string MESSAGE = "Please enter correct document number or proceed with other insurer";

    public TATAService(ILogger<TATAService> logger,
        HttpClient client,
        IOptions<TATAConfig> options,
        IApplicationClaims applicationClaims,
        IOptions<VehicleTypeConfig> vehicleTypeConfig,
        IOptions<PolicyTypeConfig> policyTypeConfig,
        ICommonService commonService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _tataConfig = options.Value;
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        _vehicleTypeConfig = vehicleTypeConfig.Value;
        _policyTypeConfig = policyTypeConfig.Value;
        _commonService = commonService;
    }
    public async Task<QuoteResponseModelGeneric> GetQuote(QuoteQueryModel query, CancellationToken cancellationToken)
    {
        var quoteVm = new QuoteResponseModelGeneric();
        try
        {
            string reg1 = string.Empty;
            string reg2 = string.Empty;
            string reg3 = string.Empty;
            string reg4 = string.Empty;
            string prevPolicyStartDate = string.Empty;
            string prevPolicyEndDate = string.Empty;
            string prevNCB = string.Empty;
            string prevNCBClaim = "false";
            string ncbProtectAddon = "No";
            string ncbNoOfClaim = string.Empty;
            bool isVehicleAgeLessThan3Years = IsYearGreaterThanValue(Convert.ToDateTime(query.RegistrationDate), Convert.ToDateTime(query.PolicyStartDate), 3);

            double vehicleAge = (Convert.ToDateTime(query.PolicyStartDate) - Convert.ToDateTime(query.RegistrationDate)).TotalDays;

            if (query.IsBrandNewVehicle)
            {
                reg1 = "NEW";
            }
            else
            {
                //vehicle number logic
                string vehicleNumber = string.IsNullOrEmpty(query.VehicleNumber) ? query.VehicleDetails.LicensePlateNumber : query.VehicleNumber;
                var vehicleNumberFormat = VehicleNumberSplit(vehicleNumber).ToList();
                if (vehicleNumberFormat != null && vehicleNumberFormat.Count() > 2 && vehicleNumberFormat.Count() == 4)
                {
                    reg1 = vehicleNumberFormat[0];
                    reg2 = vehicleNumberFormat[1];
                    reg3 = vehicleNumberFormat[2];
                    reg4 = vehicleNumberFormat[3];
                }
                else if (vehicleNumberFormat != null && vehicleNumberFormat.Count() == 2)
                {
                    reg1 = vehicleNumberFormat[0];
                    reg2 = vehicleNumberFormat[1];
                    reg3 = "AB";
                    reg4 = query.VehicleDetails.IsFourWheeler ? "1234" : "1111";
                }
                //Prev Policy Details
                if (query.PolicyTypeId.Equals(_policyTypeConfig.SATP))
                {
                    prevPolicyStartDate = query.PreviousPolicyDetails.PreviousPolicyStartDateSATP;
                    prevPolicyEndDate = query.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP;
                }
                else
                {
                    prevPolicyStartDate = query.PreviousPolicyDetails.PreviousPolicyStartDateSAOD;
                    prevPolicyEndDate = query.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD;
                }
                prevNCB = string.IsNullOrEmpty(query.PreviousPolicyDetails.PreviousNoClaimBonus) ? "0" : query.PreviousPolicyDetails.PreviousNoClaimBonus;
                prevNCBClaim = (!query.CurrentPolicyTypeId.Equals("01") && !query.PreviousPolicyDetails.PreviousPolicyType.Equals("Liability") && query.PreviousPolicyDetails.IsClaimInLastYear) ? "true" : "false";

                //NCB Protect in current policy Addon Logic
                if (query.AddOns.IsNCBRequired && !query.PreviousPolicyDetails.IsClaimInLastYear && Convert.ToInt32(query.PreviousPolicyDetails.PreviousNoClaimBonus) >= 20)
                {
                    ncbProtectAddon = "Yes";
                    ncbNoOfClaim = "1";
                }
            }

            var token = await GetToken(query.LeadId, "Quote", cancellationToken);
            query.Token = token;
            if (token != null)
            {
                _logger.LogInformation("TATA GetQuote Service Request Starts");

                if (query.VehicleDetails.IsFourWheeler)
                {
                    //Engine Secure Option Implementation
                    string engineSecureOption = string.Empty;
                    if (query.AddOns.IsEngineProtectionRequired && query.VehicleDetails.VehicleSegment != null)
                    {
                        if (query.VehicleDetails.VehicleSegment.Equals("MINI") || query.VehicleDetails.VehicleSegment.Equals("COMPACT")
                            || query.VehicleDetails.VehicleSegment.Equals("MPV SUV") || query.VehicleDetails.VehicleSegment.Equals("MID SIZE"))
                        {
                            engineSecureOption = "WITH DEDUCTIBLE";
                        }
                        else if (query.VehicleDetails.VehicleSegment.Equals("HIGH END") || query.VehicleDetails.VehicleSegment.Equals("ULTRA HIGH END")
                            || (query.VehicleDetails.VehicleMake.Equals("MARUTI") && query.AddOns.PackageName.Equals("PLATINUM")))
                        {
                            engineSecureOption = "WITHOUT DEDUCTIBLE";
                        }
                    }

                    //Package Implementation for addon resposne(which is not offering by hero) 
                    if (!string.IsNullOrEmpty(query.AddOns.PackageName))
                    {
                        if (query.AddOns.PackageName.Equals("SILVER"))
                        {
                            query.AddOns.IsGlassFiberRepair = true;
                        }
                        else
                        {
                            query.AddOns.IsGlassFiberRepair = true;
                            query.AddOns.IsEmergencyTranspotationAndHotelExp = true;
                        }
                    }

                    var pvtCarQuoteRequest = new TATAPvtCarQuoteRequestDto()
                    {
                        q_producer_email = _tataConfig.ProducerEmail,
                        q_producer_code = _tataConfig.ProducerCode,
                        pol_start_date = query.PolicyStartDate,
                        pol_plan_variant = query.CurrentPolicyType,
                        pol_plan_id = query.CurrentPolicyTypeId,
                        vehicle_make = query.VehicleDetails.VehicleMake,
                        vehicle_make_no = Convert.ToInt32(query.VehicleDetails.VehicleMakeCode),
                        vehicle_model = query.VehicleDetails.VehicleModel,
                        vehicle_model_no = Convert.ToInt32(query.VehicleDetails.VehicleModelCode),
                        vehicle_variant = query.VehicleDetails.VehicleVariant,
                        vehicle_variant_no = query.VehicleDetails.VehicleVariantCode,
                        is_posp = _applicationClaims.GetRole() == "POSP" ? "Y" : "N",
                        sol_id = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetPOSPId() : string.Empty,
                        q_office_location = _applicationClaims.GetRole() == "POSP" ? _tataConfig.POSPOfficeLocationCode : string.Empty,
                        proposer_type = "Individual",
                        proposer_pincode = "400001",
                        business_type = query.BusinessType,
                        business_type_no = query.BusinessTypeId,
                        dor = query.IsBrandNewVehicle ? DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd") : query.RegistrationDate,
                        dor_first = string.Empty,
                        man_year = Convert.ToInt32(query.RegistrationYear),
                        prev_pol_type = string.IsNullOrEmpty(query.PreviousPolicyDetails?.PreviousPolicyType) ? string.Empty : (query.PreviousPolicyDetails?.PreviousPolicyType),
                        prev_pol_start_date = prevPolicyStartDate,
                        prev_pol_end_date = prevPolicyEndDate,
                        ble_od_start = query.PolicyTypeId.Equals(_policyTypeConfig.SAOD) ? query.PreviousPolicyDetails.PreviousPolicyStartDateSAOD : string.Empty,
                        ble_od_end = query.PolicyTypeId.Equals(_policyTypeConfig.SAOD) ? query.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD : string.Empty,
                        ble_tp_start = query.PolicyTypeId.Equals(_policyTypeConfig.SAOD) ? query.PreviousPolicyDetails.PreviousPolicyStartDateSATP : string.Empty,
                        ble_tp_end = query.PolicyTypeId.Equals(_policyTypeConfig.SAOD) ? query.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP : string.Empty,
                        claim_last = prevNCBClaim,
                        pre_pol_ncb = prevNCB,
                        regno_1 = reg1,
                        regno_2 = reg2,
                        regno_3 = reg3,
                        regno_4 = reg4,
                        //Accessories
                        prev_cnglpg = query.Accessories.IsCNG ? "true" : "false",//prev assumed as yes if opted
                        cng_lpg_cover = query.Accessories.IsCNG ? "Yes" : "No",
                        cng_lpg_si = query.Accessories.IsCNG ? Convert.ToString(query.Accessories.CNGValue) : string.Empty,
                        electrical_si = query.Accessories.IsElectrical ? Convert.ToString(query.Accessories.ElectricalValue) : string.Empty,
                        non_electrical_si = query.Accessories.IsNonElectrical ? Convert.ToString(query.Accessories.NonElectricalValue) : string.Empty,
                        uw_loading = string.Empty,
                        uw_remarks = string.Empty,
                        uw_discount = string.Empty,
                        //PACovers
                        pa_owner = query.PACover.IsUnnamedOWNERDRIVER ? "true" : "false",
                        pa_owner_tenure = SetPATenure(query.PACover.IsUnnamedOWNERDRIVER, query.IsBrandNewVehicle, true),
                        pa_owner_declaration = query.PACover.IsUnnamedOWNERDRIVER ? "None" : FWPADeclaration,
                        pa_unnamed = query.PACover.IsUnnamedPassenger ? "Yes" : string.Empty,
                        pa_unnamed_no = query.PACover.IsUnnamedPassenger ? query.VehicleDetails.VehicleSeatCapacity : string.Empty,
                        pa_unnamed_si = query.PACover.IsUnnamedPassenger ? Convert.ToString(query.PACover.UnnamedPassengerValue) : string.Empty,
                        pa_named = "No",
                        pa_named_no = string.Empty,
                        pa_named_si = string.Empty,
                        pa_unnamed_csi = string.Empty,
                        pa_paid = "No",
                        pa_paid_no = string.Empty,
                        pa_paid_si = string.Empty,
                        ll_paid = query.PACover.IsPaidDriver ? "Yes" : "No",
                        ll_paid_no = query.PACover.IsPaidDriver ? "1" : string.Empty,
                        ll_paid_si = query.PACover.IsPaidDriver ? "200000" : string.Empty,
                        //Discounts
                        automobile_association_cover = "No",
                        vehicle_blind = "No",
                        antitheft_cover = "No",
                        voluntary_amount = string.Empty,
                        tppd_discount = "No",
                        vintage_car = "No",
                        own_premises = "No",
                        //Addons
                        geography_extension = query.AddOns.IsGeoAreaExtension ? "true" : "false",
                        geography_extension_bang = query.GeoAreaCountries.IsBangladesh ? "true" : "false",
                        geography_extension_bhutan = query.GeoAreaCountries.IsBhutan ? "true" : "false",
                        geography_extension_lanka = query.GeoAreaCountries.IsSrilanka ? "true" : "false",
                        geography_extension_maldives = query.GeoAreaCountries.IsMaldives ? "true" : "false",
                        geography_extension_nepal = query.GeoAreaCountries.IsNepal ? "true" : "false",
                        geography_extension_pak = query.GeoAreaCountries.IsPakistan ? "true" : "false",
                        add_towing = query.AddOns.IsTowingRequired ? "Yes" : "No",
                        add_towing_amount = query.AddOns.IsTowingRequired ? "1000" : string.Empty,
                        prev_tyre = query.AddOns.IsTyreProtectionRequired ? "true" : "false",//prev assumed as yes if opted
                        tyre_secure = query.AddOns.IsTyreProtectionRequired ? "Yes" : "No",
                        tyre_secure_options = query.AddOns.IsTyreProtectionRequired ? "REPLACEMENT BASIS" : string.Empty,
                        prev_engine = query.AddOns.IsEngineProtectionRequired ? "true" : "false",//prev assumed as yes if opted
                        engine_secure = query.AddOns.IsEngineProtectionRequired ? "Yes" : "No",
                        engine_secure_options = engineSecureOption,
                        prev_dep = query.AddOns.IsZeroDebt ? "true" : "false",//prev assumed as yes if opted
                        dep_reimburse = query.AddOns.IsZeroDebt ? "Yes" : "No",
                        dep_reimburse_claims = query.AddOns.IsZeroDebt ? "2" : string.Empty,
                        prev_rti = query.AddOns.IsReturnToInvoiceRequired ? "true" : "false",//prev assumed as yes if opted
                        return_invoice = query.AddOns.IsReturnToInvoiceRequired ? "Yes" : "No",
                        prev_consumable = query.AddOns.IsConsumableRequired ? "true" : "false",//prev assumed as yes if opted
                        consumbale_expense = query.AddOns.IsConsumableRequired ? "Yes" : "No",
                        rsa = query.AddOns.IsRoadSideAssistanceRequired ? "Yes" : "No",
                        key_replace = query.AddOns.IsKeyAndLockProtectionRequired ? "Yes" : "No",
                        personal_loss = query.AddOns.IsPersonalBelongingRequired ? "Yes" : "No",
                        emergency_expense = query.AddOns.IsEmergencyTranspotationAndHotelExp ? "Yes" : "No",
                        repair_glass = query.AddOns.IsGlassFiberRepair ? "Yes" : "No",
                        ncb_protection = ncbProtectAddon,
                        ncb_no_of_claims = ncbNoOfClaim,
                        daily_allowance = "No",
                        daily_allowance_limit = string.Empty,
                        allowance_days_accident = string.Empty,
                        allowance_days_loss = string.Empty,
                        franchise_days = string.Empty,
                        load_fibre = "No",
                        load_imported = "No",
                        load_tuition = "No",
                        place_reg_no = query.RTOLocationCode,
                        place_reg = query.RTOLocationName,
                        pre_pol_protect_ncb = string.Empty,
                        claim_last_amount = string.Empty,
                        claim_last_count = string.Empty,
                        quote_id = string.Empty,
                        product_id = _tataConfig.FWProductId,
                        product_code = _tataConfig.FWProductCode,
                        product_name = "Private Car",
                        motor_plan_opted = string.IsNullOrEmpty(query.AddOns.PackageName) ? "SILVER" : query.AddOns.PackageName,
                        motor_plan_opted_no = string.IsNullOrEmpty(query.AddOns.PackageFlag) ? "P1" : query.AddOns.PackageFlag,
                        vehicle_idv = query.BusinessTypeId.Equals("03") ? "0" : Convert.ToString(query.IDVValue),
                        no_past_pol = query.IsBrandNewVehicle ? "Y" : "N",
                        __finalize = "1"
                    };
                    string pvtCarQuoteRequestJson = JsonConvert.SerializeObject(pvtCarQuoteRequest);
                    _logger.LogInformation("TATA GetQuote Request {Request}", pvtCarQuoteRequestJson);
                    return await QuoteResponseFraming(pvtCarQuoteRequestJson, query, cancellationToken);
                }
                else if (query.VehicleDetails.IsTwoWheeler)
                {
                    var twQuoteRequest = new TATATwoWheelerQuoteRequestDto()
                    {
                        q_producer_email = _tataConfig.ProducerEmail,
                        q_producer_code = _tataConfig.ProducerCode,
                        q_agent_pan = string.Empty,
                        quote_id = string.Empty,
                        product_id = _tataConfig.TWProductId,
                        product_code = _tataConfig.TWProductCode,
                        pol_start_date = query.PolicyStartDate,
                        plan_type = query.CurrentPolicyType,
                        pol_plan_id = query.CurrentPolicyTypeId,
                        pol_tenure = query.IsBrandNewVehicle ? "5" : "1",
                        fleet_policy = "false",
                        fleet_code = string.Empty,
                        fleet_name = string.Empty,
                        vehicle_make = query.VehicleDetails.VehicleMake,
                        make_code = query.VehicleDetails.VehicleMakeCode,
                        vehicle_model = query.VehicleDetails.VehicleModel,
                        model_code = query.VehicleDetails.VehicleModelCode,
                        vehicle_variant = query.VehicleDetails.VehicleVariant,
                        variant_code = query.VehicleDetails.VehicleVariantCode,
                        is_posp = _applicationClaims.GetRole() == "POSP" ? "Y" : "N",
                        sol_id = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetPOSPId() : string.Empty,
                        q_office_location = _applicationClaims.GetRole() == "POSP" ? _tataConfig.POSPOfficeLocationCode : string.Empty,
                        proposer_type = "Individual",
                        proposer_pincode = "400001",
                        business_type = query.BusinessType,
                        business_type_no = query.BusinessTypeId,
                        dor = query.IsBrandNewVehicle ? DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd") : query.RegistrationDate,
                        man_year = query.RegistrationYear,
                        manu_month = Convert.ToDateTime(query.RegistrationDate).ToString("MM"),
                        prev_pol_type = string.IsNullOrEmpty(query.PreviousPolicyDetails?.PreviousPolicyType) ? string.Empty : (query.PreviousPolicyDetails?.PreviousPolicyType),
                        prev_pol_start_date = prevPolicyStartDate,
                        prev_pol_end_date = prevPolicyEndDate,
                        ble_od_start = query.PolicyTypeId.Equals(_policyTypeConfig.SAOD) ? query.PreviousPolicyDetails.PreviousPolicyStartDateSAOD : string.Empty,
                        ble_od_end = query.PolicyTypeId.Equals(_policyTypeConfig.SAOD) ? query.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD : string.Empty,
                        ble_tp_start = query.PolicyTypeId.Equals(_policyTypeConfig.SAOD) ? query.PreviousPolicyDetails.PreviousPolicyStartDateSATP : string.Empty,
                        ble_tp_end = query.PolicyTypeId.Equals(_policyTypeConfig.SAOD) ? query.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP : string.Empty,
                        claim_last = prevNCBClaim,
                        pre_pol_ncb = prevNCB,
                        claim_last_amount = string.Empty,
                        claim_last_count = string.Empty,
                        special_regno = "false",
                        regno_1 = reg1,
                        regno_2 = reg2,
                        regno_3 = reg3,
                        regno_4 = reg4,
                        //Accessories
                        cng_lpg = query.Accessories.IsCNG ? "true" : "false",//prev assumed as yes if opted
                        cng_lpg_cover = query.Accessories.IsCNG ? "true" : "false",
                        cng_lpg_si = query.Accessories.IsCNG ? Convert.ToString(query.Accessories.CNGValue) : string.Empty,
                        electrical_acc = query.Accessories.IsElectrical ? "true" : "false",
                        electrical_si = query.Accessories.IsElectrical ? Convert.ToString(query.Accessories.ElectricalValue) : string.Empty,
                        electrical_des = string.Empty,
                        non_electrical_acc = query.Accessories.IsNonElectrical ? "true" : "false",
                        non_electrical_si = query.Accessories.IsNonElectrical ? Convert.ToString(query.Accessories.NonElectricalValue) : string.Empty,
                        non_electrical_des = string.Empty,
                        uw_loading = string.Empty,
                        uw_discount = string.Empty,
                        //PACovers
                        pa_owner = query.PACover.IsUnnamedOWNERDRIVER ? "true" : "false",
                        pa_owner_tenure = query.CurrentPolicyTypeId.Equals("05") ? string.Empty : SetPATenure(query.PACover.IsUnnamedOWNERDRIVER, query.IsBrandNewVehicle, false),
                        pa_owner_declaration = query.PACover.IsUnnamedOWNERDRIVER ? "None" : TWPADeclaration,
                        cpa_start_date = query.PACover.IsUnnamedOWNERDRIVER ? query.PolicyStartDate : string.Empty,
                        cpa_end_date = query.PACover.IsUnnamedOWNERDRIVER ? query.PolicyEndDate : string.Empty,
                        pa_unnamed = query.PACover.IsUnnamedPillionRider ? "true" : "false",
                        pa_unnamed_no = query.PACover.IsUnnamedPillionRider ? query.VehicleDetails.VehicleSeatCapacity : string.Empty,
                        pa_unnamed_si = query.PACover.IsUnnamedPillionRider ? Convert.ToString(query.PACover.UnnamedPillonRiderValue) : string.Empty,
                        ll_paid = query.PACover.IsPaidDriver ? "true" : "false",
                        ll_paid_no = query.PACover.IsPaidDriver ? "1" : string.Empty,
                        add_pa_owner = "false",
                        add_pa_owner_si = string.Empty,
                        driver_age = string.Empty,
                        driver_gender = string.Empty,
                        driver_occupation = string.Empty,
                        add_pa_unnamed = "false",
                        add_pa_unnamed_si = string.Empty,
                        pa_paid = "false",
                        pa_paid_no = string.Empty,
                        pa_paid_si = string.Empty,
                        ll_emp = "false",
                        ll_emp_no = string.Empty,
                        //Discounts
                        automobile_association_cover = "false",
                        automobile_association_mem_no = string.Empty,
                        automobile_association_mem_exp_date = string.Empty,
                        vehicle_blind = "false",
                        antitheft_cover = "false",
                        voluntary_deductibles = "false",
                        voluntary_amount = string.Empty,
                        tppd_discount = query.Discounts.IsLimitedTPCoverage ? "true" : "false",
                        add_tppd = "false",
                        add_tppd_si = string.Empty,
                        own_premises = "false",
                        //Addons
                        geography_extension = query.AddOns.IsGeoAreaExtension ? "true" : "false",
                        geography_extension_bang = query.GeoAreaCountries.IsBangladesh ? "true" : "false",
                        geography_extension_bhutan = query.GeoAreaCountries.IsBhutan ? "true" : "false",
                        geography_extension_lanka = query.GeoAreaCountries.IsSrilanka ? "true" : "false",
                        geography_extension_maldives = query.GeoAreaCountries.IsMaldives ? "true" : "false",
                        geography_extension_nepal = query.GeoAreaCountries.IsNepal ? "true" : "false",
                        geography_extension_pak = query.GeoAreaCountries.IsPakistan ? "true" : "false",
                        dep_reimb = query.AddOns.IsZeroDebt && vehicleAge <= 2097 ? "true" : "false",//prev assumed as yes if opted
                        dep_reimburse = query.AddOns.IsZeroDebt && vehicleAge <= 2097 ? "true" : "false",//5 Years, 8 Months and 28days
                        dep_reimburse_claims = query.AddOns.IsZeroDebt && vehicleAge <= 2097 ? "2" : string.Empty,//5 Years, 8 Months and 28days
                        dep_reimburse_deductible = query.AddOns.IsZeroDebt && vehicleAge <= 2097 ? "0" : string.Empty,//5 Years, 8 Months and 28days
                        add_towing = "false",
                        add_towing_amount = string.Empty,
                        rtn_invoice = query.AddOns.IsReturnToInvoiceRequired && isVehicleAgeLessThan3Years ? "true" : "false",//prev assumed as yes if opted
                        return_invoice = query.AddOns.IsReturnToInvoiceRequired && isVehicleAgeLessThan3Years ? "true" : "false",
                        consumbale_expense = query.AddOns.IsConsumableRequired ? "true" : "false",
                        rsa = query.AddOns.IsRoadSideAssistanceRequired ? "true" : "false",
                        loss_accessories = "false",
                        loss_accessories_idv = string.Empty,
                        driving_tution = "false",
                        vehicle_trails_racing = "false",
                        event_name = string.Empty,
                        event_from_date = string.Empty,
                        event_to_date = string.Empty,
                        promoter_name = string.Empty,
                        ext_racing = "false",
                        imported_veh_without_cus_duty = "false",
                        fibre_fuel_tank = "false",
                        emg_med_exp = "false",
                        emg_med_exp_si = string.Empty,
                        side_car = "false",
                        side_car_idv = string.Empty,
                        place_reg_no = query.RTOLocationCode,
                        place_reg = query.RTOLocationName,
                        veh_plying_city = string.Empty,
                        vehicle_idv = query.BusinessTypeId.Equals("03") ? "0" : Convert.ToString(query.IDVValue),
                        no_past_pol = query.IsBrandNewVehicle ? "Y" : "N",
                        __finalize = "1"
                    };

                    string twQuoteRequestJson = JsonConvert.SerializeObject(twQuoteRequest);
                    _logger.LogInformation("TATA GetQuote Request {Request}", twQuoteRequestJson);
                    return await QuoteResponseFraming(twQuoteRequestJson, query, cancellationToken);
                }
            }
            else
            {
                quoteVm.QuoteResponseModel = new QuoteResponseModel()
                {
                    InsurerStatusCode = (int)HttpStatusCode.BadRequest,
                    ValidationMessage = AuthAPIErrorMessage
                };
                _logger.LogError("TATA GetQuote Error {exception}", AuthAPIErrorMessage);
                return quoteVm;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("TATA GetQuote Error {exception}", ex.Message);
            quoteVm.QuoteResponseModel = new QuoteResponseModel() { InsurerStatusCode = (int)HttpStatusCode.BadRequest };
            return quoteVm;
        }
        throw new NotImplementedException();
    }
    public async Task<string> GetToken(string leadId, string stage, CancellationToken cancellationToken)
    {
        int id = 0;
        try
        {
            var defaultRequestHeaders = _client.DefaultRequestHeaders;
            defaultRequestHeaders.Clear();

            var dict = new Dictionary<string, string>
            {
                { "grant_type", _tataConfig.TokenHeader.grant_type },
                { "client_id", _tataConfig.TokenHeader.client_id },
                { "client_secret", _tataConfig.TokenHeader.client_secret },
                { "scope", _tataConfig.TokenHeader.scope }
            };

            id = await InsertICLogs(string.Empty, leadId, stage, _tataConfig.TokenURL, string.Empty, JsonConvert.SerializeObject(dict));
            _logger.LogInformation("TATA GetToken HeaderBody {HeaderBody}", JsonConvert.SerializeObject(dict));

            var Res = await _client.PostAsync(_tataConfig.TokenURL, new FormUrlEncodedContent(dict), cancellationToken);

            if (Res.IsSuccessStatusCode)
            {
                string responseJson = await Res.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("TATA GetToken Response {Response}", responseJson);

                var result = JsonConvert.DeserializeObject<TATATokenResponse>(responseJson);
                if (result != null && result.access_token != null)
                {
                    UpdateICLogs(id, string.Empty, responseJson);
                    return result.access_token;
                }
            }
            return null;

        }
        catch (Exception ex)
        {
            UpdateICLogs(id, string.Empty, ex.Message);
            _logger.LogError("TATA GetToken Error {exception}", ex.Message);
            return null;
        }
    }
    private async Task<HttpResponseMessage> GetQuoteResponse(bool isFourWheeler, bool isProposal, string requestBody, string token, string leadId, string stage, CancellationToken cancellationToken)
    {
        HttpResponseMessage responseMessage = new HttpResponseMessage();
        var defaultRequestHeaders = _client.DefaultRequestHeaders;
        var id = 0;
        defaultRequestHeaders.Clear();
        defaultRequestHeaders.Add("Authorization", token);

        try
        {
            if (isProposal)
            {
                if (isFourWheeler)
                {
                    id = await InsertICLogs(requestBody, leadId, stage, _tataConfig.BaseURL + _tataConfig.FWProposalURL, token, JsonConvert.SerializeObject(defaultRequestHeaders));
                    responseMessage = await _client.PostAsJsonAsync(_tataConfig.FWProposalURL, JsonConvert.DeserializeObject<TATAPvtCarProposalRequestDto>(requestBody), cancellationToken: cancellationToken);
                }
                else
                {
                    id = await InsertICLogs(requestBody, leadId, stage, _tataConfig.BaseURL + _tataConfig.TWProposalURL, token, JsonConvert.SerializeObject(defaultRequestHeaders));
                    responseMessage = await _client.PostAsJsonAsync(_tataConfig.TWProposalURL, JsonConvert.DeserializeObject<TATATwoWheelerProposalRequestDto>(requestBody), cancellationToken: cancellationToken);
                }

            }
            else
            {
                if (isFourWheeler)
                {
                    id = await InsertICLogs(requestBody, leadId, stage, _tataConfig.BaseURL + _tataConfig.FWQuoteURL, token, JsonConvert.SerializeObject(defaultRequestHeaders));
                    responseMessage = await _client.PostAsJsonAsync(_tataConfig.FWQuoteURL, JsonConvert.DeserializeObject<TATAPvtCarQuoteRequestDto>(requestBody), cancellationToken: cancellationToken);
                }
                else
                {
                    id = await InsertICLogs(requestBody, leadId, stage, _tataConfig.BaseURL + _tataConfig.TWQuoteURL, token, JsonConvert.SerializeObject(defaultRequestHeaders));
                    responseMessage = await _client.PostAsJsonAsync(_tataConfig.TWQuoteURL, JsonConvert.DeserializeObject<TATATwoWheelerQuoteRequestDto>(requestBody), cancellationToken: cancellationToken);
                }

            }
            string responseBody = responseMessage?.Content?.ReadAsStringAsync()?.Result;
            UpdateICLogs(id, null, responseBody);
            return responseMessage;
        }
        catch (Exception ex)
        {
            UpdateICLogs(id, null, ex.Message);
            return default;
        }
    }
    private async Task<QuoteResponseModelGeneric> QuoteResponseFraming(string requestBody, QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
    {
        var quoteVm = new QuoteResponseModelGeneric();
        quoteVm.RequestBody = requestBody;
        string responseBody = string.Empty;


        var responseMessage = await GetQuoteResponse(quoteQuery.VehicleDetails.IsFourWheeler, false, requestBody, quoteQuery.Token, quoteQuery.LeadId, "Quote", cancellationToken);

        if (quoteQuery.VehicleDetails.IsFourWheeler)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                var streamResponse = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                var result = streamResponse.DeserializeFromJson<TATAPvtCarQuoteResponseDto>();
                responseBody = JsonConvert.SerializeObject(result);
                quoteVm.ResponseBody = responseBody;
                quoteVm.QuoteResponseModel = new QuoteResponseModel()
                {
                    InsurerStatusCode = (int)HttpStatusCode.BadRequest,
                    InsurerName = _tataConfig.InsurerName
                };
                _logger.LogError("TATA GetQuote Response {responseBody}", responseBody);
            }
            else
            {
                var streamResponse = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                var result = streamResponse.DeserializeFromJson<TATAPvtCarQuoteResponseDto>();
                responseBody = JsonConvert.SerializeObject(result);
                quoteVm.ResponseBody = responseBody;
                _logger.LogInformation("TATA GetQuote Response {responseBody}", responseBody);
                if (result is not null && result.status == 200)
                {
                    var accessoriesCover = SetAccessoriesCover(quoteQuery, responseBody);
                    var discountCover = SetDiscountCover(quoteQuery, responseBody);
                    var paCover = SetPACover(quoteQuery, responseBody);
                    var addOnCover = SetAddonsCover(quoteQuery, responseBody);
                    var setBaseCover = SetBaseCover(quoteQuery.PolicyTypeId, quoteQuery.VehicleTypeId, responseBody);

                    quoteVm.QuoteResponseModel = new QuoteResponseModel()
                    {
                        InsurerName = _tataConfig.InsurerName,
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        TotalPremium = Math.Round(result.data[0].data.premium_break_up.final_premium).ToString(),
                        GrossPremium = Math.Round(result.data[0].data.premium_break_up.net_premium).ToString(),
                        SelectedIDV = (quoteQuery.IsBrandNewVehicle && quoteQuery.IDVValue == 0) ? 1 : quoteQuery.SelectedIDV,
                        IDV = Convert.ToDecimal(result.data[0].pol_dlts.vehicle_system_idv1),
                        MinIDV = Convert.ToDecimal(result.data[0].data.min_idv),
                        MaxIDV = Convert.ToDecimal(result.data[0].data.max_idv),
                        NCB = Convert.ToString(result.data[0].pol_dlts.curr_ncb_perc),
                        Tax = new ServiceTax
                        {
                            igst = Convert.ToString(result.data[0].data.premium_break_up.igst_prem),
                            cgst = Convert.ToString(result.data[0].data.premium_break_up.cgst_prem),
                            sgst = Convert.ToString(result.data[0].data.premium_break_up.sgst_prem),
                            utgst = Convert.ToString(result.data[0].data.premium_break_up.ugst_prem),
                            totalTax = Math.Round(result.data[0].data.premium_break_up.igst_prem + result.data[0].data.premium_break_up.cgst_prem +
                                        result.data[0].data.premium_break_up.ugst_prem + result.data[0].data.premium_break_up.sgst_prem).ToString()
                        },
                        BasicCover = new BasicCover
                        {
                            CoverList = setBaseCover
                        },
                        PACovers = new PACovers
                        {
                            PACoverList = paCover
                        },
                        AddonCover = new AddonCover
                        {
                            AddonList = addOnCover
                        },
                        AccessoriesCover = new AccessoriesCover
                        {
                            AccessoryList = accessoriesCover
                        },
                        Discount = new Domain.GoDigit.Discount
                        {
                            DiscountList = discountCover
                        },
                        RTOCode = quoteQuery.VehicleDetails.LicensePlateNumber,
                        TransactionID = result.data[0].data.quote_no,
                        PolicyNumber = result.data[0].data.proposal_id,
                        PolicyId = result.data[0].data.policy_id,
                        PolicyStartDate = DateTime.ParseExact(quoteQuery.PolicyStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                        Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                        PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
                        IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                        IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                        RegistrationDate = quoteQuery.IsBrandNewVehicle ? DateTime.Now.AddDays(-2).ToString("dd-MM-yyyy") :
                        DateTime.ParseExact(quoteQuery.RegistrationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                        ManufacturingDate = quoteQuery.IsBrandNewVehicle ? DateTime.Now.AddDays(-2).ToString("dd-MM-yyyy") :
                        DateTime.ParseExact(quoteQuery.RegistrationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                        VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.VehicleDetails.LicensePlateNumber : quoteQuery.VehicleNumber
                    };
                }
                else
                {
                    quoteVm.QuoteResponseModel = new QuoteResponseModel()
                    {
                        InsurerStatusCode = (int)HttpStatusCode.BadRequest,
                        InsurerName = _tataConfig.InsurerName
                    };
                }
            }
        }
        else if (quoteQuery.VehicleDetails.IsTwoWheeler)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                var streamResponse = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                var result = streamResponse.DeserializeFromJson<TATATwoWheelerQuoteResponseDto>();
                responseBody = JsonConvert.SerializeObject(result);
                quoteVm.ResponseBody = responseBody;
                quoteVm.QuoteResponseModel = new QuoteResponseModel()
                {
                    InsurerStatusCode = (int)HttpStatusCode.BadRequest,
                    InsurerName = _tataConfig.InsurerName
                };
                _logger.LogError("TATA GetQuote Response {responseBody}", responseBody);
            }
            else
            {
                var streamResponse = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                var result = streamResponse.DeserializeFromJson<TATATwoWheelerQuoteResponseDto>();
                responseBody = JsonConvert.SerializeObject(result);
                quoteVm.ResponseBody = responseBody;
                _logger.LogInformation("TATA GetQuote Response {responseBody}", responseBody);
                if (result != null && result.status == 200)
                {
                    var accessoriesCover = SetAccessoriesCover(quoteQuery, responseBody);
                    var discountCover = SetDiscountCover(quoteQuery, responseBody);
                    var paCover = SetPACover(quoteQuery, responseBody);
                    var addOnCover = SetAddonsCover(quoteQuery, responseBody);
                    var setBaseCover = SetBaseCover(quoteQuery.PolicyTypeId, quoteQuery.VehicleTypeId, responseBody);
                    decimal maxIdv = Convert.ToDecimal(result.data.data.maximumIDV) > Convert.ToDecimal(result.data.data.ex_showroom_price)
                        ? Convert.ToDecimal(result.data.data.ex_showroom_price) : Convert.ToDecimal(result.data.data.maximumIDV);

                    quoteVm.QuoteResponseModel = new QuoteResponseModel()
                    {
                        InsurerName = _tataConfig.InsurerName,
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        TotalPremium = Math.Round(Convert.ToDouble(result.data.premium_break_up.net_premium)).ToString(),
                        GrossPremium = Math.Round(Convert.ToDouble(result.data.premium_break_up.final_premium)).ToString(),
                        SelectedIDV = (quoteQuery.IsBrandNewVehicle && quoteQuery.IDVValue == 0) ? 1 : quoteQuery.SelectedIDV,
                        IDV = Convert.ToDecimal(result.data.data.baseIDV),
                        MinIDV = Convert.ToDecimal(result.data.data.minimumIDV),
                        MaxIDV = maxIdv,
                        NCB = Convert.ToString(result.data.data.currentYearNCB),
                        Tax = new ServiceTax
                        {
                            igst = Convert.ToString(result.data.premium_break_up.igst_prem),
                            cgst = Convert.ToString(result.data.premium_break_up.cgst_prem),
                            sgst = Convert.ToString(result.data.premium_break_up.sgst_prem),
                            utgst =  Convert.ToString(result.data.premium_break_up.ugst_prem),
                            totalTax = Math.Round(Convert.ToDouble(result.data.premium_break_up.igst_prem) + Convert.ToDouble(result.data.premium_break_up.cgst_prem)
                                        + Convert.ToDouble(result.data.premium_break_up.sgst_prem) + Convert.ToDouble(result.data.premium_break_up.ugst_prem)).ToString()
                        },
                        BasicCover = new BasicCover
                        {
                            CoverList = setBaseCover
                        },
                        PACovers = new PACovers
                        {
                            PACoverList = paCover
                        },
                        AddonCover = new AddonCover
                        {
                            AddonList = addOnCover
                        },
                        AccessoriesCover = new AccessoriesCover
                        {
                            AccessoryList = accessoriesCover
                        },
                        Discount = new Domain.GoDigit.Discount
                        {
                            DiscountList = discountCover
                        },
                        RTOCode = quoteQuery.VehicleDetails.LicensePlateNumber,
                        TransactionID = result.data.quote_no,
                        PolicyNumber = result.data.proposal_id,
                        PolicyId = result.data.policy_id,
                        PolicyStartDate = DateTime.ParseExact(quoteQuery.PolicyStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                        Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                        PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
                        IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                        IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                        RegistrationDate = quoteQuery.IsBrandNewVehicle ? DateTime.Now.AddDays(-2).ToString("dd-MM-yyyy") :
                        DateTime.ParseExact(quoteQuery.RegistrationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                        ManufacturingDate = quoteQuery.IsBrandNewVehicle ? DateTime.Now.AddDays(-2).ToString("dd-MM-yyyy") :
                        DateTime.ParseExact(quoteQuery.RegistrationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                        VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.VehicleDetails.LicensePlateNumber : quoteQuery.VehicleNumber
                    };
                }
                else
                {
                    quoteVm.QuoteResponseModel = new QuoteResponseModel()
                    {
                        InsurerStatusCode = (int)HttpStatusCode.BadRequest,
                        InsurerName = _tataConfig.InsurerName
                    };
                }
            }
        }

        return quoteVm;
    }
    private List<NameValueModel> SetBaseCover(string policyTypeId, string vehicleTypeId, string responseBody)
    {
        List<NameValueModel> baseCoverList = new List<NameValueModel>();
        if (vehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
        {
            var result = JsonConvert.DeserializeObject<TATAPvtCarQuoteResponseDto>(responseBody);
            if (policyTypeId.Equals(_policyTypeConfig.PackageComprehensive) || policyTypeId.Equals(_policyTypeConfig.ComprehensiveBundle))
            {
                baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value =Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_od_premium.od.basic_od)).ToString(),
                        IsApplicable=IsApplicable(result.data[0].data.premium_break_up.total_od_premium.od.basic_od)
                    },
                    new NameValueModel
                    {
                        Name="Third Party Cover Premium",
                        Value =Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_tp_premium.basic_tp)).ToString(),
                        IsApplicable=IsApplicable(result.data[0].data.premium_break_up.total_tp_premium.basic_tp)
                    }
                };
            }
            if (policyTypeId.Equals(_policyTypeConfig.SAOD))
            {
                baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value =Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_od_premium.od.basic_od)).ToString(),
                        IsApplicable=IsApplicable(result.data[0].data.premium_break_up.total_od_premium.od.basic_od)
                    }
                };
            }
            if (policyTypeId.Equals(_policyTypeConfig.SATP))
            {
                baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name="Third Party Cover Premium",
                        Value =Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_tp_premium.basic_tp)).ToString(),
                        IsApplicable=IsApplicable(result.data[0].data.premium_break_up.total_tp_premium.basic_tp)
                    }
                };
            }
        }
        else if (vehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
        {
            var result = JsonConvert.DeserializeObject<TATATwoWheelerQuoteResponseDto>(responseBody);
            if (policyTypeId.Equals(_policyTypeConfig.PackageComprehensive) || policyTypeId.Equals(_policyTypeConfig.ComprehensiveBundle))
            {
                baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value =Math.Round(Convert.ToDouble(result.data.premium_break_up.total_od_premium.od.basic_od)).ToString(),
                        IsApplicable=IsApplicable(result.data.premium_break_up.total_od_premium.od.basic_od)
                    },
                    new NameValueModel
                    {
                        Name="Third Party Cover Premium",
                        Value =Math.Round(Convert.ToDouble(result.data.premium_break_up.total_tp_premium.basic_tp_prem)).ToString(),
                        IsApplicable=IsApplicable(result.data.premium_break_up.total_tp_premium.basic_tp_prem)
                    }
                };
            }
            if (policyTypeId.Equals(_policyTypeConfig.SAOD))
            {
                baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value =Math.Round(Convert.ToDouble(result.data.premium_break_up.total_od_premium.od.basic_od)).ToString(),
                        IsApplicable=IsApplicable(result.data.premium_break_up.total_od_premium.od.basic_od)
                    }
                };
            }
            if (policyTypeId.Equals(_policyTypeConfig.SATP))
            {
                baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name="Third Party Cover Premium",
                        Value =Math.Round(Convert.ToDouble(result.data.premium_break_up.total_tp_premium.basic_tp_prem)).ToString(),
                        IsApplicable=IsApplicable(result.data.premium_break_up.total_tp_premium.basic_tp_prem)
                    }
                };
            }
        }
        return baseCoverList;
    }
    private List<NameValueModel> SetPACover(QuoteQueryModel quoteQuery, string responseBody)
    {
        var paCover = new List<NameValueModel>();
        if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
        {
            var result = JsonConvert.DeserializeObject<TATAPvtCarQuoteResponseDto>(responseBody);
            if (quoteQuery.PACover.IsPaidDriver)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.PaidDriverId,
                    Name = "PA Cover for Paid Driver",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_tp_premium.ll_paid_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_tp_premium.ll_paid_prem)
                }
                );
            }
            if (quoteQuery.PACover.IsUnnamedPassenger)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedPassengerId,
                    Name = "PA Cover for Unnamed Passengers",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_tp_premium.pa_unnamed_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_tp_premium.pa_unnamed_prem)
                }
                );
            }
            if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                    Name = "PA Cover for Owner Driver",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_tp_premium.pa_owner_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_tp_premium.pa_owner_prem)
                });
            }
        }
        else if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
        {
            var result = JsonConvert.DeserializeObject<TATATwoWheelerQuoteResponseDto>(responseBody);
            if (quoteQuery.PACover.IsPaidDriver)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.PaidDriverId,
                    Name = "PA Cover for Paid Driver",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_tp_premium.ll_paid_driver_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_tp_premium.ll_paid_driver_prem)
                }
                );
            }
            if (quoteQuery.PACover.IsUnnamedPillionRider)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedPillionRiderId,
                    Name = "PA Cover For Unnamed Pillion Rider",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_tp_premium.pa_unnamed_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_tp_premium.pa_unnamed_prem)
                }
                );
            }
            if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                    Name = "PA Cover for Owner Driver",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_tp_premium.pa_cover_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_tp_premium.pa_cover_prem)
                });
            }
        }
        return paCover;
    }
    private List<NameValueModel> SetDiscountCover(QuoteQueryModel quoteQuery, string responseBody)
    {
        var discountCover = new List<NameValueModel>();
        if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
        {
            var result = JsonConvert.DeserializeObject<TATAPvtCarQuoteResponseDto>(responseBody);
            if (Convert.ToDouble(result.data[0]?.data.premium_break_up.total_od_premium.discount_od.ncb_prem) > 0)
            {
                discountCover.Add(new NameValueModel
                {
                    Name = "No Claim Bonus",
                    Value = Math.Round(Convert.ToDouble(result.data[0]?.data.premium_break_up.total_od_premium.discount_od.ncb_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0]?.data.premium_break_up.total_od_premium.discount_od.ncb_prem)
                });
            }
            if (quoteQuery.Discounts.IsLimitedTPCoverage)
            {
                discountCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.LimitedTPCoverageId,
                    Name = "Limited Third Party Coverage",
                    IsApplicable = false
                });
            }
        }
        else if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
        {
            var result = JsonConvert.DeserializeObject<TATATwoWheelerQuoteResponseDto>(responseBody);
            if (Convert.ToDouble(result.data.premium_break_up.total_addOns.ncb_prem) > 0)
            {
                discountCover.Add(new NameValueModel
                {
                    Name = "No Claim Bonus",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_addOns.ncb_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_addOns.ncb_prem)
                });
            }
            if (quoteQuery.Discounts.IsLimitedTPCoverage)
            {
                discountCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.LimitedTPCoverageId,
                    Name = "Limited Third Party Coverage",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_tp_premium.disc_tppd_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_tp_premium.disc_tppd_prem)
                });
            }
        }
        if (quoteQuery.Discounts.IsAAMemberShip)
        {
            discountCover.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AAMemberShipId,
                Name = "AA Membership",
                IsApplicable = false
            });
        }
        if (quoteQuery.Discounts.IsVoluntarilyDeductible)
        {
            discountCover.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                Name = "Voluntary Deductible",
                IsApplicable = false
            });
        }
        if (quoteQuery.Discounts.IsAntiTheft)
        {
            discountCover.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AntiTheftId,
                Name = "ARAI Approved Anti-Theft Device",
                IsApplicable = false
            });
        }
        return discountCover;
    }
    private List<NameValueModel> SetAccessoriesCover(QuoteQueryModel quoteQuery, string responseBody)
    {
        var accessoriesCover = new List<NameValueModel>();
        if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
        {
            var result = JsonConvert.DeserializeObject<TATAPvtCarQuoteResponseDto>(responseBody);
            if (quoteQuery.Accessories.IsCNG)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover OD",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_od_premium.od.cng_lpg_od_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_od_premium.od.cng_lpg_od_prem)
                }
                );
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover TP",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_tp_premium.cng_lpg_tp_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_tp_premium.cng_lpg_tp_prem)
                }
                );
            }
            if (quoteQuery.Accessories.IsElectrical)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.ElectricalId,
                    Name = "Electrical Accessory Cover",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_od_premium.od.electrical_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_od_premium.od.electrical_prem)
                });
            }
            if (quoteQuery.Accessories.IsNonElectrical)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.NonElectricalId,
                    Name = "Non-Electrical Accessory Cover",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_od_premium.od.non_electrical_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_od_premium.od.non_electrical_prem)
                });
            }
        }
        else if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
        {
            var result = JsonConvert.DeserializeObject<TATATwoWheelerQuoteResponseDto>(responseBody);
            if (quoteQuery.Accessories.IsCNG)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover OD",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_od_premium.od.cng_lpg_od_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_od_premium.od.cng_lpg_od_prem)
                }
                );
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover TP",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_tp_premium.cng_lpg_tp_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_tp_premium.cng_lpg_tp_prem)
                }
                );
            }
            if (quoteQuery.Accessories.IsElectrical)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.ElectricalId,
                    Name = "Electrical Accessory Cover",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_od_premium.od.electrical_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_od_premium.od.electrical_prem)
                });
            }
            if (quoteQuery.Accessories.IsNonElectrical)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.NonElectricalId,
                    Name = "Non-Electrical Accessory Cover",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_od_premium.od.non_electrical_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_od_premium.od.non_electrical_prem)
                });
            }
        }
        return accessoriesCover;
    }
    private List<NameValueModel> SetAddonsCover(QuoteQueryModel quoteQuery, string responseBody)
    {
        var addOnCover = new List<NameValueModel>();
        if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
        {
            var result = JsonConvert.DeserializeObject<TATAPvtCarQuoteResponseDto>(responseBody);

            //Addon which providing as package
            if (quoteQuery.AddOns.IsZeroDebt)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ZeroDebtId,
                    Name = "Zero Dep",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_addOns.dep_reimburse_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_addOns.dep_reimburse_prem)
                });
            }
            if (quoteQuery.AddOns.IsReturnToInvoiceRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ReturnToInvoiceIdId,
                    Name = "RTI",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_addOns.return_invoice_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_addOns.return_invoice_prem)
                }
                );
            }
            if (quoteQuery.AddOns.IsPersonalBelongingRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.PersonalBelongingId,
                    Name = "Personal Belongings",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_addOns.personal_loss_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_addOns.personal_loss_prem)
                });
            }
            if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                    Name = "Key And Lock Protection",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_addOns.key_replace_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_addOns.key_replace_prem)
                });
            }
            if (quoteQuery.AddOns.IsEngineProtectionRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Engine Gearbox Protection",
                    Id = quoteQuery.AddOns.EngineProtectionId,
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_addOns.engine_secure_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_addOns.engine_secure_prem)
                }
                );
            }
            if (quoteQuery.AddOns.IsTyreProtectionRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Tyre Protection",
                    Id = quoteQuery.AddOns.TyreProtectionId,
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_addOns.tyre_secure_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_addOns.tyre_secure_prem)
                }
                );
            }
            if (quoteQuery.AddOns.IsConsumableRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ConsumableId,
                    Name = "Consumables",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_addOns.consumbale_expense_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_addOns.consumbale_expense_prem)
                });
            }
            if (quoteQuery.AddOns.IsRoadSideAssistanceRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RoadSideAssistanceId,
                    Name = "Road Side Assitance",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_addOns.rsa_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_addOns.rsa_prem)
                }
                );
            }
            if (quoteQuery.AddOns.IsEmergencyTranspotationAndHotelExp)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RoadSideAssistanceId,
                    Name = "Emergency Transport And Hotel",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_addOns.emergency_expense_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_addOns.emergency_expense_prem)
                }
                );
            }
            if (quoteQuery.AddOns.IsGlassFiberRepair)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RoadSideAssistanceId,
                    Name = "Repair of Glass And Fiber",
                    Value = "Included",
                    IsApplicable = true
                }
                );
            }

            //Addons providing individually
            if (quoteQuery.AddOns.IsNCBRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "No Claim Bonus Protection",
                    Id = quoteQuery.AddOns.NCBId,
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_addOns.ncb_protection_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_addOns.ncb_protection_prem)
                }
                );
            }
            if (quoteQuery.AddOns.IsGeoAreaExtension)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    Name = "Geo Area Extension OD",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_od_premium.od.geography_extension_od_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_od_premium.od.geography_extension_od_prem)
                }
                );
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    Name = "Geo Area Extension TP",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_tp_premium.geography_extension_tp_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_tp_premium.geography_extension_tp_prem)
                }
                );
            }
            if (quoteQuery.AddOns.IsTowingRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.TowingId,
                    Name = "Towing Protection",
                    Value = Math.Round(Convert.ToDouble(result.data[0].data.premium_break_up.total_od_premium.od.add_towing_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data[0].data.premium_break_up.total_od_premium.od.add_towing_prem)
                }
                );
            }

            //Addon which are not applicable
            if (quoteQuery.AddOns.IsEMIProtectorRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "EMI Protection",
                    Id = quoteQuery.AddOns.EMIProtectorId,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsLimitedOwnPremisesRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Limited to Own Premises",
                    Id = quoteQuery.AddOns.LimitedOwnPremisesId,
                    IsApplicable = false
                });
            }
            if (quoteQuery.AddOns.IsLossOfDownTimeRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Loss of Down Time Protection",
                    Id = quoteQuery.AddOns.LossOfDownTimeId,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsRoadSideAssistanceWiderRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Road Side Assitance Wider",
                    Id = quoteQuery.AddOns.RoadSideAssistanceWiderId,
                    IsApplicable = false
                });
            }
            if (quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Road Side Assitance Advance",
                    Id = quoteQuery.AddOns.RoadSideAssistanceAdvanceId,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsDailyAllowance)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.DailyAllowanceId,
                    Name = "Daily Allowance",
                    IsApplicable = false
                });
            }
            if (quoteQuery.AddOns.IsRimProtectionRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RimProtectionId,
                    Name = "RIM Protection",
                    IsApplicable = false
                });
            }
        }
        if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
        {
            var result = JsonConvert.DeserializeObject<TATATwoWheelerQuoteResponseDto>(responseBody);
            
            if (quoteQuery.AddOns.IsReturnToInvoiceRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ReturnToInvoiceIdId,
                    Name = "RTI",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_addOns.return_invoice_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_addOns.return_invoice_prem)
                }
                );
            }
            if (quoteQuery.AddOns.IsRoadSideAssistanceRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RoadSideAssistanceId,
                    Name = "Road Side Assitance",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_addOns.rsa_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_addOns.rsa_prem)
                }
                );
            }
            if (quoteQuery.AddOns.IsZeroDebt)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ZeroDebtId,
                    Name = "Zero Dep",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_addOns.dep_reimburse_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_addOns.dep_reimburse_prem)
                });
            }
            if (quoteQuery.AddOns.IsGeoAreaExtension)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    Name = "Geo Area Extension OD",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_od_premium.od.geography_extension_od_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_od_premium.od.geography_extension_od_prem)
                }
                );
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    Name = "Geo Area Extension TP",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_tp_premium.geo_extension_tp_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_tp_premium.geo_extension_tp_prem)
                }
                );
            }
            if (quoteQuery.AddOns.IsConsumableRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ConsumableId,
                    Name = "Consumables",
                    Value = Math.Round(Convert.ToDouble(result.data.premium_break_up.total_addOns.consumbale_expense_prem)).ToString(),
                    IsApplicable = IsApplicable(result.data.premium_break_up.total_addOns.consumbale_expense_prem)
                });
            }
            if (quoteQuery.AddOns.IsPersonalBelongingRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.PersonalBelongingId,
                    Name = "Personal Belongings",
                    IsApplicable = false
                });
            }
            if (quoteQuery.AddOns.IsTowingRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.TowingId,
                    Name = "Towing Protection",
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsEMIProtectorRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "EMI Protection",
                    Id = quoteQuery.AddOns.EMIProtectorId,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsNCBRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "No Claim Bonus Protection",
                    Id = quoteQuery.AddOns.NCBId,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsEngineProtectionRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Engine Gearbox Protection",
                    Id = quoteQuery.AddOns.EngineProtectionId,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsTyreProtectionRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Tyre Protection",
                    Id = quoteQuery.AddOns.TyreProtectionId,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsLimitedOwnPremisesRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Limited to Own Premises",
                    Id = quoteQuery.AddOns.LimitedOwnPremisesId,
                    IsApplicable = false
                });
            }
            if (quoteQuery.AddOns.IsLossOfDownTimeRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Loss of Down Time Protection",
                    Id = quoteQuery.AddOns.LossOfDownTimeId,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsRoadSideAssistanceWiderRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Road Side Assitance Wider",
                    Id = quoteQuery.AddOns.RoadSideAssistanceWiderId,
                    IsApplicable = false
                });
            }
            if (quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Road Side Assitance Advance",
                    Id = quoteQuery.AddOns.RoadSideAssistanceAdvanceId,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsDailyAllowance)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.DailyAllowanceId,
                    Name = "Daily Allowance",
                    IsApplicable = false
                });
            }
            if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                    Name = "Key And Lock Protection",
                    IsApplicable = false
                });
            }
            if (quoteQuery.AddOns.IsRimProtectionRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RimProtectionId,
                    Name = "RIM Protection",
                    IsApplicable = false
                });
            }
        }
        return addOnCover;
    }
    public async Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        QuoteConfirmResponseModel quoteResponseVM = new QuoteConfirmResponseModel();
        QuoteConfirmDetailsResponseModel quoteConfirm = new QuoteConfirmDetailsResponseModel();
        TATAPvtCarQuoteRequestDto fwRequestBody = new();
        TATATwoWheelerQuoteRequestDto twRequestBody = new();

        string responseBody = string.Empty;
        string commonResponse = quoteTransactionDbModel.QuoteTransactionRequest.CommonResponse;
        QuoteResponseModel updatedResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(commonResponse);

        string newPremium = string.Empty;
        string regDate = string.Empty;
        string manufacturingYear = string.Empty;
        string manufacturingMonth = string.Empty;
        string prevPolicyStartDate = string.Empty;
        string prevPolicyEndDate = string.Empty;
        string prevClaim = "false";
        string prevNCB = string.Empty;
        string reg1 = string.Empty;
        string reg2 = string.Empty;
        string reg3 = string.Empty;
        string reg4 = string.Empty;
        string noPastPolicy = "Y";

        string policyStartDate = quoteConfirmCommand.PolicyDates.PolicyStartDate;
        string paCoverEndDate = !quoteConfirmCommand.IsPACover ? Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate)
            .AddYears(Convert.ToInt32(quoteConfirmCommand.PACoverTenure)).AddDays(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : string.Empty;
        manufacturingYear = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ManufacturingDate).ToString("yyyy");
        manufacturingMonth = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ManufacturingDate).ToString("MM");
        regDate = quoteConfirmCommand.IsBrandNewVehicle ? DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") :
             quoteConfirmCommand.PolicyDates.RegistrationDate;
        _logger.LogInformation(manufacturingYear, manufacturingMonth, regDate);

        if (quoteConfirmCommand.IsBrandNewVehicle)
        {
            reg1 = "NEW";
        }
        else
        {
            noPastPolicy = quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy ? "N" : "Y";
            var vehicleNumberFormat = VehicleNumberSplit(quoteConfirmCommand.VehicleNumber).ToList();
            if (vehicleNumberFormat != null && vehicleNumberFormat.Count() > 2 && vehicleNumberFormat.Count() == 4)
            {
                reg1 = vehicleNumberFormat[0];
                reg2 = vehicleNumberFormat[1];
                reg3 = vehicleNumberFormat[2];
                reg4 = vehicleNumberFormat[3];
            }
            //Prev Policy Details
            if (quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SATP))
            {
                prevPolicyStartDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                prevPolicyEndDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else
            {
                prevPolicyStartDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                prevPolicyEndDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            prevClaim = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? "true" : "false";
            prevNCB = string.IsNullOrEmpty(quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue) ? "0" : quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue;
        }

        var token = await GetToken(quoteTransactionDbModel.LeadDetail.LeadID, "QuoteConfirm", cancellationToken);

        if (quoteConfirmCommand.PolicyDates.IsFourWheeler)
        {
            fwRequestBody = JsonConvert.DeserializeObject<TATAPvtCarQuoteRequestDto>(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody);

            fwRequestBody.proposer_type = quoteConfirmCommand.Customertype.Equals("INDIVIDUAL") ? "Individual" : "Organization";
            fwRequestBody.pol_start_date = policyStartDate;
            fwRequestBody.man_year = Convert.ToInt32(manufacturingYear);
            fwRequestBody.dor = regDate;
            fwRequestBody.regno_1 = reg1;
            fwRequestBody.regno_2 = reg2;
            fwRequestBody.regno_3 = reg3;
            fwRequestBody.regno_4 = reg4;
            fwRequestBody.prev_cnglpg = quoteConfirmCommand.isPrevPolicyCNGLPGCover ? "true" : "false";
            fwRequestBody.prev_consumable = quoteConfirmCommand.isPrevPolicyConsumablesCover ? "true" : "false";
            fwRequestBody.prev_rti = quoteConfirmCommand.isPrevPolicyRTICover ? "true" : "false";
            fwRequestBody.prev_dep = quoteConfirmCommand.isPrevPolicyNilDeptCover ? "true" : "false";
            fwRequestBody.prev_engine = quoteConfirmCommand.isPrevPolicyEngineCover ? "true" : "false";
            fwRequestBody.prev_tyre = quoteConfirmCommand.isPrevPolicyTyreCover ? "true" : "false";
            //If CustomerType is "Organization" PA will not be provided
            fwRequestBody.pa_owner = (!quoteConfirmCommand.IsPACover && quoteConfirmCommand.Customertype.Equals("INDIVIDUAL")) ? "true" : "false";
            fwRequestBody.pa_owner_tenure = (!quoteConfirmCommand.IsPACover && quoteConfirmCommand.Customertype.Equals("INDIVIDUAL")) ? quoteConfirmCommand.PACoverTenure : string.Empty;
            fwRequestBody.pa_owner_declaration = (quoteConfirmCommand.Customertype.Equals("COMPANY") || !quoteConfirmCommand.IsPACover) ? "None" : FWPADeclaration;
            //Prev NCB and policy details
            fwRequestBody.prev_pol_start_date = prevPolicyStartDate;
            fwRequestBody.prev_pol_end_date = prevPolicyEndDate;
            fwRequestBody.ble_od_start = quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD) ?
                Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : string.Empty;
            fwRequestBody.ble_od_end = quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD) ?
                Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : string.Empty;
            fwRequestBody.ble_tp_start = quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD) ?
                Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : string.Empty;
            fwRequestBody.ble_tp_end = quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD) ?
                Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : string.Empty;
            fwRequestBody.claim_last = fwRequestBody.prev_pol_type.Equals("Liability") ? "false" : prevClaim;
            fwRequestBody.pre_pol_ncb = fwRequestBody.prev_pol_type.Equals("Liability") ? "0" : prevNCB;
            fwRequestBody.no_past_pol = noPastPolicy;

            var requestBodyFraming = JsonConvert.SerializeObject(fwRequestBody);
            _logger.LogInformation("TATA Quote Confirm Request {requestBodyFraming}", requestBodyFraming);

            if (token != null)
            {
                HttpResponseMessage confirmQuote = await GetQuoteResponse(true, false, requestBodyFraming, token, quoteTransactionDbModel.LeadDetail.LeadID, "QuoteConfirm", cancellationToken);
                var stream = await confirmQuote.Content.ReadAsStreamAsync(cancellationToken);
                var confirmQuoteResult = stream.DeserializeFromJson<TATAPvtCarQuoteResponseDto>();
                responseBody = JsonConvert.SerializeObject(confirmQuoteResult);
                _logger.LogInformation("TATA Quote Confirm Response {responseBody}", responseBody);

                if (confirmQuote.IsSuccessStatusCode)
                {
                    if (confirmQuoteResult.status.Equals(200))
                    {
                        quoteConfirm = new QuoteConfirmDetailsResponseModel
                        {
                            InsurerStatusCode = (int)HttpStatusCode.OK,
                            InsurerName = _tataConfig.InsurerName,
                            NewPremium = Convert.ToString(Math.Round(Convert.ToDouble(confirmQuoteResult.data[0].data.premium_break_up.net_premium))),
                            InsurerId = _tataConfig.InsurerId,
                            IDV = Convert.ToInt32(Math.Round(Convert.ToDouble(confirmQuoteResult.data[0].pol_dlts.vehicle_system_idv1))),
                            MinIDV = Convert.ToInt32(Math.Round(Convert.ToDouble(confirmQuoteResult.data[0].pol_dlts.min_idv))),
                            MaxIDV = Convert.ToInt32(Math.Round(Convert.ToDouble(confirmQuoteResult.data[0].pol_dlts.max_idv))),
                            NCB = Convert.ToString(confirmQuoteResult.data[0].pol_dlts.curr_ncb_perc),
                            Tax = new ServiceTaxModel
                            {
                                totalTax = Math.Round(confirmQuoteResult.data[0].data.premium_break_up.igst_prem).ToString()
                            },
                            TotalPremium = Convert.ToString(Math.Round(confirmQuoteResult.data[0].data.premium_break_up.final_premium)),
                            GrossPremium = Convert.ToString(Math.Round(confirmQuoteResult.data[0].data.premium_break_up.net_premium)),
                            IsBreakin = confirmQuoteResult.data[0].pol_dlts.has_break_in.Equals("Yes"),
                            IsSelfInspection = confirmQuoteResult.data[0].pol_dlts.inspectionFlag.Equals("true")
                        };
                        updatedResponse.TransactionID = confirmQuoteResult.data[0].data.quote_no;
                        updatedResponse.ProposalId = confirmQuoteResult.data[0].data.proposal_id;
                        updatedResponse.PolicyId = confirmQuoteResult.data[0].data.policy_id;
                        updatedResponse.GrossPremium = Convert.ToString(Math.Round(confirmQuoteResult.data[0].data.premium_break_up.net_premium));
                    }
                    else if (confirmQuoteResult.status.Equals(-102))
                    {
                        quoteConfirm = new QuoteConfirmDetailsResponseModel
                        {
                            InsurerStatusCode = (int)HttpStatusCode.BadRequest,
                            ValidationMessage = confirmQuoteResult.message_txt
                        };
                    }
                }
                else
                {
                    if ((confirmQuoteResult.status.Equals(400) || confirmQuoteResult.status.Equals(-102)) && !string.IsNullOrEmpty(confirmQuoteResult.message_txt))
                    {
                        quoteConfirm.ValidationMessage = confirmQuoteResult.message_txt;
                    }
                    else
                    {
                        quoteConfirm.ValidationMessage = ValidationMessage;
                    }
                    quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            quoteResponseVM = new QuoteConfirmResponseModel()
            {
                quoteConfirmResponse = quoteConfirm,
                quoteResponse = updatedResponse,
                RequestBody = requestBodyFraming,
                ResponseBody = responseBody,
                LeadId = quoteTransactionDbModel.LeadDetail.LeadID,
            };
        }
        else if (quoteConfirmCommand.PolicyDates.IsTwoWheeler)
        {
            twRequestBody = JsonConvert.DeserializeObject<TATATwoWheelerQuoteRequestDto>(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody);

            twRequestBody.proposer_type = quoteConfirmCommand.Customertype.Equals("INDIVIDUAL") ? "Individual" : "Organization";
            twRequestBody.pol_start_date = policyStartDate;
            twRequestBody.man_year = manufacturingYear;
            twRequestBody.manu_month = manufacturingMonth;
            twRequestBody.dor = regDate;
            twRequestBody.regno_1 = reg1;
            twRequestBody.regno_2 = reg2;
            twRequestBody.regno_3 = reg3;
            twRequestBody.regno_4 = reg4;
            //If CustomerType is "Organization" PA will not be provided
            twRequestBody.pa_owner = (!quoteConfirmCommand.IsPACover && quoteConfirmCommand.Customertype.Equals("INDIVIDUAL")) ? "true" : "false";
            twRequestBody.pa_owner_tenure = (!twRequestBody.pol_plan_id.Equals("05") && !quoteConfirmCommand.IsPACover && quoteConfirmCommand.Customertype.Equals("INDIVIDUAL")) ? quoteConfirmCommand.PACoverTenure : string.Empty;
            twRequestBody.pa_owner_declaration = (quoteConfirmCommand.Customertype.Equals("COMPANY") || !quoteConfirmCommand.IsPACover) ? "None" : TWPADeclaration;
            twRequestBody.cpa_start_date = (!quoteConfirmCommand.IsPACover && quoteConfirmCommand.Customertype.Equals("INDIVIDUAL")) ? policyStartDate : string.Empty;
            twRequestBody.cpa_end_date = (!quoteConfirmCommand.IsPACover && quoteConfirmCommand.Customertype.Equals("INDIVIDUAL")) ? paCoverEndDate : string.Empty;
            //Prev NCB and policy details
            twRequestBody.prev_pol_start_date = prevPolicyStartDate;
            twRequestBody.prev_pol_end_date = prevPolicyEndDate;
            twRequestBody.ble_od_start = quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD) ?
                Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : string.Empty;
            twRequestBody.ble_od_end = quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD) ?
                Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : string.Empty;
            twRequestBody.ble_tp_start = quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD) ?
                Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : string.Empty;
            twRequestBody.ble_tp_end = quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD) ?
                Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : string.Empty;
            twRequestBody.claim_last = twRequestBody.prev_pol_type.Equals("Liability") ? "false" : prevClaim;
            twRequestBody.pre_pol_ncb = twRequestBody.prev_pol_type.Equals("Liability") ? "0" : prevNCB;
            twRequestBody.no_past_pol = noPastPolicy;

            twRequestBody.cng_lpg = quoteConfirmCommand.isPrevPolicyCNGLPGCover ? "true" : "false";
            twRequestBody.dep_reimb = quoteConfirmCommand.isPrevPolicyNilDeptCover ? "true" : "false";
            twRequestBody.rtn_invoice = quoteConfirmCommand.isPrevPolicyRTICover ? "true" : "false";

            var requestBodyFraming = JsonConvert.SerializeObject(twRequestBody);
            _logger.LogInformation("TATA Quote Confirm Request {requestBodyFraming}", requestBodyFraming);

            if (token != null)
            {
                HttpResponseMessage confirmQuote = await GetQuoteResponse(false, false, requestBodyFraming, token, quoteTransactionDbModel.LeadDetail.LeadID, "QuoteConfirm", cancellationToken);
                var stream = await confirmQuote.Content.ReadAsStreamAsync(cancellationToken);
                var confirmQuoteResult = stream.DeserializeFromJson<TATATwoWheelerQuoteResponseDto>();
                responseBody = JsonConvert.SerializeObject(confirmQuoteResult);
                _logger.LogInformation("TATA Quote Confirm Response {responseBody}", responseBody);

                if (confirmQuote.IsSuccessStatusCode)
                {
                    if (confirmQuoteResult.status.Equals(200))
                    {
                        quoteConfirm = new QuoteConfirmDetailsResponseModel
                        {
                            InsurerStatusCode = (int)HttpStatusCode.OK,
                            InsurerName = _tataConfig.InsurerName,
                            NewPremium = Convert.ToString(Math.Round(Convert.ToDouble(confirmQuoteResult.data.premium_break_up.final_premium))),
                            InsurerId = _tataConfig.InsurerId,
                            IDV = Convert.ToInt32(Math.Round(Convert.ToDouble(confirmQuoteResult.data.data.baseIDV))),
                            MaxIDV = Convert.ToInt32(Math.Round(Convert.ToDouble(confirmQuoteResult.data.data.maximumIDV))),
                            MinIDV = Convert.ToInt32(Math.Round(Convert.ToDouble(confirmQuoteResult.data.data.minimumIDV))),
                            NCB = Convert.ToString(confirmQuoteResult.data.data.currentYearNCB),
                            Tax = new ServiceTaxModel
                            {
                                totalTax = Math.Round(Convert.ToDouble(confirmQuoteResult.data.premium_break_up.igst_prem)).ToString()
                            },
                            TotalPremium = Convert.ToString(Math.Round(Convert.ToDouble(confirmQuoteResult.data.premium_break_up.net_premium))),
                            GrossPremium = Convert.ToString(Math.Round(Convert.ToDouble(confirmQuoteResult.data.premium_break_up.final_premium))),
                            IsBreakin = false,//No Break-In for two wheelere confirmed by IC team
                            IsSelfInspection = false
                        };
                        updatedResponse.TransactionID = confirmQuoteResult.data.quote_no;
                        updatedResponse.ProposalId = confirmQuoteResult.data.proposal_id;
                        updatedResponse.PolicyId = confirmQuoteResult.data.policy_id;
                        updatedResponse.GrossPremium = Convert.ToString(Math.Round(Convert.ToDouble(confirmQuoteResult.data.premium_break_up.net_premium)));
                    }
                    else if (confirmQuoteResult.status.Equals(-102))
                    {
                        quoteConfirm = new QuoteConfirmDetailsResponseModel
                        {
                            InsurerStatusCode = (int)HttpStatusCode.BadRequest,
                            ValidationMessage = confirmQuoteResult.message_txt
                        };
                    }
                }
                else
                {
                    if ((confirmQuoteResult.status.Equals(400) || confirmQuoteResult.status.Equals(-102)) && !string.IsNullOrEmpty(confirmQuoteResult.message_txt))
                    {
                        quoteConfirm.ValidationMessage = confirmQuoteResult.message_txt;
                    }
                    else
                    {
                        quoteConfirm.ValidationMessage = ValidationMessage;
                    }
                    quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            quoteResponseVM = new QuoteConfirmResponseModel()
            {
                quoteConfirmResponse = quoteConfirm,
                quoteResponse = updatedResponse,
                RequestBody = requestBodyFraming,
                ResponseBody = responseBody,
                LeadId = quoteTransactionDbModel.LeadDetail.LeadID,
            };
        }
        return quoteResponseVM;
    }
    public async Task<ProposalResponseModel> CreateProposal(QuoteTransactionRequest quoteDetails, QuoteConfirmDetailsModel quoteConfirmDetails, TATAProposalRequest proposalRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken)
    {
        ProposalResponseModel proposalResponseVM = new ProposalResponseModel();
        TATAPvtCarProposalResponseDto pvtCarProposalResponseDto = new();
        TATATwoWheelerProposalResponseDto twProposalResponseDto = new();
        var proposalVm = new QuoteResponseModel();

        string requestBody = string.Empty;
        string responseBody = string.Empty;
        string prevInsurerName = string.Empty;
        string prevInsurerNumber = string.Empty;
        string odStartDate = string.Empty;
        string odEndDate = string.Empty;
        string odInsurer = string.Empty;
        string odInsurerCode = string.Empty;
        string odPolicyNumber = string.Empty;
        string tpStartDate = string.Empty;
        string tpEndDate = string.Empty;
        string tpPolicyNumber = string.Empty;
        string tpInsurer = string.Empty;
        string tpInsurerCode = string.Empty;
        string tpPrevType = string.Empty;
        ServiceTax tax = JsonConvert.DeserializeObject<ServiceTax>(createLeadModel?.Tax);
        try
        {
            var address = AddressSplit(proposalRequest.AddressDetails.addressLine1);
            if (!createLeadModel.IsBrandNew)
            {
                tpPrevType = createLeadModel.PrevPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive) || createLeadModel.PrevPolicyTypeId.Equals(_policyTypeConfig.SAOD) ? "Package" : "Liability";
                if (createLeadModel.PolicyTypeId.Equals(_policyTypeConfig.SAOD))
                {
                    prevInsurerName = quoteConfirmDetails.PreviousSAODInsurerName;
                    prevInsurerNumber = createLeadModel.PreviousPolicyNumberSAOD;
                    odStartDate = createLeadModel.PreviousSAODPolicyStartDate;
                    odEndDate = createLeadModel.PreviousPolicyExpirySAOD;
                    odInsurer = quoteConfirmDetails.PreviousSAODInsurerName;
                    odInsurerCode = quoteConfirmDetails.SAODInsurerCode;
                    odPolicyNumber = createLeadModel.PreviousPolicyNumberSAOD;
                    tpStartDate = createLeadModel.PreviousSATPPolicyStartDate;
                    tpEndDate = createLeadModel.PrevPolicyExpiryDate;
                    tpPolicyNumber = createLeadModel.PrevPolicyNumber;
                    tpInsurer = quoteConfirmDetails.PreviousSATPInsurerName;
                    tpInsurerCode = quoteConfirmDetails.SATPInsurerCode;
                }
                else
                {
                    prevInsurerName = quoteConfirmDetails.PreviousSATPInsurerName;
                    prevInsurerNumber = !string.IsNullOrEmpty(createLeadModel.PrevPolicyNumber) ? createLeadModel.PrevPolicyNumber : createLeadModel.PreviousPolicyNumberSAOD;
                }
            }
            string dob = Convert.ToDateTime(proposalRequest.PersonalDetails.dateOfBirth, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            string token = await GetToken(createLeadModel.LeadID, "Proposal", cancellationToken);

            if (token != null)
            {
                if (createLeadModel.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
                {
                    var proposalQuery = new TATAPvtCarProposalRequestDto()
                    {
                        proposer_fullname = string.Empty,
                        proposer_salutation = createLeadModel.CarOwnedBy.Equals("COMPANY") ? "M/s." : proposalRequest.PersonalDetails.salutation,
                        proposer_fname = createLeadModel.CarOwnedBy.Equals("COMPANY") ? proposalRequest.PersonalDetails.companyName : proposalRequest.PersonalDetails.firstName,
                        proposer_lname = createLeadModel.CarOwnedBy.Equals("COMPANY") ? string.Empty : proposalRequest.PersonalDetails.lastName,
                        proposer_dob = createLeadModel.CarOwnedBy.Equals("COMPANY") ? string.Empty : dob,
                        proposer_gender = createLeadModel.CarOwnedBy.Equals("COMPANY") ? string.Empty : proposalRequest.PersonalDetails.gender,
                        proposer_mobile = proposalRequest.PersonalDetails.mobile,
                        proposer_email = proposalRequest.PersonalDetails.emailId.ToLower(),
                        proposer_marital = createLeadModel.CarOwnedBy.Equals("COMPANY") ? string.Empty : proposalRequest.PersonalDetails.maritalStatus,
                        proposer_occupation = createLeadModel.CarOwnedBy.Equals("COMPANY") ? string.Empty : proposalRequest.PersonalDetails.occupation,
                        proposer_pan = proposalRequest.PersonalDetails.panNumber,

                        proposer_add1 = address[0] ?? string.Empty,
                        proposer_add2 = address[1] ?? string.Empty,
                        proposer_add3 = address[2] ?? string.Empty,
                        proposer_pincode = proposalRequest.AddressDetails.pincode,

                        proposer_annual = string.Empty,
                        proposer_gstin = createLeadModel.CarOwnedBy.Equals("COMPANY") ? proposalRequest.PersonalDetails.gstno : string.Empty,

                        nominee_name = proposalRequest.NomineeDetails.nomineeName,
                        nominee_age = Convert.ToInt32(proposalRequest.NomineeDetails.nomineeAge),
                        nominee_relation = proposalRequest.NomineeDetails.nomineeRelation,
                        appointee_name = string.Empty,
                        appointee_relation = string.Empty,

                        declaration = "Yes",
                        vehicle_chassis = proposalRequest.VehicleDetails.chassisNumber,
                        vehicle_engine = proposalRequest.VehicleDetails.engineNumber,
                        vehicle_puc = string.Empty,
                        vehicle_puc_expiry = string.Empty,
                        vehicle_puc_declaration = "false",

                        financier_type = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails.agreementType : string.Empty,
                        financier_name = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails.financer : string.Empty,
                        financier_address = string.Empty,

                        // Below fields need to check from db
                        pre_insurer_name = !string.IsNullOrEmpty(prevInsurerName) ? prevInsurerName  :  string.Empty,
                        pre_insurer_no = !string.IsNullOrEmpty(prevInsurerNumber) ? prevInsurerNumber : string.Empty,
                        pre_insurer_address = string.Empty,
                        ble_od_start = odStartDate,
                        ble_od_end = odEndDate,
                        ble_saod_prev_no = odPolicyNumber,
                        ble_tp_type = tpPrevType,
                        ble_tp_no = tpPolicyNumber,
                        ble_tp_start = tpStartDate,
                        ble_tp_end = tpEndDate,
                        ble_tp_name = tpInsurer,
                        ble_tp_tenure = 0,
                        od_pre_insurer_name = odInsurer,
                        od_pre_insurer_no = odPolicyNumber,
                        od_pre_insurer_address = string.Empty,

                        product_id = _tataConfig.FWProductId,
                        proposal_id = quoteDetails?.ProposalId,
                        quote_no = quoteDetails?.TransactionId,
                        carriedOutBy = "Yes",
                        __finalize = "1"
                    };
                    requestBody = JsonConvert.SerializeObject(proposalQuery);
                    _logger.LogInformation("TATA create proposal {request}", requestBody);

                    var proposalResponse = await GetQuoteResponse(true, true, requestBody, token, createLeadModel.LeadID, "Proposal", cancellationToken);

                    if (!proposalResponse.IsSuccessStatusCode)
                    {
                        var stream = await proposalResponse.Content.ReadAsStreamAsync(cancellationToken);
                        pvtCarProposalResponseDto = stream.DeserializeFromJson<TATAPvtCarProposalResponseDto>();
                        if ((pvtCarProposalResponseDto.status.Equals(400) || pvtCarProposalResponseDto.status.Equals(-102)) && !string.IsNullOrEmpty(pvtCarProposalResponseDto.message_txt))
                        {
                            proposalVm.ValidationMessage = pvtCarProposalResponseDto.message_txt;
                        }
                        else
                        {
                            proposalVm.ValidationMessage = ValidationMessage;
                        }
                        responseBody = JsonConvert.SerializeObject(pvtCarProposalResponseDto);
                        _logger.LogInformation("TATA CreateProposal {responseBody}", responseBody);
                        proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        var stream = await proposalResponse.Content.ReadAsStreamAsync(cancellationToken);
                        pvtCarProposalResponseDto = stream.DeserializeFromJson<TATAPvtCarProposalResponseDto>();

                        if (pvtCarProposalResponseDto != null && pvtCarProposalResponseDto.status.Equals(-102) && !string.IsNullOrEmpty(pvtCarProposalResponseDto.message_txt))
                        {
                            proposalVm.ValidationMessage = pvtCarProposalResponseDto.message_txt;
                            responseBody = JsonConvert.SerializeObject(pvtCarProposalResponseDto);
                            _logger.LogInformation("TATA CreateProposal {responseBody}", responseBody);
                            proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                        }
                        else if (pvtCarProposalResponseDto.status.Equals(200) && !string.IsNullOrEmpty(pvtCarProposalResponseDto?.data[0]?.proposal_no))
                        {
                            responseBody = JsonConvert.SerializeObject(pvtCarProposalResponseDto);
                            _logger.LogInformation("TATA CreateProposal {responseBody}", responseBody);

                            proposalVm = new QuoteResponseModel
                            {
                                InsurerName = _tataConfig.InsurerName,
                                InsurerStatusCode = (int)HttpStatusCode.OK,
                                Tax = tax,
                                TotalPremium = createLeadModel?.TotalPremium,
                                GrossPremium = Math.Round(pvtCarProposalResponseDto.data[0].premium_value).ToString(),
                                TransactionID = pvtCarProposalResponseDto.data[0].quote_no,
                                ProposalNumber = pvtCarProposalResponseDto.data[0].proposal_no,
                                PolicyId = pvtCarProposalResponseDto.data[0].policy_id,
                                ApplicationId = pvtCarProposalResponseDto.data[0].payment_id,
                                IsBreakIn = pvtCarProposalResponseDto.data[0].inspectionFlag.Equals("true"),
                                IsSelfInspection = pvtCarProposalResponseDto.data[0].inspectionFlag.Equals("true"),
                                BreakinInspectionURL = pvtCarProposalResponseDto.data[0].self_inspection_link,
                                BreakinId = pvtCarProposalResponseDto.data[0].inspectionFlag.Equals("true") ? pvtCarProposalResponseDto.data[0]?.ticket_number : String.Empty,
                                InspectionId = pvtCarProposalResponseDto.data[0].inspectionFlag.Equals("true") ? pvtCarProposalResponseDto.data[0]?.nstp_id : String.Empty,
                            };
                        }
                    }
                }
                else
                {
                    var proposalQuery = new TATATwoWheelerProposalRequestDto()
                    {
                        proposer_salutation = createLeadModel.CarOwnedBy.Equals("COMPANY") ? "M/s." : proposalRequest.PersonalDetails.salutation,
                        proposer_fname = createLeadModel.CarOwnedBy.Equals("COMPANY") ? proposalRequest.PersonalDetails.companyName : proposalRequest.PersonalDetails.firstName,
                        proposer_mname = string.Empty,
                        proposer_lname = createLeadModel.CarOwnedBy.Equals("COMPANY") ? string.Empty : proposalRequest.PersonalDetails.lastName,
                        proposer_dob = createLeadModel.CarOwnedBy.Equals("COMPANY") ? string.Empty : dob,
                        proposer_gender = createLeadModel.CarOwnedBy.Equals("COMPANY") ? string.Empty : proposalRequest.PersonalDetails.gender,
                        proposer_mobile = proposalRequest.PersonalDetails.mobile,
                        proposer_email = proposalRequest.PersonalDetails.emailId.ToLower(),
                        proposer_marital = createLeadModel.CarOwnedBy.Equals("COMPANY") ? string.Empty : proposalRequest.PersonalDetails.maritalStatus,
                        proposer_occupation = createLeadModel.CarOwnedBy.Equals("COMPANY") ? string.Empty : proposalRequest.PersonalDetails.occupation,
                        proposer_pan = proposalRequest.PersonalDetails.panNumber,

                        proposer_add1 = address[0] ?? string.Empty,
                        proposer_add2 = address[1] ?? string.Empty,
                        proposer_add3 = address[2] ?? string.Empty,
                        proposer_pincode = proposalRequest.AddressDetails.pincode,
                        proposer_state = proposalRequest.AddressDetails.state,
                        proposer_house = string.Empty,
                        proposer_landmark = string.Empty,
                        proposer_village = string.Empty,
                        proposer_gstin = createLeadModel.CarOwnedBy.Equals("COMPANY") ? proposalRequest.PersonalDetails.gstno : string.Empty,

                        nominee_name = proposalRequest.NomineeDetails.nomineeName,
                        nominee_age = Convert.ToInt32(proposalRequest.NomineeDetails.nomineeAge),
                        nominee_relation = proposalRequest.NomineeDetails.nomineeRelation,
                        appointee_name = string.Empty,
                        appointee_relation = string.Empty,

                        vehicle_chassis = proposalRequest.VehicleDetails.chassisNumber,
                        vehicle_engine = proposalRequest.VehicleDetails.engineNumber,
                        vehicle_puc = string.Empty,
                        vehicle_puc_expiry = string.Empty,
                        vehicle_puc_declaration = "false",

                        financier_type = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails.agreementType : string.Empty,
                        financier_name = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails.financer : string.Empty,
                        loan_acc_no = string.Empty,

                        // Below fields need to check from db
                        pre_insurer_name = !string.IsNullOrEmpty(prevInsurerName) ? prevInsurerName : string.Empty,
                        pre_insurer_no = !string.IsNullOrEmpty(prevInsurerNumber) ? prevInsurerNumber : string.Empty,

                        pre_od_insurer_code = odInsurerCode,
                        pre_od_insurer_name = odInsurer,
                        pre_od_policy_no = odPolicyNumber,
                        pre_tp_insurer_code = tpInsurerCode,
                        pre_tp_insurer_name = tpInsurer,
                        pre_tp_pol_no = tpPolicyNumber,

                        bund_od_insurer_name = odInsurer,
                        bund_od_add = string.Empty,
                        bund_od_pol_number = odPolicyNumber,
                        bund_tp_insurer_name = tpInsurer,
                        bund_tp_add = string.Empty,
                        bund_tp_pol_number = tpPolicyNumber,


                        product_id = _tataConfig.TWProductId,
                        proposal_id = quoteDetails?.ProposalId,
                        quote_no = quoteDetails?.TransactionId,
                        carriedOutBy = "",
                        proposalInspectionOverride = true,
                        __finalize = "1"
                    };
                    requestBody = JsonConvert.SerializeObject(proposalQuery);
                    _logger.LogInformation("TATA create proposal {request}", requestBody);

                    var proposalResponse = await GetQuoteResponse(false, true, requestBody, token, createLeadModel.LeadID, "Proposal", cancellationToken);

                    if (!proposalResponse.IsSuccessStatusCode)
                    {
                        var stream = await proposalResponse.Content.ReadAsStreamAsync(cancellationToken);
                        twProposalResponseDto = stream.DeserializeFromJson<TATATwoWheelerProposalResponseDto>();
                        if ((twProposalResponseDto.status.Equals(400) || twProposalResponseDto.status.Equals(-102)) && !string.IsNullOrEmpty(twProposalResponseDto.message_txt))
                        {
                            proposalVm.ValidationMessage = twProposalResponseDto.message_txt;
                        }
                        else
                        {
                            proposalVm.ValidationMessage = ValidationMessage;
                        }
                        responseBody = JsonConvert.SerializeObject(twProposalResponseDto);
                        _logger.LogInformation("TATA CreateProposal {responseBody}", responseBody);
                        proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        var stream = await proposalResponse.Content.ReadAsStreamAsync(cancellationToken);
                        twProposalResponseDto = stream.DeserializeFromJson<TATATwoWheelerProposalResponseDto>();

                        if (twProposalResponseDto != null && twProposalResponseDto.status.Equals(-102) && !string.IsNullOrEmpty(twProposalResponseDto.message_txt))
                        {
                            proposalVm.ValidationMessage = twProposalResponseDto.message_txt;
                            responseBody = JsonConvert.SerializeObject(twProposalResponseDto);
                            _logger.LogInformation("TATA CreateProposal {responseBody}", responseBody);
                            proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                        }
                        else if (twProposalResponseDto.status.Equals(200) && !string.IsNullOrEmpty(twProposalResponseDto?.data[0]?.proposal_no))
                        {
                            responseBody = JsonConvert.SerializeObject(twProposalResponseDto);
                            _logger.LogInformation("TATA CreateProposal {responseBody}", responseBody);

                            proposalVm = new QuoteResponseModel
                            {
                                InsurerName = _tataConfig.InsurerName,
                                InsurerStatusCode = (int)HttpStatusCode.OK,
                                Tax = tax,
                                TotalPremium = createLeadModel?.TotalPremium,
                                GrossPremium = Math.Round(twProposalResponseDto.data[0].premium_value).ToString(),
                                TransactionID = twProposalResponseDto.data[0].quote_no,
                                ProposalNumber = twProposalResponseDto.data[0].proposal_no,
                                PolicyId = twProposalResponseDto.data[0].policy_id,
                                ApplicationId = twProposalResponseDto.data[0].payment_id,
                                IsBreakIn = false,
                                IsSelfInspection = false,
                                PaymentCorrelationId = twProposalResponseDto.data[0].payment_id
                            };
                        }
                    }
                }
            }
            else
            {
                proposalVm = new QuoteResponseModel()
                {
                    InsurerStatusCode = (int)HttpStatusCode.BadRequest,
                    ValidationMessage = AuthAPIErrorMessage
                };
                _logger.LogError("TATA GetQuote Error {exception}", AuthAPIErrorMessage);
            }
            proposalResponseVM = new ProposalResponseModel()
            {
                quoteResponseModel = proposalVm,
                RequestBody = requestBody,
                ResponseBody = responseBody,
            };
            return proposalResponseVM;
        }
        catch (Exception ex)
        {
            _logger.LogError("TATA Proposal Error {exception}", ex.Message);
            proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return proposalResponseVM;
        }
    }
    public async Task<TATACKYCStatusResponseModel> PanCKYCVerification(TATACKYCRequestModel tataCKYCRequestModel, CancellationToken cancellationToken)
    {
        string requestBody = string.Empty;
        string responseBody = string.Empty;
        TATACKYCStatusResponseModel tataCKYCStatusResponseModel = new TATACKYCStatusResponseModel();
        TATAVerifyCKYCResponseDto result = new TATAVerifyCKYCResponseDto();
        SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();

        var token = await GetToken(tataCKYCRequestModel.LeadId, "KYC", cancellationToken);
        try
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                TATAPanVerifyCKYCRequestDto tataVerifyCKYCRequestDto = new()
                {

                    proposal_no = tataCKYCRequestModel.ProposalNumber,
                    id_type = "PAN",
                    id_num = tataCKYCRequestModel.IdNumber,
                    req_id = string.Empty
                };
                requestBody = JsonConvert.SerializeObject(tataVerifyCKYCRequestDto);

                _logger.LogInformation("GetCKYCStatusResponse RequestBody {requestBody}", requestBody);

                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", token);
                _client.DefaultRequestHeaders.Add("x-api-key", _tataConfig.TokenHeader.apiKey);


                var id = await InsertICLogs(requestBody, tataCKYCRequestModel.LeadId, "KYC",_tataConfig.BaseURL+_tataConfig.VerifyCKYCURL, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders));

                var response = await _client.PostAsync(_tataConfig.VerifyCKYCURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                    cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    result = stream.DeserializeFromJson<TATAVerifyCKYCResponseDto>();
                    responseBody = result.message_txt;
                    saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    saveCKYCResponse.KYC_Status = POI_REQUIRED;
                    saveCKYCResponse.Message = MESSAGE;
                    _logger.LogError("TATA GetCKYCStatusResponse error responseBody {responseBody}", responseBody);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    result = stream.DeserializeFromJson<TATAVerifyCKYCResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("TATA GetCKYCStatusResponse responseBody {responseBody}", responseBody);
                    if (result != null && result.status.Equals(200) && result.data.success)//success
                    {
                        saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.OK;
                        saveCKYCResponse.Name = result.data.result.registered_name;
                        saveCKYCResponse.Age = result.data.result.age;
                        saveCKYCResponse.CKYCNumber = result.data.result.ckyc_number;
                        saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                        saveCKYCResponse.PhotoId = result?.data?.req_id;
                    }
                    else if(result != null && result.status.Equals(200)) //kyc failing
                    {
                        saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.OK;
                        saveCKYCResponse.KYC_Status = string.IsNullOrEmpty(result?.data?.req_id) ? POI_REQUIRED : POA_REQUIRED;
                        saveCKYCResponse.PhotoId = result?.data?.req_id;
                        saveCKYCResponse.Message = result?.message_txt;
                    }
                    else if (result != null && result.status.Equals(400))
                    {
                        saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.OK;
                        saveCKYCResponse.KYC_Status = POI_REQUIRED;
                        saveCKYCResponse.Message = result?.message_txt;
                    }
                    else
                    {
                        saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.OK;
                        saveCKYCResponse.KYC_Status = POI_REQUIRED;
                        saveCKYCResponse.Message = MESSAGE;
                    }
                }
                tataCKYCStatusResponseModel.RequestBody = requestBody;
                tataCKYCStatusResponseModel.ResponseBody = responseBody;
                tataCKYCStatusResponseModel.TATACKYCResponse = result;
                tataCKYCStatusResponseModel.SaveCKYCResponse = saveCKYCResponse;

                UpdateICLogs(id, tataCKYCRequestModel.ProposalNumber, responseBody);
                return tataCKYCStatusResponseModel;
            }
            _logger.LogError("TATA KYC Error {exception}", AuthAPIErrorMessage);
            return tataCKYCStatusResponseModel;
        }
        catch (Exception ex)
        {
            _logger.LogError("TATA GetCKYCStatusResponses Error {exception}", ex.Message);
            return tataCKYCStatusResponseModel;
        }
    }
    public async Task<TATACKYCStatusResponseModel> POACKYCVerification(TATACKYCRequestModel tataCKYCRequestModel, CancellationToken cancellationToken)
    {
        string requestBody = string.Empty;
        string responseBody = string.Empty;
        TATACKYCStatusResponseModel tataCKYCStatusResponseModel = new TATACKYCStatusResponseModel();
        TATAVerifyCKYCResponseDto result = new TATAVerifyCKYCResponseDto();
        SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();

        var token = await GetToken(tataCKYCRequestModel.LeadId, "KYC", cancellationToken);
        try
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                if(tataCKYCRequestModel.IsCompany)
                {
                    TATANonPanVerifyCKYCCompanyRequestDto tataVerifyCKYCRequestDto = new()
                    {

                        proposal_no = tataCKYCRequestModel.ProposalNumber,
                        customer_name = tataCKYCRequestModel.Name,
                        full_name = tataCKYCRequestModel.Name,
                        id_type = tataCKYCRequestModel.IDType,
                        id_num = tataCKYCRequestModel.IdNumber,
                        req_id = tataCKYCRequestModel.ReqId
                    };
                    requestBody = JsonConvert.SerializeObject(tataVerifyCKYCRequestDto);
                }
                else
                {
                    TATANonPanVerifyCKYCIndividualRequestDto tataVerifyCKYCRequestDto = new()
                    {

                        proposal_no = tataCKYCRequestModel.ProposalNumber,
                        customer_name = tataCKYCRequestModel.Name,
                        full_name = tataCKYCRequestModel.Name,
                        id_type = tataCKYCRequestModel.IDType,
                        id_num = tataCKYCRequestModel.IdNumber,
                        dob = tataCKYCRequestModel.DateOfBirth,
                        gender = tataCKYCRequestModel.Gender,
                        req_id = tataCKYCRequestModel.ReqId
                    };
                    requestBody = JsonConvert.SerializeObject(tataVerifyCKYCRequestDto);
                }
                

                _logger.LogInformation("GetCKYCStatusResponse RequestBody {requestBody}", requestBody);

                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", token);
                _client.DefaultRequestHeaders.Add("x-api-key", _tataConfig.TokenHeader.apiKey);


                var id = await InsertICLogs(requestBody, tataCKYCRequestModel.LeadId, "KYC", _tataConfig.BaseURL + _tataConfig.VerifyCKYCURL, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders));

                var response = await _client.PostAsync(_tataConfig.VerifyCKYCURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                    cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    result = stream.DeserializeFromJson<TATAVerifyCKYCResponseDto>();
                    responseBody = result.message_txt;
                    saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    saveCKYCResponse.KYC_Status = POA_REQUIRED;
                    saveCKYCResponse.Message = MESSAGE;
                    _logger.LogError("TATA GetCKYCStatusResponse error responseBody {responseBody}", responseBody);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    result = stream.DeserializeFromJson<TATAVerifyCKYCResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("TATA GetCKYCStatusResponse responseBody {responseBody}", responseBody);
                    if (result != null && result.status.Equals(200) && result.data.success)//success
                    {
                        saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.OK;
                        saveCKYCResponse.Name = result.data.result.registered_name;
                        saveCKYCResponse.Age = result.data.result.age;
                        saveCKYCResponse.CKYCNumber = result.data.result.ckyc_number;
                        saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                        saveCKYCResponse.PhotoId = result?.data?.req_id;
                    }
                    else if (result != null && result.status.Equals(200) && result.data.otp_sent)//aadhar otp
                    {
                        saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.OK;
                        saveCKYCResponse.AadharNumber = tataCKYCRequestModel.IdNumber;
                        saveCKYCResponse.ClientId = result?.data.client_id;
                        saveCKYCResponse.KYC_Status = OTP_SENT;
                        saveCKYCResponse.PhotoId = tataCKYCRequestModel.ReqId;
                    }
                    else if (result != null && (result.status.Equals(200) || result.status.Equals(400))) //kyc failing
                    {
                        saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.OK;
                        saveCKYCResponse.KYC_Status = POA_REQUIRED;
                        saveCKYCResponse.PhotoId = string.IsNullOrEmpty(result?.data?.req_id) ? tataCKYCRequestModel.ReqId : result?.data?.req_id;
                        saveCKYCResponse.Message = result?.message_txt;
                        saveCKYCResponse.IsDocumentUpload = !string.IsNullOrEmpty(result?.data?.req_id) && (result.data?.req_id).Contains("ovd");
                    }
                    else
                    {
                        saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                        saveCKYCResponse.KYC_Status = POA_REQUIRED;
                        saveCKYCResponse.Message = MESSAGE;
                    }
                }
                tataCKYCStatusResponseModel.RequestBody = requestBody;
                tataCKYCStatusResponseModel.ResponseBody = responseBody;
                tataCKYCStatusResponseModel.TATACKYCResponse = result;
                tataCKYCStatusResponseModel.SaveCKYCResponse = saveCKYCResponse;

                UpdateICLogs(id, tataCKYCRequestModel.ProposalNumber, responseBody);
                return tataCKYCStatusResponseModel;
            }
            _logger.LogError("TATA KYC Error {exception}", AuthAPIErrorMessage);
            return tataCKYCStatusResponseModel;
        }
        catch (Exception ex)
        {
            _logger.LogError("TATA GetCKYCStatusResponses Error {exception}", ex.Message);
            return tataCKYCStatusResponseModel;
        }
    }
    public async Task<TATACKYCStatusResponseModel> POAAadharOTPSubmit(POAAadharOTPSubmitRequestModel poaAadharOTPRequest, CancellationToken cancellationToken)
    {
        string requestBody = string.Empty;
        string responseBody = string.Empty;
        TATACKYCStatusResponseModel tataCKYCStatusResponseModel = new TATACKYCStatusResponseModel();
        TATASubmitOTPResponseDto result = new TATASubmitOTPResponseDto();
        SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();

        var token = await GetToken(poaAadharOTPRequest.LeadId, "KYC", cancellationToken);
        try
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                POAAadharOTPSubmitRequestDto poaAadharOTPSubmitRequest = new POAAadharOTPSubmitRequestDto
                {
                    proposal_no = poaAadharOTPRequest.ProposalNo,
                    customer_name = poaAadharOTPRequest.CustomerName,
                    id_type = "AADHAAR",
                    id_num = poaAadharOTPRequest.IdNumber,
                    client_id = poaAadharOTPRequest.ClientId,
                    otp = poaAadharOTPRequest.OTP,
                    product = "Motor"
                };

                requestBody = JsonConvert.SerializeObject(poaAadharOTPSubmitRequest);
                _logger.LogInformation("POAAadharOTPSubmitResponse RequestBody {requestBody}", requestBody);
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", token);
                _client.DefaultRequestHeaders.Add("x-api-key", _tataConfig.TokenHeader.apiKey);


                var id = await InsertICLogs(requestBody, poaAadharOTPRequest.LeadId, "KYC", _tataConfig.BaseURL + _tataConfig.TataKycOtpSubmitURL, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders));

                var response = await _client.PostAsync(_tataConfig.TataKycOtpSubmitURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                    cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    result = stream.DeserializeFromJson<TATASubmitOTPResponseDto>();
                    saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    responseBody = result.message_txt;
                    _logger.LogError("TATA GetCKYCStatusResponse error responseBody {responseBody}", responseBody);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    result = stream.DeserializeFromJson<TATASubmitOTPResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("TATA GetCKYCStatusResponse responseBody {responseBody}", responseBody);
                    if (result != null && result.status.Equals(200) && result.data.success)
                    {
                        saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.OK;
                        saveCKYCResponse.Name = result.data.result.registered_name;
                        saveCKYCResponse.Age = result.data.result.age;
                        saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                        saveCKYCResponse.Message = result.message_txt;
                    }
                    else
                    {
                        saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                        saveCKYCResponse.Message = result?.message_txt;
                    }
                }
                tataCKYCStatusResponseModel.RequestBody = requestBody;
                tataCKYCStatusResponseModel.ResponseBody = responseBody;
                tataCKYCStatusResponseModel.SaveCKYCResponse = saveCKYCResponse;

                UpdateICLogs(id, string.Empty, responseBody);
                return tataCKYCStatusResponseModel;
            }
            _logger.LogError("TATA KYC Error {exception}", AuthAPIErrorMessage);
            return tataCKYCStatusResponseModel;
        }
		catch (Exception ex)
		{
			_logger.LogError("TATA GetCKYCStatusResponses Error {exception}", ex.Message);
			return tataCKYCStatusResponseModel;
		}
	}
	public async Task<TATACKYCStatusResponseModel> POADocumentUpdload(POADocumentUploadRequestModel poaDocumentUploadRequest, CancellationToken cancellationToken)
	{
		string requestBody = string.Empty;
		string responseBody = string.Empty;
        TATACKYCStatusResponseModel tATACKYCStatusResponseModel = new();
        TATAPOADocumentUploadResponseDto poaDocumentUploadResponse = new ();
		SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();


        var token = await GetToken(poaDocumentUploadRequest.LeadId, "KYC", cancellationToken);
        try
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                TATAPOADocumentUploadRequestDto docUploadPostProposalRequest = new()
                {
                    req_id = poaDocumentUploadRequest.req_id,
                    proposal_no = poaDocumentUploadRequest.proposal_no,
                    doc_type = poaDocumentUploadRequest.doc_type,
                    id_type = "OVD",
                    doc_base64 = poaDocumentUploadRequest.doc_base64
                };
                requestBody = JsonConvert.SerializeObject(docUploadPostProposalRequest);
                _logger.LogInformation("TATA POADocumentUpdload RequestBody {requestBody}", requestBody);

                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", token);
                _client.DefaultRequestHeaders.Add("x-api-key", _tataConfig.TokenHeader.apiKey);


                var id = await InsertICLogs(requestBody, poaDocumentUploadRequest.LeadId, "KYC", _tataConfig.BaseURL + _tataConfig.TataDocumentUploadURL, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders));

                var response = await _client.PostAsync(_tataConfig.TataDocumentUploadURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                    cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    poaDocumentUploadResponse = stream.DeserializeFromJson<TATAPOADocumentUploadResponseDto>();
                    responseBody = JsonConvert.SerializeObject(poaDocumentUploadResponse);
                    saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    responseBody = poaDocumentUploadResponse.message_txt;
                    _logger.LogError("TATA POADocumentUpdload error responseBody {responseBody}", responseBody);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    poaDocumentUploadResponse = stream.DeserializeFromJson<TATAPOADocumentUploadResponseDto>();
                    responseBody = JsonConvert.SerializeObject(poaDocumentUploadResponse);
                    _logger.LogInformation("TATA POADocumentUpdload responseBody {responseBody}", responseBody);
                    saveCKYCResponse.InsurerStatusCode = (int)HttpStatusCode.OK;

                    if (poaDocumentUploadResponse is not null && poaDocumentUploadResponse.status.Equals(200) && poaDocumentUploadResponse.data.verified)
                    {
                        saveCKYCResponse.Message = poaDocumentUploadResponse.message_txt;
                        saveCKYCResponse.Name = poaDocumentUploadResponse.data.result.registered_name;
                        saveCKYCResponse.Age = poaDocumentUploadResponse.data.result.age;
                        saveCKYCResponse.CKYCNumber = poaDocumentUploadResponse.data.result.gender;
                        saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                    }
                    else
                    {
                        saveCKYCResponse.KYC_Status = KYC_FAILED;
                        saveCKYCResponse.PhotoId = poaDocumentUploadRequest.req_id;
                        saveCKYCResponse.Message = poaDocumentUploadResponse?.message_txt;
                    }
                }
                tATACKYCStatusResponseModel.SaveCKYCResponse = saveCKYCResponse;
                tATACKYCStatusResponseModel.ResponseBody = responseBody;
                tATACKYCStatusResponseModel.RequestBody = requestBody;
                UpdateICLogs(id, saveCKYCResponse.ProposalId, responseBody);
                return tATACKYCStatusResponseModel;
            }
            else
            {
                _logger.LogError("TATA POADocumentUpdload Error {exception}", AuthAPIErrorMessage);
                return tATACKYCStatusResponseModel;
            }
		}
		catch (Exception ex)
		{
			_logger.LogError("TATA POADocumentUpdload Error {exception}", ex.Message);
			return tATACKYCStatusResponseModel;
		}
	}
	public async Task<TATAPaymentResponseDataDto> GetPaymentLink(TATAPaymentRequestModel requestModel, CancellationToken cancellationToken)
    {
        TATAPaymentResponseDataDto paymentResponse = new TATAPaymentResponseDataDto();
        try
        {
            var token = await GetToken(requestModel.LeadId, "Payment", cancellationToken);

            var result = new TATAPaymentRequestModelDto()
            {
                payment_mode = "onlinePayment",
                online_payment_mode = "UPI",
                payer_type = "Customer",
                payer_id = string.Empty,
                deposit_in = "Bank",
                pan_no = string.Empty,
                payer_relationship = "self",
                payer_pan_no = requestModel.PAN,
                mobile_no = requestModel.MobileNo,
                payer_name = string.IsNullOrEmpty(requestModel.Name) ? string.Empty : (requestModel.Name).TrimEnd(),
                payment_id = new string[1] { requestModel.TATAPaymentId },
                email = requestModel.Email.ToLower(),
                returnurl = $"{_tataConfig.PGStatusRedirectionURL}{requestModel.QuoteTransactionId}/{_applicationClaims.GetUserId()}"
            };

            _logger.LogInformation("TATA GetPaymentLink RequestBody {requestBody}", JsonConvert.SerializeObject(result));

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", token);
            _client.DefaultRequestHeaders.Add("x-api-key", _tataConfig.TokenHeader.apiKey);

            string paymentURL = string.Empty;
            if(requestModel.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
            {
                paymentURL = _tataConfig.FWPaymentLinkURL;
            }
            else if (requestModel.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
            {
                paymentURL = _tataConfig.TWPaymentLinkURL;
            }

            var id = await InsertICLogs(JsonConvert.SerializeObject(result), requestModel.LeadId, "Payment", _tataConfig.BaseURL + paymentURL, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders));
            var response = await _client.PostAsync(paymentURL, new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json"),
                        cancellationToken);

            var dataResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            UpdateICLogs(id, requestModel.TATAPaymentId, dataResponse);

            if(!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("TATA GetPaymentLink error");
                return default;
            }
            paymentResponse = JsonConvert.DeserializeObject<TATAPaymentResponseDataDto>(dataResponse);
            return paymentResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError("TATA GetPaymentLink Error {exception}", ex.Message);
            return paymentResponse;
        }
    }
    public async Task<TATAPolicyDocumentResponseModel> GetPolicyDocument(string encriptedPolicyId, string vehicleTypeId, string leadId, CancellationToken cancellationToken)
    {
        var policyResponse = new TATAPolicyDocumentResponseModel();
        var requestBody = string.Empty;
        var id = 0;
        var token = await GetToken(leadId, "Payment", cancellationToken);


        _logger.LogInformation("TATA policy documents {request}", requestBody);
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("Authorization", token);
        _client.DefaultRequestHeaders.Add("x-api-key", _tataConfig.TokenHeader.apiKey);

        string paymentURL = string.Empty;
        if (vehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
        {
            paymentURL = _tataConfig.FWPolicyDocumentURL;
        }
        else if (vehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
        {
            paymentURL = _tataConfig.TWPolicyDocumentURL;
        }

        id = await InsertICLogs(requestBody, leadId, "Payment", $"{_tataConfig.BaseURL}/{paymentURL}/{encriptedPolicyId}", token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders));
        try
        {

            var response = await _client.GetAsync($"{paymentURL}/{encriptedPolicyId}", cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var result = stream.DeserializeFromJson<TATAPolicyDocumentResponseDto>();
                _logger.LogInformation("Policy Document {response}", JsonConvert.SerializeObject(result));
                policyResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var result = stream.DeserializeFromJson<TATAPolicyDocumentResponseDto>();
                _logger.LogInformation("Policy Document {response}", JsonConvert.SerializeObject(result));
                policyResponse.PolicyDocumentBase64 = result?.byteStream;
                policyResponse.InsurerStatusCode = (int)HttpStatusCode.OK;
                UpdateICLogs(id, encriptedPolicyId, JsonConvert.SerializeObject(result));
            }
            return policyResponse;
        }
        catch (Exception ex)
        {
            UpdateICLogs(id, encriptedPolicyId, ex.Message);
            _logger.LogError("TATA Policy Download Error {exception}", ex.Message);
            return default;
        }

    }
    public async Task<TATAVerifyPaymentStatusResponseDto> VerifyPaymentDetails(string vehicleTypeId, string paymentId, string leadId, CancellationToken cancellationToken)
    {
        TATAVerifyPaymentStatusResponseDto paymentResponse = new();
        try
        {
            var token = await GetToken(leadId, "Payment", cancellationToken);

            var result = new TATAVerifyPaymentStatusRequestDto()
            {
                payment_id = paymentId
            };

            _logger.LogInformation("TATA VerifyPaymentDetails RequestBody {requestBody}", JsonConvert.SerializeObject(result));

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", token);
            _client.DefaultRequestHeaders.Add("x-api-key", _tataConfig.TokenHeader.apiKey);

            string paymentURL = string.Empty;
            if (vehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
            {
                paymentURL = _tataConfig.FWVerifyPaymentStatusURL;
            }
            else if (vehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
            {
                paymentURL = _tataConfig.TWVerifyPaymentStatusURL;
            }

            var id = await InsertICLogs(JsonConvert.SerializeObject(result), leadId, "Payment", _tataConfig.BaseURL + paymentURL, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders));

            var responseMessage = await _client.PostAsync(paymentURL, new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json"),
                        cancellationToken);

            var streamResponse = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            paymentResponse = streamResponse.DeserializeFromJson<TATAVerifyPaymentStatusResponseDto>();
            _logger.LogInformation("TATA VerifyPaymentDetails Response {response}", JsonConvert.SerializeObject(paymentResponse));


            UpdateICLogs(id, paymentId, JsonConvert.SerializeObject(paymentResponse));

            return paymentResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError("TATA VerifyPaymentDetails Error {exception}", ex.Message);
            return paymentResponse;
        }
    }
    public async Task<TATABreakInResponseModel> VerifyBreakIn(TATABreakInPaymentRequestModel tataBreakInRequestModel, CancellationToken cancellationToken)
    {
        var BreakInResponse = new TATABreakInResponseModel();
        var id = 0;
        var token = await GetToken(tataBreakInRequestModel.LeadId, "BreakIn", cancellationToken);

        var result = new TATABreakInRequestDto()
        {
            proposal_no = tataBreakInRequestModel.ProposalNo,
            ticket_no = tataBreakInRequestModel.TicketId
        };

        _logger.LogInformation("TATA Break In {request}", JsonConvert.SerializeObject(result));
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("Authorization", token);
        _client.DefaultRequestHeaders.Add("x-api-key", _tataConfig.TokenHeader.apiKey);
        id = await InsertICLogs(JsonConvert.SerializeObject(result), tataBreakInRequestModel.LeadId, "BreakIn", _tataConfig.VerifyBreakInUrl, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders));
        try
        {

            var response = await _client.PostAsync(_tataConfig.VerifyBreakInUrl, new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var ResponseResult = stream.DeserializeFromJson<TATABreakInPesponseDto>();
                _logger.LogInformation("Break In {response}", JsonConvert.SerializeObject(ResponseResult));
                BreakInResponse = new TATABreakInResponseModel()
                {
                    InsurerStatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            else
            {
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var ResponseResult = stream.DeserializeFromJson<TATABreakInPesponseDto>();
                _logger.LogInformation("Break In {response}", JsonConvert.SerializeObject(ResponseResult));
                if (ResponseResult != null)
                {
                    BreakInResponse = new TATABreakInResponseModel()
                    {
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        TATABreakInPesponseDto = ResponseResult
                    };
                }
                UpdateICLogs(id, tataBreakInRequestModel.LeadId, JsonConvert.SerializeObject(ResponseResult));
            }
            return BreakInResponse;
        }
        catch (Exception ex)
        {
            UpdateICLogs(id, tataBreakInRequestModel.LeadId, ex.Message);
            _logger.LogError("TATA Break In Error {exception}", ex.Message);
            return default;
        }
    }
    public static IEnumerable<string> VehicleNumberSplit(string input)
    {
        var words = new List<StringBuilder> { new StringBuilder() };
        for (var i = 0; i < input.Length; i++)
        {
            words[words.Count - 1].Append(input[i]);
            if (i + 1 < input.Length && char.IsLetter(input[i]) != char.IsLetter(input[i + 1]))
            {
                words.Add(new StringBuilder());
            }
        }
        return words.Select(x => x.ToString());
    }
    private static string SetPATenure(bool isPaCover, bool isBrandNew, bool isFourWheeler)
    {
        if (isPaCover)
        {
            if (!isBrandNew)
                return "1";
            else if (isBrandNew && isFourWheeler)
                return "3";
            else if (isBrandNew && !isFourWheeler)
                return "5";
            else
                return default;
        }
        else
            return string.Empty;
    }
    private static bool IsApplicable(object _val)
    {
        string val = Convert.ToString(_val);
        return !(string.IsNullOrEmpty(val) || val == "0" || Convert.ToDecimal(val) == 0);
    }
    private void UpdateICLogs(int id, string applicationId, string data)
    {
        var logsModel = new LogsModel
        {
            Id = id,
            ResponseBody = data,
            ResponseTime = DateTime.Now,
            ApplicationId = applicationId
        };
        _commonService.UpdateLogs(logsModel);
    }
    private async Task<int> InsertICLogs(string requestBody, string leadId, string stage, string api, string token, string header)
    {
        var logsModel = new LogsModel
        {
            InsurerId = _tataConfig.InsurerId,
            RequestBody = requestBody,
            API = api,
            UserId = _applicationClaims.GetUserId(),
            Token = token,
            Headers = header,
            LeadId = leadId,
            Stage = stage
        };

        var id = await _commonService.InsertLogs(logsModel);
        return id;
    }
    private static bool IsYearGreaterThanValue(DateTime registrationDate, DateTime policyStartDate, int yearCheck)
    {
        double year = (policyStartDate - registrationDate).ToYear();
        return year <= yearCheck;
    }
    public async Task<TATAQuoteResponseDuringProposal> GetQuoteToAppendPincode(string vehicleTypeId, string requestBody, string pincode, string leadId, CancellationToken cancellationToken)
    {
        TATAQuoteResponseDuringProposal quoteVm = new();
        string responseBody = string.Empty;
        var id = 0;
        

        try
        {
            string token = await GetToken(leadId, "Proposal", cancellationToken);
            if (token == null)
            {
                quoteVm = new TATAQuoteResponseDuringProposal()
                {
                    InsurerStatusCode = (int)HttpStatusCode.BadRequest,
                    ValidationMessage = ValidationMessage
                };
                return quoteVm;
            }

            HttpResponseMessage responseMessage = new HttpResponseMessage();
            var defaultRequestHeaders = _client.DefaultRequestHeaders;
            defaultRequestHeaders.Clear();
            defaultRequestHeaders.Add("Authorization", token);

            if (vehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
            {
                TATAPvtCarQuoteRequestDto fwQuoteReq = JsonConvert.DeserializeObject<TATAPvtCarQuoteRequestDto>(requestBody);
                fwQuoteReq.proposer_pincode = pincode;

                id = await InsertICLogs(JsonConvert.SerializeObject(fwQuoteReq), leadId, "Proposal", _tataConfig.BaseURL + _tataConfig.FWQuoteURL, token, JsonConvert.SerializeObject(defaultRequestHeaders));
                responseMessage = await _client.PostAsJsonAsync(_tataConfig.FWQuoteURL, fwQuoteReq, cancellationToken: cancellationToken);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    var streamResponse = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                    var result = streamResponse.DeserializeFromJson<TATAPvtCarQuoteResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    quoteVm = new TATAQuoteResponseDuringProposal()
                    {
                        InsurerStatusCode = (int)HttpStatusCode.BadRequest,
                        ValidationMessage = result?.message_txt
                    };
                    _logger.LogError("TATA GetQuoteToAppendPincode Response {responseBody}", responseBody);
                }
                else
                {
                    var streamResponse = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                    var result = streamResponse.DeserializeFromJson<TATAPvtCarQuoteResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("TATA GetQuoteToAppendPincode Response {responseBody}", responseBody);

                    if (result is not null && result.status == 200)
                    {
                        quoteVm = new TATAQuoteResponseDuringProposal()
                        {
                            InsurerStatusCode = (int)HttpStatusCode.OK,
                            TransactionId = result.data[0].data.quote_no,
                            ProposalId = result.data[0].data.proposal_id,
                            TotalPremium = Math.Round(result.data[0].data.premium_break_up.final_premium).ToString(),
                            GrossPremium = Math.Round(result.data[0].data.premium_break_up.net_premium).ToString(),
                            ServiceTax = new ServiceTax
                            {
                                igst = Convert.ToString(result.data[0].data.premium_break_up.igst_prem),
                                cgst = Convert.ToString(result.data[0].data.premium_break_up.cgst_prem),
                                sgst = Convert.ToString(result.data[0].data.premium_break_up.sgst_prem),
                                utgst = Convert.ToString(result.data[0].data.premium_break_up.ugst_prem),
                                totalTax = Math.Round(result.data[0].data.premium_break_up.igst_prem + result.data[0].data.premium_break_up.cgst_prem +
                                        result.data[0].data.premium_break_up.ugst_prem + result.data[0].data.premium_break_up.sgst_prem).ToString()
                            },
                        };
                    }

                }
            }
            else if (vehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
            {
                TATATwoWheelerQuoteRequestDto twQuoteReq = JsonConvert.DeserializeObject<TATATwoWheelerQuoteRequestDto>(requestBody);
                twQuoteReq.proposer_pincode = pincode;

                id = await InsertICLogs(JsonConvert.SerializeObject(twQuoteReq), leadId, "Proposal", _tataConfig.BaseURL + _tataConfig.TWQuoteURL, token, JsonConvert.SerializeObject(defaultRequestHeaders));
                responseMessage = await _client.PostAsJsonAsync(_tataConfig.TWQuoteURL, twQuoteReq, cancellationToken: cancellationToken);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    var streamResponse = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                    var result = streamResponse.DeserializeFromJson<TATATwoWheelerQuoteResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    quoteVm = new TATAQuoteResponseDuringProposal()
                    {
                        InsurerStatusCode = (int)HttpStatusCode.BadRequest,
                        ValidationMessage = result?.message_txt
                    };
                    _logger.LogError("TATA GetQuoteToAppendPincode Response {responseBody}", responseBody);
                }
                else
                {
                    var streamResponse = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                    var result = streamResponse.DeserializeFromJson<TATATwoWheelerQuoteResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("TATA GetQuoteToAppendPincode Response {responseBody}", responseBody);

                    if (result is not null && result.status == 200)
                    {
                        quoteVm = new TATAQuoteResponseDuringProposal()
                        {
                            InsurerStatusCode = (int)HttpStatusCode.OK,
                            TransactionId = result.data.quote_no,
                            ProposalId = result.data.proposal_id,
                            TotalPremium = Math.Round(Convert.ToDouble(result.data.premium_break_up.net_premium)).ToString(),
                            GrossPremium = Math.Round(Convert.ToDouble(result.data.premium_break_up.final_premium)).ToString(),
                            ServiceTax = new ServiceTax
                            {
                                igst = Convert.ToString(result.data.premium_break_up.igst_prem),
                                cgst = Convert.ToString(result.data.premium_break_up.cgst_prem),
                                sgst = Convert.ToString(result.data.premium_break_up.sgst_prem),
                                utgst = Convert.ToString(result.data.premium_break_up.ugst_prem),
                                totalTax = Math.Round(Convert.ToDouble(result.data.premium_break_up.igst_prem) + Convert.ToDouble(result.data.premium_break_up.cgst_prem)
                                        + Convert.ToDouble(result.data.premium_break_up.sgst_prem) + Convert.ToDouble(result.data.premium_break_up.ugst_prem)).ToString()
                            }
                        };
                    }

                }
            }
            UpdateICLogs(id, null, responseBody);
            return quoteVm;
        }
        catch (Exception ex)
        {
            UpdateICLogs(id, null, ex.Message);
            return default;
        }
        
    }
    private static string[] AddressSplit(string address)
    {
        var addressSplit = (dynamic)null;
        if (address.Contains(","))
        {
            addressSplit = address.Split(",");
        }
        else
        {
            addressSplit = address.Split(" ");

        }
        var length = addressSplit.Length;
        string[] addressArray = new string[3];
        int i = 0;
        int j = 0;
        string temp = string.Empty;

        while (i < length)
        {
        Adding:
            string temp1 = temp;
            if (i < length)
            {
                temp = temp + " " + addressSplit[i].TrimStart(' ');
            }
            if (temp.Length <= 55 && i < length)
            {
                i++;
                goto Adding;
            }
            else if (temp.Length > 55 || i <= length)
            {
                addressArray[j] = temp1.TrimStart(' ');
                j++;
                temp = string.Empty;
            }
        }
        return addressArray;
    }
}
