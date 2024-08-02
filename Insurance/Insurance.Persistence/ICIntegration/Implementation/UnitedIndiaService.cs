using Insurance.Core.Contracts.Common;
using Insurance.Core.Features.UnitedIndia.Command;
using Insurance.Core.Features.UnitedIndia.Payment;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.UnitedIndia;
using Insurance.Domain.UnitedIndia.Payment;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Persistence.ICIntegration.Implementation;

public class UnitedIndiaService : IUnitedIndiaService
{
    private readonly UnitedIndiaConfig _unitedIndiaConfig;
    private readonly ICommonService _commonService;
    private readonly IApplicationClaims _applicationClaims;
    private readonly ILogger<UnitedIndiaService> _logger;
    private readonly HttpClient _client;
    private readonly PolicyTypeConfig _policyTypeConfig;
    private const string KYC_SUCCESS = "KYC_SUCCESS";
    private const string InsurerName = "United India";
    private const string FAILED = "FAILED";
    private const string MESSAGE = "Please enter correct document number or proceed with other insurer";
    private const string POA_REQUIRED = "POA_REQUIRED";

    public UnitedIndiaService(IOptions<UnitedIndiaConfig> unitedIndiaConfig,
        ICommonService commonService,
        IApplicationClaims applicationClaims,
        ILogger<UnitedIndiaService> logger,
        HttpClient client,
        IOptions<PolicyTypeConfig> policyTypeConfig)
    {
        _unitedIndiaConfig = unitedIndiaConfig.Value;
        _commonService = commonService;
        _applicationClaims = applicationClaims;
        _logger = logger;
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _policyTypeConfig = policyTypeConfig.Value;
    }

    #region Quotation
    public async Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
    {
        var quoteVm = new QuoteResponseModel();
        string requestBody = string.Empty;
        var responseBody = string.Empty;
        var registrationNumber = new List<string>();
        var paCoverPeriod = "0";
        var keyAndLockProductSum = "0";
        var paUnnamed = string.Empty;
        var vehicleAge = GetVehicleAge(quoteQuery.RegistrationDate);
        bool isBreakin = false;

        if (!string.IsNullOrEmpty(quoteQuery.VehicleNumber))
        {
            registrationNumber = VehicleNumberSplit(quoteQuery.VehicleNumber).ToList();
        }
        else
        {
            var regNum = VehicleNumberSplit(quoteQuery.RegistrationRTOCode);
            registrationNumber = regNum.ToList();
            registrationNumber.Add("C");
            registrationNumber.Add("5632");
        }

        if (registrationNumber.Count == 2)
        {
            registrationNumber.Add("C");
            registrationNumber.Add("5632");
        }

        if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired && quoteQuery.VehicleDetails.IsFourWheeler)
        {
            keyAndLockProductSum = "10000";
        }
        else
        {
            keyAndLockProductSum = "3000";
        }

        if (quoteQuery.IsBrandNewVehicle)
        {
            if (quoteQuery.VehicleDetails.IsFourWheeler)
            {
                paCoverPeriod = "3";
            }
            else
            {
                paCoverPeriod = "5";
            }
        }
        else
        {
            paCoverPeriod = "1";

            if (DateTime.Compare(Convert.ToDateTime(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD), DateTime.UtcNow.Date) < 0 && DateTime.Compare(Convert.ToDateTime(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD), DateTime.UtcNow.Date) != 0)
            {
                isBreakin = true;
            }
        }

        if (quoteQuery.CurrentPolicyType.Equals("StandAloneOD") && quoteQuery.VehicleDetails.IsTwoWheeler)
        {
            paUnnamed = "-1";
        }
        else if (quoteQuery.VehicleDetails.IsTwoWheeler || quoteQuery.PACover.IsUnnamedPassenger)
        {
            paUnnamed = quoteQuery.VehicleDetails.VehicleSeatCapacity;
        }

        try
        {
            var requestFraming = new UnitedIndiaRoot()
            {
                HEADER = new UnitedIndiaHeader()
                {
                    DAT_DATE_OF_EXPIRY_OF_POLICY = Convert.ToDateTime(quoteQuery.PolicyEndDate).ToString("dd/MM/yyyy").Replace("-", "/"),
                    DAT_DATE_OF_ISSUE_OF_POLICY = Convert.ToDateTime(quoteQuery.PolicyStartDate).ToString("dd/MM/yyyy").Replace("-", "/"),
                    DAT_DATE_OF_REGISTRATION = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd/MM/yyyy").Replace("-", "/"),
                    NUM_BUSINESS_CODE = quoteQuery.IsBrandNewVehicle ? "New Business" : "Roll Over",
                    NUM_CLIENT_TYPE = "I",
                    NUM_CUBIC_CAPACITY = quoteQuery.VehicleDetails.VehicleCubicCapacity,
                    NUM_RGSTRD_SEATING_CAPACITY = quoteQuery.VehicleDetails.VehicleSeatCapacity,
                    NUM_IEV_BASE_VALUE = quoteQuery.IDVValue.ToString(),
                    NUM_MONTH_OF_MANUFACTURE = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("MM"),
                    NUM_YEAR_OF_MANUFACTURE = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("yyyy"),
                    NUM_POLICY_TYPE = quoteQuery.CurrentPolicyType,
                    TXT_FUEL = quoteQuery.VehicleDetails.Fuel,
                    YN_INBUILT_LPG = (string.Equals(quoteQuery.VehicleDetails.Fuel, "LPG", StringComparison.OrdinalIgnoreCase)) ? "-1" : "0",
                    YN_INBUILT_CNG = (string.Equals(quoteQuery.VehicleDetails.Fuel, "CNG", StringComparison.OrdinalIgnoreCase)) ? "-1" : "0",
                    TXT_NAME_OF_MANUFACTURER = quoteQuery.VehicleDetails.VehicleMake,
                    TXT_OEM_DEALER_CODE = _unitedIndiaConfig.DealerCode,
                    TXT_OEM_TRANSACTION_ID = $"UIIC{DateTime.Now.ToString("yyyyMMddHHmmss")}",
                    TXT_OTHER_MAKE = quoteQuery.VehicleDetails.VehicleModel,
                    TXT_TYPE_BODY = quoteQuery.VehicleDetails.VehicleSegment,
                    TXT_VARIANT = quoteQuery.VehicleDetails.VehicleVariant,
                    YN_VALID_DRIVING_LICENSE = "Y",
                    NUM_VEHICLE_MODEL_CODE = quoteQuery.VehicleDetails.IsFourWheeler ? quoteQuery.VehicleDetails.VehicleModelCode : string.Empty,
                    TXT_REGISTRATION_NUMBER_1 = quoteQuery.IsBrandNewVehicle ? "NEW" : registrationNumber[0],
                    TXT_REGISTRATION_NUMBER_2 = quoteQuery.IsBrandNewVehicle ? string.Empty : registrationNumber[1],
                    TXT_REGISTRATION_NUMBER_3 = quoteQuery.IsBrandNewVehicle  ? string.Empty : registrationNumber[2],
                    TXT_REGISTRATION_NUMBER_4 = quoteQuery.IsBrandNewVehicle ? string.Empty : registrationNumber[3],
                    TXT_ENGINE_NUMBER = "2FGJHJ459",
                    TXT_CHASSIS_NUMBER = "MAN91338328487344378002",
                    TXT_DOB = "10/04/1998",
                    TXT_NAME_OF_INSURED = "Nantha",
                    NUM_PIN_CODE = "411001",
                    MEM_ADDRESS_OF_INSURED = "XYZ",
                    DAT_HOURS_EFFECTIVE_FROM = "00:00:01",
                    //Accessories
                    TXT_ELEC_DESC = quoteQuery.Accessories.IsElectrical ? "Electrical Accessories" : string.Empty,
                    NUM_IEV_ELEC_ACC_VALUE = quoteQuery.Accessories.IsElectrical ? quoteQuery.Accessories.ElectricalValue.ToString() : string.Empty,
                    TXT_NON_ELEC_DESC = quoteQuery.Accessories.IsNonElectrical ? "Non-Electrical Accessories" : string.Empty,
                    NUM_IEV_NON_ELEC_ACC_VALUE = quoteQuery.Accessories.IsNonElectrical ? quoteQuery.Accessories.ElectricalValue.ToString() : string.Empty,
                    NUM_IEV_CNG_VALUE = (quoteQuery.Accessories.IsCNG && string.Equals(quoteQuery.VehicleDetails.Fuel, "CNG", StringComparison.OrdinalIgnoreCase)) ? quoteQuery.Accessories.CNGValue.ToString() : string.Empty,
                    //Discounts
                    NUM_TPPD_AMOUNT = quoteQuery.VehicleDetails.IsTwoWheeler ? "6000" : "750000",
                    YN_ANTI_THEFT = quoteQuery.Discounts.IsAntiTheft ? "-1" : "0",
                    NUM_VOLUNTARY_EXCESS_AMOUNT = quoteQuery.Discounts.IsVoluntarilyDeductible ? quoteQuery.VoluntaryExcess : string.Empty,
                    //PA Covers
                    YN_COMPULSORY_PA_DTLS = quoteQuery.PACover.IsUnnamedOWNERDRIVER ? "-1" : "0",
                    TXT_CPA_COVER_PERIOD = quoteQuery.PACover.IsUnnamedOWNERDRIVER ? paCoverPeriod : string.Empty,
                    TXT_RELATION_WITH_NOMINEE = quoteQuery.PACover.IsUnnamedOWNERDRIVER ? "Self" : string.Empty,
                    TXT_NAME_OF_NOMINEE = quoteQuery.PACover.IsUnnamedOWNERDRIVER ? "NomineeName" : string.Empty,
                    YN_PAID_DRIVER = quoteQuery.PACover.IsPaidDriver ? "Y" : "N",
                    NUM_LL1 = quoteQuery.PACover.IsPaidDriver ? "1" : string.Empty,
                    NUM_PA_UNNAMED_AMOUNT = quoteQuery.PACover.IsUnnamedPassenger ? quoteQuery.PACover.UnnamedPassengerValue.ToString() : string.Empty,
                    NUM_PA_UNNAMED_NUMBER = paUnnamed,

                    //Addons
                    TXT_GEOG_AREA_EXTN_COUNTRY = quoteQuery.AddOns.IsGeoAreaExtension ? quoteQuery.GeogExtension : string.Empty,
                    YN_RSA_COVER = quoteQuery.AddOns.IsRoadSideAssistanceRequired ? "Y" : "N",
                    YN_NCB_PROTECT = quoteQuery.AddOns.IsNCBRequired && quoteQuery.VehicleDetails.IsFourWheeler && !quoteQuery.IsBrandNewVehicle ? "True" : "False",
                    YN_ENGINE_GEAR_COVER = string.Empty,
                    YN_NIL_DEPR_WITHOUT_EXCESS = string.Empty
                }
            };

            if (Convert.ToDouble(vehicleAge) <= 4.5)
            {
                requestFraming.HEADER.YN_NIL_DEPR_WITHOUT_EXCESS = quoteQuery.AddOns.IsZeroDebt && !isBreakin ? "-1" : "0";
                requestFraming.HEADER.YN_ENGINE_GEAR_COVER = quoteQuery.AddOns.IsEngineProtectionRequired && !isBreakin ? "Y" : "N";
                requestFraming.HEADER.YN_CONSUMABLE = quoteQuery.AddOns.IsConsumableRequired ? "Y" : "N";
                requestFraming.HEADER.YN_LOSS_OF_KEY = quoteQuery.AddOns.IsKeyAndLockProtectionRequired ? "Y" : "N";
                requestFraming.HEADER.NUM_LOSS_OF_KEY_SUM_INSURED = keyAndLockProductSum;
                requestFraming.HEADER.YN_TYRE_RIM_PROTECTOR = quoteQuery.AddOns.IsRimProtectionRequired || quoteQuery.AddOns.IsTyreProtectionRequired && quoteQuery.VehicleDetails.IsFourWheeler ? "Y" : "N";
                requestFraming.HEADER.NUM_TYRE_RIM_SUM_INSURED = quoteQuery.AddOns.IsRimProtectionRequired || quoteQuery.AddOns.IsTyreProtectionRequired && quoteQuery.VehicleDetails.IsFourWheeler ? "25000" : string.Empty;
            }

            if (Convert.ToDouble(vehicleAge) <= 2.5)
            {
                requestFraming.HEADER.YN_RTI_APPLICABLE = quoteQuery.AddOns.IsReturnToInvoiceRequired && !isBreakin ? "Y" : "N";
            }

            if (quoteQuery.CurrentPolicyType.Equals("StandAloneOD"))
            {
                requestFraming.HEADER.TXT_TP_POLICY_NUMBER = quoteQuery.PreviousPolicyDetails.PreviousPolicyNumberTP;
                requestFraming.HEADER.TXT_TP_POLICY_INSURER = quoteQuery.PreviousPolicyDetails.PreviousInsurerCode;
                requestFraming.HEADER.TXT_TP_POLICY_START_DATE = Convert.ToDateTime(quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP).ToString("dd/MM/yyyy").Replace("-", "/");
                requestFraming.HEADER.TXT_TP_POLICY_END_DATE = Convert.ToDateTime(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP).ToString("dd/MM/yyyy").Replace("-", "/");
                requestFraming.HEADER.TXT_TP_POLICY_INSURER_ADDRESS = quoteQuery.PreviousPolicyDetails.PreviousSATPInsurer;
            }

            if (!quoteQuery.IsBrandNewVehicle)
            {
                requestFraming.HEADER.DAT_PREV_POLICY_EXPIRY_DATE = Convert.ToDateTime(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD).ToString("dd/MM/yyyy").Replace("-", "/");
                requestFraming.HEADER.CUR_BONUS_MALUS_PERCENT = quoteQuery.PreviousPolicyDetails.PreviousNoClaimBonus;
                requestFraming.HEADER.NUM_POLICY_NUMBER = quoteQuery.PreviousPolicyDetails.PreviousPolicyNumber;
                requestFraming.HEADER.TXT_PREV_INSURER_CODE = quoteQuery.PreviousPolicyDetails.PreviousInsurerCode;
                requestFraming.HEADER.YN_CLAIM = quoteQuery.PreviousPolicyDetails.IsClaimInLastYear ? "yes" : "no";
            }

            if (_applicationClaims.GetRole().Equals("POSP"))
            {
                requestFraming.HEADER.TXT_POSP_CODE = _applicationClaims.GetPOSPId();
                requestFraming.HEADER.TXT_POSP_NAME = _applicationClaims.GetUserName();
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UnitedIndiaRoot));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            StringBuilder requestBuilder = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(requestBuilder, settings);
            xmlSerializer.Serialize(xw, requestFraming, ns);
            var request = requestBuilder.ToString();
            _logger.LogInformation("UnitedIndia Quote request {request}", request);

