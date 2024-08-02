using Dapper;
using DocumentFormat.OpenXml.InkML;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.IFFCO.Command.CKYC;
using Insurance.Core.Features.IFFCO.Command.CreateCKYC;
using Insurance.Core.Features.IFFCO.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Insurance.Persistence.Repository
{
    public class IFFCORepository : IIFFCORepository
    {
        private readonly IFFCOConfig _iFFCOConfig;
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly IIFFCOService _iFFCOService;
        private readonly IQuoteRepository _quoteRepository;
        private readonly ILogger<IFFCORepository> _logger;
        private readonly IApplicationClaims _applicationClaims;
        private readonly VehicleTypeConfig _vehicleTypeConfig;
        private readonly PolicyTypeConfig _policyTypeConfig;

        public IFFCORepository(IOptions<IFFCOConfig> options,
            ApplicationDBContext applicationDBContext,
            IIFFCOService iFFCOService,
            IQuoteRepository quoteRepository,
            ILogger<IFFCORepository> logger,
            IApplicationClaims applicationClaims,
            IOptions<VehicleTypeConfig> vehicleTypeConfig,
            IOptions<PolicyTypeConfig> policyTypeConfig)
        {
            _iFFCOConfig = options.Value;
            _applicationDBContext = applicationDBContext ?? throw new ArgumentException(nameof(applicationDBContext));
            _iFFCOService = iFFCOService ?? throw new ArgumentException(nameof(iFFCOService));
            _quoteRepository = quoteRepository;
            _logger = logger;
            _applicationClaims = applicationClaims;
            _vehicleTypeConfig = vehicleTypeConfig.Value;
            _policyTypeConfig = policyTypeConfig.Value;
        }
        public async Task<QuoteResponseModel> GetQuote(GetIFFCOQuery query, CancellationToken cancellationToken)
        {
            var quoteQuery = await QuoteMasterMapping(query);
            var idvDetails = (dynamic)null;
            if (query.PolicyDates.IsCommercial)
            {
                idvDetails = await _iFFCOService.GetCVIDV(quoteQuery, cancellationToken);
            }
            else
            {
                idvDetails = await _iFFCOService.GetIDV(quoteQuery, cancellationToken);
            }
            if (idvDetails?.StatusCode == 200)
            {
                decimal MaxIDV = Convert.ToDecimal(idvDetails.maximumIdvAllowed);
                decimal MinIDV = Convert.ToDecimal(idvDetails.minimumIdvAllowed);
                decimal RecommendedIDV = Convert.ToDecimal(idvDetails.Idv);
                quoteQuery.MaxIDV = MaxIDV;
                quoteQuery.MinIDV = MinIDV;
                quoteQuery.IDVValue = RecommendedIDV;

                quoteQuery.RecommendedIDV = query.IDV switch
                {
                    1 => RecommendedIDV,
                    2 => MinIDV,
                    3 => MaxIDV,
                    > 3 => query.IDV,
                    _ => MaxIDV,
                };

                var quoteResponse = await _iFFCOService.GetQuote(quoteQuery, cancellationToken);

                SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
                {
                    quoteResponseModel = quoteResponse.Item1,
                    RequestBody = quoteResponse.Item2,
                    ResponseBody = quoteResponse.Item3,
                    Stage = "Quote",
                    InsurerId = _iFFCOConfig.InsurerId,
                    LeadId = query.LeadId,
                    MaxIDV = MaxIDV,
                    MinIDV = MinIDV,
                    RecommendedIDV = RecommendedIDV,
                    TransactionId = quoteResponse.Item1.ApplicationId,
                    PolicyNumber = string.Empty,
                    SelectedIDV = Convert.ToString(query.IDV),
                    PolicyTypeId = query.PreviousPolicy.PreviousPolicyTypeId,
                    IsPreviousPolicy = query.PreviousPolicy.IsPreviousPolicy,
                    SAODInsurerId = query.PreviousPolicy.SAODInsurer,
                    SATPInsurerId = query.PreviousPolicy.TPInsurer,
                    SAODPolicyStartDate = Convert.ToDateTime(query.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    SAODPolicyExpiryDate = Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    SATPPolicyStartDate = Convert.ToDateTime(query.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    SATPPolicyExpiryDate = Convert.ToDateTime(query.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    RegistrationDate = Convert.ToDateTime(query.PolicyDates.RegistrationDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    VehicleNumber = query.IsVehicleNumberPresent ? query.VehicleNumber : quoteQuery.VehicleDetails.LicensePlateNumber,
                    IsPreviousYearClaim = query.PreviousPolicy.IsPreviousYearClaim,
                    PreviousYearNCB = query.PreviousPolicy.NCBId,
                    IsBrandNew = query.IsBrandNewVehicle,
                    RTOId = query.RTOId,
                };
                await _quoteRepository.SaveQuoteTransaction(saveQuoteTransactionModel, cancellationToken);

                if (quoteResponse.Item1.InsurerName != null)
                {
                    quoteResponse.Item1.InsurerId = _iFFCOConfig.InsurerId;
                    return quoteResponse.Item1;
                }
                else
                {
                    return new QuoteResponseModel
                    {
                        InsurerId = _iFFCOConfig.InsurerId,
                        InsurerName = _iFFCOConfig.InsurerName,
                        InsurerLogo = _iFFCOConfig.InsurerLogo,
                        InsurerStatusCode = quoteResponse.Item1.InsurerStatusCode,
                        ValidationMessage = quoteResponse?.Item1?.ValidationMessage
                    };
                }
            }
            else
            {
                return new QuoteResponseModel
                {
                    InsurerId = _iFFCOConfig.InsurerId,
                    InsurerName = _iFFCOConfig.InsurerName,
                    InsurerLogo = _iFFCOConfig.InsurerLogo,
                    InsurerStatusCode = idvDetails.StatusCode,
                    ValidationMessage = idvDetails.erorMessage
                };
            }
        }
        public async Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
        {
            if (!quoteConfirmCommand.IsBrandNewVehicle && quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy && !quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim && !quoteTransactionDbModel.QuoteConfirmDetailsModel.CurrentPolicyType.Equals("SATP"))
            {
                quoteConfirmCommand.CurrentNCBPercentage = GetCurrentNCB(quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue);
            }
            else
            {
                quoteConfirmCommand.CurrentNCBPercentage = "0";
            }
            var res = await _iFFCOService.QuoteConfirmDetails(quoteTransactionDbModel, quoteConfirmCommand, cancellationToken);
            return res;
        }
        public async Task<SaveCKYCResponse> GetCKYCDetails(IFFCOCKYCCommand iffcoCKYCCommand, CancellationToken cancellationToken)
        {
            var cKycResponse = await _iFFCOService.GetCKYCResponse(iffcoCKYCCommand, cancellationToken);

            CreateLeadModel createLeadModelObject = cKycResponse.Item4;
            var response = await _quoteRepository.SaveLeadDetails(iffcoCKYCCommand.InsurerId, iffcoCKYCCommand.QuoteTransactionId, cKycResponse.Item1, cKycResponse.Item2, "POI", createLeadModelObject, cancellationToken);

            cKycResponse.Item3.LeadID = response.LeadID;
            cKycResponse.Item3.CKYCNumber = response.CKYCNumber;
            cKycResponse.Item3.KYCId = response.KYCId;
            if (cKycResponse != null)
            {
                return cKycResponse.Item3;
            }
            return default;
        }
        public async Task<UploadCKYCDocumentResponse> UploadCKYCDocument(CreateIFFCOCKYCCommand createIFFCOCKYCCommand, CancellationToken cancellationToken)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            var stateCode = createIFFCOCKYCCommand.State;
            var cityCode = createIFFCOCKYCCommand.City;
            parameters.Add("QuoteTransactionId", createIFFCOCKYCCommand.QuoteTransactionId, DbType.String, ParameterDirection.Input);
            parameters.Add("StateCode", createIFFCOCKYCCommand.State, DbType.String, ParameterDirection.Input);
            parameters.Add("CityCode", createIFFCOCKYCCommand.City, DbType.String, ParameterDirection.Input);
            parameters.Add("Salutation", createIFFCOCKYCCommand.Salutation, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetIFFCOCustomerType]", parameters,
                         commandType: CommandType.StoredProcedure);

            var ckycMasters = result.Read<IFFCOGetCKYCMasterModel>().FirstOrDefault();

            createIFFCOCKYCCommand.CustomerType = ckycMasters.CustomerType;
            createIFFCOCKYCCommand.State = ckycMasters.State;
            createIFFCOCKYCCommand.City = ckycMasters.City;

            var ckycResponse = await _iFFCOService.UploadCKYCDocument(createIFFCOCKYCCommand, cancellationToken);

            if (ckycResponse != null)
            {
                ckycResponse.Item4.State = stateCode;
                ckycResponse.Item4.City = cityCode;
                ckycResponse.Item4.Salutation = ckycMasters.Salutation;
                var response = await _quoteRepository.SaveLeadDetails(createIFFCOCKYCCommand.InsurerId, createIFFCOCKYCCommand.QuoteTransactionId, ckycResponse.Item1, ckycResponse.Item2, "POA", ckycResponse.Item4, cancellationToken);
                ckycResponse.Item3.LeadID = response.LeadID;
                ckycResponse.Item3.CKYCNumber = response.CKYCNumber;
                ckycResponse.Item3.KYCId = response.KYCId;
            }
            return ckycResponse?.Item3;
        }
        public async Task<SaveQuoteTransactionModel> CreateProposal(QuoteTransactionDbModel quoteDetails, CancellationToken cancellationToken)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(IFFCOEnvelope));
            StringReader reader = new StringReader(quoteDetails.QuoteTransactionRequest?.ResponseBody);
            StringReader requestReader = new StringReader(quoteDetails.QuoteTransactionRequest?.RequestBody.ToString().Replace("getMotorPremium", "getNewVehiclePremium"));

            var quoteResponse = (IFFCOEnvelope)(xmlSerializer.Deserialize(reader));
            var quoteRequest = (IFFCOEnvelope)(xmlSerializer.Deserialize(requestReader));
            CreateLeadModel leadDetails = (quoteDetails.LeadDetail);
            IFFCOProposalDynamicDetails proposalDynamicDetails = JsonConvert.DeserializeObject<IFFCOProposalDynamicDetails>(quoteDetails.ProposalRequestBody);
            QuoteResponseModel commonResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(quoteDetails.QuoteTransactionRequest.CommonResponse);


            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("RTOCityCode", quoteRequest.Body.GetNewVehiclePremium.IFFCOPolicy.IFFCOVehicle.RegictrationCity, DbType.String, ParameterDirection.Input);
            parameters.Add("FinancierName", proposalDynamicDetails.VehicleDetails.financer, DbType.String, ParameterDirection.Input);
            parameters.Add("LeadId", leadDetails.LeadID, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<IFFCOPreviousPolicyDetailsModel>("[dbo].[Insurance_GetIFFCOProposalDetails]", parameters,
                         commandType: CommandType.StoredProcedure);
            var previousPolicyDetails = result.FirstOrDefault();

            var proposalResponse = await _iFFCOService.GetProposal(quoteRequest, quoteResponse, leadDetails, proposalDynamicDetails, commonResponse, previousPolicyDetails, cancellationToken);

            if (proposalResponse != null)
            {
                SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
                {
                    quoteResponseModel = proposalResponse.Item1,
                    RequestBody = proposalResponse.Item2,
                    ResponseBody = string.Empty,
                    Stage = "Proposal",
                    InsurerId = _iFFCOConfig.InsurerId,
                    LeadId = leadDetails.LeadID,
                    MaxIDV = (decimal)(commonResponse.MaxIDV),
                    MinIDV = (decimal)commonResponse.MinIDV,
                    RecommendedIDV = (decimal)commonResponse.IDV,
                    TransactionId = proposalResponse.Item1.ApplicationId,
                };
                return saveQuoteTransactionModel;
            }
            else
            {
                return default;
            }
        }
        public async Task<IFFCOPolicyDocumentResponse> GetPolicyDocumentURL(IFFCOPaymentResponseModel iFFCOPaymentResponseModel, CancellationToken cancellationToken)
        {
            var policyDocumentResponse = await _iFFCOService.GetPolicyDownloadURL(iFFCOPaymentResponseModel, cancellationToken);
            return policyDocumentResponse;
        }
        public async Task<byte[]> GetPolicyDocument(string leadId, string url, CancellationToken cancellationToken)
        {
            var policyDocumentResponse = await _iFFCOService.PolicyDownload(leadId, url, cancellationToken);
            return policyDocumentResponse;
        }
        public async Task<string> GetProposalQuotetransactionId(string proposalNumber, CancellationToken cancellationToken)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("ProposalNumber", proposalNumber, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<string>("[dbo].[Insurance_GetIFFCOProposalTransactionId]",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }
        public async Task<SaveQuoteTransactionModel> GenerateBreakin(QuoteTransactionDbModel quoteDetails, CancellationToken cancellationToken)
        {
            CreateLeadModel leadDetails = (quoteDetails.LeadDetail);
            IFFCOProposalDynamicDetails proposalDynamicDetails = JsonConvert.DeserializeObject<IFFCOProposalDynamicDetails>(quoteDetails.ProposalRequestBody);
            QuoteResponseModel commonResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(quoteDetails.QuoteTransactionRequest?.CommonResponse);
            if (leadDetails != null && proposalDynamicDetails != null)
            {
                var breakinResponse = await _iFFCOService.GenerateBreakin(proposalDynamicDetails, leadDetails, cancellationToken);
                if (breakinResponse != null && !string.IsNullOrEmpty(breakinResponse?.Item2))
                {
                    commonResponse.ApplicationId = breakinResponse?.Item1;
                    commonResponse.BreakinId = breakinResponse?.Item2;
                    commonResponse.ProposalNumber = breakinResponse?.Item1;
                    commonResponse.PolicyNumber = breakinResponse?.Item1;
                    commonResponse.IsBreakIn = true;
                    commonResponse.IsSelfInspection = true;
                    SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
                    {
                        quoteResponseModel = commonResponse,
                        RequestBody = breakinResponse?.Item3,
                        ResponseBody = breakinResponse?.Item4,
                        Stage = "BreakIn",
                        InsurerId = _iFFCOConfig.InsurerId,
                        LeadId = leadDetails.LeadID,
                        MaxIDV = Convert.ToDecimal(commonResponse.MaxIDV),
                        MinIDV = Convert.ToDecimal(commonResponse.MinIDV),
                        RecommendedIDV = Convert.ToDecimal(commonResponse.IDV),
                        TransactionId = commonResponse.TransactionID,
                        PolicyNumber = string.Empty,
                    };
                    return saveQuoteTransactionModel;
                }
                return default;
            }
            return default;
        }
        public async Task<IFFCOBreakinStatusResponseModel> BreakinInspectionStatus(string leadId, string breakinId, CancellationToken cancellationToken)
        {
            var breakinStatus = await _iFFCOService.GetBreakinStatus(leadId, breakinId, cancellationToken);
            return breakinStatus;
        }
        public async Task<Tuple<string, string, string>> GetPaymentLink(string proposalRequest, bool isCorporate, CancellationToken cancellationToken)
        {
            try
            {
                var requestBody = string.Empty;
                var oldProposalNumber = string.Empty;

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(IFFCOProposalRequest));
                StringReader reader = new StringReader(proposalRequest);
                var proposalRequestBody = (IFFCOProposalRequest)(xmlSerializer.Deserialize(reader));

                Random random = new Random();
                var uniqId = "HERO" + random.Next(10000, 99999).ToString();
                oldProposalNumber = proposalRequestBody.Policy.UniqueQuoteId;
                proposalRequestBody.Policy.UniqueQuoteId = uniqId;
                if (!isCorporate)
                {
                    proposalRequestBody.Contact.ExternalClientNo = uniqId;
                }
                else
                {
                    proposalRequestBody.Account.ExternalAccountId = uniqId;
                }

                var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                XmlSerializer xmlRequestSerializer = new XmlSerializer(typeof(IFFCOProposalRequest));
                var settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;
                using (var stream = new StringWriter())
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    xmlRequestSerializer.Serialize(writer, proposalRequestBody, emptyNamespaces);
                    requestBody = stream.ToString();
                }
                return Tuple.Create(requestBody, uniqId, oldProposalNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError("IFFCO Get PaymentLink {exception}", ex.Message);
                return default;
            }
        }
        public async Task<CreateLeadModel> GetBreakinDetails(string quoteTransactionId, CancellationToken cancellationToken)
        {
            var dbConnection = _applicationDBContext.CreateConnection();
            var dbParameters = new DynamicParameters();
            dbParameters.Add("QuotetransactionId", quoteTransactionId);
            var result = await dbConnection.QueryAsync<CreateLeadModel>("[dbo].[Insurance_GetIFFCOBreakinDetails]", dbParameters,
                 commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }
        public async Task<string> UpdateLeadPaymentLink(string insurerId, string quoteTransactionId, string paymentLink, string uniqId, string proposalRequest, string oldProposalNumber, CancellationToken cancellationToken)
        {
            using var connections = _applicationDBContext.CreateConnection();
            var parameter = new DynamicParameters();
            parameter.Add("InsurerId", insurerId, DbType.String, ParameterDirection.Input);
            parameter.Add("QuoteTransactionId", quoteTransactionId, DbType.String, ParameterDirection.Input);
            parameter.Add("PaymentLink", paymentLink, DbType.String, ParameterDirection.Input);
            parameter.Add("ProposalRequest", proposalRequest, DbType.String, ParameterDirection.Input);
            parameter.Add("ProposalNumber", uniqId, DbType.String, ParameterDirection.Input);
            parameter.Add("UpdatedBy", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameter.Add("OldProposalNumber", oldProposalNumber, DbType.String, ParameterDirection.Input);

            var updatePaymentLink = await connections.QueryMultipleAsync("[dbo].[Insurance_UpdateIFFCOPaymentLinkDetails]",
                parameter,
                commandType: CommandType.StoredProcedure);

            var updatePaymentLinkResponse = updatePaymentLink.Read<string>().FirstOrDefault();
            if (updatePaymentLinkResponse != null)
            {
                return updatePaymentLinkResponse;
            }
            return default;
        }

        private async Task<QuoteQueryModel> QuoteMasterMapping(GetIFFCOQuery query)
        {
            var addOnId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnId} ")) : String.Empty;
            var discountId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList?.Select(x => $"{x.DiscountId} ")) : String.Empty;
            var paCoverId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverId} ")) : String.Empty;
            var accessoryId = (query.AccessoryList != null && query.AccessoryList.Any()) ? String.Join(",", query.AccessoryList?.Select(x => $"{x.AccessoryId} ")) : String.Empty;
            var discountExtensionId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList.Select(x => $"{x.DiscountExtensionId}")) : string.Empty;
            var paCoverExtensionId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverExtensionId} ")) : String.Empty;
            var addOnsExtensionId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnsExtensionId} ")) : String.Empty;

            var previousInsurer = query.PreviousPolicy == null && !string.IsNullOrEmpty(query.PreviousPolicy?.SAODInsurer) && query.PreviousPolicy?.SAODInsurer != "" ? query.PreviousPolicy.SAODInsurer : _iFFCOConfig.InsurerId;

            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("PACoverId", paCoverId, DbType.String, ParameterDirection.Input);
            parameters.Add("AccessoryId", accessoryId, DbType.String, ParameterDirection.Input);
            parameters.Add("AddonId", addOnId, DbType.String, ParameterDirection.Input);
            parameters.Add("DiscountId", discountId, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", _iFFCOConfig.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("RTOId", query.RTOId, DbType.String, ParameterDirection.Input);
            parameters.Add("VariantId", query.VariantId, DbType.String, ParameterDirection.Input);
            parameters.Add("NCBId", query.PreviousPolicy?.NCBId, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyTypeId", query.PolicyDates.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleTypeId", query.VehicleTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleNumber", query.VehicleNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("DiscountExtensionId", discountExtensionId, DbType.String, ParameterDirection.Input);
            parameters.Add("PACoverExtensionId", paCoverExtensionId, DbType.String, ParameterDirection.Input);
            parameters.Add("AddOnsExtensionId", addOnsExtensionId, DbType.String, ParameterDirection.Input);
            parameters.Add("SAODInsurerId", previousInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("SATPInsurerId", previousInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("IsBrandNew", query.IsBrandNewVehicle, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetIFFCOQuoteMasterMapping]", parameters,
                         commandType: CommandType.StoredProcedure);

            var paCoverList = result.Read<PACoverModel>();
            var accessoryList = result.Read<AccessoryModel>();
            var addOnList = result.Read<AddonsModel>();
            var discountList = result.Read<DiscountModel>();
            var discountExtensionList = result.Read<DiscountExtensionModel>();
            var paCoverExtensionList = result.Read<PACoverExtensionModel>();
            var addOnsExtensionList = result.Read<AddOnsExtensionModel>();
            var codeList = result.Read<RTOVehiclePreviousInsurerModel>();
            var quoteQuery = new QuoteQueryModel
            {
                AddOns = new Domain.GoDigit.AddOns(),
                PACover = new PACover(),
                Accessories = new Domain.GoDigit.Accessories(),
                Discounts = new Discounts(),
                VehicleDetails = new VehicleDetails(),
                PreviousPolicyDetails = new PreviousPolicy(),
            };

            string previousInsurerCode = string.Empty;
            string ncbValue = string.Empty;
            quoteQuery.LeadId = query.LeadId;

            if (discountExtensionList.Any())
            {
                quoteQuery.VoluntaryExcess = discountExtensionList.FirstOrDefault()?.DiscountExtension;
            }
            if (paCoverExtensionList.Any())
            {
                quoteQuery.UnnamedPassangerAndPillonRider = paCoverExtensionList.FirstOrDefault()?.PACoverExtension;
            }
            if (addOnsExtensionList.Any())
            {
                quoteQuery.GeogExtension = addOnsExtensionList.FirstOrDefault()?.AddOnsExtension;
            }

            if (addOnList.Any())
            {
                foreach (var item in addOnList)
                {
                    if (item.AddOns.Equals("PartsDepreciation"))
                    {
                        quoteQuery.AddOns.ZeroDebtId = (addOnList?.Where(x => x.AddOns == "PartsDepreciation").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsZeroDebt = true;
                    }
                    else if (item.AddOns.Equals("EngineProtection"))
                    {
                        quoteQuery.AddOns.EngineProtectionId = (addOnList?.Where(x => x.AddOns == "EngineProtection").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsEngineProtectionRequired = true;
                    }
                    else if (item.AddOns.Equals("RoadSideAssistance"))
                    {
                        quoteQuery.AddOns.RoadSideAssistanceId = (addOnList?.Where(x => x.AddOns == "RoadSideAssistance").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsRoadSideAssistanceRequired = true;
                    }
                    else if (item.AddOns.Equals("KeyAndLockReplacement"))
                    {
                        quoteQuery.AddOns.KeyAndLockProtectionId = (addOnList?.Where(x => x.AddOns == "KeyAndLockReplacement").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsKeyAndLockProtectionRequired = true;
                    }
                    else if (item.AddOns.Equals("Consumables"))
                    {
                        quoteQuery.AddOns.ConsumableId = (addOnList?.Where(x => x.AddOns == "Consumables").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsConsumableRequired = true;
                    }
                    else if (item.AddOns.Equals("PersonalBelongings"))
                    {
                        quoteQuery.AddOns.PersonalBelongingId = (addOnList?.Where(x => x.AddOns == "PersonalBelongings").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsPersonalBelongingRequired = true;
                    }
                    else if (item.AddOns.Equals("ReturnToInvoice"))
                    {
                        quoteQuery.AddOns.ReturnToInvoiceIdId = (addOnList?.Where(x => x.AddOns == "ReturnToInvoice").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsInvoiceCoverRequired = true;
                    }
                    else if (item.AddOns.Equals("DailyAllowance"))
                    {
                        quoteQuery.AddOns.DailyAllowanceId = (addOnList?.Where(x => x.AddOns == "DailyAllowance").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsDailyAllowance = true;
                    }
                    else if (item.AddOns.Equals("EMIProtectorCover"))
                    {
                        quoteQuery.AddOns.EMIProtectorId = (addOnList?.Where(x => x.AddOns == "EMIProtectorCover").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsEMIProtectorRequired = true;
                    }
                    else if (item.AddOns.Equals("GeoAreaExtension"))
                    {
                        quoteQuery.AddOns.GeoAreaExtensionId = (addOnList?.Where(x => x.AddOns == "GeoAreaExtension").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsGeoAreaExtension = true;
                    }
                    else if (item.AddOns.Equals("LimitedtoOwnPremisesCover"))
                    {
                        quoteQuery.AddOns.LimitedOwnPremisesId = (addOnList?.Where(x => x.AddOns == "LimitedtoOwnPremisesCover").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsLimitedOwnPremisesRequired = true;
                    }
                    else if (item.AddOns.Equals("LossofUseDownTimeProtCover"))
                    {
                        quoteQuery.AddOns.LossOfDownTimeId = (addOnList?.Where(x => x.AddOns == "LossofUseDownTimeProtCover").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsLossOfDownTimeRequired = true;
                    }
                    else if (item.AddOns.Equals("NCBProtect"))
                    {
                        quoteQuery.AddOns.NCBId = (addOnList?.Where(x => x.AddOns == "NCBProtect").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsNCBRequired = true;
                    }
                    else if (item.AddOns.Equals("RimProtection"))
                    {
                        quoteQuery.AddOns.RimProtectionId = (addOnList?.Where(x => x.AddOns == "RimProtection").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsRimProtectionRequired = true;
                    }
                    else if (item.AddOns.Equals("RoadsideAssistanceAdvance"))
                    {
                        quoteQuery.AddOns.RoadSideAssistanceAdvanceId = (addOnList?.Where(x => x.AddOns == "RoadsideAssistanceAdvance").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired = true;
                    }
                    else if (item.AddOns.Equals("RoadsideAssistanceWider"))
                    {
                        quoteQuery.AddOns.RoadSideAssistanceWiderId = (addOnList?.Where(x => x.AddOns == "RoadsideAssistanceWider").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsRoadSideAssistanceWiderRequired = true;
                    }
                    else if (item.AddOns.Equals("TowingCover"))
                    {
                        quoteQuery.AddOns.TowingId = (addOnList?.Where(x => x.AddOns == "TowingCover").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsTowingRequired = true;
                    }
                    else if (item.AddOns.Equals("TyreProtection"))
                    {
                        quoteQuery.AddOns.TyreProtectionId = (addOnList?.Where(x => x.AddOns == "TyreProtection").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsTyreProtectionRequired = true;
                    }
                    else if (item.AddOns.Equals("IMT23"))
                    {
                        quoteQuery.AddOns.IMT23Id = (addOnList?.Where(x => x.AddOns == "IMT23").Select(d => d.AddOnId).FirstOrDefault());
                        quoteQuery.AddOns.IsIMT23 = true;
                    }
                }
                if (quoteQuery.AddOns.IsConsumableRequired || quoteQuery.AddOns.IsNCBRequired)
                {
                    quoteQuery.AddOns.IsZeroDebt = true;
                }
            }
            if (accessoryList.Any())
            {
                foreach (var item in accessoryList)
                {
                    if (item.Accessory.Equals("CNG"))
                    {
                        quoteQuery.Accessories.CNGId = (accessoryList?.Where(x => x.Accessory == "CNG").Select(d => d.AccessoryId).FirstOrDefault());
                        quoteQuery.Accessories.IsCNG = true;
                        quoteQuery.Accessories.CNGValue = (int)(query.AccessoryList?.Where(x => x.AccessoryId == item.AccessoryId).Select(d => d.AccessoryValue).FirstOrDefault());
                    }
                    else if (item.Accessory.Equals("Electrical"))
                    {
                        quoteQuery.Accessories.ElectricalId = (accessoryList?.Where(x => x.Accessory == "Electrical").Select(d => d.AccessoryId).FirstOrDefault());
                        quoteQuery.Accessories.IsElectrical = true;
                        quoteQuery.Accessories.ElectricalValue = (int)(query.AccessoryList?.Where(x => x.AccessoryId == item.AccessoryId).Select(d => d.AccessoryValue).FirstOrDefault());
                    }
                    else if (item.Accessory.Equals("NonElectrical"))
                    {
                        quoteQuery.Accessories.NonElectricalId = (accessoryList?.Where(x => x.Accessory == "NonElectrical").Select(d => d.AccessoryId).FirstOrDefault());
                        quoteQuery.Accessories.IsNonElectrical = true;
                        quoteQuery.Accessories.NonElectricalValue = (int)(query.AccessoryList?.Where(x => x.AccessoryId == item.AccessoryId).Select(d => d.AccessoryValue).FirstOrDefault());
                    }
                }
            }
            if (paCoverList.Any())
            {
                foreach (var item in paCoverList)
                {
                    if (item.CoverName.Equals("UnnamedPax"))
                    {
                        quoteQuery.PACover.UnnamedPassengerId = (paCoverList?.Where(x => x.CoverName == "UnnamedPax").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsUnnamedPassenger = true;

                    }
                    else if (item.CoverName.Equals("UnnamedPillionRider"))
                    {
                        quoteQuery.PACover.UnnamedPassengerId = (paCoverList?.Where(x => x.CoverName == "UnnamedPillionRider").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsUnnamedPassenger = true;

                    }
                    else if (item.CoverName.Equals("UnnamedOWNERDRIVER"))
                    {
                        quoteQuery.PACover.UnnamedOWNERDRIVERId = (paCoverList?.Where(x => x.CoverName == "UnnamedOWNERDRIVER").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsUnnamedOWNERDRIVER = true;
                    }
                    else if (item.CoverName.Equals("UnnamedPaidDriver"))
                    {
                        quoteQuery.PACover.PaidDriverId = (paCoverList?.Where(x => x.CoverName == "UnnamedPaidDriver").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsPaidDriver = true;
                    }
                    //For Commercial Vehicle
                    else if (item.CoverName.Equals("UnnamedConductor"))
                    {
                        quoteQuery.PACover.UnnamedConductorId = (paCoverList?.Where(x => x.CoverName == "UnnamedPax").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsUnnamedConductor = true;

                    }
                    else if (item.CoverName.Equals("UnnamedHirer"))
                    {
                        quoteQuery.PACover.UnnamedHirerId = (paCoverList?.Where(x => x.CoverName == "UnnamedPillionRider").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsUnnamedHirer = true;

                    }
                    else if (item.CoverName.Equals("UnnamedCleaner"))
                    {
                        quoteQuery.PACover.UnnamedCleanerId = (paCoverList?.Where(x => x.CoverName == "UnnamedOWNERDRIVER").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsUnnamedCleaner = true;
                    }
                    else if (item.CoverName.Equals("UnnamedHelper"))
                    {
                        quoteQuery.PACover.UnnamedHelperId = (paCoverList?.Where(x => x.CoverName == "UnnamedOWNERDRIVER").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsUnnamedHelper = true;
                    }

                }
            }
            if (discountList.Any())
            {
                foreach (var item in discountList)
                {
                    if (item.DiscountName.Equals("AAMembership"))
                    {
                        quoteQuery.Discounts.AAMemberShipId = (discountList?.Where(x => x.DiscountName == "AAMembership").Select(d => d.DiscountId).FirstOrDefault());
                        quoteQuery.Discounts.IsAAMemberShip = true;
                    }
                    else if (item.DiscountName.Equals("ARAIApprovedAntiTheftDevice"))
                    {
                        quoteQuery.Discounts.AntiTheftId = (discountList?.Where(x => x.DiscountName == "ARAIApprovedAntiTheftDevice").Select(d => d.DiscountId).FirstOrDefault());
                        quoteQuery.Discounts.IsAntiTheft = true;
                    }
                    else if (item.DiscountName.Equals("LimitedThirdPartyCoverage"))
                    {
                        quoteQuery.Discounts.LimitedTPCoverageId = (discountList?.Where(x => x.DiscountName == "LimitedThirdPartyCoverage").Select(d => d.DiscountId).FirstOrDefault());
                        quoteQuery.Discounts.IsLimitedTPCoverage = true;
                    }
                    else if (item.DiscountName.Equals("VoluntaryDeductible"))
                    {
                        quoteQuery.Discounts.VoluntarilyDeductibleId = (discountList?.Where(x => x.DiscountName == "VoluntaryDeductible").Select(d => d.DiscountId).FirstOrDefault());
                        quoteQuery.Discounts.IsVoluntarilyDeductible = true;
                    }
                }
            }

            if (codeList.Any())
            {
                var codeData = codeList.FirstOrDefault();
                quoteQuery.VehicleDetails.VehicleMakeCode = codeData?.VehicleMakeCode;
                quoteQuery.VehicleDetails.VehicleModelCode = codeData.VehicleModelCode;
                quoteQuery.VehicleDetails.VehicleClass = codeData.vehicleclass;
                quoteQuery.VehicleDetails.VehicleType = codeData.VehicleType;
                quoteQuery.CityCode = codeData.CityCode;
                quoteQuery.RTOCityName = codeData.RTOCityName;
                quoteQuery.VehicleDetails.Zone = codeData.Zone;
                quoteQuery.VehicleDetails.VehicleCubicCapacity = codeData.vehicleCubicCapacity;
                quoteQuery.VehicleDetails.VehicleSeatCapacity = codeData.vehicleSeatCapacity;
                ncbValue = codeData?.NCBValue;
                quoteQuery.CurrentPolicyType = codeData.CurrentPolicyType;
                quoteQuery.IsSAODMandatry = false;
                quoteQuery.IsSATPMandatory = false;
                quoteQuery.RTOLocationCode = codeData?.RTOCode;
                quoteQuery.VehicleNumber = query.VehicleNumber;
                quoteQuery.VehicleDetails.VehicleSubTypeCode = codeData.VehicleSubTypeCode;
                quoteQuery.VehicleDetails.GrossVehicleWeight = codeData?.GrossVehicleWeight;
                quoteQuery.VehicleDetails.IsCommercialVehicle = query.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical) ? true : false;
            }
            quoteQuery.PlanType = query.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical) && codeList.FirstOrDefault().CurrentPolicyType.Equals("SATP") ? _iFFCOConfig.SATPPlanType : _iFFCOConfig.NewVehiclePlanType;
            if (!query.IsBrandNewVehicle)
            {
                quoteQuery.CurrentNCBPercentage = "0";
                quoteQuery.PreviousPolicyDetails.IsClaimInLastYear = query.PreviousPolicy.IsPreviousYearClaim;
                if (query.PreviousPolicy != null && query.PreviousPolicy.IsPreviousPolicy)
                {
                    quoteQuery.PreviousPolicyDetails = new PreviousPolicy
                    {
                        IsPreviousInsurerKnown = query.PreviousPolicy.IsPreviousPolicy,
                        PreviousInsurerCode = previousInsurerCode,
                        IsClaimInLastYear = query.PreviousPolicy.IsPreviousYearClaim,
                        PreviousNoClaimBonus = ncbValue
                    };
                    if (!query.PreviousPolicy.IsPreviousYearClaim && !quoteQuery.CurrentPolicyType.Equals("SATP"))
                    {
                        quoteQuery.CurrentNCBPercentage = GetCurrentNCB(quoteQuery.PreviousPolicyDetails.PreviousNoClaimBonus);
                    }
                    else
                    {
                        quoteQuery.CurrentNCBPercentage = "0";
                    }
                }

                if (!query.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                {
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyType = codeList.FirstOrDefault().PreviousPolicyType;
                    quoteQuery.PreviousPolicyDetails.PreviousSAODInsurer = codeList.FirstOrDefault().SAODInsurer;
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = query.PolicyDates.ODPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd") : null;
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = query.PolicyDates.ODPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd") : null;

                    if (quoteQuery.PreviousPolicyDetails.PreviousPolicyType.Equals("SAOD"))
                    {
                        quoteQuery.PreviousPolicyDetails.PreviousSATPInsurer = codeList.FirstOrDefault().SATPInsurer;
                        quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP = query.PolicyDates.TPPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd") : null;
                        quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP = query.PolicyDates.TPPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd") : null;
                        quoteQuery.IsSATPMandatory = true;
                        quoteQuery.IsSAODMandatry = true;
                        quoteQuery.PlanType = _iFFCOConfig.SAODPlanType;
                    }
                    else if (codeList.FirstOrDefault().CurrentPolicyType.Equals("SATP"))
                    {
                        quoteQuery.IsSATPMandatory = true;
                        quoteQuery.PlanType = _iFFCOConfig.SATPPlanType;
                    }
                    else
                    {
                        quoteQuery.IsSAODMandatry = true;
                        quoteQuery.PlanType = _iFFCOConfig.ComprehensivePlanType;
                    }
                }
                else
                {
                    if (query.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SATP))
                    {
                        quoteQuery.PlanType = _iFFCOConfig.SATPPlanType;
                        quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = quoteQuery.IsBrandNewVehicle ? null : Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd");
                        quoteQuery.IsSATPMandatory = true;
                    }
                    else if (query.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive))
                    {
                        quoteQuery.PlanType = _iFFCOConfig.ComprehensivePlanType;
                        quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = quoteQuery.IsBrandNewVehicle ? null : Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd");
                        quoteQuery.IsSAODMandatry = true;
                    }
                    else
                    {
                        quoteQuery.PlanType = _iFFCOConfig.NewVehiclePlanType;
                    }
                }
            }

            quoteQuery.PolicyStartDate = query.PolicyDates.PolicyStartDate;
            quoteQuery.PolicyEndDate = Convert.ToDateTime(query.PolicyDates.PolicyEndDate).AddDays(1).ToString("dd-MMM-yyyy");
            quoteQuery.RegistrationDate = query.PolicyDates.RegistrationDate;
            quoteQuery.VehicleODTenure = query.PolicyDates.VehicleODTenure;
            quoteQuery.VehicleTPTenure = query.PolicyDates.VehicleTPTenure;
            quoteQuery.SelectedIDV = query.IDV;
            quoteQuery.RegistrationYear = query.RegistrationYear;
            quoteQuery.VehicleDetails.IsTwoWheeler = query.PolicyDates.IsTwoWheeler;
            quoteQuery.VehicleDetails.IsFourWheeler = query.PolicyDates.IsFourWheeler;
            quoteQuery.IsBrandNewVehicle = query.IsBrandNewVehicle;
            quoteQuery.CategoryId = query.CategoryId;
            return quoteQuery;
        }
        private static string GetCurrentNCB(string previousNCB)
        {
            string currentNCB = string.Empty;
            if (previousNCB.Equals("0"))
            {
                currentNCB = "20";
            }
            else if (previousNCB.Equals("20"))
            {
                currentNCB = "25";
            }
            else if (previousNCB.Equals("25"))
            {
                currentNCB = "35";
            }
            else if (previousNCB.Equals("35"))
            {
                currentNCB = "45";
            }
            else if (previousNCB.Equals("45") || previousNCB.Equals("50"))
            {
                currentNCB = "50";
            }
            return currentNCB;
        }

        public async Task UpdateIFFCOProposalId(string quoteTransactionId, string proposalNumber, CancellationToken cancellationToken)
        {
            using var connections = _applicationDBContext.CreateConnection();
            var parameter = new DynamicParameters();
            parameter.Add("QuoteTransactionId", quoteTransactionId, DbType.String, ParameterDirection.Input);
            parameter.Add("ProposalNumber", proposalNumber, DbType.String, ParameterDirection.Input);

            var iuserIdDetails = await connections.ExecuteAsync("[dbo].[Insurance_UpdateIFFCOProposalId]",
                parameter,
                commandType: CommandType.StoredProcedure);
        }
        public async Task<VariantAndRTOIdCheckModel> DoesIFFCOVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, CancellationToken cancellationToken)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("VariantId", variantId, DbType.String, ParameterDirection.Input);
            parameters.Add("RTOId", rtoId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_DoesIFFCOVariantAndRTOExists]",
                parameters,
                commandType: CommandType.StoredProcedure);

            var response = result.Read<VariantAndRTOIdCheckModel>();
            return response.FirstOrDefault();
        }
    }
}
