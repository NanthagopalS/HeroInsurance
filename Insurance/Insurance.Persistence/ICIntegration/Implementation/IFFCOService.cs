using Insurance.Core.Contracts.Common;
using Insurance.Core.Features.IFFCO.Command.CKYC;
using Insurance.Core.Features.IFFCO.Command.CreateCKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Persistence.ICIntegration.Implementation;

public class IFFCOService : IIFFCOService
{
    private readonly ILogger<IFFCOService> _logger;
    private readonly HttpClient _client;
    private readonly IFFCOConfig _iFFCOConfig;
    private readonly ICommonService _commonService;
    private readonly IApplicationClaims _applicationClaims;
    private readonly PolicyTypeConfig _policyTypeConfig;
    private readonly VehicleTypeConfig _vehicleTypeConfig;
    private const string KYC_SUCCESS = "KYC_SUCCESS";
    private const string FAILED = "FAILED";
    private const string POA_REQUIRED = "POA_REQUIRED";
    private const string MESSAGE = "Please enter correct document number or proceed with other insurer";
    private const string POA_SUCCESS = "POA_SUCCESS";
    public IFFCOService(ILogger<IFFCOService> logger,
                         HttpClient client,
                         IOptions<IFFCOConfig> options,
                         ICommonService commonService,
                         IApplicationClaims applicationClaims,
                         IOptions<PolicyTypeConfig> policyType,
                         IOptions<VehicleTypeConfig> vehicleTypeConfig)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _iFFCOConfig = options.Value;
        _commonService = commonService;
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        _policyTypeConfig = policyType.Value;
        _vehicleTypeConfig = vehicleTypeConfig.Value;
    }
    public async Task<IFFCOIDVResponseModel> GetIDV(QuoteQueryModel quoteQueryModel, CancellationToken cancellation)
    {
        IFFCOIDVResponseModel iFFCOIDVResponseModel = new IFFCOIDVResponseModel();
        var requestBody = string.Empty;
        var responseBody = string.Empty;
        var id = 0;
        try
        {
            _logger.LogInformation("Get IFFCO IDV");
            string makeCode = $"{quoteQueryModel.VehicleDetails.VehicleClass}-{quoteQueryModel.VehicleDetails.VehicleMakeCode}-{quoteQueryModel.RegistrationYear}";

            var idvrequest = new IFFCOEnvelope()
            {
                Body = new IFFCOBody()
                {
                    GetVehicleIdv = new GetVehicleIdv()
                    {
                        IdvWebServiceRequest = new IdvWebServiceRequest()
                        {
                            DateOfRegistration = Convert.ToDateTime(quoteQueryModel.RegistrationDate).ToString("MM/dd/yyyy").Replace("-", "/"),
                            InceptionDate = Convert.ToDateTime(quoteQueryModel.PolicyStartDate).ToString("MM/dd/yyyy HH:mm:ss").Replace("-", "/"),
                            MakeCode = makeCode,
                            RtoCity = quoteQueryModel.CityCode
                        }
                    }
                }
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(IFFCOEnvelope));
            StringBuilder requestBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(requestBuilder);
            xmlSerializer.Serialize(stringWriter, idvrequest);
            requestBody = requestBuilder.ToString();
            _logger.LogInformation("IFFCO IDV RequestBody {RequestBody}", requestBody);

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client.DefaultRequestHeaders.Add("SOAPAction", "getVehicleIdv");

            id = await InsertICLogs(requestBody, quoteQueryModel.LeadId, _iFFCOConfig.BaseURL + _iFFCOConfig.IdvURL, string.Empty, _client.DefaultRequestHeaders.ToString(), "Quote");
            try
            {
                var idvResponse = await _client.PostAsync(_iFFCOConfig.IdvURL, new StringContent(requestBody, Encoding.UTF8, "application/xml"), cancellation);

                if (!idvResponse.IsSuccessStatusCode)
                {
                    responseBody = idvResponse.ReasonPhrase;
                    iFFCOIDVResponseModel.StatusCode = (int)idvResponse.StatusCode;
                    iFFCOIDVResponseModel.erorMessage = responseBody;
                    _logger.LogError("IIFCO IDV Data not found {responseBody}", responseBody);
                }
                else
                {
                    responseBody = idvResponse.Content.ReadAsStringAsync().Result.ToString();
                    _logger.LogInformation("IFFCO IDV Response {ResponseBody}", responseBody);

                    StringReader reader = new StringReader(responseBody);
                    var response = (IFFCOEnvelope)(xmlSerializer.Deserialize(reader));
                    if (response.Body.GetVehicleIdvResponse != null && string.IsNullOrEmpty(response.Body.GetVehicleIdvResponse.GetVehicleIdvReturn.erorMessage))
                    {
                        iFFCOIDVResponseModel.Idv = Convert.ToInt32(response.Body.GetVehicleIdvResponse.GetVehicleIdvReturn.Idv);
                        iFFCOIDVResponseModel.maximumIdvAllowed = Convert.ToInt32(response.Body.GetVehicleIdvResponse.GetVehicleIdvReturn.maximumIdvAllowed);
                        iFFCOIDVResponseModel.minimumIdvAllowed = Convert.ToInt32(response.Body.GetVehicleIdvResponse.GetVehicleIdvReturn.minimumIdvAllowed);
                        iFFCOIDVResponseModel.StatusCode = (int)HttpStatusCode.OK;

                        quoteQueryModel.MinIDV = Convert.ToDecimal(response.Body.GetVehicleIdvResponse.GetVehicleIdvReturn.minimumIdvAllowed);
                        quoteQueryModel.MaxIDV = Convert.ToDecimal(response.Body.GetVehicleIdvResponse.GetVehicleIdvReturn.maximumIdvAllowed);
                        quoteQueryModel.RecommendedIDV = Convert.ToDecimal(response.Body.GetVehicleIdvResponse.GetVehicleIdvReturn.Idv);
                    }
                    else
                    {
                        iFFCOIDVResponseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        iFFCOIDVResponseModel.erorMessage = response.Body.GetVehicleIdvResponse.GetVehicleIdvReturn.erorMessage;
                    }
                }
                await UpdateICLogs(id, string.Empty, responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("IFFCO IDV Error {exception}", ex.Message);
                iFFCOIDVResponseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                await UpdateICLogs(id, string.Empty, ex.Message);
                return iFFCOIDVResponseModel;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("IFFCO IDV Error {exception}", ex.Message);
            iFFCOIDVResponseModel.StatusCode = (int)HttpStatusCode.BadRequest;
            await UpdateICLogs(id, string.Empty, ex.Message);
            return iFFCOIDVResponseModel;
        }
        return iFFCOIDVResponseModel;
    }

    public async Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
    {
        var quoteVm = new QuoteResponseModel();
        string requestBody = string.Empty;
        var responseBody = string.Empty;
        try
        {

            var quoteRequest = new IFFCOEnvelope()
            {
                Body = new IFFCOBody()
                {
                    GetNewVehiclePremium = new GetNewVehiclePremium()
                    {
                        PolicyHeader = new PolicyHeader()
                        {
                            MessageId = _iFFCOConfig.MessageId
                        },
                        IFFCOPolicy = new IFFCOPolicy()
                        {
                            ContractType = quoteQuery.VehicleDetails.VehicleClass,
                            ExpiryDate = Convert.ToDateTime(quoteQuery.PolicyEndDate).AddTicks(-1).ToString("MM/dd/yyyy HH:mm:ss").Replace("-", "/"),
                            InceptionDate = Convert.ToDateTime(quoteQuery.PolicyStartDate).ToString("MM/dd/yyyy HH:mm:ss").Replace("-", "/"),
                            PreviousPolicyEndDate = quoteQuery.IsBrandNewVehicle ? null : quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD,
                            IFFCOVehicle = new IFFCOVehicle
                            {
                                Capacity = quoteQuery.VehicleDetails.VehicleSeatCapacity,
                                EngineCpacity = quoteQuery.VehicleDetails.VehicleCubicCapacity,
                                ItgiZone = quoteQuery.VehicleDetails.Zone,
                                Make = quoteQuery.VehicleDetails.VehicleMakeCode,
                                RegictrationCity = quoteQuery.CityCode,
                                RegistrationDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("MM/dd/yyyy").Replace("-", "/"),
                                SeatingCapacity = quoteQuery.VehicleDetails.VehicleSeatCapacity,
                                Type = quoteQuery.PlanType,
                                NewVehicleFlag = quoteQuery.IsBrandNewVehicle ? "Y" : "N",
                                VehicleClass = quoteQuery.VehicleDetails.VehicleClass,
                                GrossVehicleWt = quoteQuery.VehicleDetails.GrossVehicleWeight,
                                VehicleCoverage = new VehicleCoverage
                                {
                                    item = new List<Item>
                                    {
                                        new Item
                                        {
                                            CoverageId = "IDV Basic",
                                            SumInsured = quoteQuery.CurrentPolicyType.Equals("SATP") ? "1" :quoteQuery.RecommendedIDV.ToString()
                                        }
                                    }
                                },
                                VehicleSubclass = quoteQuery.VehicleDetails.VehicleClass,
                                YearOfManufacture = quoteQuery.RegistrationYear,
                                Zcover = quoteQuery.PlanType.Equals(_iFFCOConfig.SATPPlanType) ? "AC" : "CO"
                            }
                        },
                        Partner = new Partner()
                        {
                            PartnerBranch = quoteQuery.VehicleDetails.IsCommercialVehicle ? _iFFCOConfig.CVPartnerBranch : _iFFCOConfig.PartnerBranch,
                            PartnerCode = quoteQuery.VehicleDetails.IsCommercialVehicle ? _iFFCOConfig.CVPartnerCode : _iFFCOConfig.PartnerCode,
                            PartnerSubBranch = quoteQuery.VehicleDetails.IsCommercialVehicle ? _iFFCOConfig.CVPartnerSubBranch : _iFFCOConfig.PartnerSubBranch
                        }
                    }
                }
            };
            if (quoteQuery.VehicleDetails.IsCommercialVehicle)
            {
                GetCVCoverMapping(quoteQuery, quoteRequest);
            }
            else
            {
                GetCoverMapping(quoteQuery, quoteRequest);
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(IFFCOEnvelope));
            StringBuilder requestBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(requestBuilder);
            xmlSerializer.Serialize(stringWriter, quoteRequest);
            requestBody = requestBuilder.ToString();
            bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(quoteQuery.RegistrationYear, 5);
            return await QuoteResponseFraming(requestBody, quoteQuery, quoteVm, isVehicleAgeLessThan5Years, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("IFFCO Error {exception}", ex.Message);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return Tuple.Create(quoteVm, requestBody, responseBody);
        }
    }

    public async Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        QuoteConfirmDetailsResponseModel quoteConfirm = new QuoteConfirmDetailsResponseModel();

        var quoteResponseVM = new QuoteConfirmResponseModel();
        var responseBody = string.Empty;
        string planType = string.Empty;
        ServiceTaxModel tax = new ServiceTaxModel();
        string commonResponse = quoteTransactionDbModel.QuoteTransactionRequest.CommonResponse;
        QuoteResponseModel updatedResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(commonResponse);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(IFFCOEnvelope));
        StringReader reader = new StringReader(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody.ToString().Replace("getMotorPremium", "getNewVehiclePremium"));
        var requestframing = (IFFCOEnvelope)(xmlSerializer.Deserialize(reader));
        string requestBody = string.Empty;
        bool isBreakin = false;
        bool isPolicyExpired = false;
        bool policyTypeSelfInspection = false;
        DateTime prevPolicyExpiryDate = new DateTime();
        DateTime currentDateTime = DateTime.UtcNow.Date;
        var responseReferance = string.Empty;

        if (requestframing != null)
        {
            string manufactureYear = (quoteConfirmCommand.PolicyDates.ManufacturingDate).Substring(Math.Max(0, quoteConfirmCommand.PolicyDates.ManufacturingDate.Length - 4));
            var iFFCOVehicle = requestframing.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle;
            iFFCOVehicle.YearOfManufacture = manufactureYear;

            if (!quoteConfirmCommand.IsBrandNewVehicle)
            {
                iFFCOVehicle.RegistrationDate = Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("MM/dd/yyyy").Replace("-", "/");
                requestframing.Body.GetNewVehiclePremium.IFFCOPolicy.PreviousPolicyEndDate = quoteConfirmCommand.PreviousPolicy.SAODPolicyExpiryDate;
                if (!quoteConfirmCommand.PolicyDates.IsTwoWheeler)
                {
                    if (quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId != null && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId != null && quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId.Equals(_policyTypeConfig.SATP) && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive))
                    {
                        policyTypeSelfInspection = true;
                    }

                    if (quoteTransactionDbModel.LeadDetail.PolicyTypeId != null && quoteTransactionDbModel.LeadDetail.PolicyTypeId.Equals(_policyTypeConfig.SATP))
                    {
                        prevPolicyExpiryDate = Convert.ToDateTime(quoteConfirmCommand.PreviousPolicy.TPPolicyExpiryDate);
                    }
                    else
                    {
                        prevPolicyExpiryDate = Convert.ToDateTime(quoteConfirmCommand.PreviousPolicy.SAODPolicyExpiryDate);
                    }
                    if (DateTime.Compare(prevPolicyExpiryDate, currentDateTime) < 0 && DateTime.Compare(prevPolicyExpiryDate, currentDateTime) != 0)
                    {
                        isPolicyExpired = true;
                    }
                    if (isPolicyExpired || policyTypeSelfInspection)
                    {
                        isBreakin = true;
                        var startDate = Convert.ToDateTime(requestframing.Body.GetNewVehiclePremium.IFFCOPolicy.InceptionDate).AddDays(3).ToString("MM/dd/yyyy HH:mm:ss").Replace("-", "/");
                        var endDate = Convert.ToDateTime(requestframing.Body.GetNewVehiclePremium.IFFCOPolicy.ExpiryDate).AddDays(3).ToString("MM/dd/yyyy HH:mm:ss").Replace("-", "/");
                        requestframing.Body.GetNewVehiclePremium.IFFCOPolicy.InceptionDate = startDate;
                        requestframing.Body.GetNewVehiclePremium.IFFCOPolicy.ExpiryDate = endDate;
                    }
                }
            }

            if (!string.IsNullOrEmpty(quoteConfirmCommand.CurrentNCBPercentage) && quoteConfirmCommand.CurrentNCBPercentage != "0")
            {
                if (iFFCOVehicle.VehicleCoverage?.item?.Find(x => x.CoverageId.Equals("No Claim Bonus")) != null)
                {
                    iFFCOVehicle.VehicleCoverage.item.Find(x => x.CoverageId.Equals("No Claim Bonus")).SumInsured = quoteConfirmCommand.CurrentNCBPercentage;
                }
                else
                {
                    iFFCOVehicle.VehicleCoverage.item.Add(new Item
                    {
                        CoverageId = "No Claim Bonus",
                        Number = string.Empty,
                        SumInsured = quoteConfirmCommand.CurrentNCBPercentage
                    });
                }

            }

            planType = iFFCOVehicle?.Type;
            var paItem = iFFCOVehicle.VehicleCoverage?.item?.Find(x => x.CoverageId.Equals("PA Owner / Driver"));
            if (quoteConfirmCommand.IsPACover && paItem != null)
            {
                paItem.SumInsured = "N";
            }
            else if (!quoteConfirmCommand.IsPACover && paItem != null)
            {
                iFFCOVehicle.VehicleCoverage.item.Find(x => x.CoverageId.Equals("PA Owner / Driver")).Number = quoteConfirmCommand.PACoverTenure;
            }
            else if (!iFFCOVehicle.Type.Equals("OD"))
            {
                iFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "PA Owner / Driver",
                    Number = quoteConfirmCommand.PACoverTenure,
                    SumInsured = "Y"
                });
            }

            StringBuilder requestBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(requestBuilder);
            xmlSerializer.Serialize(stringWriter, requestframing);
            requestBody = requestBuilder.ToString();
            bool isCommercial = (quoteConfirmCommand.VehicleTypeId == "88a807b3-90e4-484b-b5d2-65059a8e1a91") ? true : false;
            var getQuoteResponse = await GetQuoteResponse(quoteConfirmCommand.IsBrandNewVehicle, planType, quoteTransactionDbModel.LeadDetail.LeadID, requestBody, "QuoteConfirm", isCommercial, cancellationToken);



            if (!getQuoteResponse.Item1.IsSuccessStatusCode)
            {
                responseBody = getQuoteResponse.Item1.ReasonPhrase;
                quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogError("IIFCO QuoteConfirm Data not found {responseBody}", responseBody);
            }
            else
            {
                responseBody = getQuoteResponse.Item1.Content.ReadAsStringAsync().Result.ToString();
                reader = new StringReader(responseBody);
                var response = (IFFCOEnvelope)(xmlSerializer.Deserialize(reader));
                int responseBodyIndex;
                bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(manufactureYear, 5);
                if (iFFCOVehicle.VehicleCoverage?.item?.Find(x => x.CoverageId.Equals("Depreciation Waiver") || x.CoverageId.Equals("Towing & Related") || x.CoverageId.Equals("NCB Protection") || x.CoverageId.Equals("Consumable")) != null && isVehicleAgeLessThan5Years)
                {
                    responseBodyIndex = 1;
                }
                else
                {
                    responseBodyIndex = 0;
                }

                if (response != null && string.IsNullOrEmpty(response.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[responseBodyIndex]?.iFFCOError?.errorCode))
                {
                    var quoteResponse = (dynamic)null;
                    if (quoteConfirmCommand.IsBrandNewVehicle && !isCommercial)
                    {
                        if (iFFCOVehicle.VehicleCoverage?.item?.Find(x => x.CoverageId.Equals("Depreciation Waiver") || x.CoverageId.Equals("Towing & Related") || x.CoverageId.Equals("NCB Protection") || x.CoverageId.Equals("Consumable")) != null)
                        {
                            quoteResponse = response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1];
                            tax.totalTax = RoundOffValue(response.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1]?.gstAmount);
                            responseReferance = "1";
                        }
                        else
                        {
                            quoteResponse = response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0];
                            tax.totalTax = RoundOffValue(response.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.gstAmount);
                            responseReferance = "0";
                        }
                    }
                    else if (isCommercial)
                    {
                        if (iFFCOVehicle.VehicleCoverage?.item?.Find(x => x.CoverageId.Equals("Depreciation Waiver") || x.CoverageId.Equals("IMT 23") || x.CoverageId.Equals("Consumable")) != null)
                        {
                            quoteResponse = response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1];
                            tax.totalTax = RoundOffValue(response.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1]?.serviceTax);
                            responseReferance = "1";
                        }
                        else
                        {
                            quoteResponse = response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0];
                            tax.totalTax = RoundOffValue(response.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0]?.serviceTax);
                            responseReferance = "0";
                        }
                    }
                    else
                    {
                        if (iFFCOVehicle.VehicleCoverage?.item?.Find(x => x.CoverageId.Equals("Depreciation Waiver") || x.CoverageId.Equals("Towing & Related") || x.CoverageId.Equals("NCB Protection") || x.CoverageId.Equals("Consumable")) != null)
                        {
                            quoteResponse = response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1];
                            tax.totalTax = RoundOffValue(response.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1]?.serviceTax);
                            responseReferance = "1";
                        }
                        else
                        {
                            quoteResponse = response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0];
                            tax.totalTax = RoundOffValue(response.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0]?.serviceTax);
                            responseReferance = "0";
                        }
                    }
                    updatedResponse.GrossPremium = RoundOffValue(quoteResponse.premiumPayable);
                    quoteConfirm = new QuoteConfirmDetailsResponseModel()
                    {
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        InsurerName = _iFFCOConfig.InsurerName,
                        NewPremium = RoundOffValue(quoteResponse.premiumPayable),
                        InsurerId = _iFFCOConfig.InsurerId,
                        NCB = quoteConfirmCommand?.CurrentNCBPercentage,
                        Tax = tax,
                        TotalPremium = RoundOffValue(quoteResponse.totalPremimAfterDiscLoad),
                        GrossPremium = RoundOffValue(quoteResponse.premiumPayable),
                        IsBreakin = isBreakin,
                        IsSelfInspection = policyTypeSelfInspection,
                        IDV = Convert.ToInt32(updatedResponse.IDV),
                        MinIDV = Convert.ToInt32(updatedResponse.MinIDV),
                        MaxIDV = Convert.ToInt32(updatedResponse.MaxIDV)
                    };
                }
                else
                {
                    quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    quoteConfirm.ValidationMessage = quoteConfirmCommand.IsBrandNewVehicle ? response?.Body.GetNewVehiclePremiumResponse.GetNewVehiclePremiumReturn[1].iFFCOError.errorMessage : response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn?.Find(x => x.iFFCOError != null)?.iFFCOError.errorMessage;
                }
            }

            quoteResponseVM = new QuoteConfirmResponseModel()
            {
                quoteConfirmResponse = quoteConfirm,
                quoteResponse = updatedResponse,
                RequestBody = requestBody,
                ResponseBody = responseBody,
                LeadId = quoteTransactionDbModel.LeadDetail.LeadID,
                ResponseReferanceFlag = responseReferance
            };
            await UpdateICLogs(getQuoteResponse.Item2, string.Empty, responseBody);
            return quoteResponseVM;
        }
        return default;
    }

    private async Task<Tuple<QuoteResponseModel, string, string>> QuoteResponseFraming(string requestBody, QuoteQueryModel quoteQuery, QuoteResponseModel quoteVm, bool isVehicleAgeLessThan5Years, CancellationToken cancellationToken)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(IFFCOEnvelope));
        var responseBody = string.Empty;
        var tax = new ServiceTax();
        bool isSuccessResponse = false;

        var getQuoteResponse = await GetQuoteResponse(quoteQuery.IsBrandNewVehicle, quoteQuery.PlanType, quoteQuery.LeadId, requestBody, "Quote", quoteQuery.VehicleDetails.IsCommercialVehicle, cancellationToken);

        if (!getQuoteResponse.Item1.IsSuccessStatusCode)
        {
            responseBody = getQuoteResponse.Item1.ReasonPhrase;
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            _logger.LogError("IIFCO Quote Data not found {responseBody}", responseBody);
        }
        else
        {
            int responseBodyIndex;
            responseBody = getQuoteResponse.Item1.Content.ReadAsStringAsync().Result.ToString();
            StringReader reader = new StringReader(responseBody);
            var response = (IFFCOEnvelope)(xmlSerializer.Deserialize(reader));
            if ((quoteQuery.AddOns.IsZeroDebt || quoteQuery.AddOns.IsTowingRequired || quoteQuery.AddOns.IsNCBRequired || quoteQuery.AddOns.IsConsumableRequired) && isVehicleAgeLessThan5Years)
            {
                responseBodyIndex = 1;
            }
            else
            {
                responseBodyIndex = 0;
            }

            if (quoteQuery.VehicleDetails.IsCommercialVehicle)
            {
                isSuccessResponse = string.IsNullOrEmpty(response.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1]?.iFFCOError?.errorCode) && !string.IsNullOrEmpty(response.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[responseBodyIndex]?.premiumPayable) ? true : false;
            }
            else if (quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle)
            {
                isSuccessResponse = string.IsNullOrEmpty(response.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[responseBodyIndex]?.iFFCOError?.errorCode) && !string.IsNullOrEmpty(response.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[responseBodyIndex]?.premiumPayable) ? true : false;
            }
            else
            {
                isSuccessResponse = string.IsNullOrEmpty(response.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[responseBodyIndex]?.iFFCOError?.errorCode) && !string.IsNullOrEmpty(response.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[responseBodyIndex]?.premiumPayable) ? true : false;
            }
            if (response != null && isSuccessResponse)
            {
                var quoteResponse = (dynamic)null;
                if (quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle)
                {
                    if ((quoteQuery.AddOns.IsZeroDebt || quoteQuery.AddOns.IsTowingRequired || quoteQuery.AddOns.IsNCBRequired || quoteQuery.AddOns.IsConsumableRequired) && isVehicleAgeLessThan5Years)
                    {
                        quoteResponse = response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1];
                        tax.totalTax = RoundOffValue(response.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1]?.gstAmount);
                    }
                    else
                    {
                        quoteResponse = response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0];
                        tax.totalTax = RoundOffValue(response.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.gstAmount);
                    }
                }
                else
                {
                    if ((quoteQuery.AddOns.IsZeroDebt || quoteQuery.AddOns.IsIMT23 || quoteQuery.AddOns.IsConsumableRequired) && isVehicleAgeLessThan5Years)
                    {
                        quoteResponse = response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1];
                        tax.totalTax = RoundOffValue(response.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1]?.serviceTax);
                    }
                    else
                    {
                        quoteResponse = response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0];
                        tax.totalTax = RoundOffValue(response.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0]?.serviceTax);
                    }
                }

                List<NameValueModel> paCoverList = SetPACoverResponse(quoteQuery, response);
                List<NameValueModel> addOnsList = SetAddOnsResponse(quoteQuery, response, isVehicleAgeLessThan5Years);
                List<NameValueModel> accessoryList = SetAccessoryResponse(quoteQuery, response);
                List<NameValueModel> discountList = SetDiscountResponse(quoteQuery, response);

                quoteVm = new QuoteResponseModel
                {
                    InsurerName = _iFFCOConfig.InsurerName,
                    InsurerStatusCode = (int)HttpStatusCode.OK,
                    SelectedIDV = quoteQuery.SelectedIDV,
                    IDV = quoteQuery.RecommendedIDV,
                    MinIDV = quoteQuery.MinIDV,
                    MaxIDV = quoteQuery.MaxIDV,
                    Tax = tax,
                    BasicCover = new BasicCover
                    {
                        CoverList = SetBaseCover(quoteQuery.CurrentPolicyType, quoteQuery.IsBrandNewVehicle, quoteQuery.VehicleDetails.IsCommercialVehicle, response)
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
                    NCB = quoteQuery.CurrentNCBPercentage,
                    TotalPremium = RoundOffValue(quoteResponse.totalPremimAfterDiscLoad),
                    GrossPremium = RoundOffValue(quoteResponse.premiumPayable),
                    RTOCode = quoteQuery.RTOLocationCode,
                    PolicyStartDate = Convert.ToDateTime(quoteQuery.PolicyStartDate).ToString("dd-MMM-yyyy"),
                    Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                    PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
                    IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                    IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                    RegistrationDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                    ManufacturingDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                    VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.RTOLocationCode : quoteQuery.VehicleNumber
                };
            }
            else
            {
                quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteVm.ValidationMessage = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ? response?.Body.GetNewVehiclePremiumResponse.GetNewVehiclePremiumReturn[1].iFFCOError.errorMessage : response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn?.FirstOrDefault(x => x.iFFCOError != null)?.iFFCOError.errorMessage;
            }
        }
        await UpdateICLogs(getQuoteResponse.Item2, string.Empty, responseBody);
        return Tuple.Create(quoteVm, getQuoteResponse.Item3, responseBody);
    }

    public async Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(IFFCOCKYCCommand iffcoCKYCCommand, CancellationToken cancellationToken)
    {
        string responseBody = string.Empty;
        string requestBody = string.Empty;
        SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();
        CreateLeadModel createLeadModel = new CreateLeadModel();
        createLeadModel.PermanentAddress = new LeadAddressModel();
        var id = 0;
        var authenticationString = string.Empty;
        if (iffcoCKYCCommand.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
        {
            authenticationString = $"{_iFFCOConfig.BasicAuth.CVUsername}:{_iFFCOConfig.BasicAuth.CVPassword}";
        }
        else
        {
            authenticationString = $"{_iFFCOConfig.BasicAuth.UserName}:{_iFFCOConfig.BasicAuth.Password}";
        }
        var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        IffcoCKYCFetchRequestModel iffcoCKYCFetchRequestModel = new IffcoCKYCFetchRequestModel()
        {
            firstName = iffcoCKYCCommand?.firstName,
            middleName = iffcoCKYCCommand?.middleName,
            lastName = iffcoCKYCCommand?.lastName,
            dateofBirth = Convert.ToDateTime(iffcoCKYCCommand?.DateOfBirth).ToString("dd-MM-yyyy"),
            gender = iffcoCKYCCommand?.Gender,
            idNumber = iffcoCKYCCommand?.DocumentId,
            clientType = iffcoCKYCCommand.CustomerType.Equals("I", StringComparison.Ordinal) ? "IND" : "LE",
        };
        switch (iffcoCKYCCommand.DocumentType.ToUpper())
        {
            case ("PAN"):
                iffcoCKYCFetchRequestModel.idType = "PAN";
                break;
            case ("PASSPORT"):
                iffcoCKYCFetchRequestModel.idType = "PASSPORT";
                break;
            case ("ITGIUNIQUEIDENTIFIER"):
                iffcoCKYCFetchRequestModel.idType = "ITGI UNIQUE IDENTIFIER";
                break;
            case ("CKYCIDENTIFIER"):
                iffcoCKYCFetchRequestModel.idType = "CKYC IDENTIFIER";
                break;
            case ("VOTERID"):
                iffcoCKYCFetchRequestModel.idType = "VOTER ID";
                break;
            case ("DRIVINGLICENSE"):
                iffcoCKYCFetchRequestModel.idType = "DRIVING LICENSE";
                break;
            case ("AADHARCARDNUMBER"):
                iffcoCKYCFetchRequestModel.idType = "AADHAR CARD NUMBER";
                break;
            case ("NREGAJOBCARD"):
                iffcoCKYCFetchRequestModel.idType = "NREGA JOB CARD";
                break;
            case ("NATIONALPOPULATIONREGISTERLETTER"):
                iffcoCKYCFetchRequestModel.idType = "NATIONAL POPULATION REGISTER LETTER";
                break;
        }
        requestBody = JsonConvert.SerializeObject(iffcoCKYCFetchRequestModel);
        _logger.LogInformation("GetCKYCResponse requestBody {requestBody}", requestBody);
        id = await InsertICLogs(requestBody, iffcoCKYCCommand.LeadId, _iFFCOConfig.BaseURL + _iFFCOConfig.VerifyCKYCURL, string.Empty, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "KYC");
        try
        {
            var response = await _client.PostAsync(_iFFCOConfig.VerifyCKYCURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
            cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                responseBody = response.ReasonPhrase;
                _logger.LogError("GetCKYCResponse error {responseBody}", responseBody);
            }
            else
            {
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var result = stream.DeserializeFromJson<IffcoCKYCFetchResponseModel>();
                responseBody = JsonConvert.SerializeObject(result);
                _logger.LogInformation("GetCKYCResponse responseBody {responseBody}", responseBody);
                saveCKYCResponse.IsKYCRequired = true;

                if (result != null && result.result.status.Equals("CKYCSuccess"))
                {
                    if (iffcoCKYCCommand.CustomerType.Equals("I"))
                    {

                        createLeadModel.LeadName = string.Concat(result.result.firstName, " ", result.result.middleName, " ").Replace("  ", " ").Trim();
                        createLeadModel.LastName = result.result.lastName;
                        createLeadModel.Gender = result.result.gender;
                        createLeadModel.DOB = string.Join("-", result.result.dateofBirth.Split("-").ToArray().Reverse());
                        createLeadModel.Salutation = result.result.prefix == "MISS" ? "MS" : result.result.prefix;
                    }
                    else
                    {
                        createLeadModel.LeadName = result.result.firstName;
                        createLeadModel.DateOfIncorporation = result.result.dateofBirth;
                    }
                    createLeadModel.kyc_id = result.result.itgiUniqueReferenceId;
                    createLeadModel.PermanentAddress = new LeadAddressModel()
                    {
                        AddressType = "PRIMARY",
                        Address1 = result.result.addressLine1,
                        Address2 = string.Concat(result.result.city, " ", result.result.district, " ", result.result.state),
                        Pincode = result.result.pinCode,

                    };
                    createLeadModel.CommunicationAddress = new LeadAddressModel()
                    {
                        AddressType = "SECONDARY",
                        Address1 = result.result.correspondenceAddressLine1,
                        Address2 = string.Concat(result.result.correspondenceCity, " ", result.result.correspondenceDistrict, " ", result.result.correspondenceState),
                        Pincode = result.result.correspondencePinCode,
                    };
                    string address = string.Empty;
                    saveCKYCResponse.Name = string.Concat(result.result.firstName, " ", result.result.middleName, " ", result.result.lastName).Replace("  ", " ").Trim();
                    saveCKYCResponse.MiddleName = result.result.middleName;
                    saveCKYCResponse.LastName = result.result.lastName;
                    saveCKYCResponse.Gender = result.result?.gender == "M" ? "Male" : "Female";
                    saveCKYCResponse.DOB = string.Join("-", result.result.dateofBirth.Split("-").ToArray().Reverse());
                    saveCKYCResponse.KYCId = result.result.itgiUniqueReferenceId;
                    saveCKYCResponse.IsKYCRequired = true;
                    saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                    saveCKYCResponse.Message = KYC_SUCCESS;
                    saveCKYCResponse.InsurerName = _iFFCOConfig.InsurerName;
                    address = string.IsNullOrEmpty(result.result.addressLine1) ? "" : result.result.addressLine1 + ",";
                    address += $"{result.result.city},{result.result.district},{result.result.state},{result.result.pinCode}";
                    saveCKYCResponse.Address = address;
                    await UpdateICLogs(id, result.result.itgiUniqueReferenceId, responseBody);
                    return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
                }
                else
                {
                    saveCKYCResponse.KYC_Status = POA_REQUIRED;
                    saveCKYCResponse.Message = result?.result.status.ToString();
                    saveCKYCResponse.InsurerName = _iFFCOConfig.InsurerName;
                    await UpdateICLogs(id, string.Empty, responseBody);
                    return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
                }
            }
            saveCKYCResponse.KYC_Status = POA_REQUIRED;
            saveCKYCResponse.Message = MESSAGE;
            saveCKYCResponse.InsurerName = _iFFCOConfig.InsurerName;
            await UpdateICLogs(id, string.Empty, responseBody);
            return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
        }
        catch (Exception ex)
        {
            saveCKYCResponse.KYC_Status = FAILED;
            saveCKYCResponse.Message = MESSAGE;
            _logger.LogError("Iffco Ckyc Error {exception}", ex.Message);
            await UpdateICLogs(id, string.Empty, ex.Message);
            return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
        }
    }
    public async Task<Tuple<string, string, UploadCKYCDocumentResponse, CreateLeadModel>> UploadCKYCDocument(CreateIFFCOCKYCCommand createIFFCOCKYCCommand, CancellationToken cancellationToken)
    {
        var responseBody = string.Empty;
        string requestBody = string.Empty;
        UploadCKYCDocumentResponse uploadCKYCDocument = new UploadCKYCDocumentResponse();
        CreateLeadModel createLeadModel = new CreateLeadModel();
        createLeadModel.PermanentAddress = new LeadAddressModel();
        var authenticationString = string.Empty;
        if (createIFFCOCKYCCommand.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
        {
            authenticationString = $"{_iFFCOConfig.BasicAuth.CVUsername}:{_iFFCOConfig.BasicAuth.CVPassword}";
        }
        else
        {
            authenticationString = $"{_iFFCOConfig.BasicAuth.UserName}:{_iFFCOConfig.BasicAuth.Password}";
        }
        var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        var id = 0;
        try
        {
            IFFCOCreateCKYCRequestModel iFFCOCreateCKYCRequestModel = new IFFCOCreateCKYCRequestModel()
            {
                firstName = createIFFCOCKYCCommand.FirstName,
                dateofBirth = Convert.ToDateTime(createIFFCOCKYCCommand.DateOfBirth).ToString("dd-MM-yyyy"),
                mobileNumber = createIFFCOCKYCCommand.Mobile,
                emailAddress = createIFFCOCKYCCommand.EmailId,
                addressLine1 = createIFFCOCKYCCommand.Address,
                pinCode = createIFFCOCKYCCommand.Pincode,
                city = createIFFCOCKYCCommand.City,
                state = createIFFCOCKYCCommand.State,
                country = "India",//createIFFCOCKYCCommand.Country,
                district = createIFFCOCKYCCommand.City,
                correspondenceAddressLine1 = createIFFCOCKYCCommand.CorrespondenceAddress,
                correspondencePinCode = createIFFCOCKYCCommand.CorrespondencePincode,
                correspondenceCity = createIFFCOCKYCCommand.City,
                correspondenceState = createIFFCOCKYCCommand.State,
                correspondenceCountry = "India",//createIFFCOCKYCCommand.CorrespondenceCountry,
                correspondenceDistrict = createIFFCOCKYCCommand.City
            };
            if (createIFFCOCKYCCommand.CustomerType.Equals("INDIVIDUAL"))
            {
                iFFCOCreateCKYCRequestModel.prefix = createIFFCOCKYCCommand.Salutation;
                iFFCOCreateCKYCRequestModel.gender = createIFFCOCKYCCommand.Gender;
                iFFCOCreateCKYCRequestModel.lastName = createIFFCOCKYCCommand.LastName;
                iFFCOCreateCKYCRequestModel.clientType = "IND";
                iFFCOCreateCKYCRequestModel.fatherPrefix = "Mr.";
                iFFCOCreateCKYCRequestModel.fatherFirstName = createIFFCOCKYCCommand.FatherFirstName;
                iFFCOCreateCKYCRequestModel.fatherLastName = createIFFCOCKYCCommand.FatherLastName;
                iFFCOCreateCKYCRequestModel.kycDocuments = new Kycdocument[]
                {
                    new Kycdocument()
                    {
                        idType = "IDENTITY_PROOF",
                        idName = createIFFCOCKYCCommand.ProofOfIdentity,
                        idNumber = createIFFCOCKYCCommand.POIDocumentId,
                        fileName = createIFFCOCKYCCommand.POIDocumentName,
                        fileExtension = createIFFCOCKYCCommand.poiDocumentUploadExtension,
                        fileBase64 = createIFFCOCKYCCommand.POIDocumentUpload
                    },
                    new Kycdocument()
                    {
                        idType = "ADDRESS_PROOF",
                        idName = createIFFCOCKYCCommand.ProofOfAddress,
                        idNumber = createIFFCOCKYCCommand.poaDocumentId,
                        fileName = createIFFCOCKYCCommand.POADocumentName,
                        fileExtension = createIFFCOCKYCCommand.poaDocumentUploadExtension,
                        fileBase64 = createIFFCOCKYCCommand.POADocumentUpload
                    },
                    new Kycdocument()
                    {
                        idType = "OTHERS",
                        idName = "Photograph",
                        fileName = createIFFCOCKYCCommand.PhotographName,
                        fileExtension = createIFFCOCKYCCommand.PhotographUploadExtension,
                        fileBase64 = createIFFCOCKYCCommand.PhotographUpload
                    }
                };
            }
            else
            {
                iFFCOCreateCKYCRequestModel.clientType = "LE";
                iFFCOCreateCKYCRequestModel.kycDocuments = new Kycdocument[]
                {
                    new Kycdocument()
                    {
                        idType = "IDENTITY_PROOF",
                        idName = createIFFCOCKYCCommand.ProofOfIdentity,
                        idNumber = createIFFCOCKYCCommand.POIDocumentId,
                        fileName = createIFFCOCKYCCommand.POIDocumentName,
                        fileExtension = createIFFCOCKYCCommand.poiDocumentUploadExtension,
                        fileBase64 = createIFFCOCKYCCommand.POIDocumentUpload
                    },
                    new Kycdocument()
                    {
                        idType = "ADDRESS_PROOF",
                        idName = createIFFCOCKYCCommand.ProofOfAddress,
                        idNumber = createIFFCOCKYCCommand.poaDocumentId,
                        fileName = createIFFCOCKYCCommand.POADocumentName,
                        fileExtension = createIFFCOCKYCCommand.poaDocumentUploadExtension,
                        fileBase64 = createIFFCOCKYCCommand.POADocumentUpload
                    },
                };
            }

            requestBody = JsonConvert.SerializeObject(iFFCOCreateCKYCRequestModel);
            _logger.LogInformation("Create IFFCO CKYCResponse requestBody {requestBody}", requestBody);
            id = await InsertICLogs(requestBody, createIFFCOCKYCCommand.LeadId, _iFFCOConfig.BaseURL + _iFFCOConfig.CreateCKYC, string.Empty, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "KYC");
            try
            {
                var response = await _client.PostAsync(_iFFCOConfig.CreateCKYC, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    responseBody = response.ReasonPhrase;
                    await UpdateICLogs(id, string.Empty, responseBody);
                    _logger.LogError("GetCKYCResponse error {responseBody}", responseBody);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var resultModel = stream.DeserializeFromJson<IFFCOCreateCKYCResponseModel>();
                    responseBody = JsonConvert.SerializeObject(resultModel);
                    _logger.LogInformation("Create IFFCO CKYCResponse responseBody {responseBody}", responseBody);

                    if (resultModel.result.status.Equals("EXISTING RECORD") || resultModel.result.status.Equals("SUCCESS") && !string.IsNullOrEmpty(resultModel.result.itgiUniqueReferenceId) && resultModel.result.itgiUniqueReferenceId != "")
                    {
                        string aadharNumber = null;

                        if (createIFFCOCKYCCommand.ProofOfIdentity.Equals("AADHAR CARD NUMBER"))
                        {
                            aadharNumber = createIFFCOCKYCCommand.POIDocumentId;
                        }
                        else if (createIFFCOCKYCCommand.ProofOfAddress.Equals("AADHAR CARD NUMBER"))
                        {
                            aadharNumber = createIFFCOCKYCCommand.poaDocumentId;
                        }
                        uploadCKYCDocument.CKYCStatus = POA_SUCCESS;
                        uploadCKYCDocument.Message = POA_SUCCESS;
                        createLeadModel.kyc_id = resultModel.result.itgiUniqueReferenceId;
                        createLeadModel.LeadName = createIFFCOCKYCCommand.FirstName;
                        createLeadModel.LastName = !createIFFCOCKYCCommand.CustomerType.Equals("COMPANY") ? createIFFCOCKYCCommand.LastName : null;
                        createLeadModel.PhoneNumber = createIFFCOCKYCCommand.Mobile;
                        createLeadModel.Email = createIFFCOCKYCCommand.EmailId;
                        createLeadModel.DOB = createIFFCOCKYCCommand.DateOfBirth;
                        createLeadModel.Gender = createIFFCOCKYCCommand.Gender;
                        createLeadModel.PANNumber = createIFFCOCKYCCommand.ProofOfIdentity.Equals("PAN") ? createIFFCOCKYCCommand.POIDocumentId : null;
                        createLeadModel.AadharNumber = aadharNumber;
                        createLeadModel.Email = createIFFCOCKYCCommand.EmailId;
                        createLeadModel.PermanentAddress.AddressType = "PRIMARY";
                        createLeadModel.PermanentAddress.Address1 = createIFFCOCKYCCommand.Address;
                        createLeadModel.PermanentAddress.Address2 = createIFFCOCKYCCommand.City;
                        createLeadModel.PermanentAddress.Address3 = createIFFCOCKYCCommand.State;
                        createLeadModel.PermanentAddress.Pincode = createIFFCOCKYCCommand.Pincode;
                        createLeadModel.Stage = "POA";
                        createLeadModel.CKYCstatus = "approved";
                        createLeadModel.City = createIFFCOCKYCCommand.City;
                        createLeadModel.State = createIFFCOCKYCCommand.State;

                        createLeadModel.CompanyName = createIFFCOCKYCCommand.CustomerType.Equals("COMPANY") ? createIFFCOCKYCCommand.FirstName : null;
                        createLeadModel.DateOfIncorporation = createIFFCOCKYCCommand.CustomerType.Equals("COMPANY") ? createIFFCOCKYCCommand.DateOfInsertion : null;

                        uploadCKYCDocument.InsurerId = _iFFCOConfig.InsurerId;
                        uploadCKYCDocument.InsurerName = _iFFCOConfig.InsurerName;
                        uploadCKYCDocument.Name = createIFFCOCKYCCommand.FirstName;
                        uploadCKYCDocument.DOB = createIFFCOCKYCCommand.CustomerType.Equals("COMPANY") ? createIFFCOCKYCCommand.DateOfInsertion : createIFFCOCKYCCommand.DateOfBirth;
                        uploadCKYCDocument.LastName = !createIFFCOCKYCCommand.CustomerType.Equals("COMPANY") ? createIFFCOCKYCCommand.LastName : null;
                        uploadCKYCDocument.Gender = createIFFCOCKYCCommand.Gender;
                        uploadCKYCDocument.Address = createIFFCOCKYCCommand.Address + "," + createIFFCOCKYCCommand.City + "," + createIFFCOCKYCCommand.State + "," + createIFFCOCKYCCommand.Pincode;

                        await UpdateICLogs(id, resultModel.result.itgiUniqueReferenceId, responseBody);
                        return Tuple.Create(requestBody, responseBody, uploadCKYCDocument, createLeadModel);
                    }
                    else
                    {
                        uploadCKYCDocument.CKYCStatus = FAILED;
                        uploadCKYCDocument.Message = resultModel.result.status;
                        await UpdateICLogs(id, string.Empty, responseBody);
                        return Tuple.Create(requestBody, responseBody, uploadCKYCDocument, createLeadModel);
                    }
                }
                uploadCKYCDocument.CKYCStatus = FAILED;
                uploadCKYCDocument.Message = MESSAGE;
                await UpdateICLogs(id, string.Empty, responseBody);
                return Tuple.Create(requestBody, responseBody, uploadCKYCDocument, createLeadModel);
            }
            catch (Exception ex)
            {
                uploadCKYCDocument.CKYCStatus = FAILED;
                uploadCKYCDocument.Message = MESSAGE;
                _logger.LogError("IFFCO UploadCKYCDocument Error {exception}", ex.Message);
                await UpdateICLogs(id, string.Empty, ex.Message);
                return Tuple.Create(requestBody, responseBody, uploadCKYCDocument, createLeadModel);
            }
        }
        catch (Exception ex)
        {
            uploadCKYCDocument.CKYCStatus = FAILED;
            uploadCKYCDocument.Message = MESSAGE;
            _logger.LogError("IFFCO UploadCKYCDocument Error {exception}", ex.Message);
            return Tuple.Create(requestBody, responseBody, uploadCKYCDocument, createLeadModel);
        }
    }
    public async Task<Tuple<QuoteResponseModel, string>> GetProposal(IFFCOEnvelope quoteRequest, IFFCOEnvelope quoteResponse,
        CreateLeadModel createLeadModel, IFFCOProposalDynamicDetails proposalDynamicDetails, QuoteResponseModel commonResponse, IFFCOPreviousPolicyDetailsModel previousPolicyolicyDetails, CancellationToken cancellationToken)
    {
        var proposalVm = new QuoteResponseModel();
        var requestBody = string.Empty;
        var id = 0;
        var policyStartDate = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.InceptionDate;
        var policyEndDate = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.ExpiryDate;

        try
        {
            Random random = new Random();
            var uniqId = _iFFCOConfig.UniqIdConfig + DateTime.Now.ToString("yyyyMMddHHmmss"); 
            var tax = new ServiceTax();
            var findQuoteResponse = (dynamic)null;
            var coverageResponse = (dynamic)null;
            var vehicleNumber = VehicleNumberSplit(createLeadModel.VehicleNumber).ToList();
            bool isCorporate = !createLeadModel.CarOwnedBy.Equals("INDIVIDUAL");
            string isBreakInofMorethan90days = string.Empty;
            var previousPolicyExpiryDate = string.Empty;
            bool isCommercial = createLeadModel.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical);

            if (createLeadModel.IsBreakin) //isCommercial - removed from and condition
            {
                if (previousPolicyolicyDetails.PolictTypeId.Equals(_policyTypeConfig.PackageComprehensive))
                {
                    previousPolicyExpiryDate = previousPolicyolicyDetails.TPEndDate;
                }
                else if (previousPolicyolicyDetails.PolictTypeId.Equals(_policyTypeConfig.SAOD))
                {
                    previousPolicyExpiryDate = previousPolicyolicyDetails.ODEndDate;
                }
            }

            if (createLeadModel.IsBrandNew && !isCommercial)
            {
                if (createLeadModel.ResponseReferanceFlag.Equals("0"))
                {
                    findQuoteResponse = quoteResponse?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0];
                    tax.totalTax = RoundOffValue(quoteResponse?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.gstAmount);
                    coverageResponse = quoteResponse?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0].inscoverageResponse.coverageResponse.coverageResponse;
                }
                else
                {
                    findQuoteResponse = quoteResponse?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1];
                    tax.totalTax = RoundOffValue(quoteResponse?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1]?.gstAmount);
                    coverageResponse = quoteResponse?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1].inscoverageResponse.coverageResponse.coverageResponse;
                }
            }
            else
            {
                if (createLeadModel.ResponseReferanceFlag.Equals("0"))
                {
                    findQuoteResponse = quoteResponse?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0];
                    tax.totalTax = RoundOffValue(quoteResponse?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0]?.serviceTax);
                    coverageResponse = quoteResponse?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail;
                }
                else
                {
                    findQuoteResponse = quoteResponse?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1];
                    tax.totalTax = RoundOffValue(quoteResponse?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1]?.serviceTax);
                    coverageResponse = quoteResponse?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1].coveragePremiumDetail;
                }
            }
            if (createLeadModel.IsBreakin && createLeadModel.IsBreakinApproved)
            {
                var startDateTime = DateTime.Now.AddDays(3).ToString("dd/MMM/yyyy");
                var endDateTime = Convert.ToDateTime(startDateTime).AddYears(1).AddDays(-1).ToString("dd/MMM/yyyy");

                int breakinDays = (DateTime.Now.Date - Convert.ToDateTime(previousPolicyExpiryDate).Date).Days;
                isBreakInofMorethan90days = breakinDays > 90 ? "Y" : "N";
                policyStartDate = Convert.ToDateTime(startDateTime).ToString("MM/dd/yyyy HH:mm:ss").Replace("-", "/");
                policyEndDate = Convert.ToDateTime(endDateTime).ToString("MM/dd/yyyy HH:mm:ss").Replace("-", "/");
            }

            var totalSumInsured = GetTotalSumInsured(quoteRequest);

            var address = await AddressSplit(proposalDynamicDetails.AddressDetails.street);

            var proposalRequest = new IFFCOProposalRequest()
            {
                Policy = new IFFCOProposalPolicy()
                {
                    Product = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.ContractType,
                    CreatedDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss").Replace("-", "/"),
                    InceptionDate = policyStartDate,
                    UniqueQuoteId = uniqId,
                    CorporateClient = isCorporate ? "Y" : "N",
                    ExpiryDate = policyEndDate,
                    PreviousPolicyEnddate = createLeadModel.IsBrandNew ? string.Empty : Convert.ToDateTime(previousPolicyolicyDetails.ODEndDate).ToString("MM/dd/yyyy").Replace("-", "/"),
                    PreviousPolicyStartdate = createLeadModel.IsBrandNew ? string.Empty : Convert.ToDateTime(previousPolicyolicyDetails.ODStartDate).ToString("MM/dd/yyyy").Replace("-", "/"),
                    PreviousPolicyInsurer = createLeadModel.IsBrandNew ? string.Empty : previousPolicyolicyDetails.ODPreviousInsurer,
                    PreviousPolicyNo = createLeadModel.IsBrandNew ? string.Empty : previousPolicyolicyDetails.ODPreviouspolicyNumber,
                    BreakInofMorethan90days = isBreakInofMorethan90days,
                    OdDiscountLoading = findQuoteResponse?.discountLoading,
                    OdDiscountAmt = findQuoteResponse?.discountLoadingAmt,
                    OdSumDisLoad = findQuoteResponse?.totalODPremium,
                    TpSumDisLoad = findQuoteResponse?.totalTPPremium,
                    GrossPremium = findQuoteResponse?.totalPremimAfterDiscLoad,
                    IffcoServiceTax = tax.totalTax,
                    NetPremiumPayable = findQuoteResponse.premiumPayable,
                    TotalSumInsured = totalSumInsured.ToString(),
                    ExternalBranch = isCommercial ? _iFFCOConfig.CVPartnerBranch : _iFFCOConfig.PartnerBranch,
                    ExternalSubBranch = isCommercial ? _iFFCOConfig.CVPartnerSubBranch : _iFFCOConfig.PartnerSubBranch,
                    ExternalServiceConsumer = isCommercial ? _iFFCOConfig.CVPartnerCode : _iFFCOConfig.PartnerCode,
                    IffcoNominee = proposalDynamicDetails.NomineeDetails.nomineeName,
                    NomineeRelationship = proposalDynamicDetails.NomineeDetails.nomineeRelation,
                    PartnerType = _applicationClaims.GetRole().Equals("POSP") ? "POS" : string.Empty,
                    POSPanNumber = _applicationClaims.GetRole().Equals("POSP") ? _applicationClaims.GetPAN() : string.Empty,
                    GeneralPage = string.Empty
                },
                Vehicle = new IFFCOProposalVehicle()
                {
                    Capacity = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.Capacity,
                    EngineCapacity = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.EngineCpacity,
                    Make = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.Make,
                    RegistrationNumber1 = createLeadModel.IsBrandNew ? "NEW0000001" : vehicleNumber[0],
                    RegistrationNumber2 = createLeadModel.IsBrandNew ? string.Empty : vehicleNumber[1],
                    RegistrationNumber3 = createLeadModel.IsBrandNew ? string.Empty : vehicleNumber[2],
                    RegistrationNumber4 = createLeadModel.IsBrandNew ? string.Empty : vehicleNumber[3],
                    PolicyType = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.Type,
                    ManufacturingYear = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.YearOfManufacture,
                    Zone = previousPolicyolicyDetails.Zone,
                    EngineNumber = proposalDynamicDetails.VehicleDetails.engineNumber,
                    ChassisNumber = proposalDynamicDetails.VehicleDetails.chassisNumber,
                    SeatingCapacity = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.Capacity,
                    RegistrationDate = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.RegistrationDate,
                    RTOCity = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.RegictrationCity,
                    NewVehicleFlag = createLeadModel.IsBrandNew ? "Y" : "N",
                    ValidDrivingLicence = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Find(x => x.CoverageId.Equals("PA Owner / Driver")) != null ? "Y" : "N",
                    AlternatePACover = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Find(x => x.CoverageId.Equals("PA Owner / Driver")) != null ? "N" : "Y",
                    Zcover = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.Zcover,
                    GrossVehicleWeight = string.Empty,
                    RiskOccupationCode = string.Empty,
                    VehicleBody = string.Empty
                },
                VehicleThirdParty = new IFFCOProposalVehicleThirdParty()
                {
                    InterestedParty = createLeadModel.FinancierCode,
                    InterestedPartyName = proposalDynamicDetails.VehicleDetails.financer,
                    Relation = proposalDynamicDetails.VehicleDetails.isFinancier.Equals("Yes") ? "HY" : string.Empty
                },
            };
            if (!isCorporate)
            {
                proposalRequest.Contact = new IFFCOProposalContact()
                {
                    PassPort = string.Empty,
                    PAN = string.Empty,
                    SiebelContactNumber = string.Empty,
                    ItgiClientNumber = string.Empty,
                    DOB = Convert.ToDateTime(proposalDynamicDetails.PersonalDetails.dateOfBirth).ToString("MM/dd/yyyy").Replace("-", "/"),
                    ExternalClientNo = uniqId,
                    Salutation = proposalDynamicDetails.PersonalDetails.salutation,
                    FirstName = proposalDynamicDetails.PersonalDetails.firstName,
                    LastName = proposalDynamicDetails.PersonalDetails.lastName,
                    Sex = proposalDynamicDetails.PersonalDetails.gender.Equals("FEMALE") ? "F" : "M",
                    AddressType = "P",
                    PinCode = proposalDynamicDetails.AddressDetails.pincode,
                    AddressLine1 = address[0] != null ? address[0] : string.Empty,
                    AddressLine2 = address[1] != null ? address[1] : string.Empty,
                    AddressLine3 = address[2] != null ? address[2] : string.Empty,
                    AddressLine4 = address[3] != null ? address[3] : string.Empty,
                    FaxNo = string.Empty,
                    Source = string.Empty,
                    HomePhone = string.Empty,
                    OfficePhone = string.Empty,
                    Pager = string.Empty,
                    TaxId = string.Empty,
                    StafFlag = string.Empty,
                    City = proposalDynamicDetails.AddressDetails.city,
                    State = proposalDynamicDetails.AddressDetails.state,
                    Country = "IND",
                    CountryOrigin = "IND",
                    Occupation = proposalDynamicDetails.PersonalDetails.occupation,
                    Nationality = "IND",
                    MobilePhone = proposalDynamicDetails.PersonalDetails.mobile,
                    MailId = proposalDynamicDetails.PersonalDetails.emailId,
                    ItgiKYCReferenceNo = createLeadModel.kyc_id,
                    Married = proposalDynamicDetails.PersonalDetails.maritalStatus
                };
            }
            else
            {
                proposalRequest.Account = new IFFCOProposalAccount()
                {
                    DOB = string.Empty,
                    PAN = string.Empty,
                    AccountNumber = string.Empty,
                    ClientNumber = string.Empty,
                    TaxId = string.Empty,
                    ExternalAccountId = uniqId,
                    Name = proposalDynamicDetails.PersonalDetails.companyName,
                    PrimaryAccountStreetAddress = address[0] != null ? address[0] : string.Empty,
                    PrimaryAccountStreetAddress2 = address[1] != null ? address[1] : string.Empty,
                    PrimaryAccountStreetAddress3 = address[2] != null ? address[2] : string.Empty,
                    PrimaryAccountStreetAddress4 = address[3] != null ? address[3] : string.Empty,
                    MainFaxNumber = string.Empty,
                    EconomicActivity = string.Empty,
                    PaidCapital = string.Empty,
                    Source = string.Empty,
                    Licence = string.Empty,
                    MainPhoneNumber = proposalDynamicDetails.PersonalDetails.mobile,
                    MailId = proposalDynamicDetails.PersonalDetails.emailId,
                    PrimaryAccountPostalCode = proposalDynamicDetails.AddressDetails.pincode,
                    PrimaryAccountState = proposalDynamicDetails.AddressDetails.state,
                    PrimaryAccountCity = proposalDynamicDetails.AddressDetails.city,
                    PrimaryAccountCountry = "IND",
                    AccountGSTIN = proposalDynamicDetails.PersonalDetails.gstno
                };
            }

            if (!previousPolicyolicyDetails.PolictTypeId.Equals(_policyTypeConfig.PackageComprehensive))
            {
                proposalRequest.Policy.TpExpiryDate = createLeadModel.IsBrandNew ? string.Empty : Convert.ToDateTime(previousPolicyolicyDetails.TPEndDate).ToString("MM/dd/yyyy").Replace("-", "/");
                proposalRequest.Policy.TpInceptionDate = createLeadModel.IsBrandNew ? string.Empty : Convert.ToDateTime(previousPolicyolicyDetails.TPStartDate).ToString("MM/dd/yyyy").Replace("-", "/");
                proposalRequest.Policy.TpInsurerName = createLeadModel.IsBrandNew ? string.Empty : previousPolicyolicyDetails.TPPreviousInsurer;
                proposalRequest.Policy.TpPolicyNo = createLeadModel.IsBrandNew ? string.Empty : previousPolicyolicyDetails.TPPreviouspolicyNumber;
            }
            proposalRequest.Coverage = new List<IFFCOProposalCoverage>();

            if (isCommercial)
            {
                ProposalCoverMappingForCVI(quoteRequest, coverageResponse, proposalRequest);
            }
            else
            {
                ProposalCoverMapping(quoteRequest, createLeadModel, coverageResponse, proposalRequest);
            }

            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(IFFCOProposalRequest));
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                xmlSerializer.Serialize(writer, proposalRequest, emptyNamespaces);
                requestBody = stream.ToString();
            }

            _logger.LogInformation("Create IFFCO Proposal requestBody {requestBody}", requestBody);
            id = await InsertICLogs(requestBody, createLeadModel.LeadID, _iFFCOConfig.BaseURL + _iFFCOConfig.PaymentURL, string.Empty, string.Empty, "Proposal");

            await UpdateICLogs(id, uniqId, requestBody);

            proposalVm = new QuoteResponseModel
            {
                InsurerName = _iFFCOConfig.InsurerName,
                InsurerStatusCode = (int)HttpStatusCode.OK,
                IDV = commonResponse.IDV,
                MinIDV = commonResponse.MinIDV,
                MaxIDV = commonResponse.MaxIDV,
                Tax = tax,
                NCB = commonResponse.NCB,
                TotalPremium = RoundOffValue(findQuoteResponse.totalPremimAfterDiscLoad),
                GrossPremium = RoundOffValue(findQuoteResponse.premiumPayable),
                RTOCode = commonResponse.RTOCode,
                PolicyNumber = uniqId,
                ApplicationId = uniqId
            };
            return Tuple.Create(proposalVm, requestBody);
        }
        catch (Exception ex)
        {
            _logger.LogError("IFFCO Proposal {exception}", ex.Message);
            return default;
        }
    }
    public async Task<IFFCOPolicyDocumentResponse> GetPolicyDownloadURL(IFFCOPaymentResponseModel iFFCOPaymentResponseModel, CancellationToken cancellationToken)
    {
        var requestBody = string.Empty;
        var responseBody = string.Empty;
        HttpResponseMessage policyDownloadResponse;
        string applicationId = string.Empty;
        var id = 0;
        try
        {
            var policyDownloadRequest = new IFFCOPolicyDocumentRequest()
            {
                uniqueReferenceNo = iFFCOPaymentResponseModel.ProposalNumber,
                contractType = iFFCOPaymentResponseModel.Product,
                policyDownloadNo = iFFCOPaymentResponseModel.PolicyNumber,
                partnerDetail = new Partnerdetail()
                {
                    partnerCode = iFFCOPaymentResponseModel.Product.Equals("CVI") ? _iFFCOConfig.CVPartnerCode : _iFFCOConfig.PartnerCode
                }
            };

            requestBody = JsonConvert.SerializeObject(policyDownloadRequest);
            var authenticationString = $"{_iFFCOConfig.BasicAuth.UserName}:{_iFFCOConfig.BasicAuth.Password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

            id = await InsertICLogs(requestBody, iFFCOPaymentResponseModel.LeadId, _iFFCOConfig.BaseURL + _iFFCOConfig.PolicyDownload, string.Empty, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "Payment");
            try
            {
                policyDownloadResponse = await _client.PostAsync(_iFFCOConfig.PolicyDownload, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken);
                if (!policyDownloadResponse.IsSuccessStatusCode)
                {
                    responseBody = policyDownloadResponse.ReasonPhrase;
                    _logger.LogError("IFFCO PolicyDownload error {responseBody}", responseBody);
                }
                else
                {
                    var stream = await policyDownloadResponse.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<IFFCOPolicyDocumentResponse>();
                    responseBody = JsonConvert.SerializeObject(result);
                    applicationId = result.uniqueReferenceNo;
                    _logger.LogInformation("IFFCO PolicyDownload responseBody {responseBody}", responseBody);
                    await UpdateICLogs(id, applicationId, responseBody);
                    return result;
                }
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("IFFCO PolicyDownload {exception}", ex.Message);
                await UpdateICLogs(id, applicationId, ex.Message);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("IFFCO PolicyDownload error {exception}", ex.Message);
            return default;
        }
    }
    public async Task<byte[]> PolicyDownload(string leadId, string url, CancellationToken cancellationToken)
    {
        var policyBaseString = (dynamic)null;
        var responseBody = (dynamic)null;
        var id = 0;
        try
        {
            id = await InsertICLogs(url, leadId, url, string.Empty, string.Empty, "Payment");
            try
            {
                responseBody = await _client.GetAsync(url);

                if (responseBody.IsSuccessStatusCode && responseBody != null)
                {
                    policyBaseString = await responseBody.Content.ReadAsByteArrayAsync(cancellationToken);
                    //_logger.LogInformation("IFFCO PolicyDownload responseBody {responseBody}", policyBaseString.ToString());
                    await UpdateICLogs(id, string.Empty, policyBaseString.ToString());
                    return policyBaseString;
                }
                else
                {
                    policyBaseString = await policyBaseString.Content.ReadAsStringAsync(cancellationToken);
                    await UpdateICLogs(id, string.Empty, policyBaseString.ToString());
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("IFFCO PolicyDownload error {exception}", ex.Message);
                await UpdateICLogs(id, string.Empty, ex.Message);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("IFFCO PolicyDownload error {exception}", ex.Message);
            return default;
        }
    }
    public async Task<Tuple<string, string, string, string>> GenerateBreakin(IFFCOProposalDynamicDetails iFFCOProposalDynamicDetails, CreateLeadModel createLeadModel, CancellationToken cancellationToken)
    {
        var requestBody = string.Empty;
        var responseBody = string.Empty;
        var id = 0;
        var breakinId = string.Empty;
        try
        {
            Random random = new Random();
            var uniqId = _iFFCOConfig.UniqIdConfig + random.Next(10000, 99999).ToString();
            var name = createLeadModel.CarOwnedBy.Equals("INDIVIDUAL") ? iFFCOProposalDynamicDetails.PersonalDetails.firstName + " " + iFFCOProposalDynamicDetails.PersonalDetails.lastName : iFFCOProposalDynamicDetails.PersonalDetails.companyName;

            _client.DefaultRequestHeaders.Add("X-WIM-TOKEN", _iFFCOConfig.BreakinToken);

            var breakinRequest = new IFFCOBreakinRequestModel()
            {
                CustomerName = name,
                CustomerPhoneNumber = iFFCOProposalDynamicDetails.PersonalDetails.mobile,
                VehicleNumber = createLeadModel.VehicleNumber,
                VehicleType = _iFFCOConfig.BreakinVehicleType,
                QuoteNumber = uniqId,
                PurposeOfInspection = _iFFCOConfig.InspectionPurpose,
                InspectionNumber = uniqId,
                ChassisNumber = iFFCOProposalDynamicDetails.VehicleDetails.chassisNumber,
                EngineNumber = iFFCOProposalDynamicDetails.VehicleDetails.engineNumber,
                PaidBy = _iFFCOConfig.BreakinPaidBy
            };

            requestBody = JsonConvert.SerializeObject(breakinRequest);
            id = await InsertICLogs(requestBody, createLeadModel.LeadID, _iFFCOConfig.BreakinURL, _iFFCOConfig.BreakinToken, string.Empty, "BreakIn");
            try
            {
                var response = await _client.PostAsync(_iFFCOConfig.BreakinURL, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    responseBody = response.ReasonPhrase;
                    _logger.LogError("IFFCO Breakin {response}", requestBody);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<IFFCOBreakinResponseModel>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("IFFCO PolicyDownload responseBody {responseBody}", responseBody);
                    await UpdateICLogs(id, breakinId, responseBody);
                    if (result.Status.Equals("SCHEDULED"))
                    {
                        breakinId = result.ID.ToString();
                        return Tuple.Create(uniqId, breakinId, requestBody, responseBody);
                    }
                }
                return Tuple.Create(uniqId, breakinId, requestBody, responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("IFFCO Breakin {exception}", ex.Message);
                await UpdateICLogs(id, breakinId, responseBody);
                return Tuple.Create(uniqId, breakinId, requestBody, responseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("IFFCO Breakin {exception}", ex.Message);
            return default;
        }
    }
    public async Task<IFFCOBreakinStatusResponseModel> GetBreakinStatus(string leadId, string breakinId, CancellationToken cancellationToken)
    {
        var requestBody = string.Empty;
        var responseBody = string.Empty;
        var id = 0;
        string url = string.Empty;
        try
        {
            url = _iFFCOConfig.BreakinStatusURL + breakinId;
            _client.DefaultRequestHeaders.Add("X-WIM-TOKEN", _iFFCOConfig.BreakinToken);
            id = await InsertICLogs(breakinId, leadId, url, _iFFCOConfig.BreakinToken, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "BreakIn");
            try
            {
                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    responseBody = response.ReasonPhrase;
                    _logger.LogError("IFFCO Breakin Status Check API {response}", requestBody);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<IFFCOBreakinStatusResponseModel>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("IFFCO Breakin Status Check API responseBody {responseBody}", responseBody);
                    await UpdateICLogs(id, breakinId, responseBody);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("IFFCO Breakin Status API {exception}", ex.Message);
                await UpdateICLogs(id, breakinId, responseBody);
            }
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError("IFFCO Breakin {exception}", ex.Message);
            return default;
        }
    }

    private void ProposalCoverMapping(IFFCOEnvelope quoteRequest, CreateLeadModel createLeadModel, dynamic coverageResponse, IFFCOProposalRequest proposalRequest)
    {
        foreach (var item in coverageResponse)
        {
            if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("IDV Basic") : item?.coverageName.Equals("IDV Basic"))
            {
                proposalRequest.Coverage.Add
                (
                    new IFFCOProposalCoverage
                    {
                        Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                        SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("IDV Basic", StringComparison.Ordinal)).SumInsured,
                        ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                        TPPremium = createLeadModel.IsBrandNew ? TotalAndRoundOffValue(item) : item.tpPremium,
                    }
                );
            }

            //Accessories
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("Cost of Accessories") : item?.coverageName.Equals("Cost of Accessories"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("Cost of Accessories", StringComparison.Ordinal)).SumInsured,
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? item.TP1 : item.tpPremium,
                });
            }
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("Electrical Accessories") : item?.coverageName.Equals("Electrical Accessories"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("Electrical Accessories", StringComparison.Ordinal)).SumInsured,
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? item.TP1 : item.tpPremium,
                });
            }
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("CNG Kit") : item?.coverageName.Equals("CNG Kit"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("CNG Kit", StringComparison.Ordinal)).SumInsured,
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? TotalAndRoundOffValue(item) : item.tpPremium,
                });
            }

            //Discounts
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("TPPD") : item?.coverageName.Equals("TPPD"))
            {
                var tpPremiumValue = createLeadModel.IsBrandNew ? item.TP1 : item.tpPremium;
                if (tpPremiumValue == "-0.00")
                {
                    tpPremiumValue = "0";
                }
                if (!tpPremiumValue.Equals("0"))
                {
                    proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                    {
                        Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                        SumInsured = createLeadModel.VehicleTypeId.Equals("2d566966-5525-4ed7-bd90-bb39e8418f39") ? "750000" : "6000",
                        ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                        TPPremium = createLeadModel.IsBrandNew ? TotalAndRoundOffValue(item) : item.tpPremium,
                    });
                }
            }
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("No Claim Bonus") : item?.coverageName.Equals("No Claim Bonus"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("No Claim Bonus", StringComparison.Ordinal)).SumInsured,
                    ODPremium = item.odPremium,
                    TPPremium = item.tpPremium,
                });
            }
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("Voluntary Excess") : item?.coverageName.Equals("Voluntary Excess"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("Voluntary Excess", StringComparison.Ordinal)).SumInsured,
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? item.TP1 : item.tpPremium,
                });
            }
            //else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("AAI Discount") : item?.coverageName.Equals("AAI Discount"))
            //{
            //    proposalRequest.Coverage.Add(new IFFCOProposalCoverage
            //    {
            //        Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
            //        SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("AAI Discount", StringComparison.Ordinal)).SumInsured,
            //        ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
            //        TPPremium = createLeadModel.IsBrandNew ? item.TP1 : item.tpPremium,
            //    });
            //}
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("Anti-Theft") : item?.coverageName.Equals("Anti-Theft"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("Anti-Theft", StringComparison.Ordinal)).SumInsured,
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? item.TP1 : item.tpPremium,
                });
            }

            //PA Cover
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("PA Owner / Driver") : item?.coverageName.Equals("PA Owner / Driver"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    Number = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("PA Owner / Driver", StringComparison.Ordinal)).Number,
                    SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("PA Owner / Driver", StringComparison.Ordinal)).SumInsured,
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? TotalAndRoundOffValue(item) : item.tpPremium,
                });
            }
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("Legal Liability to Driver") : item?.coverageName.Equals("Legal Liability to Driver"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("Legal Liability to Driver", StringComparison.Ordinal)).SumInsured,
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? TotalAndRoundOffValue(item) : item.tpPremium,
                });
            }
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("PA to Passenger") : item?.coverageName.Equals("PA to Passenger"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("PA to Passenger", StringComparison.Ordinal)).SumInsured,
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? TotalAndRoundOffValue(item) : item.tpPremium,
                });
            }

            //Addons
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("Depreciation Waiver") : item?.coverageName.Equals("Depreciation Waiver"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = "Y",
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? item.TP1 : item.tpPremium,
                });
            }
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("Consumable") : item?.coverageName.Equals("Consumable"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = "Y",
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? item.TP1 : item.tpPremium,
                });
            }
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("NCB Protection") : item?.coverageName.Equals("NCB Protection"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = "Y",
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? item.TP1 : item.tpPremium,
                });
            }
            else if (createLeadModel.IsBrandNew ? item?.coverageCode.Equals("Towing & Related") : item?.coverageName.Equals("Towing & Related"))
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = createLeadModel.IsBrandNew ? item.coverageCode : item.coverageName,
                    SumInsured = "Y",
                    ODPremium = createLeadModel.IsBrandNew ? item.OD1 : item.odPremium,
                    TPPremium = createLeadModel.IsBrandNew ? item.TP1 : item.tpPremium,
                });
            }
        }
    }
    private void ProposalCoverMappingForCVI(IFFCOEnvelope quoteRequest, dynamic coverageResponse, IFFCOProposalRequest proposalRequest)
    {
        bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.RegistrationDate).ToString("yyyy"), 5);
        var optedCovers = quoteRequest?.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage;
        foreach (var item in coverageResponse)
        {
            if (item?.coverageName.Equals("IDV Basic"))
            {
                proposalRequest.Coverage.Add
                (
                    new IFFCOProposalCoverage
                    {
                        Code = item.coverageName,
                        SumInsured = quoteRequest?.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("IDV Basic", StringComparison.Ordinal)).SumInsured,
                        ODPremium = item.odPremium,
                        TPPremium = item.tpPremium,
                    }
                );
            }
            else if (item?.coverageName.Equals("No Claim Bonus"))
            {
                if (optedCovers?.item.Find(x => x.CoverageId.Equals("No Claim Bonus", StringComparison.Ordinal)) != null)
                {
                    proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                    {
                        Code = item.coverageName,
                        SumInsured = quoteRequest?.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("No Claim Bonus", StringComparison.Ordinal)).SumInsured,
                        ODPremium = item.odPremium,
                        TPPremium = item.tpPremium,
                    });
                }
            }
            else if (item?.coverageName.Equals("Electrical Accessories") && optedCovers?.item.Find(x => x.CoverageId.Equals("Electrical Accessories", StringComparison.Ordinal)) != null)
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = item.coverageName,
                    SumInsured = quoteRequest?.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("Electrical Accessories", StringComparison.Ordinal)).SumInsured,
                    ODPremium = item.odPremium,
                    TPPremium = item.tpPremium,
                });
            }
            else if (item?.coverageName.Equals("CNG Kit") && optedCovers?.item.Find(x => x.CoverageId.Equals("CNG Kit", StringComparison.Ordinal)) != null)
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = item.coverageName,
                    SumInsured = quoteRequest?.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("CNG Kit", StringComparison.Ordinal)).SumInsured,
                    ODPremium = item.odPremium,
                    TPPremium = item.tpPremium,
                });
            }
            //Discounts
            else if (item?.coverageName.Equals("TPPD") && optedCovers?.item.Find(x => x.CoverageId.Equals("TPPD", StringComparison.Ordinal)) != null)
            {
                var tpPremiumValue = item.tpPremium;
                if (tpPremiumValue == "-0.00")
                {
                    tpPremiumValue = "0";
                }
                if (!tpPremiumValue.Equals("0"))
                {
                    proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                    {
                        Code = item.coverageName,
                        SumInsured = "750000",
                        ODPremium = item.odPremium,
                        TPPremium = item.tpPremium,
                    });
                }
            }
            //PA Cover
            else if (item?.coverageName.Equals("PA Owner / Driver") && optedCovers?.item.Find(x => x.CoverageId.Equals("PA Owner / Driver", StringComparison.Ordinal)) != null)
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = item.coverageName,
                    SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("PA Owner / Driver", StringComparison.Ordinal))?.SumInsured,
                    ODPremium = item.odPremium,
                    TPPremium = item.tpPremium,
                });
            }
            else if (item?.coverageName.Equals("PA to Passenger") && optedCovers?.item.Find(x => x.CoverageId.Equals("PA to Passenger", StringComparison.Ordinal)) != null)
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = item.coverageName,
                    SumInsured = quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("PA to Passenger", StringComparison.Ordinal))?.SumInsured,
                    ODPremium = item.odPremium,
                    TPPremium = item.tpPremium,
                });
            }
            //Addons
            else if (item?.coverageName.Equals("Depreciation Waiver") && isVehicleAgeLessThan5Years && optedCovers?.item.Find(x => x.CoverageId.Equals("Depreciation Waiver", StringComparison.Ordinal)) != null)
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = item.coverageName, 
                    SumInsured = "Y",
                    ODPremium = item.coveragePremium,
                    TPPremium = item.tpPremium,
                });
            }
            else if (item?.coverageName.Equals("Consumable") && isVehicleAgeLessThan5Years && optedCovers?.item.Find(x => x.CoverageId.Equals("Consumable", StringComparison.Ordinal)) != null)
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = item.coverageName,
                    SumInsured = "Y",
                    ODPremium = item.coveragePremium,
                    TPPremium = item.tpPremium,
                });
            }
            else if (item?.coverageName.Equals("IMT 23") && isVehicleAgeLessThan5Years && optedCovers?.item.Find(x => x.CoverageId.Equals("IMT 23", StringComparison.Ordinal)) != null)
            {
                proposalRequest.Coverage.Add(new IFFCOProposalCoverage
                {
                    Code = item.coverageName,
                    SumInsured = "Y",
                    ODPremium = item.odPremium,
                    TPPremium = item.tpPremium,
                });
            }
        }
    }
    private static int GetTotalSumInsured(IFFCOEnvelope quoteRequest)
    {
        var totalSumInsured = 0;
        if (quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Exists(x => x.CoverageId.Equals("Cost of Accessories")))
        {
            totalSumInsured += Convert.ToInt32(quoteRequest?.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("Cost of Accessories", StringComparison.Ordinal))?.SumInsured);
        }

        if (quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Exists(x => x.CoverageId.Equals("CNG Kit")))
        {
            totalSumInsured += Convert.ToInt32(quoteRequest?.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("CNG Kit", StringComparison.Ordinal))?.SumInsured);
        }

        if (quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Exists(x => x.CoverageId.Equals("Electrical Accessories")))
        {
            totalSumInsured += Convert.ToInt32(quoteRequest?.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("Electrical Accessories", StringComparison.Ordinal))?.SumInsured);
        }

        if (quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Exists(x => x.CoverageId.Equals("IDV Basic")))
        {
            totalSumInsured += Convert.ToInt32(quoteRequest?.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item?.Find(x => x.CoverageId.Equals("IDV Basic", StringComparison.Ordinal))?.SumInsured);
        }
        return totalSumInsured;
    }
    private async Task<Tuple<HttpResponseMessage, int, string>> GetQuoteResponse(bool isBrandNewVehicle, string planType, string leadId, string requestBody, string stage, bool isCommercial, CancellationToken cancellationToken)
    {
        HttpResponseMessage quoteResponse;

        string url = string.Empty;
        int id = 0;

        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        if (isCommercial)
        {
            url = _iFFCOConfig.CVQuoteUrl;
            _client.DefaultRequestHeaders.Add("SOAPAction", "getMotorPremium");
            requestBody = requestBody.Replace("getNewVehiclePremium", "getMotorPremium");
        }
        else if (isBrandNewVehicle && !isCommercial)
        {
            url = _iFFCOConfig.QuoteURLForBrandNew;
            _client.DefaultRequestHeaders.Add("SOAPAction", "getNewVehiclePremium");
        }
        else if (planType.Equals(_iFFCOConfig.SAODPlanType) || planType.Equals(_iFFCOConfig.ComprehensivePlanType))
        {
            url = _iFFCOConfig.QuoteUrlForODAndComp;
            _client.DefaultRequestHeaders.Add("SOAPAction", "getMotorPremium");
            requestBody = requestBody.Replace("getNewVehiclePremium", "getMotorPremium");
        }
        else
        {
            url = _iFFCOConfig.QuoteUrlForTP;
            _client.DefaultRequestHeaders.Add("SOAPAction", "getMotorPremium");
            requestBody = requestBody.Replace("getNewVehiclePremium", "getMotorPremium");
        }
        _logger.LogInformation("IFFCO Quote RequestBody {RequestBody}", requestBody);
        id = await InsertICLogs(requestBody, leadId, _iFFCOConfig.BaseURL + url, string.Empty, _client.DefaultRequestHeaders.ToString(), stage);
        try
        {
            quoteResponse = await _client.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/xml"), cancellationToken);
            return Tuple.Create(quoteResponse, id, requestBody);
        }
        catch (Exception ex)
        {
            _logger.LogError("IFFCO GetQuoteResponse {exception}", ex.Message);
            await UpdateICLogs(id, string.Empty, ex.Message);
            return default;
        }
    }
    private void GetCoverMapping(QuoteQueryModel quoteQuery, IFFCOEnvelope quoteRequest)
    {
        bool isVehicleAgeLessThan3Years = IsYearGreaterThanValue(quoteQuery.RegistrationYear, 3);
        bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(quoteQuery.RegistrationYear, 5);
        //NCB Mapping
        if (quoteQuery.PreviousPolicyDetails.IsPreviousInsurerKnown && !quoteQuery.PreviousPolicyDetails.IsClaimInLastYear)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "No Claim Bonus",
                Number = string.Empty,
                SumInsured = quoteQuery.CurrentNCBPercentage
            });
        }
        //Accessories Mapping
        if (quoteQuery.Accessories.IsCNG)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "CNG Kit",
                Number = string.Empty,
                SumInsured = quoteQuery.Accessories.CNGValue.ToString()
            });
        }
        if (quoteQuery.Accessories.IsElectrical)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "Electrical Accessories",
                Number = string.Empty,
                SumInsured = quoteQuery.Accessories.ElectricalValue.ToString()
            });
        }
        if (quoteQuery.Accessories.IsNonElectrical && !quoteQuery.VehicleDetails.IsCommercialVehicle)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "Cost of Accessories",
                Number = string.Empty,
                SumInsured = quoteQuery.Accessories.NonElectricalValue.ToString()
            });
        }

        //Discount Mapping
        if (quoteQuery.Discounts.IsVoluntarilyDeductible && !quoteQuery.VehicleDetails.IsCommercialVehicle)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "Voluntary Excess",
                Number = string.Empty,
                SumInsured = quoteQuery.VoluntaryExcess
            });
        }
        ////Commented becuse of AAI Discount requires additional information like membership start date membership id
        //if (quoteQuery.Discounts.IsAAMemberShip)
        //{
        //    quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
        //    {
        //        CoverageId = "AAI Discount",
        //        Number = string.Empty,
        //        SumInsured = "Y"
        //    });
        //}
        if (quoteQuery.Discounts.IsAntiTheft && !quoteQuery.VehicleDetails.IsCommercialVehicle)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "Anti-Theft",
                Number = string.Empty,
                SumInsured = "Y"
            });
        }
        if (quoteQuery.Discounts.IsLimitedTPCoverage && !quoteQuery.VehicleDetails.IsCommercialVehicle) // tppd valus for comercial vehicle needed
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "TPPD",
                Number = string.Empty,
                SumInsured = quoteQuery.VehicleDetails.IsTwoWheeler ? "6000" : "750000"
            });
        }

        //PA Cover Mapping
        if (quoteQuery.PACover.IsUnnamedPassenger)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "PA to Passenger",
                Number = string.Empty,
                SumInsured = quoteQuery.UnnamedPassangerAndPillonRider
            });
        }
        if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "PA Owner / Driver",
                Number = string.Empty,
                SumInsured = "Y"
            });
        }
        if (quoteQuery.PACover.IsPaidDriver && quoteQuery.VehicleDetails.IsFourWheeler)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "Legal Liability to Driver",
                Number = string.Empty,
                SumInsured = "Y"
            });
        }

        //Addon Mapping
        if (quoteQuery.AddOns.IsNCBRequired && isVehicleAgeLessThan3Years && !quoteQuery.VehicleDetails.IsCommercialVehicle)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "NCB Protection",
                Number = string.Empty,
                SumInsured = "Y"
            });
        }
        if (quoteQuery.AddOns.IsZeroDebt && isVehicleAgeLessThan5Years)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "Depreciation Waiver",
                Number = string.Empty,
                SumInsured = "Y"
            });
        }
        if (quoteQuery.AddOns.IsConsumableRequired && isVehicleAgeLessThan5Years)
        {
            quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
            {
                CoverageId = "Consumable",
                Number = string.Empty,
                SumInsured = "Y"
            });
        }
        ////Commented because of premium mismatch issue showing in Payment page
        //if (quoteQuery.AddOns.IsTowingRequired)
        //{
        //    quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
        //    {
        //        CoverageId = "Towing & Related",
        //        Number = string.Empty,
        //        SumInsured = "Y"
        //    });
        //}
    }
    private static List<NameValueModel> SetBaseCover(string previousPolicy, bool isBrandNewVehicle, bool isComercial, IFFCOEnvelope response)
    {
        List<NameValueModel> baseCoverList = new List<NameValueModel>();
        if (previousPolicy.Equals("Package Comprehensive") || previousPolicy.Equals("Comprehensive Bundle"))
        {
            baseCoverList = new List<NameValueModel>
            {
                new NameValueModel
                {
                    Name = "Basic Own Damage Premium",
                    Value = isBrandNewVehicle && !isComercial ?
                        RoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("IDV Basic")).Select(y => y.OD1).FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("IDV Basic"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                    IsApplicable = isBrandNewVehicle && !isComercial ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("IDV Basic"))?.Select(y => y.OD1).FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("IDV Basic"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                },
                new NameValueModel
                {
                    Name = "Third Party Cover Premium",
                    Value = isBrandNewVehicle && !isComercial ?
                        TotalAndRoundOffValue(response?.Body?.GetNewVehiclePremiumResponse.GetNewVehiclePremiumReturn[0].inscoverageResponse.coverageResponse.coverageResponse?.Where(x => x.coverageCode == "IDV Basic").FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("IDV Basic"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
                    IsApplicable = isBrandNewVehicle && !isComercial ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("IDV Basic"))?.Select(x => x.TP1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("IDV Basic"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
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
                    Value = RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("IDV Basic"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                    IsApplicable = IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("IDV Basic"))?.Select(x => x.odPremium)?.FirstOrDefault()),
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
                    Value = RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("IDV Basic"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
                    IsApplicable = IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("IDV Basic"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
               },
            };
        }
        return baseCoverList;
    }
    private List<NameValueModel> SetPACoverResponse(QuoteQueryModel quoteQuery, IFFCOEnvelope response)
    {
        List<NameValueModel> paCoverList = new List<NameValueModel>();
        if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
        {
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                Name = "PA Cover for Owner Driver",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        TotalAndRoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("PA Owner / Driver"))?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("PA Owner / Driver"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("PA Owner / Driver"))?.Select(x => x.TP1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("PA Owner / Driver"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
            });
        }
        if (quoteQuery.PACover.IsPaidDriver)
        {
            if (quoteQuery.VehicleDetails.IsFourWheeler)
            {
                paCoverList.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.PaidDriverId,
                    Name = "PA Cover for Paid Driver",
                    Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                            TotalAndRoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Legal Liability to Driver"))?.FirstOrDefault()) :
                            RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("Legal Liability to Driver"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
                    IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                            IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Legal Liability to Driver"))?.Select(x => x.TP1)?.FirstOrDefault()) :
                            IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("Legal Liability to Driver"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
                });
            }
            else
            {
                paCoverList.Add(new NameValueModel
                {
                    Id = quoteQuery.PACover.PaidDriverId,
                    Name = "PA Cover for Paid Driver",
                    Value = null,
                    IsApplicable = false,
                });
            }
        }
        if (quoteQuery.PACover.IsUnnamedPassenger)
        {
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedPassengerId,
                Name = "PA Cover for Unnamed Passengers",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        TotalAndRoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("PA to Passenger"))?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("PA to Passenger"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("PA to Passenger"))?.Select(x => x.TP1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("PA to Passenger"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
            });
        }
        return paCoverList;
    }
    private static List<NameValueModel> SetAddOnsResponse(QuoteQueryModel quoteQuery, IFFCOEnvelope response, bool isVehicleAgeLessThan5Years)
    {
        List<NameValueModel> addOnsList = new List<NameValueModel>();
        if (quoteQuery.AddOns.IsZeroDebt && isVehicleAgeLessThan5Years)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ZeroDebtId,
                Name = "Zero Dep",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        RoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Depreciation Waiver"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1].coveragePremiumDetail.Where(x => x.coverageName.Equals("Depreciation Waiver"))?.Select(x => x.coveragePremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Depreciation Waiver"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1].coveragePremiumDetail.Where(x => x.coverageName.Equals("Depreciation Waiver"))?.Select(x => x.coveragePremium)?.FirstOrDefault()),
            }
            );
        }
        else if (quoteQuery.AddOns.IsZeroDebt)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ZeroDebtId,
                Name = "Zero Dep",
                Value = null,
                IsApplicable = false,
            }
            );
        }
        if (quoteQuery.AddOns.IsConsumableRequired && isVehicleAgeLessThan5Years)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ConsumableId,
                Name = "Consumables",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        RoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Consumable"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1].coveragePremiumDetail.Where(x => x.coverageName.Equals("Consumable"))?.Select(x => x.coveragePremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Consumable"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1].coveragePremiumDetail.Where(x => x.coverageName.Equals("Consumable"))?.Select(x => x.coveragePremium)?.FirstOrDefault()),
            }
            );
        }
        else if (quoteQuery.AddOns.IsConsumableRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ConsumableId,
                Name = "Consumables",
                Value = null,
                IsApplicable = false,
            }
            );
        }
        if (quoteQuery.AddOns.IsNCBRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.NCBId,
                Name = "No Claim Bonus Protection",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        RoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("NCB Protection"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1].coveragePremiumDetail.Where(x => x.coverageName.Equals("NCB Protection"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("NCB Protection"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1].coveragePremiumDetail.Where(x => x.coverageName.Equals("NCB Protection"))?.Select(x => x.odPremium)?.FirstOrDefault()),
            }
            );
        }
        if (quoteQuery.AddOns.IsTowingRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.TowingId,
                Name = "Towing Protection",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        RoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Towing & Related"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1].coveragePremiumDetail.Where(x => x.coverageName.Equals("Towing & Related"))?.Select(x => x.coveragePremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[1]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Towing & Related"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1].coveragePremiumDetail.Where(x => x.coverageName.Equals("Towing & Related"))?.Select(x => x.coveragePremium)?.FirstOrDefault()),
            });
        }
        if (quoteQuery.AddOns.IsIMT23 && quoteQuery.VehicleDetails.IsCommercialVehicle)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.IMT23Id,
                Name = "IMT 23",
                Value = RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1].coveragePremiumDetail.Where(x => x.coverageName.Equals("IMT 23"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                IsApplicable = IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[1].coveragePremiumDetail.Where(x => x.coverageName.Equals("IMT 23"))?.Select(x => x.odPremium)?.FirstOrDefault()),
            }
            );
        }
        if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                Name = "Key And Lock Protection",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsTyreProtectionRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.TyreProtectionId,
                Name = "Tyre Protection",
                Value = null,
                IsApplicable = false
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
        if (quoteQuery.AddOns.IsInvoiceCoverRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                Name = "RTI",
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
                Value = null,
                IsApplicable = false
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
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsEngineProtectionRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.EngineProtectionId,
                Name = "Engine Gearbox Protection",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsGeoAreaExtension)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.GeoAreaExtensionId,
                Name = "Geo Area Extension OD",
                Value = null,
                IsApplicable = false
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
    private static List<NameValueModel> SetDiscountResponse(QuoteQueryModel quoteQuery, IFFCOEnvelope response)
    {
        List<NameValueModel> discountList = new List<NameValueModel>();
        if (quoteQuery.Discounts.IsAntiTheft)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AntiTheftId,
                Name = "ARAI Approved Anti-Theft Device",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        RoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Anti-Theft"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("Anti-Theft"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Anti-Theft"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("Anti-Theft"))?.Select(x => x.odPremium)?.FirstOrDefault()),
            });
        }
        if (quoteQuery.Discounts.IsAAMemberShip)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AAMemberShipId,
                Name = "AA Membership",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        RoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("AAI Discount"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("AAI Discount"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("AAI Discount"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("AAI Discount"))?.Select(x => x.odPremium)?.FirstOrDefault()),
            });
        }
        if (quoteQuery.Discounts.IsVoluntarilyDeductible)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                Name = "Voluntary Deductible",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        RoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Voluntary Excess"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("Voluntary Excess"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Voluntary Excess"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("Voluntary Excess"))?.Select(x => x.odPremium)?.FirstOrDefault()),
            });
        }
        if (quoteQuery.Discounts.IsLimitedTPCoverage)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.LimitedTPCoverageId,
                Name = "Limited Third Party Coverage",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                         "-" + TotalAndRoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("TPPD"))?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("TPPD"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("TPPD"))?.Select(x => x.TP1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("TPPD"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
            });
        }
        if (quoteQuery.PreviousPolicyDetails.IsPreviousInsurerKnown && !quoteQuery.PreviousPolicyDetails.IsClaimInLastYear && !quoteQuery.CurrentPolicyType.Equals("SATP"))
        {
            discountList.Add(new NameValueModel
            {
                Name = $"No Claim Bonus ({quoteQuery.CurrentNCBPercentage}%)",
                Value = RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("No Claim Bonus"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                IsApplicable = IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("No Claim Bonus"))?.Select(x => x.odPremium)?.FirstOrDefault()),
            });
        }

        return discountList;
    }
    private List<NameValueModel> SetAccessoryResponse(QuoteQueryModel quoteQuery, IFFCOEnvelope response)
    {
        List<NameValueModel> accessoryList = new List<NameValueModel>();
        if (quoteQuery.Accessories.IsCNG)
        {
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover OD",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        RoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("CNG Kit"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("CNG Kit"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("CNG Kit"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("CNG Kit"))?.Select(x => x.odPremium)?.FirstOrDefault()),
            });
            if (!quoteQuery.CurrentPolicyType.Equals("SAOD", StringComparison.Ordinal))
            {
                accessoryList.Add(new NameValueModel
                {
                    Id = quoteQuery.Accessories.CNGId,
                    Name = "CNG/LPG Accessory Cover TP",
                    Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                            TotalAndRoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("CNG Kit"))?.FirstOrDefault()) :
                            RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("CNG Kit"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
                    IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                            IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("CNG Kit"))?.Select(x => x.TP1)?.FirstOrDefault()) :
                            IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("CNG Kit"))?.Select(x => x.tpPremium)?.FirstOrDefault()),
                });
            }
        }
        if (quoteQuery.Accessories.IsElectrical)
        {
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.ElectricalId,
                Name = "Electrical Accessory Cover",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        RoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Electrical Accessories"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("Electrical Accessories"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Electrical Accessories"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("Electrical Accessories"))?.Select(x => x.odPremium)?.FirstOrDefault()),
            });
        }
        if (quoteQuery.Accessories.IsNonElectrical)
        {
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.NonElectricalId,
                Name = "Non-Electrical Accessory Cover",
                Value = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        RoundOffValue(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Cost of Accessories"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        RoundOffValue(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("Cost of Accessories"))?.Select(x => x.odPremium)?.FirstOrDefault()),
                IsApplicable = quoteQuery.IsBrandNewVehicle && !quoteQuery.VehicleDetails.IsCommercialVehicle ?
                        IsApplicable(response?.Body?.GetNewVehiclePremiumResponse?.GetNewVehiclePremiumReturn[0]?.inscoverageResponse.coverageResponse.coverageResponse.Where(x => x.coverageCode.Equals("Cost of Accessories"))?.Select(x => x.OD1)?.FirstOrDefault()) :
                        IsApplicable(response?.Body?.GetMotorPremiumResponse?.GetMotorPremiumReturn[0].coveragePremiumDetail.Where(x => x.coverageName.Equals("Cost of Accessories"))?.Select(x => x.odPremium)?.FirstOrDefault()),
            });
        }
        return accessoryList;
    }
    private static string RoundOffValue(string value)
    {
        if (!string.IsNullOrEmpty(value) && value != "-")
        {
            decimal val = Math.Round(Convert.ToDecimal(value));
            return val.ToString();
        }
        return "0";
    }
    private static string TotalAndRoundOffValue(IIFCOCoverageResponse value)
    {
        var tp1 = value.TP1 != "" ? Math.Abs(Convert.ToDecimal(value.TP1)) : 0;
        var tp2 = value.TP2 != "" ? Math.Abs(Convert.ToDecimal(value.TP2)) : 0;
        var tp3 = value.TP3 != "" ? Math.Abs(Convert.ToDecimal(value.TP3)) : 0;
        var tp4 = value.TP4 != "" ? Math.Abs(Convert.ToDecimal(value.TP4)) : 0;
        var tp5 = value.TP5 != "" ? Math.Abs(Convert.ToDecimal(value.TP5)) : 0;

        decimal totalValue = tp1 + tp2 + tp3 + tp4 + tp5;

        decimal val = Math.Round(Convert.ToDecimal(totalValue));
        return val.ToString();
    }
    private static bool IsApplicable(object value)
    {
        string val = Convert.ToString(value);

        if (!string.IsNullOrEmpty(val) && val != "-")
        {
            return !(string.IsNullOrEmpty(val) || val == "0" || Convert.ToDecimal(val) == 0);
        }
        return false;

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
            InsurerId = _iFFCOConfig.InsurerId,
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
    private static bool IsYearGreaterThanValue(string registrationYear, int yearCheck)
    {
        int year = (Convert.ToInt32(DateTime.Now.Year) - Convert.ToInt32(registrationYear));
        return year <= yearCheck;
    }
    private static IEnumerable<string> VehicleNumberSplit(string input)
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
    private async Task<string[]> AddressSplit(string address)
    {
        _logger.LogInformation("Entering in To Address Split");
        string[] addressArray = new string[4];
        var addressSplit = (dynamic)null;
        if (address.Length <= 30)
        {
            _logger.LogInformation("Entering in To Address Split 1 IF");
            if (address.Contains(","))
            {
                addressSplit = address.Split(",");
                for(int i = 0; i < addressSplit.Length-1; i++)
                {
                    addressArray[0] = addressArray[0] + "," + addressSplit[i];
                }
            }
            else
            {
                addressSplit = address.Split(" ");
                for (int i = 0; i < addressSplit.Length - 1; i++)
                {
                    addressArray[0] = addressArray[0] + " " + addressSplit[i];
                }
            }
            addressArray[1] = addressSplit[addressSplit.Length - 1];
            return addressArray;
        }
        else
        {
            if (address.Contains(","))
            {
                addressSplit = address.Split(",");
            }
            else
            {
                addressSplit = address.Split(" ");
            }
            List<string> finalList = new();
            for (int x = 0; x <= addressSplit.Length - 1; x++)
            {
                _logger.LogInformation("Entering in To Address Split for loop for more than 30 charcater");
                string str = addressSplit[x];
                if (str.Length >= 29)
                {
                    var split = str.Select((c, index) => new { c, index })
                    .GroupBy(x => x.index / 29)
                    .Select(group => group.Select(elem => elem.c))
                    .Select(chars => new string(chars.ToArray()));
                    foreach(var item in split)
                    {
                        finalList.Add(item);
                    }
                    _logger.LogInformation("Entering in To Address Split finallist{addresslist}",finalList);
                }
                else
                {
                    finalList.Add(addressSplit[x]);
                }
            }
            addressSplit = finalList.ToArray();

            var length = addressSplit.Length;

            int i = 0;
            int j = 0;
            string temp = string.Empty;

            while (i < length)
            {
            Adding:
                string temp1 = temp;
                if (i < length)
                {
                    temp = temp + " " + addressSplit[i];
                }
                if (temp.Length <= 30 && i < length)
                {
                    i++;
                    goto Adding;
                }
                else if (temp.Length > 30 || i <= length)
                {
                    addressArray[j] = temp1;
                    j++;
                    temp = string.Empty;
                }
            }
            addressArray[0] = addressArray[0][1..];
        }
        return addressArray;
    }

    //Commercial Vehicle
    public async Task<IFFCOIDVResponseModel> GetCVIDV(QuoteQueryModel quoteQueryModel, CancellationToken cancellation)
    {

        IFFCOIDVResponseModel iFFCOIDVResponseModel = new IFFCOIDVResponseModel();
        var requestBody = string.Empty;
        var responseBody = string.Empty;
        var id = 0;
        try
        {
            _logger.LogInformation("Get CV IFFCO IDV");
            var idvrequest = new IFFCOEnvelope()
            {
                Body = new IFFCOBody()
                {
                    GetVehicleCVIIdv = new GetVehicleCVIIdv()
                    {
                        IdvWebServiceRequestCV = new IdvWebServiceRequestCV()
                        {
                            ContractType = quoteQueryModel.VehicleDetails.VehicleClass,
                            DateOfRegistration = Convert.ToDateTime(quoteQueryModel.RegistrationDate).ToString("MM/dd/yyyy").Replace("-", "/"),
                            InceptionDate = Convert.ToDateTime(quoteQueryModel.PolicyStartDate).ToString("MM/dd/yyyy HH:mm:ss").Replace("-", "/"),
                            MakeCode = quoteQueryModel.VehicleDetails.VehicleMakeCode,
                            Model = quoteQueryModel.VehicleDetails.VehicleModelCode,
                            RtoCity = quoteQueryModel.CityCode,
                            VehicleClass = quoteQueryModel.VehicleDetails.VehicleSubTypeCode.Equals("C1A") ? "C" : "A",
                            VehicleSubClass = quoteQueryModel.VehicleDetails.VehicleSubTypeCode,
                            YearOfMake = quoteQueryModel.RegistrationYear
                        }
                    }
                }
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(IFFCOEnvelope));
            StringBuilder requestBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(requestBuilder);
            xmlSerializer.Serialize(stringWriter, idvrequest);
            requestBody = requestBuilder.ToString();
            _logger.LogInformation("IFFCO CV IDV RequestBody {RequestBody}", requestBody);

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client.DefaultRequestHeaders.Add("SOAPAction", "getVehicleCVIIdv");

            id = await InsertICLogs(requestBody, quoteQueryModel.LeadId, _iFFCOConfig.BaseURL + _iFFCOConfig.IdvCVUrl, string.Empty, _client.DefaultRequestHeaders.ToString(), "Quote");
            try
            {
                var idvResponse = await _client.PostAsync(_iFFCOConfig.IdvCVUrl, new StringContent(requestBody, Encoding.UTF8, "application/xml"), cancellation);

                if (!idvResponse.IsSuccessStatusCode)
                {
                    responseBody = idvResponse.ReasonPhrase;
                    iFFCOIDVResponseModel.StatusCode = (int)idvResponse.StatusCode;
                    iFFCOIDVResponseModel.erorMessage = responseBody;
                    _logger.LogError("IIFCO CV IDV Data not found {responseBody}", responseBody);
                }
                else
                {
                    responseBody = idvResponse.Content.ReadAsStringAsync().Result.ToString();
                    _logger.LogInformation("IFFCO CV IDV Response {ResponseBody}", responseBody);

                    StringReader reader = new StringReader(responseBody);
                    var response = (IFFCOEnvelope)(xmlSerializer.Deserialize(reader));
                    if (response?.Body.GetVehicleCVIIdvResponse != null && string.IsNullOrEmpty(response?.Body.GetVehicleCVIIdvResponse.GetVehicleCVIIdvReturn.ErorMessage))
                    {
                        iFFCOIDVResponseModel.Idv = Convert.ToInt32(response.Body.GetVehicleCVIIdvResponse.GetVehicleCVIIdvReturn.Idv);
                        iFFCOIDVResponseModel.maximumIdvAllowed = Convert.ToInt32(response.Body.GetVehicleCVIIdvResponse.GetVehicleCVIIdvReturn.MaximumIdvAllowed);
                        iFFCOIDVResponseModel.minimumIdvAllowed = Convert.ToInt32(response.Body.GetVehicleCVIIdvResponse.GetVehicleCVIIdvReturn.MinimumIdvAllowed);
                        iFFCOIDVResponseModel.StatusCode = (int)HttpStatusCode.OK;
                        quoteQueryModel.MinIDV = Convert.ToDecimal(response.Body.GetVehicleCVIIdvResponse.GetVehicleCVIIdvReturn.MaximumIdvAllowed);
                        quoteQueryModel.MaxIDV = Convert.ToDecimal(response.Body.GetVehicleCVIIdvResponse.GetVehicleCVIIdvReturn.MinimumIdvAllowed);
                        quoteQueryModel.RecommendedIDV = Convert.ToDecimal(response.Body.GetVehicleCVIIdvResponse.GetVehicleCVIIdvReturn.Idv);
                    }
                    else
                    {
                        iFFCOIDVResponseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        iFFCOIDVResponseModel.erorMessage = response?.Body.GetVehicleCVIIdvResponse.GetVehicleCVIIdvReturn.ErorMessage;
                    }
                }
                await UpdateICLogs(id, string.Empty, responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("IFFCO CV IDV Error {exception}", ex.Message);
                iFFCOIDVResponseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                await UpdateICLogs(id, string.Empty, ex.Message);
                return iFFCOIDVResponseModel;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("IFFCO CV IDV Error {exception}", ex.Message);
            iFFCOIDVResponseModel.StatusCode = (int)HttpStatusCode.BadRequest;
            await UpdateICLogs(id, string.Empty, ex.Message);
            return iFFCOIDVResponseModel;
        }
        return iFFCOIDVResponseModel;
    }
    private void GetCVCoverMapping(QuoteQueryModel quoteQuery, IFFCOEnvelope quoteRequest)
    {
        bool isVehicleAgeLessThan3Years = IsYearGreaterThanValue(quoteQuery.RegistrationYear, 3);
        bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(quoteQuery.RegistrationYear, 5);
        //NCB Mapping

        if (quoteQuery.CategoryId == "2")
        {
            if (quoteQuery.PreviousPolicyDetails.IsPreviousInsurerKnown && !quoteQuery.PreviousPolicyDetails.IsClaimInLastYear)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "No Claim Bonus",
                    Number = string.Empty,
                    SumInsured = quoteQuery.CurrentNCBPercentage
                });
            }
            if (quoteQuery.Accessories.IsCNG)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "CNG Kit",
                    Number = string.Empty,
                    SumInsured = quoteQuery.Accessories.CNGValue.ToString()
                });
            }
            if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "PA Owner / Driver",
                    Number = string.Empty,
                    SumInsured = "Y"
                });
            }
            if (quoteQuery.AddOns.IsZeroDebt && isVehicleAgeLessThan5Years)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "Depreciation Waiver",
                    Number = string.Empty,
                    SumInsured = "Y"
                });
            }
            if (quoteQuery.AddOns.IsConsumableRequired && isVehicleAgeLessThan5Years)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "Consumable",
                    Number = string.Empty,
                    SumInsured = "Y"
                });
            }
            if (quoteQuery.AddOns.IsIMT23)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "IMT 23",
                    Number = string.Empty,
                    SumInsured = "Y"
                });
            }
        }
        else if (quoteQuery.CategoryId == "1")
        {
            if (quoteQuery.PreviousPolicyDetails.IsPreviousInsurerKnown && !quoteQuery.PreviousPolicyDetails.IsClaimInLastYear)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "No Claim Bonus",
                    Number = string.Empty,
                    SumInsured = quoteQuery.CurrentNCBPercentage
                });
            }
            //Accessories Mapping
            if (quoteQuery.Accessories.IsCNG)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "CNG Kit",
                    Number = string.Empty,
                    SumInsured = quoteQuery.Accessories.CNGValue.ToString()
                });
            }
            if (quoteQuery.Accessories.IsElectrical)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "Electrical Accessories",
                    Number = string.Empty,
                    SumInsured = quoteQuery.Accessories.ElectricalValue.ToString()
                });
            }


            //Discount Mapping
            if (quoteQuery.Discounts.IsLimitedTPCoverage && !quoteQuery.VehicleDetails.IsCommercialVehicle)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "TPPD",
                    Number = string.Empty,
                    SumInsured = "750000"
                });
            }

            //PA Cover Mapping
            if (quoteQuery.PACover.IsUnnamedPassenger)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "PA to Passenger",
                    Number = string.Empty,
                    SumInsured = quoteQuery.UnnamedPassangerAndPillonRider
                });
            }
            if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "PA Owner / Driver",
                    Number = string.Empty,
                    SumInsured = "Y"
                });
            }

            //Addon Mapping
            if (quoteQuery.AddOns.IsZeroDebt && isVehicleAgeLessThan5Years)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "Depreciation Waiver",
                    Number = string.Empty,
                    SumInsured = "Y"
                });
            }
            if (quoteQuery.AddOns.IsConsumableRequired && isVehicleAgeLessThan5Years)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "Consumable",
                    Number = string.Empty,
                    SumInsured = "Y"
                });
            }
            if (quoteQuery.AddOns.IsIMT23 && isVehicleAgeLessThan5Years)
            {
                quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.VehicleCoverage.item.Add(new Item
                {
                    CoverageId = "IMT 23",
                    Number = string.Empty,
                    SumInsured = "Y"
                });
            }
        }
    }
}

