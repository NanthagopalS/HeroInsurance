using Insurance.Core.Contracts.Common;
using Insurance.Core.Features.HDFC.Command.CKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.HDFC;
using Insurance.Domain.ICICI;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Xml.Serialization;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Persistence.ICIntegration.Implementation;
public class HDFCService : IHDFCService
{
    private readonly HttpClient _client;
    private readonly ILogger<HDFCService> _logger;
    private readonly HDFCConfig _hdfcConfig;
    private readonly IApplicationClaims _applicationClaims;
    private readonly ICommonService _commonService;
    private readonly VehicleTypeConfig _vehicleTypeConfig;
    private readonly PolicyTypeConfig _policyTypeConfig;
    private const string KYC_SUCCESS = "KYC_SUCCESS";
    private const string POA_SUCCESS = "POA_SUCCESS";
    private const string FAILED = "FAILED";
    private const string POA_REQUIRED = "POA_REQUIRED";
    private const string MESSAGE = "Please enter correct document number or proceed with other insurer";
    private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";
    private const string BreakInMessage = "Insurer is currently not providing break-in services. Please retry with any other insurance partner. Please contact support for any assistance.";
    private const string InsurerName = "HDFC ERGO";

    public HDFCService(ILogger<HDFCService> logger,
        HttpClient client,
        IOptions<HDFCConfig> options,
        IApplicationClaims applicationClaims,
        IOptions<VehicleTypeConfig> vehicleTypeConfig,
        IOptions<PolicyTypeConfig> policyTypeConfig,
        ICommonService commonService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _hdfcConfig = options.Value;
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        _vehicleTypeConfig = vehicleTypeConfig.Value;
        _policyTypeConfig = policyTypeConfig.Value;
        _commonService = commonService;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel query, CancellationToken cancellationToken)
    {
        var dateTime = DateTime.Now.AddDays(-2);
        _logger.LogInformation("HDFC GetQuote RegDate {RegDate}", dateTime);
        string regDate = query.IsBrandNewVehicle ? $"{dateTime:dd}/{dateTime:MM}/{query.RegistrationYear}" : query.RegistrationDate;
        _logger.LogInformation("HDFC GetQuote RegDate {RegDate}", regDate);

        var quoteVm = new QuoteResponseModel();
        string requestBody = string.Empty;
        string tpStartDate = null;
        string tpEndDate = null;
        string tpInsurer = null;
        string tpPolicyNumber = null;
        var responseBody = string.Empty;
        try
        {
            _logger.LogInformation("HDFC Service Start");
            quoteVm.InsurerName = "HDFC";
            string policyEndDate = DateTime.ParseExact(query.PolicyStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddYears(1).AddDays(-1).ToString("dd/MM/yyyy");
            string proposalDate = DateTime.Now.ToString("dd/MM/yyyy");
            
            Req_Pvtcar req_PvtCar = null;

            if (query.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
            {
                req_PvtCar = GetPvtCarDetails(query);
            }
            Req_TW req_TW = null;

            if (query.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
            {
                req_TW = GetTWDetails(query);
            }

            Req_PCV req_PCV = null;
            Req_GCV req_GCV = null;
            Req_MISD req_MISD = null;
            if (query.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
            {
                if (query.CategoryId.Equals("1"))
                    req_GCV = GetGCVDetails(query);
                else if (query.CategoryId.Equals("2"))
                    req_PCV = GetPCVDetails(query);
                else if(query.CategoryId.Equals("3"))
                    req_MISD = GetMISDDetails(query);
            }

            if (!query.IsBrandNewVehicle && !query.PolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive))
            {
                tpStartDate = query.PreviousPolicyDetails.PreviousPolicyStartDateSATP;
                tpEndDate = query.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP;
                tpInsurer = query.PreviousPolicyDetails?.PreviousInsurerCode;
                tpPolicyNumber = query.PreviousPolicyDetails?.PreviousPolicyNumber;
            }
            var hdfcrequest = new HDFCServiceRequestModel()
            {
                ApplicationNumber = null,
                TransactionID = query.TransactionId,
                GoGreen = false,
                IsReadyToWait = null,
                PolicyCertifcateNo = null,
                PolicyNo = null,
                Proposal_no = null,
                Inward_no = null,
                Request_IP = null,
                Customer_Details = null,

                Policy_Details = new Policy_Details()
                {
                    PolicyStartDate = query.PolicyStartDate,
                    ProposalDate = proposalDate,
                    PolicyEndDate = query.IsBrandNewVehicle ? policyEndDate : query.PolicyEndDate,
                    AgreementType = "",
                    FinancierCode = null,
                    BranchName = null,
                    PreviousPolicy_CorporateCustomerId_Mandatary = "",
                    PreviousPolicy_NCBPercentage = query.IsBrandNewVehicle ? 0 : Convert.ToInt32(query.PreviousPolicyDetails?.PreviousNoClaimBonus),
                    PreviousPolicy_PolicyEndDate = query.IsBrandNewVehicle ? null : query.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD,
                    PreviousPolicy_PolicyStartDate = query.IsBrandNewVehicle ? null : query.PreviousPolicyDetails.PreviousPolicyStartDateSAOD,
                    PreviousPolicy_PolicyNo = query.IsBrandNewVehicle ? null : query.PreviousPolicyDetails?.PreviousPolicyNumber,
                    PreviousPolicy_PolicyClaim = query.IsBrandNewVehicle ? null : query.PreviousPolicyDetails.IsClaimInLastYear ? "YES" : "NO",
                    PreviousPolicy_PreviousPolicyType = query.IsBrandNewVehicle ? null : query.PreviousPolicyDetails?.PreviousPolicyType,
                    PreviousPolicy_TPENDDATE = tpEndDate,
                    PreviousPolicy_TPSTARTDATE = tpStartDate,
                    PreviousPolicy_TPINSURER = tpInsurer,
                    PreviousPolicy_TPPOLICYNO = tpPolicyNumber,
                    PreviousPolicy_IsRTI_Cover = true,
                    PreviousPolicy_IsZeroDept_Cover = true,
                    BusinessType_Mandatary = query.IsBrandNewVehicle ? "New Vehicle" : "Roll Over",
                    VehicleModelCode = query.VehicleDetails.VehicleModelCode,
                    DateofDeliveryOrRegistration = regDate,
                    DateofFirstRegistration = regDate,
                    YearOfManufacture = query.RegistrationYear,
                    Registration_No = null,
                    EngineNumber = query.VehicleDetails.EngineNumber,
                    ChassisNumber = query.VehicleDetails.Chassis,
                    RTOLocationCode = query.RTOLocationCode,
                    Vehicle_IDV = query.PolicyTypeId.Equals(_policyTypeConfig.SATP) ? 0 : Convert.ToDouble(query.IDVValue),
                    EndorsementEffectiveDate = null,
                    SumInsured = 0,//doubt
                    Premium = 0.0,
                    EXEMPTED_KERALA_FLOOD_CESS = null,
                    CUSTOMER_STATE_CD = query.State_Id,
                    TXT_GIR_NO = null,
                    TSECode = null,
                    AVCode = null,
                    SMCode = null,
                    LGCode = null,
                    BankLocation = null,
                    ChannelName = null,
                    SPCode = null,
                    BANK_BRANCH_ID = null,
                    AV_SP_Code = null,
                    PB_Code = null,
                    Lead_ID = null,
                    AutoRenewal = null,
                    Type_of_payment = null,
                    PolicyType = null,//doubt
                    FamilyType = null,
                    TypeofPlan = null,
                    PolicyTenure = 0,//doubt
                    Deductible = 0.0
                },
                Req_GCV = req_GCV,
                Req_MISD = req_MISD,
                Req_PCV = req_PCV,
                Payment_Details = null,
                IDV_DETAILS = null,
                Req_ExtendedWarranty = null,
                Req_Policy_Document = null,
                Req_PEE = null,
                Req_TW = req_TW,
                Req_RE = null,
                Req_Fire2111 = null,
                Req_ClaimIntimation = null,
                Req_ClaimStatus = null,
                Req_Renewal = null,
                Req_PvtCar = req_PvtCar,
                Req_HInsurance = null,
                Req_IPA = null,
                Req_CI = null,
                Req_HomeInsurance = null,
                Req_RetailTravel = null,
                Req_HCA = null,
                Req_HF = null,
                Req_HI = null,
                Req_HSTPI = null,
                Req_HSTPF = null,
                Req_ST = null,
                Req_WC = null,
                Req_BSC = null,
                Req_Discount = null,
                Req_POSP = null,
                Req_HSF = null,
                Req_HSI = null,
                Req_CustDec = null,
                Req_TW_Multiyear = null,
                Req_OptimaRestore = null,
                Req_Aviation = null,
                Req_NE = null,
                Req_TravelXDFD = null,
                Req_OptimaSenior = null,
                Req_Energy = null,
                Req_HW = null,
                Req_EH = null,
                Req_Ican = null,
                Req_GetStatus = null,
                Request_UploadDocument = null,
                Req_PolicyDetails = null,
                Req_AMIPA = null,
                Req_PolicyStatus = null,
                Req_MasterData = null,
                Req_ChequeDetails = null,
                Req_appstatus = null,
                Req_OptimaSuper = null,
                PaymentStatusDetails = null,
                Req_PospCodeStatus = null,
                Req_TvlSportify = null,
                Request_Data_OS = null,
                Req_GHCIP = null,
                Req_PolicyConfirmation = null,
                Req_MarineOpen = null,
                Req_CyberSachet = null
            };
            return await QuoteResponseFraming(hdfcrequest, query, quoteVm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("HDFC Error {exception}", ex.Message);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return Tuple.Create(quoteVm, requestBody, responseBody);
        }
    }
    private async Task<HttpResponseMessage> GetQuoteResponse(bool isProposal, bool isCommercial, string leadId, HDFCServiceRequestModel hdfcrequest, string token, string productCode, string stage, CancellationToken cancellationToken)
    {
        HttpResponseMessage responseMessage = new HttpResponseMessage();
        var defaultRequestHeaders = _client.DefaultRequestHeaders;
        var id = 0;
        defaultRequestHeaders.Clear();
        defaultRequestHeaders.Add("SOURCE", _hdfcConfig.CommanHeaderData.SOURCE);
        defaultRequestHeaders.Add("CHANNEL_ID", _hdfcConfig.CommanHeaderData.CHANNEL_ID);
        defaultRequestHeaders.Add("PRODUCT_CODE", productCode);
        defaultRequestHeaders.Add("Token", token);
        string requestBody = JsonConvert.SerializeObject(hdfcrequest);
        try
        {
            if (isProposal)
            {
                if(isCommercial)
                {
                    id = await InsertICLogs(requestBody, leadId, _hdfcConfig.CommercialProposalURL, token, JsonConvert.SerializeObject(defaultRequestHeaders), stage);
                    responseMessage = await _client.PostAsJsonAsync(_hdfcConfig.CommercialProposalURL, hdfcrequest, cancellationToken: cancellationToken);
                }
                else
                {
                    id = await InsertICLogs(requestBody, leadId, _hdfcConfig.BaseURL + _hdfcConfig.ProposalURL, token, JsonConvert.SerializeObject(defaultRequestHeaders), stage);
                    responseMessage = await _client.PostAsJsonAsync(_hdfcConfig.ProposalURL, hdfcrequest, cancellationToken: cancellationToken);
                }
            }
            else
            {
                id = await InsertICLogs(requestBody, leadId, _hdfcConfig.BaseURL + _hdfcConfig.QuoteURL, token, JsonConvert.SerializeObject(defaultRequestHeaders), stage);
                responseMessage = await _client.PostAsJsonAsync(_hdfcConfig.QuoteURL, hdfcrequest, cancellationToken: cancellationToken);
            }
            string responseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            UpdateICLogs(id, hdfcrequest?.ApplicationNumber, responseBody);
            return responseMessage;
        }
        catch (Exception ex)
        {
            UpdateICLogs(id, hdfcrequest?.ApplicationNumber, ex.Message);
            return default;
        }
    }
    private async Task<Tuple<QuoteResponseModel, string, string>> QuoteResponseFraming(HDFCServiceRequestModel hdfcrequest, QuoteQueryModel quoteQuery, QuoteResponseModel quoteVm, CancellationToken cancellationToken)
    {
        string requestBody = JsonConvert.SerializeObject(hdfcrequest);
        string responseBody = string.Empty;

        var responseMessage = await GetQuoteResponse(false, quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical), quoteQuery.LeadId, hdfcrequest, quoteQuery.Token, quoteQuery.ProductCode, "Quote", cancellationToken);

        _logger.LogInformation("HDFC Service Request {request}", requestBody);

        if (!responseMessage.IsSuccessStatusCode)
        {
            var streamResponse = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            var result = streamResponse.DeserializeFromJson<HDFCServiceResponseModel>();
            responseBody = JsonConvert.SerializeObject(result);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            _logger.LogError("Data not found {responseBody}", responseBody);
        }
        else
        {
            var streamResponse = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            var result = streamResponse.DeserializeFromJson<HDFCServiceResponseModel>();
            responseBody = JsonConvert.SerializeObject(result);
            _logger.LogInformation("HDFC Response {responseBody}", responseBody);
            if (result != null && result.StatusCode == 200)
            {
                quoteVm.InsurerStatusCode = (int)HttpStatusCode.OK;
                List<NameValueModel> accessoriesCover = new List<NameValueModel>();
                List<NameValueModel> discountCover = new List<NameValueModel>();
                List<NameValueModel> paCover = new List<NameValueModel>();
                List<NameValueModel> addOnCover = new List<NameValueModel>();
                List<NameValueModel> setBaseCover = new List<NameValueModel>();

                if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                {
                    accessoriesCover = SetAccessoriesCoverCommercial(quoteQuery, result);
                    discountCover = SetDiscountCoverCommercial(quoteQuery, result);
                    paCover = SetPACoverCommercial(quoteQuery, result);
                    addOnCover = SetAddonsCoverCommercial(quoteQuery, result);
                    setBaseCover = SetBaseCoverCommercial(quoteQuery.PreviousPolicyDetails.OriginalPreviousPolicyType, quoteQuery.CategoryId, result);
                }
                else
                {
                    accessoriesCover = SetAccessoriesCover(quoteQuery, result);
                    discountCover = SetDiscountCover(quoteQuery, result);
                    paCover = SetPACover(quoteQuery, result);
                    addOnCover = SetAddonsCover(quoteQuery, result);
                    setBaseCover = SetBaseCover(quoteQuery.PreviousPolicyDetails.OriginalPreviousPolicyType, quoteQuery.VehicleTypeId, result);
                }
                
                double ncbPercentage = 0;
                if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
                {

                    if (result.Resp_PvtCar.NCBBonusDisc_Premium > 0)
                    {
                        ncbPercentage = (result.Resp_PvtCar.NCBBonusDisc_Premium / ((result.Resp_PvtCar.Basic_OD_Premium + result.Resp_PvtCar.Electical_Acc_Premium + result.Resp_PvtCar.NonElectical_Acc_Premium + result.Resp_PvtCar.BiFuel_Kit_OD_Premium + result.Resp_PvtCar.BreakIN_Premium + result.Resp_PvtCar.GeogExtension_ODPremium) - (result.Resp_PvtCar.Automobile_Disc_premium + result.Resp_PvtCar.VoluntartDisc_premium))) * 100;
                    }
                    quoteVm = new QuoteResponseModel()
                    {
                        InsurerName = "HDFC",
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        TotalPremium = Math.Round(result.Resp_PvtCar.Net_Premium).ToString(),
                        GrossPremium = Math.Round(result.Resp_PvtCar.Total_Premium).ToString(),
                        SelectedIDV = quoteQuery.SelectedIDV,
                        IDV = Convert.ToDecimal(Math.Round(quoteQuery.RecommendedIDV)),
                        MinIDV = Convert.ToDecimal(Math.Round(quoteQuery.MinIDV)),
                        MaxIDV = Convert.ToDecimal(Math.Round(quoteQuery.MaxIDV)),
                        NCB = Convert.ToDecimal(Math.Round(ncbPercentage)).ToString(),
                        Tax = new ServiceTax
                        {
                            totalTax = Math.Round(result.Resp_PvtCar.Service_Tax).ToString()
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
                        RTOCode = quoteQuery.VehicleDetails.RegNo,
                        TransactionID = result.TransactionID,
                        PolicyStartDate = DateTime.ParseExact(quoteQuery.PolicyStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                        Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                        PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
                        IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                        IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                        RegistrationDate = DateTime.ParseExact(quoteQuery.RegistrationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                        ManufacturingDate = DateTime.ParseExact(quoteQuery.RegistrationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                        VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.VehicleDetails.LicensePlateNumber : quoteQuery.VehicleNumber
                    };
                }
                else if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
                {
                    if (result.Resp_TW.NCBBonusDisc_Premium > 0)
                    {
                        ncbPercentage = (result.Resp_TW.NCBBonusDisc_Premium / ((result.Resp_TW.Basic_OD_Premium + result.Resp_TW.Electical_Acc_Premium + result.Resp_TW.NonElectical_Acc_Premium + result.Resp_TW.BiFuel_Kit_OD_Premium + result.Resp_TW.GeogExtension_ODPremium) - (result.Resp_TW.Automobile_Disc_premium + result.Resp_TW.VoluntartDisc_premium))) * 100;
                    }
                    quoteVm = new QuoteResponseModel()
                    {
                        InsurerName = "HDFC",
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        TotalPremium = Math.Round(result.Resp_TW.Net_Premium).ToString(),
                        GrossPremium = Math.Round(result.Resp_TW.Total_Premium).ToString(),
                        SelectedIDV = quoteQuery.SelectedIDV,
                        IDV = Convert.ToDecimal(Math.Round(quoteQuery.RecommendedIDV)),
                        MinIDV = Convert.ToDecimal(Math.Round(quoteQuery.MinIDV)),
                        MaxIDV = Convert.ToDecimal(Math.Round(quoteQuery.MaxIDV)),
                        NCB = Convert.ToDecimal(Math.Round(ncbPercentage)).ToString(),
                        Tax = new ServiceTax
                        {
                            totalTax = Math.Round(result.Resp_TW.Service_Tax).ToString()
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
                        RTOCode = quoteQuery.VehicleDetails.RegNo,
                        TransactionID = result.TransactionID,
                        PolicyStartDate = DateTime.ParseExact(quoteQuery.PolicyStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                        Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                        PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
                        IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                        IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                        RegistrationDate = DateTime.ParseExact(quoteQuery.RegistrationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                        ManufacturingDate = DateTime.ParseExact(quoteQuery.RegistrationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                        VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == string.Empty ? quoteQuery.VehicleDetails.LicensePlateNumber : quoteQuery.VehicleNumber
                    };
                }
                else if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                {
                    var totalPremium = string.Empty;
                    var grossPremium = string.Empty;
                    var totalTax = string.Empty;

                    if (quoteQuery.CategoryId.Equals("2"))
                    {
                        totalPremium = Math.Round(result.Resp_PCV.Net_Premium).ToString();
                        grossPremium = Math.Round(result.Resp_PCV.Total_Premium).ToString();
                        totalTax = Math.Round(result.Resp_PCV.Service_Tax).ToString();
                        if (result.Resp_PCV.NCBBonusDisc_Premium > 0)
                        {
                            ncbPercentage = (result.Resp_PCV.NCBBonusDisc_Premium / ((result.Resp_PCV.Basic_OD_Premium + result.Resp_PCV.Electical_Acc_Premium + result.Resp_PCV.NonElectical_Acc_Premium + result.Resp_PCV.BiFuel_Kit_OD_Premium + result.Resp_PCV.BreakIN_Premium + result.Resp_PCV.GeogExtension_ODPremium))) * 100;
                        }
                    }
                    if (quoteQuery.CategoryId.Equals("1"))
                    {
                        totalPremium = Math.Round(result.Resp_GCV.Net_Premium).ToString();
                        grossPremium = Math.Round(result.Resp_GCV.Total_Premium).ToString();
                        totalTax = Math.Round(result.Resp_GCV.Service_Tax).ToString();
                        if (result.Resp_GCV.NCBBonusDisc_Premium > 0)
                        {
                            ncbPercentage = (result.Resp_GCV.NCBBonusDisc_Premium / ((result.Resp_GCV.Basic_OD_Premium + result.Resp_GCV.Electical_Acc_Premium + result.Resp_GCV.NonElectical_Acc_Premium + result.Resp_GCV.BiFuel_Kit_OD_Premium + result.Resp_GCV.BreakIN_Premium + result.Resp_GCV.GeogExtension_ODPremium))) * 100;
                        }
                    }
                    if (quoteQuery.CategoryId.Equals("3"))
                    {
                        totalPremium = Math.Round(result.Resp_MISD.Net_Premium).ToString();
                        grossPremium = Math.Round(result.Resp_MISD.Total_Premium).ToString();
                        totalTax = Math.Round(result.Resp_MISD.Service_Tax).ToString();
                        if (result.Resp_MISD.NCBBonusDisc_Premium > 0)
                        {
                            ncbPercentage = (result.Resp_MISD.NCBBonusDisc_Premium / ((result.Resp_MISD.Basic_OD_Premium + result.Resp_MISD.Electical_Acc_Premium + result.Resp_MISD.NonElectical_Acc_Premium + result.Resp_MISD.BiFuel_Kit_OD_Premium + result.Resp_MISD.BreakIN_Premium + result.Resp_MISD.GeogExtension_ODPremium))) * 100;
                        }
                    }

                    quoteVm = new QuoteResponseModel()
                    {
                        InsurerName = "HDFC",
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        TotalPremium = totalPremium,
                        GrossPremium = grossPremium,
                        SelectedIDV = quoteQuery.SelectedIDV,
                        IDV = Convert.ToDecimal(Math.Round(quoteQuery.RecommendedIDV)),
                        MinIDV = Convert.ToDecimal(Math.Round(quoteQuery.MinIDV)),
                        MaxIDV = Convert.ToDecimal(Math.Round(quoteQuery.MaxIDV)),
                        NCB = Convert.ToDecimal(Math.Round(ncbPercentage)).ToString(),
                        Tax = new ServiceTax
                        {
                            totalTax = totalTax
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
                        RTOCode = quoteQuery.VehicleDetails.RegNo,
                        TransactionID = result.TransactionID,
                        PolicyStartDate = DateTime.ParseExact(quoteQuery.PolicyStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                        Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                        PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
                        IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                        IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                        RegistrationDate = DateTime.ParseExact(quoteQuery.RegistrationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                        ManufacturingDate = DateTime.ParseExact(quoteQuery.RegistrationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                        VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.VehicleDetails.LicensePlateNumber : quoteQuery.VehicleNumber
                    };
                }
            }
            else
            {
                quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
        return Tuple.Create(quoteVm, requestBody, responseBody);
    }
    private static List<NameValueModel> SetBaseCover(string previousPolicy, string vehicleTypeId, HDFCServiceResponseModel result)
    {
        List<NameValueModel> baseCoverList = new List<NameValueModel>();
        if (vehicleTypeId == "2d566966-5525-4ed7-bd90-bb39e8418f39")
        {
            if (previousPolicy.Equals("Package Comprehensive") || previousPolicy.Equals("Comprehensive Bundle"))
            {
                if (result.Resp_PvtCar.BreakIN_Premium > 0)
                {
                    baseCoverList = new List<NameValueModel>
                    {
                        new NameValueModel
                        {
                            Name = "Basic Own Damage Premium",
                            Value =Math.Round(result.Resp_PvtCar.Basic_OD_Premium).ToString(),
                            IsApplicable=IsApplicable((result.Resp_PvtCar.Basic_OD_Premium).ToString())
                        },
                        new NameValueModel
                        {
                            Name="Third Party Cover Premium",
                            Value=Math.Round(result.Resp_PvtCar.Basic_TP_Premium).ToString(),
                            IsApplicable=IsApplicable((result.Resp_PvtCar.Basic_TP_Premium).ToString())
                        },
                        new NameValueModel
                        {
                            Name="BreakIn Premium",
                            Value=Math.Round(result.Resp_PvtCar.BreakIN_Premium).ToString(),
                            IsApplicable=IsApplicable((result.Resp_PvtCar.BreakIN_Premium).ToString())
                        }
                    };
                }
                else
                {
                    baseCoverList = new List<NameValueModel>
                    {
                        new NameValueModel
                        {
                            Name = "Basic Own Damage Premium",
                            Value =Math.Round(result.Resp_PvtCar.Basic_OD_Premium).ToString(),
                            IsApplicable=IsApplicable((result.Resp_PvtCar.Basic_OD_Premium).ToString())
                        },
                        new NameValueModel
                        {
                            Name="Third Party Cover Premium",
                            Value=Math.Round(result.Resp_PvtCar.Basic_TP_Premium).ToString(),
                            IsApplicable=IsApplicable((result.Resp_PvtCar.Basic_TP_Premium).ToString())
                        }
                    };
                }
            }
            if (previousPolicy.Equals("SAOD"))
            {
                if (result.Resp_PvtCar.BreakIN_Premium > 0)
                {
                    baseCoverList = new List<NameValueModel>
                    {
                        new NameValueModel
                        {
                            Name = "Basic Own Damage Premium",
                            Value =Math.Round(result.Resp_PvtCar.Basic_OD_Premium).ToString(),
                            IsApplicable=IsApplicable((result.Resp_PvtCar.Basic_OD_Premium).ToString())
                        },
                        new NameValueModel
                        {
                            Name="BreakIn Premium",
                            Value=Math.Round(result.Resp_PvtCar.BreakIN_Premium).ToString(),
                            IsApplicable=IsApplicable((result.Resp_PvtCar.BreakIN_Premium).ToString())
                        }
                    };
                }
                else
                {
                    baseCoverList = new List<NameValueModel>
                    {
                        new NameValueModel
                        {
                            Name = "Basic Own Damage Premium",
                            Value =Math.Round(result.Resp_PvtCar.Basic_OD_Premium).ToString(),
                            IsApplicable=IsApplicable((result.Resp_PvtCar.Basic_OD_Premium).ToString())
                        }
                    };
                }
            }
            if (previousPolicy.Equals("SATP"))
            {
                if (result.Resp_PvtCar.BreakIN_Premium > 0)
                {
                    baseCoverList = new List<NameValueModel>
                    {
                    new NameValueModel
                    {
                        Name = "Third Party Cover Premium",
                        Value = Math.Round(result.Resp_PvtCar.Basic_TP_Premium).ToString(),
                        IsApplicable = IsApplicable((result.Resp_PvtCar.Basic_TP_Premium).ToString())
                    },
                    new NameValueModel
                    {
                        Name = "BreakIn Premium",
                        Value = Math.Round(result.Resp_PvtCar.BreakIN_Premium).ToString(),
                        IsApplicable = IsApplicable((result.Resp_PvtCar.BreakIN_Premium).ToString())
                    }
                    };
                }
                else
                {
                    baseCoverList = new List<NameValueModel>
                    {
                        new NameValueModel
                        {
                            Name="Third Party Cover Premium",
                            Value=Math.Round(result.Resp_PvtCar.Basic_TP_Premium).ToString(),
                            IsApplicable=IsApplicable((result.Resp_PvtCar.Basic_TP_Premium).ToString())
                        }
                    };
                }
            }
        }
        else if (vehicleTypeId == "6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056")
        {
            if (previousPolicy.Equals("Package Comprehensive") || previousPolicy.Equals("Comprehensive Bundle"))
            {
                baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value =Math.Round(result.Resp_TW.Basic_OD_Premium).ToString(),
                        IsApplicable=IsApplicable((result.Resp_TW.Basic_OD_Premium).ToString())
                    },
                    new NameValueModel
                    {
                        Name="Third Party Cover Premium",
                        Value=Math.Round(result.Resp_TW.Basic_TP_Premium).ToString(),
                        IsApplicable=IsApplicable((result.Resp_TW.Basic_TP_Premium).ToString())
                    }
                };
            }
            if (previousPolicy.Equals("SAOD"))
            {
                baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value =Math.Round(result.Resp_TW.Basic_OD_Premium).ToString(),
                        IsApplicable=IsApplicable((result.Resp_TW.Basic_OD_Premium).ToString())
                    }
                };
            }
            if (previousPolicy.Equals("SATP"))
            {
                baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name="Third Party Cover Premium",
                        Value=Math.Round(result.Resp_TW.Basic_TP_Premium).ToString(),
                        IsApplicable=IsApplicable((result.Resp_TW.Basic_TP_Premium).ToString())
                    }
                };
            }
        }
        return baseCoverList;
    }
    private static List<NameValueModel> SetPACover(QuoteQueryModel quoteQuery, HDFCServiceResponseModel result)
    {
        var paCover = new List<NameValueModel>();
        if (quoteQuery.VehicleTypeId == "2d566966-5525-4ed7-bd90-bb39e8418f39")
        {
            if (quoteQuery.PACover.IsPaidDriver)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.PaidDriverId,
                    Name = "PA Cover for Paid Driver",
                    Value = Math.Round(result.Resp_PvtCar.PaidDriver_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.PaidDriver_Premium)
                }
                );
            }
            if (quoteQuery.PACover.IsUnnamedPassenger)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedPassengerId,
                    Name = "PA Cover for Unnamed Passengers",
                    Value = Math.Round(result.Resp_PvtCar.UnnamedPerson_premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.UnnamedPerson_premium)
                }
                );
            }
            if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                    Name = "PA Cover for Owner Driver",
                    Value = Math.Round(result.Resp_PvtCar.PAOwnerDriver_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.PAOwnerDriver_Premium)
                });
            }
        }
        else if (quoteQuery.VehicleTypeId == "6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056")
        {
            if (quoteQuery.PACover.IsPaidDriver)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.PaidDriverId,
                    Name = "PA Cover for Paid Driver",
                    Value = Math.Round(result.Resp_TW.PaidDriver_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_TW.PaidDriver_Premium)
                }
                );
            }
            if (quoteQuery.PACover.IsUnnamedPillionRider)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedPillionRiderId,
                    Name = "PA Cover For Unnamed Pillion Rider",
                    Value = Math.Round(result.Resp_TW.UnnamedPerson_premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_TW.UnnamedPerson_premium)
                }
                );
            }
            if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
            {
                paCover.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                    Name = "PA Cover for Owner Driver",
                    Value = Math.Round(result.Resp_TW.PAOwnerDriver_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_TW.PAOwnerDriver_Premium)
                });
            }
        }
        
        return paCover;
    }
    private static List<NameValueModel> SetDiscountCover(QuoteQueryModel quoteQuery, HDFCServiceResponseModel result)
    {
        var discountCover = new List<NameValueModel>();
        if (quoteQuery.VehicleTypeId == "2d566966-5525-4ed7-bd90-bb39e8418f39")
        {
            if (quoteQuery.Discounts.IsLimitedTPCoverage)
            {
                discountCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.LimitedTPCoverageId,
                    Name = "Limited Third Party Coverage",
                    Value = Math.Round(result.Resp_PvtCar.TPPD_premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.TPPD_premium)
                });
            }
            if (quoteQuery.Discounts.IsAAMemberShip)
            {
                discountCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.AAMemberShipId,
                    Name = "AA Membership",
                    Value = Math.Round(result.Resp_PvtCar.Automobile_Disc_premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.Automobile_Disc_premium)
                });
            }
            discountCover.Add(new NameValueModel
            {
                Name = "No Claim Bonus",
                Value = Math.Round(result.Resp_PvtCar.NCBBonusDisc_Premium).ToString(),
                IsApplicable = IsApplicable(result.Resp_PvtCar.NCBBonusDisc_Premium),
            });
        }
        else if (quoteQuery.VehicleTypeId == "6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056")
        {
            if (quoteQuery.Discounts.IsLimitedTPCoverage)
            {
                discountCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.LimitedTPCoverageId,
                    Name = "Limited Third Party Coverage",
                    Value = Math.Round(result.Resp_TW.TPPD_premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_TW.TPPD_premium)
                });
            }
            if (quoteQuery.Discounts.IsAAMemberShip)
            {
                discountCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.AAMemberShipId,
                    Name = "AA Membership",
                    Value = Math.Round(result.Resp_TW.Automobile_Disc_premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_TW.Automobile_Disc_premium)
                });
            }
            discountCover.Add(new NameValueModel
            {
                Name = "No Claim Bonus",
                Value = Math.Round(result.Resp_TW.NCBBonusDisc_Premium).ToString(),
                IsApplicable = IsApplicable(result.Resp_TW.NCBBonusDisc_Premium),
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
    private static List<NameValueModel> SetAccessoriesCover(QuoteQueryModel quoteQuery, HDFCServiceResponseModel result)
    {
        var accessoriesCover = new List<NameValueModel>();
        if (quoteQuery.VehicleTypeId == "2d566966-5525-4ed7-bd90-bb39e8418f39")
        {
            if (quoteQuery.Accessories.IsCNG)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover OD",
                    Value = Math.Round(result.Resp_PvtCar.BiFuel_Kit_OD_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.BiFuel_Kit_OD_Premium)
                }
                );
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover TP",
                    Value = Math.Round(result.Resp_PvtCar.BiFuel_Kit_TP_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.BiFuel_Kit_TP_Premium)
                }
                );
            }
            if( quoteQuery.VehicleDetails.Fuel.Equals("CNG") || quoteQuery.VehicleDetails.Fuel.Equals("LPG"))
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Inbuilt Accessory Cover",
                    Value = Math.Round(result.Resp_PvtCar.InBuilt_BiFuel_Kit_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.InBuilt_BiFuel_Kit_Premium)
                }
                );
            }
            if (quoteQuery.Accessories.IsElectrical)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.ElectricalId,
                    Name = "Electrical Accessory Cover",
                    Value = Math.Round(result.Resp_PvtCar.Electical_Acc_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.Electical_Acc_Premium)
                });
            }
            if (quoteQuery.Accessories.IsNonElectrical)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.NonElectricalId,
                    Name = "Non-Electrical Accessory Cover",
                    Value = Math.Round(result.Resp_PvtCar.NonElectical_Acc_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.NonElectical_Acc_Premium)
                });
            }
        }
        else if (quoteQuery.VehicleTypeId == "6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056")
        {
            if (quoteQuery.Accessories.IsCNG)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover OD",
                    Value = Math.Round(result.Resp_TW.BiFuel_Kit_OD_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_TW.BiFuel_Kit_OD_Premium)
                }
                );
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover TP",
                    Value = Math.Round(result.Resp_TW.BiFuel_Kit_TP_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_TW.BiFuel_Kit_TP_Premium)
                }
                );
            }
            if (quoteQuery.VehicleDetails.Fuel.Equals("CNG") || quoteQuery.VehicleDetails.Fuel.Equals("LPG"))
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Inbuilt Accessory Cover",
                    Value = Math.Round(result.Resp_TW.InBuilt_BiFuel_Kit_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_TW.InBuilt_BiFuel_Kit_Premium)
                }
                );
            }
            if (quoteQuery.Accessories.IsElectrical)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.ElectricalId,
                    Name = "Electrical Accessory Cover",
                    Value = Math.Round(result.Resp_TW.Electical_Acc_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_TW.Electical_Acc_Premium)
                });
            }
            if (quoteQuery.Accessories.IsNonElectrical)
            {
                accessoriesCover.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.NonElectricalId,
                    Name = "Non-Electrical Accessory Cover",
                    Value = Math.Round(result.Resp_TW.NonElectical_Acc_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_TW.NonElectical_Acc_Premium)
                });
            }
        }
        
        return accessoriesCover;
    }
    private static List<NameValueModel> SetAddonsCover(QuoteQueryModel quoteQuery, HDFCServiceResponseModel result)
    {
        var addOnCover = new List<NameValueModel>();
        if (quoteQuery.VehicleTypeId == "2d566966-5525-4ed7-bd90-bb39e8418f39")
        {
            if (quoteQuery.AddOns.IsReturnToInvoiceRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "RTI",
                    Id = quoteQuery.AddOns.ReturnToInvoiceIdId,
                    Value = Math.Round(result.Resp_PvtCar.Vehicle_Base_RTI_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.Vehicle_Base_RTI_Premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsConsumableRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Consumables",
                    Id = quoteQuery.AddOns.ConsumableId,
                    Value = Math.Round(result.Resp_PvtCar.Vehicle_Base_COC_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.Vehicle_Base_COC_Premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsEngineProtectionRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Engine Gearbox Protection",
                    Id = quoteQuery.AddOns.EngineProtectionId,
                    Value = Math.Round(result.Resp_PvtCar.Vehicle_Base_ENG_Premium).ToString(),
                    IsApplicable = IsApplicable(result.Resp_PvtCar.Vehicle_Base_ENG_Premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsLossOfDownTimeRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Loss of Down Time Protection",
                    Value = Math.Round(result.Resp_PvtCar.Loss_of_Use_Premium).ToString(),
                    Id = quoteQuery.AddOns.LossOfDownTimeId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.Loss_of_Use_Premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsRoadSideAssistanceRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Road Side Assitance",
                    Value = Math.Round(result.Resp_PvtCar.EA_premium).ToString(),
                    Id = quoteQuery.AddOns.RoadSideAssistanceId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.EA_premium)
                });
            }
            if (quoteQuery.AddOns.IsRoadSideAssistanceWiderRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Road Side Assitance Wider",
                    Value = Math.Round(result.Resp_PvtCar.EAW_premium).ToString(),
                    Id = quoteQuery.AddOns.RoadSideAssistanceWiderId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.EAW_premium)
                });
            }
            if (quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Road Side Assitance Advance",
                    Value = Math.Round(result.Resp_PvtCar.EAAdvance_premium).ToString(),
                    Id = quoteQuery.AddOns.RoadSideAssistanceAdvanceId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.EAAdvance_premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsTowingRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Towing Protection",
                    Value = Math.Round(result.Resp_PvtCar.Towing_premium).ToString(),
                    Id = quoteQuery.AddOns.TowingId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.Towing_premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsEMIProtectorRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "EMI Protection",
                    Value = Math.Round(result.Resp_PvtCar.EMI_PROTECTOR_PREMIUM).ToString(),
                    Id = quoteQuery.AddOns.EMIProtectorId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.EMI_PROTECTOR_PREMIUM)
                }
                );
            }
            if (quoteQuery.AddOns.IsNCBRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "No Claim Bonus Protection",
                    Value = Math.Round(result.Resp_PvtCar.Vehicle_Base_NCB_Premium + result.Resp_PvtCar.Bifuel_NCB_Premium + result.Resp_PvtCar.NonElec_NCB_Premium + result.Resp_PvtCar.NonElec_NCB_Premium).ToString(),
                    Id = quoteQuery.AddOns.NCBId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.Vehicle_Base_NCB_Premium + result.Resp_PvtCar.Bifuel_NCB_Premium + result.Resp_PvtCar.NonElec_NCB_Premium + result.Resp_PvtCar.NonElec_NCB_Premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsZeroDebt)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Zero Dep",
                    Value = Math.Round(result.Resp_PvtCar.Vehicle_Base_ZD_Premium + result.Resp_PvtCar.Bifuel_ZD_Premium + result.Resp_PvtCar.NonElec_ZD_Premium + result.Resp_PvtCar.Elec_ZD_Premium).ToString(),
                    Id = quoteQuery.AddOns.ZeroDebtId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.Vehicle_Base_ZD_Premium + result.Resp_PvtCar.Bifuel_ZD_Premium + result.Resp_PvtCar.NonElec_ZD_Premium + result.Resp_PvtCar.Elec_ZD_Premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsTyreProtectionRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Tyre Protection",
                    Value = Math.Round(result.Resp_PvtCar.Vehicle_Base_TySec_Premium).ToString(),
                    Id = quoteQuery.AddOns.TyreProtectionId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.Vehicle_Base_TySec_Premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsLimitedOwnPremisesRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Limited to Own Premises OD",
                    Value = Math.Round(result.Resp_PvtCar.LimitedtoOwnPremises_OD_Premium).ToString(),
                    Id = quoteQuery.AddOns.LimitedOwnPremisesId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.LimitedtoOwnPremises_OD_Premium)
                }
                );
                addOnCover.Add(new NameValueModel
                {
                    Name = "Limited to Own Premises TP",
                    Value = Math.Round(result.Resp_PvtCar.LimitedtoOwnPremises_TP_Premium).ToString(),
                    Id = quoteQuery.AddOns.LimitedOwnPremisesId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.LimitedtoOwnPremises_TP_Premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsGeoAreaExtension)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Geo Area Extension OD",
                    Value = Math.Round(result.Resp_PvtCar.GeogExtension_ODPremium).ToString(),
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.GeogExtension_ODPremium)
                }
                );
                addOnCover.Add(new NameValueModel
                {
                    Name = "Geo Area Extension TP",
                    Value = Math.Round(result.Resp_PvtCar.GeogExtension_TPPremium).ToString(),
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    IsApplicable = IsApplicable(result.Resp_PvtCar.GeogExtension_TPPremium)
                }
                );
            }
        }
        else if (quoteQuery.VehicleTypeId == "6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056")
        {
            if (quoteQuery.AddOns.IsReturnToInvoiceRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "RTI",
                    Value = Math.Round(result.Resp_TW.Vehicle_Base_RTI_Premium).ToString(),
                    Id = quoteQuery.AddOns.ReturnToInvoiceIdId,
                    IsApplicable = IsApplicable(result.Resp_TW.Vehicle_Base_RTI_Premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsRoadSideAssistanceRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Road Side Assitance",
                    Value = Math.Round(result.Resp_TW.EA_premium).ToString(),
                    Id = quoteQuery.AddOns.RoadSideAssistanceId,
                    IsApplicable = IsApplicable(result.Resp_TW.EA_premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsZeroDebt)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Zero Dep",
                    Value = Math.Round(result.Resp_TW.Vehicle_Base_ZD_Premium + result.Resp_TW.Bifuel_ZD_Premium + result.Resp_TW.NonElec_ZD_Premium + result.Resp_TW.Elec_ZD_Premium).ToString(),
                    Id = quoteQuery.AddOns.ZeroDebtId,
                    IsApplicable = IsApplicable(result.Resp_TW.Vehicle_Base_ZD_Premium + result.Resp_TW.Bifuel_ZD_Premium + result.Resp_TW.NonElec_ZD_Premium + result.Resp_TW.Elec_ZD_Premium)
                }
                );
            }
            if (quoteQuery.AddOns.IsGeoAreaExtension)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Geo Area Extension OD",
                    Value = Math.Round(result.Resp_TW.GeogExtension_ODPremium).ToString(),
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    IsApplicable = IsApplicable(result.Resp_TW.GeogExtension_ODPremium)
                }
                );
                addOnCover.Add(new NameValueModel
                {
                    Name = "Geo Area Extension TP",
                    Value = Math.Round(result.Resp_TW.GeogExtension_TPPremium).ToString(),
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    IsApplicable = IsApplicable(result.Resp_TW.GeogExtension_TPPremium)
                }
                );
            }
            if (quoteQuery.AddOns.IsConsumableRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ConsumableId,
                    Name = "Consumables",
                    IsApplicable = false
                });
            }
            if (quoteQuery.AddOns.IsTowingRequired)
            {
                addOnCover.Add(new NameValueModel
                {
                    Name = "Towing Protection",
                    Id = quoteQuery.AddOns.TowingId,
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
            };
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
        if (quoteQuery.AddOns.IsPersonalBelongingRequired)
        {
            addOnCover.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.PersonalBelongingId,
                Name = "Personal Belongings",
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
        return addOnCover;
    }
    private static List<NameValueModel> SetBaseCoverCommercial(string previousPolicy, string categoryId, HDFCServiceResponseModel result)
    {
        List<NameValueModel> baseCoverList = new List<NameValueModel>();
        switch (categoryId)
        {
            case "1":
                if (previousPolicy.Equals("Package Comprehensive") || previousPolicy.Equals("Comprehensive Bundle"))
                {
                    if (result.Resp_GCV.BreakIN_Premium > 0)
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name = "Basic Own Damage Premium",
                                Value =Math.Round(result.Resp_GCV.Basic_OD_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_GCV.Basic_OD_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name="Third Party Cover Premium",
                                Value=Math.Round(result.Resp_GCV.Basic_TP_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_GCV.Basic_TP_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name="BreakIn Premium",
                                Value=Math.Round(result.Resp_GCV.BreakIN_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_GCV.BreakIN_Premium).ToString())
                            }
                        };
                    }
                    else
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name = "Basic Own Damage Premium",
                                Value =Math.Round(result.Resp_GCV.Basic_OD_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_GCV.Basic_OD_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name="Third Party Cover Premium",
                                Value=Math.Round(result.Resp_GCV.Basic_TP_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_GCV.Basic_TP_Premium).ToString())
                            }
                        };
                    }
                }
                if (previousPolicy.Equals("SATP"))
                {
                    if (result.Resp_GCV.BreakIN_Premium > 0)
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name = "Third Party Cover Premium",
                                Value = Math.Round(result.Resp_GCV.Basic_TP_Premium).ToString(),
                                IsApplicable = IsApplicable((result.Resp_GCV.Basic_TP_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name = "BreakIn Premium",
                                Value = Math.Round(result.Resp_GCV.BreakIN_Premium).ToString(),
                                IsApplicable = IsApplicable((result.Resp_GCV.BreakIN_Premium).ToString())
                            }
                        };
                    }
                    else
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name="Third Party Cover Premium",
                                Value=Math.Round(result.Resp_GCV.Basic_TP_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_GCV.Basic_TP_Premium).ToString())
                            }
                        };
                    }
                }
                break;
            case "2":
                if (previousPolicy.Equals("Package Comprehensive") || previousPolicy.Equals("Comprehensive Bundle"))
                {
                    if (result.Resp_PCV.BreakIN_Premium > 0)
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name = "Basic Own Damage Premium",
                                Value =Math.Round(result.Resp_PCV.Basic_OD_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_PCV.Basic_OD_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name="Third Party Cover Premium",
                                Value=Math.Round(result.Resp_PCV.Basic_TP_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_PCV.Basic_TP_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name="BreakIn Premium",
                                Value=Math.Round(result.Resp_PCV.BreakIN_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_PCV.BreakIN_Premium).ToString())
                            }
                        };
                    }
                    else
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name = "Basic Own Damage Premium",
                                Value =Math.Round(result.Resp_PCV.Basic_OD_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_PCV.Basic_OD_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name="Third Party Cover Premium",
                                Value=Math.Round(result.Resp_PCV.Basic_TP_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_PCV.Basic_TP_Premium).ToString())
                            }
                        };
                    }
                }
                if (previousPolicy.Equals("SATP"))
                {
                    if (result.Resp_PCV.BreakIN_Premium > 0)
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name = "Third Party Cover Premium",
                                Value = Math.Round(result.Resp_PCV.Basic_TP_Premium).ToString(),
                                IsApplicable = IsApplicable((result.Resp_PCV.Basic_TP_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name = "BreakIn Premium",
                                Value = Math.Round(result.Resp_PCV.BreakIN_Premium).ToString(),
                                IsApplicable = IsApplicable((result.Resp_PCV.BreakIN_Premium).ToString())
                            }
                        };
                    }
                    else
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name="Third Party Cover Premium",
                                Value=Math.Round(result.Resp_PCV.Basic_TP_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_PCV.Basic_TP_Premium).ToString())
                            }
                        };
                    }
                }
                break;
            case "3":
                if (previousPolicy.Equals("Package Comprehensive") || previousPolicy.Equals("Comprehensive Bundle"))
                {
                    if (result.Resp_MISD.BreakIN_Premium > 0)
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name = "Basic Own Damage Premium",
                                Value =Math.Round(result.Resp_MISD.Basic_OD_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_MISD.Basic_OD_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name="Third Party Cover Premium",
                                Value=Math.Round(result.Resp_MISD.Basic_TP_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_MISD.Basic_TP_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name="BreakIn Premium",
                                Value=Math.Round(result.Resp_MISD.BreakIN_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_MISD.BreakIN_Premium).ToString())
                            }
                        };
                    }
                    else
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name = "Basic Own Damage Premium",
                                Value =Math.Round(result.Resp_MISD.Basic_OD_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_MISD.Basic_OD_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name="Third Party Cover Premium",
                                Value=Math.Round(result.Resp_MISD.Basic_TP_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_MISD.Basic_TP_Premium).ToString())
                            }
                        };
                    }
                }
                if (previousPolicy.Equals("SATP"))
                {
                    if (result.Resp_MISD.BreakIN_Premium > 0)
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name = "Third Party Cover Premium",
                                Value = Math.Round(result.Resp_MISD.Basic_TP_Premium).ToString(),
                                IsApplicable = IsApplicable((result.Resp_MISD.Basic_TP_Premium).ToString())
                            },
                            new NameValueModel
                            {
                                Name = "BreakIn Premium",
                                Value = Math.Round(result.Resp_MISD.BreakIN_Premium).ToString(),
                                IsApplicable = IsApplicable((result.Resp_MISD.BreakIN_Premium).ToString())
                            }
                        };
                    }
                    else
                    {
                        baseCoverList = new List<NameValueModel>
                        {
                            new NameValueModel
                            {
                                Name="Third Party Cover Premium",
                                Value=Math.Round(result.Resp_MISD.Basic_TP_Premium).ToString(),
                                IsApplicable=IsApplicable((result.Resp_MISD.Basic_TP_Premium).ToString())
                            }
                        };
                    }
                }
                break;
        }
        return baseCoverList;
    }
    private static List<NameValueModel> SetPACoverCommercial(QuoteQueryModel quoteQuery, HDFCServiceResponseModel result)
    {
        var paCover = new List<NameValueModel>();

        double pAOwnerDriverPremium = 0.0;
        double pAPaidDriverPremium = 0.0;
        double pACleanerConductorHelperPremium = 0.0;

        switch (quoteQuery.CategoryId)
        {
            case "1":
                pAOwnerDriverPremium = result.Resp_GCV.PAOwnerDriver_Premium;
                pAPaidDriverPremium = result.Resp_GCV.NumberOfDrivers_Premium;
                pACleanerConductorHelperPremium = result.Resp_GCV.PAPaidDriverCleaCondCool_Premium;
                break;
            case "2":
                pAOwnerDriverPremium = result.Resp_PCV.PAOwnerDriver_Premium;
                pAPaidDriverPremium = result.Resp_PCV.NumberOfDrivers_Premium;
                pACleanerConductorHelperPremium = result.Resp_PCV.PAPaidDriverCleaCondCool_Premium;
                break;
            case "3":
                pAOwnerDriverPremium = result.Resp_MISD.PAOwnerDriver_Premium;
                pAPaidDriverPremium = result.Resp_MISD.NumberOfDrivers_Premium;
                pACleanerConductorHelperPremium = result.Resp_MISD.PAPaidDriverCleaCondCool_Premium;
                break;
        }

        if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
        {
            paCover.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                Name = "PA Cover for Owner Driver",
                Value = Math.Round(pAOwnerDriverPremium).ToString(),
                IsApplicable = IsApplicable(pAOwnerDriverPremium)
            });
        }
        if (quoteQuery.PACover.IsPaidDriver)
        {
            paCover.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.PaidDriverId,
                Name = "PA Cover For Paid Driver",
                Value = Math.Round(pAPaidDriverPremium).ToString(),
                IsApplicable = IsApplicable(pAPaidDriverPremium)
            }
            );
        }
        if (quoteQuery.PACover.IsUnnamedCleaner || quoteQuery.PACover.IsUnnamedConductor || quoteQuery.PACover.IsUnnamedHelper)
        {
            paCover.Add(new NameValueModel
            {
                Name = "PA Cover For Conductor or Cleaner or Helper",
                Value = Math.Round(pACleanerConductorHelperPremium).ToString(),
                IsApplicable = IsApplicable(pACleanerConductorHelperPremium)
            }
            );
        }
        if (quoteQuery.PACover.IsUnnamedHirer)
        {
            paCover.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedHirerId,
                Name = "PA Cover For Hirer",
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.PACover.IsUnnamedPassenger)
        {
            paCover.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedPassengerId,
                Name = "PA Cover For Unnamed Passanger",
                IsApplicable = false
            }
            );
        }


        return paCover;
    }
    private static List<NameValueModel> SetDiscountCoverCommercial(QuoteQueryModel quoteQuery, HDFCServiceResponseModel result)
    {
        var discountCover = new List<NameValueModel>();
        double tppdPremium = 0.0, antiTheftDiscPremium = 0.0, ncbDoscumentPremium = 0.0;

        switch (quoteQuery.CategoryId)
        {
            case "1":
                tppdPremium = result.Resp_GCV.TPPD_premium;
                antiTheftDiscPremium = result.Resp_GCV.AntiTheftDisc_Premium;
                ncbDoscumentPremium = result.Resp_GCV.NCBBonusDisc_Premium;
                break;
            case "2":
                tppdPremium = result.Resp_PCV.TPPD_premium;
                antiTheftDiscPremium = result.Resp_PCV.AntiTheftDisc_Premium;
                ncbDoscumentPremium = result.Resp_PCV.NCBBonusDisc_Premium;
                break;
            case "3":
                tppdPremium = result.Resp_MISD.TPPD_premium;
                antiTheftDiscPremium = result.Resp_MISD.AntiTheftDisc_Premium;
                ncbDoscumentPremium = result.Resp_MISD.NCBBonusDisc_Premium;
                break;
        }
        if(ncbDoscumentPremium > 0)
        {
            discountCover.Add(new NameValueModel
            {
                Name = "No Claim Bonus",
                Value = Math.Round(ncbDoscumentPremium).ToString(),
                IsApplicable = true,
            });
        }
        if (quoteQuery.Discounts.IsLimitedTPCoverage)
        {
            discountCover.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.LimitedTPCoverageId,
                Name = "Limited Third Party Coverage",
                Value = Math.Round(tppdPremium).ToString(),
                IsApplicable = IsApplicable(tppdPremium)
            });
        }
        if (quoteQuery.Discounts.IsAntiTheft)
        {
            discountCover.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AntiTheftId,
                Name = "ARAI Approved Anti-Theft Device",
                Value = Math.Round(antiTheftDiscPremium).ToString(),
                IsApplicable = quoteQuery.VehicleTypeId.Equals("88a807b3-90e4-484b-b5d2-65059a8e1a91") ? IsApplicable(antiTheftDiscPremium) : false
            });
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
        return discountCover;
    }
    private static List<NameValueModel> SetAccessoriesCoverCommercial(QuoteQueryModel quoteQuery, HDFCServiceResponseModel result)
    {
        var accessoriesCover = new List<NameValueModel>();
        double biFuelKitODPremium = 0.0, biFuelKitTPPremium = 0.0, electicalAccPremium = 0.0, nonElecticalAccPremium = 0.0;
        switch (quoteQuery.CategoryId)
        {
            case ("1"):
                biFuelKitODPremium = result.Resp_GCV.BiFuel_Kit_OD_Premium;
                biFuelKitTPPremium = result.Resp_GCV.BiFuel_Kit_TP_Premium;
                electicalAccPremium = result.Resp_GCV.Electical_Acc_Premium;
                nonElecticalAccPremium = result.Resp_GCV.NonElectical_Acc_Premium;
                break;
            case ("2"):
                biFuelKitODPremium = result.Resp_PCV.BiFuel_Kit_OD_Premium;
                biFuelKitTPPremium = result.Resp_PCV.BiFuel_Kit_TP_Premium;
                electicalAccPremium = result.Resp_PCV.Electical_Acc_Premium;
                nonElecticalAccPremium = result.Resp_PCV.NonElectical_Acc_Premium;
                break;
            case ("3"):
                biFuelKitODPremium = result.Resp_MISD.BiFuel_Kit_OD_Premium;
                biFuelKitTPPremium = result.Resp_MISD.BiFuel_Kit_TP_Premium;
                electicalAccPremium = result.Resp_MISD.Electical_Acc_Premium;
                nonElecticalAccPremium = result.Resp_MISD.NonElectical_Acc_Premium;
                break;
        }

        if (quoteQuery.Accessories.IsCNG)
        {
            accessoriesCover.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover OD",
                Value = Math.Round(biFuelKitODPremium).ToString(),
                IsApplicable = IsApplicable(biFuelKitODPremium)
            }
            );
            accessoriesCover.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover TP",
                Value = Math.Round(biFuelKitTPPremium).ToString(),
                IsApplicable = IsApplicable(biFuelKitTPPremium)
            }
            );
        }
        if (quoteQuery.Accessories.IsElectrical)
        {
            accessoriesCover.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.ElectricalId,
                Name = "Electrical Accessory Cover",
                Value = Math.Round(electicalAccPremium).ToString(),
                IsApplicable = IsApplicable(electicalAccPremium)
            });
        }
        if (quoteQuery.Accessories.IsNonElectrical)
        {
            accessoriesCover.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.NonElectricalId,
                Name = "Non-Electrical Accessory Cover",
                Value = Math.Round(nonElecticalAccPremium).ToString(),
                IsApplicable = IsApplicable(nonElecticalAccPremium)
            });
        }

        return accessoriesCover;
    }
    private static List<NameValueModel> SetAddonsCoverCommercial(QuoteQueryModel quoteQuery, HDFCServiceResponseModel result)
    {
        var addOnCover = new List<NameValueModel>();
        double imt23Premium = 0.0, geoODPremium = 0.0, geoTPPremium = 0.0;
        switch (quoteQuery.CategoryId)
        {
            case ("1"):
                imt23Premium = result.Resp_GCV.Trailer_InclusionofIMT23_Premium + result.Resp_GCV.VB_InclusionofIMT23_Premium + result.Resp_GCV.Elec_InclusionofIMT23_Premium + result.Resp_GCV.NonElec_InclusionofIMT23_Premium + result.Resp_GCV.BiFuel_InclusionofIMT23_Premium;
                geoODPremium = result.Resp_GCV.GeogExtension_ODPremium;
                geoTPPremium = result.Resp_GCV.GeogExtension_TPPremium;
                break;
            case ("2"):
                imt23Premium = result.Resp_PCV.VB_InclusionofIMT23_Premium + result.Resp_PCV.Elec_InclusionofIMT23_Premium + result.Resp_PCV.NonElec_InclusionofIMT23_Premium + result.Resp_PCV.BiFuel_InclusionofIMT23_Premium;
                geoODPremium = result.Resp_PCV.GeogExtension_ODPremium;
                geoTPPremium = result.Resp_PCV.GeogExtension_TPPremium;
                break;
            case ("3"):
                imt23Premium = result.Resp_MISD.Trailer_InclusionofIMT23_Premium + result.Resp_MISD.VB_InclusionofIMT23_Premium + result.Resp_MISD.Elec_InclusionofIMT23_Premium + result.Resp_MISD.NonElec_InclusionofIMT23_Premium + result.Resp_MISD.BiFuel_InclusionofIMT23_Premium;
                geoODPremium = result.Resp_MISD.GeogExtension_ODPremium;
                geoTPPremium = result.Resp_MISD.GeogExtension_TPPremium;
                break;
        }

        if (quoteQuery.AddOns.IsIMT23)
        {
            addOnCover.Add(new NameValueModel
            {
                Name = "IMT23",
                Id = quoteQuery.AddOns.IMT23Id,
                Value = Math.Round(imt23Premium).ToString(),
                IsApplicable = IsApplicable(imt23Premium)
            }
            );
        }
        if (quoteQuery.AddOns.IsGeoAreaExtension)
        {
            addOnCover.Add(new NameValueModel
            {
                Name = "Geo Area Extension OD",
                Id = quoteQuery.AddOns.GeoAreaExtensionId,
                Value = Math.Round(geoODPremium).ToString(),
                IsApplicable = IsApplicable(geoODPremium)
            }
            );
            addOnCover.Add(new NameValueModel
            {
                Name = "Geo Area Extension TP",
                Id = quoteQuery.AddOns.GeoAreaExtensionId,
                Value = Math.Round(geoTPPremium).ToString(),
                IsApplicable = IsApplicable(geoTPPremium)
            }
            );
        }
        if (quoteQuery.AddOns.IsReturnToInvoiceRequired)
        {
            addOnCover.Add(new NameValueModel
            {
                Name = "RTI",
                Id = quoteQuery.AddOns.ReturnToInvoiceIdId,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsConsumableRequired)
        {
            addOnCover.Add(new NameValueModel
            {
                Name = "Consumables",
                Id = quoteQuery.AddOns.ConsumableId,
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
        if (quoteQuery.AddOns.IsRoadSideAssistanceRequired)
        {
            addOnCover.Add(new NameValueModel
            {
                Name = "Road Side Assitance",
                Id = quoteQuery.AddOns.RoadSideAssistanceId,
                IsApplicable = false
            });
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
        if (quoteQuery.AddOns.IsZeroDebt)
        {
            addOnCover.Add(new NameValueModel
            {
                Name = "Zero Dep",
                Id = quoteQuery.AddOns.ZeroDebtId,
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
        return addOnCover;
    }
    public async Task<(string Token, string TransactionId, string ProductCode)> GetToken(string vehicleTypeId, string policyTypeId, string stage, string categoryId, string leadId, CancellationToken cancellationToken)
    {
        var quoteVm = new TokenResponseModel();
        try
        {
            string product_CODE = string.Empty;
            string transactionID = "HERO";
            var id = 0;
            if (vehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
            {
                product_CODE = policyTypeId.Equals(_policyTypeConfig.SATP) ? _hdfcConfig.PC_ProductCode_TP : _hdfcConfig.PC_ProductCode_NonTP;
                transactionID = transactionID + "PC" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            else if (vehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
            {
                product_CODE = policyTypeId.Equals(_policyTypeConfig.SATP) ? _hdfcConfig.TW_ProductCode_TP : _hdfcConfig.TW_ProductCode_NonTP;
                transactionID = transactionID + "TW" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            else if (vehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
            {
                if (categoryId.Equals("1"))
                    product_CODE = policyTypeId.Equals(_policyTypeConfig.SATP) ? _hdfcConfig.GCV_ProductCode_TP : _hdfcConfig.GCV_ProductCode_NonTP;
                else if (categoryId.Equals("2"))
                    product_CODE = policyTypeId.Equals(_policyTypeConfig.SATP) ? _hdfcConfig.PCV_ProductCode_TP : _hdfcConfig.PCV_ProductCode_NonTP;
                else
                    product_CODE = _hdfcConfig.MISDProductCode;

                transactionID = transactionID + "CV" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            var defaultRequestHeaders = _client.DefaultRequestHeaders;
            defaultRequestHeaders.Clear();
            defaultRequestHeaders.Add("SOURCE", _hdfcConfig.CommanHeaderData.SOURCE);
            defaultRequestHeaders.Add("CHANNEL_ID", _hdfcConfig.CommanHeaderData.CHANNEL_ID);
            defaultRequestHeaders.Add("CREDENTIAL", _hdfcConfig.CommanHeaderData.CREDENTIAL);
            defaultRequestHeaders.Add("PRODUCT_CODE", product_CODE);
            defaultRequestHeaders.Add("TRANSACTIONID", transactionID);


            string url = _hdfcConfig.TokenURL;
            id = await InsertICLogs(string.Empty, leadId, _hdfcConfig.BaseURL + url, transactionID, JsonConvert.SerializeObject(defaultRequestHeaders), stage);
            try
            {
                var Res = await _client.GetAsync(url, cancellationToken);

                if (Res.IsSuccessStatusCode)
                {
                    string json = await Res.Content.ReadAsStringAsync(cancellationToken);
                    var result = JsonConvert.DeserializeObject<TokenResponseModel>(json);
                    if (result != null && result.StatusCode == 200)
                    {
                        UpdateICLogs(id, vehicleTypeId, json);
                        return (result.Authentication.Token, result.TransactionID, product_CODE);
                    }
                }
                return default;
            }
            catch (Exception ex)
            {
                UpdateICLogs(id, vehicleTypeId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("HDFC Error {exception}", ex.Message);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return default;
        }
    }
    public async Task<IDVResponseModel> GetIDV(QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
    {
        var quoteVm = new IDVResponseModel();
        string requestBody = string.Empty;
        var responseBody = string.Empty;

        try
        {
            _logger.LogInformation("Get HDFC IDV");

            var idvrequest = new IDVRequestModel()
            {
                TransactionID = quoteQuery.TransactionId,
                IDV_DETAILS = new IDV_DETAILS()
                {
                    ModelCode = quoteQuery.VehicleDetails.VehicleModelCode,
                    RTOCode = quoteQuery.RTOLocationCode,
                    Vehicle_Registration_Date = quoteQuery.RegistrationDate,
                    Policy_Start_Date = quoteQuery.PolicyStartDate
                }
            };

            requestBody = JsonConvert.SerializeObject(idvrequest);
            _logger.LogInformation("HDFC IDV RequestBody {RequestBody}", requestBody);
            var defaultRequestHeaders = _client.DefaultRequestHeaders;
            defaultRequestHeaders.Clear();
            defaultRequestHeaders.Add("SOURCE", _hdfcConfig.CommanHeaderData.SOURCE);
            defaultRequestHeaders.Add("CHANNEL_ID", _hdfcConfig.CommanHeaderData.CHANNEL_ID);
            defaultRequestHeaders.Add("PRODUCT_CODE", quoteQuery.ProductCode);
            defaultRequestHeaders.Add("Token", quoteQuery.Token);

            var id = 0;
            id = await InsertICLogs(requestBody, quoteQuery?.LeadId, _hdfcConfig.BaseURL + _hdfcConfig.IdvURL, quoteQuery.Token, JsonConvert.SerializeObject(defaultRequestHeaders), "Quote");
            var responseMessage = await _client.PostAsJsonAsync(_hdfcConfig.IdvURL, idvrequest);
            try
            {
                var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                var result = stream.DeserializeFromJson<IDVResponseModel>();
                responseBody = JsonConvert.SerializeObject(result);
                UpdateICLogs(id, quoteQuery?.TransactionId, responseBody);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    quoteVm.StatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogError("Data not found {responseBody}", responseBody);
                }
                else
                {
                    if (result != null && result.StatusCode == 200)
                    {
                        quoteVm.StatusCode = (int)HttpStatusCode.OK;
                        quoteQuery.MaxIDV = Convert.ToDecimal(result?.CalculatedIDV?.MAX_IDV_AMOUNT);
                        quoteQuery.MinIDV = Convert.ToDecimal(result?.CalculatedIDV?.MIN_IDV_AMOUNT);
                        quoteQuery.RecommendedIDV = Convert.ToDecimal(result?.CalculatedIDV?.IDV_AMOUNT);
                        quoteVm = result;
                    }
                    else
                    {
                        quoteVm.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateICLogs(id, quoteQuery?.TransactionId, responseBody);
                _logger.LogError("HDFC IDV Error {exception}", ex.Message);
            }
            return quoteVm;
        }
        catch (Exception ex)
        {
            _logger.LogError("HDFC IDV Error {exception}", ex.Message);
            quoteVm.StatusCode = (int)HttpStatusCode.BadRequest;
            return quoteVm;
        }
    }
    private static Req_Pvtcar GetPvtCarDetails(QuoteQueryModel quoteQuery)
    {
        bool isVehicleAgeLessThan3Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 3);
        bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 5);

        var req_PvtCar = new Req_Pvtcar()
        {
            POSP_CODE = string.Empty,
            POLICY_TYPE = string.IsNullOrEmpty(quoteQuery.CurrentPolicyType) ? null : quoteQuery.CurrentPolicyType,
            POLICY_TENURE = quoteQuery.IsBrandNewVehicle ? 3 : 1,
            ExtensionCountryCode = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsGeoAreaExtension) ? Convert.ToDouble(quoteQuery.GeogExtensionCode) : 0,
            ExtensionCountryName = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsGeoAreaExtension && !string.IsNullOrEmpty(quoteQuery.GeogExtension)) ? (quoteQuery.GeogExtension).Trim() : null,
            BreakIN_ID = string.Empty,
            BreakInStatus = string.Empty,
            BreakInInspectionFlag = string.Empty,
            BreakinWaiver = false,
            BreakinInspectionDate = null,
            Effectivedrivinglicense = !quoteQuery.PACover.IsUnnamedOWNERDRIVER,
            NumberOfEmployees = 0,
            BiFuelType = quoteQuery.Accessories.IsCNG ? "CNG" : null,
            BiFuel_Kit_Value = quoteQuery.Accessories.IsCNG ? quoteQuery.Accessories.CNGValue : 0,
            LLPaiddriver = quoteQuery.PACover.IsPaidDriver ? 1 : 0,
            PAPaiddriverSI = 0,
            Owner_Driver_Nominee_Name = string.Empty,
            Owner_Driver_Nominee_Age = 0,
            Owner_Driver_Nominee_Relationship = string.Empty,
            Owner_Driver_Appointee_Name = string.Empty,
            Owner_Driver_Appointee_Relationship = string.Empty,
            IsZeroDept_Cover = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsZeroDebt) ? 1 : 0,
            IsTyreSecure_Cover = (isVehicleAgeLessThan3Years && quoteQuery.AddOns.IsTyreProtectionRequired) ? 1 : 0,
            ElecticalAccessoryIDV = quoteQuery.Accessories.IsElectrical ? quoteQuery.Accessories.ElectricalValue : 0,
            NonElecticalAccessoryIDV = quoteQuery.Accessories.IsNonElectrical ? quoteQuery.Accessories.NonElectricalValue : 0,
            OtherLoadDiscRate = 0.0,//doubt
            AntiTheftDiscFlag = false,
            HandicapDiscFlag = false,
            IsNCBProtection_Cover = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsNCBRequired) ? 1 : 0,
            IsRTI_Cover = (isVehicleAgeLessThan3Years && quoteQuery.AddOns.IsReturnToInvoiceRequired) ? 1 : 0,
            IsCOC_Cover = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsConsumableRequired) ? 1 : 0,
            IsEngGearBox_Cover = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsEngineProtectionRequired) ? 1 : 0,
            IsLossofUseDownTimeProt_Cover = 0,//not available
            IsEA_Cover = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsRoadSideAssistanceRequired) ? 1 : 0,
            IsEAW_Cover = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsRoadSideAssistanceWiderRequired) ? 1 : 0,
            IsEAAdvance_Cover = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired) ? 1 : 0,
            IsTowing_Cover = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsTowingRequired) ? 1 : 0,
            Towing_Limit = string.Empty,//doubt
            IsEMIProtector_Cover = 0,//not availble
            NoOfEmi = string.Empty,
            EMIAmount = 0,
            NoofUnnamedPerson = quoteQuery.PACover.IsUnnamedPassenger ? Convert.ToInt32(quoteQuery.VehicleDetails.VehicleSeatCapacity) : 0,
            UnnamedPersonSI = quoteQuery.PACover.IsUnnamedPassenger ? Convert.ToInt32(quoteQuery.PACover.UnnamedPassengerValue) : 0,
            Voluntary_Excess_Discount = 0,
            IsLimitedtoOwnPremises = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsLimitedOwnPremisesRequired) ? 1 : 0,// not changing
            TPPDLimit = quoteQuery.Discounts.IsLimitedTPCoverage ? 100000 : 0,
            NoofnamedPerson = 0,
            namedPersonSI = 0,
            NamedPersons = string.Empty,
            AutoMobile_Assoication_No = quoteQuery.Discounts.IsAAMemberShip ? "Yes" : string.Empty,
            fuel_type = quoteQuery.VehicleDetails.Fuel,
            CPA_Tenure = SetTpaTenure(quoteQuery.PACover.IsUnnamedOWNERDRIVER, quoteQuery.IsBrandNewVehicle, 3),
            PayAsYouDrive = false,
            InitialOdometerReading = 0,
            InitialOdometerReadingDate = null
        };
        return req_PvtCar;
    }
    private static Req_TW GetTWDetails(QuoteQueryModel quoteQuery)
    {
        bool isVehicleAgeLessThan3Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 3);
        bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 5);
        var req_TW = new Req_TW()
        {
            POSP_CODE = string.Empty,
            POLICY_TYPE = quoteQuery.CurrentPolicyType,
            POLICY_TENURE = quoteQuery.IsBrandNewVehicle ? 5 : 1,
            ExtensionCountryCode = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsGeoAreaExtension) ? Convert.ToDouble(quoteQuery.GeogExtensionCode) : 0,
            ExtensionCountryName = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsGeoAreaExtension && !string.IsNullOrEmpty(quoteQuery.GeogExtension)) ? quoteQuery.GeogExtension : null,
            Effectivedrivinglicense = !quoteQuery.PACover.IsUnnamedOWNERDRIVER,
            NumberOfEmployees = 0,
            BiFuelType = quoteQuery.Accessories.IsCNG ? "CNG" : null,
            BiFuel_Kit_Value = quoteQuery.Accessories.IsCNG ? quoteQuery.Accessories.CNGValue : 0,
            Paiddriver = quoteQuery.PACover.IsPaidDriver ? quoteQuery.PACover.UnnamedPaidDriverValue : 0,
            Owner_Driver_Nominee_Name = string.Empty,
            Owner_Driver_Nominee_Age = 0,
            Owner_Driver_Nominee_Relationship = string.Empty,
            Owner_Driver_Appointee_Name = string.Empty,
            Owner_Driver_Appointee_Relationship = string.Empty,
            IsZeroDept_Cover = (isVehicleAgeLessThan3Years && quoteQuery.AddOns.IsZeroDebt) ? 1 : 0,
            ElecticalAccessoryIDV = quoteQuery.Accessories.IsElectrical ? quoteQuery.Accessories.ElectricalValue : 0,
            NonElecticalAccessoryIDV = quoteQuery.Accessories.IsNonElectrical ? quoteQuery.Accessories.NonElectricalValue : 0,
            OtherLoadDiscRate = 0.0,
            AntiTheftDiscFlag = false,
            HandicapDiscFlag = false,
            IsNCBProtection_Cover = 0,
            IsRTI_Cover = (isVehicleAgeLessThan3Years && quoteQuery.AddOns.IsReturnToInvoiceRequired) ? 1 : 0,
            IsCOC_Cover = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsConsumableRequired) ? 1 : 0,
            IsEA_Cover = (isVehicleAgeLessThan3Years && quoteQuery.AddOns.IsRoadSideAssistanceRequired) ? 1 : 0,
            NoofUnnamedPerson = quoteQuery.PACover.IsUnnamedPassenger ? Convert.ToInt32(quoteQuery.VehicleDetails.VehicleSeatCapacity) : 0,
            UnnamedPersonSI = quoteQuery.PACover.IsUnnamedPassenger ? Convert.ToInt32(quoteQuery.PACover.UnnamedPassengerValue) : 0,
            Voluntary_Excess_Discount = 0,
            IsLimitedtoOwnPremises = 0,
            TPPDLimit = 0,//tppd cover is not providing now
            NoofnamedPerson = 0,
            namedPersonSI = 0.0,
            NamedPersons = null,
            AutoMobile_Assoication_No = quoteQuery.Discounts.IsAAMemberShip ? "Yes" : "",
            CPA_Tenure = SetTpaTenure(quoteQuery.PACover.IsUnnamedOWNERDRIVER, quoteQuery.IsBrandNewVehicle, 5),
            service_type = null,
        };
        return req_TW;
    }
    private static Req_PCV  GetPCVDetails(QuoteQueryModel quoteQuery)
    {
        bool isVehicleAgeLessThan3Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 3);
        bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 5);

        int paCoverCleanerConductorCoolie = 0;
        if (quoteQuery.PACover.IsUnnamedCleaner)
            paCoverCleanerConductorCoolie++;
        if (quoteQuery.PACover.IsUnnamedConductor)
            paCoverCleanerConductorCoolie++;
        if (quoteQuery.PACover.IsUnnamedHelper)
            paCoverCleanerConductorCoolie++;

        var req_PCV = new Req_PCV()
        {
            POSP_CODE = string.Empty,
            BreakIN_ID = string.Empty,
            BreakInStatus = string.Empty,
            BreakinInspectionDate = null,
            //PACover
            Effectivedrivinglicense = !quoteQuery.PACover.IsUnnamedOWNERDRIVER,
            NumberOfDrivers = quoteQuery.PACover.IsPaidDriver ? 1 : 0,
            NoOfCleanerConductorCoolies = paCoverCleanerConductorCoolie,
            Paiddriver_Si = paCoverCleanerConductorCoolie > 0 ? 100000 : 0,
            //Discounts
            TPPDLimit = quoteQuery.Discounts.IsLimitedTPCoverage ? 6000 : 0,
            AntiTheftDiscFlag = quoteQuery.Discounts.IsAntiTheft ? true : false,
            //Accessroies
            BiFuelType = quoteQuery.Accessories.IsCNG ? "CNG" : null,
            BiFuel_Kit_Value = quoteQuery.Accessories.IsCNG ? quoteQuery.Accessories.CNGValue : 0,
            ElecticalAccessoryIDV = quoteQuery.Accessories.IsElectrical ? quoteQuery.Accessories.ElectricalValue : 0,
            NonElecticalAccessoryIDV = quoteQuery.Accessories.IsNonElectrical ? quoteQuery.Accessories.NonElectricalValue : 0,
            //Addons
            IsInclusionofIMT23 = quoteQuery.AddOns.IsIMT23 ? 1 : 0,
            ExtensionCountryCode = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsGeoAreaExtension) ? Convert.ToDouble(quoteQuery.GeogExtensionCode) : 0,
            ExtensionCountryName = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsGeoAreaExtension && !string.IsNullOrEmpty(quoteQuery.GeogExtension)) ? (quoteQuery.GeogExtension).Trim() : null,
            IsZeroDept_Cover =  0,
            IsNCBProtection_Cover =  0,
            IsRTI_Cover =  0,
            IsCOC_Cover =  0,
            IsEngGearBox_Cover = 0,
            IsEA_Cover = 0,
            NumberOfEmployees = 0,
            Owner_Driver_Nominee_Name = string.Empty,
            Owner_Driver_Nominee_Age = 0,
            Owner_Driver_Nominee_Relationship = string.Empty,
            Owner_Driver_Appointee_Name = string.Empty,
            Owner_Driver_Appointee_Relationship = string.Empty,
            OtherLoadDiscRate = 0.0,
            IsEAW_Cover = 0,
            IsTowing_Cover = 0,
            Is_SchoolBus = 0,
            IsPrivateUseLoading = 0,
            Bus_Type = string.Empty,
            Towing_Limit = string.Empty,
            NoOfEmi = string.Empty,
            EMIAmount = 0,
            IsLimitedtoOwnPremises = 0,
            NoOfFPP = 0,
            NoOfNFPP = 0,
        };
        return req_PCV;
    }
    private static Req_GCV GetGCVDetails(QuoteQueryModel quoteQuery)
    {
        bool isVehicleAgeLessThan3Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 3);
        bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 5);

        int paCoverCleanerConductorCoolie = 0;
        if (quoteQuery.PACover.IsUnnamedCleaner)
            paCoverCleanerConductorCoolie++;
        if (quoteQuery.PACover.IsUnnamedConductor)
            paCoverCleanerConductorCoolie++;
        if (quoteQuery.PACover.IsUnnamedHelper)
            paCoverCleanerConductorCoolie++;

        var req_GCV = new Req_GCV()
        {
            POSP_CODE = string.Empty,
            BreakIN_ID = string.Empty,
            BreakInStatus = string.Empty,
            BreakinInspectionDate = null,
            //PACover
            Effectivedrivinglicense = !quoteQuery.PACover.IsUnnamedOWNERDRIVER,
            NumberOfDrivers = quoteQuery.PACover.IsPaidDriver ? 1 : 0,
            NoOfCleanerConductorCoolies = paCoverCleanerConductorCoolie,
            Paiddriver_Si = paCoverCleanerConductorCoolie > 0 ? 100000 : 0,
            //Discounts
            TPPDLimit = quoteQuery.Discounts.IsLimitedTPCoverage ? 6000 : 0,
            AntiTheftDiscFlag = quoteQuery.Discounts.IsAntiTheft ? true : false,
            //Accessroies
            BiFuelType = quoteQuery.Accessories.IsCNG ? "CNG" : null,
            BiFuel_Kit_Value = quoteQuery.Accessories.IsCNG ? quoteQuery.Accessories.CNGValue : 0,
            ElecticalAccessoryIDV = quoteQuery.Accessories.IsElectrical ? quoteQuery.Accessories.ElectricalValue : 0,
            NonElecticalAccessoryIDV = quoteQuery.Accessories.IsNonElectrical ? quoteQuery.Accessories.NonElectricalValue : 0,
            //Addons
            IsInclusionofIMT23 = quoteQuery.AddOns.IsIMT23 ? 1 : 0,
            ExtensionCountryCode = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsGeoAreaExtension) ? Convert.ToDouble(quoteQuery.GeogExtensionCode) : 0,
            ExtensionCountryName = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsGeoAreaExtension && !string.IsNullOrEmpty(quoteQuery.GeogExtension)) ? (quoteQuery.GeogExtension).Trim() : null,
            IsZeroDept_Cover = 0,
            IsNCBProtection_Cover = 0,
            IsEngGearBox_Cover = 0,
            NumberOfEmployees = 0,
            Owner_Driver_Nominee_Name = string.Empty,
            Owner_Driver_Nominee_Age = 0,
            Owner_Driver_Nominee_Relationship = string.Empty,
            Owner_Driver_Appointee_Name = string.Empty,
            Owner_Driver_Appointee_Relationship = string.Empty,
            OtherLoadDiscRate = 0.0,
            IsPrivateUseLoading = 0,
            IsLimitedtoOwnPremises = 0,
            NoOfFPP = 0,
            NoOfNFPP = 0,
            fuel_type = "",
            IsFibertank = 0,
            ISHired_vehicles = 0,
            NoOfTrailers = 0,
            NoOfWorkmen = 0,
            PrivateCarrier = false,
            TrailerChassisNo = "",
            TrailerIDV = 0,
        };
        return req_GCV;
    }
    private static Req_MISD GetMISDDetails(QuoteQueryModel quoteQuery)
    {
        bool isVehicleAgeLessThan3Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 3);
        bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 5);

        int paCoverCleanerConductorCoolie = 0;
        if (quoteQuery.PACover.IsUnnamedCleaner)
            paCoverCleanerConductorCoolie++;
        if (quoteQuery.PACover.IsUnnamedConductor)
            paCoverCleanerConductorCoolie++;
        if (quoteQuery.PACover.IsUnnamedHelper)
            paCoverCleanerConductorCoolie++;

        var req_MISD = new Req_MISD()
        {
            POSP_CODE = string.Empty,
            BreakIN_ID = string.Empty,
            BreakInStatus = string.Empty,
            BreakinInspectionDate = null,
            //PACover
            Effectivedrivinglicense = !quoteQuery.PACover.IsUnnamedOWNERDRIVER,
            NumberOfDrivers = quoteQuery.PACover.IsPaidDriver ? 1 : 0,
            NoOfCleanerConductorCoolies = paCoverCleanerConductorCoolie,
            Paiddriver_Si = paCoverCleanerConductorCoolie > 0 ? 100000 : 0,
            //Discounts
            TPPDLimit = quoteQuery.Discounts.IsLimitedTPCoverage ? 6000 : 0,
            AntiTheftDiscFlag = quoteQuery.Discounts.IsAntiTheft ? true : false,
            //Accessroies
            BiFuelType = quoteQuery.Accessories.IsCNG ? "CNG" : null,
            BiFuel_Kit_Value = quoteQuery.Accessories.IsCNG ? quoteQuery.Accessories.CNGValue : 0,
            ElecticalAccessoryIDV = quoteQuery.Accessories.IsElectrical ? quoteQuery.Accessories.ElectricalValue : 0,
            NonElecticalAccessoryIDV = quoteQuery.Accessories.IsNonElectrical ? quoteQuery.Accessories.NonElectricalValue : 0,
            //Addons
            IsInclusionofIMT23 = quoteQuery.AddOns.IsIMT23 ? 1 : 0,
            ExtensionCountryCode = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsGeoAreaExtension) ? Convert.ToDouble(quoteQuery.GeogExtensionCode) : 0,
            ExtensionCountryName = (isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsGeoAreaExtension && !string.IsNullOrEmpty(quoteQuery.GeogExtension)) ? (quoteQuery.GeogExtension).Trim() : null,
            IsZeroDept_Cover = 0,
            NumberOfEmployees = 0,
            Owner_Driver_Nominee_Name = string.Empty,
            Owner_Driver_Nominee_Age = 0,
            Owner_Driver_Nominee_Relationship = string.Empty,
            Owner_Driver_Appointee_Name = string.Empty,
            Owner_Driver_Appointee_Relationship = string.Empty,
            OtherLoadDiscRate = 0.0,
            IsPrivateUseLoading = 0,
            IsLimitedtoOwnPremises = 0,
            IsOverTurningLoading = 0,
            NoOfTrailers = 0,
            TrailerChassisNo = "",
            TrailerIDV = 0,
        };
        return req_MISD;
    }
    private static int SetTpaTenure(bool isUnnamedOwnerDriver, bool isBrancdNewVehicle, int vehicleAge)
    {
        if (isUnnamedOwnerDriver && isBrancdNewVehicle)
        {
            return vehicleAge;
        }
        else if (isUnnamedOwnerDriver)
        {
            return 1;
        }

        return 0;
    }
    private static bool IsApplicable(object _val)
    {
        string val = Convert.ToString(_val);
        return !(string.IsNullOrEmpty(val) || val == "0" || Convert.ToDecimal(val) == 0);
    }
    private static bool IsYearGreaterThanValue(DateTime registrationDate, int yearCheck)
    {
        double year = (DateTime.Now - registrationDate).ToYear();
        return year <= yearCheck;
    }
    public async Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        QuoteConfirmResponseModel quoteResponseVM = new QuoteConfirmResponseModel();
        bool isSelfInspection = false;
        bool isCurrPolicyRTICover = false;
        bool isCurrPolicyPartDept = false;
        bool isPolicyTypeSelfInspection = false;
        var vehicleNumber = string.Join("-", VehicleNumberSplit(quoteConfirmCommand.VehicleNumber));

        var tokenAndTransactionId = await GetToken(quoteConfirmCommand.VehicleTypeId, quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId, "QuoteConfirm", quoteConfirmCommand.CategoryId, quoteTransactionDbModel.LeadDetail.LeadID, cancellationToken);

        HDFCServiceRequestModel requestBody = JsonConvert.DeserializeObject<HDFCServiceRequestModel>(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody);

        string transactionId = requestBody.TransactionID;
        string policyStartDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        string policyEndDate = DateTime.ParseExact(Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), "dd/MM/yyyy", CultureInfo.InvariantCulture)
            .AddYears(1).AddDays(-1).ToString("dd/MM/yyyy");
        string newPremium = string.Empty;
        string mnufacturingYear = string.Empty;

        if (quoteConfirmCommand.PolicyDates.ManufacturingDate != null && quoteConfirmCommand.PolicyDates.ManufacturingDate.Length > 4)
        {
            mnufacturingYear = quoteConfirmCommand.PolicyDates.ManufacturingDate.Substring(quoteConfirmCommand.PolicyDates.ManufacturingDate.Length - 4);
        }
        else if (quoteConfirmCommand.PolicyDates.ManufacturingDate != null && quoteConfirmCommand.PolicyDates.ManufacturingDate.Length == 4)
        {
            mnufacturingYear = quoteConfirmCommand.PolicyDates.ManufacturingDate;
        }
        requestBody.Policy_Details.PolicyStartDate = policyStartDate;
        requestBody.Policy_Details.PolicyEndDate = quoteConfirmCommand.IsBrandNewVehicle ? policyEndDate : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        requestBody.Policy_Details.DateofDeliveryOrRegistration = quoteConfirmCommand.IsBrandNewVehicle ? DateTime.Today.AddDays(-2).ToString("dd/MM/yyyy") :
            Convert.ToDateTime(quoteConfirmCommand.PolicyDates.RegistrationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        requestBody.Policy_Details.DateofFirstRegistration = quoteTransactionDbModel.LeadDetail.IsBrandNew ? DateTime.Today.AddDays(-2).ToString("dd/MM/yyyy") :
            Convert.ToDateTime(quoteConfirmCommand.PolicyDates.RegistrationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        requestBody.Policy_Details.YearOfManufacture = mnufacturingYear;
        requestBody.Policy_Details.EngineNumber = quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine != null ? quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine : requestBody.Policy_Details.EngineNumber;
        requestBody.Policy_Details.ChassisNumber = quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis != null ? quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis : requestBody.Policy_Details.ChassisNumber;
        requestBody.Policy_Details.Registration_No = quoteConfirmCommand.VehicleNumber.Length > 4 ? vehicleNumber : null;
        requestBody.Policy_Details.PreviousPolicy_IsRTI_Cover = quoteConfirmCommand.isPrevPolicyRTICover;
        requestBody.Policy_Details.PreviousPolicy_IsZeroDept_Cover = quoteConfirmCommand.isPrevPolicyNilDeptCover;

        if (quoteConfirmCommand.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
        {
            isCurrPolicyPartDept = requestBody.Req_TW.IsZeroDept_Cover == 1;
            isCurrPolicyRTICover = requestBody.Req_TW.IsRTI_Cover == 1;
            requestBody.Req_TW.Effectivedrivinglicense = quoteConfirmCommand.IsPACover;
            requestBody.Req_TW.CPA_Tenure = !quoteConfirmCommand.IsPACover ? Convert.ToInt32(quoteConfirmCommand.PACoverTenure) : 0;
        }
        else if (quoteConfirmCommand.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
        {
            isCurrPolicyPartDept = requestBody.Req_PvtCar.IsZeroDept_Cover == 1;
            isCurrPolicyRTICover = requestBody.Req_PvtCar.IsRTI_Cover == 1;
            requestBody.Req_PvtCar.Effectivedrivinglicense = quoteConfirmCommand.IsPACover;
            requestBody.Req_PvtCar.CPA_Tenure = !quoteConfirmCommand.IsPACover ? Convert.ToInt32(quoteConfirmCommand.PACoverTenure) : 0;
        }
        else if(quoteConfirmCommand.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
        {
            if (quoteConfirmCommand.CategoryId.Equals("1"))
            {
                isCurrPolicyPartDept = requestBody.Req_GCV.IsZeroDept_Cover == 1;
                requestBody.Req_GCV.Effectivedrivinglicense = quoteConfirmCommand.IsPACover;
            }
            else if (quoteConfirmCommand.CategoryId.Equals("2"))
            {
                isCurrPolicyPartDept = requestBody.Req_PCV.IsZeroDept_Cover == 1;
                isCurrPolicyRTICover = requestBody.Req_PCV.IsRTI_Cover == 1;
                requestBody.Req_PCV.Effectivedrivinglicense = quoteConfirmCommand.IsPACover;
            }
            else if (quoteConfirmCommand.CategoryId.Equals("3"))
            {
                isCurrPolicyPartDept = requestBody.Req_MISD.IsZeroDept_Cover == 1;
                requestBody.Req_MISD.Effectivedrivinglicense = quoteConfirmCommand.IsPACover;
            }
        }

        if (!quoteConfirmCommand.IsBrandNewVehicle && quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy)
        {
            if (quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive))
            {
                requestBody.Policy_Details.PreviousPolicy_CorporateCustomerId_Mandatary = quoteTransactionDbModel?.QuoteConfirmDetailsModel?.SAODInsurerCode;
                requestBody.Policy_Details.PreviousPolicy_PolicyEndDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                requestBody.Policy_Details.PreviousPolicy_PolicyStartDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyStartDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                requestBody.Policy_Details.PreviousPolicy_PolicyNo = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
                requestBody.Policy_Details.PreviousPolicy_PolicyClaim = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? "YES" : "NO";
                requestBody.Policy_Details.PreviousPolicy_NCBPercentage = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? 0 : Convert.ToInt32(quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue);

                if (quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId.Equals(_policyTypeConfig.SATP))
                {
                    requestBody.Policy_Details.PreviousPolicy_TPENDDATE = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    requestBody.Policy_Details.PreviousPolicy_TPSTARTDATE = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyStartDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    requestBody.Policy_Details.PreviousPolicy_TPINSURER = quoteTransactionDbModel?.QuoteConfirmDetailsModel?.SAODInsurerCode;
                    requestBody.Policy_Details.PreviousPolicy_TPPOLICYNO = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
                }
            }
            else if (quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD))
            {
                requestBody.Policy_Details.PreviousPolicy_CorporateCustomerId_Mandatary = quoteTransactionDbModel.QuoteConfirmDetailsModel.SAODInsurerCode;
                requestBody.Policy_Details.PreviousPolicy_PolicyEndDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                requestBody.Policy_Details.PreviousPolicy_PolicyStartDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyStartDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                requestBody.Policy_Details.PreviousPolicy_PolicyNo = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
                requestBody.Policy_Details.PreviousPolicy_PolicyClaim = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? "YES" : "NO";
                requestBody.Policy_Details.PreviousPolicy_NCBPercentage = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? 0 : Convert.ToInt32(quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue);

                requestBody.Policy_Details.PreviousPolicy_TPENDDATE = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                requestBody.Policy_Details.PreviousPolicy_TPSTARTDATE = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                requestBody.Policy_Details.PreviousPolicy_TPINSURER = quoteTransactionDbModel?.QuoteConfirmDetailsModel?.SATPInsurerCode;
                requestBody.Policy_Details.PreviousPolicy_TPPOLICYNO = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP;
            }
            else
            {
                requestBody.Policy_Details.PreviousPolicy_CorporateCustomerId_Mandatary = quoteTransactionDbModel?.QuoteConfirmDetailsModel?.SATPInsurerCode;
                requestBody.Policy_Details.PreviousPolicy_PolicyEndDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                requestBody.Policy_Details.PreviousPolicy_PolicyStartDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                requestBody.Policy_Details.PreviousPolicy_PolicyNo = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP;
                requestBody.Policy_Details.PreviousPolicy_PolicyClaim = "NO";
                requestBody.Policy_Details.PreviousPolicy_NCBPercentage = 0;

                requestBody.Policy_Details.PreviousPolicy_TPENDDATE = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                requestBody.Policy_Details.PreviousPolicy_TPSTARTDATE = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                requestBody.Policy_Details.PreviousPolicy_TPINSURER = quoteTransactionDbModel?.QuoteConfirmDetailsModel?.SATPInsurerCode;
                requestBody.Policy_Details.PreviousPolicy_TPPOLICYNO = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP;
            }
        }

        var requestBodyFraming = JsonConvert.SerializeObject(requestBody);
        _logger.LogInformation("HDFC Quote Confirm Response {request}", requestBodyFraming);
        HttpResponseMessage confirmQuote = await GetQuoteResponse(false, quoteConfirmCommand.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical), quoteTransactionDbModel.LeadDetail.LeadID, requestBody, tokenAndTransactionId.Token, tokenAndTransactionId.ProductCode, "QuoteConfirm", cancellationToken);
        QuoteConfirmDetailsResponseModel quoteConfirm = new QuoteConfirmDetailsResponseModel();

        string responseBody = string.Empty;
        string commonResponse = quoteTransactionDbModel.QuoteTransactionRequest.CommonResponse;
        QuoteResponseModel updatedResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(commonResponse);

        var leadId = quoteTransactionDbModel.LeadDetail.LeadID;

        var stream = await confirmQuote.Content.ReadAsStreamAsync(cancellationToken);
        var confirmQuoteResult = stream.DeserializeFromJson<HDFCServiceResponseModel>();
        responseBody = JsonConvert.SerializeObject(confirmQuoteResult);
        _logger.LogInformation("HDFC Quote Confirm Response {responseBody}", responseBody);

        if (!quoteConfirmCommand.IsBrandNewVehicle)
        {
            if (!string.IsNullOrEmpty(quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId))
            {
                if (quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId.Equals(_policyTypeConfig.SATP) && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive))
                {
                    isPolicyTypeSelfInspection = true;
                }
            }
            if ((!quoteConfirmCommand.isPrevPolicyRTICover && isCurrPolicyRTICover) || (!quoteConfirmCommand.isPrevPolicyNilDeptCover && isCurrPolicyPartDept) || isPolicyTypeSelfInspection)
            {
                isSelfInspection = true;
            }
        }

        if (isPolicyTypeSelfInspection || isSelfInspection)
        {
            quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.OK;
            quoteConfirm.ValidationMessage = BreakInMessage;
            quoteConfirm.IsBreakin = true;
            quoteConfirm.IsSelfInspection = true;
            quoteConfirm.isNavigateToQuote = true;
        }
        else
        {
            if (confirmQuote.IsSuccessStatusCode)
            {
                if (confirmQuoteResult.StatusCode.Equals(200))
                {
                    double ncbPercentage = 0;
                    if (quoteConfirmCommand.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
                    {
                        if (confirmQuoteResult.Resp_TW.NCBBonusDisc_Premium > 0)
                        {
                            ncbPercentage = (confirmQuoteResult.Resp_TW.NCBBonusDisc_Premium / ((confirmQuoteResult.Resp_TW.Basic_OD_Premium + confirmQuoteResult.Resp_TW.Electical_Acc_Premium + confirmQuoteResult.Resp_TW.NonElectical_Acc_Premium + confirmQuoteResult.Resp_TW.BiFuel_Kit_OD_Premium + confirmQuoteResult.Resp_TW.GeogExtension_ODPremium) - (confirmQuoteResult.Resp_TW.Automobile_Disc_premium + confirmQuoteResult.Resp_TW.VoluntartDisc_premium))) * 100;
                        }
                        newPremium = Convert.ToString(Math.Round(confirmQuoteResult.Resp_TW.Total_Premium));

                        quoteConfirm = new QuoteConfirmDetailsResponseModel
                        {
                            InsurerStatusCode = (int)HttpStatusCode.OK,
                            InsurerName = _hdfcConfig.InsurerName,
                            NewPremium = newPremium,
                            InsurerId = _hdfcConfig.InsurerId,
                            IDV = Convert.ToInt32(Math.Round(confirmQuoteResult.Resp_TW.IDV)),
                            NCB = Convert.ToString(Convert.ToDecimal(Math.Round(ncbPercentage))),
                            Tax = new ServiceTaxModel
                            {
                                totalTax = Convert.ToString(Math.Round(confirmQuoteResult.Resp_TW.Service_Tax))
                            },
                            TotalPremium = Convert.ToString(Math.Round(confirmQuoteResult.Resp_TW.Net_Premium)),
                            GrossPremium = Convert.ToString(Math.Round(confirmQuoteResult.Resp_TW.Total_Premium)),
                        };
                    }
                    else if (quoteConfirmCommand.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
                    {
                        if (confirmQuoteResult.Resp_PvtCar.BreakIN_Premium <= 0)
                        {
                            if (confirmQuoteResult.Resp_PvtCar.NCBBonusDisc_Premium > 0)
                            {
                                ncbPercentage = (confirmQuoteResult.Resp_PvtCar.NCBBonusDisc_Premium / ((confirmQuoteResult.Resp_PvtCar.Basic_OD_Premium + confirmQuoteResult.Resp_PvtCar.Electical_Acc_Premium + confirmQuoteResult.Resp_PvtCar.NonElectical_Acc_Premium + confirmQuoteResult.Resp_PvtCar.BiFuel_Kit_OD_Premium + confirmQuoteResult.Resp_PvtCar.BreakIN_Premium + confirmQuoteResult.Resp_PvtCar.GeogExtension_ODPremium) - (confirmQuoteResult.Resp_PvtCar.Automobile_Disc_premium + confirmQuoteResult.Resp_PvtCar.VoluntartDisc_premium))) * 100;
                            }
                            newPremium = Convert.ToString(Math.Round(confirmQuoteResult.Resp_PvtCar.Total_Premium));

                            quoteConfirm = new QuoteConfirmDetailsResponseModel
                            {
                                InsurerStatusCode = (int)HttpStatusCode.OK,
                                InsurerName = _hdfcConfig.InsurerName,
                                NewPremium = newPremium,
                                InsurerId = _hdfcConfig.InsurerId,
                                IDV = Convert.ToInt32(Math.Round(confirmQuoteResult.Resp_PvtCar.IDV)),
                                NCB = Convert.ToString(Convert.ToDecimal(Math.Round(ncbPercentage))),
                                Tax = new ServiceTaxModel
                                {
                                    totalTax = Convert.ToString(Math.Round(confirmQuoteResult.Resp_PvtCar.Service_Tax))
                                },
                                TotalPremium = Convert.ToString(Math.Round(confirmQuoteResult.Resp_PvtCar.Net_Premium)),
                                GrossPremium = Convert.ToString(Math.Round(confirmQuoteResult.Resp_PvtCar.Total_Premium)),
                            };
                        }
                        else
                        {
                            _logger.LogInformation("HDFC Response {responseBody}", responseBody);
                            quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.OK;
                            quoteConfirm.ValidationMessage = BreakInMessage;
                            quoteConfirm.IsBreakin = true;
                            quoteConfirm.IsSelfInspection = true;
                            quoteConfirm.isNavigateToQuote = true;
                        }
                    }
                    else if (quoteConfirmCommand.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                    {
                        double breakInPremium = 0.0, ncbBonusDiscPremium = 0.0, basicODPremium = 0.0, electicalAccPremium = 0.0,nonElecticalAccPremium = 0.0,
                               biFuelKitODPremium = 0.0,geogExtensionODPremium = 0.0, totalPremium = 0.0,idv = 0.0,netPremium = 0.0,serviceTax = 0.0;
                        if (confirmQuoteResult.Resp_PCV != null)
                        {
                            breakInPremium = confirmQuoteResult.Resp_PCV.BreakIN_Premium;
                            ncbBonusDiscPremium = confirmQuoteResult.Resp_PCV.NCBBonusDisc_Premium;
                            basicODPremium = confirmQuoteResult.Resp_PCV.Basic_OD_Premium;
                            electicalAccPremium = confirmQuoteResult.Resp_PCV.Electical_Acc_Premium;
                            nonElecticalAccPremium = confirmQuoteResult.Resp_PCV.NonElectical_Acc_Premium;
                            biFuelKitODPremium = confirmQuoteResult.Resp_PCV.BiFuel_Kit_OD_Premium;
                            geogExtensionODPremium = confirmQuoteResult.Resp_PCV.GeogExtension_ODPremium;
                            totalPremium = confirmQuoteResult.Resp_PCV.Total_Premium;
                            idv = confirmQuoteResult.Resp_PCV.IDV;
                            netPremium = confirmQuoteResult.Resp_PCV.Net_Premium;
                            serviceTax = confirmQuoteResult.Resp_PCV.Service_Tax;
                        }
                        else if (confirmQuoteResult.Resp_GCV != null)
                        {
                            breakInPremium = confirmQuoteResult.Resp_GCV.BreakIN_Premium;
                            ncbBonusDiscPremium = confirmQuoteResult.Resp_GCV.NCBBonusDisc_Premium;
                            basicODPremium = confirmQuoteResult.Resp_GCV.Basic_OD_Premium;
                            electicalAccPremium = confirmQuoteResult.Resp_GCV.Electical_Acc_Premium;
                            nonElecticalAccPremium = confirmQuoteResult.Resp_GCV.NonElectical_Acc_Premium;
                            biFuelKitODPremium = confirmQuoteResult.Resp_GCV.BiFuel_Kit_OD_Premium;
                            geogExtensionODPremium = confirmQuoteResult.Resp_GCV.GeogExtension_ODPremium;
                            totalPremium = confirmQuoteResult.Resp_GCV.Total_Premium;
                            idv = confirmQuoteResult.Resp_GCV.IDV;
                            netPremium = confirmQuoteResult.Resp_GCV.Net_Premium;
                            serviceTax = confirmQuoteResult.Resp_GCV.Service_Tax;
                        }
                        else if (confirmQuoteResult.Resp_MISD != null)
                        {
                            breakInPremium = confirmQuoteResult.Resp_MISD.BreakIN_Premium;
                            ncbBonusDiscPremium = confirmQuoteResult.Resp_MISD.NCBBonusDisc_Premium;
                            basicODPremium = confirmQuoteResult.Resp_MISD.Basic_OD_Premium;
                            electicalAccPremium = confirmQuoteResult.Resp_MISD.Electical_Acc_Premium;
                            nonElecticalAccPremium = confirmQuoteResult.Resp_MISD.NonElectical_Acc_Premium;
                            biFuelKitODPremium = confirmQuoteResult.Resp_MISD.BiFuel_Kit_OD_Premium;
                            geogExtensionODPremium = confirmQuoteResult.Resp_MISD.GeogExtension_ODPremium;
                            totalPremium = confirmQuoteResult.Resp_MISD.Total_Premium;
                            idv = confirmQuoteResult.Resp_MISD.IDV;
                            netPremium = confirmQuoteResult.Resp_MISD.Net_Premium;
                            serviceTax = confirmQuoteResult.Resp_MISD.Service_Tax;
                        }

                        if (!quoteConfirmCommand.IsBrandNewVehicle && DateTime.ParseExact(requestBody.Policy_Details.PreviousPolicy_PolicyEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) < DateTime.Now)
                        {
                            _logger.LogInformation("HDFC Response {responseBody}", responseBody);
                            quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.OK;
                            quoteConfirm.ValidationMessage = BreakInMessage;
                            quoteConfirm.IsBreakin = true;
                            quoteConfirm.IsSelfInspection = true;
                            quoteConfirm.isNavigateToQuote = true;
                        }
                        else
                        {
                            if (ncbBonusDiscPremium > 0)
                            {
                                ncbPercentage = (ncbBonusDiscPremium / ((basicODPremium + electicalAccPremium + nonElecticalAccPremium + biFuelKitODPremium + breakInPremium + geogExtensionODPremium))) * 100;
                            }
                            newPremium = Convert.ToString(Math.Round(totalPremium));

                            quoteConfirm = new QuoteConfirmDetailsResponseModel
                            {
                                InsurerStatusCode = (int)HttpStatusCode.OK,
                                InsurerName = _hdfcConfig.InsurerName,
                                NewPremium = newPremium,
                                InsurerId = _hdfcConfig.InsurerId,
                                IDV = Convert.ToInt32(Math.Round(idv)),
                                NCB = Convert.ToString(Convert.ToDecimal(Math.Round(ncbPercentage))),
                                Tax = new ServiceTaxModel
                                {
                                    totalTax = Convert.ToString(Math.Round(serviceTax))
                                },
                                TotalPremium = Convert.ToString(Math.Round(netPremium)),
                                GrossPremium = Convert.ToString(Math.Round(totalPremium)),
                            };
                        }
                    }

                    updatedResponse.GrossPremium = newPremium;
                }
            }
            else
            {
                if (confirmQuoteResult.StatusCode.Equals(400) && !string.IsNullOrEmpty(confirmQuoteResult.Error))
                {
                    quoteConfirm.ValidationMessage = confirmQuoteResult.Error;
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
            LeadId = leadId,
            TransactionId = transactionId
        };
        return quoteResponseVM;
    }
    public async Task<(string Token, string Expiry)> GetCKYCToken(string LeadId,CancellationToken cancellationToken)
    {
        var quoteVm = new HDFCCkycTokenResponseModel();
        try
        {
            var defaultRequestHeaders = _client.DefaultRequestHeaders;
            defaultRequestHeaders.Clear();
            defaultRequestHeaders.Add("api_key", _hdfcConfig.Kyc_Key);
            var id = 0;
            string url = _hdfcConfig.CKYCTokenURL;
            id = await InsertICLogs(string.Empty, LeadId, url, string.Empty, JsonConvert.SerializeObject(defaultRequestHeaders), "KYC");
            var Res = await _client.GetAsync(url);

            if (Res.IsSuccessStatusCode)
            {
                string json = await Res.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<HDFCCkycTokenResponseModel>(json);
                if (result != null && result.success.Equals(true))
                {
                    UpdateICLogs(id, result.data.token, json);
                    return (result.data.token, result.data.expiry);
                }
            }
            return default;
        }
        catch (Exception ex)
        {

            _logger.LogError("HDFC CKYC Token Error {exception}", ex.Message);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return default;
        }
    }
    public async Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(HDFCCKYCCommand hdfcCKYCCommand, CancellationToken cancellationToken)
    {
        string responseBody = string.Empty;
        string requestBody = string.Empty;
        SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();

        CreateLeadModel createLeadModel = new CreateLeadModel();
        try
        {
            var getToken = await GetCKYCToken(hdfcCKYCCommand.LeadId, cancellationToken);
            string searchTypeL = string.Empty;
            string searchValueU = string.Empty;
            string dob_value = Convert.ToDateTime(hdfcCKYCCommand?.DateOfBirth).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            switch (hdfcCKYCCommand.DocumentType)
            {
                case ("Pan"):
                    searchTypeL = "pan";
                    searchValueU = hdfcCKYCCommand.DocumentId.ToUpper();
                    break;
                case ("Aadhaar"):
                    searchTypeL = "aadhar_uid";
                    searchValueU = (hdfcCKYCCommand.DocumentId).Substring(Math.Max(0, hdfcCKYCCommand.DocumentId.Length - 4)).ToUpper();
                    break;
                case ("CKYC No"):
                    searchTypeL = "ckyc_number";
                    searchValueU = hdfcCKYCCommand.DocumentId.ToUpper();
                    break;
                default:
                    break;
            }
            var id = 0;
            string url = _hdfcConfig.FetchKYCURL + searchTypeL + "=" + searchValueU + "&dob=" + dob_value + "&redirect_url=" + _hdfcConfig.PGCKYCURL + hdfcCKYCCommand.QuoteTransactionId + "/" + _applicationClaims.GetUserId();
            var client = new RestClient(url);
            var request = new RestRequest(string.Empty, Method.Get);
            request.AddHeader("token", getToken.Token);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            id = await InsertICLogs(string.Empty, hdfcCKYCCommand.LeadId, url, getToken.Token, JsonConvert.SerializeObject(request), "KYC");
            try
            {
                var result = await client.ExecuteAsync(request, cancellationToken);
                requestBody = client.BuildUri(request).ToString();
                _logger.LogInformation("HDFC CKYC POI Request {Request}", requestBody);
                responseBody = result.Content;
                _logger.LogInformation("HDFC CKYC POI Request {Response}", responseBody);


                var response = JsonConvert.DeserializeObject<HDFCCkycFetchModel>(responseBody);

                if (response != null && response.success.Equals(true) && response.data.iskycVerified.Equals(1))
                {
                    DateTime date = DateTime.ParseExact(response.data.dob, "dd/MM/yyyy", null);
                    string dob = date.ToString("yyyy-MM-dd");
                    var namesList = response.data.name.Split(' ');
                    createLeadModel.LeadName = namesList.Length > 0 ? namesList[0] : string.Empty;
                    createLeadModel.LastName = namesList.Length > 1 ? namesList[1] : string.Empty;
                    createLeadModel.MiddleName = namesList.Length > 2 ? namesList[2] : string.Empty;
                    createLeadModel.DOB = dob;
                    createLeadModel.PhoneNumber = response.data.mobile;
                    createLeadModel.Email = response.data.email;
                    createLeadModel.PANNumber = response.data.pan;
                    createLeadModel.ckycNumber = response.data.ckycNumber;
                    createLeadModel.kyc_id = response.data.kyc_id;
                    createLeadModel.CKYCstatus = response.data.status;
                    createLeadModel.PermanentAddress = new LeadAddressModel
                    {
                        AddressType = "PRIMARY",
                        Address1 = response.data?.permanentAddress1,
                        Address2 = response.data?.permanentAddress2,
                        Address3 = response.data?.permanentAddress3,
                        Pincode = response.data?.permanentPincode
                    };
                    saveCKYCResponse.Name = response.data.name;
                    saveCKYCResponse.LastName = namesList.Length > 1 ? namesList[1] : string.Empty;
                    saveCKYCResponse.MiddleName = namesList.Length > 2 ? namesList[2] : string.Empty;
                    saveCKYCResponse.DOB = response.data.dob;
                    saveCKYCResponse.Address = response.data.permanentAddress;
                    saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                    saveCKYCResponse.Message = KYC_SUCCESS;
                    saveCKYCResponse.IsKYCRequired = true;
                    saveCKYCResponse.InsurerName = InsurerName;
                    return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
                }
                else if (response != null && response.success.Equals(true) && response.data.iskycVerified.Equals(0))
                {
                    saveCKYCResponse.KYC_Status = POA_REQUIRED;
                    saveCKYCResponse.Message = POA_REQUIRED;
                    saveCKYCResponse.IsKYCRequired = true;
                    saveCKYCResponse.redirect_link = response.data.redirect_link;
                    saveCKYCResponse.InsurerName = InsurerName;
                    return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
                }
                saveCKYCResponse.KYC_Status = FAILED;
                saveCKYCResponse.Message = MESSAGE;
                saveCKYCResponse.InsurerName = InsurerName;
                saveCKYCResponse.IsKYCRequired = true;
                UpdateICLogs(id, hdfcCKYCCommand?.QuoteTransactionId, responseBody);
                return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
            }
            catch (Exception ex)
            {
                UpdateICLogs(id, hdfcCKYCCommand?.QuoteTransactionId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            saveCKYCResponse.KYC_Status = FAILED;
            saveCKYCResponse.Message = MESSAGE;
            saveCKYCResponse.InsurerName = InsurerName;
            _logger.LogError("HDFC Get CKYC Error {exception}", ex.Message);
            return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
        }
    }
    public async Task<HDFCUploadDocumentResponseModel> GetCKYCPOAResponse(string transactionId, string kycId, string leadId, CancellationToken cancellationToken)
    {
        HDFCUploadDocumentResponseModel uploadDocumentVm = new HDFCUploadDocumentResponseModel();
        string responseBody = string.Empty;
        string requestBody = string.Empty;
        var id = 0;
        UploadCKYCDocumentResponse uploadCKYCDocumentVM = new UploadCKYCDocumentResponse();
        CreateLeadModel createLeadModel = new CreateLeadModel();

        try
        {
            var getToken = await GetCKYCToken(leadId ,cancellationToken);
            string searchTypeL = "kyc_id";
            string searchValueU = kycId.ToUpper();

            var client = new RestClient(_hdfcConfig.FetchKYCURL + searchTypeL + "=" + searchValueU + "&redirect_url=" + _hdfcConfig.PGCKYCURL + transactionId + "/" + _applicationClaims.GetUserId());


            var request = new RestRequest(string.Empty, Method.Get);
            request.AddHeader("token", getToken.Token);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            id = await InsertICLogs(request.ToString(), leadId, _hdfcConfig.FetchKYCURL, getToken.Token, JsonConvert.SerializeObject(request), "KYC");
            try
            {
                var result = await client.ExecuteAsync(request, cancellationToken);
                requestBody = client.BuildUri(request).ToString();
                _logger.LogInformation("HDFC CKYC POI Request {Request}", requestBody);
                responseBody = result.Content;
                _logger.LogInformation("HDFC CKYC POI Request {Response}", responseBody);

                uploadDocumentVm = new HDFCUploadDocumentResponseModel()
                {
                    RequestBody = requestBody,
                    ResponseBody = responseBody
                };

                var response = JsonConvert.DeserializeObject<HDFCCkycFetchModel>(responseBody);

                if (response != null && response.success.Equals(true) && response.data.iskycVerified.Equals(1))
                {
                    DateTime date = DateTime.ParseExact(response.data.dob, "dd/MM/yyyy", null);
                    string dob = date.ToString("yyyy-MM-dd");
                    var namesList = response.data.name.Split(' ');
                    createLeadModel.LeadName = namesList.Length > 0 ? namesList[0] : string.Empty;
                    createLeadModel.LastName = namesList.Length > 1 ? namesList[1] : string.Empty;
                    createLeadModel.MiddleName = namesList.Length > 2 ? namesList[2] : string.Empty;
                    createLeadModel.DOB = dob;
                    createLeadModel.PhoneNumber = response.data.mobile;
                    createLeadModel.Email = response.data.email;
                    createLeadModel.PANNumber = response.data.pan;
                    createLeadModel.ckycNumber = response.data.ckycNumber;
                    createLeadModel.kyc_id = response.data.kyc_id;
                    createLeadModel.CKYCstatus = response.data.status;
                    createLeadModel.PermanentAddress = new LeadAddressModel
                    {
                        AddressType = "PRIMARY",
                        Address1 = response.data?.permanentAddress1,
                        Address2 = response.data?.permanentAddress2,
                        Address3 = response.data?.permanentAddress3,
                        Pincode = response.data?.permanentPincode
                    };
                    uploadCKYCDocumentVM.Name = namesList.Length > 0 ? namesList[0] : string.Empty;
                    uploadCKYCDocumentVM.LastName = namesList.Length > 1 ? namesList[1] : string.Empty;
                    uploadCKYCDocumentVM.MiddleName = namesList.Length > 2 ? namesList[2] : string.Empty;
                    uploadCKYCDocumentVM.DOB = response.data.dob;
                    uploadCKYCDocumentVM.Address = response.data.permanentAddress;
                    uploadCKYCDocumentVM.CKYCStatus = POA_SUCCESS;
                    uploadCKYCDocumentVM.Message = POA_SUCCESS;
                    uploadCKYCDocumentVM.KYCId = kycId;
                    uploadCKYCDocumentVM.TransactionId = transactionId;
                    uploadCKYCDocumentVM.InsurerName = InsurerName;

                    uploadDocumentVm.uploadCKYCDocumentResponse = uploadCKYCDocumentVM;
                    uploadDocumentVm.createLeadModel = createLeadModel;
                    return uploadDocumentVm;
                }
                else if (response != null && response.success.Equals(true) && response.data.iskycVerified.Equals(0))
                {
                    uploadCKYCDocumentVM.CKYCStatus = FAILED;
                    uploadCKYCDocumentVM.Message = MESSAGE;
                    uploadCKYCDocumentVM.KYCId = kycId;
                    uploadCKYCDocumentVM.TransactionId = transactionId;
                    uploadCKYCDocumentVM.InsurerName = InsurerName;
                }
                uploadCKYCDocumentVM.CKYCStatus = FAILED;
                uploadCKYCDocumentVM.Message = MESSAGE;
                uploadCKYCDocumentVM.KYCId = kycId;
                uploadCKYCDocumentVM.TransactionId = transactionId;
                uploadCKYCDocumentVM.InsurerName = InsurerName;

                uploadDocumentVm.uploadCKYCDocumentResponse = uploadCKYCDocumentVM;
                uploadDocumentVm.createLeadModel = createLeadModel;
                UpdateICLogs(id, transactionId, responseBody);
                return uploadDocumentVm;
            }
            catch (Exception ex)
            {
                UpdateICLogs(id, transactionId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            uploadCKYCDocumentVM.CKYCStatus = FAILED;
            uploadCKYCDocumentVM.Message = MESSAGE;
            uploadCKYCDocumentVM.KYCId = kycId;
            uploadCKYCDocumentVM.TransactionId = transactionId;
            uploadCKYCDocumentVM.InsurerName = InsurerName;
            _logger.LogError("HDFC Upload CKYC Error {exception}", ex.Message);
            uploadDocumentVm.uploadCKYCDocumentResponse = uploadCKYCDocumentVM;
            uploadDocumentVm.createLeadModel = createLeadModel;
            return uploadDocumentVm;
        }
    }
    public async Task<HDFCCkycPOAStatusResponseModel> GetCKYCPOAStatus(string kycId,string leadId, CancellationToken cancellationToken)
    {
        string responseBody = string.Empty;
        string requestBody = string.Empty;
        var id = 0;
        HDFCCkycPOAStatusResponseModel kycResModel = new HDFCCkycPOAStatusResponseModel();
        try
        {
            var getToken = await GetCKYCToken(leadId ,cancellationToken);

            var client = new RestClient(_hdfcConfig.FetchKYCPOAURL + kycId);
            var request = new RestRequest(string.Empty, Method.Get);
            request.AddHeader("token", getToken.Token);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            id = await InsertICLogs(JsonConvert.SerializeObject(request), leadId, _hdfcConfig.FetchKYCPOAURL, getToken.Token, JsonConvert.SerializeObject(request), "KYC");
            try
            {
                var result = client.Execute(request);

                requestBody = client.BuildUri(request).ToString();
                _logger.LogInformation("Get CKYC POA Status Request Body", requestBody);

                if (result.IsSuccessStatusCode)
                {
                    responseBody = result.Content;
                    _logger.LogInformation("Get CKYC POA Status Response Body", responseBody);
                    kycResModel = JsonConvert.DeserializeObject<HDFCCkycPOAStatusResponseModel>(responseBody);
                }
                UpdateICLogs(id, kycId, responseBody);
                return kycResModel;
            }
            catch (Exception ex)
            {
                UpdateICLogs(id, kycId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("GoDigit Error {exception}", ex.Message);
            return kycResModel;
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
    public async Task<ProposalResponseModel> CreateProposal(HDFCServiceRequestModel proposalQuery, HDFCProposalRequest proposalRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken)
    {
        ProposalResponseModel proposalResponseVM = new ProposalResponseModel();
        HDFCServiceResponseModel proposalResponseDto = new HDFCServiceResponseModel();
        var proposalVm = new QuoteResponseModel();
        string requestBody = string.Empty;
        string responseBody = string.Empty;
        bool isBreakIn = false;
        bool isSelfInspection = false;
        bool isCommercial = false;
        string cvCategory = string.Empty;
        try
        {
            string dob = Convert.ToDateTime(proposalRequest.PersonalDetails.dateOfBirth, CultureInfo.InvariantCulture).ToString("dd/MM/yyyy").Replace('-', '/');
            proposalQuery.Customer_Details = new Customer_Details
            {
                Customer_Type = createLeadModel.CarOwnedBy,
                Customer_Pehchaan_id = createLeadModel.kyc_id,
                Customer_FirstName = proposalRequest.PersonalDetails.firstName,
                Customer_LastName = proposalRequest.PersonalDetails.lastName,
                Customer_DateofBirth = dob,
                Customer_Email = proposalRequest.PersonalDetails.emailId,
                Customer_Mobile = proposalRequest.PersonalDetails.mobile,
                Customer_PanNo = proposalRequest.PersonalDetails.panNumber,
                Customer_Salutation = proposalRequest.PersonalDetails.salutation,
                Customer_Gender = proposalRequest.PersonalDetails.gender,
                Customer_Perm_Address1 = proposalRequest.AddressDetails.perm_address,
                Customer_Perm_CityDistrictCode = proposalRequest.AddressDetails.perm_city,
                Customer_Perm_StateCode = proposalRequest.AddressDetails.perm_state,
                Customer_Perm_PinCode = proposalRequest.AddressDetails.perm_pincode,
                Customer_Mailing_Address1 = proposalRequest.AddressDetails.mail_address,
                Customer_Mailing_CityDistrictCode = proposalRequest.AddressDetails.mail_city,
                Customer_Mailing_StateCode = proposalRequest.AddressDetails.mail_state,
                Customer_Mailing_PinCode = proposalRequest.AddressDetails.mail_pincode
            };

            proposalQuery.Policy_Details.EngineNumber = proposalRequest.VehicleDetails.engineNumber;
            proposalQuery.Policy_Details.ChassisNumber = proposalRequest.VehicleDetails.chassisNumber;
            proposalQuery.Policy_Details.FinancierCode = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails.financer : null;
            proposalQuery.Policy_Details.BranchName = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails.branch : null;
            proposalQuery.Policy_Details.AgreementType = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails.agreementType : null;

            if (proposalQuery.Req_PvtCar != null)
            {
                proposalQuery.Req_PvtCar.POSP_CODE = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetPOSPId() : string.Empty;
                proposalQuery.Req_PvtCar.Owner_Driver_Nominee_Name = proposalRequest.NomineeDetails.nomineeName;
                proposalQuery.Req_PvtCar.Owner_Driver_Nominee_Age = Convert.ToInt32(proposalRequest.NomineeDetails.nomineeAge);
                proposalQuery.Req_PvtCar.Owner_Driver_Nominee_Relationship = proposalRequest.NomineeDetails.nomineeRelation;
            }
            else if (proposalQuery.Req_TW != null)
            {
                proposalQuery.Req_TW.POSP_CODE = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetPOSPId() : string.Empty;
                proposalQuery.Req_TW.Owner_Driver_Nominee_Name = proposalRequest.NomineeDetails.nomineeName;
                proposalQuery.Req_TW.Owner_Driver_Nominee_Age = Convert.ToInt32(proposalRequest.NomineeDetails.nomineeAge);
                proposalQuery.Req_TW.Owner_Driver_Nominee_Relationship = proposalRequest.NomineeDetails.nomineeRelation;
            }
            else
            {
                if (proposalQuery.Req_GCV != null)
                {
                    cvCategory = "1";
                    proposalQuery.Req_GCV.POSP_CODE = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetPOSPId() : string.Empty;
                    proposalQuery.Req_GCV.Owner_Driver_Nominee_Name = proposalRequest.NomineeDetails.nomineeName;
                    proposalQuery.Req_GCV.Owner_Driver_Nominee_Age = Convert.ToInt32(proposalRequest.NomineeDetails.nomineeAge);
                    proposalQuery.Req_GCV.Owner_Driver_Nominee_Relationship = proposalRequest.NomineeDetails.nomineeRelation;
                }
                else if (proposalQuery.Req_PCV != null)
                {
                    cvCategory = "2";
                    proposalQuery.Req_PCV.POSP_CODE = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetPOSPId() : string.Empty;
                    proposalQuery.Req_PCV.Owner_Driver_Nominee_Name = proposalRequest.NomineeDetails.nomineeName;
                    proposalQuery.Req_PCV.Owner_Driver_Nominee_Age = Convert.ToInt32(proposalRequest.NomineeDetails.nomineeAge);
                    proposalQuery.Req_PCV.Owner_Driver_Nominee_Relationship = proposalRequest.NomineeDetails.nomineeRelation;
                }
                else if (proposalQuery.Req_MISD != null)
                {
                    cvCategory = "3";
                    proposalQuery.Req_MISD.POSP_CODE = _applicationClaims.GetRole() == "POSP" ? _applicationClaims.GetPOSPId() : string.Empty;
                    proposalQuery.Req_MISD.Owner_Driver_Nominee_Name = proposalRequest.NomineeDetails.nomineeName;
                    proposalQuery.Req_MISD.Owner_Driver_Nominee_Age = Convert.ToInt32(proposalRequest.NomineeDetails.nomineeAge);
                    proposalQuery.Req_MISD.Owner_Driver_Nominee_Relationship = proposalRequest.NomineeDetails.nomineeRelation;
                }

                isCommercial = true;
                proposalQuery.Payment_Details.BANK_NAME = "BIZDIRECT";
                proposalQuery.Payment_Details.BANK_BRANCH_NAME = "Andheri";
                proposalQuery.Payment_Details.PAYMENT_MODE_CD = "EP";
                proposalQuery.Payment_Details.PAYER_TYPE = _applicationClaims.GetRole() == "POSP" ? "DEALER" : "CUSTOMER";
                proposalQuery.Payment_Details.PAYMENT_AMOUNT = createLeadModel.GrossPremium;
                proposalQuery.Payment_Details.PAYMENT_DATE = DateTime.Now.ToString("dd/MM/yyyy").Replace('-', '/');
            }

            requestBody = JsonConvert.SerializeObject(proposalQuery);
            _logger.LogInformation("HDFC create proposal {request}", requestBody);

            var tokenAndTransactionId = await GetToken(createLeadModel.VehicleTypeId, createLeadModel.PolicyTypeId, "Proposal", cvCategory, createLeadModel.LeadID, cancellationToken);
            var proposalResponse = await GetQuoteResponse(true, isCommercial, createLeadModel.LeadID, proposalQuery, tokenAndTransactionId.Token, tokenAndTransactionId.ProductCode, "Proposal", cancellationToken);

            if (!proposalResponse.IsSuccessStatusCode)
            {
                var data = await proposalResponse.Content.ReadAsStringAsync(cancellationToken);
                proposalResponseDto = JsonConvert.DeserializeObject<HDFCServiceResponseModel>(data);
                if ((proposalResponseDto.StatusCode.Equals(400) || proposalResponseDto.StatusCode.Equals(404)) && !string.IsNullOrEmpty(proposalResponseDto.Error))
                {
                    proposalVm.ValidationMessage = proposalResponseDto.Error;
                }
                else
                {
                    proposalVm.ValidationMessage = ValidationMessage;
                }
                responseBody = JsonConvert.SerializeObject(proposalResponseDto);
                _logger.LogInformation("create proposal {responseBody}", responseBody);
                proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                var data = await proposalResponse.Content.ReadAsStringAsync(cancellationToken);
                proposalResponseDto = JsonConvert.DeserializeObject<HDFCServiceResponseModel>(data);
                if (proposalResponseDto != null)
                {
                    responseBody = JsonConvert.SerializeObject(proposalResponseDto);
                    _logger.LogInformation("create proposal {responseBody}", responseBody);
                    if (proposalResponseDto.StatusCode.Equals(200))
                    {
                        string totalPremium = "0";
                        string grossPremium = "0";
                        string tax = "0";
                        if(proposalResponseDto.Resp_PvtCar is not null || proposalResponseDto.Resp_TW is not null)
                        {
                            totalPremium = Math.Round(proposalResponseDto.Policy_Details.NetPremium).ToString();
                            grossPremium = Math.Round(proposalResponseDto.Policy_Details.TotalPremium).ToString();
                            tax = Math.Round(proposalResponseDto.Policy_Details.ServiceTax).ToString();
                        }
                        else if (proposalQuery.Req_PCV is not null)
                        {
                            if(proposalResponseDto.Resp_PCV is not null)
                            {
                                totalPremium = Math.Round(proposalResponseDto.Resp_PCV.Net_Premium).ToString();
                                grossPremium = Math.Round(proposalResponseDto.Resp_PCV.Total_Premium).ToString();
                                tax = Math.Round(proposalResponseDto.Resp_PCV.Service_Tax).ToString();
                            }
                            else
                            {
                                totalPremium = Math.Round(proposalResponseDto.Policy_Details.NetPremium).ToString();
                                grossPremium = Math.Round(proposalResponseDto.Policy_Details.TotalPremium).ToString();
                                tax = Math.Round(proposalResponseDto.Policy_Details.ServiceTax).ToString();
                            }
                        }
                        else if (proposalQuery.Req_GCV is not null)
                        {
                            if (proposalResponseDto.Resp_GCV is not null)
                            {
                                totalPremium = Math.Round(proposalResponseDto.Resp_GCV.Net_Premium).ToString();
                                grossPremium = Math.Round(proposalResponseDto.Resp_GCV.Total_Premium).ToString();
                                tax = Math.Round(proposalResponseDto.Resp_GCV.Service_Tax).ToString();
                            }
                            else
                            {
                                totalPremium = Math.Round(proposalResponseDto.Policy_Details.NetPremium).ToString();
                                grossPremium = Math.Round(proposalResponseDto.Policy_Details.TotalPremium).ToString();
                                tax = Math.Round(proposalResponseDto.Policy_Details.ServiceTax).ToString();
                            }
                        }
                        else if (proposalQuery.Req_MISD is not null)
                        {
                            if (proposalResponseDto.Resp_MISD is not null)
                            {
                                totalPremium = Math.Round(proposalResponseDto.Resp_MISD.Net_Premium).ToString();
                                grossPremium = Math.Round(proposalResponseDto.Resp_MISD.Total_Premium).ToString();
                                tax = Math.Round(proposalResponseDto.Resp_MISD.Service_Tax).ToString();
                            }
                            else
                            {
                                totalPremium = Math.Round(proposalResponseDto.Policy_Details.NetPremium).ToString();
                                grossPremium = Math.Round(proposalResponseDto.Policy_Details.TotalPremium).ToString();
                                tax = Math.Round(proposalResponseDto.Policy_Details.ServiceTax).ToString();
                            }
                        }

                        proposalVm = new QuoteResponseModel
                        {
                            InsurerName = _hdfcConfig.InsurerName,
                            InsurerStatusCode = (int)HttpStatusCode.OK,
                            Tax = new ServiceTax()
                            {
                                totalTax = tax,
                            },
                            TotalPremium = totalPremium,
                            GrossPremium = grossPremium,
                            TransactionID = proposalResponseDto.TransactionID,
                            CustomerId = proposalResponseDto.Customer_Details.CustomerID,
                            ProposalNumber = proposalResponseDto.Policy_Details.ProposalNumber,
                            PolicyNumber = null,
                            IsBreakIn = isBreakIn,
                            IsSelfInspection = isSelfInspection
                        };
                    }
                    else
                        proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                    proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
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
            _logger.LogError("HDFC Proposal Error {exception}", ex.Message);
            proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return proposalResponseVM;
        }
    }
    public async Task<HDFCSubmitPOSPCertificateResponse> CreatePOSP(HDFCCreateIMBrokerRequestDto hDFCCreateIMBrokerRequest, CancellationToken cancellationToken)
    {
        var requestBody = string.Empty;
        DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
        string transationID = Convert.ToString(now.ToUnixTimeSeconds());
        var tokenAndTransactionId = await GetToken("2d566966-5525-4ed7-bd90-bb39e8418f39", "517D8F9C-F532-4D45-8034-ABECE46693E3",string.Empty, string.Empty, hDFCCreateIMBrokerRequest.LeadId, cancellationToken);

        DateTime curDate = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
        DateTime curDateAdd5Years = curDate.AddYears(5).AddDays(-1);
        var id = 0;
        string startDate = curDate.ToString("dd/MM/yyyy");
        string endDate = curDateAdd5Years.ToString("dd/MM/yyyy");
        try
        {
            var request = new HDFCCreatePOSPRequest
            {
                TransactionID = transationID,
                Req_POSP = new ReqPOSP
                {
                    NUM_MOBILE_NO = hDFCCreateIMBrokerRequest.MobileNumber,
                    ADHAAR_CARD = hDFCCreateIMBrokerRequest.AadharNumber,
                    EMAILID = hDFCCreateIMBrokerRequest.EmailId,
                    NAME = hDFCCreateIMBrokerRequest.Name,
                    PAN_CARD = hDFCCreateIMBrokerRequest.PanNumber,
                    STATE = hDFCCreateIMBrokerRequest.State,
                    UNIQUE_CODE = hDFCCreateIMBrokerRequest.PospId,
                    START_DT = startDate,
                    END_DT = endDate,
                    REGISTRATION_NO = hDFCCreateIMBrokerRequest.PanNumber
                }
            };
            requestBody = JsonConvert.SerializeObject(request);
            _logger.LogError("Issue with Create POSP {response}", requestBody);


            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Token", tokenAndTransactionId.Token);
            _client.DefaultRequestHeaders.Add("TRANSACTIONID", transationID);
            _client.DefaultRequestHeaders.Add("PRODUCT_CODE", "2311");
            _client.DefaultRequestHeaders.Add("CHANNEL_ID", _hdfcConfig.CommanHeaderData.CHANNEL_ID);
            _client.DefaultRequestHeaders.Add("SOURCE", _hdfcConfig.CommanHeaderData.SOURCE);
            id = await InsertICLogs(requestBody, hDFCCreateIMBrokerRequest.LeadId, _hdfcConfig.CreatePOSPURL, tokenAndTransactionId.Token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), string.Empty);
            try
            {
                var response = await _client.PostAsync(_hdfcConfig.CreatePOSPURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
               cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<HDFCSubmitPOSPCertificateResponse>();
                    UpdateICLogs(id, transationID, response.Content.ToString());
                    _logger.LogInformation("Create POSP {response}", JsonConvert.SerializeObject(result));
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<HDFCSubmitPOSPCertificateResponse>();
                    _logger.LogInformation("Create POSP {response}", JsonConvert.SerializeObject(result));
                    UpdateICLogs(id, transationID, response.Content.ToString());
                    return result;
                }
                return default;
            }
            catch (Exception ex)
            {
                UpdateICLogs(id, transationID, ex.Message);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("HDFC Submit POSP Error {exception}", ex.Message);
            return default;
        }
    }
    public async Task<string> GeneratePaymentCheckSum(string transactionId, string amount, string redirectionURL, string leadId, CancellationToken cancellationToken)
    {
        string checkSumValueId = string.Empty;
        var id = 0;
        try
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _hdfcConfig.GenerateCheckSumURL);
            
            try
            {
                var content = new StringContent("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n    " +
               "<soap:Body>\r\n        " +
               "<GenerateRequestChecksum xmlns=\"http://hdfcergo.com/\">\r\n            " +
               "<TransactionNo>" + transactionId + "</TransactionNo>\r\n            " +
               "<TotalAmount>" + amount + "</TotalAmount>\r\n            " +
               "<AppID>" + _hdfcConfig.AppId + "</AppID>\r\n            " +
               "<SubscriptionID>" + _hdfcConfig.SubscriptionId + "</SubscriptionID>\r\n            " +
               "<SuccessUrl>" + redirectionURL + "</SuccessUrl>\r\n            " +
               "<FailureUrl>" + redirectionURL + "</FailureUrl>\r\n            " +
               "<Source>POST</Source>\r\n        " +
               "</GenerateRequestChecksum>\r\n    " +
               "</soap:Body>\r\n</soap:Envelope>\r\n", null, "text/xml");

                id = await InsertICLogs(await content.ReadAsStringAsync(cancellationToken), leadId, _hdfcConfig.GenerateCheckSumURL, string.Empty, string.Empty, "Payment");
                _logger.LogInformation("HDFC Generate CheckSum {request}", content);
                request.Content = content;
                var response = await client.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStreamAsync(cancellationToken);
                    HDFCGenerateCheckSumResponse checkSum = new HDFCGenerateCheckSumResponse();
                    XmlSerializer xmlSerializer = new XmlSerializer(checkSum.GetType());

                    using (StreamReader reader = new StreamReader(res))
                    {
                        var resDesiralize = (HDFCGenerateCheckSumResponse)xmlSerializer.Deserialize(reader);
                        _logger.LogInformation("HDFC Generate CheckSum {response}", JsonConvert.SerializeObject(resDesiralize));
                        checkSumValueId = resDesiralize.Body.GenerateRequestChecksumResponse.GenerateRequestChecksumResult;
                        UpdateICLogs(id, transactionId, JsonConvert.SerializeObject(resDesiralize));
                    }
                }

                return checkSumValueId;
            }
            catch (Exception ex)
            {
                UpdateICLogs(id, transactionId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("HDFC Proposal Error {exception}", ex.Message);
            return checkSumValueId;
        }
    }
    public async Task<HDFCPolicyDocumentResponseModel> GetPolicyDocument(HDFCPolicyRequestModel paymentFieldModel, CancellationToken cancellationToken)
    {
        var policyResponse = new HDFCPolicyDocumentResponseModel();
        var requestBody = string.Empty;
        var id = 0;
        var tokenAndTransactionId = await GetToken(paymentFieldModel.VehicleTypeId, paymentFieldModel.PolicyTypeId, "Payment", Convert.ToString(paymentFieldModel.CategoryId), paymentFieldModel.LeadId, cancellationToken);
        try
        {
            var submitPaymentDetails = await SubmitPaymentDetails(paymentFieldModel, tokenAndTransactionId.ProductCode, tokenAndTransactionId.Token, cancellationToken);

            if (submitPaymentDetails != null && submitPaymentDetails.InsurerStatusCode.Equals(200))
            {
                policyResponse.PolicyNumber = submitPaymentDetails.PolicyNumber;
                policyResponse.CustomerId = submitPaymentDetails.CustomerId;


                var request = new HDFCPolicyDocumentRequest
                {
                    TransactionID = paymentFieldModel.TransactionId,
                    Req_Policy_Document = new ReqPolicyDocument
                    {
                        Policy_Number = submitPaymentDetails.PolicyNumber
                    }
                };
                requestBody = JsonConvert.SerializeObject(request);
                _logger.LogError("HDFC policy documents {request}", requestBody);

                var defaultRequestHeaders = _client.DefaultRequestHeaders;
                defaultRequestHeaders.Clear();
                defaultRequestHeaders.Add("SOURCE", _hdfcConfig.CommanHeaderData.SOURCE);
                defaultRequestHeaders.Add("CHANNEL_ID", _hdfcConfig.CommanHeaderData.CHANNEL_ID);
                defaultRequestHeaders.Add("PRODUCT_CODE", tokenAndTransactionId.ProductCode);
                defaultRequestHeaders.Add("Token", tokenAndTransactionId.Token);
                id = await InsertICLogs(requestBody, paymentFieldModel.LeadId, _hdfcConfig.PolicyDocumentURL, tokenAndTransactionId.Token, JsonConvert.SerializeObject(defaultRequestHeaders), "Payment");
                try
                {
                    var response = await _client.PostAsJsonAsync(_hdfcConfig.PolicyDocumentURL, request, cancellationToken: cancellationToken);
                    if (!response.IsSuccessStatusCode)
                    {
                        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                        var result = stream.DeserializeFromJson<HDFCPolicyDocumentResponseDto>();
                        _logger.LogInformation("Policy Document {response}", JsonConvert.SerializeObject(result));
                        policyResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                        var result = stream.DeserializeFromJson<HDFCPolicyDocumentResponseDto>();
                        _logger.LogInformation("Policy Document {response}", JsonConvert.SerializeObject(result));
                        policyResponse.PolicyDocumentBase64 = result?.Resp_Policy_Document?.PDF_BYTES;
                        policyResponse.InsurerStatusCode = (int)HttpStatusCode.OK;
                        UpdateICLogs(id, paymentFieldModel.TransactionId, JsonConvert.SerializeObject(result));
                    }
                }
                catch (Exception ex)
                {
                    UpdateICLogs(id, paymentFieldModel.TransactionId, ex.Message);
                    return default;
                }
            }
            else
            {
                policyResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            }
            return policyResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError("HDFC Policy Download Error {exception}", ex.Message);
            return default;
        }
    }
    public async Task<HDFCPolicyDocumentResponseModel> SubmitPaymentDetails(HDFCPolicyRequestModel hdfcPolicyReqModel, string productCode, string token, CancellationToken cancellationToken)
    {
        HDFCPolicyDocumentResponseModel paymentTaggingResponseVM = new HDFCPolicyDocumentResponseModel();
        HDFCSubmitPaymentResponseDto hdfcResponseDto = new HDFCSubmitPaymentResponseDto();
        string requestBody = string.Empty;
        string responseBody = string.Empty;
        var id = 0;
        try
        {
            var paymentDetails = new Payment_Detailss()
            {
                BANK_NAME = "BIZDIRECT",
                BANK_BRANCH_NAME = "BHANDUP",
                PAYMENT_MODE_CD = "EP",
                PAYER_TYPE = _applicationClaims.GetRole() == "POSP" ? "DEALER" : "CUSTOMER",
                PAYMENT_AMOUNT = Convert.ToDecimal(hdfcPolicyReqModel.GrossPremium),
                INSTRUMENT_NUMBER = hdfcPolicyReqModel.ApplicationId,
                PAYMENT_DATE = hdfcPolicyReqModel.PaymentDate,
                OTC_Transaction_No = string.Empty,
                IsReserved = 0,
                IsPolicyIssued = "0"
            };
            var paymentReq = new HDFCSubmitPaymentRequestDto()
            {
                TransactionID = hdfcPolicyReqModel.TransactionId,
                Proposal_no = hdfcPolicyReqModel.ProposalNumber,
                Payment_Details = paymentDetails
            };
            requestBody = JsonConvert.SerializeObject(paymentReq);
            _logger.LogInformation("Submit Payment Details {requestBody}", requestBody);


            var defaultRequestHeaders = _client.DefaultRequestHeaders;
            defaultRequestHeaders.Clear();
            defaultRequestHeaders.Add("SOURCE", _hdfcConfig.CommanHeaderData.SOURCE);
            defaultRequestHeaders.Add("CHANNEL_ID", _hdfcConfig.CommanHeaderData.CHANNEL_ID);
            defaultRequestHeaders.Add("PRODUCT_CODE", productCode);
            defaultRequestHeaders.Add("Token", token);
            id = await InsertICLogs(requestBody, hdfcPolicyReqModel.LeadId, _hdfcConfig.SubmitPaymentDetailsURL, token, JsonConvert.SerializeObject(defaultRequestHeaders), "Payment");
            try
            {
                var responseMessage = await _client.PostAsJsonAsync(_hdfcConfig.SubmitPaymentDetailsURL, paymentReq, cancellationToken: cancellationToken);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                    hdfcResponseDto = stream.DeserializeFromJson<HDFCSubmitPaymentResponseDto>();
                    responseBody = JsonConvert.SerializeObject(hdfcResponseDto);
                    _logger.LogInformation("Submit Payment Details {responseBody}", responseBody);
                    paymentTaggingResponseVM.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                    hdfcResponseDto = stream.DeserializeFromJson<HDFCSubmitPaymentResponseDto>();
                    if (hdfcResponseDto != null)
                    {
                        responseBody = JsonConvert.SerializeObject(hdfcResponseDto);
                        _logger.LogInformation("create proposal {responseBody}", responseBody);

                        if (hdfcResponseDto.StatusCode.Equals(200))
                        {
                            paymentTaggingResponseVM = new HDFCPolicyDocumentResponseModel()
                            {
                                InsurerStatusCode = (int)HttpStatusCode.OK,
                                CustomerId = hdfcResponseDto.Customer_Details.CustomerID,
                                ProposalNumber = hdfcResponseDto.Policy_Details.ProposalNumber,
                                PolicyNumber = hdfcResponseDto.Policy_Details.PolicyNumber,
                                PaymentID = hdfcResponseDto.Payment_Details.PaymentID,
                                TransactionID = hdfcResponseDto.TransactionID
                            };
                        }
                        else
                            paymentTaggingResponseVM.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    else
                        paymentTaggingResponseVM.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                }
                UpdateICLogs(id, hdfcPolicyReqModel.TransactionId, responseBody);
                return paymentTaggingResponseVM;
            }
            catch (Exception ex)
            {
                UpdateICLogs(id, hdfcPolicyReqModel.TransactionId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("HDFC Payment Tagging Error {exception}", ex.Message);
            paymentTaggingResponseVM.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return paymentTaggingResponseVM;
        }
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
    private async Task<int> InsertICLogs(string requestBody, string leadId, string api, string token, string header, string stage)
    {
        var logsModel = new LogsModel
        {
            InsurerId = _hdfcConfig.InsurerId,
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