            var addingHeader = new UnitedIndiaEnvelope()
            {
                Body = new UnitedIndiaBody()
                {
                    calculatePremium = new CalculatePremium()
                    {
                        application = _unitedIndiaConfig.ApplicationId,
                        userid = _unitedIndiaConfig.UserId,
                        password = _unitedIndiaConfig.Password,
                        productCode = quoteQuery.VehicleDetails.IsFourWheeler ? _unitedIndiaConfig.ProductCode4W : _unitedIndiaConfig.ProductCode2W,
                        subproductCode = quoteQuery.VehicleDetails.IsFourWheeler ? _unitedIndiaConfig.SubProductCode4W : _unitedIndiaConfig.SubProductCode2W,
                        proposalXml = string.Empty
                    }
                }
            };

            var cDataRequest = "<![CDATA[" + request + "]]>";

            XmlSerializer requestSerializer = new XmlSerializer(typeof(UnitedIndiaEnvelope));
            ns.Add("ws", "http://ws.uiic.com/");
            StringBuilder finalRequestBuilder = new StringBuilder();
            StringWriter requestStringWriter = new StringWriter(finalRequestBuilder);
            requestSerializer.Serialize(requestStringWriter, addingHeader, ns);
            requestBody = finalRequestBuilder.ToString();
            requestBody = requestBody.Replace("<proposalXml />", "<proposalXml>" + cDataRequest + "</proposalXml>");
            requestBody = requestBody.Replace("Body", "soapenv:Body");
            requestBody = requestBody.Replace("Envelope", "soapenv:Envelope");
            requestBody = requestBody.Replace("calculatePremium", "ws:calculatePremium");
            requestBody = requestBody.Replace("xmlns", "xmlns:soapenv");
            requestBody = requestBody.Replace("xmlns:soapenv:ws", "xmlns:ws");

