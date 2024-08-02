using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Features.ICICI.Command.CKYC;
using Insurance.Core.Features.ICICI.Command.UploadICICICKYCDocument;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.ICICI;
using Insurance.Domain.ICICI.Request;
using Insurance.Domain.ICICI.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Persistence.ICIntegration.Implementation
{
    public class ICICIService : IICICIService
    {
        private readonly ILogger<ICICIService> _logger;
        private readonly HttpClient _client;
        private QuoteQueryModel quoteQueryModel;
        private readonly ICICIConfig _iCICIConfig;
        private readonly IApplicationClaims _applicationClaims;
        private readonly VehicleTypeConfig _vehicleTypeConfig;
        private readonly PolicyTypeConfig _policyTypeConfig;
        private readonly ICommonService _commonService;
        private const string KYC_SUCCESS = "KYC_SUCCESS";
        private const string POA_SUCCESS = "POA_SUCCESS";
        private const string FAILED = "FAILED";
        private const string POA_REQUIRED = "POA_REQUIRED";
        private const string SUCCESS = "Success";
        private const string MESSAGE = "Please enter correct document number or proceed with other insurer";
        private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";
        private readonly IMapper _mapper;

        public ICICIService(ILogger<ICICIService> logger,
                             HttpClient client,
                             IOptions<ICICIConfig> options,
                             IApplicationClaims applicationClaims,
                             IOptions<VehicleTypeConfig> vehicleTypeConfig,
                             IOptions<PolicyTypeConfig> policyTypeConfig,
                             ICommonService commonService
            , IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _iCICIConfig = options.Value;
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
            _vehicleTypeConfig = vehicleTypeConfig.Value;
            _policyTypeConfig = policyTypeConfig.Value;
            _commonService = commonService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
        {
            quoteQueryModel = quoteQuery;
            var quoteVm = new QuoteResponseModel();
            string requestBody = string.Empty;
            var responseBody = string.Empty;
            string policyType = quoteQuery.CurrentPolicyType;
            bool isVehicleAgeLessThan3Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 3);
            bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 5);
            string paTenure = "1";
            string dealId = string.Empty;

            try
            {
                quoteVm.InsurerName = _iCICIConfig.InsurerName;
                var previousPolicyDetails = new PreviousPolicyDetails();

                //dealId = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "DealId").Select(x => x.ConfigValue).FirstOrDefault();
                if (!_applicationClaims.GetRole().Equals("POSP"))
                {
                    dealId = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "DealId").Select(x => x.ConfigValue).FirstOrDefault();
                }

                if (!quoteQuery.IsBrandNewVehicle)
                {
                    if (!(quoteQuery.PreviousPolicyDetails != null && !string.IsNullOrEmpty(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD)))
                    {
                        quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP;
                        quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP;
                    }
                    previousPolicyDetails = new PreviousPolicyDetails
                    {
                        previousPolicyStartDate = quoteQuery.PreviousPolicyDetails?.PreviousPolicyStartDateSAOD,
                        previousPolicyEndDate = quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD,
                        ClaimOnPreviousPolicy = (quoteQuery.PreviousPolicyDetails.IsClaimInLastYear) ? 1 : 0,
                        NoOfClaimsOnPreviousPolicy = (quoteQuery.PreviousPolicyDetails.IsClaimInLastYear) ? 1 : 0,
                        PreviousPolicyType = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == ICICIEnum.PreviousPolicyType).Select(x => x.ConfigValue).FirstOrDefault(),
                        BonusOnPreviousPolicy = quoteQuery.PreviousPolicyDetails.PreviousNoClaimBonus,
                        PreviousVehicleSaleDate = quoteQuery.RegistrationDate,
                        PreviousPolicyNumber = quoteQuery.PreviousPolicyDetails.PreviousPolicyNumber,
                        PreviousInsurerName = quoteQuery.PreviousPolicyDetails.PreviousSAODInsurer.Split(" ")[0]
                    };
                }
                else
                {
                    previousPolicyDetails = null;
                    if (quoteQuery.VehicleDetails.IsTwoWheeler)
                    {
                        paTenure = "5";
                    }
                    else
                        paTenure = "3";
                }
                var iCICIRequest = new ICICIRequestDto
                {
                    CorrelationId = quoteQuery.CorrelationId,
                    BusinessType = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == ICICIEnum.BusinessType).Select(x => x.ConfigValue).FirstOrDefault(),
                    DealId = dealId,
                    VehicleMakeCode = quoteQuery.VehicleDetails.VehicleMakeCode,
                    VehicleModelCode = quoteQuery.VehicleDetails.VehicleModelCode,
                    RTOLocationCode = quoteQuery.RTOLocationCode,
                    ExShowRoomPrice = quoteQuery.ExShowRoomPrice,
                    IsNoPrevInsurance = quoteQuery.IsBrandNewVehicle ? true : false,
                    ManufacturingYear = quoteQuery.RegistrationYear,
                    DeliveryOrRegistrationDate = quoteQuery.IsBrandNewVehicle ? DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") : quoteQuery.RegistrationDate,
                    FirstRegistrationDate = quoteQuery.IsBrandNewVehicle ? DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") : quoteQuery.RegistrationDate,
                    IsTransferOfNCB = false,
                    TransferOfNCBPercent = 0,
                    IsVehicleHaveLPG = false,
                    IsVehicleHaveCNG = quoteQuery.Accessories.IsCNG,
                    SIVehicleHaveLPG_CNG = quoteQuery.Accessories.CNGValue,
                    IsFiberGlassFuelTank = false,
                    PolicyStartDate = quoteQuery.PolicyStartDate,
                    PolicyEndDate = quoteQuery.PolicyEndDate,
                    CustomerType = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "CustomerType").Select(x => x.ConfigValue).FirstOrDefault(),
                    IsHaveElectricalAccessories = quoteQuery.Accessories.IsElectrical,
                    IsHaveNonElectricalAccessories = quoteQuery.Accessories.IsNonElectrical,
                    SIHaveElectricalAccessories = quoteQuery.Accessories.IsElectrical ? quoteQuery.Accessories.ElectricalValue : 0,
                    SIHaveNonElectricalAccessories = quoteQuery.Accessories.IsNonElectrical ? quoteQuery.Accessories.NonElectricalValue : 0,
                    TPPDLimit = quoteQuery.VehicleDetails.VehicleType == "2-Wheeler" ? "6000" : "750000",
                    IsLegalLiabilityToPaidDriver = quoteQuery.PACover.IsPaidDriver,
                    IsLegalLiabilityToPaidEmployee = false,
                    NoOfEmployee = 0,
                    NoOfDriver = quoteQuery.PACover.IsPaidDriver ? 1 : 0,
                    IsLegaLiabilityToWorkmen = false,
                    NoOfWorkmen = 0,
                    IsPACoverUnnamedPassenger = quoteQuery.PACover.IsUnnamedPassenger,
                    SIPACoverUnnamedPassenger = quoteQuery.PACover.IsUnnamedPassenger ? Convert.ToDecimal(quoteQuery.UnnamedPassangerAndPillonRider) : 0,
                    IsValidDrivingLicense = false,
                    IsMoreThanOneVehicle = false,
                    IsPACoverOwnerDriver = quoteQuery.PACover.IsUnnamedOWNERDRIVER,
                    IsVoluntaryDeductible = quoteQuery.Discounts.IsVoluntarilyDeductible,
                    VoluntaryDeductiblePlanName = quoteQuery.Discounts.IsVoluntarilyDeductible ? quoteQuery.VehicleDetails.IsTwoWheeler ? "500" : quoteQuery.VoluntaryExcess : null,
                    IsAutomobileAssocnFlag = quoteQuery.Discounts.IsAAMemberShip,
                    IsAntiTheftDisc = quoteQuery.Discounts.IsAntiTheft,
                    IsHandicapDisc = quoteQuery.Discounts.IsHandicapDisc,
                    IsExtensionCountry = quoteQuery.AddOns.IsGeoAreaExtension && !quoteQuery.GeogExtension.Equals("Srilanka") ? true : false,
                    ExtensionCountryName = quoteQuery.AddOns.IsGeoAreaExtension && !quoteQuery.GeogExtension.Equals("Srilanka") ? quoteQuery.GeogExtension : null,
                    IsGarageCash = false,
                    GarageCashPlanName = "",
                    ZeroDepPlanName = isVehicleAgeLessThan5Years ? quoteQuery.AddOns.IsZeroDebt ?
                        quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "ZeroDepPlanName").Select(x => x.ConfigValue).FirstOrDefault() : string.Empty : string.Empty,
                    IsRTIApplicableflag = quoteQuery.AddOns.IsInvoiceCoverRequired,
                    IsConsumables = quoteQuery.VehicleDetails.IsFourWheeler && isVehicleAgeLessThan5Years ? quoteQuery.AddOns.IsConsumableRequired : false,
                    IsTyreProtect = isVehicleAgeLessThan3Years ? quoteQuery.AddOns.IsTyreProtectionRequired : false,
                    IsEngineProtectPlus = isVehicleAgeLessThan5Years && quoteQuery.AddOns.IsEngineProtectionRequired,
                    RSAPlanName = quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired ?
                        quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "RSAPlanName").Select(x => x.ConfigValue).FirstOrDefault() : "",
                    KeyProtectPlan = quoteQuery.AddOns.IsKeyAndLockProtectionRequired ?
                        quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "KeyProtectPlan").Select(x => x.ConfigValue).FirstOrDefault() : "",
                    LossOfPersonalBelongingPlanName = quoteQuery.AddOns.IsZeroDebt && quoteQuery.AddOns.IsPersonalBelongingRequired ?
                        quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "LossOfPersonalBelongingPlanName").Select(x => x.ConfigValue).FirstOrDefault() : "",
                    OtherLoading = 0.0,
                    OtherDiscount = 0.0,
                    GSTToState = quoteQuery.GSTToState,
                    Tenure = quoteQuery.VehicleODTenure,
                    TPTenure = quoteQuery.VehicleTPTenure,
                    PACoverTenure = Convert.ToInt32(paTenure),
                    IsPACoverWaiver = quoteQuery.PACover.IsUnnamedOWNERDRIVER ? false : true,
                    PreviousPolicyDetails = previousPolicyDetails,
                    TPStartDate = quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP,
                    TPEndDate = quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP,
                    TPPolicyNo = quoteQuery.PreviousPolicyDetails.PreviousPolicyNumberTP,
                    TPInsurerName = quoteQuery.PreviousPolicyDetails.PreviousSATPInsurer,
                    EngineNumber = quoteQuery.VehicleDetails.EngineNumber,
                    ChassisNumber = quoteQuery.VehicleDetails.Chassis,
                    RegistrationNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) ? quoteQuery.VehicleDetails.LicensePlateNumber + "AA" : quoteQuery.VehicleNumber,
                    CustomerDetails = new Domain.ICICI.Request.CustomerDetails
                    {
                        CustomerType = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "CustomerType").Select(x => x.ConfigValue).FirstOrDefault(),
                        CustomerName = string.IsNullOrEmpty(_applicationClaims.GetUserName()) ? "Test User" : _applicationClaims.GetUserName(),
                        DateOfBirth = string.Empty,
                        PinCode = "226022",
                        PANCardNo = _applicationClaims.GetPAN() ?? string.Empty,
                        Email = "",
                        MobileNumber = string.IsNullOrEmpty(_applicationClaims.GetMobileNo()) ? "9898989898" : _applicationClaims.GetMobileNo(),
                        AddressLine1 = "Test Address",
                        CountryCode = Convert.ToInt32(quoteQuery.CountryCode),
                        StateCode = Convert.ToInt32(quoteQuery.StateCode),
                        CityCode = Convert.ToInt32(quoteQuery.CityCode),
                        AadharNumber = _applicationClaims.GetAadhaarNumber() ?? string.Empty
                    }
                };

                requestBody = JsonConvert.SerializeObject(iCICIRequest);
                var result = await QuoteResponseFraming(requestBody, quoteQuery, quoteVm, iCICIRequest.CorrelationId.ToString(), "", cancellationToken);

                return Tuple.Create(result.Item1, requestBody, result.Item2);
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI Error {exception}", ex.Message);
                quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                return Tuple.Create(quoteVm, requestBody, responseBody);
            }
        }
        public async Task<string> GetToken(string leadId, bool quoteToken, bool idvToken, bool ckycToken, bool paymentoken, bool policyGeneration, string stage)
        {
            var id = 0;
            string scope = string.Empty;
            var responseBody = string.Empty;
            try
            {
                if (quoteToken)
                {
                    scope = _iCICIConfig.QuoteScope;
                }
                else if (idvToken)
                {
                    scope = _iCICIConfig.IdvScope;
                }
                else if (ckycToken)
                {
                    scope = _iCICIConfig.CKYCScope;
                }
                else if (paymentoken)
                {
                    scope = _iCICIConfig.PaymentScope;
                }
                else if (policyGeneration)
                {
                    scope = _iCICIConfig.PolicyGenerationScope;
                }
                else
                {
                    scope = _iCICIConfig.POSPScope;
                }
                _client.DefaultRequestHeaders.Clear();

                var defaultRequestHeaders = _client.DefaultRequestHeaders;
                if (_client.DefaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
                {
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                var dict = new Dictionary<string, string>
                {
                    { "grant_type", _iCICIConfig.Token.grant_type },
                    { "username", _iCICIConfig.Token.username },
                    { "password", _iCICIConfig.Token.password },
                    { "scope", scope },
                    { "client_id", _iCICIConfig.Token.client_id },
                    { "client_secret", _iCICIConfig.Token.client_secret }
                };

                var items = from kvp in dict
                            select kvp.Key + "=" + kvp.Value;

                var header = "{" + string.Join(",", items) + "}";

                string url = _iCICIConfig.TokenURL;
                id = await InsertICLogs(string.Empty, leadId, _iCICIConfig.BaseURL + _iCICIConfig.TokenURL, string.Empty, header, stage);
                try
                {
                    HttpResponseMessage Res = await _client.PostAsync(url, new FormUrlEncodedContent(dict));

                    if (Res.IsSuccessStatusCode)
                    {
                        string json = await Res.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ICICITokenResponse>(json);
                        responseBody = JsonConvert.SerializeObject(result);
                        if (result != null && !string.IsNullOrWhiteSpace(result.access_token))
                        {
                            await UpdateICLogs(id, scope, responseBody);
                            return result.access_token;
                        }
                    }
                    else
                    {
                        responseBody = await Res.Content.ReadAsStringAsync();
                        await UpdateICLogs(id, scope, responseBody);
                        return default;
                    }
                }
                catch (Exception ex)
                {
                    await UpdateICLogs(id, scope, ex.Message);
                    _logger.LogError("Error on GetToken {message}", ex.Message);
                }
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error on GetToken {message}", ex.Message);
                return default;
            }
        }
        public async Task<ICICIIdvResponseModel> GetIDV(QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
        {
            var quoteVm = new ICICIIdvResponseModel();
            string requestBody = string.Empty;
            var responseBody = string.Empty;
            var id = 0;

            try
            {
                _logger.LogInformation("Get ICICI IDV");
                var idvrequest = new ICICIIdvRequestModel()
                {
                    manufacturercode = Convert.ToInt32(quoteQuery.VehicleDetails.VehicleMakeCode),
                    BusinessType = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "BusinessType").Select(x => x.ConfigValue).FirstOrDefault(),
                    rtolocationcode = Convert.ToInt32(quoteQuery.RTOLocationCode),
                    DeliveryOrRegistrationDate = quoteQuery.RegistrationDate,
                    PolicyStartDate = quoteQuery.PolicyStartDate,
                    DealID = string.IsNullOrEmpty(quoteQuery.DealID) ? quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "DealId").Select(x => x.ConfigValue).FirstOrDefault() : quoteQuery.DealID,
                    vehiclemodelcode = Convert.ToInt32(quoteQuery.VehicleDetails.VehicleModelCode),
                    correlationId = quoteQuery.CorrelationId
                };

                requestBody = JsonConvert.SerializeObject(idvrequest);
                _logger.LogInformation("ICICI IDV RequestBody {RequestBody}", requestBody);
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + quoteQuery.Token);
                id = await InsertICLogs(requestBody, quoteQuery.LeadId, _iCICIConfig.BaseURL + _iCICIConfig.IdvURL, quoteQuery.Token, string.Empty, "Quote");

                try
                {
                    var responseMessage = await _client.PostAsJsonAsync(_iCICIConfig.IdvURL, idvrequest, cancellationToken: cancellationToken);

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        responseBody = responseMessage.ReasonPhrase;
                        quoteVm.StatusCode = (int)HttpStatusCode.BadRequest;
                        _logger.LogError("Data not found {responseBody}", responseBody);
                    }
                    else
                    {
                        var stream = await responseMessage.Content.ReadAsStreamAsync();
                        var result = stream.DeserializeFromJson<ICICIIdvResponseModel>();
                        responseBody = JsonConvert.SerializeObject(result);
                        if (result != null && result.statusmessage.Equals("Success"))
                        {
                            quoteQuery.MinIDV = Convert.ToDecimal(result.minidv);
                            quoteQuery.MaxIDV = Convert.ToDecimal(result.maxidv);
                            quoteQuery.RecommendedIDV = quoteQuery.IsBrandNewVehicle ? Convert.ToDecimal(result.maxidv) : Convert.ToDecimal(result.minidv);
                            quoteVm = result;
                            quoteVm.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            quoteVm.StatusCode = (int)HttpStatusCode.BadRequest;
                        }
                    }
                    await UpdateICLogs(id, quoteQuery.CorrelationId.ToString(), responseBody);
                    return quoteVm;
                }
                catch (Exception ex)
                {
                    _logger.LogError("ICICI IDV Error {exception}", ex.Message);
                    quoteVm.StatusCode = (int)HttpStatusCode.BadRequest;
                    await UpdateICLogs(id, quoteQuery.CorrelationId.ToString(), ex.Message);
                    return quoteVm;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI IDV Error {exception}", ex.Message);
                quoteVm.StatusCode = (int)HttpStatusCode.BadRequest;
                return quoteVm;
            }
        }
        public async Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
        {
            bool isCommercial = quoteConfirmCommand.PolicyDates.IsCommercial;

            //calling quote
            string token = await GetToken(quoteTransactionDbModel.LeadDetail.LeadID, true, false, false, false, false, "QuoteConfirm");
            string requestBody = string.Empty;
            string CorrelationId = string.Empty;
            string ProductCode = string.Empty;
            bool isCurrPolicyEngineCover = false;
            bool isCurrPolicyPartDept = false;
            bool isSelfInspection = false;
            bool policyTypeSelfInspection = false;
            if (!isCommercial)
            {
                ICICIRequestDto request = JsonConvert.DeserializeObject<ICICIRequestDto>(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody);

                isCurrPolicyEngineCover = request.IsEngineProtectPlus;
                isCurrPolicyPartDept = !string.IsNullOrEmpty(request.ZeroDepPlanName);
                isSelfInspection = false;
                policyTypeSelfInspection = false;
                CorrelationId = request.CorrelationId.ToString();
                ProductCode = "";
                //request.CorrelationId = Guid.NewGuid();
                request.CustomerType = quoteConfirmCommand.Customertype.Equals("INDIVIDUAL") ? "INDIVIDUAL" : "CORPORATE";
                request.CustomerDetails.CustomerType = quoteConfirmCommand.Customertype.Equals("INDIVIDUAL") ? "INDIVIDUAL" : "CORPORATE";
                request.RegistrationNumber = quoteConfirmCommand.VehicleNumber;
                request.EngineNumber = quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine != null ? quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine : request.EngineNumber;
                request.ChassisNumber = quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis != null ? quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis : request.ChassisNumber;
                request.PolicyStartDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("yyyy-MM-dd");
                request.PolicyEndDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyEndDate).ToString("yyyy-MM-dd");
                request.DeliveryOrRegistrationDate = quoteConfirmCommand.IsBrandNewVehicle ? DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") : Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("yyyy-MM-dd");
                request.FirstRegistrationDate = quoteConfirmCommand.IsBrandNewVehicle ? DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") : Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("yyyy-MM-dd");
                request.ManufacturingYear = Convert.ToDateTime(quoteConfirmCommand.ManufacturingMonthYear).ToString("yyyy-MM-dd").Substring(0, 4);
                request.IsPACoverOwnerDriver = !quoteConfirmCommand.IsPACover ? true : false;
                request.IsPACoverWaiver = !quoteConfirmCommand.IsPACover ? false : true;
                request.PACoverTenure = !quoteConfirmCommand.IsPACover && quoteConfirmCommand.PACoverTenure != null ? Convert.ToInt32(quoteConfirmCommand.PACoverTenure) : 1;

                if (!quoteConfirmCommand.IsBrandNewVehicle && quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy)
                {
                    request.PreviousPolicyDetails.PreviousPolicyNumber = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber != null ? quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber
                        : quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP;
                    request.PreviousPolicyDetails.PreviousInsurerName = quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSAODInsurerName != null ?
                        quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSAODInsurerName.Split(" ")[0] : quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSATPInsurerName.Split(" ")[0];
                    request.PreviousPolicyDetails.ClaimOnPreviousPolicy = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? 1 : 0;
                    request.PreviousPolicyDetails.NoOfClaimsOnPreviousPolicy = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? 1 : 0;
                    request.PreviousPolicyDetails.BonusOnPreviousPolicy = quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue;
                    request.PreviousPolicyDetails.previousPolicyStartDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd");
                    request.PreviousPolicyDetails.previousPolicyEndDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd");
                    if (request.PreviousPolicyDetails.PreviousPolicyType.Equals("SAOD"))
                    {
                        request.TPInsurerName = quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSATPInsurerName;
                        request.TPStartDate = quoteConfirmCommand.PolicyDates.TPPolicyStartDate != null ? Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd") : null;
                        request.TPEndDate = quoteConfirmCommand.PolicyDates.TPPolicyEndDate != null ? Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd") : null;
                        request.TPPolicyNo = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP != null ? quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP
                            : quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
                    }
                }
                requestBody = JsonConvert.SerializeObject(request);
            }
            else
            {
                ICICICVRequestDto request = JsonConvert.DeserializeObject<ICICICVRequestDto>(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody);

                isCurrPolicyEngineCover = request.IsEngineProtectPlus;
                isCurrPolicyPartDept = !string.IsNullOrEmpty(request.ZeroDepPlanName);
                isSelfInspection = false;
                policyTypeSelfInspection = false;
                CorrelationId = request.CorrelationId.ToString();
                //
                ProductCode = request.ProductCode;
                request.CustomerType = quoteConfirmCommand.Customertype.Equals("INDIVIDUAL") ? "INDIVIDUAL" : "CORPORATE";
                request.CustomerDetails.CustomerType = quoteConfirmCommand.Customertype.Equals("INDIVIDUAL") ? "INDIVIDUAL" : "CORPORATE";
                request.RegistrationNumber = quoteConfirmCommand.VehicleNumber;
                request.EngineNumber = quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine != null ? quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine : request.EngineNumber;
                request.ChassisNumber = quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis != null ? quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis : request.ChassisNumber;
                request.PolicyStartDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("yyyy-MM-dd");
                request.PolicyEndDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyEndDate).ToString("yyyy-MM-dd");
                request.DeliveryOrRegistrationDate = quoteConfirmCommand.IsBrandNewVehicle ? DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") : Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("yyyy-MM-dd");
                request.FirstRegistrationDate = quoteConfirmCommand.IsBrandNewVehicle ? DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") : Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("yyyy-MM-dd");
                request.ManufacturingYear = Convert.ToDateTime(quoteConfirmCommand.ManufacturingMonthYear).ToString("yyyy-MM-dd").Substring(0, 4);
                request.IsPACoverOwnerDriver = !quoteConfirmCommand.IsPACover ? true : false;
                request.IsPACoverWaiver = !quoteConfirmCommand.IsPACover ? false : true;
                request.PACoverTenure = !quoteConfirmCommand.IsPACover && quoteConfirmCommand.PACoverTenure != null ? Convert.ToInt32(quoteConfirmCommand.PACoverTenure) : 1;

                if (!quoteConfirmCommand.IsBrandNewVehicle && quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy)
                {
                    request.PreviousPolicyDetails.PreviousPolicyNumber = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber != null ? quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber
                        : quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP;
                    request.PreviousPolicyDetails.PreviousInsurerName = quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSAODInsurerName != null ?
                        quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSAODInsurerName.Split(" ")[0] : quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSATPInsurerName.Split(" ")[0];
                    request.PreviousPolicyDetails.ClaimOnPreviousPolicy = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? 1 : 0;
                    request.PreviousPolicyDetails.NoOfClaimsOnPreviousPolicy = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? 1 : 0;
                    request.PreviousPolicyDetails.BonusOnPreviousPolicy = quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue;
                    if (request.PreviousPolicyDetails.PreviousPolicyType.Equals("TP"))
                    {
                        request.PreviousPolicyDetails.previousPolicyStartDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd");
                        request.PreviousPolicyDetails.previousPolicyEndDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        request.PreviousPolicyDetails.previousPolicyStartDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd");
                        request.PreviousPolicyDetails.previousPolicyEndDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd");
                    }

                    //if (request.PreviousPolicyDetails.PreviousPolicyType.Equals("SAOD"))
                    //{
                    //    request.TPInsurerName = quoteTransactionDbModel.QuoteConfirmDetailsModel.PreviousSATPInsurerName;
                    //    request.TPStartDate = quoteConfirmCommand.PolicyDates.TPPolicyStartDate != null ? Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd") : null;
                    //    request.TPEndDate = quoteConfirmCommand.PolicyDates.TPPolicyEndDate != null ? Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd") : null;
                    //    request.TPPolicyNo = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP != null ? quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP
                    //        : quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
                    //}
                }
                //
                requestBody = JsonConvert.SerializeObject(request);
            }

            string responseBody = string.Empty;
            var confirmQuote = await GetQuoteResponse(quoteTransactionDbModel.QuoteConfirmDetailsModel.CurrentPolicyType, quoteConfirmCommand.PolicyDates.IsTwoWheeler,
                quoteConfirmCommand.PolicyDates.IsFourWheeler, quoteConfirmCommand.PolicyDates.IsCommercial, token, requestBody, quoteTransactionDbModel.LeadDetail.LeadID, CorrelationId, "QuoteConfirm", ProductCode, cancellationToken);
            QuoteConfirmDetailsResponseModel quoteConfirm = new QuoteConfirmDetailsResponseModel();

            string commonResponse = quoteTransactionDbModel.QuoteTransactionRequest.CommonResponse;
            QuoteResponseModel updatedResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(commonResponse);

            var leadId = quoteTransactionDbModel.LeadDetail.LeadID;
            string transactionId = null;
            ICICIResponseDto confirmQuoteResult = new ICICIResponseDto();
            ICICICVResponseDto confirmQuoteResultCV = new ICICICVResponseDto();

            if (confirmQuote.Item1.IsSuccessStatusCode)
            {
                var stream = await confirmQuote.Item1.Content.ReadAsStreamAsync(cancellationToken);
                //confirmQuoteResult = stream.DeserializeFromJson<ICICIResponseDto>();
                if (quoteConfirmCommand.PolicyDates.IsCommercial)
                {
                    confirmQuoteResultCV = stream.DeserializeFromJson<ICICICVResponseDto>();
                    confirmQuoteResult = _mapper.Map<ICICIResponseDto>(confirmQuoteResultCV);
                    responseBody = JsonConvert.SerializeObject(confirmQuoteResultCV);
                }
                else
                {
                    confirmQuoteResult = stream.DeserializeFromJson<ICICIResponseDto>();
                    responseBody = JsonConvert.SerializeObject(confirmQuoteResult);
                }
                updatedResponse.GrossPremium = confirmQuoteResult.finalPremium;
                if (!quoteConfirmCommand.IsBrandNewVehicle)
                {
                    if (quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId != null && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId != null && quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId.Equals("2AA7FDCA-9E36-4A8D-9583-15ADA737574B") && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals("517D8F9C-F532-4D45-8034-ABECE46693E3"))
                    {
                        policyTypeSelfInspection = true;
                    }
                    if ((!quoteConfirmCommand.isPrevPolicyEngineCover && isCurrPolicyEngineCover) || (!quoteConfirmCommand.isPrevPolicyNilDeptCover && isCurrPolicyPartDept) || policyTypeSelfInspection)
                    {
                        if (quoteConfirmCommand.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
                        {
                            isSelfInspection = true;
                        }
                        if (quoteConfirmCommand.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler) && Convert.ToInt32(confirmQuoteResult.generalInformation.cubicCapacity) > 350)
                        {
                            isSelfInspection = true;
                        }
                        if (quoteConfirmCommand.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                        {
                            isSelfInspection = true;
                        }
                    }
                }

                quoteConfirm = new QuoteConfirmDetailsResponseModel
                {
                    InsurerStatusCode = (int)HttpStatusCode.OK,
                    InsurerId = _iCICIConfig.InsurerId,
                    InsurerName = _iCICIConfig.InsurerName,
                    NewPremium = Math.Round(Convert.ToDecimal(confirmQuoteResult.finalPremium)).ToString(),
                    NCB = Math.Round(Convert.ToDecimal(confirmQuoteResult.riskDetails.ncbPercentage)).ToString(),
                    IDV = Convert.ToInt32(Math.Round(confirmQuoteResult.generalInformation.depriciatedIDV)),
                    Tax = new ServiceTaxModel
                    {
                        totalTax = Math.Round(confirmQuoteResult.totalTax).ToString()
                    },
                    TotalPremium = (string.IsNullOrEmpty(confirmQuoteResult.packagePremium)) ?
                    Math.Round(Convert.ToDecimal(confirmQuoteResult.totalLiabilityPremium)).ToString() :
                    Math.Round(Convert.ToDecimal(confirmQuoteResult.packagePremium)).ToString(),

                    GrossPremium = Math.Round(Convert.ToDecimal(confirmQuoteResult.finalPremium)).ToString(),
                    IsBreakin = confirmQuoteResult.breakingFlag,
                    IsSelfInspection = isSelfInspection
                };
            }
            else
            {
                var stream = await confirmQuote.Item1.Content.ReadAsStreamAsync();
                //confirmQuoteResult = stream.DeserializeFromJson<ICICIResponseDto>();
                if (quoteConfirmCommand.PolicyDates.IsCommercial)
                {
                    confirmQuoteResultCV = stream.DeserializeFromJson<ICICICVResponseDto>();
                    confirmQuoteResult = _mapper.Map<ICICIResponseDto>(confirmQuoteResultCV);
                    responseBody = JsonConvert.SerializeObject(confirmQuoteResultCV);
                }
                else
                {
                    confirmQuoteResult = stream.DeserializeFromJson<ICICIResponseDto>();
                    responseBody = JsonConvert.SerializeObject(confirmQuoteResult);
                }
                if (confirmQuote.Item1.StatusCode == HttpStatusCode.BadRequest)
                {
                    quoteConfirm.ValidationMessage = await confirmQuote.Item1.Content.ReadAsStringAsync();
                }
                else
                {
                    quoteConfirm.ValidationMessage = ValidationMessage;
                }
                //responseBody = JsonConvert.SerializeObject(confirmQuoteResult);
                quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            }
            await UpdateICLogs(confirmQuote.Item2, CorrelationId, responseBody);
            return Tuple.Create(quoteConfirm, updatedResponse, requestBody, responseBody, leadId, transactionId);
        }
        public async Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(ICICICKYCCommand iCICICKYCCommand, CancellationToken cancellationToken)
        {
            string responseBody = string.Empty;
            string requestJson = string.Empty;
            SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();
            CreateLeadModel createLeadModel = new CreateLeadModel();
            createLeadModel.PermanentAddress = new LeadAddressModel();
            var id = 0;
            try
            {
                string token = await GetToken(iCICICKYCCommand.LeadId, false, false, true, false, false, "KYC");
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);
                var publicKey = jwtSecurityToken.Claims.First(claim => claim.Type == "pbk").Value;

                ICICIEncrypt cryptoHelper = new ICICIEncrypt();
                cryptoHelper.ImportPublicKey(publicKey);

                string cKYCDocumentType = "";
                string cKYCDocumentDetails = "";
                var date = Convert.ToDateTime(iCICICKYCCommand.DateOfBirth).ToString("dd-MM-yyyy");

                switch (iCICICKYCCommand.DocumentType)
                {
                    case ("AADHAAR"):
                        cKYCDocumentType = "AADHAAR";
                        cKYCDocumentDetails = "\"aadhaar_details\": {" + "\"aadhaar_number\":\" " + cryptoHelper.Encrypt(iCICICKYCCommand.AadharNumber) + "\"," +
                                              "\"full_name\": \"" + iCICICKYCCommand.FullName + "\"," +
                                              "\"dob\": \"" + cryptoHelper.Encrypt(date) + "\"," +
                                              "\"gender\": \"" + iCICICKYCCommand.Gender + "\"}";
                        break;
                    case ("PAN"):
                        cKYCDocumentType = "PAN";
                        cKYCDocumentDetails = "\"pan_details\": {" + "\"pan\": \"" + cryptoHelper.Encrypt(iCICICKYCCommand.PAN) + "\"," +
                                              "\"dob\": \"" + cryptoHelper.Encrypt(date) + "\"}";
                        break;
                    case ("CKYC"):
                        cKYCDocumentType = "CKYC";
                        cKYCDocumentDetails = "\"ckyc_details\": {" + "\"ckyc_number\": \"" + cryptoHelper.Encrypt(iCICICKYCCommand.CKYCNumber) + "\"," +
                                              "\"dob\": \"" + cryptoHelper.Encrypt(date) + "\"}";
                        break;
                    case ("DL"):
                        cKYCDocumentType = "DL";
                        cKYCDocumentDetails = "\"dl_details\": {" + "\"dl_number\": \"" + cryptoHelper.Encrypt(iCICICKYCCommand.DrivingLicenceNumber) + "\"," +
                                              "\"dob\": \"" + cryptoHelper.Encrypt(date) + "\"}";
                        break;
                    case ("PASPORTNO"):
                        cKYCDocumentType = "PASSPORT";
                        cKYCDocumentDetails = "\"passport_details\": {" + "\"passport_number\": \"" + cryptoHelper.Encrypt(iCICICKYCCommand.PassportNumber) + "\"," +
                                              "\"dob\": \"" + cryptoHelper.Encrypt(date) + "\"}";
                        break;
                    case ("VOTERID"):
                        cKYCDocumentType = "VOTER";
                        cKYCDocumentDetails = "\"epic_details\": {" + "\"epic_number \": \"" + cryptoHelper.Encrypt(iCICICKYCCommand.VoterId) + "\"," +
                                              "\"dob\": \"" + cryptoHelper.Encrypt(date) + "\"}";
                        break;
                    case ("CIN"):
                        cKYCDocumentType = "CIN";
                        cKYCDocumentDetails = "\"cin_details\": {" + "\"cin\": \"" + cryptoHelper.Encrypt(iCICICKYCCommand.CIN) + "\"," +
                                              "\"doi\": \"" + cryptoHelper.Encrypt(date) + "\"}";
                        break;
                }
                string docdetail = cKYCDocumentDetails;
                requestJson = "{  " + "\"correlationId\":\"" + iCICICKYCCommand.TransactionId + "\"," +
                                     "\"certificate_type\":\"" + cKYCDocumentType + "\"," +
                              (iCICICKYCCommand.IsPoliticallyExposedPerson ? "\"pep_flag\":true," : "\"pep_flag\":false,") + cKYCDocumentDetails + "}";

                requestJson = requestJson.Replace("\n", "").Replace(" ", "").Replace("\r", "");

                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                id = await InsertICLogs(requestJson, iCICICKYCCommand.LeadId, _iCICIConfig.CKYCURL, token, string.Empty, "KYC");

                try
                {
                    var result = await _client.PostAsync(_iCICIConfig.CKYCURL, new StringContent(requestJson, Encoding.UTF8, "application/json"), cancellationToken);
                    saveCKYCResponse.IsKYCRequired = false;
                    if (result.IsSuccessStatusCode)
                    {
                        var stream = await result.Content.ReadAsStreamAsync();
                        var response = stream.DeserializeFromJson<ICICICKYCResponseDto>();
                        responseBody = JsonConvert.SerializeObject(response);
                        if (response.statusMessage.Equals(SUCCESS) && response.status)
                        {
                            string dob = DateTime.ParseExact(response.kyc_details.dob, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);

                            createLeadModel.LeadName = response.kyc_details.first_name;
                            createLeadModel.LastName = response.kyc_details.last_name;
                            createLeadModel.DOB = dob;
                            createLeadModel.Gender = response.kyc_details.gender;
                            createLeadModel.PhoneNumber = response.kyc_details.mobile_number;
                            createLeadModel.Email = response.kyc_details.email;
                            createLeadModel.PANNumber = response.kyc_details.certificate_type.Equals("PAN") ? response.kyc_details.certificate_number : null;
                            createLeadModel.AadharNumber = response.kyc_details.certificate_type.Equals("AADHAAR") ? iCICICKYCCommand.AadharNumber : null;
                            createLeadModel.ckycNumber = response.kyc_details.ckyc_number;
                            createLeadModel.CKYCstatus = response.statusMessage;
                            createLeadModel.kyc_id = response.kyc_details.il_kyc_ref_no;
                            createLeadModel.PermanentAddress = new LeadAddressModel()
                            {
                                AddressType = "PRIMARY",
                                Address1 = response.kyc_details.permanent_address.address_line_1,
                                Address2 = response.kyc_details.permanent_address.address_line_2,
                                Address3 = response.kyc_details.permanent_address.address_line_3,
                                Pincode = response.kyc_details.permanent_address.pin_code,
                            };
                            saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                            saveCKYCResponse.Message = KYC_SUCCESS;
                            saveCKYCResponse.Name = (response.kyc_details.first_name + ' ' + response.kyc_details.last_name).Trim();
                            saveCKYCResponse.Gender = response.kyc_details.gender == "M" ? "Male" : "Female";
                            saveCKYCResponse.DOB = dob;
                            saveCKYCResponse.Address = $"{response.kyc_details.permanent_address.address_line_1},{response.kyc_details.permanent_address.city}," +
                                $"{response.kyc_details.permanent_address.district},{response.kyc_details.permanent_address.state}," +
                                $"{response.kyc_details.permanent_address.pin_code}";
                            saveCKYCResponse.InsurerName = _iCICIConfig.InsurerName;
                            saveCKYCResponse.IsKYCRequired = true;
                            await UpdateICLogs(id, iCICICKYCCommand.TransactionId, responseBody);
                            return Tuple.Create(requestJson, responseBody, saveCKYCResponse, createLeadModel);
                        }
                        saveCKYCResponse.KYC_Status = POA_REQUIRED;
                        saveCKYCResponse.Message = POA_REQUIRED;
                    }
                    else
                    {
                        responseBody = await result.Content.ReadAsStringAsync();
                        saveCKYCResponse.KYC_Status = FAILED;
                        saveCKYCResponse.Message = "Please try again.";
                        _logger.LogError("ICICI Ckyc API {responsebody}", responseBody);
                    }
                    await UpdateICLogs(id, iCICICKYCCommand.TransactionId, responseBody);
                    return Tuple.Create(requestJson, responseBody, saveCKYCResponse, createLeadModel);
                }
                catch (Exception ex)
                {
                    saveCKYCResponse.KYC_Status = FAILED;
                    saveCKYCResponse.Message = "Please try again.";
                    saveCKYCResponse.IsKYCRequired = true;
                    _logger.LogError("ICICI Ckyc Error {exception}", ex.Message);
                    await UpdateICLogs(id, iCICICKYCCommand.TransactionId, ex.Message);
                    return Tuple.Create(requestJson, responseBody, saveCKYCResponse, createLeadModel);
                }
            }
            catch (Exception ex)
            {
                saveCKYCResponse.KYC_Status = FAILED;
                saveCKYCResponse.Message = "Please try again.";
                saveCKYCResponse.IsKYCRequired = true;
                _logger.LogError("ICICI Ckyc Error {exception}", ex.Message);
                return Tuple.Create(requestJson, responseBody, saveCKYCResponse, createLeadModel);
            }
        }
        public async Task<Tuple<string, string, UploadCKYCDocumentResponse, CreateLeadModel>> UploadCKYCDocument(UploadICICICKYCDocumentCommand iCICICKYCDocumentCommand, CancellationToken cancellationToken)
        {
            var responseBody = string.Empty;
            string requestBody = JsonConvert.SerializeObject(iCICICKYCDocumentCommand);
            UploadCKYCDocumentResponse uploadCKYCDocument = new UploadCKYCDocumentResponse();
            CreateLeadModel createLeadModel = new CreateLeadModel();
            var id = 0;
            try
            {
                var token = await GetToken(iCICICKYCDocumentCommand.LeadId, false, false, true, false, false, "KYC");
                bool isPOIAndPOASame = false;
                string poiDocumentName = iCICICKYCDocumentCommand.ProofOfIdentity;
                string poaDocumentName = iCICICKYCDocumentCommand.ProofOfAddress;

                if (iCICICKYCDocumentCommand.ProofOfIdentity.Equals(iCICICKYCDocumentCommand.ProofOfAddress))
                {
                    isPOIAndPOASame = true;
                }

                byte[] poiDoc = Convert.FromBase64String(iCICICKYCDocumentCommand.POIDocumentUpload);
                byte[] poaDoc = Convert.FromBase64String(iCICICKYCDocumentCommand.POADocumentUpload);

                var client = new RestClient(_iCICIConfig.POADocumentUpload);
                var request = new RestRequest(string.Empty, Method.Post);
                request.AlwaysMultipartFormData = true;
                request.AddHeader("Authorization", "Bearer " + token + "");
                request.AddParameter("mobile_number", "91" + iCICICKYCDocumentCommand.Mobile);
                request.AddParameter("email", iCICICKYCDocumentCommand.EmailId);
                request.AddParameter("is_poa_poi_same", isPOIAndPOASame);
                request.AddParameter("poi[0].certificate_type", iCICICKYCDocumentCommand.ProofOfIdentity);
                request.AddFile("poi[0].document", poiDoc, poiDocumentName + "." + iCICICKYCDocumentCommand.poiDocumentUploadExtension);
                request.AddParameter("poa[0].certificate_type", iCICICKYCDocumentCommand.ProofOfAddress);
                request.AddFile("poa[0].document", poaDoc, poaDocumentName + "." + iCICICKYCDocumentCommand.poaDocumentUploadExtension);
                request.AddParameter("CorrelationId", iCICICKYCDocumentCommand.TransactionId);

                id = await InsertICLogs(JsonConvert.SerializeObject(request.Parameters), iCICICKYCDocumentCommand.LeadId, _iCICIConfig.CKYCURL, token, string.Empty, "KYC");
                try
                {
                    var response = await client.ExecuteAsync(request, cancellationToken);
                    string data = response.Content;
                    var result = JsonConvert.DeserializeObject<ICICICKYCResponseDto>(data);
                    responseBody = JsonConvert.SerializeObject(result);

                    if (response.IsSuccessStatusCode)
                    {
                        if (result.status && result.statusMessage.Equals("Success"))
                        {
                            uploadCKYCDocument.CKYCStatus = POA_SUCCESS;
                            uploadCKYCDocument.Message = POA_SUCCESS;
                            createLeadModel.kyc_id = result.kyc_details.il_kyc_ref_no;
                            await UpdateICLogs(id, iCICICKYCDocumentCommand.TransactionId, responseBody);
                            return Tuple.Create(requestBody, data, uploadCKYCDocument, createLeadModel);
                        }
                        else if (result.status && result.statusMessage.Equals("ACCEPTED_FOR_MANUAL_QC"))
                        {
                            uploadCKYCDocument.CKYCStatus = POA_SUCCESS;
                            uploadCKYCDocument.Message = POA_SUCCESS;
                            await UpdateICLogs(id, iCICICKYCDocumentCommand.TransactionId, responseBody);
                            return Tuple.Create(requestBody, data, uploadCKYCDocument, createLeadModel);
                        }
                    }
                    else
                    {
                        uploadCKYCDocument.CKYCStatus = FAILED;
                        uploadCKYCDocument.Message = MESSAGE;
                        await UpdateICLogs(id, iCICICKYCDocumentCommand.TransactionId, responseBody);
                        return Tuple.Create(requestBody, data, uploadCKYCDocument, createLeadModel);
                    }
                    return default;
                }
                catch (Exception ex)
                {
                    uploadCKYCDocument.CKYCStatus = FAILED;
                    uploadCKYCDocument.Message = MESSAGE;
                    _logger.LogError("ICICI CKYC Error {exception}", ex.Message);
                    await UpdateICLogs(id, iCICICKYCDocumentCommand.TransactionId, ex.Message);
                    return Tuple.Create(iCICICKYCDocumentCommand.ToString(), responseBody, uploadCKYCDocument, createLeadModel);
                }
            }
            catch (Exception ex)
            {
                uploadCKYCDocument.CKYCStatus = FAILED;
                uploadCKYCDocument.Message = MESSAGE;
                _logger.LogError("ICICI CKYC Error {exception}", ex.Message);
                return Tuple.Create(iCICICKYCDocumentCommand.ToString(), responseBody, uploadCKYCDocument, createLeadModel);
            }
        }
        public async Task<Tuple<QuoteResponseModel, string, string>> CreateProposal(ICICIProposalRequestDto proposalQuery, ICICIProposalRequest proposalRequest, ICICICKYCRequest iCICICKYCRequest, string vehicleTypeId, CreateLeadModel createLeadModel, CancellationToken cancellationToken)
        {
            var proposalVm = new QuoteResponseModel();
            string requestBody = string.Empty;
            var responseBody = string.Empty;
            var id = 0;
            string url = string.Empty;
            bool breakin = false;

            try
            {
                proposalQuery.CustomerDetails = new Domain.ICICI.CustomerDetails()
                {
                    CustomerType = proposalQuery.CustomerType,
                    CustomerName = proposalRequest.PersonalDetails.customerName,
                    DateOfBirth = proposalRequest.PersonalDetails.dateOfBirth,
                    PANCardNo = proposalRequest.PersonalDetails.panNumber,
                    Email = proposalRequest.PersonalDetails.emailId,
                    MobileNumber = proposalRequest.PersonalDetails.mobile,
                    PinCode = proposalRequest.AddressDetails.pincode,
                    AddressLine1 = proposalRequest.AddressDetails.addressLine1,
                    CountryCode = 100,
                    StateCode = Convert.ToInt32(proposalRequest.AddressDetails.state),
                    CityCode = Convert.ToInt32(proposalRequest.AddressDetails.city),
                    Gender = proposalRequest.PersonalDetails.gender,
                    MobileISD = "91",
                    AadharNumber = proposalRequest.PersonalDetails.aadharNumbrer,
                    CKYCId = null,
                    EKYCid = null,
                    PEPFlag = iCICICKYCRequest.pep_flag,
                    ILKYCReferenceNumber = createLeadModel.kyc_id
                };
                proposalQuery.NomineeDetails = new Domain.ICICI.NomineeDetails()
                {
                    NameOfNominee = proposalRequest.NomineeDetails.nomineeName,
                    Age = Convert.ToInt32(proposalRequest.NomineeDetails.nomineeAge),
                    Relationship = proposalRequest.NomineeDetails.nomineeRelation
                };


                if (proposalRequest.VehicleDetails.isFinancier.Equals("Yes"))
                {
                    proposalQuery.FinancierDetails = new Domain.ICICI.FinancierDetails()
                    {
                        FinancierName = proposalRequest.VehicleDetails.financer,
                        BranchName = proposalRequest.VehicleDetails.branch,
                        AgreementType = "Hypothecation"
                    };
                }
                proposalQuery.EngineNumber = proposalRequest.VehicleDetails.engineNumber;
                proposalQuery.ChassisNumber = proposalRequest.VehicleDetails.chassisNumber;
                proposalQuery.RegistrationNumber = (proposalQuery.BusinessType.Equals("New Business")) ? "NEW" : proposalQuery.RegistrationNumber;

                proposalVm.InsurerName = "ICICI";
                requestBody = JsonConvert.SerializeObject(proposalQuery);
                string token = await GetToken(createLeadModel.LeadID, true, false, false, false, false, "Proposal");
                string productCode = string.Empty;
                if (vehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
                {
                    if (createLeadModel.PolicyTypeId.Equals(_policyTypeConfig.SATP))
                    {
                        productCode = _iCICIConfig.TwoWheelerTPProdectCode;
                    }
                    else
                    {
                        productCode = _iCICIConfig.TwoWheelerProdectCode;
                    }
                }
                else
                {
                    if (createLeadModel.PolicyTypeId.Equals(_policyTypeConfig.SATP))
                    {
                        productCode = _iCICIConfig.FourWheelerTPProdectCode;
                    }
                    else
                    {
                        productCode = _iCICIConfig.FourWheelerProdectCode;
                    }
                }
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                if (_applicationClaims.GetRole().Equals("POSP"))
                {
                    _client.DefaultRequestHeaders.Add("IRDALicenceNumber", _iCICIConfig.LicenseNo);
                    _client.DefaultRequestHeaders.Add("CertificateNumber", _applicationClaims.GetPOSPId());
                    _client.DefaultRequestHeaders.Add("PanCardNo", _applicationClaims.GetPAN());
                    _client.DefaultRequestHeaders.Add("AadhaarNo", _applicationClaims.GetAadhaarNumber());
                    _client.DefaultRequestHeaders.Add("ProductCode", productCode);
                }

                HttpResponseMessage quoteResponse = new HttpResponseMessage();
                if (vehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
                {
                    if (createLeadModel.PolicyTypeId.Equals(_policyTypeConfig.SATP))
                    {
                        url = _iCICIConfig.ProposalTPURL2W;
                    }
                    else
                    {
                        url = _iCICIConfig.ProposalURL2W;
                    }
                }
                else if (vehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
                {
                    if (createLeadModel.PolicyTypeId.Equals(_policyTypeConfig.SATP))
                    {
                        url = _iCICIConfig.ProposalTPURL4W;
                    }
                    else
                    {
                        url = _iCICIConfig.ProposalURL4W;
                    }
                }
                else if (vehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                {
                    url = _iCICIConfig.ProposalURLCV;
                }
                id = await InsertICLogs(requestBody, createLeadModel.LeadID, _iCICIConfig.BaseURL + url, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "Proposal");

                try
                {
                    quoteResponse = await _client.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                                        cancellationToken);

                    string proposalQueryData = JsonConvert.SerializeObject(proposalQuery);
                    QuoteQueryModel quoteQuery = JsonConvert.DeserializeObject<QuoteQueryModel>(proposalQueryData);

                    if (!quoteResponse.IsSuccessStatusCode)
                    {
                        responseBody = await quoteResponse.Content.ReadAsStringAsync(cancellationToken); //quoteResponse.ReasonPhrase;
                        proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                        _logger.LogError("CreateProposal Unable to fetch quote {responseBody}", responseBody);
                    }
                    else
                    {
                        var stream = await quoteResponse.Content.ReadAsStreamAsync();
                        var result = stream.DeserializeFromJson<ICICIResponseDto>();
                        responseBody = JsonConvert.SerializeObject(result);
                        _logger.LogError("CreateProposal {responseBody}", responseBody);
                        if (result != null && result.status.Equals("Success"))
                        {
                            string totalTax = result.totalTax.ToString();
                            decimal? ncbPercentage = Convert.ToInt32(result.riskDetails?.ncbPercentage);
                            var tax = new ServiceTax
                            {
                                totalTax = totalTax
                            };

                            breakin = result.breakingFlag || result.isQuoteDeviation || result.isApprovalRequired ? true : false;
                            proposalVm = new QuoteResponseModel
                            {
                                InsurerName = _iCICIConfig.InsurerName,
                                InsurerStatusCode = (int)HttpStatusCode.OK,
                                IDV = quoteQuery.RecommendedIDV,
                                MinIDV = quoteQuery.MinIDV,
                                MaxIDV = quoteQuery.MaxIDV,
                                Tax = tax,
                                NCB = ncbPercentage.ToString(),
                                TotalPremium = createLeadModel.PolicyTypeId.Equals(_policyTypeConfig.SATP) ? result.totalLiabilityPremium : result.packagePremium,
                                GrossPremium = result.finalPremium,
                                RTOCode = quoteQuery.RTOLocationCode,
                                ProposalNumber = result.generalInformation.proposalNumber.ToString(),
                                IsBreakIn = breakin,
                                isApprovalRequired = result.isApprovalRequired,
                                isQuoteDeviation = result.isQuoteDeviation,
                                ApplicationId = result.correlationId
                            };
                            if (vehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                            {
                                var cvResult = stream.DeserializeFromJson<ICICICVProposalResponseDto>();
                                responseBody = JsonConvert.SerializeObject(result);
                                proposalVm.TotalPremium = Convert.ToString(cvResult.premiumDetails?.packagePremium);
                                proposalVm.GrossPremium = Convert.ToString(cvResult.premiumDetails?.finalPremium);
                            }

                        }
                        else
                        {
                            responseBody = await quoteResponse.Content.ReadAsStringAsync(cancellationToken); //quoteResponse.ReasonPhrase;
                            proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                            proposalVm.ValidationMessage = result.message;
                            _logger.LogError("CreateProposal Unable to fetch quote {responseBody}", responseBody);
                        }
                    }
                    await UpdateICLogs(id, proposalQuery?.CorrelationId, responseBody);
                    return Tuple.Create(proposalVm, requestBody, responseBody);

                }
                catch (Exception ex)
                {
                    _logger.LogError("ICICI Proposal Error {exception}", ex.Message);
                    proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    await UpdateICLogs(id, proposalQuery?.CorrelationId, responseBody);
                    return Tuple.Create(proposalVm, requestBody, responseBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI Proposal Error {exception}", ex.Message);
                proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                return Tuple.Create(proposalVm, requestBody, responseBody);
            }
        }
        public async Task<Tuple<string, string>> CreatePaymentURL(string leadId, string proposalRequest, string proposalResponse, CancellationToken cancellation)
        {
            string PaymentURL = string.Empty;
            var id = 0;
            var responseBody = string.Empty;
            string correlationId = string.Empty;
            try
            {
                var proposalResponseBody = JsonConvert.DeserializeObject<ICICIResponseDto>(proposalResponse);
                var proposalRequestBody = JsonConvert.DeserializeObject<ICICIProposalRequestDto>(proposalRequest);
                correlationId = proposalRequestBody.CorrelationId;
                string paymentCorrelationId = Guid.NewGuid().ToString();
                ICICIPaymentRequestDto paymentRequestDto = new ICICIPaymentRequestDto()
                {
                    TransactionId = paymentCorrelationId,
                    Amount = proposalResponseBody.finalPremium,
                    ApplicationId = _iCICIConfig.ApplicationId,
                    ReturnUrl = $"{_iCICIConfig.ReturnURL}/{correlationId}/{_applicationClaims.GetUserId()}?",
                    AdditionalInfo1 = proposalResponseBody.generalInformation.proposalNumber,
                    AdditionalInfo2 = proposalResponseBody.generalInformation.customerId,
                    AdditionalInfo3 = proposalRequestBody.RegistrationNumber,
                    AdditionalInfo4 = proposalRequestBody.CustomerDetails.MobileNumber,
                };

                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(_iCICIConfig.AuthCode));
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                var requestBody = JsonConvert.SerializeObject(paymentRequestDto);
                _logger.LogInformation("ICICI Payment Link {paymentRequest}", requestBody);
                id = await InsertICLogs(requestBody, leadId, _iCICIConfig.CreateBaseTransaction, base64EncodedAuthenticationString, string.Empty, "Payment");
                try
                {
                    var paymentResponse = await _client.PostAsync(_iCICIConfig.CreateBaseTransaction, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellation);

                    if (!paymentResponse.IsSuccessStatusCode)
                    {
                        var stream = await paymentResponse.Content.ReadAsStreamAsync(cancellation);
                        responseBody = stream.DeserializeFromJson<string>();
                        PaymentURL = string.Empty;
                        await UpdateICLogs(id, proposalRequestBody?.CorrelationId, responseBody);
                        _logger.LogError("Unable to fetch Payment URL {paymentResponse}", paymentResponse);
                    }
                    else
                    {
                        var stream = await paymentResponse.Content.ReadAsStreamAsync(cancellation);
                        var result = stream.DeserializeFromJson<string>();
                        responseBody = result;
                        await UpdateICLogs(id, proposalRequestBody?.CorrelationId, responseBody);
                        if (result != null)
                        {
                            PaymentURL = _iCICIConfig.PaymentURL + result.ToString();
                            _logger.LogInformation("ICICI Payment Link {paymentResponse}", PaymentURL);

                            return Tuple.Create(PaymentURL, paymentCorrelationId);
                        }
                    }
                    return Tuple.Create(PaymentURL, paymentCorrelationId);
                }
                catch (Exception ex)
                {
                    _logger.LogError("ICICI Error {exception}", ex.Message);
                    await UpdateICLogs(id, correlationId, ex.Message);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI Error {exception}", ex.Message);
                return default;
            }
        }
        public async Task<ICICIPaymentEnquiryResponse> TransactionEnquiry(string transactionId, string leadId, CancellationToken cancellation)
        {
            var id = 0;
            var responseBody = string.Empty;
            try
            {
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(_iCICIConfig.AuthCode));
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                string enquiryURL = _iCICIConfig.TransactionEnquiryURL + transactionId;
                _logger.LogInformation("ICICI Transaction Enquiry {request}", enquiryURL);
                id = await InsertICLogs(string.Empty, leadId, enquiryURL, base64EncodedAuthenticationString, string.Empty, "Payment");
                try
                {
                    var response = await _client.GetAsync(enquiryURL, cancellation);
                    if (response.IsSuccessStatusCode)
                    {
                        var stream = await response.Content.ReadAsStreamAsync(cancellation);
                        var result = stream.DeserializeFromJson<ICICIPaymentEnquiryResponse>();
                        _logger.LogInformation("ICICI Transaction Enquiry {response}", JsonConvert.SerializeObject(result));
                        responseBody = JsonConvert.SerializeObject(result);
                        await UpdateICLogs(id, transactionId, responseBody);
                        return result;
                    }
                    else
                    {
                        responseBody = await response.Content.ReadAsStringAsync(cancellation);
                        _logger.LogInformation("ICICI Transaction Enquiry Error {response}", responseBody);
                        await UpdateICLogs(id, transactionId, responseBody);
                        return default;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("ICICI Transaction Enquiry Exception {exception}", ex.Message);
                    await UpdateICLogs(id, transactionId, ex.Message);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI Transaction Enquiry Exception {exception}", ex.Message);
                return default;
            }
        }
        public async Task<ICICIPaymentTaggingResponse> PaymentTagging(ICICIPaymentEnquiryResponse paymentEnquiryResponse, ICICIResponseDto iCICIResponseDto, string dealId, QuoteTransactionRequest quoteTransactionRequest, ICICIPOSPDetails iCICIPOSPDetails, string ProductCode = "")
        {
            var token = await GetToken(quoteTransactionRequest.LeadId, false, false, false, true, false, "Payment");
            string productCode = string.Empty;
            _logger.LogInformation("PaymentTagging iCICIResponseDto {iCICIResponseDto}, dealId {dealId}, QuoteTransactionDetails {vehicleTypeId}, iCICIPOSPDetails {iCICIPOSPDetails}", JsonConvert.SerializeObject(iCICIResponseDto), dealId, JsonConvert.SerializeObject(quoteTransactionRequest), JsonConvert.SerializeObject(iCICIPOSPDetails));
            var paymentTaggingResponse = string.Empty;
            var id = 0;
            var responseBody = string.Empty;
            try
            {
                if (quoteTransactionRequest.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
                {
                    if (quoteTransactionRequest.PolicyTypeId.Equals(_policyTypeConfig.SATP))
                    {
                        productCode = _iCICIConfig.TwoWheelerTPProdectCode;
                    }
                    else
                    {
                        productCode = _iCICIConfig.TwoWheelerProdectCode;
                    }
                }
                else if (quoteTransactionRequest.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                {
                    productCode = ProductCode;
                }
                else
                {
                    if (quoteTransactionRequest.PolicyTypeId.Equals(_policyTypeConfig.SATP))
                    {
                        productCode = _iCICIConfig.FourWheelerTPProdectCode;
                    }
                    else
                    {
                        productCode = _iCICIConfig.FourWheelerProdectCode;
                    }
                }
                CustomerProposal proposalInfo = new CustomerProposal
                {
                    CustomerID = iCICIResponseDto?.generalInformation?.customerId?.ToString(),
                    ProposalNo = iCICIResponseDto?.generalInformation?.proposalNumber?.ToString(),
                };
                ICICIPaymentTaggingRequestDto iCICIPaymentTaggingRequestDto = new ICICIPaymentTaggingRequestDto()
                {
                    CorrelationId = iCICIResponseDto.correlationId,
                    dealid = dealId,
                    isMappingRequired = true,
                    isTaggingRequired = true,
                    PaymentEntry = new PaymentEntry
                    {
                        onlineDAEntry = new OnlineDAEntry
                        {
                            AuthCode = paymentEnquiryResponse.AuthCode,
                            MerchantID = paymentEnquiryResponse.MerchantId,
                            TransactionId = paymentEnquiryResponse.PGtransactionId,
                            PaymentAmount = paymentEnquiryResponse.Amount,
                            InstrumentDate = iCICIResponseDto?.generalInformation?.proposalDate,
                            CustomerID = iCICIResponseDto?.generalInformation?.customerId?.ToString(),
                        }
                    },
                    PaymentMapping = new PaymentMapping
                    {
                        customerProposal = new CustomerProposal[]
                        {
                            proposalInfo
                        }
                    },
                    PaymentTagging = new PaymentTagging
                    {
                        customerProposal = new CustomerProposal[]
                        {
                            proposalInfo
                        }
                    }
                };
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                if (iCICIPOSPDetails.RoleName.Equals("POSP"))
                {
                    _client.DefaultRequestHeaders.Add("IRDALicenceNumber", _iCICIConfig.LicenseNo);
                    _client.DefaultRequestHeaders.Add("CertificateNumber", iCICIPOSPDetails.POSPId);
                    _client.DefaultRequestHeaders.Add("PanCardNo", iCICIPOSPDetails.PAN);
                    _client.DefaultRequestHeaders.Add("AadhaarNo", iCICIPOSPDetails.AadhaarNumber);
                    _client.DefaultRequestHeaders.Add("ProductCode", productCode);
                }

                var requestBody = JsonConvert.SerializeObject(iCICIPaymentTaggingRequestDto);
                _logger.LogInformation("ICICI PaymentTagging request {request}", requestBody);
                id = await InsertICLogs(requestBody, quoteTransactionRequest.LeadId, _iCICIConfig.PaymentTaggingURL, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "Payment");
                try
                {
                    var paymentResponse = await _client.PostAsync(_iCICIConfig.PaymentTaggingURL, new StringContent(requestBody, Encoding.UTF8, "application/json"));

                    if (!paymentResponse.IsSuccessStatusCode)
                    {
                        responseBody = await paymentResponse.Content.ReadAsStringAsync();
                        _logger.LogError("PaymentTagging paymentResponse error {paymentResponse}", responseBody);
                    }
                    else
                    {
                        var stream = await paymentResponse.Content.ReadAsStreamAsync();
                        var result = stream.DeserializeFromJson<ICICIPaymentTaggingResponse>();
                        responseBody = JsonConvert.SerializeObject(result);
                        await UpdateICLogs(id, iCICIResponseDto?.correlationId, responseBody);
                        _logger.LogInformation("ICICI PaymentTagging response {response}", responseBody);
                        return result;
                    }
                    await UpdateICLogs(id, iCICIResponseDto?.correlationId, responseBody);
                    return default;
                }
                catch (Exception ex)
                {
                    _logger.LogError("ICICI Payment Tagging Error {exception}", ex.Message);
                    await UpdateICLogs(id, iCICIResponseDto?.correlationId, responseBody);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI Payment Tagging Error {exception}", ex.Message);
                return default;
            }
        }
        public async Task<byte[]> GeneratePolicy(ICICIPaymentTaggingResponseDto iCICIPaymentTaggingResponseDto, ICICIPOSPDetails iCICIPOSPDetails)
        {
            string token = await GetToken(iCICIPaymentTaggingResponseDto.LeadId, false, false, false, false, true, "Payment");
            var requestBody = string.Empty;
            string productCode = string.Empty;
            var id = 0;
            var responseBody = string.Empty;
            try
            {
                if (iCICIPaymentTaggingResponseDto.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
                {
                    if (iCICIPaymentTaggingResponseDto.PolicyTypeId.Equals(_policyTypeConfig.SATP))
                    {
                        productCode = _iCICIConfig.TwoWheelerTPProdectCode;
                    }
                    else
                    {
                        productCode = _iCICIConfig.TwoWheelerProdectCode;
                    }
                }
                else if (iCICIPaymentTaggingResponseDto.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                {
                    productCode = iCICIPaymentTaggingResponseDto.ProductCode;
                }
                else
                {
                    if (iCICIPaymentTaggingResponseDto.PolicyTypeId.Equals(_policyTypeConfig.SATP))
                    {
                        productCode = _iCICIConfig.FourWheelerTPProdectCode;
                    }
                    else
                    {
                        productCode = _iCICIConfig.FourWheelerProdectCode;
                    }
                }
                ICICIPolicyGenerationRequest iCICIPolicyGenerationRequest = new ICICIPolicyGenerationRequest()
                {
                    CorrelationId = iCICIPaymentTaggingResponseDto.iCICIPaymentTaggingResponse.correlationId,
                    policyNo = iCICIPaymentTaggingResponseDto.PolicyNumber,
                    customerId = iCICIPaymentTaggingResponseDto.CustomerId,
                    dealId = iCICIPaymentTaggingResponseDto.DealId
                };
                requestBody = JsonConvert.SerializeObject(iCICIPolicyGenerationRequest);
                _logger.LogInformation("ICICI Policy Generate {request}", requestBody);
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                if (iCICIPOSPDetails.RoleName.Equals("POSP"))
                {
                    _client.DefaultRequestHeaders.Add("IRDALicenceNumber", _iCICIConfig.LicenseNo);
                    _client.DefaultRequestHeaders.Add("CertificateNumber", iCICIPOSPDetails.POSPId);
                    _client.DefaultRequestHeaders.Add("PanCardNo", iCICIPOSPDetails.PAN);
                    _client.DefaultRequestHeaders.Add("AadhaarNo", iCICIPOSPDetails.AadhaarNumber);
                    _client.DefaultRequestHeaders.Add("ProductCode", productCode);
                }

                id = await InsertICLogs(requestBody, iCICIPaymentTaggingResponseDto.LeadId, _iCICIConfig.PolicyGenerationURL, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "Payment");
                try
                {
                    var response = await _client.PostAsync(_iCICIConfig.PolicyGenerationURL, new StringContent(requestBody, Encoding.UTF8, "application/json"));
                    _logger.LogInformation("ICICI Policy Download request {request}", requestBody);

                    if (!response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        await UpdateICLogs(id, iCICIPaymentTaggingResponseDto?.iCICIPaymentTaggingResponse.correlationId, responseBody);
                        _logger.LogError("Unable to Generate Policy response {responseBody}", response);
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var memoryStream = new MemoryStream();
                        stream.CopyTo(memoryStream);
                        byte[] byteArray = memoryStream.ToArray();
                        _logger.LogInformation("ICICI Policy Download {response}", byteArray);
                        //System.IO.File.WriteAllBytes(iCICIPolicyGenerationRequest.CorrelationId + ".pdf", byteArray);
                        responseBody = Convert.ToBase64String(byteArray);
                        await UpdateICLogs(id, iCICIPaymentTaggingResponseDto?.iCICIPaymentTaggingResponse.correlationId, responseBody);
                        return byteArray;
                    }
                    return default;
                }
                catch (Exception ex)
                {
                    await UpdateICLogs(id, iCICIPaymentTaggingResponseDto?.iCICIPaymentTaggingResponse.correlationId, ex.Message);
                    _logger.LogError("ICICI Policy Generation Error {exception}", ex.Message);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI Policy Generation Error {exception}", ex.Message);
                return default;
            }
        }
        public async Task<ICICISubmitPOSPCertificateResponse> SubmitPOSPCertificate(ICICICreateIMBrokerModel iCICICreateIMBrokerModel, CancellationToken cancellationToken)
        {
            var requestBody = string.Empty;
            string token = await GetToken(iCICICreateIMBrokerModel.POSPId, false, false, false, false, false, string.Empty); //POSP Onboarding,Hence passing POSPID LeadID not applicable
            string date = DateTime.Now.ToString("dd-MM-yyyy");
            string endDate = DateTime.Now.AddYears(15).AddDays(-1).ToString("dd-MM-yyyy");
            var id = 0;
            var responseBody = string.Empty;
            string correlationId = string.Empty;
            try
            {
                var request = new ICICISubmitPOSPCertificateRequest
                {
                    IRDALicNo = _iCICIConfig.LicenseNo,
                    CertificateNo = iCICICreateIMBrokerModel.POSPId,
                    StartDate = date,
                    EndDate = endDate,
                    PanNumber = iCICICreateIMBrokerModel.PAN,
                    CertificateUserName = iCICICreateIMBrokerModel.UserName,
                    CertificateStatus = _iCICIConfig.CertificateStatus,
                    Gender = iCICICreateIMBrokerModel.Gender,
                    AadhaarNo = iCICICreateIMBrokerModel.AadhaarNumber,
                    CorrelationId = Guid.NewGuid().ToString()
                };
                correlationId = request.CorrelationId;
                requestBody = JsonConvert.SerializeObject(request);
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                id = await InsertICLogs(requestBody, iCICICreateIMBrokerModel.POSPId, _iCICIConfig.SubmitPOSPCertificateURL, token, string.Empty, string.Empty);
                try
                {
                    var response = await _client.PostAsync(_iCICIConfig.SubmitPOSPCertificateURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                                cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        await UpdateICLogs(id, request.CorrelationId, responseBody);
                        _logger.LogError("Issue with  submit POSP certificate {response}", response);
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                        var result = stream.DeserializeFromJson<ICICISubmitPOSPCertificateResponse>();
                        responseBody = JsonConvert.SerializeObject(result);
                        _logger.LogInformation("Submit POSP Certificate {response}", JsonConvert.SerializeObject(result));
                        await UpdateICLogs(id, request.CorrelationId, responseBody);
                        return result;
                    }
                    return default;
                }
                catch (Exception ex)
                {
                    await UpdateICLogs(id, correlationId, ex.Message);
                    _logger.LogError("ICICI SUbmit POSP Certificate Error {exception}", ex.Message);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI SUbmit POSP Certificate Error {exception}", ex.Message);
                return default;
            }
        }
        public async Task<ICICICreateIMBrokerResponse> CreateBroker(ICICICreateIMBrokerModel iCICICreateIMBrokerModel, CancellationToken cancellationToken)
        {
            var requestBody = string.Empty;
            string token = await GetToken(iCICICreateIMBrokerModel.POSPId, false, false, false, false, false, string.Empty);
            var id = 0;
            var responseBody = string.Empty;
            var firstName = string.Empty;
            var lastName = string.Empty;
            var userNameArray = iCICICreateIMBrokerModel.UserName.Split(" ");
            var dob = DateTime.Parse(iCICICreateIMBrokerModel.DateofBirth, new System.Globalization.CultureInfo("pt-BR")).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (userNameArray.Length > 2)
            {
                for (int i = 0; i < userNameArray.Length - 1; i++)
                {
                    firstName += " " + userNameArray[i];
                }
            }
            else
            {
                firstName = userNameArray[0];
            }
            lastName = userNameArray[^1];
            try
            {
                var request = new ICICICreateIMBrokerRequest
                {
                    PanNumber = iCICICreateIMBrokerModel.PAN,
                    LicenseNo = _iCICIConfig.LicenseNo,
                    IlLocation = _iCICIConfig.IlLocation,
                    CertificateNo = iCICICreateIMBrokerModel.POSPId,
                    FirstName = firstName,
                    MiddleName = string.Empty,
                    LastName = string.IsNullOrEmpty(lastName) ? "." : lastName,
                    FatherHusbandName = "",
                    DateOfBirth = dob,
                    Gender = iCICICreateIMBrokerModel.Gender,
                    Mobile = iCICICreateIMBrokerModel.MobileNo,
                    EmailId = iCICICreateIMBrokerModel.EmailId,
                    ContactPersonMobile = iCICICreateIMBrokerModel.MobileNo,
                    ContactPersonEmailId = iCICICreateIMBrokerModel.EmailId,
                    Address = iCICICreateIMBrokerModel.AddressLine1 + "," + iCICICreateIMBrokerModel.AddressLine2,
                    State = iCICICreateIMBrokerModel.StateName,
                    City = iCICICreateIMBrokerModel.CityName,
                    Country = "India",
                    PostalCode = iCICICreateIMBrokerModel.Pincode,
                    CorrelationId = iCICICreateIMBrokerModel.CorrelationId
                };
                requestBody = JsonConvert.SerializeObject(request);
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                id = await InsertICLogs(requestBody, iCICICreateIMBrokerModel.POSPId, _iCICIConfig.CreateIMBrokerURL, token, string.Empty, string.Empty);

                try
                {
                    var response = await _client.PostAsync(_iCICIConfig.CreateIMBrokerURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                                cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        await UpdateICLogs(id, iCICICreateIMBrokerModel?.CorrelationId, responseBody);
                        _logger.LogError("Create IM Broker {response}", response);
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var result = stream.DeserializeFromJson<ICICICreateIMBrokerResponse[]>();
                        responseBody = JsonConvert.SerializeObject(result);
                        await UpdateICLogs(id, iCICICreateIMBrokerModel?.CorrelationId, responseBody);
                        _logger.LogInformation("Create IM Broker {response}", JsonConvert.SerializeObject(result));
                        if (result.FirstOrDefault().status.ToLower().Equals("success"))
                        {
                            return result.FirstOrDefault();
                        }
                        else
                        {
                            return default;
                        }
                    }
                    return default;
                }
                catch (Exception ex)
                {
                    _logger.LogError("ICICI Create IM Broker Error {exception}", ex.Message);
                    await UpdateICLogs(id, iCICICreateIMBrokerModel?.CorrelationId, ex.Message);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI Create IM Broker Error {exception}", ex.Message);
                return default;
            }
        }
        public async Task<Tuple<string, string,string>> CreateBreakinId(ICICIProposalRequestDto iCICIProposalRequestDto, ICICIResponseDto iCICIResponseDto, string city, string state, string vehicleTypeId, string categoryId, CancellationToken cancellationToken)
        {
            var requestBody = string.Empty;
            var responseBody = string.Empty;
            string token = await GetToken(iCICIProposalRequestDto.LeadId, true, false, false, false, false, "BreakIn");
            bool isPolicyExpired = DateTime.Now > Convert.ToDateTime(iCICIProposalRequestDto.PreviousPolicyDetails?.previousPolicyEndDate) ? true : false;
            var breakinDays = isPolicyExpired ? (DateTime.Now - Convert.ToDateTime(iCICIProposalRequestDto.PreviousPolicyDetails?.previousPolicyEndDate)).Days : 0;
            var id = 0;
            try
            {
                string typeVehicle = string.Empty;
                if (vehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
                {
                    typeVehicle = _iCICIConfig.VehicleTypeTwoWheeler;
                }
                else if (vehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
                {
                    typeVehicle = _iCICIConfig.VehicleTypeFourWheeler;
                }
                else if (vehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                {
                    typeVehicle = iCICIProposalRequestDto.CategoryId == "1" ? _iCICIConfig.GCVVehicleType : iCICIProposalRequestDto.CategoryId == "2" ? _iCICIConfig.PCVVehicleType : iCICIProposalRequestDto.CategoryId == "3" ? _iCICIConfig.MISCVehicleType : "";
                }
                var request = new ICICICreateBreakinIdRequest()
                {
                    CorrelationId = iCICIResponseDto.correlationId,
                    BreakInType = _iCICIConfig.BreakinType,
                    BreakInDays = breakinDays,
                    CustomerName = iCICIProposalRequestDto.CustomerDetails.CustomerName,
                    CustomerAddress = iCICIProposalRequestDto.CustomerDetails.AddressLine1,
                    State = state,
                    City = city,
                    MobileNumber = Convert.ToInt64(iCICIProposalRequestDto.CustomerDetails.MobileNumber),
                    TypeVehicle = typeVehicle,
                    VehicleMake = iCICIResponseDto.generalInformation.manufacturerName,
                    VehicleModel = iCICIResponseDto.generalInformation.vehicleModel,
                    ManufactureYear = iCICIResponseDto.generalInformation.manufacturingYear,
                    RegistrationNo = iCICIProposalRequestDto.RegistrationNumber,
                    EngineNo = iCICIProposalRequestDto.EngineNumber,
                    ChassisNo = iCICIProposalRequestDto.ChassisNumber,
                    SubLocation = city,
                    DistributorInterID = _iCICIConfig.DistributorInterId,
                    DistributorName = _iCICIConfig.DistributorName,
                    InspectionType = _iCICIConfig.InspectionType,
                    DealId = iCICIProposalRequestDto.DealId
                };
                requestBody = JsonConvert.SerializeObject(request);
                string productCode = vehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler) ? _iCICIConfig.TwoWheelerProdectCode : _iCICIConfig.FourWheelerProdectCode;

                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                if (_applicationClaims.GetRole().Equals("POSP"))
                {
                    _client.DefaultRequestHeaders.Add("IRDALicenceNumber", _iCICIConfig.LicenseNo);
                    _client.DefaultRequestHeaders.Add("CertificateNumber", _applicationClaims.GetPOSPId());
                    _client.DefaultRequestHeaders.Add("PanCardNo", _applicationClaims.GetPAN());
                    _client.DefaultRequestHeaders.Add("AadhaarNo", _applicationClaims.GetAadhaarNumber());
                    _client.DefaultRequestHeaders.Add("ProductCode", productCode);
                }
                id = await InsertICLogs(requestBody, iCICIProposalRequestDto.LeadId, _iCICIConfig.CreateBreakinId, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "BreakIn");
                try
                {
                    var response = await _client.PostAsync(_iCICIConfig.CreateBreakinId, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                                cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        await UpdateICLogs(id, iCICIResponseDto?.correlationId, responseBody);
                        _logger.LogError("Create Breakin Id response {response}", response);
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var result = stream.DeserializeFromJson<ICICICreateBreakinIdResponse>();
                        responseBody = JsonConvert.SerializeObject(result);
                        await UpdateICLogs(id, iCICIResponseDto?.correlationId, responseBody);
                        _logger.LogInformation("Create Breakin Id {response}", JsonConvert.SerializeObject(result));
                        //if (result.brkId != 0)
                        //{
                            //return result.brkId.ToString();
                            return Tuple.Create(result.brkId.ToString(), result.status.ToString(), result.message.ToString());
                        //}
                    }
                    return Tuple.Create("", response.IsSuccessStatusCode.ToString(), "");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Create Breakin Id Error {exception}", ex.Message);
                    await UpdateICLogs(id, iCICIResponseDto?.correlationId, ex.Message);
                    //return default;
                    return Tuple.Create("", "Error", "");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Breakin Id Error {exception}", ex.Message);
                //return default;
                return Tuple.Create("", "Error", "");
            }
        }
        public async Task<ICICIGetBreakinStatusResponseModel> GetBreakinStatus(CancellationToken cancellationToken)
        {
            string token = await GetToken(string.Empty, true, false, false, false, false, "BreakIn");
            var id = 0;
            var responseBody = string.Empty;
            try
            {
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                id = await InsertICLogs(string.Empty, string.Empty, _iCICIConfig.GetBreakinStatus, token, string.Empty, "BreakIn");
                try
                {
                    var response = await _client.GetAsync(_iCICIConfig.GetBreakinStatus);

                    if (!response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        await UpdateICLogs(id, string.Empty, responseBody);
                        _logger.LogError("Get Breakin Status response {response}", response);
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var result = stream.DeserializeFromJson<ICICIGetBreakinStatusResponseModel>();
                        responseBody = JsonConvert.SerializeObject(result);
                        _logger.LogInformation("Get Breakin Status {response}", JsonConvert.SerializeObject(result));
                        await UpdateICLogs(id, result.correlationId, responseBody);
                        if (result != null && result.status && result.statusMessage.ToLower().Equals("success"))
                        {
                            return result;
                        }
                    }
                    return default;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Get Breakin Status Error {exception}", ex.Message);
                    await UpdateICLogs(id, string.Empty, ex.Message);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Breakin Status Error {exception}", ex.Message);
                return default;
            }
        }
        public async Task<ICICIClearInspectionStatusResponse> ClearInspectionStatus(ICICIGetBreakinDetails iCICIGetBreakinDetails, ICICIPOSPDetails iCICIPOSPDetails, CancellationToken cancellationToken)
        {
            var requestBody = string.Empty;
            string token = await GetToken(iCICIGetBreakinDetails.LeadId, true, false, false, false, false, "BreakIn");
            var proposalRequest = JsonConvert.DeserializeObject<ICICIProposalRequestDto>(iCICIGetBreakinDetails?.RequestBody);
            var proposalResponse = JsonConvert.DeserializeObject<ICICIResponseDto>(iCICIGetBreakinDetails?.ResponseBody);
            var id = 0;
            var responseBody = string.Empty;
            try
            {
                var request = new ICICIClearInspectionStatusRequest()
                {
                    InspectionId = iCICIGetBreakinDetails.BreakinId,
                    DealNo = proposalRequest.DealId,
                    ReferenceDate = proposalResponse.generalInformation?.proposalDate,
                    InspectionStatus = "OK",
                    CorrelationId = iCICIGetBreakinDetails.TransactionId,
                    ReferenceNo = proposalResponse.generalInformation?.proposalNumber,
                };
                requestBody = JsonConvert.SerializeObject(request);

                string productCode = iCICIGetBreakinDetails.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler) ? _iCICIConfig.TwoWheelerProdectCode : _iCICIConfig.FourWheelerProdectCode;

                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                if (iCICIPOSPDetails.RoleName.Equals("POSP"))
                {
                    _client.DefaultRequestHeaders.Add("IRDALicenceNumber", _iCICIConfig.LicenseNo);
                    _client.DefaultRequestHeaders.Add("CertificateNumber", iCICIPOSPDetails.POSPId);
                    _client.DefaultRequestHeaders.Add("PanCardNo", iCICIPOSPDetails.PAN);
                    _client.DefaultRequestHeaders.Add("AadhaarNo", iCICIPOSPDetails.AadhaarNumber);
                    _client.DefaultRequestHeaders.Add("ProductCode", productCode);
                }
                id = await InsertICLogs(requestBody, iCICIGetBreakinDetails.LeadId, _iCICIConfig.ClearInspectionStatus, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "BreakIn");
                try
                {
                    var response = await _client.PostAsync(_iCICIConfig.ClearInspectionStatus, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                        cancellationToken);
                    if (!response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        await UpdateICLogs(id, string.Empty, responseBody);
                        _logger.LogError("Clear Inspection Status response {response}", response);
                    }
                    else
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var result = stream.DeserializeFromJson<ICICIClearInspectionStatusResponse>();
                        responseBody = JsonConvert.SerializeObject(result);
                        await UpdateICLogs(id, iCICIGetBreakinDetails?.TransactionId, responseBody);
                        _logger.LogInformation("Clear Inspection Status {response}", JsonConvert.SerializeObject(result));
                        return result;
                    }
                    return default;
                }
                catch (Exception ex)
                {
                    await UpdateICLogs(id, iCICIGetBreakinDetails?.TransactionId, ex.Message);
                    _logger.LogError("Clear Inspection Status Error {exception}", ex.Message);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Clear Inspection Status Error {exception}", ex.Message);
                return default;
            }
        }
        private async Task<Tuple<HttpResponseMessage, int>> GetQuoteResponse(string policyType, bool isTwoWheeler, bool isFourWheeler, bool isCommercial, string token, string requestBody, string leadId, string correlationId, string stage, string ProductCode, CancellationToken cancellationToken)
        {
            var id = 0;
            HttpResponseMessage quoteResponse = new HttpResponseMessage();

            try
            {
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                //if (!isCommercial)
                //{
                string productCode = isCommercial ? ProductCode : isTwoWheeler ? _iCICIConfig.TwoWheelerProdectCode : _iCICIConfig.FourWheelerProdectCode;
                if (_applicationClaims.GetRole().Equals("POSP"))
                {
                    _client.DefaultRequestHeaders.Add("IRDALicenceNumber", _iCICIConfig.LicenseNo);
                    _client.DefaultRequestHeaders.Add("CertificateNumber", _applicationClaims.GetPOSPId());
                    _client.DefaultRequestHeaders.Add("PanCardNo", _applicationClaims.GetPAN());
                    _client.DefaultRequestHeaders.Add("AadhaarNo", _applicationClaims.GetAadhaarNumber());
                    _client.DefaultRequestHeaders.Add("ProductCode", productCode);
                }
                //}

                var url = string.Empty;
                if (isTwoWheeler)
                {
                    if (policyType.Equals("SATP"))
                    {
                        url = _iCICIConfig.QuoteURL2WTP;
                    }
                    else
                    {
                        url = _iCICIConfig.QuoteURL2W;
                    }
                }
                else if (isFourWheeler)
                {
                    if (policyType.Equals("SATP"))
                    {
                        url = _iCICIConfig.QuoteURL4WTP;
                    }
                    else
                    {
                        url = _iCICIConfig.QuoteURL4W;
                    }
                }
                else if (isCommercial)
                {
                    url = _iCICIConfig.QuoteURLCV;
                }
                id = await InsertICLogs(requestBody, leadId, _iCICIConfig.BaseURL + url, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), stage);

                try
                {
                    _logger.LogInformation("ICICI Quote Request {requestBody}", requestBody);
                    quoteResponse = await _client.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                                    cancellationToken);
                    return Tuple.Create(quoteResponse, id);
                }
                catch (Exception ex)
                {
                    _logger.LogError("ICICI GetQuoteResponse {exception}", ex.Message);
                    await UpdateICLogs(id, correlationId, ex.Message);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI GetQuoteResponse {exception}", ex.Message);
                return default;
            }
        }
        private async Task<Tuple<QuoteResponseModel, string>> QuoteResponseFraming(string requestBody, QuoteQueryModel quoteQuery, QuoteResponseModel quoteVm, string correlationId, string ProductCode, CancellationToken cancellationToken)
        {
            var responseBody = string.Empty;
            var quoteResponse = await GetQuoteResponse(quoteQuery.CurrentPolicyType, quoteQuery.VehicleDetails.IsTwoWheeler, quoteQuery.VehicleDetails.IsFourWheeler, quoteQuery.VehicleDetails.IsCommercial, quoteQuery.Token, requestBody, quoteQuery.LeadId, correlationId, "Quote", ProductCode, cancellationToken);

            try
            {
                if (!quoteResponse.Item1.IsSuccessStatusCode)
                {
                    var stream = await quoteResponse.Item1.Content.ReadAsStreamAsync();
                    var result = stream.DeserializeFromJson<ICICIResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogError("Unable to fetch quote {responseBody}", responseBody);
                }
                else
                {
                    var stream = await quoteResponse.Item1.Content.ReadAsStreamAsync();
                    var result = stream.DeserializeFromJson<ICICIResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("ICICI Quote {responseBody}", responseBody);
                    if (result != null && result.status.Equals("Success"))
                    {
                        string totalTax = RoundOffValue(result.totalTax.ToString());
                        decimal? ncbPercentage = Convert.ToInt32(result.riskDetails?.ncbPercentage);
                        var tax = new ServiceTax
                        {
                            totalTax = totalTax
                        };

                        List<NameValueModel> paCoverList = SetPACoverResponse(quoteQuery, result);
                        List<NameValueModel> addOnsList = SetAddOnsResponse(quoteQuery, result);
                        List<NameValueModel> accessoryList = SetAccessoryResponse(quoteQuery, result);
                        List<NameValueModel> discountList = SetDiscountResponse(quoteQuery, result);

                        quoteVm = new QuoteResponseModel
                        {
                            InsurerName = _iCICIConfig.InsurerName,
                            InsurerStatusCode = (int)HttpStatusCode.OK,
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
                            NCB = ncbPercentage.ToString(),
                            TotalPremium = quoteQuery.CurrentPolicyType.Equals("SATP") ? RoundOffValue(result.totalLiabilityPremium) : RoundOffValue(result.packagePremium),
                            GrossPremium = RoundOffValue(result.finalPremium),
                            RTOCode = quoteQuery.RTOLocationCode,
                            PolicyStartDate = quoteQuery.PolicyStartDate,
                            Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                            PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
                            IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                            IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                            RegistrationDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                            ManufacturingDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                            VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.VehicleDetails.LicensePlateNumber : quoteQuery.VehicleNumber
                        };
                    }
                    else
                    {
                        quoteVm.ValidationMessage = result?.message;
                        quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                await UpdateICLogs(quoteResponse.Item2, correlationId, responseBody);
                return Tuple.Create(quoteVm, responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI QuoteResponseFraming {exception}", ex.Message);
                await UpdateICLogs(quoteResponse.Item2, correlationId, ex.Message);
                return default;
            }
        }

        private static List<NameValueModel> SetDiscountResponse(QuoteQueryModel quoteQuery, ICICIResponseDto result)
        {

            string ncbPercentage = Convert.ToString(result.riskDetails.ncbPercentage).Split(".")[0];
            List<NameValueModel> discountList = new List<NameValueModel>();
            if (quoteQuery.Discounts.IsAntiTheft)
            {
                discountList.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.AntiTheftId,
                    Name = "ARAI Approved Anti-Theft Device",
                    Value = RoundOffValue(result.riskDetails?.antiTheftDiscount),
                    IsApplicable = IsApplicable(result.riskDetails?.antiTheftDiscount)
                });
            }
            if (quoteQuery.Discounts.IsAAMemberShip)
            {
                discountList.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.AAMemberShipId,
                    Name = "AA Membership",
                    Value = RoundOffValue(result.riskDetails?.automobileAssociationDiscount),
                    IsApplicable = IsApplicable(result.riskDetails?.automobileAssociationDiscount)
                });
            }
            if (quoteQuery.Discounts.IsVoluntarilyDeductible)
            {
                discountList.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                    Name = "Voluntary Deductible",
                    Value = RoundOffValue(result.riskDetails?.voluntaryDiscount),
                    IsApplicable = IsApplicable(result.riskDetails?.voluntaryDiscount)
                });
            }
            if (!quoteQuery.CurrentPolicyType.Equals("SAOD"))
            {
                discountList.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.LimitedTPCoverageId,
                    Name = "Limited Third Party Coverage",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.tppD_Discount)),
                    IsApplicable = IsApplicable(result.riskDetails?.tppD_Discount),
                });
            }
            discountList.Add(new NameValueModel
            {
                Name = $"No Claim Bonus ({ncbPercentage}%)",
                Value = RoundOffValue(result.riskDetails.bonusDiscount.ToString()),
                IsApplicable = IsApplicable(result.riskDetails.bonusDiscount),
            });
            return discountList;
        }
        private static List<NameValueModel> SetAccessoryResponse(QuoteQueryModel quoteQuery, ICICIResponseDto result)
        {
            List<NameValueModel> accessoryList = new List<NameValueModel>();
            if (quoteQuery.Accessories.IsCNG)
            {
                accessoryList.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover OD",
                    Value = RoundOffValue(result.riskDetails?.biFuelKitOD),
                    IsApplicable = IsApplicable(result.riskDetails?.biFuelKitOD)
                });
                accessoryList.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover TP",
                    Value = RoundOffValue(result.riskDetails?.biFuelKitTP),
                    IsApplicable = IsApplicable(result.riskDetails?.biFuelKitTP)
                });
            }
            if (quoteQuery.Accessories.IsElectrical)
            {
                accessoryList.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.ElectricalId,
                    Name = "Electrical Accessory Cover",
                    Value = RoundOffValue(result.riskDetails?.electricalAccessories),
                    IsApplicable = IsApplicable(result.riskDetails?.electricalAccessories)
                });
            }
            if (quoteQuery.Accessories.IsNonElectrical)
            {
                accessoryList.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.NonElectricalId,
                    Name = "Non-Electrical Accessory Cover",
                    Value = RoundOffValue(result.riskDetails?.nonElectricalAccessories),
                    IsApplicable = IsApplicable(result.riskDetails?.nonElectricalAccessories)
                });
            }
            return accessoryList;
        }
        private static List<NameValueModel> SetPACoverResponse(QuoteQueryModel quoteQuery, ICICIResponseDto result)
        {
            List<NameValueModel> paCoverList = new List<NameValueModel>();
            if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
            {
                paCoverList.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                    Name = "PA Cover for Owner Driver",
                    Value = RoundOffValue(result?.riskDetails?.paCoverForOwnerDriver),
                    IsApplicable = IsApplicable(result?.riskDetails?.paCoverForOwnerDriver)
                });
            }
            if (quoteQuery.PACover.IsPaidDriver)
            {
                paCoverList.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.PaidDriverId,
                    Name = "PA Cover for Paid Driver",
                    Value = RoundOffValue(result?.riskDetails?.paidDriver.ToString()),
                    IsApplicable = IsApplicable(result?.riskDetails?.paidDriver)
                });
            }
            if (quoteQuery.PACover.IsUnnamedPassenger)
            {
                paCoverList.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedPassengerId,
                    Name = "PA Cover for Unnamed Passengers",
                    Value = RoundOffValue(result?.riskDetails?.paCoverForUnNamedPassenger),
                    IsApplicable = IsApplicable(result?.riskDetails?.paCoverForUnNamedPassenger)
                });
            }
            return paCoverList;
        }
        private static List<NameValueModel> SetAddOnsResponse(QuoteQueryModel quoteQuery, ICICIResponseDto result)
        {
            List<NameValueModel> addOnsList = new List<NameValueModel>();
            if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                    Name = "Key And Lock Protection",
                    Value = RoundOffValue(result.riskDetails?.keyProtect),
                    IsApplicable = IsApplicable(result.riskDetails?.keyProtect)
                }
                );
            }
            if (quoteQuery.AddOns.IsTyreProtectionRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.TyreProtectionId,
                    Name = "Tyre Protection",
                    Value = RoundOffValue(result.riskDetails?.tyreProtect),
                    IsApplicable = IsApplicable(result.riskDetails?.tyreProtect)
                }
                );
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
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.NCBId,
                    Name = "No Claim Bonus Protection Protect",
                    Value = null,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsInvoiceCoverRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                    Name = "RTI",
                    Value = RoundOffValue(result.riskDetails?.returnToInvoice),
                    IsApplicable = IsApplicable(result.riskDetails?.returnToInvoice)
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
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ZeroDebtId,
                    Name = "Zero Dep",
                    Value = RoundOffValue(result.riskDetails?.zeroDepreciation),
                    IsApplicable = IsApplicable(result.riskDetails?.zeroDepreciation)
                }
                );
            }
            if (quoteQuery.AddOns.IsEMIProtectorRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.EMIProtectorId,
                    Name = "EMI Protection",
                    Value = RoundOffValue(result.riskDetails?.emiProtect.ToString()),
                    IsApplicable = IsApplicable(Convert.ToString(result.riskDetails?.emiProtect))
                }
                );
            }
            if (quoteQuery.AddOns.IsConsumableRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ConsumableId,
                    Name = "Consumables",
                    Value = RoundOffValue(result.riskDetails?.consumables),
                    IsApplicable = IsApplicable(result.riskDetails?.consumables)
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
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RoadSideAssistanceId,
                    Name = "Road Side Assistance",
                    Value = RoundOffValue(result.riskDetails?.roadSideAssistance),
                    IsApplicable = IsApplicable(result.riskDetails?.roadSideAssistance)
                }
                );
            }
            if (quoteQuery.AddOns.IsRimProtectionRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RimProtectionId,
                    Name = "RIM Protection",
                    Value = null,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsPersonalBelongingRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.PersonalBelongingId,
                    Name = "Personal Belongings",
                    Value = RoundOffValue(result.riskDetails?.lossOfPersonalBelongings),
                    IsApplicable = IsApplicable(result.riskDetails?.lossOfPersonalBelongings)
                }
                );
            }
            if (quoteQuery.AddOns.IsEngineProtectionRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.EngineProtectionId,
                    Name = "Engine Gearbox Protection",
                    Value = RoundOffValue(result.riskDetails?.engineProtect),
                    IsApplicable = IsApplicable(result.riskDetails?.engineProtect)
                }
                );
            }
            if (quoteQuery.AddOns.IsGeoAreaExtension)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    Name = "Geo Area Extension OD",
                    Value = RoundOffValue(result.riskDetails.geographicalExtensionOD.ToString()),
                    IsApplicable = IsApplicable(result.riskDetails.geographicalExtensionOD)
                });
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    Name = "Geo Area Extension TP",
                    Value = RoundOffValue(result.riskDetails.geographicalExtensionTP.ToString()),
                    IsApplicable = IsApplicable(result.riskDetails.geographicalExtensionTP)
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

        private static List<NameValueModel> SetBaseCover(string previousPolicy, ICICIResponseDto result)
        {
            List<NameValueModel> baseCoverList = new List<NameValueModel>();
            if (previousPolicy.Equals("Package Comprehensive") || previousPolicy.Equals("Comprehensive Bundle"))
            {
                baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value = (Convert.ToInt32(RoundOffValue(result.riskDetails.basicOD)) + Convert.ToInt32(RoundOffValue(result.riskDetails.breakinLoadingAmount))).ToString(),
                        IsApplicable = IsApplicable(Convert.ToString(result.totalOwnDamagePremium))
                    },
                    new NameValueModel
                    {
                        Name = "Third Party Cover Premium",
                        Value = RoundOffValue(result.riskDetails.basicTP),
                        IsApplicable = IsApplicable(Convert.ToString(result.riskDetails.basicTP))
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
                        Value = (Convert.ToInt32(RoundOffValue(result.riskDetails.basicOD)) + Convert.ToInt32(RoundOffValue(result.riskDetails.breakinLoadingAmount))).ToString(),
                        IsApplicable = IsApplicable(Convert.ToString(result.totalOwnDamagePremium))
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
                        Value = RoundOffValue(result.riskDetails.basicTP),
                        IsApplicable = IsApplicable(Convert.ToString(result.riskDetails.basicTP))
                   },
                };
            }
            return baseCoverList;
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
                InsurerId = _iCICIConfig.InsurerId,
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


        #region Commercial
        public async Task<Tuple<QuoteResponseModel, string, string>> GetCVQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
        {
            quoteQueryModel = quoteQuery;
            var quoteVm = new QuoteResponseModel();
            string requestBody = string.Empty;
            var responseBody = string.Empty;
            string policyType = quoteQuery.CurrentPolicyType;
            bool isVehicleAgeLessThan3Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 3);
            bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 5);
            //string paTenure = "1";
            string dealId = string.Empty;

            try
            {
                quoteVm.InsurerName = _iCICIConfig.InsurerName;
                var previousPolicyDetails = new PreviousPolicyDetailsCV();

                //dealId = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "DealId").Select(x => x.ConfigValue).FirstOrDefault();
                if (!_applicationClaims.GetRole().Equals("POSP"))
                {
                    dealId = quoteQuery.DealID;
                }

                if (!quoteQuery.IsBrandNewVehicle && quoteQuery.PreviousPolicyDetails.IsPreviousInsurerKnown)
                {
                    if (!(quoteQuery.PreviousPolicyDetails != null && !string.IsNullOrEmpty(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD)))
                    {
                        quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP;
                        quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP;
                    }
                    previousPolicyDetails = new PreviousPolicyDetailsCV
                    {
                        previousPolicyStartDate = quoteQuery.PreviousPolicyDetails?.PreviousPolicyStartDateSAOD,
                        previousPolicyEndDate = quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD,
                        ClaimOnPreviousPolicy = (quoteQuery.PreviousPolicyDetails.IsClaimInLastYear) ? 1 : 0,
                        NoOfClaimsOnPreviousPolicy = (quoteQuery.PreviousPolicyDetails.IsClaimInLastYear) ? 1 : 0,
                        PreviousPolicyType = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == ICICIEnum.PreviousPolicyType).Select(x => x.ConfigValue).FirstOrDefault(),
                        BonusOnPreviousPolicy = quoteQuery.PreviousPolicyDetails.PreviousNoClaimBonus,
                        PreviousVehicleSaleDate = quoteQuery.RegistrationDate,
                        PreviousPolicyNumber = quoteQuery.PreviousPolicyDetails.PreviousPolicyNumber,
                        PreviousInsurerName = quoteQuery.PreviousPolicyDetails.PreviousSAODInsurer.Split(" ")[0],

                        PreviousPolicyTenure = 1,
                        TotalNoOfODClaims = ""
                    };
                }
                else
                {
                    previousPolicyDetails = null;
                    //if (quoteQuery.VehicleDetails.IsTwoWheeler)
                    //{
                    //    paTenure = "5";
                    //}
                    //else
                    //    paTenure = "3";
                }

                var iCICIRequest = new ICICICVRequestDto
                {
                    CorrelationId = quoteQuery.CorrelationId,
                    CategoryId = quoteQuery.CategoryId,
                    BusinessType = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == ICICIEnum.BusinessType).Select(x => x.ConfigValue).FirstOrDefault(),
                    DealId = dealId,
                    VehicleMakeCode = quoteQuery.VehicleDetails.VehicleMakeCode,
                    VehicleModelCode = quoteQuery.VehicleDetails.VehicleModelCode,
                    RTOLocationCode = quoteQuery.RTOLocationCode,
                    //ExShowRoomPrice = quoteQuery.ExShowRoomPrice,

                    ManufacturingYear = quoteQuery.RegistrationYear,
                    DeliveryOrRegistrationDate = quoteQuery.IsBrandNewVehicle ? DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") : quoteQuery.RegistrationDate,
                    FirstRegistrationDate = quoteQuery.IsBrandNewVehicle ? DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") : quoteQuery.RegistrationDate,
                    IsTransferOfNCB = false,
                    TransferOfNCBPercent = 0,
                    IsVehicleHaveLPG = false,
                    IsVehicleHaveCNG = quoteQuery.Accessories.IsCNG,
                    SI_VehicleLPGCNG_KIT = quoteQuery.Accessories.CNGValue,
                    IsFiberGlassFuelTank = false,
                    PolicyStartDate = quoteQuery.IsBrandNewVehicle ? DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") : quoteQuery.PolicyStartDate,
                    PolicyEndDate = quoteQuery.IsBrandNewVehicle ? DateTime.Today.AddYears(1).AddDays(-3).ToString("yyyy-MM-dd") : quoteQuery.PolicyEndDate,
                    CustomerType = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "CustomerType").Select(x => x.ConfigValue).FirstOrDefault(),
                    IsHaveElectricalAccessories = quoteQuery.Accessories.IsElectrical,
                    IsHaveNonElectricalAccessories = quoteQuery.Accessories.IsNonElectrical,
                    SIHaveElectricalAccessories = quoteQuery.Accessories.IsElectrical ? quoteQuery.Accessories.ElectricalValue : 0,
                    SIHaveNonElectricalAccessories = quoteQuery.Accessories.IsNonElectrical ? quoteQuery.Accessories.NonElectricalValue : 0,
                    TPPDLimit = "750000",//quoteQuery.VehicleDetails.VehicleType == "2-Wheeler" ? "6000" : "750000",
                    IsLegalLiabilityToPaidDriver = quoteQuery.PACover.IsPaidDriver,
                    IsLegalLiabilityToPaidEmployee = false,
                    NoOfEmployee = 0,
                    NoOfDriver = quoteQuery.PACover.IsPaidDriver ? 1 : 0,
                    //IsLegaLiabilityToWorkmen = false,
                    NoOfWorkmen = 0,
                    IsPACoverUnnamedPassenger = false,//quoteQuery.PACover.IsUnnamedPassenger,
                    SIPACoverUnnamedPassenger = 0,//quoteQuery.PACover.IsUnnamedPassenger ? Convert.ToDecimal(quoteQuery.UnnamedPassangerAndPillonRider) : 0,
                    IsValidDrivingLicense = false,
                    IsMoreThanOneVehicle = false,
                    IsPACoverOwnerDriver = quoteQuery.PACover.IsUnnamedOWNERDRIVER,
                    IsVoluntaryDeductible = quoteQuery.Discounts.IsVoluntarilyDeductible,

                    // Checke Value quoteQuery.VoluntaryExcess
                    VoluntaryDeductiblePlanName = quoteQuery.Discounts.IsVoluntarilyDeductible ? quoteQuery.VehicleDetails.IsTwoWheeler ? "500" : quoteQuery.VoluntaryExcess : null,

                    IsAutomobileAssocnFlag = quoteQuery.Discounts.IsAAMemberShip,
                    IsAntiTheftDisc = quoteQuery.Discounts.IsAntiTheft,
                    //IsHandicapDisc = quoteQuery.Discounts.IsHandicapDisc,
                    IsExtensionCountry = quoteQuery.AddOns.IsGeoAreaExtension,
                    ExtensionCountryName = quoteQuery.AddOns.IsGeoAreaExtension ? quoteQuery.GeogExtension : null,
                    IsGarageCash = false,
                    GarageCashPlanName = "",
                    ZeroDepPlanName = isVehicleAgeLessThan5Years ? quoteQuery.AddOns.IsZeroDebt ?
        quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "ZeroDepPlanName").Select(x => x.ConfigValue).FirstOrDefault() : string.Empty : string.Empty,
                    IsRTIApplicableflag = quoteQuery.AddOns.IsInvoiceCoverRequired,
                    IsConsumables = isVehicleAgeLessThan5Years ? quoteQuery.AddOns.IsConsumableRequired : false,
                    IsTyreProtect = isVehicleAgeLessThan3Years ? quoteQuery.AddOns.IsTyreProtectionRequired : false,
                    IsEngineProtectPlus = quoteQuery.AddOns.IsEngineProtectionRequired,
                    RSAPlanName = quoteQuery.AddOns.IsRoadSideAssistanceRequired ?
        quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "RSAPlanName").Select(x => x.ConfigValue).FirstOrDefault() : null,
                    KeyProtectPlan = quoteQuery.AddOns.IsKeyAndLockProtectionRequired ?
        quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "KeyProtectPlan").Select(x => x.ConfigValue).FirstOrDefault() : "",
                    LossOfPersonalBelongingPlanName = quoteQuery.AddOns.IsZeroDebt && quoteQuery.AddOns.IsPersonalBelongingRequired ?
        quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "LossOfPersonalBelongingPlanName").Select(x => x.ConfigValue).FirstOrDefault() : "",
                    OtherLoading = 0.0,
                    OtherDiscount = 0.0,
                    GSTToState = quoteQuery.GSTToState,
                    IsPACoverWaiver = quoteQuery.PACover.IsUnnamedOWNERDRIVER ? false : true,
                    IsNoPrevInsurance = !quoteQuery.PreviousPolicyDetails.IsPreviousInsurerKnown ? true : false,
                    PreviousPolicyDetails = previousPolicyDetails,
                    ProductCode = quoteQuery.ProductCode,// 2315 - Goods carrying Vehicle (GCV), 2314 - Passenger Carrying Vehicle (PCV), 2316 - Miscellaneous Vehicles(Misc.)  
                    VehiclebodyPrice = 0,//quoteQuery.VehicleDetails.,
                    VehiclechassisPrice = 0,//quoteQuery.VehicleDetails.,
                    IsPrivateUse = (quoteQuery.PreviousPolicyDetails != null && !string.IsNullOrEmpty(quoteQuery.PreviousPolicyDetails.PreviousPolicyNumber)) ? false : true,
                    IsLimitedToOwnPremises = quoteQuery.AddOns.IsLimitedOwnPremisesRequired,
                    IsNonFarePayingPassengers = false,
                    NoOfNonFarePayingPassenger = 0,
                    IsHirerOrHirersEmployee = false,
                    NoOfHirerOrHirersEmployee = 0,
                    InclusionOfIMT = quoteQuery.AddOns.IsIMT23,
                    NoOfCleanerOrConductor = 0,
                    NoOfTrailerTowed = 0,
                    TrailerType = "",
                    IsLegalLiabilityToWorkmen = false,
                    OverTurningLoading = false,
                    VehicleCarryingcapacity = 0,//quoteQuery.VehicleDetails.VehicleCarryingcapacity,
                    VehicleSeatingCapacity = 0,
                    IsSchoolBus = false,
                    IsNCBProtect = quoteQuery.AddOns.IsNCBRequired,
                    NcbProtectPlanName = quoteQuery.AddOns.IsNCBRequired ?
        quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "NcbProtectPlanName").Select(x => x.ConfigValue).FirstOrDefault() : null,
                    CustomerDetails = new Domain.ICICI.Request.CustomerDetails
                    {
                        CustomerType = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "CustomerType").Select(x => x.ConfigValue).FirstOrDefault(),
                        CustomerName = string.IsNullOrEmpty(_applicationClaims.GetUserName()) ? "Test User" : _applicationClaims.GetUserName(),
                        DateOfBirth = string.Empty,
                        PinCode = "226022",
                        PANCardNo = _applicationClaims.GetPAN() ?? string.Empty,
                        Email = "",
                        MobileNumber = string.IsNullOrEmpty(_applicationClaims.GetMobileNo()) ? "9898989898" : _applicationClaims.GetMobileNo(),
                        AddressLine1 = "Test Address",
                        CountryCode = Convert.ToInt32(quoteQuery.CountryCode),
                        StateCode = Convert.ToInt32(quoteQuery.StateCode),
                        CityCode = Convert.ToInt32(quoteQuery.CityCode),
                        AadharNumber = _applicationClaims.GetAadhaarNumber() ?? string.Empty
                    }

                };

                requestBody = JsonConvert.SerializeObject(iCICIRequest);
                var result = await CVQuoteResponseFraming(requestBody, quoteQuery, quoteVm, iCICIRequest.CorrelationId.ToString(), iCICIRequest.ProductCode, cancellationToken);

                return Tuple.Create(result.Item1, requestBody, result.Item2);
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI CV Error {exception}", ex.Message);
                quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                return Tuple.Create(quoteVm, requestBody, responseBody);
            }
        }

        private async Task<Tuple<QuoteResponseModel, string>> CVQuoteResponseFraming(string requestBody, QuoteQueryModel quoteQuery, QuoteResponseModel quoteVm, string correlationId, string ProductCode, CancellationToken cancellationToken)
        {
            var responseBody = string.Empty;
            var quoteResponse = await GetQuoteResponse(quoteQuery.CurrentPolicyType, quoteQuery.VehicleDetails.IsTwoWheeler, quoteQuery.VehicleDetails.IsFourWheeler, quoteQuery.VehicleDetails.IsCommercial, quoteQuery.Token, requestBody, quoteQuery.LeadId, correlationId, "Quote", ProductCode, cancellationToken);

            try
            {
                if (!quoteResponse.Item1.IsSuccessStatusCode)
                {
                    var stream = await quoteResponse.Item1.Content.ReadAsStreamAsync();
                    var result = stream.DeserializeFromJson<ICICICVResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogError("Unable to fetch quote {responseBody}", responseBody);
                }
                else
                {
                    var stream = await quoteResponse.Item1.Content.ReadAsStreamAsync();
                    var result = stream.DeserializeFromJson<ICICICVResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("ICICI CV Quote {responseBody}", responseBody);
                    if (result != null && string.Equals(result.statusMessage, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        string totalTax = RoundOffValue(Convert.ToString(result.premiumDetails?.totalTax));
                        decimal? ncbPercentage = Convert.ToInt32(result.riskDetails?.ncbPercentage);
                        var tax = new ServiceTax
                        {
                            totalTax = totalTax
                        };

                        List<NameValueModel> paCoverList = CVSetPACoverResponse(quoteQuery, result);
                        List<NameValueModel> addOnsList = CVSetAddOnsResponse(quoteQuery, result);
                        List<NameValueModel> accessoryList = CVSetAccessoryResponse(quoteQuery, result);
                        List<NameValueModel> discountList = CVSetDiscountResponse(quoteQuery, result);

                        quoteVm = new QuoteResponseModel
                        {
                            InsurerName = _iCICIConfig.InsurerName,
                            InsurerStatusCode = (int)HttpStatusCode.OK,
                            SelectedIDV = quoteQuery.SelectedIDV,
                            IDV = quoteQuery.RecommendedIDV,
                            MinIDV = quoteQuery.MinIDV,
                            MaxIDV = quoteQuery.MaxIDV,
                            Tax = tax,
                            BasicCover = new BasicCover
                            {
                                CoverList = CVSetBaseCover(quoteQuery.CurrentPolicyType, result)
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
                            NCB = Convert.ToString(ncbPercentage),
                            TotalPremium = quoteQuery.CurrentPolicyType.Equals("SATP") ? RoundOffValue(Convert.ToString(result.premiumDetails?.totalLiabilityPremium)) : RoundOffValue(Convert.ToString(result.premiumDetails?.packagePremium)),
                            GrossPremium = RoundOffValue(Convert.ToString(result.premiumDetails?.finalPremium)),
                            RTOCode = quoteQuery.RTOLocationCode,
                            PolicyStartDate = quoteQuery.PolicyStartDate,
                            Tenure = Convert.ToString((quoteQuery.VehicleODTenure)) + " OD " + "+ " + Convert.ToString((quoteQuery.VehicleTPTenure)) + " TP",
                            PlanType = Convert.ToString((quoteQuery.VehicleODTenure)) + "OD " + "_" + Convert.ToString((quoteQuery.VehicleTPTenure)) + "TP",
                            IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                            IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                            RegistrationDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                            ManufacturingDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                            VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.VehicleDetails.LicensePlateNumber : quoteQuery.VehicleNumber
                        };
                    }
                    else
                    {
                        quoteVm.ValidationMessage = result?.message;
                        quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                await UpdateICLogs(quoteResponse.Item2, correlationId, responseBody);
                return Tuple.Create(quoteVm, responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI CV QuoteResponseFraming {exception}", ex.Message);
                await UpdateICLogs(quoteResponse.Item2, correlationId, ex.Message);
                return default;
            }
        }

        private static bool IsYearGreaterThanValue(DateTime registrationDate, int yearCheck)
        {
            double year = (DateTime.Now - registrationDate).ToYear();
            return year <= yearCheck;
        }
        private static List<NameValueModel> CVSetDiscountResponse(QuoteQueryModel quoteQuery, ICICICVResponseDto result)
        {

            string ncbPercentage = Convert.ToString(result.riskDetails?.ncbPercentage).Split(".")[0];
            List<NameValueModel> discountList = new List<NameValueModel>();
            if (quoteQuery.Discounts.IsAntiTheft)
            {
                discountList.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.AntiTheftId,
                    Name = "ARAI Approved Anti-Theft Device",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.antiTheftDiscount)),
                    IsApplicable = IsApplicable(Convert.ToString(result.riskDetails?.antiTheftDiscount))
                });
            }
            if (quoteQuery.Discounts.IsAAMemberShip)
            {
                discountList.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.AAMemberShipId,
                    Name = "AA Membership",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.automobileAssociationDiscount)),
                    IsApplicable = IsApplicable(result.riskDetails?.automobileAssociationDiscount)
                });
            }
            if (quoteQuery.Discounts.IsVoluntarilyDeductible)
            {
                discountList.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                    Name = "Voluntary Deductible",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.voluntaryDiscount)),
                    IsApplicable = IsApplicable(result.riskDetails?.voluntaryDiscount)
                });
            }
            if (!quoteQuery.CurrentPolicyType.Equals("SAOD"))
            {
                discountList.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.LimitedTPCoverageId,
                    Name = "Limited Third Party Coverage",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.tppD_Discount)),
                    IsApplicable = IsApplicable(result.riskDetails?.tppD_Discount),
                });
            }
            discountList.Add(new NameValueModel
            {
                Name = $"No Claim Bonus ({ncbPercentage}%)",
                Value = RoundOffValue(Convert.ToString(result.riskDetails.bonusDiscount)),
                IsApplicable = IsApplicable(result.riskDetails.bonusDiscount),
            });
            return discountList;
        }
        private static List<NameValueModel> CVSetAccessoryResponse(QuoteQueryModel quoteQuery, ICICICVResponseDto result)
        {
            List<NameValueModel> accessoryList = new List<NameValueModel>();
            if (quoteQuery.Accessories.IsCNG)
            {
                accessoryList.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover OD",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.biFuelKitOD)),
                    IsApplicable = IsApplicable(result.riskDetails?.biFuelKitOD)
                });
                accessoryList.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover TP",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.biFuelKitTP)),
                    IsApplicable = IsApplicable(result.riskDetails?.biFuelKitTP)
                });
            }
            if (quoteQuery.Accessories.IsElectrical)
            {
                accessoryList.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.ElectricalId,
                    Name = "Electrical Accessory Cover",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.electricalAccessories)),
                    IsApplicable = IsApplicable(result.riskDetails?.electricalAccessories)
                });
            }
            if (quoteQuery.Accessories.IsNonElectrical)
            {
                accessoryList.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.NonElectricalId,
                    Name = "Non-Electrical Accessory Cover",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.nonElectricalAccessories)),
                    IsApplicable = IsApplicable(result.riskDetails?.nonElectricalAccessories)
                });
            }
            return accessoryList;
        }
        private static List<NameValueModel> CVSetPACoverResponse(QuoteQueryModel quoteQuery, ICICICVResponseDto result)
        {
            List<NameValueModel> paCoverList = new List<NameValueModel>();
            if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
            {
                paCoverList.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                    Name = "PA Cover for Owner Driver",
                    Value = RoundOffValue(Convert.ToString(result?.riskDetails?.paCoverForOwnerDriver)),
                    IsApplicable = IsApplicable(result?.riskDetails?.paCoverForOwnerDriver)
                });
            }
            if (quoteQuery.PACover.IsPaidDriver)
            {
                paCoverList.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.PaidDriverId,
                    Name = "PA Cover for Paid Driver",
                    Value = RoundOffValue(Convert.ToString(result?.riskDetails?.paidDriver)),
                    IsApplicable = IsApplicable(result?.riskDetails?.paidDriver)
                });
            }
            if (quoteQuery.PACover.IsUnnamedPassenger)
            {
                paCoverList.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.UnnamedPassengerId,
                    Name = "PA Cover for Unnamed Passengers",
                    Value = RoundOffValue(Convert.ToString(result?.riskDetails?.paCoverForUnNamedPassenger)),
                    IsApplicable = IsApplicable(result?.riskDetails?.paCoverForUnNamedPassenger)
                });
            }
            return paCoverList;
        }
        private static List<NameValueModel> CVSetAddOnsResponse(QuoteQueryModel quoteQuery, ICICICVResponseDto result)
        {
            List<NameValueModel> addOnsList = new List<NameValueModel>();
            if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                    Name = "Key And Lock Protection",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.keyProtect)),
                    IsApplicable = IsApplicable(result.riskDetails?.keyProtect)
                }
                );
            }
            if (quoteQuery.AddOns.IsTyreProtectionRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.TyreProtectionId,
                    Name = "Tyre Protection",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.tyreProtect)),
                    IsApplicable = IsApplicable(result.riskDetails?.tyreProtect)
                }
                );
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
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.NCBId,
                    Name = "No Claim Bonus Protection Protect",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.ncbProtect)),
                    IsApplicable = IsApplicable(result.riskDetails?.ncbProtect)
                }
                );
            }
            if (quoteQuery.AddOns.IsInvoiceCoverRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                    Name = "RTI",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.returnToInvoice)),
                    IsApplicable = IsApplicable(result.riskDetails?.returnToInvoice)
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
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ZeroDebtId,
                    Name = "Zero Dep",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.zeroDepreciation)),
                    IsApplicable = IsApplicable(result.riskDetails?.zeroDepreciation)
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
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ConsumableId,
                    Name = "Consumables",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.consumables)),
                    IsApplicable = IsApplicable(result.riskDetails?.consumables)
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
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RoadSideAssistanceId,
                    Name = "Road Side Assistance",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.roadSideAssistance)),
                    IsApplicable = IsApplicable(result.riskDetails?.roadSideAssistance)
                }
                );
            }
            if (quoteQuery.AddOns.IsRimProtectionRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RimProtectionId,
                    Name = "RIM Protection",
                    Value = null,
                    IsApplicable = false
                }
                );
            }
            if (quoteQuery.AddOns.IsPersonalBelongingRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.PersonalBelongingId,
                    Name = "Personal Belongings",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.lossOfPersonalBelongings)),
                    IsApplicable = IsApplicable(result.riskDetails?.lossOfPersonalBelongings)
                }
                );
            }
            if (quoteQuery.AddOns.IsEngineProtectionRequired)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.EngineProtectionId,
                    Name = "Engine Gearbox Protection",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.engineProtect)),
                    IsApplicable = IsApplicable(result.riskDetails?.engineProtect)
                }
                );
            }
            if (quoteQuery.AddOns.IsGeoAreaExtension)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    Name = "Geo Area Extension OD",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.geographicalExtensionOD)),
                    IsApplicable = IsApplicable(result.riskDetails?.geographicalExtensionOD)
                });
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.GeoAreaExtensionId,
                    Name = "Geo Area Extension TP",
                    Value = RoundOffValue(Convert.ToString(result.riskDetails?.geographicalExtensionTP)),
                    IsApplicable = IsApplicable(result.riskDetails?.geographicalExtensionTP)
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
            if (quoteQuery.AddOns.IsIMT23)
            {
                addOnsList.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.IMT23Id,
                    Name = "IMT 23",
                    Value = (Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.imT23OD))) + Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.imtDiscountOrLoadingValue)))).ToString(),
                    IsApplicable = IsApplicable(result.riskDetails?.imT23OD)
                }
                );
            }
            return addOnsList;
        }
        private static string RoundOffValue(string _val)
        {
            decimal val = Math.Round(Convert.ToDecimal(_val));
            return val.ToString();
        }
        private static bool IsApplicable(object _val)
        {
            string val = Convert.ToString(_val);
            return !(string.IsNullOrEmpty(val) || val == "0" || Convert.ToDecimal(val) == 0);
        }
        private static List<NameValueModel> CVSetBaseCover(string previousPolicy, ICICICVResponseDto result)
        {
            List<NameValueModel> baseCoverList = new List<NameValueModel>();
            if (previousPolicy.Equals("Package Comprehensive") || previousPolicy.Equals("Comprehensive Bundle"))
            {
                baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value = (Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.basicOD))) + (Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.privateUseOD))))).ToString() ,//+ Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.breakinLoadingAmount)))
                        IsApplicable = IsApplicable(Convert.ToString(result.premiumDetails?.totalOwnDamagePremium))
                    },
                    new NameValueModel
                    {
                        Name = "Third Party Cover Premium",
                        Value = (Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.basicTP))) + Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.privateUseTP)))).ToString(),
                        IsApplicable = IsApplicable(Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.basicTP))) + Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.privateUseTP))))
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
                        Value = (Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.basicOD))) + (Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.privateUseOD)))) ).ToString(),//+ Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.breakinLoadingAmount)))
                        IsApplicable = IsApplicable(Convert.ToString(result.premiumDetails?.totalOwnDamagePremium))
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
                        Value = (Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.basicTP))) + Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.privateUseTP)))).ToString(),
                        IsApplicable = IsApplicable(Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.basicTP))) + Convert.ToInt32(RoundOffValue(Convert.ToString(result.riskDetails?.privateUseTP))))
                   },
                };
            }
            return baseCoverList;
        }

        public async Task<Tuple<QuoteResponseModel, string, string>> CreateCvProposal(ICICICVProposalRequestDto proposalQuery, ICICIProposalRequest proposalRequest, ICICICKYCRequest iCICICKYCRequest, string vehicleTypeId, CreateLeadModel createLeadModel, CancellationToken cancellationToken)
        {

            var proposalVm = new QuoteResponseModel();
            string requestBody = string.Empty;
            var responseBody = string.Empty;
            var id = 0;
            string url = string.Empty;
            bool breakin = false;

            try
            {
                proposalQuery.CustomerDetails = new Domain.ICICI.CustomerDetails()
                {
                    CustomerType = proposalQuery.CustomerType,
                    CustomerName = proposalRequest.PersonalDetails.customerName,
                    DateOfBirth = proposalRequest.PersonalDetails.dateOfBirth,
                    PANCardNo = proposalRequest.PersonalDetails.panNumber,
                    Email = proposalRequest.PersonalDetails.emailId,
                    MobileNumber = proposalRequest.PersonalDetails.mobile,
                    PinCode = proposalRequest.AddressDetails.pincode,
                    AddressLine1 = proposalRequest.AddressDetails.addressLine1,
                    CountryCode = 100,
                    StateCode = Convert.ToInt32(proposalRequest.AddressDetails.state),
                    CityCode = Convert.ToInt32(proposalRequest.AddressDetails.city),
                    Gender = proposalRequest.PersonalDetails.gender,
                    MobileISD = "91",
                    AadharNumber = proposalRequest.PersonalDetails.aadharNumbrer,
                    CKYCId = null,
                    EKYCid = null,
                    PEPFlag = iCICICKYCRequest.pep_flag,
                    ILKYCReferenceNumber = createLeadModel.kyc_id
                };
                proposalQuery.NomineeDetails = new Domain.ICICI.NomineeDetails()
                {
                    NameOfNominee = proposalRequest.NomineeDetails.nomineeName,
                    Age = Convert.ToInt32(proposalRequest.NomineeDetails.nomineeAge),
                    Relationship = proposalRequest.NomineeDetails.nomineeRelation
                };


                if (proposalRequest.VehicleDetails.isFinancier.Equals("Yes"))
                {
                    proposalQuery.FinancierDetails = new Domain.ICICI.FinancierDetails()
                    {
                        FinancierName = proposalRequest.VehicleDetails.financer,
                        BranchName = proposalRequest.VehicleDetails.branch,
                        AgreementType = "Hypothecation"
                    };
                }
                proposalQuery.EngineNumber = proposalRequest.VehicleDetails.engineNumber;
                proposalQuery.ChassisNumber = proposalRequest.VehicleDetails.chassisNumber;
                proposalQuery.RegistrationNumber = (proposalQuery.BusinessType.Equals("New Business")) ? "NEW" : proposalQuery.RegistrationNumber;

                proposalVm.InsurerName = "ICICI";
                requestBody = JsonConvert.SerializeObject(proposalQuery);
                string token = await GetToken(createLeadModel.LeadID, true, false, false, false, false, "Proposal");
                //string productCode = string.Empty;

                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                if (_applicationClaims.GetRole().Equals("POSP"))
                {
                    _client.DefaultRequestHeaders.Add("IRDALicenceNumber", _iCICIConfig.LicenseNo);
                    _client.DefaultRequestHeaders.Add("CertificateNumber", _applicationClaims.GetPOSPId());
                    _client.DefaultRequestHeaders.Add("PanCardNo", _applicationClaims.GetPAN());
                    _client.DefaultRequestHeaders.Add("AadhaarNo", _applicationClaims.GetAadhaarNumber());
                    _client.DefaultRequestHeaders.Add("ProductCode", proposalQuery.ProductCode);
                }

                HttpResponseMessage quoteResponse = new HttpResponseMessage();
                url = _iCICIConfig.ProposalURLCV;

                id = await InsertICLogs(requestBody, createLeadModel.LeadID, _iCICIConfig.BaseURL + url, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "Proposal");

                try
                {
                    quoteResponse = await _client.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                                        cancellationToken);

                    string proposalQueryData = JsonConvert.SerializeObject(proposalQuery);
                    QuoteQueryModel quoteQuery = JsonConvert.DeserializeObject<QuoteQueryModel>(proposalQueryData);

                    if (!quoteResponse.IsSuccessStatusCode)
                    {
                        responseBody = await quoteResponse.Content.ReadAsStringAsync(cancellationToken); //quoteResponse.ReasonPhrase;
                        proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                        _logger.LogError("CreateProposal Unable to fetch quote {responseBody}", responseBody);
                    }
                    else
                    {
                        var stream = await quoteResponse.Content.ReadAsStreamAsync();
                        var result = stream.DeserializeFromJson<ICICICVProposalResponseDto>();
                        responseBody = JsonConvert.SerializeObject(result);
                        _logger.LogError("CreateProposal {responseBody}", responseBody);
                        //if (result != null && result.status.Equals("Success"))
                        if (result != null && string.Equals(result.statusMessage, "Success", StringComparison.OrdinalIgnoreCase))
                        {
                            string totalTax = result.premiumDetails?.totalTax.ToString();
                            decimal? ncbPercentage = Convert.ToInt32(result.riskDetails?.ncbPercentage);
                            var tax = new ServiceTax
                            {
                                totalTax = totalTax
                            };

                            breakin = result.breakingFlag || result.isQuoteDeviation || result.isApprovalRequired ? true : false;

                            proposalVm = new QuoteResponseModel
                            {
                                InsurerName = _iCICIConfig.InsurerName,
                                InsurerStatusCode = (int)HttpStatusCode.OK,
                                IDV = quoteQuery.RecommendedIDV,
                                MinIDV = quoteQuery.MinIDV,
                                MaxIDV = quoteQuery.MaxIDV,
                                Tax = tax,
                                NCB = Convert.ToString(ncbPercentage),
                                TotalPremium = Convert.ToString(result.premiumDetails?.packagePremium),
                                GrossPremium = Convert.ToString(result.premiumDetails?.finalPremium),
                                RTOCode = quoteQuery.RTOLocationCode,
                                ProposalNumber = Convert.ToString(result.generalInformation.proposalNumber),
                                IsBreakIn = breakin,
                                isApprovalRequired = result.isApprovalRequired,
                                isQuoteDeviation = result.isQuoteDeviation,
                                ApplicationId = result.correlationId
                            };
                        }
                        else
                        {
                            responseBody = await quoteResponse.Content.ReadAsStringAsync(cancellationToken);//quoteResponse.ReasonPhrase;
                            proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                            proposalVm.ValidationMessage = result.message;
                            _logger.LogError("CreateProposal Unable to fetch quote {responseBody}", responseBody);
                        }
                    }
                    await UpdateICLogs(id, proposalQuery?.CorrelationId, responseBody);
                    return Tuple.Create(proposalVm, requestBody, responseBody);

                }
                catch (Exception ex)
                {
                    _logger.LogError("ICICI Proposal Error {exception}", ex.Message);
                    proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    await UpdateICLogs(id, proposalQuery?.CorrelationId, responseBody);
                    return Tuple.Create(proposalVm, requestBody, responseBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ICICI Proposal Error {exception}", ex.Message);
                proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                return Tuple.Create(proposalVm, requestBody, responseBody);
            }
        }
        #endregion
    }
}
