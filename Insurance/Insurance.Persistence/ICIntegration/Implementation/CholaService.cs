using FirebaseAdmin.Messaging;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Features.Chola.Command.CKYC;
using Insurance.Core.Features.Chola.Queries.GetBreakinStatus;
using Insurance.Core.Features.Chola.Queries.GetCKYCStatus;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.ICICI.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using ThirdPartyUtilities.Helpers;
using static Google.Apis.Requests.BatchRequest;
using static System.Net.Mime.MediaTypeNames;

namespace Insurance.Persistence.ICIntegration.Implementation;

public class CholaService : ICholaService
{
    private readonly ILogger<CholaService> _logger;
    private readonly HttpClient _client;
    private readonly CholaConfig _cholaConfig;
    private readonly IApplicationClaims _applicationClaims;
    private readonly PolicyTypeConfig _policyTypeConfig;
    private const string KYC_SUCCESS = "KYC_SUCCESS";
    private const string FAILED = "FAILED";
    private const string POA_REQUIRED = "POA_REQUIRED";
    private const string MESSAGE = "Please enter correct document number or proceed with other insurer";
    private const string CNG_LPG_IDV = "10000";
    private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";
    private readonly ICommonService _commonService;
    public CholaService(ILogger<CholaService> logger,
        HttpClient client,
        IOptions<CholaConfig> option,
        IApplicationClaims applicationClaims,
        ICommonService commonService,
        IOptions<PolicyTypeConfig> policyType,
        IOptions<VehicleTypeConfig> vehicleType)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _cholaConfig = option.Value;
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        _commonService = commonService;
        _policyTypeConfig = policyType.Value;
    }

    public async Task<(string Token, string Expiry)> GetToken(string leadId, string stage)
    {
        var quoteVm = new CholaTokenResponse();
        var id = 0;
        var responseBody = string.Empty;
        try
        {
            var defaultRequestHeaders = _client.DefaultRequestHeaders;
            if (_client.DefaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
            {
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            var dict = new Dictionary<string, string>
            {
                { "grant_type", _cholaConfig.Token.grant_type }
                //{ "username", _cholaConfig.Token.username },
                //{ "password", _cholaConfig.Token.password }
            };

            var items = from kvp in dict
                        select kvp.Key + "=" + kvp.Value;

            var header = "{" + string.Join(",", items) + "}";

            string url = _cholaConfig.TokenURL;
            id = await InsertICLogs(string.Empty, leadId, _cholaConfig.BaseURL + url, string.Empty, header, stage);
            try
            {
                var cholaResponse = await _client.PostAsync(url, new FormUrlEncodedContent(dict));

                if (cholaResponse.IsSuccessStatusCode)
                {
                    string json = await cholaResponse.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<CholaTokenResponse>(json);
                    responseBody = JsonConvert.SerializeObject(result);
                    await UpdateICLogs(id, string.Empty, responseBody);
                    if (result != null && !string.IsNullOrWhiteSpace(result.access_token))
                    {
                        return (result.access_token, result.expires_in);
                    }
                }
                else
                {
                    responseBody = cholaResponse.ReasonPhrase;
                    await UpdateICLogs(id, string.Empty, responseBody);
                }
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("Chola GetToken Error {exception}", ex.Message);
                await UpdateICLogs(id, string.Empty, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("Chola GetToken Error {exception}", ex.Message);
            quoteVm.StatusCode = (int)HttpStatusCode.BadRequest;
            return default;
        }
    }

    public async Task<CholaIDVResponseModel> GetIDV(QuoteQueryModel quoteQueryModel, CancellationToken cancellationToken)
    {
        var quoteVm = new CholaIDVResponseModel();
        var id = 0;
        try
        {
            var idvRequestBody = string.Empty;
            var idvResponseBody = string.Empty;
            var vehicleModelVarient = quoteQueryModel.VehicleDetails.VehicleModel + " / " + quoteQueryModel.VehicleDetails.Fuel + " / " + quoteQueryModel.VehicleDetails.VehicleCubicCapacity;
            var fromRTO = quoteQueryModel.RegistrationRTOCode + "-" + quoteQueryModel.RTOCityName + "(" + quoteQueryModel.RTOStateName + ")";
            var generalDate = quoteQueryModel.IsBrandNewVehicle ? Convert.ToInt32(Convert.ToDateTime(quoteQueryModel.PolicyStartDate).ToOADate()) : Convert.ToInt32(Convert.ToDateTime(quoteQueryModel.RegistrationDate).ToOADate());

            var idvRequest = new CholaIDVRequestModel
            {
                user_code = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "user_code").Select(x => x.ConfigValue).FirstOrDefault(),
                make = quoteQueryModel.VehicleDetails.VehicleMake,
                model_variant = quoteQueryModel.VehicleDetails.VehicleModel,
                vehicle_model_code = quoteQueryModel.VehicleDetails.VehicleModelCode,
                frm_model_variant = vehicleModelVarient,
                rto_location_code = quoteQueryModel.RTOLocationCode,
                frm_rto = fromRTO,
                ex_show_room = quoteQueryModel.ExShowRoomPrice,
                mobile_no = "",
                date_of_reg = generalDate,
                product_id = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "product_id").Select(x => x.ConfigValue).FirstOrDefault(),
                sel_policy_type = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "sel_policy_type").Select(x => x.ConfigValue).FirstOrDefault(),
                no_previous_insurer_chk = !quoteQueryModel.PolicyTypeId.Equals(_policyTypeConfig.SAOD),//quoteQueryModel.IsBrandNewVehicle,
                no_prev_ins = quoteQueryModel.PolicyTypeId.Equals(_policyTypeConfig.SAOD) ? "No" : "Yes",
                IMDShortcode_Dev = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "IMDShortcode_Dev").Select(x => x.ConfigValue).FirstOrDefault(),
            };
            if (quoteQueryModel.PolicyTypeId.Equals(_policyTypeConfig.SAOD))
            {
                idvRequest.tp_rsd = Convert.ToInt32(Convert.ToDateTime(quoteQueryModel?.PreviousPolicyDetails.PreviousPolicyStartDateSATP).ToOADate());
                idvRequest.tp_red = Convert.ToInt32(Convert.ToDateTime(quoteQueryModel?.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP).ToOADate());
                idvRequest.od_rsd = Convert.ToInt32(Convert.ToDateTime(quoteQueryModel?.PreviousPolicyDetails.PreviousPolicyStartDateSAOD).ToOADate());
                idvRequest.od_red = Convert.ToInt32(Convert.ToDateTime(quoteQueryModel?.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD).ToOADate());
            }
            idvRequestBody = JsonConvert.SerializeObject(idvRequest);
            _logger.LogInformation("Chola GetIDV request {idvRequestBody}", idvRequestBody);
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + quoteQueryModel.Token);
            id = await InsertICLogs(idvRequestBody, quoteQueryModel.LeadId, _cholaConfig.BaseURL + _cholaConfig.IdvURL, quoteQueryModel.Token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders),"Quote");
            try
            {
                var response = await _client.PostAsync(_cholaConfig.IdvURL, new StringContent(idvRequestBody, Encoding.UTF8, "application/json"), cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    idvResponseBody = result;
                    quoteVm.StatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogError("Unable to fetch quote {responseBody}", idvResponseBody);
                    await UpdateICLogs(id, string.Empty, idvResponseBody);
                    return quoteVm;
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var result = stream.DeserializeFromJson<CholaIDVResponseModel>();
                    idvResponseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("Chola GetIDV response {idvResponseBody}", idvResponseBody);
                    var idvResponse = new CholaIDVResponseModel
                    {
                        Code = result.Code,
                        Message = result.Message,
                        Status = result.Status,
                        RequestId = result.RequestId,
                        RequestDateTime = result.RequestDateTime,
                        Data = new IDVValues
                        {
                            idv_1 = result.Data.idv_1,
                            idv_2 = result.Data.idv_2,
                            idv_3 = result.Data.idv_3,
                            idv_4 = result.Data.idv_4
                        },
                    };
                    quoteQueryModel.RecommendedIDV = idvResponse.Data.idv_3;
                    quoteQueryModel.MinIDV = idvResponse.Data.idv_1;
                    quoteQueryModel.MaxIDV = idvResponse.Data.idv_4;
                    await UpdateICLogs(id, string.Empty, idvResponseBody);
                    return idvResponse;
                }
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, string.Empty, ex.Message);
                _logger.LogError("Chola GetIDV Error {exception}", ex.Message);
                quoteVm.StatusCode = (int)HttpStatusCode.BadRequest;
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Chola GetIDV Error {exception}", ex.Message);
            quoteVm.StatusCode = (int)HttpStatusCode.BadRequest;
            return default;
        }
    }
    private static bool IsYearGreaterThanValue(DateTime registrationDate, int yearCheck)
    {
        double year = (DateTime.Now - registrationDate).ToYear();
        return year <= yearCheck;
    }
    public async Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQueryModel, CancellationToken cancellationToken)
    {
        var quoteVm = new QuoteResponseModel();
        string requestBody = string.Empty;
        var responseBody = string.Empty;
        bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQueryModel.RegistrationDate), 5);
        try
        {
            quoteVm.InsurerName = _cholaConfig.InsurerName;
            var generalDate = quoteQueryModel.IsBrandNewVehicle ? Convert.ToInt32(Convert.ToDateTime(quoteQueryModel.PolicyStartDate).ToOADate()) : Convert.ToInt32(Convert.ToDateTime(quoteQueryModel.RegistrationDate).ToOADate());

            var vehicleModelVarient = quoteQueryModel.VehicleDetails.VehicleModel + " / " + quoteQueryModel.VehicleDetails.Fuel + " / " + quoteQueryModel.VehicleDetails.VehicleCubicCapacity;
            var fromRTO = quoteQueryModel.RegistrationRTOCode + "-" + quoteQueryModel.RTOCityName + "(" + quoteQueryModel.RTOStateName + ")";

            var policyType = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "sel_policy_type").Select(x => x.ConfigValue).FirstOrDefault();

            var cholaRequest = new CholaRequestDto
            {
                user_code = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "user_code").Select(x => x.ConfigValue).FirstOrDefault(),
                make = quoteQueryModel.VehicleDetails.VehicleMake,
                model_variant = quoteQueryModel.VehicleDetails.VehicleModel,
                vehicle_model_code = quoteQueryModel.VehicleDetails.VehicleModelCode,
                frm_model_variant = vehicleModelVarient,
                rto_location_code = quoteQueryModel.RTOLocationCode,
                frm_rto = fromRTO,
                ex_show_room = quoteQueryModel.ExShowRoomPrice,
                d2cdtd_masterfetched = "",
                usr_make = "",
                usr_model = "",
                usr_variant = "",
                usr_name = "",
                usr_mobile = "",
                first_name = "",
                phone_no = "",
                mobile_no = "",
                email_id = "",
                authorizeChk = true,
                YOR = generalDate,
                date_of_reg = generalDate,
                IMDState = quoteQueryModel.RegistrationStateCode,
                place_of_reg = quoteQueryModel.RTOCityName + "(" + quoteQueryModel.RTOStateName + ")",
                place_of_reg_short_code = quoteQueryModel.RegistrationStateCode,
                val_claim = "",
                claim_year = "",
                B2B_NCB_App = "",
                Lastyrncb_percentage = "",
                PrvyrClaim = quoteQueryModel.PreviousPolicyDetails.IsClaimInLastYear ? "Yes" : "No",
                D2C_NCB_PERCENTAGE = quoteQueryModel.PreviousPolicyDetails.PreviousNoClaimBonus + "%",
                state = quoteQueryModel.RTOStateName,
                fuel_type = quoteQueryModel.VehicleDetails.Fuel,
                cubic_capacity = quoteQueryModel.VehicleDetails.VehicleCubicCapacity,
                PAAddon = quoteQueryModel.PACover.IsUnnamedOWNERDRIVER ? "Yes" : "No",
                pa_cover = quoteQueryModel.PACover.IsUnnamedOWNERDRIVER ? "Yes" : "No",
                unnamed_cover_opted = (quoteQueryModel.PACover.IsUnnamedPassenger || quoteQueryModel.PACover.IsUnnamedPillionRider) ? "Yes" : "No",
                NilDepselected = (isVehicleAgeLessThan5Years && quoteQueryModel.AddOns.IsZeroDebt) ? "Yes" : "No",
                intermediary_code = "",
                broker_code = "",
                partner_name = "",
                save_percentage = "",
                Customertype = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "Customertype").Select(x => x.ConfigValue).FirstOrDefault(),//"Individual",
                title = "",
                fullName = "",
                aadhar = "",
                cus_mobile_no = "",
                email = "",
                customer_dob_input = "",
                cmp_gst_no = "",
                reg_no = "",
                YOM = Convert.ToInt32(Convert.ToDateTime(quoteQueryModel.RegistrationDate).Year),
                engine_no = "",
                vehicle_color = "",
                chassis_no = "",
                prev_insurer_name = "",
                prev_policy_no = "",
                od_prev_insurer_name = "",
                od_prev_policy_no = "",
                city = "",
                pincode = "",
                reg_pincode = "",
                reg_state = "",
                reg_city = "",
                reg_area = "",
                reg_houseno = "",
                reg_street = "",
                reg_toggle = false,
                address = "||",
                communi_pincode = "",
                communi_state = "",
                communi_city = "",
                communi_area = "",
                communi_houseno = "",
                communi_street = "",
                commaddress = "",
                nominee_name = "",
                nominee_relationship = "",
                hypothecated = "No",
                financier_details = "",
                financieraddress = "",
                contract_no = "",
                sel_idv = "",
                idv_input = quoteQueryModel.IDVValue,
                product_id = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "product_id").Select(x => x.ConfigValue).FirstOrDefault(),
                proposal_id = "",
                quote_id = "",
                sel_policy_type = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "sel_policy_type").Select(x => x.ConfigValue).FirstOrDefault(),
                agree_checbox = "",
                city_of_reg = quoteQueryModel.RTOCityName,
                prev_exp_date_comp = !quoteQueryModel.IsBrandNewVehicle ? Convert.ToDateTime(quoteQueryModel.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD).ToOADate().ToString() : null,
                no_previous_insurer_chk = quoteQueryModel.IsBrandNewVehicle,
                no_prev_ins = quoteQueryModel.IsBrandNewVehicle ? "Yes" : "No",
                user_type = "",
                employee_id = "",
                branch_code_sol_id = "",
                cust_mobile = "",
                account_no = "",
                enach_reg = "",
                utm_details = "",
                utm_source = "",
                utm_medium = "",
                utm_campaign = "",
                utm_content = "",
                utm_term = "",
                covid19_addon = "",
                covid19_dcb_addon = "",
                covid19_dcb_benefit = "",
                covid19_lossofjob_addon = "",
                IMDShortcode_Dev = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "IMDShortcode_Dev").Select(x => x.ConfigValue).FirstOrDefault(),
                emp_code = "",
                sol_id = "",
                seo_master_availability = "",
                d2cmodel_master_availability = "",
                b2brto_master_availability = "",
                d2crto_master_availability = "",
                seo_vehicle_type = "",
                seo_policy_type = "",
                seo_preferred_time = "",
                ncb_protect_app = "No",
                vehicle_replacement_advantage_app = "No",
                reinstatement_value_basis = "No",
                daily_cash_allowance = "No",//quoteQueryModel.AddOns.IsDailyAllowance ? "Yes" : "No",
                sel_allowance = "0",//quoteQueryModel.AddOns.IsDailyAllowance ? "Rs. 250 per day allowance" : "0", //250/500/750    
                monthly_installment_cover = "No",
                emi_entered = 0,//1000,
                sel_time_excess = 0, // 5/10/15/30/40/50
                hydrostatic_lock_cover_app = (isVehicleAgeLessThan5Years && quoteQueryModel.AddOns.IsEngineProtectionRequired) ? "Yes" : "No",
                return_to_invoice = (isVehicleAgeLessThan5Years && quoteQueryModel.AddOns.IsInvoiceCoverRequired) ? "Yes" : "No",
                registrationcost = (isVehicleAgeLessThan5Years && quoteQueryModel.AddOns.IsInvoiceCoverRequired) ? 1000 : 0,
                roadtaxpaid = (isVehicleAgeLessThan5Years && quoteQueryModel.AddOns.IsInvoiceCoverRequired) ? 1000 : 0,
                chola_value_added_services = "No",
                Plan_1 = quoteQueryModel.IsBrandNewVehicle ? "YES" : "NO",
                pa_lt_dropdown = (quoteQueryModel.VehicleTPTenure).ToString(),
                tp_rsd = !quoteQueryModel.IsBrandNewVehicle ? Convert.ToDateTime(quoteQueryModel.PreviousPolicyDetails.PreviousPolicyStartDateSATP).ToOADate().ToString() : null,
                tp_red = !quoteQueryModel.IsBrandNewVehicle ? Convert.ToDateTime(quoteQueryModel.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP).ToOADate().ToString() : null,
                od_rsd = !quoteQueryModel.IsBrandNewVehicle ? Convert.ToDateTime(quoteQueryModel.PreviousPolicyDetails.PreviousPolicyStartDateSAOD).ToOADate().ToString() : null,
                od_red = !quoteQueryModel.IsBrandNewVehicle ? Convert.ToDateTime(quoteQueryModel.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD).ToOADate().ToString() : null,
                paid_driver_opted = quoteQueryModel.PACover.IsPaidDriver ? "Yes" : "No",
                pc_cvas_cover = "No",
                no_of_unnamed = quoteQueryModel.PACover.IsUnnamedPassenger || quoteQueryModel.PACover.IsUnnamedPillionRider ? (Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleSeatCapacity) - 1) : 0,
                rsa_cover_app = (isVehicleAgeLessThan5Years && quoteQueryModel.AddOns.IsRoadSideAssistanceRequired) ? "Yes" : "No",
                key_replacement_cover_app = (isVehicleAgeLessThan5Years && quoteQueryModel.AddOns.IsKeyAndLockProtectionRequired) ? "Yes" : "No",
                personal_belonging_cover_app = (isVehicleAgeLessThan5Years && quoteQueryModel.AddOns.IsPersonalBelongingRequired) ? "Yes" : "No",
                consumables_cover_app = (isVehicleAgeLessThan5Years && quoteQueryModel.AddOns.IsConsumableRequired) ? "Yes" : "No",
                cng_lpg_app = "No",
                cng_lpg_value = "",
                externally_fitted_cng_lpg_opted = (quoteQueryModel.VehicleDetails.Fuel != "PETROL" && quoteQueryModel.VehicleDetails.Fuel != "DIESEL" && quoteQueryModel.Accessories.IsCNG) ? "Yes" : "No",
                externally_fitted_cng_lpg_idv = (quoteQueryModel.Accessories.IsCNG && quoteQueryModel.Accessories.CNGValue > 10000) ? quoteQueryModel.Accessories.CNGValue.ToString() : CNG_LPG_IDV,
                non_elec_acc_app = quoteQueryModel.Accessories.IsNonElectrical ? "Yes" : "No",
                non_elec_acc_value_1 = quoteQueryModel.Accessories.IsNonElectrical ? quoteQueryModel.Accessories.ElectricalValue.ToString() : "",
                elec_acc_app = quoteQueryModel.Accessories.IsElectrical ? "Yes" : "No",
                elec_acc_value_1 = quoteQueryModel.Accessories.IsElectrical ? quoteQueryModel.Accessories.ElectricalValue.ToString() : "",
            };
            return await QuoteResponseFraming(cholaRequest, quoteQueryModel, policyType, quoteVm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Chola Error {exception}", ex.Message);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return Tuple.Create(quoteVm, requestBody, responseBody);
        }
    }
    public async Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        string requestBody = string.Empty;
        string responseBody = string.Empty;
        string applicationId = string.Empty;
        var id = 0;
        try
        {
            bool policyTypeSelfInspection = false;
            bool isSelfInspection = false;
            bool isPrevPolicyExpired = false;
            DateTime prevPolicyExpiryDate;
            DateTime currentDateTime = DateTime.Now.Date;
            var vehicleNumber = string.Join("-", VehicleNumberSplit(quoteConfirmCommand.VehicleNumber));
            var quoteResponseVM = new QuoteConfirmResponseModel();
            CholaRequestDto request = JsonConvert.DeserializeObject<CholaRequestDto>(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody);

            var token = await GetToken(quoteTransactionDbModel?.LeadDetail?.LeadID, "QuoteConfirm");

            request.YOR = Convert.ToInt32(Convert.ToDateTime(quoteConfirmCommand.PolicyDates.RegistrationDate).ToOADate());

            if (quoteConfirmCommand.PolicyDates.IsTwoWheeler)
            {
                request.pa_lt_dropdown = quoteConfirmCommand.IsBrandNewVehicle ? "5" : "1";
            }
            else
            {
                request.pa_lt_dropdown = quoteConfirmCommand.IsBrandNewVehicle ? "3" : "1";
            }
            string manufactureYear = (quoteConfirmCommand.PolicyDates.ManufacturingDate).Substring(Math.Max(0, quoteConfirmCommand.PolicyDates.ManufacturingDate.Length - 4));
            request.YOM = Convert.ToInt32(manufactureYear);
            request.PAAddon = !quoteConfirmCommand.IsPACover && !quoteConfirmCommand.Customertype.Equals("COMPANY") ? "Yes" : "No";
            request.pa_cover = !quoteConfirmCommand.IsPACover && !quoteConfirmCommand.Customertype.Equals("COMPANY") ? "Yes" : "No";
            request.Customertype = quoteConfirmCommand.Customertype.Equals("COMPANY") ? "Coporate" : "Individual";
            request.reg_no = vehicleNumber;
            request.engine_no = quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine;
            request.chassis_no = quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis;

            if (!quoteConfirmCommand.IsBrandNewVehicle && quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy)
            {
                request.PrvyrClaim = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? "Yes" : "No";
                request.D2C_NCB_PERCENTAGE = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? "0" : quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue + "%";
                request.prev_exp_date_comp = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToOADate().ToString();
                request.prev_insurer_name = quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSATPInsurerName != null ? quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSATPInsurerName : quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSAODInsurerName;
                request.prev_policy_no = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP != null ?
                    quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP : quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
                if (request.sel_policy_type.Equals("Standalone OD"))
                {
                    request.od_prev_insurer_name = quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSAODInsurerName;
                    request.od_prev_policy_no = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
                }
                request.od_rsd = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyStartDate).ToOADate().ToString();
                request.od_red = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToOADate().ToString();
                request.tp_rsd = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToOADate().ToString();
                request.tp_red = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToOADate().ToString();
            }
            else
            {
                request.prev_insurer_name = "Acko General Insurance Limited";
                request.prev_policy_no = "CHOLA12345";
            }

            requestBody = JsonConvert.SerializeObject(request);
            _logger.LogInformation("Chola ConfirmDetails requestBody {requestBody}", requestBody);

            var responseMessage = await GetQuoteResponse(request.sel_policy_type, requestBody, token.Token, quoteTransactionDbModel.LeadDetail.LeadID, "QuoteConfirm", cancellationToken);
            QuoteConfirmDetailsResponseModel quoteConfirm = new QuoteConfirmDetailsResponseModel();
            string commonResponse = quoteTransactionDbModel.QuoteTransactionRequest.CommonResponse;
            QuoteResponseModel updatedResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(commonResponse);
            id = responseMessage.Item2;
            string transactionId = string.Empty;
            var leadId = quoteTransactionDbModel.LeadDetail.LeadID;

            var stream = await responseMessage.Item1.Content.ReadAsStreamAsync(cancellationToken);
            var confirmQuoteResult = stream.DeserializeFromJson<CholaResponseDto>();
            responseBody = JsonConvert.SerializeObject(confirmQuoteResult);
            _logger.LogInformation("Chola ConfirmDetails Response {Response}", responseBody);

            if (responseMessage.Item1.IsSuccessStatusCode)
            {
                applicationId = confirmQuoteResult.Data.quote_id;
                updatedResponse.GrossPremium = confirmQuoteResult.Data.Total_Premium;
                updatedResponse.QuoteId = confirmQuoteResult.Data.quote_id;
                updatedResponse.ProposalId = confirmQuoteResult.Data.proposal_id;
                updatedResponse.PolicyId = confirmQuoteResult.Data.policy_id;
                transactionId = confirmQuoteResult.Data.quote_id;

                if (!quoteConfirmCommand.IsBrandNewVehicle && !quoteConfirmCommand.PolicyDates.IsTwoWheeler && !request.sel_policy_type.Equals("Liability"))
                {
                    if (quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId != null && quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId.Equals("2AA7FDCA-9E36-4A8D-9583-15ADA737574B"))
                    {
                        prevPolicyExpiryDate = Convert.ToDateTime(quoteConfirmCommand.PreviousPolicy.TPPolicyExpiryDate);
                    }
                    else
                    {
                        prevPolicyExpiryDate = Convert.ToDateTime(quoteConfirmCommand.PreviousPolicy.SAODPolicyExpiryDate);
                    }
                    if (DateTime.Compare(prevPolicyExpiryDate, currentDateTime) < 0 && DateTime.Compare(prevPolicyExpiryDate, currentDateTime) != 0)
                    {
                        isPrevPolicyExpired = true;
                    }

                    if (quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId != null &&
                        quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId.Equals("2AA7FDCA-9E36-4A8D-9583-15ADA737574B") &&
                        quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals("517D8F9C-F532-4D45-8034-ABECE46693E3"))
                    {
                        policyTypeSelfInspection = true;
                    }
                    if (policyTypeSelfInspection || isPrevPolicyExpired)
                    {
                        isSelfInspection = true;
                    }
                }

                quoteConfirm = new QuoteConfirmDetailsResponseModel()
                {
                    InsurerStatusCode = (int)HttpStatusCode.OK,
                    InsurerName = _cholaConfig.InsurerName,
                    NewPremium = confirmQuoteResult.Data.Total_Premium,
                    InsurerId = _cholaConfig.InsurerId,
                    NCB = confirmQuoteResult.Data?.NCB_percentage?.Replace("%", string.Empty),
                    Tax = new ServiceTaxModel
                    {
                        totalTax = confirmQuoteResult.Data.GST
                    },
                    TotalPremium = confirmQuoteResult.Data.Net_Premium,
                    GrossPremium = confirmQuoteResult.Data.Total_Premium,
                    IsBreakin = isSelfInspection,
                    IsSelfInspection = isSelfInspection,

                };
            }
            else
            {
                if (responseMessage.Item1.StatusCode == HttpStatusCode.ExpectationFailed || responseMessage.Item1.StatusCode == HttpStatusCode.BadRequest)
                {
                    quoteConfirm.InsurerStatusCode = responseMessage.Item1.StatusCode == HttpStatusCode.ExpectationFailed ? (int)HttpStatusCode.ExpectationFailed : (int)HttpStatusCode.BadRequest;
                    quoteConfirm.ValidationMessage = confirmQuoteResult.Message;
                }
                else
                {
                    responseBody = responseMessage.Item1.ReasonPhrase;
                    quoteConfirm.InsurerStatusCode = (int)responseMessage.Item1.StatusCode;
                    quoteConfirm.ValidationMessage = responseMessage.Item1.ReasonPhrase;
                }
                _logger.LogInformation("Chola ConfirmDetails Exception {Exception}", responseBody);
            }

            quoteResponseVM = new QuoteConfirmResponseModel()
            {
                quoteConfirmResponse = quoteConfirm,
                quoteResponse = updatedResponse,
                RequestBody = requestBody,
                ResponseBody = responseBody,
                LeadId = leadId,
                TransactionId = transactionId
            };
            await UpdateICLogs(responseMessage.Item2, applicationId, responseBody);
            return quoteResponseVM;
        }
        catch (Exception ex)
        {
            _logger.LogError("QuoteConfirmDetails Error {exception}", ex.Message);
            await UpdateICLogs(id, applicationId, ex.Message);
            return default;
        }
    }
    public async Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(CholaCKYCCommand cholaCKYCCommand, CancellationToken cancellationToken)
    {
        string responseBody = string.Empty;
        string requestBody = string.Empty;
        SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();
        CreateLeadModel createLeadModel = new CreateLeadModel();
        createLeadModel.PermanentAddress = new LeadAddressModel();
        var id = 0;
        try
        {
            var date = cholaCKYCCommand.CustomerType.Equals("I", StringComparison.OrdinalIgnoreCase) ?
                DateTime.Parse(cholaCKYCCommand.DateOfBirth, new System.Globalization.CultureInfo("pt-BR")).ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture)  
                : DateTime.Parse(cholaCKYCCommand.DateOfInsertion, new System.Globalization.CultureInfo("pt-BR")).ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture);
                
            string token = await GetCKYCToken(cholaCKYCCommand.LeadId, cancellationToken);

            CholaCKYCRequestModel request = new CholaCKYCRequestModel()
            {
                Verify_Type = "VERIFY",
                App_Ref_No = cholaCKYCCommand.TransactionId,
                Customer_Type = cholaCKYCCommand.CustomerType.Equals("I", StringComparison.OrdinalIgnoreCase) ? "I" : "C",
                Customer_Name = cholaCKYCCommand.FullName.ToUpper(),
                Gender = cholaCKYCCommand.Gender,
                DOB_DOI = date,
                Mobile_No = cholaCKYCCommand.Mobile,
                CIN = cholaCKYCCommand.DocumentType.ToUpper() != "CIN" ? string.Empty : cholaCKYCCommand.CIN,
                Redirection_URL = _cholaConfig.PGCKYCURL + cholaCKYCCommand.QuoteTransactionId + "/" + _applicationClaims.GetUserId()
            };

            switch (cholaCKYCCommand.DocumentType.ToUpper())
            {
                case ("AADHAAR"):
                    request.Aadhar_No = cholaCKYCCommand.DocumentId;

                    break;
                case ("PAN"):
                    request.PAN_No = cholaCKYCCommand.DocumentId;
                    break;
                case ("CKYC"):
                    request.CKYC_No = cholaCKYCCommand.DocumentId;
                    break;
                case ("DL"):
                    request.DL_No = cholaCKYCCommand.DocumentId;
                    break;
                case ("PASPORTNO"):
                    request.Passport_no = cholaCKYCCommand.DocumentId;
                    break;
                case ("VOTERID"):
                    request.Voter_ID = cholaCKYCCommand.DocumentId;
                    break;
                case ("CIN"):
                    request.CIN = cholaCKYCCommand.CIN;
                    break;
            }

            requestBody = JsonConvert.SerializeObject(request);
            _logger.LogInformation("GetCKYCResponse requestBody {requestBody}", requestBody);
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("TokenKey", token);
            id = await InsertICLogs(requestBody, cholaCKYCCommand.LeadId, _cholaConfig.VerifyCKYCURL, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders),"KYC");
            try
            {
                var response = await _client.PostAsync(_cholaConfig.VerifyCKYCURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    responseBody = response.ReasonPhrase;
                    _logger.LogError("GetCKYCResponse error {responseBody}", responseBody);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<CholaCKYCResponseModel>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("GetCKYCResponse responseBody {responseBody}", responseBody);
                    saveCKYCResponse.IsKYCRequired = false;
                    if (result != null && result.Status.Equals("Success") && result.eKYC_Redirection_URL == null)
                    {
                        createLeadModel.LeadName = result.Customer_Name;
                        if (cholaCKYCCommand.CustomerType.Equals("I"))
                        {
                            createLeadModel.DOB = Convert.ToDateTime(result.DOB_DOI).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            createLeadModel.DateOfIncorporation = Convert.ToDateTime(result.DOB_DOI).ToString("yyyy-MM-dd");
                        }
                        createLeadModel.Gender = result.Gender;
                        createLeadModel.PhoneNumber = result.Mobile_No;
                        createLeadModel.PANNumber = result.PAN_No;
                        createLeadModel.AadharNumber = result.Aadhar_No;
                        createLeadModel.ckycNumber = result.CKYC_No;
                        createLeadModel.DrivingLicenceNumber = result.DL_No;
                        createLeadModel.VoterId = result.Voter_ID;
                        createLeadModel.PassportNo = result.Passport_no;
                        createLeadModel.CIN = result.CIN;
                        createLeadModel.kyc_id = result.Transaction_ID;
                        createLeadModel.CKYCstatus = result.Status.Trim();
                        createLeadModel.PermanentAddress = new LeadAddressModel()
                        {
                            AddressType = "PRIMARY",
                            Address1 = result.Address1,
                            Address2 = result.Address2,
                            Address3 = result.Address3 + " " + result.City + " " + result.District + " " + result.State,
                            Pincode = result.Pincode,
                        };
                        createLeadModel.CommunicationAddress = new LeadAddressModel()
                        {
                            AddressType = "SECONDARY",
                            Address1 = result.CorresAddress1,
                            Address2 = result.CorresAddress2,
                            Address3 = result.CorresAddress3 + " " + result.CorresCity + " " + result.CorresDistrict + " " + result.CorresState,
                            Pincode = result.CorresPincode,
                        };
                        string address = string.Empty;
                        saveCKYCResponse.Name = result.CKYC_Verified_Cust_Name;
                        saveCKYCResponse.Gender = result.Gender == "M" ? "Male" : "Female";
                        saveCKYCResponse.DOB = result.CKYC_DOB_DOI != null || result.CKYC_DOB_DOI != "" ? Convert.ToDateTime(result.DOB_DOI).ToString("yyyy-MM-dd") : null;
                        saveCKYCResponse.CKYCNumber = result.CKYC_No;
                        saveCKYCResponse.IsKYCRequired = true;
                        saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                        saveCKYCResponse.Message = KYC_SUCCESS;
                        saveCKYCResponse.InsurerName = _cholaConfig.InsurerName;
                        address = string.IsNullOrEmpty(result.Address1) ? string.Empty : result.Address1 + " ";
                        address += string.IsNullOrEmpty(result.Address2) ? string.Empty : result.Address2 + " ";
                        address += string.IsNullOrEmpty(result.Address3) ? string.Empty : result.Address3 + " ";
                        address += $"{result.City} {result.District} {result.State} {result.Pincode}";
                        saveCKYCResponse.Address = address.Trim();
                        await UpdateICLogs(id, cholaCKYCCommand?.TransactionId, responseBody);
                        return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
                    }
                    else if (result != null && result.Status.Equals("Failure") && result.eKYC_Redirection_URL != null)
                    {
                        createLeadModel.LeadName = result.Customer_Name;
                        if (cholaCKYCCommand.CustomerType.Equals("I"))
                        {
                            createLeadModel.DOB = Convert.ToDateTime(result.DOB_DOI).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            createLeadModel.DateOfIncorporation = Convert.ToDateTime(result.DOB_DOI).ToString("yyyy-MM-dd");
                        }
                        createLeadModel.Gender = result.Gender;
                        createLeadModel.PANNumber = result.PAN_No;
                        createLeadModel.AadharNumber = result.Aadhar_No;
                        createLeadModel.ckycNumber = result.CKYC_No;
                        createLeadModel.DrivingLicenceNumber = result.DL_No;
                        createLeadModel.VoterId = result.Voter_ID;
                        createLeadModel.PassportNo = result.Passport_no;
                        createLeadModel.CIN = result.CIN;
                        createLeadModel.kyc_id = result.Transaction_ID;
                        createLeadModel.CKYCstatus = result.Status.Trim();

                        saveCKYCResponse.KYC_Status = POA_REQUIRED;
                        saveCKYCResponse.redirect_link = result.eKYC_Redirection_URL;
                        saveCKYCResponse.Message = result.ErrorMsg.ToString();
                        saveCKYCResponse.InsurerName = _cholaConfig.InsurerName;
                        await UpdateICLogs(id, cholaCKYCCommand?.TransactionId, responseBody);
                        return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
                    }
                    saveCKYCResponse.KYC_Status = FAILED;
                    saveCKYCResponse.Message = result?.ErrorMsg.ToString();
                    saveCKYCResponse.InsurerName = _cholaConfig.InsurerName;
                }
                await UpdateICLogs(id, cholaCKYCCommand?.TransactionId, responseBody);
                return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
            }
            catch (Exception ex)
            {
                saveCKYCResponse.KYC_Status = FAILED;
                saveCKYCResponse.Message = MESSAGE;
                _logger.LogError("Chola Ckyc Error {exception}", ex.Message);
                await UpdateICLogs(id, cholaCKYCCommand?.TransactionId, ex.Message);
                return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
            }
        }
        catch (Exception ex)
        {
            saveCKYCResponse.KYC_Status = FAILED;
            saveCKYCResponse.Message = MESSAGE;
            _logger.LogError("Chola Ckyc Error {exception}", ex.Message);
            return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
        }
    }
    public async Task<CholaCKYCStatusReponseModel> GetCKYCStatusResponse(GetCholaCKYCStatusQuery cholaCKYCStatusQuery, CancellationToken cancellationToken)
    {
        var requestBody = string.Empty;
        var responseBody = string.Empty;
        CholaCKYCStatusReponseModel cholaCKYCStatusReponseModel = new CholaCKYCStatusReponseModel();
        CreateLeadModel createLeadModel = new CreateLeadModel();
        CholaCKYCResponseModel result = new CholaCKYCResponseModel();
        CholaCKYCStatusRequestModel request;
        var id = 0;
        try
        {
            string token = await GetCKYCToken(cholaCKYCStatusQuery.LeadId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request = new CholaCKYCStatusRequestModel()
                {
                    Transaction_ID = cholaCKYCStatusQuery.TransactionID,
                    App_Ref_No = cholaCKYCStatusQuery.AppRefNo
                };

                requestBody = JsonConvert.SerializeObject(request);
                _logger.LogInformation("GetCKYCStatusResponse RequestBody {requestBody}", requestBody);
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("TokenKey", token);
                id = await InsertICLogs(requestBody, cholaCKYCStatusQuery.LeadId, _cholaConfig.VerifyCKYCSTATUSURL, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders),"KYC");
                try
                {
                    var response = await _client.PostAsync(_cholaConfig.VerifyCKYCSTATUSURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                        cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        responseBody = response.ReasonPhrase;
                        _logger.LogError("GetCKYCStatusResponse error responseBody {responseBody}", responseBody);
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                        result = stream.DeserializeFromJson<CholaCKYCResponseModel>();
                        responseBody = JsonConvert.SerializeObject(result);
                        _logger.LogInformation("GetCKYCStatusResponse responseBody {responseBody}", responseBody);
                        if (result != null && result.Status.Trim().Equals("Kyc Verified"))
                        {
                            createLeadModel.LeadName = result.Customer_Name;
                            if (result.Customer_Type.Equals("I"))
                            {
                                createLeadModel.DOB = Convert.ToDateTime(result.DOB_DOI).ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                createLeadModel.DateOfIncorporation = Convert.ToDateTime(result.DOB_DOI).ToString("yyyy-MM-dd");
                            }
                            createLeadModel.Gender = result.Gender;
                            createLeadModel.PhoneNumber = result.Mobile_No;
                            createLeadModel.PANNumber = result.PAN_No;
                            createLeadModel.AadharNumber = result.Aadhar_No;
                            createLeadModel.ckycNumber = result.CKYC_No;
                            createLeadModel.DrivingLicenceNumber = result.DL_No;
                            createLeadModel.VoterId = result.Voter_ID;
                            createLeadModel.PassportNo = result.Passport_no;
                            createLeadModel.CIN = result.CIN;
                            createLeadModel.kyc_id = result.Transaction_ID;
                            createLeadModel.CKYCstatus = "approved";
                            createLeadModel.PermanentAddress = new LeadAddressModel()
                            {
                                AddressType = "PRIMARY",
                                Address1 = result.Address1,
                                Address2 = result.Address2,
                                Address3 = (result.Address3 + " " + result.City + " " + result.District + " " + result.State).Trim(),
                                Pincode = result.Pincode,
                            };
                            createLeadModel.CommunicationAddress = new LeadAddressModel()
                            {
                                AddressType = "SECONDARY",
                                Address1 = result.CorresAddress1,
                                Address2 = result.CorresAddress2,
                                Address3 = result.CorresAddress3 + " " + result.CorresCity + " " + result.CorresDistrict + " " + result.CorresState,
                                Pincode = result.CorresPincode,
                            };
                        }
                        else if (result != null && result.Status.Trim().Equals("Failure"))
                        {
                            createLeadModel.LeadName = result.Customer_Name;
                            if (result.Customer_Type.Equals("I"))
                            {
                                createLeadModel.DOB = Convert.ToDateTime(result.DOB_DOI).ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                createLeadModel.DateOfIncorporation = Convert.ToDateTime(result.DOB_DOI).ToString("yyyy-MM-dd");
                            }
                            createLeadModel.Gender = !string.IsNullOrEmpty(result.Gender) ? result.Gender.Equals("M") ? "MALE" : "FEMALE" : string.Empty;
                            createLeadModel.PhoneNumber = result.Mobile_No;
                            createLeadModel.PANNumber = result.PAN_No;
                            createLeadModel.AadharNumber = result.Aadhar_No;
                            createLeadModel.ckycNumber = result.CKYC_No;
                            createLeadModel.DrivingLicenceNumber = result.DL_No;
                            createLeadModel.VoterId = result.Voter_ID;
                            createLeadModel.PassportNo = result.Passport_no;
                            createLeadModel.CIN = result.CIN;
                            createLeadModel.kyc_id = result.Transaction_ID;
                            createLeadModel.CKYCstatus = "Rejected";
                            createLeadModel.PermanentAddress = new LeadAddressModel()
                            {
                                AddressType = "PRIMARY",
                                Address1 = result.Address1,
                                Address2 = result.Address2,
                                Address3 = result.Address3 + " " + result.City + " " + result.District + " " + result.State,
                                Pincode = result.Pincode,
                            };
                            createLeadModel.CommunicationAddress = new LeadAddressModel()
                            {
                                AddressType = "SECONDARY",
                                Address1 = result.CorresAddress1,
                                Address2 = result.CorresAddress2,
                                Address3 = (result.CorresAddress3 + " " + result.CorresCity + " " + result.CorresDistrict + " " + result.CorresState).Trim(),
                                Pincode = result.CorresPincode,
                            };
                        }
                        else
                        {
                            createLeadModel.LeadName = result?.Customer_Name;
                            if (result.Customer_Type.Equals("I"))
                            {
                                createLeadModel.DOB = Convert.ToDateTime(result.DOB_DOI).ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                createLeadModel.DateOfIncorporation = Convert.ToDateTime(result.DOB_DOI).ToString("yyyy-MM-dd");
                            }
                            createLeadModel.Gender = result.Gender;
                            createLeadModel.PANNumber = result.PAN_No;
                            createLeadModel.CKYCstatus = "Pending";
                        }
                    }
                    cholaCKYCStatusReponseModel.RequestBody = requestBody;
                    cholaCKYCStatusReponseModel.ResponseBody = responseBody;
                    cholaCKYCStatusReponseModel.CholaCKYCResponse = result;
                    cholaCKYCStatusReponseModel.CreateLeadModel = createLeadModel;
                    await UpdateICLogs(id, cholaCKYCStatusQuery?.AppRefNo, responseBody);
                    return cholaCKYCStatusReponseModel;
                }
                catch (Exception ex)
                {
                    _logger.LogError("GetCKYCStatusResponses Error {exception}", ex.Message);
                    await UpdateICLogs(id, cholaCKYCStatusQuery?.AppRefNo, ex.Message);
                    return cholaCKYCStatusReponseModel;
                }
            }
            return cholaCKYCStatusReponseModel;
        }
        catch (Exception ex)
        {
            _logger.LogError("GetCKYCStatusResponses Error {exception}", ex.Message);
            return cholaCKYCStatusReponseModel;
        }
    }
    private async Task<string> GetCKYCToken(string leadId, CancellationToken cancellationToken)
    {
        var responseBody = string.Empty;
        var id = 0;
        try
        {
            CholaCKYCTokenRequestModel request = new CholaCKYCTokenRequestModel()
            {
                PrivateKey = _cholaConfig.PrivateKey,
                UserID = string.Empty
            };

            var requestBody = JsonConvert.SerializeObject(request);
            _logger.LogError("Get CKYC Token requestBody {requestBody}", requestBody);
            _client.DefaultRequestHeaders.Clear();
            id = await InsertICLogs(requestBody, leadId, _cholaConfig.CKYCTokenURL, string.Empty, string.Empty,"KYC");
            try
            {
                var response = await _client.PostAsync(_cholaConfig.CKYCTokenURL, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    responseBody = response.ReasonPhrase;
                    _logger.LogError("Unable get CKYC Token {responseBody}", responseBody);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<CholaCKYCTokenResponseModel>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogError("Get CKYC Token responseBody {responseBody}", responseBody);
                    await UpdateICLogs(id, string.Empty, responseBody);
                    if (result != null && result.ErrorMSG.Equals("Success"))
                    {
                        return result.TokenKey;
                    }
                }
                await UpdateICLogs(id, string.Empty, responseBody);
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, string.Empty, ex.Message);
                _logger.LogError("Chola GetCKYCToken Error {exception}", ex.Message);
                return default;
            }
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError("Chola CKYC Token Generation Error {exception}", ex.Message);
            return default;
        }
    }
    private async Task<Tuple<HttpResponseMessage, int>> GetQuoteResponse(string policyType, string requestBody, string token, string leadId,string stage, CancellationToken cancellationToken)
    {
        var id = 0;
        var response = new HttpResponseMessage();

        try
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            string url = string.Empty;
            if (policyType.Equals("Long Term") || policyType.Equals("Comprehensive"))
            {
                url = _cholaConfig.QuoteURLComprehensive;
            }
            else if (policyType.Equals("Standalone OD"))
            {
                url = _cholaConfig.QuoteURLSAOD;
            }
            else if (policyType.Equals("Liability"))
            {
                url = _cholaConfig.QuoteURLTP;
            }
            id = await InsertICLogs(requestBody, leadId, _cholaConfig.BaseURL + url, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders),stage);
            try
            {
                response = await _client.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken);
                return Tuple.Create(response, id);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetQuoteResponse Error {exception}", ex.Message);
                await UpdateICLogs(id, string.Empty, ex.Message);
                return Tuple.Create(response, id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("GetQuoteResponse Chola Error {exception}", ex.Message);
            return Tuple.Create(response, id);
        }
    }
    private async Task<Tuple<QuoteResponseModel, string, string>> QuoteResponseFraming(CholaRequestDto cholaRequest, QuoteQueryModel quoteQuery, string policyType, QuoteResponseModel quoteVm, CancellationToken cancellationToken)
    {
        string requestBody = JsonConvert.SerializeObject(cholaRequest);

        var response = await GetQuoteResponse(policyType, requestBody, quoteQuery.Token, quoteQuery.LeadId, "Quote", cancellationToken);
        var responseBody = string.Empty;
        var applicationId = string.Empty;
        try
        {
            if (!response.Item1.IsSuccessStatusCode)
            {
                var stream = await response.Item1.Content.ReadAsStreamAsync(cancellationToken);
                var result = stream.DeserializeFromJson<CholaResponseDto>();
                responseBody = JsonConvert.SerializeObject(result);
                quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogError("Unable to fetch quote {responseBody}", responseBody);
            }
            else
            {
                var stream = await response.Item1.Content.ReadAsStreamAsync(cancellationToken);
                var result = stream.DeserializeFromJson<CholaResponseDto>();

                if (result != null && result.Message.Equals("Success"))
                {
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation(responseBody);
                    List<NameValueModel> paCoverList = SetPACoverResponse(quoteQuery, result);
                    List<NameValueModel> addOnsList = SetAddOnsResponse(quoteQuery, result);
                    List<NameValueModel> accessoryList = SetAccessoryResponse(quoteQuery, result);
                    List<NameValueModel> discountList = SetDiscountResponse(quoteQuery, result);
                    applicationId = result.Data.quote_id;
                    var tax = new ServiceTax
                    {
                        totalTax = result.Data.GST,
                    };
                    quoteVm = new QuoteResponseModel
                    {
                        InsurerName = "Cholamandalam",
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        InsurerId = _cholaConfig.InsurerId,
                        SelectedIDV = quoteQuery.SelectedIDV,
                        IDV = quoteQuery.RecommendedIDV,
                        MinIDV = quoteQuery.MinIDV,
                        MaxIDV = quoteQuery.MaxIDV,
                        Tax = tax,

                        BasicCover = new BasicCover
                        {
                            CoverList = SetBaseCover(quoteQuery.CurrentPolicyType, result)
                        },
                        PACovers = new PACovers
                        {
                            PACoverList = paCoverList,
                        },
                        AddonCover = new AddonCover
                        {
                            AddonList = addOnsList,
                        },
                        Discount = new Domain.GoDigit.Discount
                        {
                            DiscountList = discountList,
                        },
                        AccessoriesCover = new AccessoriesCover
                        {
                            AccessoryList = accessoryList,
                        },
                        TotalPremium = result.Data.Net_Premium,
                        GrossPremium = result.Data.Total_Premium,
                        NCB = Convert.ToString(result.Data?.NCB_percentage).Replace("%", string.Empty),
                        PolicyStartDate = Convert.ToDateTime(quoteQuery.PolicyStartDate).ToString("dd-MMM-yyyy"),
                        Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                        PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
                        IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                        IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                        RegistrationDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                        ManufacturingDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                        VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ?
                        quoteQuery.RegistrationRTOCode : quoteQuery.VehicleNumber
                    };
                }
                else
                {
                    quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("QuoteResponseFraming Error {exception}", ex.Message);
            await UpdateICLogs(response.Item2, applicationId, ex.Message);
            return Tuple.Create(quoteVm, requestBody, responseBody);
        }
        await UpdateICLogs(response.Item2, applicationId, responseBody);
        return Tuple.Create(quoteVm, requestBody, responseBody);
    }
    private static List<NameValueModel> SetDiscountResponse(QuoteQueryModel quoteQuery, CholaResponseDto result)
    {
        string ncbPercentage = Convert.ToString(result.Data?.NCB_percentage);

        List<NameValueModel> discountList = new List<NameValueModel>();
        if (quoteQuery.Discounts.IsAntiTheft)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AntiTheftId,
                Name = "ARAI Approved Anti-Theft Device",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.Discounts.IsAAMemberShip)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AAMemberShipId,
                Name = "AA Membership",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.Discounts.IsVoluntarilyDeductible)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                Name = "Voluntary Deductible",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.Discounts.IsLimitedTPCoverage)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.LimitedTPCoverageId,
                Name = "Limited Third Party Coverage",
                Value = null,
                IsApplicable = false,
            });
        }
        discountList.Add(new NameValueModel
        {
            Name = $"No Claim Bonus ({ncbPercentage})",
            Value = result.Data.No_Claim_Bonus.ToString(),
            IsApplicable = IsApplicable(result.Data.No_Claim_Bonus),
        });


        return discountList;
    }
    private static List<NameValueModel> SetAccessoryResponse(QuoteQueryModel quoteQuery, CholaResponseDto result)
    {
        List<NameValueModel> accessoryList = new List<NameValueModel>();
        if (quoteQuery.Accessories.IsCNG)
        {
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover OD",
                Value = Convert.ToString(result.Data.CNG_LPG_Own_Damage),
                IsApplicable = IsApplicable(result.Data.CNG_LPG_Own_Damage)
            });
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover TP",
                Value = Convert.ToString(result.Data.CNG_LPG_TP),
                IsApplicable = IsApplicable(result.Data.CNG_LPG_TP)
            });
        }
        else if (!quoteQuery.Accessories.IsCNG && (result.Data.CNG_LPG_Own_Damage > 0 || result.Data.CNG_LPG_TP > 0))
        {
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Inbuilt Accessory Cover OD",
                Value = Convert.ToString(result.Data.CNG_LPG_Own_Damage),
                IsApplicable = IsApplicable(result.Data.CNG_LPG_Own_Damage)
            });
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Inbuilt Accessory Cover TP",
                Value = Convert.ToString(result.Data.CNG_LPG_TP),
                IsApplicable = IsApplicable(result.Data.CNG_LPG_TP)
            });
        }
        if (quoteQuery.Accessories.IsElectrical)
        {
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.ElectricalId,
                Name = "Electrical Accessory Cover",
                Value = Convert.ToString(result.Data.Electrical_Accessory_Prem),
                IsApplicable = IsApplicable(result.Data.Electrical_Accessory_Prem)
            });
        }
        if (quoteQuery.Accessories.IsNonElectrical)
        {
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.NonElectricalId,
                Name = "Non-Electrical Accessory Cover",
                Value = Convert.ToString(result.Data.Non_Electrical_Accessory_Prem),
                IsApplicable = IsApplicable(result.Data.Non_Electrical_Accessory_Prem)
            });
        }

        return accessoryList;
    }
    private static List<NameValueModel> SetAddOnsResponse(QuoteQueryModel quoteQuery, CholaResponseDto result)
    {
        List<NameValueModel> addOnsList = new List<NameValueModel>();
        if (quoteQuery.AddOns.IsZeroDebt)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ZeroDebtId,
                Name = "Zero Dep",
                Value = Convert.ToString(result.Data.Zero_Depreciation),
                IsApplicable = IsApplicable(result.Data.Zero_Depreciation)
            });
        }
        if (quoteQuery.AddOns.IsEngineProtectionRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.EngineProtectionId,
                Name = "Engine Gearbox Protection",
                Value = Convert.ToString(result.Data.Hydrostatic_Lock_Cover),
                IsApplicable = IsApplicable(result.Data.Hydrostatic_Lock_Cover)
            });
        }
        if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                Name = "Key And Lock Protection",
                Value = Convert.ToString(result.Data.Key_Replacement_Cover),
                IsApplicable = IsApplicable(result.Data.Key_Replacement_Cover)
            });
        }
        if (quoteQuery.AddOns.IsRoadSideAssistanceRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RoadSideAssistanceId,
                Name = "Road Side Assistance",
                Value = Convert.ToString(result.Data.RSA_Cover),
                IsApplicable = IsApplicable(result.Data.RSA_Cover)
            });
        }
        if (quoteQuery.AddOns.IsConsumableRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ConsumableId,
                Name = "Consumables",
                Value = Convert.ToString(result.Data.Consumables_Cover),
                IsApplicable = IsApplicable(result.Data.Consumables_Cover)
            });
        }
        if (quoteQuery.AddOns.IsInvoiceCoverRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ReturnToInvoiceIdId,
                Name = "RTI",
                Value = Convert.ToString(result.Data.Final_Return_To_Invoice_Cover_Premium),
                IsApplicable = IsApplicable(result.Data.Final_Return_To_Invoice_Cover_Premium)
            });
        }
        if (quoteQuery.AddOns.IsPersonalBelongingRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.PersonalBelongingId,
                Name = "Personal Belongings",
                Value = Convert.ToString(result.Data.Personal_Belonging_Cover),
                IsApplicable = IsApplicable(result.Data.Personal_Belonging_Cover)
            });
        }
        if (quoteQuery.AddOns.IsDailyAllowance)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.DailyAllowanceId,
                Name = "Daily Allowance",
                Value = Convert.ToString(result.Data.Final_Daily_Cash_Allowance_Cover_Premium),
                IsApplicable = IsApplicable(result.Data.Final_Daily_Cash_Allowance_Cover_Premium)
            });
        }
        if (quoteQuery.AddOns.IsTyreProtectionRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.TyreProtectionId,
                Name = "Tyre Protection",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.AddOns.IsNCBRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.NCBId,
                Name = "No Claim Bonus Protection",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.AddOns.IsRimProtectionRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RimProtectionId,
                Name = "RIM Protection",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.AddOns.IsEMIProtectorRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.EMIProtectorId,
                Name = "EMI Protection",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.AddOns.IsGeoAreaExtension)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.GeoAreaExtensionId,
                Name = "Geo Area Extension",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.AddOns.IsLossOfDownTimeRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.LossOfDownTimeId,
                Name = "Limited to Own Premises",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RoadSideAssistanceAdvanceId,
                Name = "Road Side Assistance Advance",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.AddOns.IsRoadSideAssistanceWiderRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RoadSideAssistanceWiderId,
                Name = "Road Side Assistance Wider",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.AddOns.IsTowingRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.TowingId,
                Name = "Towing Protection",
                Value = null,
                IsApplicable = false,
            });
        }
        if (quoteQuery.AddOns.IsLimitedOwnPremisesRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.LimitedOwnPremisesId,
                Name = "Limited to Own Premises",
                Value = null,
                IsApplicable = false,
            });
        }

        return addOnsList;
    }
    private static List<NameValueModel> SetPACoverResponse(QuoteQueryModel quoteQuery, CholaResponseDto result)
    {
        List<NameValueModel> paCoverList = new List<NameValueModel>();
        if (quoteQuery.PACover.IsUnnamedPassenger)
        {
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedPassengerId,
                Name = "PA Cover for Unnamed Passengers",
                Value = Convert.ToString(result.Data.Unnamed_Passenger_Cover),
                IsApplicable = IsApplicable(result.Data.Unnamed_Passenger_Cover)
            });
        }
        if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
        {
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                Name = "PA Cover for Owner Driver",
                Value = Convert.ToString(result.Data.Personal_Accident),
                IsApplicable = IsApplicable(result.Data.Personal_Accident)
            });
        }
        if (quoteQuery.PACover.IsUnnamedPillionRider)
        {
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedPillionRiderId,
                Name = "PA Cover For Unnamed Pillion Rider",
                Value = Convert.ToString(result.Data.Unnamed_Passenger_Cover),
                IsApplicable = IsApplicable(result.Data.Unnamed_Passenger_Cover)
            });
        }
        if (!quoteQuery.CurrentPolicyType.Equals("SAOD") && quoteQuery.VehicleTypeId.Equals("2d566966-5525-4ed7-bd90-bb39e8418f39"))
        {
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.PaidDriverId,
                Name = "PA Cover for Paid Driver",
                Value = Convert.ToString(result.Data.Legal_Liability_to_paid_driver),
                IsApplicable = IsApplicable(result.Data.Legal_Liability_to_paid_driver)
            });
        }
        else if (quoteQuery.PACover.IsPaidDriver)
        {
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.PaidDriverId,
                Name = "PA Cover for Paid Driver",
                Value = Convert.ToString(result.Data.Legal_Liability_to_paid_driver),
                IsApplicable = IsApplicable(result.Data.Legal_Liability_to_paid_driver)
            });
        }
        return paCoverList;
    }
    private static bool IsApplicable(object _val)
    {
        string val = Convert.ToString(_val);
        return !(string.IsNullOrEmpty(val) || val == "0" || Convert.ToDecimal(val) == 0);
    }
    private static List<NameValueModel> SetBaseCover(string previousPolicy, CholaResponseDto result)
    {
        List<NameValueModel> baseCoverList = new List<NameValueModel>();
        if (previousPolicy.Equals("Package Comprehensive") || previousPolicy.Equals("Comprehensive Bundle"))
        {
            baseCoverList = new List<NameValueModel>
            {
                new NameValueModel
                {
                        Name = "Basic Own Damage Premium",
                        Value = Convert.ToString(result.Data?.Basic_Own_Damage_CNG_Elec_Non_Elec - result.Data?.DTD_Discounts),
                        IsApplicable = IsApplicable(result.Data?.Basic_Own_Damage_CNG_Elec_Non_Elec),
                },
                new NameValueModel
                {
                        Name = "Third Party Cover Premium",
                        Value = Convert.ToString(result.Data?.Basic_Third_Party_Premium),
                        IsApplicable = IsApplicable(result.Data?.Basic_Third_Party_Premium),
                },
            };
        }
        if (previousPolicy.Equals("SAOD"))
        {
            baseCoverList = new List<NameValueModel>
            {
                new NameValueModel
                {
                        Name = "Basic Own Damage Premium",
                        Value = Convert.ToString(result.Data?.Basic_Own_Damage_CNG_Elec_Non_Elec  - result.Data?.DTD_Discounts),
                        IsApplicable = IsApplicable(result.Data?.Basic_Own_Damage_CNG_Elec_Non_Elec),
                }
            };
        }
        if (previousPolicy.Equals("SATP"))
        {
            baseCoverList = new List<NameValueModel>
            {
               new NameValueModel
                {
                         Name = "Third Party Cover Premium",
                        Value = Convert.ToString(result.Data?.Basic_Third_Party_Premium),
                        IsApplicable = IsApplicable(result.Data?.Basic_Third_Party_Premium),
                }
            };
        }
        return baseCoverList;
    }
    public async Task<CholaPaymentWrapperModel> GetPaymentDetails(CholaPaymentTaggingRequestModel requestModel, CancellationToken cancellationToken)
    {
        var paymentResponse = new CholaPaymentWrapperModel();
        var responseBody = string.Empty;
        var id = 0;
        try
        {
            var paymentRequestBody = string.Empty;
            var token = await GetToken(requestModel.LeadId, "Payment");
            var req = new PaymentRequestModel()
            {
                user_code = _cholaConfig.UserCode,
                billdesk_txn_amount = requestModel.Amount,
                billdesk_txn_date = requestModel.TransactionDate,
                payment_id = requestModel.PaymentId,
                billdesk_txn_ref_no = requestModel.TransactionReferenceNumber,
                payment_mode = "PG",
                total_amount = requestModel.Amount
            };
            paymentRequestBody = JsonConvert.SerializeObject(req);
            _logger.LogInformation("Chola GetPaymentDetails Request {Request}", paymentRequestBody);

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);
            id = await InsertICLogs(paymentRequestBody, requestModel.LeadId, _cholaConfig.PaymentWrapperURL, token.Token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "Payment");
            try
            {
                var response = await _client.PostAsync(_cholaConfig.PaymentWrapperURL, new StringContent(paymentRequestBody, Encoding.UTF8, "application/json"), cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    responseBody = response.ReasonPhrase;
                    _logger.LogError("Chola GetPaymentDetails Exception {Exception}", paymentRequestBody);
                    await UpdateICLogs(id, requestModel?.PaymentId, responseBody);
                    return paymentResponse;
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var result = stream.DeserializeFromJson<CholaPaymentWrapperModel>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("Chola GetPaymentDetails Response {Response}", JsonConvert.SerializeObject(result));
                    // Get Policy PDF Download 
                    if (result != null && result.Data != null && result.Data.policy_id != null)
                    {
                        string documentURL = _cholaConfig.PolicyDocumentURL + result.Data.policy_id + "&user_code=" + _cholaConfig.UserCode;
                        result.Data.PolicyURL = documentURL;
                        result.Data.PdfBase64 = await GetDocumentPDFBase64(requestModel.LeadId, documentURL, result.Data.policy_id, cancellationToken);
                    }
                    // Policy PDF download code
                    await UpdateICLogs(id, requestModel?.PaymentId, responseBody);
                    return result;
                }
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, requestModel?.PaymentId, ex.Message);
                _logger.LogError("Chola Payment Response {exception}", ex.Message);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Chola Payment Response {exception}", ex.Message);
            return default;
        }
    }
    public async Task<byte[]> GetDocumentPDFBase64(string leadId, string documentLink, string policyId, CancellationToken cancellationToken)
    {
        var id = 0;
        try
        {
            _logger.LogInformation("Chola GetDocumentPDFBase64 Request {Request}", documentLink);
            id = await InsertICLogs(string.Empty, leadId, documentLink, string.Empty, string.Empty, "Payment");
            try
            {
                var token = await GetToken(leadId, "Payment");
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);
                var response = await _client.GetAsync(documentLink, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                    await UpdateICLogs(id, policyId, Convert.ToBase64String(data));
                    _logger.LogInformation("Chola GetDocumentPDFBase64 Response {Response}", data);
                    return data;
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    await UpdateICLogs(id, policyId, responseBody);
                }
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, policyId, ex.Message);
                _logger.LogError("Chola GetDocumentPDFBase64 Exception {Exception}", ex.Message);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Chola GetDocumentPDFBase64 Exception {Exception}", ex.Message);
            return default;
        }
        return default;
    }

    #region Breakin
    public async Task<Tuple<string, string, string, string, string>> CreateBreakIn(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken)
    {
        var requestBody = string.Empty;
        var responseBody = string.Empty;
        string breakInPin = string.Empty;
        string errorMessage = string.Empty;
        string inspectionURL = string.Empty;
        var id = 0;
        try
        {
            CholaProposalRequest cholaProposalRequest = JsonConvert.DeserializeObject<CholaProposalRequest>(quoteTransactionDbModel.ProposalRequestBody);
            CholaRequestDto quoteRequest = JsonConvert.DeserializeObject<CholaRequestDto>(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody);
            CholaResponseDto quoteResponse = JsonConvert.DeserializeObject<CholaResponseDto>(quoteTransactionDbModel.QuoteTransactionRequest.ResponseBody);

            CholaVehicleInspectionRequestModel breakinRequestModel = new CholaVehicleInspectionRequestModel()
            {
                QuoteID = quoteResponse.Data.quote_id,
                CustomerName = cholaProposalRequest.PersonalDetails.firstName,
                EmailId = cholaProposalRequest.PersonalDetails.emailId,
                MobileNumber = cholaProposalRequest.PersonalDetails.mobile,
                Productcode = _cholaConfig.Productcode,
                Vehiclemodelcode = quoteRequest.vehicle_model_code,
                RegistrationNumber = quoteRequest.reg_no,
                Intermediary_Code = _cholaConfig.IntermediaryCode,//DateTime.Now.Millisecond.ToString(),
                TieupFlag = _cholaConfig.TieupFlag
            };

            _client.DefaultRequestHeaders.Clear();
            requestBody = JsonConvert.SerializeObject(breakinRequestModel);
            _logger.LogError("Chola CreateBreakIn RequestBody {requestBody}", requestBody);
            _client.DefaultRequestHeaders.Clear();
            id = await InsertICLogs(requestBody, quoteTransactionDbModel?.LeadDetail?.LeadID, _cholaConfig.BreakInURL, string.Empty, string.Empty, "BreakIn");
            try
            {
                var response = await _client.PostAsync(_cholaConfig.BreakInURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    responseBody = response.ReasonPhrase;
                    _logger.LogError("Chola CreateBreakIn Error {responseBody}", responseBody);
                    await UpdateICLogs(id, quoteResponse?.Data.quote_id, responseBody);
                    return Tuple.Create(breakInPin, requestBody, responseBody, errorMessage, inspectionURL);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var breakinResponse = stream.DeserializeFromJson<CholaVehicleInspectionResponseModel>();
                    responseBody = JsonConvert.SerializeObject(breakinResponse);
                    if (breakinResponse.Status && breakinResponse.Flag.ToLower().Equals("success") && breakinResponse.Breakin_InspectionURL != null)
                    {
                        inspectionURL = breakinResponse.Breakin_InspectionURL;
                        breakInPin = breakinResponse.Referencenumber;
                        errorMessage = breakinResponse.Message;
                    }
                    else
                    {
                        errorMessage = breakinResponse.Message;
                    }
                    _logger.LogError("Chola CreateBreakIn responseBody {responseBody}", responseBody);
                    await UpdateICLogs(id, quoteResponse?.Data.quote_id, responseBody);
                    return Tuple.Create(breakInPin, requestBody, responseBody, errorMessage, inspectionURL);
                }
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, quoteResponse?.Data.quote_id, ex.Message);
                _logger.LogError("Chola CreateBreakIn exception {exception}", ex.Message);
                return Tuple.Create(breakInPin, requestBody, responseBody, errorMessage, inspectionURL);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Chola CreateBreakIn exception {exception}", ex.Message);
            return Tuple.Create(breakInPin, requestBody, responseBody, errorMessage, inspectionURL);
        }
    }
    public async Task<CholaBreakInResponseModel> GetBreakInStatus(GetBreakinStatusQuery getBreakinStatusQuery, CancellationToken cancellationToken)
    {
        var id = 0;
        var responseBody = string.Empty;
        try
        {
            var request = string.Empty;
            request = getBreakinStatusQuery.ReferenceNumber.ToString();
            string requestUrl = _cholaConfig.BreakInStatusURL + request;
            _logger.LogInformation("Chola GetBreakInStatus Request {requestUrl}", requestUrl);
            id = await InsertICLogs(string.Empty, getBreakinStatusQuery.LeadId, requestUrl, string.Empty, string.Empty, "BreakIn");
            try
            {
                var response = await _client.GetAsync(requestUrl, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var cholaBreakInResponse = stream.DeserializeFromJson<List<CholaBreakInResponseModel>>();
                    responseBody = JsonConvert.SerializeObject(cholaBreakInResponse);
                    _logger.LogInformation("Chola GetBreakInStatus Response {cholaBreakInResponse}", JsonConvert.SerializeObject(cholaBreakInResponse));
                    await UpdateICLogs(id, getBreakinStatusQuery?.ReferenceNumber, responseBody);
                    return cholaBreakInResponse.FirstOrDefault();
                }
                else
                {
                    responseBody = await response.Content.ReadAsStringAsync();
                    await UpdateICLogs(id, getBreakinStatusQuery?.ReferenceNumber, responseBody);
                    return default;
                }
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, getBreakinStatusQuery?.ReferenceNumber, ex.Message);
                _logger.LogError("Chola GetBreakInStatus Error {ex.Message}", ex.Message);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Chola GetBreakInStatus Error {ex.Message}", ex.Message);
            return default;
        }
    }
    #endregion
    public async Task<ProposalResponseModel> CreateProposal(CholaServiceRequestModel proposalQuery, CholaProposalRequest proposalRequest, CreateLeadModel createLeadModel, CholaCKYCRequestModel ckycRequestModel, CancellationToken cancellationToken)
    {
        ProposalResponseModel proposalResponseVM = new ProposalResponseModel();
        var proposalVm = new QuoteResponseModel();
        string requestBody = string.Empty;
        string responseBody = string.Empty;
        var id = 0;
        try
        {
            //Vehicle Details
            proposalQuery.chassis_no = proposalRequest.VehicleDetails?.chassisNumber;
            proposalQuery.engine_no = proposalRequest.VehicleDetails?.engineNumber;
            proposalQuery.hypothecated = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? "Yes" : "No";
            proposalQuery.financier_details = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails?.financer : string.Empty;
            proposalQuery.financieraddress = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails?.branch : string.Empty;
            // CKYC Details
            proposalQuery.CKYC_App_Ref_No = ckycRequestModel?.App_Ref_No;
            proposalQuery.CKYC_DOB_DOI = ckycRequestModel?.DOB_DOI;
            proposalQuery.CKYC_No = ckycRequestModel?.CKYC_No;
            proposalQuery.CKYC_PAN_No = ckycRequestModel?.PAN_No;
            proposalQuery.CKYC_Aadhar_No = ckycRequestModel?.Aadhar_No;
            proposalQuery.CKYC_DL_No = ckycRequestModel?.DL_No;
            proposalQuery.CKYC_Voter_ID = ckycRequestModel?.Voter_ID;
            proposalQuery.CKYC_Passport_no = ckycRequestModel?.Passport_no;
            proposalQuery.CKYC_CIN = ckycRequestModel?.CIN;
            proposalQuery.CKYC_KYC_Verified = ckycRequestModel?.Verify_Type == "VERIFY" ? "Yes" : "No";
            // POSP Details
            proposalQuery.posp_name = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetUserName() : string.Empty;
            proposalQuery.POSPcode = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetPOSPId() : string.Empty;
            proposalQuery.POSPPAN = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetPAN() : string.Empty;
            proposalQuery.POSPaadhar = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetAadhaarNumber() : string.Empty;
            proposalQuery.POSPcontactno = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetMobileNo() : string.Empty;
            proposalQuery.posp_direct = "Direct";
            //User Details
            proposalQuery.usr_name = createLeadModel.CarOwnedBy.Equals("INDIVIDUAL") ? proposalRequest.PersonalDetails?.firstName : string.Empty;
            proposalQuery.usr_mobile = proposalRequest.PersonalDetails?.mobile;
            proposalQuery.first_name = createLeadModel.CarOwnedBy.Equals("INDIVIDUAL") ?
                proposalRequest.PersonalDetails?.firstName : proposalRequest.PersonalDetails.companyName;
            proposalQuery.phone_no = proposalRequest.PersonalDetails?.mobile;
            proposalQuery.email_id = proposalRequest.PersonalDetails?.emailId;
            //Nominee Details
            proposalQuery.nominee_name = proposalRequest.NomineeDetails?.nomineeName;
            proposalQuery.nominee_relationship = proposalRequest.NomineeDetails?.nomineeRelation;
            proposalQuery.customer_dob_input = createLeadModel.CarOwnedBy.Equals("INDIVIDUAL") ?
                Convert.ToString(Convert.ToDateTime(proposalRequest.PersonalDetails.dateOfIncorporation).ToOADate()) :
                Convert.ToString(Convert.ToDateTime(proposalRequest.PersonalDetails.dateOfBirth).ToOADate());
            // Address Details
            proposalQuery.address = proposalRequest.AddressDetails.addressLine1;
            proposalQuery.city = proposalRequest.AddressDetails.city;
            proposalQuery.state = proposalRequest.AddressDetails.state;
            proposalQuery.pincode = proposalRequest.AddressDetails.pincode;
            //Breakin Details
            proposalQuery.breakin_insp_date = !string.IsNullOrEmpty(createLeadModel?.BreakinId) && !string.IsNullOrEmpty(createLeadModel.BreakinInspectionDate) ? createLeadModel.BreakinInspectionDate.Split(" ")[0] : string.Empty;
            proposalQuery.breakin_insp_time = !string.IsNullOrEmpty(createLeadModel?.BreakinId) && !string.IsNullOrEmpty(createLeadModel.BreakinInspectionDate) ? createLeadModel.BreakinInspectionDate.Split(" ")[1] : string.Empty;
            proposalQuery.breakin_insp_ref_number = !string.IsNullOrEmpty(createLeadModel.BreakinId) ? createLeadModel.BreakinId : string.Empty;
            proposalQuery.breakin_insp_agency = !string.IsNullOrEmpty(createLeadModel.InspectionAgency) ? createLeadModel.InspectionAgency : string.Empty;
            proposalQuery.breakin_insp_place = string.Empty;

            requestBody = JsonConvert.SerializeObject(proposalQuery);
            _logger.LogInformation("Chola CreateProposal Request {Request}", requestBody);

            var token = await GetToken(createLeadModel.LeadID, "Proposal");
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);
            id = await InsertICLogs(requestBody, createLeadModel.LeadID, _cholaConfig.ProposalWrapperURL, token.Token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "Proposal");
            try
            {
                var response = await _client.PostAsync(_cholaConfig.ProposalWrapperURL, new StringContent(requestBody, Encoding.UTF8,
                            "application/json"), cancellationToken);

                var stream = await response.Content.ReadAsStreamAsync();
                var result = stream.DeserializeFromJson<CholaServiceResponseModel>();
                responseBody = JsonConvert.SerializeObject(result);
                _logger.LogInformation("Chola CreateProposal Response {Response}", responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    if ((result.Code.Equals("400") && !string.IsNullOrEmpty(result.Message)))
                    {
                        proposalVm.ValidationMessage = result.Message;
                    }
                    else
                    {
                        proposalVm.ValidationMessage = result?.Message;
                    }

                    proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    await UpdateICLogs(id, proposalQuery?.proposal_id, responseBody);
                    return proposalResponseVM;
                }
                else
                {
                    if (result != null && result.Code.Equals("200"))
                    {
                        proposalVm = new QuoteResponseModel
                        {
                            Tax = new ServiceTax()
                            {
                                totalTax = Convert.ToString(result.Data.GST),
                            },
                            ApplicationId = result.Data.payment_id,
                            InsurerName = _cholaConfig.InsurerName,
                            InsurerStatusCode = (int)HttpStatusCode.OK,
                            TotalPremium = Convert.ToString(result.Data.Net_Premium),
                            GrossPremium = Convert.ToString(result.Data.Total_Premium),
                            IsBreakIn = false,
                            IsSelfInspection = false
                        };
                    }
                    else
                        proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                }
                proposalResponseVM = new ProposalResponseModel()
                {
                    quoteResponseModel = proposalVm,
                    RequestBody = requestBody,
                    ResponseBody = responseBody
                };
                await UpdateICLogs(id, proposalQuery?.proposal_id, responseBody);
                return proposalResponseVM;
            }
            catch (Exception ex)
            {
                _logger.LogError("Chola Proposal Error {exception}", ex.Message);
                proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                await UpdateICLogs(id, proposalQuery?.proposal_id, ex.Message);
                return proposalResponseVM;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Chola Proposal Error {exception}", ex.Message);
            proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return proposalResponseVM;
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
    private async Task UpdateICLogs(int id, string applicationId, string data)
    {
        var logsModel = new LogsModel
        {
            Id = id,
            ResponseBody = data,
            ResponseTime = DateTime.Now,
            ApplicationId = applicationId
        };
        await _commonService.UpdateLogs(logsModel);
    }
    private async Task<int> InsertICLogs(string requestBody, string leadId, string api, string token, string header, string stage)
    {
        var logsModel = new LogsModel
        {
            InsurerId = _cholaConfig.InsurerId,
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
}