            _logger.LogInformation("UnitedIndia Quote request {request}", requestBody);
            return await QuoteResponseFraming(requestBody, quoteQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("UnitedIndia Quote Error {exception}", ex.Message);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return Tuple.Create(quoteVm, requestBody, responseBody);
        }
    }
    public async Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        QuoteConfirmDetailsResponseModel quoteConfirm = new QuoteConfirmDetailsResponseModel();
        var quoteResponseVM = new QuoteConfirmResponseModel();
        var responseBody = string.Empty;
        var id = 0;
        ServiceTaxModel tax = new ServiceTaxModel();
        string commonResponse = quoteTransactionDbModel.QuoteTransactionRequest.CommonResponse;
        QuoteResponseModel updatedResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(commonResponse);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(UIICRequestEnvelopeRequest));

        var content = quoteTransactionDbModel.QuoteTransactionRequest.RequestBody;
        content = content.Replace("<![CDATA[", "");
        content = content.Replace("]]>", "");
        quoteTransactionDbModel.QuoteTransactionRequest.RequestBody = content;

        StringReader reader = new StringReader(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody);
        var requestframing = (UIICRequestEnvelopeRequest)(xmlSerializer.Deserialize(reader));
        string requestBody = string.Empty;
        bool isBreakin = false;
        bool isSelfInspection = false;
        bool isPolicyExpired = false;
        bool policyTypeSelfInspection = false;
        var todayDate = DateTime.Now.ToString("yyyy-MM-dd");

        bool isCurrPolicyEngineCover = requestframing.Body.CalculatePremium.proposalXml.ROOT.HEADER.YN_ENGINE_GEAR_COVER.Equals("Y");
        bool isCurrPolicyPartDept = requestframing.Body.CalculatePremium.proposalXml.ROOT.HEADER.YN_NIL_DEPR_WITHOUT_EXCESS.Equals("-1") ? true : false;
        var header = requestframing.Body.CalculatePremium.proposalXml.ROOT.HEADER;

        header.DAT_DATE_OF_EXPIRY_OF_POLICY = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyEndDate).ToString("dd/MM/yyyy").Replace("-", "/");
        header.DAT_DATE_OF_ISSUE_OF_POLICY = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("dd/MM/yyyy").Replace("-", "/");
        header.DAT_DATE_OF_REGISTRATION = Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("dd/MM/yyyy").Replace("-", "/");
        header.NUM_MONTH_OF_MANUFACTURE = Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("MM");
        header.NUM_YEAR_OF_MANUFACTURE = Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("yyyy");

        //PACover
        header.YN_COMPULSORY_PA_DTLS = !quoteConfirmCommand.IsPACover ? "-1" : "0";
        header.TXT_CPA_COVER_PERIOD = !quoteConfirmCommand.IsPACover ? quoteConfirmCommand.PACoverTenure : string.Empty;
        header.TXT_RELATION_WITH_NOMINEE = !quoteConfirmCommand.IsPACover ? "Self" : string.Empty;
        header.TXT_NAME_OF_NOMINEE = !quoteConfirmCommand.IsPACover ? "NomineeName" : string.Empty;

        if (!quoteConfirmCommand.IsBrandNewVehicle)
        {
            if (quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId != null && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId != null && quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId.Equals(_policyTypeConfig.SATP) && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive))
            {
                policyTypeSelfInspection = true;
            }

            if ((!quoteConfirmCommand.isPrevPolicyEngineCover && isCurrPolicyEngineCover) || (!quoteConfirmCommand.isPrevPolicyNilDeptCover && isCurrPolicyPartDept) || policyTypeSelfInspection)
            {
                isSelfInspection = true;
            }

            if (!quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SATP) && quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy)
            {
                if (Convert.ToDateTime(quoteConfirmCommand.PreviousPolicy.SAODPolicyExpiryDate) < Convert.ToDateTime(todayDate))
                {
                    isPolicyExpired = true;
                }
            }
            else if (!quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy)
            {
                isPolicyExpired = true;
            }

            if (quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy)
            {
                header.DAT_PREV_POLICY_EXPIRY_DATE = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("dd/MM/yyyy").Replace("-", "/");
                header.NUM_POLICY_NUMBER = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
                header.TXT_PREV_INSURER_CODE = quoteTransactionDbModel.QuoteConfirmDetailsModel.SAODInsurerCode;
                header.CUR_BONUS_MALUS_PERCENT = quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue;
                header.YN_CLAIM = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? "yes" : "no";

                if (header.NUM_POLICY_TYPE.Equals("StandAloneOD"))
                {
                    header.TXT_TP_POLICY_NUMBER = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP;
                    header.TXT_TP_POLICY_INSURER = quoteTransactionDbModel.QuoteConfirmDetailsModel.SATPInsurerCode;
                    header.TXT_TP_POLICY_START_DATE = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("dd/MM/yyyy").Replace("-", "/");
                    header.TXT_TP_POLICY_END_DATE = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("dd/MM/yyyy").Replace("-", "/");
                    header.TXT_TP_POLICY_INSURER_ADDRESS = quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSATPInsurerName;
                }
            }
            var registrationNumber = VehicleNumberSplit(quoteConfirmCommand.VehicleNumber).ToList();
            header.TXT_ENGINE_NUMBER = quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine.Replace(" ", "");
            header.TXT_CHASSIS_NUMBER = quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine.Replace(" ", "");
            header.TXT_REGISTRATION_NUMBER_1 = registrationNumber[0];
            header.TXT_REGISTRATION_NUMBER_2 = registrationNumber[1];
            header.TXT_REGISTRATION_NUMBER_3 = registrationNumber[2];
            header.TXT_REGISTRATION_NUMBER_4 = registrationNumber[3];

            if (quoteConfirmCommand.Customertype.Equals("COMPANY"))
            {
                header.TXT_NAME_OF_INSURED = quoteConfirmCommand.CompanyName;
                header.TXT_DOB = Convert.ToDateTime(quoteConfirmCommand.DOI).ToString("dd/MM/yyyy").Replace("-", "/");
                header.TXT_GSTIN_NUMBER = quoteConfirmCommand.GSTNo;
            }
        }

        XmlSerializer xmlSerializerSerialize = new XmlSerializer(typeof(UnitedIndiaRoot));
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        ns.Add("", "");
        var settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        StringBuilder requestBuilder = new StringBuilder();
        XmlWriter xw = XmlWriter.Create(requestBuilder, settings);
        xmlSerializerSerialize.Serialize(xw, requestframing.Body.CalculatePremium.proposalXml.ROOT, ns);
        var requestRoot = requestBuilder.ToString();
        _logger.LogInformation("UnitedIndia ConfirmQuote request Root {request}", requestRoot);


        var cDataRequest = "<![CDATA[" + requestRoot + "]]>";

        var request = new UnitedIndiaEnvelope()
        {
            Body = new UnitedIndiaBody()
            {
                calculatePremium = new CalculatePremium()
                {
                    application = _unitedIndiaConfig.ApplicationId,
                    userid = _unitedIndiaConfig.UserId,
                    password = _unitedIndiaConfig.Password,
                    productCode = requestframing.Body.CalculatePremium.productCode.ToString(),
                    subproductCode = requestframing.Body.CalculatePremium.subproductCode.ToString(),
                    proposalXml = string.Empty
                }
            }
        };

        XmlSerializer requestSerializer = new XmlSerializer(typeof(UnitedIndiaEnvelope));
        ns.Add("ws", "http://ws.uiic.com/");
        StringBuilder finalRequestBuilder = new StringBuilder();
        StringWriter requestStringWriter = new StringWriter(finalRequestBuilder);
        requestSerializer.Serialize(requestStringWriter, request, ns);
        requestBody = finalRequestBuilder.ToString();
        requestBody = requestBody.Replace("<proposalXml />", "<proposalXml>" + cDataRequest + "</proposalXml>");
        requestBody = requestBody.Replace("Body", "soapenv:Body");
        requestBody = requestBody.Replace("Envelope", "soapenv:Envelope");
        requestBody = requestBody.Replace("calculatePremium", "ws:calculatePremium");
        requestBody = requestBody.Replace("xmlns", "xmlns:soapenv");
        requestBody = requestBody.Replace("xmlns:soapenv:ws", "xmlns:ws");

        _logger.LogInformation("UnitedIndia ConfirmQuote request {request}", requestBody);


        var getQuoteResponse = await GetQuoteResponse(quoteTransactionDbModel.LeadDetail.LeadID, requestBody, "QuoteConfirm", cancellationToken);

        if (!getQuoteResponse.Item1.IsSuccessStatusCode)
        {
            responseBody = getQuoteResponse.Item1.ReasonPhrase;
            quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            _logger.LogError("UnitedIndia QuoteConfirm Not Found {responseBody}", responseBody);
        }
        else
        {
            id = getQuoteResponse.Item2;
            responseBody = getQuoteResponse.Item1.Content.ReadAsStringAsync().Result.ToString();
            responseBody = responseBody.Replace("&lt;", "<");
            responseBody = responseBody.Replace("&gt;", ">");

            StringReader responseReader = new StringReader(responseBody);
            XmlSerializer responseSerializer = new XmlSerializer(typeof(UnitedIndiaResponseEnvelope));
            var response = (UnitedIndiaResponseEnvelope)responseSerializer.Deserialize(responseReader);

            if (response != null && string.IsNullOrEmpty(response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_ERR_MSG))
            {
                var ncbValue = "0";
                var ncbItemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropLoadingDiscount_Col?.LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description == "Bonus Discount" || x.PropLoadingDiscount_Description.Equals("No Claim Bonus Discount"));

                if (ncbItemData != null)
                {
                    ncbValue = ncbItemData?.PropLoadingDiscount_Rate;
                }

                tax.totalTax = RoundOffValue(response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.CUR_FINAL_SERVICE_TAX);
                quoteConfirm = new QuoteConfirmDetailsResponseModel()
                {
                    InsurerStatusCode = (int)HttpStatusCode.OK,
                    InsurerName = _unitedIndiaConfig.InsurerName,
                    NewPremium = RoundOffValue(response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.CUR_FINAL_TOTAL_PREMIUM),
                    InsurerId = _unitedIndiaConfig.InsurerId,
                    NCB = ncbValue,
                    Tax = tax,
                    TotalPremium = RoundOffValue(response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.CUR_NET_FINAL_PREMIUM),
                    GrossPremium = RoundOffValue(response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.CUR_FINAL_TOTAL_PREMIUM),
                    IsBreakin = isBreakin,
                    IsSelfInspection = policyTypeSelfInspection,
                    IDV = Convert.ToInt32(updatedResponse.IDV),
                    MinIDV = Convert.ToInt32(updatedResponse.MinIDV),
                    MaxIDV = Convert.ToInt32(updatedResponse.MaxIDV),
                };
            }
            else
            {
                quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteConfirm.ValidationMessage = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_ERR_MSG;
            }
            quoteResponseVM = new QuoteConfirmResponseModel()
            {
                quoteConfirmResponse = quoteConfirm,
                quoteResponse = updatedResponse,
                RequestBody = requestBody,
                ResponseBody = responseBody,
                LeadId = quoteTransactionDbModel.LeadDetail.LeadID,
            };
        }
        await UpdateICLogs(id, string.Empty, responseBody);
        return quoteResponseVM;
    }

    private async Task<Tuple<QuoteResponseModel, string, string>> QuoteResponseFraming(string requestBody, QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
    {
        QuoteResponseModel quoteVm = new QuoteResponseModel();
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(UnitedIndiaResponseEnvelope));
        var responseBody = string.Empty;

        var getQuoteResponse = await GetQuoteResponse(quoteQuery.LeadId, requestBody, "Quote", cancellationToken);
        if (!getQuoteResponse.Item1.IsSuccessStatusCode)
        {
            responseBody = getQuoteResponse.Item1.ReasonPhrase;
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            _logger.LogError("UnitedIndia Quotation Not Found {responseBody}", responseBody);
        }
        else
        {
            responseBody = getQuoteResponse.Item1.Content.ReadAsStringAsync().Result.ToString();
            responseBody = responseBody.Replace("&lt;", "<");
            responseBody = responseBody.Replace("&gt;", ">");

            StringReader reader = new StringReader(responseBody);
            var response = (UnitedIndiaResponseEnvelope)xmlSerializer.Deserialize(reader);

            if (response != null && string.IsNullOrEmpty(response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_ERR_MSG))
            {
                List<NameValueModel> paCoverList = SetPACoverResponse(quoteQuery, response);
                List<NameValueModel> addOnsList = SetAddOnsResponse(quoteQuery, response);
                List<NameValueModel> accessoryList = SetAccessoryResponse(quoteQuery, response);
                List<NameValueModel> discountList = SetDiscountResponse(quoteQuery, response);

                var ncbItemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropLoadingDiscount_Col?.LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description.Equals("Bonus Discount") || x.PropLoadingDiscount_Description.Equals("No Claim Bonus Discount"));

                string totalTax = RoundOffValue(response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.CUR_FINAL_SERVICE_TAX);
                var tax = new ServiceTax
                {
                    totalTax = totalTax
                };

                quoteVm = new QuoteResponseModel
                {
                    InsurerName = _unitedIndiaConfig.InsurerName,
                    InsurerStatusCode = (int)HttpStatusCode.OK,
                    SelectedIDV = quoteQuery.SelectedIDV,
                    IDV = quoteQuery.IDVValue,
                    MinIDV = quoteQuery.MinIDV,
                    MaxIDV = quoteQuery.MaxIDV,
                    Tax = tax,
                    BasicCover = new BasicCover
                    {
                        CoverList = SetBaseCover(quoteQuery.CurrentPolicyType, response)
                    },
                    PACovers = new PACovers
                    {
                        PACoverList = paCoverList
                    },
                    AddonCover = new AddonCover
                    {
                        AddonList = addOnsList
                    },
                    Discount = new Domain.GoDigit.Discount
                    {
                        DiscountList = discountList
                    },
                    AccessoriesCover = new AccessoriesCover
                    {
                        AccessoryList = accessoryList
                    },
                    NCB = !string.IsNullOrEmpty(ncbItemData?.PropLoadingDiscount_Rate) ? ncbItemData?.PropLoadingDiscount_Rate : "0",
                    TotalPremium = RoundOffValue(response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.CUR_NET_FINAL_PREMIUM),
                    GrossPremium = RoundOffValue(response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.CUR_FINAL_TOTAL_PREMIUM),
                    RTOCode = quoteQuery.RegistrationRTOCode,
                    PolicyStartDate = Convert.ToDateTime(quoteQuery.PolicyStartDate).ToString("dd-MMM-yyyy"),
                    Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                    PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
                    IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                    IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                    RegistrationDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                    ManufacturingDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                    VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.RegistrationRTOCode : quoteQuery.VehicleNumber,
                    PolicyNumber = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_CUST_POLICY_NO,
                    ApplicationId = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_TRANSACTION_ID,
                };
            }
            else
            {
                quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteVm.InsurerName = _unitedIndiaConfig.InsurerName;
                quoteVm.ValidationMessage = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_ERR_MSG;
                _logger.LogError("UnitedIndia Quotation Not Found {responseBody}", responseBody);
            }
        }
        await UpdateICLogs(getQuoteResponse.Item2, string.Empty, responseBody);
        return Tuple.Create(quoteVm, getQuoteResponse.Item3, responseBody);
    }
    private async Task<Tuple<HttpResponseMessage, int, string>> GetQuoteResponse(string leadId, string requestBody, string stage, CancellationToken cancellationToken)
    {
        HttpResponseMessage quoteResponse;

        string url = _unitedIndiaConfig.BaseURL + _unitedIndiaConfig.QuoteURL;
        int id = 0;

        try
        {
            id = await InsertICLogs(requestBody, leadId, url, string.Empty, string.Empty, stage);
            quoteResponse = await _client.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "text/xml"), cancellationToken);
            return Tuple.Create(quoteResponse, id, requestBody);
        }
        catch (Exception ex)
        {
            _logger.LogError("UnitedIndia GetQuoteResponse {exception}", ex.Message);
            await UpdateICLogs(id, string.Empty, ex.Message);
            return default;
        }
    }
    private static List<NameValueModel> SetDiscountResponse(QuoteQueryModel quoteQuery, UnitedIndiaResponseEnvelope response)
    {
        var premium = "0";
        List<NameValueModel> discountList = new List<NameValueModel>();
        if (quoteQuery.Discounts.IsAntiTheft)
        {
            var itemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropLoadingDiscount_Col?.LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description == "Anti-Theft Device - OD");
            premium = !string.IsNullOrEmpty(itemData?.PropLoadingDiscount_EndorsementAmount) ? itemData?.PropLoadingDiscount_EndorsementAmount : "0";
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AntiTheftId,
                Name = "ARAI Approved Anti-Theft Device",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropLoadingDiscount_Applicable),
            });
        }

        if (quoteQuery.Discounts.IsAAMemberShip)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AAMemberShipId,
                Name = "AA Membership",
                Value = null,
                IsApplicable = false
            });
        }
        if (quoteQuery.Discounts.IsVoluntarilyDeductible)
        {
            var itemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropLoadingDiscount_Col?.LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description.Equals("Voluntary Excess Discount") || x.PropLoadingDiscount_Description.Equals("Voluntary Excess Discount-OD"));
            premium = !string.IsNullOrEmpty(itemData?.PropLoadingDiscount_EndorsementAmount) ? itemData?.PropLoadingDiscount_EndorsementAmount : "0";
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                Name = "Voluntary Deductible",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropLoadingDiscount_Applicable)
            });
        }
        if (!quoteQuery.CurrentPolicyType.Equals("StandAloneOD"))
        {

            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks?.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");

            var tpItemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups.Equals("Basic - TP") || x.PropCoverDetails_CoverGroups.Equals("Basic TP"));

            var itemData = tpItemData?.PropCoverDetails_LoadingDiscount_Col?.CoverDetails_LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description == "TPPD Discount");

            premium = !string.IsNullOrEmpty(itemData?.PropLoadingDiscount_EndorsementAmount) ? itemData?.PropLoadingDiscount_EndorsementAmount : "0";
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.LimitedTPCoverageId,
                Name = "Limited Third Party Coverage",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        var ncbItemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropLoadingDiscount_Col?.LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description.Equals("Bonus Discount") || x.PropLoadingDiscount_Description.Equals("No Claim Bonus Discount"));
        premium = !string.IsNullOrEmpty(ncbItemData?.PropLoadingDiscount_EndorsementAmount) ? ncbItemData?.PropLoadingDiscount_EndorsementAmount : "0";
        if (premium != "0")
        {
            discountList.Add(new NameValueModel
            {
                Name = $"No Claim Bonus ({ncbItemData?.PropLoadingDiscount_Rate}%)",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(ncbItemData?.PropLoadingDiscount_Applicable),
            });
        }
        return discountList;
    }
    private static List<NameValueModel> SetAccessoryResponse(QuoteQueryModel quoteQuery, UnitedIndiaResponseEnvelope response)
    {
        var premium = "0";
        List<NameValueModel> accessoryList = new List<NameValueModel>();

        if ((string.Equals(quoteQuery.VehicleDetails.Fuel, "LPG", StringComparison.OrdinalIgnoreCase)))
        {
            var itemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropLoadingDiscount_Col?.LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description.Replace(" ", "") == "BuiltinLPG-ODloading-OD");
            premium = !string.IsNullOrEmpty(itemData?.PropLoadingDiscount_EndorsementAmount) ? itemData?.PropLoadingDiscount_EndorsementAmount : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = "0",
                Name = "Built in LPG-OD loading-OD",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropLoadingDiscount_Applicable),
            });

            itemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropLoadingDiscount_Col?.LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description.Replace(" ", "") == "BuiltinLPG-TPLoading-TP");
            premium = !string.IsNullOrEmpty(itemData?.PropLoadingDiscount_EndorsementAmount) ? itemData?.PropLoadingDiscount_EndorsementAmount : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = "0",
                Name = "Built in LPG-TP Loading-TP",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropLoadingDiscount_Applicable),
            });
        }

        if ((string.Equals(quoteQuery.VehicleDetails.Fuel, "CNG", StringComparison.OrdinalIgnoreCase)))
        {
            var itemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropLoadingDiscount_Col?.LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description.Replace(" ", "") == "BuiltinCNG-ODloading-OD");
            premium = !string.IsNullOrEmpty(itemData?.PropLoadingDiscount_EndorsementAmount) ? itemData?.PropLoadingDiscount_EndorsementAmount : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = "0",
                Name = "Built in CNG-OD loading-OD",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropLoadingDiscount_Applicable),
            });

            itemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropLoadingDiscount_Col?.LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description.Replace(" ", "") == "BuiltinCNG-TPLoading-TP");
            premium = !string.IsNullOrEmpty(itemData?.PropLoadingDiscount_EndorsementAmount) ? itemData?.PropLoadingDiscount_EndorsementAmount : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = "0",
                Name = "Built in CNG-TP Loading-TP",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropLoadingDiscount_Applicable),
            });
        }

        if (quoteQuery.Accessories.IsCNG)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "CNG");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "CNG Kit-OD");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover OD",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });

            itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "CNG Kit-TP");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover TP",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        if (quoteQuery.Accessories.IsElectrical)
        {
            var itemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Electrical Accessories");
            premium = !string.IsNullOrEmpty(itemData?.PropRisks_EndorsementAmount) ? itemData?.PropRisks_EndorsementAmount : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.ElectricalId,
                Name = "Electrical Accessory Cover",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        if (quoteQuery.Accessories.IsNonElectrical)
        {
            var itemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Non-Electrical Accessories");
            premium = !string.IsNullOrEmpty(itemData?.PropRisks_EndorsementAmount) ? itemData?.PropRisks_EndorsementAmount : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.NonElectricalId,
                Name = "Non-Electrical Accessory Cover",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        return accessoryList;
    }
    private static List<NameValueModel> SetPACoverResponse(QuoteQueryModel quoteQuery, UnitedIndiaResponseEnvelope response)
    {
        var premium = "0";
        List<NameValueModel> paCoverList = new List<NameValueModel>();
        if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "PA Owner Driver");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                Name = "PA Cover for Owner Driver",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropCoverDetails_Applicable)
            });
        }
        if (quoteQuery.PACover.IsPaidDriver)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "LL to Paid Driver IMT 28");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.PaidDriverId,
                Name = "PA Cover for Paid Driver",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropCoverDetails_Applicable)
            });
        }
        if (quoteQuery.PACover.IsUnnamedPassenger)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Unnamed PA Cover");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups.Equals("Personal Accident Cover-Unnamed") || x.PropCoverDetails_CoverGroups.Equals("Personal accident cover Unnamed"));
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedPassengerId,
                Name = "PA Cover for Unnamed Passengers",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropCoverDetails_Applicable)
            });
        }
        return paCoverList;
    }
    private static List<NameValueModel> SetAddOnsResponse(QuoteQueryModel quoteQuery, UnitedIndiaResponseEnvelope response)
    {
        var premium = "0";
        List<NameValueModel> addOnsList = new List<NameValueModel>();
        if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "Loss Of Key Cover");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                Name = "Key And Lock Protection",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropCoverDetails_Applicable)
            }
            );
        }
        if (quoteQuery.AddOns.IsTyreProtectionRequired && quoteQuery.AddOns.IsRimProtectionRequired && !quoteQuery.VehicleDetails.IsTwoWheeler)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "Tyre And Rim Protector Cover");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";

            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.TyreProtectionId,
                Name = "Tyre Protection",
                Value = RoundOffValue(premium),
                IsApplicable = true
            }
            );

            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RimProtectionId,
                Name = "RIM Protection",
                Value = "Included",
                IsApplicable = true
            });
        }
        else if(!quoteQuery.VehicleDetails.IsTwoWheeler)
        {
            if (quoteQuery.AddOns.IsTyreProtectionRequired)
            {
                var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
                var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "Tyre And Rim Protector Cover");
                premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";

                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.TyreProtectionId,
                    Name = "Tyre Protection",
                    Value = RoundOffValue(premium),
                    IsApplicable = true
                }
                );

                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RimProtectionId,
                    Name = "RIM Protection",
                    Value = "Included",
                    IsApplicable = true
                });
            }
            if (quoteQuery.AddOns.IsRimProtectionRequired)
            {
                var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
                var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "Tyre And Rim Protector Cover");
                premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";

                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RimProtectionId,
                    Name = "RIM Protection",
                    Value = RoundOffValue(premium),
                    IsApplicable = Convert.ToBoolean(itemData?.PropCoverDetails_Applicable)
                }
                );

                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.TyreProtectionId,
                    Name = "Tyre Protection",
                    Value = "Included",
                    IsApplicable = true
                }
               );
            }
        }
        if (quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RoadSideAssistanceId,
                Name = "Road Side Assistance Advance",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsNCBRequired)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "NCB Protect");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.NCBId,
                Name = "No Claim Bonus Protection Protect",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropCoverDetails_Applicable)
            }
            );
        }
        if (quoteQuery.AddOns.IsReturnToInvoiceRequired)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "Return To Invoice");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                Name = "RTI",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropCoverDetails_Applicable)
            }
            );
        }
        if (quoteQuery.AddOns.IsTowingRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.TowingId,
                Name = "Towing Protection",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsLossOfDownTimeRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.LossOfDownTimeId,
                Name = "Loss of Down Time Protection",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsDailyAllowance)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.DailyAllowanceId,
                Name = "Daily Allowance",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsZeroDebt)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "Nil Depreciation Without Excess");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ZeroDebtId,
                Name = "Zero Dep",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropCoverDetails_Applicable)
            }
            );
        }
        if (quoteQuery.AddOns.IsEMIProtectorRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.EMIProtectorId,
                Name = "EMI Protection",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsConsumableRequired)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "Consumables Cover");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ConsumableId,
                Name = "Consumables",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropCoverDetails_Applicable)
            }
            );
        }
        if (quoteQuery.AddOns.IsLimitedOwnPremisesRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.LimitedOwnPremisesId,
                Name = "Limited to Own Premises",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsRoadSideAssistanceRequired)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "Road Side Assistance");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RoadSideAssistanceId,
                Name = "Road Side Assistance",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropCoverDetails_Applicable)
            }
            );
        }
        if (quoteQuery.AddOns.IsPersonalBelongingRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.PersonalBelongingId,
                Name = "Personal Belongings",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsEngineProtectionRequired)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
            var itemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "Engine and Gearbox Protection Standard AddOn Cover");
            premium = !string.IsNullOrEmpty(itemData?.PropCoverDetails_EndorsementAmount) ? itemData?.PropCoverDetails_EndorsementAmount : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.EngineProtectionId,
                Name = "Engine Gearbox Protection",
                Value = RoundOffValue(premium),
                IsApplicable = Convert.ToBoolean(itemData?.PropCoverDetails_Applicable)
            }
            );
        }
        if (quoteQuery.AddOns.IsGeoAreaExtension)
        {
            var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
            var Risks_CoverDetailsitemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "Basic - OD" || x.PropCoverDetails_CoverGroups.Equals("Basic OD"));
            var itemData = Risks_CoverDetailsitemData?.PropCoverDetails_LoadingDiscount_Col?.CoverDetails_LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description == "Geographical Extension - OD");
            var odPremium = !string.IsNullOrEmpty(itemData?.PropLoadingDiscount_EndorsementAmount) ? itemData?.PropLoadingDiscount_EndorsementAmount : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.GeoAreaExtensionId,
                Name = "Geo Area Extension OD",
                Value = RoundOffValue(odPremium),
                IsApplicable = Convert.ToBoolean(itemData?.PropLoadingDiscount_Applicable)
            });

            Risks_CoverDetailsitemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups == "Basic - TP" || x.PropCoverDetails_CoverGroups.Equals("Basic TP"));
            itemData = Risks_CoverDetailsitemData?.PropCoverDetails_LoadingDiscount_Col?.CoverDetails_LoadingDiscount?.Find(x => x.PropLoadingDiscount_Description == "Geographical Extension - TP");
            var tpPremium = !string.IsNullOrEmpty(itemData?.PropLoadingDiscount_EndorsementAmount) ? itemData?.PropLoadingDiscount_EndorsementAmount : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.GeoAreaExtensionId,
                Name = "Geo Area Extension TP",
                Value = RoundOffValue(tpPremium),
                IsApplicable = Convert.ToBoolean(itemData?.PropLoadingDiscount_Applicable)
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
        return addOnsList;
    }
    private static List<NameValueModel> SetBaseCover(string previousPolicy, UnitedIndiaResponseEnvelope response)
    {
        var odPremium = "0";
        var tpPremium = "0";
        List<NameValueModel> baseCoverList = new List<NameValueModel>();

        var RisksitemData = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_PRODUCT_USERDATA.WorkSheet?.PropRisks_Col?.Risks?.Find(x => x.PropRisks_VehicleSIComponent == "Vehicle Base Value");
        var odItemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups.Equals("Basic - OD") || x.PropCoverDetails_CoverGroups.Equals("Basic OD"));
        var tpItemData = RisksitemData?.PropRisks_CoverDetails_Col?.Risks_CoverDetails.Find(x => x.PropCoverDetails_CoverGroups.Equals("Basic - TP") || x.PropCoverDetails_CoverGroups.Equals("Basic TP"));

        odPremium = !string.IsNullOrEmpty(odItemData?.PropCoverDetails_EndorsementAmount) ? odItemData?.PropCoverDetails_EndorsementAmount : "0";
        tpPremium = !string.IsNullOrEmpty(tpItemData?.PropCoverDetails_EndorsementAmount) ? tpItemData?.PropCoverDetails_EndorsementAmount : "0";

        if (previousPolicy.Equals("PackagePolicy") || previousPolicy.Equals("Comprehensive Bundle"))
        {
            baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value = RoundOffValue(odPremium),
                        IsApplicable = IsApplicable(odPremium)
                    },
                    new NameValueModel
                    {
                        Name = "Third Party Cover Premium",
                        Value = RoundOffValue(tpPremium),
                        IsApplicable = IsApplicable(tpPremium)
                    },
                };
        }
        if (previousPolicy.Equals("StandAloneOD"))
        {
            baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value = RoundOffValue(odPremium),
                        IsApplicable = IsApplicable(odPremium)
                    }
                };
        }
        if (previousPolicy.Equals("LiabilityOnly"))
        {
            baseCoverList = new List<NameValueModel>
                {
                   new NameValueModel
                   {
                        Name = "Third Party Cover Premium",
                        Value = RoundOffValue(tpPremium),
                        IsApplicable = IsApplicable(tpPremium)
                   },
                };
        }
        return baseCoverList;
    }
    #endregion Quotation

    #region Proposal
    public async Task<UnitedIndiaQuoteResponseDuringProposal> UpdateUserDetailsInQuotation(QuoteTransactionRequest quoteTransactionRequest, CreateLeadModel createLeadModel, UnitedProposalDynamicDetail proposalDynamicDetails, CancellationToken cancellationToken)
    {
        var updatedQuoteResponse = new UnitedIndiaQuoteResponseDuringProposal();
        var responseBody = string.Empty;
        string requestBody = string.Empty;
        var id = 0;
        ServiceTaxModel tax = new ServiceTaxModel();
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(UIICRequestEnvelopeRequest));

        try
        {
            var content = quoteTransactionRequest.RequestBody;
            content = content.Replace("<![CDATA[", "");
            content = content.Replace("]]>", "");
            quoteTransactionRequest.RequestBody = content;

            StringReader reader = new StringReader(quoteTransactionRequest.RequestBody);
            var requestframing = (UIICRequestEnvelopeRequest)(xmlSerializer.Deserialize(reader));

            var header = requestframing.Body.CalculatePremium.proposalXml.ROOT.HEADER;

            header.TXT_OEM_TRANSACTION_ID = $"UIIC{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            //VehicleDetails
            header.TXT_ENGINE_NUMBER = proposalDynamicDetails.VehicleDetails.engineNumber;
            header.TXT_CHASSIS_NUMBER = proposalDynamicDetails.VehicleDetails.chassisNumber;
            header.NUM_FINANCIER_NAME_1 = proposalDynamicDetails.VehicleDetails.isFinancier.Equals("Yes") ? quoteTransactionRequest.FinancierName : string.Empty;
            header.TXT_FIN_ACCOUNT_CODE_1 = proposalDynamicDetails.VehicleDetails.isFinancier.Equals("Yes") ? proposalDynamicDetails.VehicleDetails.financer : string.Empty;
            header.TXT_FIN_BRANCH_NAME_1 = proposalDynamicDetails.VehicleDetails.isFinancier.Equals("Yes") ? quoteTransactionRequest.FinancierBranch : string.Empty;
            header.TXT_FINANCIER_BRANCH_ADDRESS1 = proposalDynamicDetails.VehicleDetails.isFinancier.Equals("Yes") ? quoteTransactionRequest.FinancierAddress : string.Empty;
            header.NUM_AGREEMENT_NAME_1 = proposalDynamicDetails.VehicleDetails.isFinancier.Equals("Yes") ? "Hypothecation" : string.Empty;

            //PersonalDetails
            header.TXT_NAME_OF_INSURED = createLeadModel.CarOwnedBy.Equals("INDIVIDUAL") ? proposalDynamicDetails.PersonalDetails.customerName : proposalDynamicDetails.PersonalDetails.companyName;
            header.TXT_DOB = createLeadModel.CarOwnedBy.Equals("INDIVIDUAL") ? Convert.ToDateTime(proposalDynamicDetails.PersonalDetails.dateOfBirth).ToString("dd/MM/yyyy").Replace("-", "/") : Convert.ToDateTime(proposalDynamicDetails.PersonalDetails.dateOfIncorporation).ToString("dd/MM/yyyy").Replace("-", "/");
            header.TXT_PAN_NO = proposalDynamicDetails.PersonalDetails.panNumber;
            header.TXT_GENDER = proposalDynamicDetails.PersonalDetails.gender;
            header.TXT_MOBILE = proposalDynamicDetails.PersonalDetails.mobile;
            header.TXT_EMAIL_ADDRESS = proposalDynamicDetails.PersonalDetails.emailId;
            header.TXT_GSTIN_NUMBER = createLeadModel.Equals("INDIVIDUAL") ? string.Empty : proposalDynamicDetails.PersonalDetails.gstno;

            //AddressDetails
            header.MEM_ADDRESS_OF_INSURED = proposalDynamicDetails.AddressDetails.addressLine1;
            header.NUM_PIN_CODE = proposalDynamicDetails.AddressDetails.pincode;

            //NomineeDetails
            header.TXT_RELATION_WITH_NOMINEE = proposalDynamicDetails.NomineeDetails.nomineeRelation;
            header.TXT_NAME_OF_NOMINEE = proposalDynamicDetails.NomineeDetails.nomineeName;

            XmlSerializer xmlSerializerSerialize = new XmlSerializer(typeof(UnitedIndiaRoot));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            StringBuilder requestBuilder = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(requestBuilder, settings);
            xmlSerializerSerialize.Serialize(xw, requestframing.Body.CalculatePremium.proposalXml.ROOT, ns);
            var requestRoot = requestBuilder.ToString();
            _logger.LogInformation("UnitedIndia ConfirmQuote request Root {request}", requestRoot);

            var cDataRequest = "<![CDATA[" + requestRoot + "]]>";

            var request = new UnitedIndiaEnvelope()
            {
                Body = new UnitedIndiaBody()
                {
                    calculatePremium = new CalculatePremium()
                    {
                        application = _unitedIndiaConfig.ApplicationId,
                        userid = _unitedIndiaConfig.UserId,
                        password = _unitedIndiaConfig.Password,
                        productCode = requestframing.Body.CalculatePremium.productCode.ToString(),
                        subproductCode = requestframing.Body.CalculatePremium.subproductCode.ToString(),
                        proposalXml = string.Empty
                    }
                }
            };

            XmlSerializer requestSerializer = new XmlSerializer(typeof(UnitedIndiaEnvelope));
            ns.Add("ws", "http://ws.uiic.com/");
            StringBuilder finalRequestBuilder = new StringBuilder();
            StringWriter requestStringWriter = new StringWriter(finalRequestBuilder);
            requestSerializer.Serialize(requestStringWriter, request, ns);
            requestBody = finalRequestBuilder.ToString();
            requestBody = requestBody.Replace("<proposalXml />", "<proposalXml>" + cDataRequest + "</proposalXml>");
            requestBody = requestBody.Replace("Body", "soapenv:Body");
            requestBody = requestBody.Replace("Envelope", "soapenv:Envelope");
            requestBody = requestBody.Replace("calculatePremium", "ws:calculatePremium");
            requestBody = requestBody.Replace("xmlns", "xmlns:soapenv");
            requestBody = requestBody.Replace("xmlns:soapenv:ws", "xmlns:ws");

            _logger.LogInformation("UnitedIndia Proposa API Quotation request {request}", requestBody);

            var getQuoteResponse = await GetQuoteResponse(createLeadModel.LeadID, requestBody, "Proposal", cancellationToken);

            if (!getQuoteResponse.Item1.IsSuccessStatusCode)
            {
                responseBody = getQuoteResponse.Item1.ReasonPhrase;
                updatedQuoteResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogError("UnitedIndia Proposal API Update Quote Not Found {responseBody}", responseBody);
            }
            else
            {
                id = getQuoteResponse.Item2;
                responseBody = getQuoteResponse.Item1.Content.ReadAsStringAsync().Result.ToString();
                responseBody = responseBody.Replace("&lt;", "<");
                responseBody = responseBody.Replace("&gt;", ">");

                StringReader responseReader = new StringReader(responseBody);
                XmlSerializer responseSerializer = new XmlSerializer(typeof(UnitedIndiaResponseEnvelope));
                var response = (UnitedIndiaResponseEnvelope)responseSerializer.Deserialize(responseReader);

                if (response != null && string.IsNullOrEmpty(response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_ERR_MSG))
                {
                    updatedQuoteResponse.InsurerStatusCode = 200;
                    updatedQuoteResponse.TransactionId = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_TRANSACTION_ID;
                    updatedQuoteResponse.RequestBody = requestBody;
                    updatedQuoteResponse.OEMUniqId = header.TXT_OEM_TRANSACTION_ID;
                }
                else
                {
                    updatedQuoteResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    updatedQuoteResponse.ValidationMessage = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_ERR_MSG;
                }
            }
            await UpdateICLogs(id, string.Empty, responseBody);
            return updatedQuoteResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError("UnitedIndia Proposal API Update Quote Error {exception}", ex.Message);
            updatedQuoteResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return updatedQuoteResponse;
        }
    }

    public async Task<ProposalResponseModel> CreateProposal(string requestBody, string transactionId, string leadId, CancellationToken cancellationToken)
    {
        var responseBody = string.Empty;
        var id = 0;
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(UIICRequestEnvelopeRequest));
        ProposalResponseModel proposalResponseModel = new ProposalResponseModel();
        var proposalVm = new QuoteResponseModel();
        try
        {
            var content = requestBody;
            content = content.Replace("<![CDATA[", "");
            content = content.Replace("]]>", "");
            requestBody = content;

            StringReader reader = new StringReader(requestBody);
            var requestframing = (UIICRequestEnvelopeRequest)(xmlSerializer.Deserialize(reader));
            var header = requestframing?.Body?.CalculatePremium?.proposalXml?.ROOT?.HEADER;
            header.TXT_TRANSACTION_ID = transactionId;

            XmlSerializer xmlSerializerSerialize = new XmlSerializer(typeof(UnitedIndiaRoot));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            StringBuilder requestBuilder = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(requestBuilder, settings);
            xmlSerializerSerialize.Serialize(xw, requestframing.Body.CalculatePremium.proposalXml.ROOT, ns);
            var requestRoot = requestBuilder.ToString();
            _logger.LogInformation("UnitedIndia Proposal request Root {request}", requestRoot);

            var cDataRequest = "<![CDATA[" + requestRoot + "]]>";

            var request = new UnitedIndiaEnvelope()
            {
                Body = new UnitedIndiaBody()
                {
                    calculatePremium = new CalculatePremium()
                    {
                        application = _unitedIndiaConfig.ApplicationId,
                        userid = _unitedIndiaConfig.UserId,
                        password = _unitedIndiaConfig.Password,
                        productCode = requestframing.Body.CalculatePremium.productCode.ToString(),
                        subproductCode = requestframing.Body.CalculatePremium.subproductCode.ToString(),
                        proposalXml = string.Empty
                    }
                }
            };

            XmlSerializer requestSerializer = new XmlSerializer(typeof(UnitedIndiaEnvelope));
            ns.Add("ws", "http://ws.uiic.com/");
            StringBuilder finalRequestBuilder = new StringBuilder();
            StringWriter requestStringWriter = new StringWriter(finalRequestBuilder);
            requestSerializer.Serialize(requestStringWriter, request, ns);
            requestBody = finalRequestBuilder.ToString();
            requestBody = requestBody.Replace("<proposalXml />", "<proposalXml>" + cDataRequest + "</proposalXml>");
            requestBody = requestBody.Replace("Body", "soapenv:Body");
            requestBody = requestBody.Replace("Envelope", "soapenv:Envelope");
            requestBody = requestBody.Replace("calculatePremium", "ws:calculatePremium");
            requestBody = requestBody.Replace("xmlns", "xmlns:soapenv");
            requestBody = requestBody.Replace("xmlns:soapenv:ws", "xmlns:ws");
            requestBody = requestBody.Replace("ws:calculatePremium", "ws:saveProposal");
            proposalResponseModel.RequestBody = requestBody;
            _logger.LogInformation("UnitedIndia create proposal {request}", requestBody);

            var proposalResponse = await GetQuoteResponse(leadId, requestBody, "Proposal", cancellationToken);
            id = proposalResponse.Item2;
            if (!proposalResponse.Item1.IsSuccessStatusCode)
            {
                responseBody = proposalResponse.Item1.ReasonPhrase;
                proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                proposalVm.InsurerName = _unitedIndiaConfig.InsurerName;
                proposalVm.InsurerId = _unitedIndiaConfig.InsurerId;
                proposalVm.InsurerLogo = _unitedIndiaConfig.InsurerLogo;
                _logger.LogError("UnitedIndia Proposal Not Found {responseBody}", responseBody);
            }
            else
            {
                responseBody = proposalResponse.Item1.Content.ReadAsStringAsync().Result.ToString();
                responseBody = responseBody.Replace("&lt;", "<");
                responseBody = responseBody.Replace("&gt;", ">");

                XmlSerializer responseXMLSerializer = new XmlSerializer(typeof(ProposalResponseEnvelope));
                StringReader responseReader = new StringReader(responseBody);
                var response = (ProposalResponseEnvelope)responseXMLSerializer.Deserialize(responseReader);

                if (response != null && string.IsNullOrEmpty(response?.Body?.SaveProposalResponse?.@return?.ROOT?.HEADER?.TXT_ERR_MSG))
                {
                    string totalTax = RoundOffValue(response?.Body?.SaveProposalResponse?.@return?.ROOT?.HEADER?.CUR_FINAL_SERVICE_TAX);
                    var tax = new ServiceTax
                    {
                        totalTax = totalTax
                    };

                    proposalVm = new QuoteResponseModel
                    {
                        InsurerName = _unitedIndiaConfig.InsurerName,
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        Tax = tax,
                        TotalPremium = RoundOffValue(response?.Body?.SaveProposalResponse?.@return?.ROOT?.HEADER?.CUR_NET_FINAL_PREMIUM),
                        GrossPremium = RoundOffValue(response?.Body?.SaveProposalResponse?.@return?.ROOT?.HEADER?.CUR_FINAL_TOTAL_PREMIUM),
                        TransactionID = response?.Body?.SaveProposalResponse?.@return?.ROOT?.HEADER?.TXT_TRANSACTION_ID,
                        ApplicationId = response?.Body?.SaveProposalResponse?.@return?.ROOT?.HEADER?.TXT_TRANSACTION_ID,
                        CustomerId = response?.Body?.SaveProposalResponse?.@return?.ROOT?.HEADER?.TXT_CUSTOMER_ID,
                        ProposalNumber = response?.Body?.SaveProposalResponse?.@return?.ROOT?.HEADER?.NUM_REFERENCE_NUMBER
                    };
                }
                else
                {
                    proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    proposalVm.InsurerName = _unitedIndiaConfig.InsurerName;
                    proposalVm.ValidationMessage = response?.Body?.SaveProposalResponse?.@return?.ROOT?.HEADER?.TXT_ERR_MSG;
                    _logger.LogError("UnitedIndia Proposal Not Found {responseBody}", responseBody);
                }
            }
            proposalResponseModel.ResponseBody = responseBody;
            proposalResponseModel.quoteResponseModel = proposalVm;
            await UpdateICLogs(proposalResponse.Item2, transactionId, responseBody);
            return proposalResponseModel;
        }
        catch (Exception ex)
        {
            _logger.LogError("UnitedIndia Proposal API Error {exception}", ex.Message);
            proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            proposalVm.InsurerName = _unitedIndiaConfig.InsurerName;
            proposalVm.InsurerId = _unitedIndiaConfig.InsurerId;
            proposalVm.InsurerLogo = _unitedIndiaConfig.InsurerLogo;
            proposalResponseModel.quoteResponseModel = proposalVm;
            await UpdateICLogs(id, transactionId, responseBody);
            return proposalResponseModel;
        }
    }
    #endregion Proposal

    #region CKYC
    public async Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(CKYCModel cKYCModel, UnitedProposalDynamicDetail unitedProposalDynamicDetail, CancellationToken cancellationToken)
    {
        string responseBody = string.Empty;
        string requestBody = string.Empty;
        SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();

        CreateLeadModel createLeadModel = new CreateLeadModel();
        UnitedIndiaCkycFetchRequest unitedIndiaCKYCFetchRequest = new();
        try
        {
            unitedIndiaCKYCFetchRequest.oem_unique_identifier = cKYCModel.TransactionId;
            unitedIndiaCKYCFetchRequest.customer_type = cKYCModel.CustomerType.ToUpper();
            unitedIndiaCKYCFetchRequest.customer_name = cKYCModel.CustomerType.Equals("I") ? unitedProposalDynamicDetail?.PersonalDetails?.customerName : unitedProposalDynamicDetail?.PersonalDetails?.companyName;
            unitedIndiaCKYCFetchRequest.email = unitedProposalDynamicDetail?.PersonalDetails?.emailId;
            unitedIndiaCKYCFetchRequest.mobile_no = unitedProposalDynamicDetail?.PersonalDetails?.mobile;
            unitedIndiaCKYCFetchRequest.pincode = unitedProposalDynamicDetail?.AddressDetails?.pincode;
            if (cKYCModel.CustomerType.Equals("I"))
            {
                unitedIndiaCKYCFetchRequest.gender = unitedProposalDynamicDetail.PersonalDetails.gender.Equals("Male") ? "M" : "F";
            }
            else 
            {
                unitedIndiaCKYCFetchRequest.gender = string.Empty;
            }
            unitedIndiaCKYCFetchRequest.pan = unitedProposalDynamicDetail?.PersonalDetails?.panNumber;
            unitedIndiaCKYCFetchRequest.dob = cKYCModel.CustomerType.Equals("I") ? Convert.ToDateTime(unitedProposalDynamicDetail?.PersonalDetails?.dateOfBirth).ToString("dd/MM/yyyy").Replace("-", "/") : Convert.ToDateTime(unitedProposalDynamicDetail?.PersonalDetails?.dateOfIncorporation).ToString("dd/MM/yyyy").Replace("-", "/");
            unitedIndiaCKYCFetchRequest.tieup_name = "UIWBGENAGG";
            unitedIndiaCKYCFetchRequest.aadhar_last_four_digits = "";
            unitedIndiaCKYCFetchRequest.ckyc_no = "";
            unitedIndiaCKYCFetchRequest.transactionid = "";
            unitedIndiaCKYCFetchRequest.redirecturl = $"{_unitedIndiaConfig.CKYCRedirectURL}/{cKYCModel.QuotetransactionId}/{_applicationClaims.GetUserId()}";


            //Only With PAN so Commented Aadhar 
            //switch (cKYCModel?.DocumentType)
            //{
            //    case ("Pan"):
            //        unitedIndiaCKYCFetchRequest.pan = cKYCModel.DocumentId.ToUpper();
            //        break;
            //    case ("Aadhaar"):
            //        unitedIndiaCKYCFetchRequest.aadhar_last_four_digits = (cKYCModel.DocumentId).Substring(Math.Max(0, cKYCModel.DocumentId.Length - 4)).ToUpper();
            //        break;
            //    default:
            //        break;
            //}

            var id = 0;
            string url = _unitedIndiaConfig.BaseURL + _unitedIndiaConfig.FetchKYCURL;
            var client = new RestClient(url);
            var request = new RestRequest(string.Empty, Method.Post);
            request.AddHeader("userid", _unitedIndiaConfig.CKYC_UserId);
            request.AddHeader("password", _unitedIndiaConfig.CKYC_Password);
            request.AddBody(unitedIndiaCKYCFetchRequest);
            var req = JsonConvert.SerializeObject(unitedIndiaCKYCFetchRequest);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            id = await InsertICLogs(string.Empty, cKYCModel?.LeadId, url, _unitedIndiaConfig.CKYC_UserId + "," + _unitedIndiaConfig.CKYC_Password, JsonConvert.SerializeObject(request), "KYC");
            try
            {
                var result = await client.ExecuteAsync(request, cancellationToken);
                requestBody = client.BuildUri(request).ToString();
                _logger.LogInformation("United India POI Request {Request}", requestBody);
                responseBody = result.Content;
                _logger.LogInformation("United India POI Request {Response}", responseBody);


                var response = JsonConvert.DeserializeObject<UnitedIndiaCkycFetchModel>(responseBody);

                if (response is not null && response.kyc_verification_status.Equals("Y") && response.status.Equals("success"))
                {
                    DateTime date = DateTime.ParseExact(response.dob, "dd/MM/yyyy", null);
                    string dob = date.ToString("yyyy-MM-dd");
                    var namesList = response.customer_name.Split(' ');
                    createLeadModel.LeadName = namesList.Length > 0 ? namesList[0] : string.Empty;
                    createLeadModel.LastName = namesList.Length > 1 ? namesList[1] : string.Empty;
                    createLeadModel.MiddleName = namesList.Length > 2 ? namesList[2] : string.Empty;
                    createLeadModel.DOB = dob;
                    createLeadModel.PhoneNumber = response.mobile_no;
                    createLeadModel.Email = response.email;
                    createLeadModel.PANNumber = response.pan;
                    createLeadModel.ckycNumber = response.ckyc_no;
                    createLeadModel.kyc_id = response.ckyc_no;
                    createLeadModel.CKYCstatus = response.status;
                    createLeadModel.PermanentAddress = new LeadAddressModel
                    {
                        AddressType = "PRIMARY",
                        Address1 = response.address1,
                        Address2 = response.address2,
                        Pincode = response.pincode
                    };
                    saveCKYCResponse.Name = response.customer_name;
                    saveCKYCResponse.LastName = namesList.Length > 1 ? namesList[1] : string.Empty;
                    saveCKYCResponse.MiddleName = namesList.Length > 2 ? namesList[2] : string.Empty;
                    saveCKYCResponse.DOB = response.dob;
                    saveCKYCResponse.Address = response.address1 + " , " + response.address2;
                    saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                    saveCKYCResponse.Message = KYC_SUCCESS;
                    saveCKYCResponse.IsKYCRequired = true;
                    saveCKYCResponse.InsurerName = InsurerName;
                    return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
                }
                else if (response is not null && response.kyc_verification_status.Equals("N"))
                {
                    saveCKYCResponse.KYC_Status = POA_REQUIRED;
                    saveCKYCResponse.Message = POA_REQUIRED;
                    saveCKYCResponse.IsKYCRequired = true;
                    saveCKYCResponse.redirect_link = response.url;
                    saveCKYCResponse.InsurerName = InsurerName;
                    return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
                }
                saveCKYCResponse.KYC_Status = FAILED;
                saveCKYCResponse.Message = MESSAGE;
                saveCKYCResponse.InsurerName = InsurerName;
                saveCKYCResponse.IsKYCRequired = true;
                await UpdateICLogs(id, cKYCModel.TransactionId, responseBody);
                return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, cKYCModel.TransactionId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            saveCKYCResponse.KYC_Status = FAILED;
            saveCKYCResponse.Message = MESSAGE;
            saveCKYCResponse.InsurerName = InsurerName;
            _logger.LogError("United India CKYC Error {exception}", ex.Message);
            return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
        }
    }

    #endregion CKYC

    #region Payment


    public async Task<string> InitiatePayment(InitiatePaymentRequestDto InitiatePaymentRequestDtoObj, CancellationToken cancellationToken)
    {
        string PaymentURL = string.Empty;
        HttpResponseMessage paymentResponse;
        InitiatePaymentResponse vInitiatePaymentResponse = new InitiatePaymentResponse();
        var responseBody = string.Empty;
        string requestBody = string.Empty;
        string InitiatePaymentURL = _unitedIndiaConfig.PaymentBaseURL + _unitedIndiaConfig.InitiatePaymentURL;
        int id = 0;
        try
        {
            InitiatePaymentURL = InitiatePaymentURL + "?mid=" + _unitedIndiaConfig.MID + "&orderId=" + InitiatePaymentRequestDtoObj.orderId;
            InitiatePaymentRequest InitiatePaymentRequestObj = new InitiatePaymentRequest();
            InitiatePaymentRequestObj.body = new Domain.UnitedIndia.Body()
            {
                requestType = _unitedIndiaConfig.PaymentRequestType,
                mid = _unitedIndiaConfig.MID,
                websiteName = _unitedIndiaConfig.WebsiteForWeb,//_unitedIndiaConfig.WebsiteName,
                orderId = InitiatePaymentRequestDtoObj.orderId, // Transaction Id from Proposal Response
                txnAmount = new Domain.UnitedIndia.Txnamount()
                {
                    currency = _unitedIndiaConfig.Currency,
                    value = InitiatePaymentRequestDtoObj.txnAmount
                },
                userInfo = InitiatePaymentRequestDtoObj.userInfo,
                callbackUrl = $"{_unitedIndiaConfig.PGStatuscallbackUrl}/{InitiatePaymentRequestDtoObj.QuoteTransactionId}/{_applicationClaims.GetUserId()}/{InitiatePaymentRequestDtoObj.num_reference_number}"
            };

            string paytmChecksum = Checksum.generateSignature(JsonConvert.SerializeObject(InitiatePaymentRequestObj.body), _unitedIndiaConfig.MerchantKEY);
            //bool verifySignature = Checksum.verifySignature(JsonConvert.SerializeObject(InitiatePaymentRequestObj.body), _unitedIndiaConfig.MerchantKEY, paytmChecksum);
            InitiatePaymentRequestObj.head = new Domain.UnitedIndia.Head()
            {
                signature = paytmChecksum,
            };

            requestBody = JsonConvert.SerializeObject(InitiatePaymentRequestObj);
            _logger.LogInformation("UnitedIndia Initiate Payment {paymentRequest}", requestBody);
            id = await InsertICLogs(requestBody, InitiatePaymentRequestDtoObj.LeadId, InitiatePaymentURL, string.Empty, string.Empty, "Payment");

            paymentResponse = await _client.PostAsync(InitiatePaymentURL, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken);

            if (!paymentResponse.IsSuccessStatusCode)
            {
                var stream = await paymentResponse.Content.ReadAsStreamAsync(cancellationToken);
                var result = stream.DeserializeFromJson<InitiatePaymentResponse>();
                vInitiatePaymentResponse = result;
                responseBody = JsonConvert.SerializeObject(result);
                _logger.LogError("Unable to Initiate Payment {paymentResponse}", paymentResponse);
            }
            else
            {
                var stream = await paymentResponse.Content.ReadAsStreamAsync(cancellationToken);
                var result = stream.DeserializeFromJson<InitiatePaymentResponse>();
                vInitiatePaymentResponse = result;
                responseBody = JsonConvert.SerializeObject(result);
                _logger.LogInformation("UnitedIndia Initiate Payment {responseBody}", responseBody);
                if (result != null && result.body?.resultInfo.resultStatus == "S")
                {
                    //?orderId = GR202311200001945951 & token = 7baa7ff8412b455a957394b4fb1a5ee01700640135424 & amount = 1.00
                    PaymentURL = _unitedIndiaConfig.PGSubmitPayment + "?orderId=" + InitiatePaymentRequestDtoObj.orderId + "&token=" + result.body.txnToken + "&amount=" + InitiatePaymentRequestDtoObj.txnAmount;
                    _logger.LogInformation("UnitedIndia Payment Link {paymentResponse}", PaymentURL);
                }
            }
            await UpdateICLogs(id, InitiatePaymentRequestDtoObj.transaction_id, responseBody);
            return PaymentURL;
        }
        catch (Exception ex)
        {
            _logger.LogError("UnitedIndia Initiate Payment {exception}", ex.Message);
            await UpdateICLogs(id, string.Empty, ex.Message);
            return PaymentURL;
        }
    }

    public async Task<PaymentstausResponse> GetPaymentStatus(InitiatePaymentRequestDto InitiatePaymentRequest, CancellationToken cancellationToken)
    {
        Dictionary<string, string> body = new Dictionary<string, string>();
        Dictionary<string, string> head = new Dictionary<string, string>();
        Dictionary<string, Dictionary<string, string>> requestBody = new Dictionary<string, Dictionary<string, string>>();

        body.Add("mid", _unitedIndiaConfig.MID);
        body.Add("orderId", InitiatePaymentRequest.orderId);
        string paytmChecksum = Checksum.generateSignature(JsonConvert.SerializeObject(body), _unitedIndiaConfig.MerchantKEY);

        head.Add("signature", paytmChecksum);

        requestBody.Add("body", body);
        requestBody.Add("head", head);

        string post_data = JsonConvert.SerializeObject(requestBody);

        string url = _unitedIndiaConfig.PaymentBaseURL + _unitedIndiaConfig.VerifyPaymentStatusPath;//"https://securegw-stage.paytm.in/v3/order/status";

        HttpResponseMessage quoteResponse;
        quoteResponse = await _client.PostAsync(url, new StringContent(post_data, Encoding.UTF8, "application/json"));
        try
        {
            if (quoteResponse.IsSuccessStatusCode)
            {
                string responseContent = await quoteResponse.Content.ReadAsStringAsync();

                PaymentstausResponse paymentStatus = JsonConvert.DeserializeObject<PaymentstausResponse>(responseContent);
                return paymentStatus;
            }
            throw new Exception("something went wrong.");
        }
        catch (Exception ex)
        {
            throw;
        }

    }

    public async Task<UnitedIndiaPaymentInfoResponseEnvelope> GetPaymentInfo(UIICPaymentInfoHEADER requestDto, string LeadId, CancellationToken cancellationToken)
    {
        UnitedIndiaPaymentInfoResponseEnvelope responseEnvelope = new UnitedIndiaPaymentInfoResponseEnvelope();
        var responseBody = string.Empty;
        string requestBody = string.Empty;
        var id = 0;
        try
        {
            var requestFraming = new UIICPaymentInfoROOT()
            {
                HEADER = new UIICPaymentInfoHEADER()
                {
                    NUM_PREMIUM_AMOUNT = requestDto.NUM_PREMIUM_AMOUNT,
                    DAT_UTR_DATE = requestDto.DAT_UTR_DATE,
                    NUM_REFERENCE_NUMBER = requestDto.NUM_REFERENCE_NUMBER,
                    NUM_UTR_PAYMENT_AMOUNT = requestDto.NUM_UTR_PAYMENT_AMOUNT,
                    TXT_BANK_CODE = requestDto.TXT_BANK_CODE,
                    TXT_BANK_NAME = requestDto.TXT_BANK_NAME,
                    TXT_MERCHANT_ID = requestDto.TXT_MERCHANT_ID,
                    TXT_TRANSACTION_ID = requestDto.TXT_TRANSACTION_ID,
                    TXT_UTR_NUMBER = requestDto.TXT_UTR_NUMBER
                }
            };
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UIICPaymentInfoROOT));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            StringBuilder requestBuilder = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(requestBuilder, settings);
            xmlSerializer.Serialize(xw, requestFraming, ns);
            var request = requestBuilder.ToString();
            _logger.LogInformation("UnitedIndia PaymentInfo request {request}", request);

            var addingHeader = new UIICPaymentInfoEnvelope()
            {
                Body = new UIICPaymentInfoRequestBody()
                {
                    PaymentInfo = new UIICPaymentInfo()
                    {
                        application = _unitedIndiaConfig.ApplicationId,
                        userid = _unitedIndiaConfig.UserId,
                        password = _unitedIndiaConfig.Password,
                        paymentXml = string.Empty
                    }
                }
            };

            var cDataRequest = "<![CDATA[" + request + "]]>";

            XmlSerializer requestSerializer = new XmlSerializer(typeof(UIICPaymentInfoEnvelope));
            ns.Add("ws", "http://ws.uiic.com/");
            StringBuilder finalRequestBuilder = new StringBuilder();
            StringWriter requestStringWriter = new StringWriter(finalRequestBuilder);
            requestSerializer.Serialize(requestStringWriter, addingHeader, ns);
            requestBody = finalRequestBuilder.ToString();
            requestBody = requestBody.Replace("<paymentXml />", "<paymentXml>" + cDataRequest + "</paymentXml>");
            requestBody = requestBody.Replace("<Body>", "<soapenv:Body>");
            requestBody = requestBody.Replace("</Body>", "</soapenv:Body>");
            requestBody = requestBody.Replace("<Envelope", "<soapenv:Envelope");
            requestBody = requestBody.Replace("</Envelope", "</soapenv:Envelope");
            requestBody = requestBody.Replace("paymentInfo", "ws:paymentInfo");
            requestBody = requestBody.Replace("xmlns", "xmlns:soapenv");
            requestBody = requestBody.Replace("xmlns:soapenv:ws", "xmlns:ws");

            _logger.LogInformation("UnitedIndia PaymentInfo request {request}", requestBody);

            var getQuoteResponse = await GetQuoteResponse(LeadId, requestBody, "PaymentInfo", cancellationToken);

            if (!getQuoteResponse.Item1.IsSuccessStatusCode)
            {
                responseBody = await getQuoteResponse.Item1.Content.ReadAsStringAsync(cancellationToken);//getQuoteResponse.Item1.ReasonPhrase;
                _logger.LogError("UnitedIndia PaymentInfo Not Found {responseBody}", responseBody);
            }
            else
            {
                responseBody = getQuoteResponse.Item1.Content.ReadAsStringAsync().Result.ToString();
                responseBody = responseBody.Replace("&lt;", "<");
                responseBody = responseBody.Replace("&gt;", ">");

                StringReader responseReader = new StringReader(responseBody);
                XmlSerializer responseSerializer = new XmlSerializer(typeof(UnitedIndiaPaymentInfoResponseEnvelope));
                responseEnvelope = (UnitedIndiaPaymentInfoResponseEnvelope)responseSerializer.Deserialize(responseReader);
                return responseEnvelope;
            }
            await UpdateICLogs(getQuoteResponse.Item2, string.Empty, responseBody);
        }
        catch (Exception ex)
        {
            _logger.LogError("UnitedIndia PaymentInfo Error {exception}", ex.Message);
            return default;
        }
        return responseEnvelope;
    }
    #endregion Payment

    private static string RoundOffValue(string value)
    {
        if (!string.IsNullOrEmpty(value) && value != "-")
        {
            decimal val = Math.Round(Convert.ToDecimal(value));
            return val.ToString();
        }
        return "0";
    }
    private static bool IsApplicable(object _val)
    {
        string val = Convert.ToString(_val);
        return !(string.IsNullOrEmpty(val) || val == "0" || Convert.ToDecimal(val) == 0);
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
            InsurerId = _unitedIndiaConfig.InsurerId,
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

    private static string GetVehicleAge(string registrationDate)
    {
        DateTime regDate = Convert.ToDateTime(registrationDate);
        DateTime CurrentDate = DateTime.Now;

        TimeSpan day = CurrentDate - regDate;
        double year = day.TotalDays / 365.25;
        return Math.Round(year, 1).ToString();
    }


}
