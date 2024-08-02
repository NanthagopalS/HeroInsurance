using AutoMapper;
using Dapper;
using DnsClient.Internal;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.InsuranceMaster;
using Insurance.Core.Features.Quote.Command.SaveUpdateLead;
using Insurance.Domain;
using Insurance.Domain.Bajaj;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.ICICI;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Oriental;
using Insurance.Domain.Quote;
using Insurance.Domain.Reliance;
using Insurance.Domain.TATA;
using Insurance.Domain.UnitedIndia;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Text;
using ThirdPartyUtilities.Abstraction;
using CashlessGarage = Insurance.Domain.GoDigit.CashlessGarage;

namespace Insurance.Persistence.Repository
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IICICIService _iCICIService;
        private readonly IGodigitService _godigitService;
        private readonly IBajajService _bajajService;
        private readonly ICholaService _cholaService;
        private readonly IHDFCService _hDFCService;
        private readonly IIFFCOService _IFFCOService;
        private readonly ITATAService _tataService;
        private readonly InsurerIdConfig _insurerConfig;
        private readonly IMapper _mapper;
        private readonly IMongoDBService _mongoDBService;
        private readonly ILogger<QuoteRepository> _logger;
        private readonly IApplicationClaims _applicationClaims;
        private readonly LogoConfig _logoConfig;
        private readonly IdentityApplicationDBContext _identityApplicationDBContext;
        private readonly GoDigitConfig _goDigitConfig;
        private readonly CholaConfig _cholaConfig;
        private readonly VehicleTypeConfig _vehicleTypeConfig;
        private readonly ICommonService _commonService;
        public QuoteRepository(ApplicationDBContext context,
            IICICIService iCICIService,
            IGodigitService godigitService,
            IBajajService bajajService,
            ICholaService cholaService,
            IHDFCService hDFCService,
            IOptions<InsurerIdConfig> insurerConfig,
            IMapper mapper, IMongoDBService mongoDBService, ILogger<QuoteRepository> logger, IApplicationClaims applicationClaims,
            IOptions<LogoConfig> options,
            IdentityApplicationDBContext identityApplicationDBContext,
            IOptions<GoDigitConfig> goDigitConfig, IOptions<CholaConfig> option,
            IOptions<VehicleTypeConfig> vehicleTypeConfig,
            IIFFCOService iFFCOService,
            ITATAService tataService,
            ICommonService commonService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _iCICIService = iCICIService ?? throw new ArgumentNullException(nameof(iCICIService));
            _godigitService = godigitService ?? throw new ArgumentNullException(nameof(godigitService));
            _bajajService = bajajService ?? throw new ArgumentNullException(nameof(bajajService));
            _cholaService = cholaService ?? throw new ArgumentNullException(nameof(cholaService));
            _hDFCService = hDFCService ?? throw new ArgumentNullException(nameof(hDFCService));
            _insurerConfig = insurerConfig?.Value;
            _mapper = mapper;
            _mongoDBService = mongoDBService;
            _logger = logger;
            _applicationClaims = applicationClaims;
            _logoConfig = options.Value;
            _identityApplicationDBContext = identityApplicationDBContext ?? throw new ArgumentException(nameof(identityApplicationDBContext));
            _goDigitConfig = goDigitConfig.Value;
            _cholaConfig = option.Value;
            _vehicleTypeConfig = vehicleTypeConfig.Value;
            _IFFCOService = iFFCOService;
            _tataService = tataService ?? throw new ArgumentNullException(nameof(tataService));
            _commonService = commonService;
            //_hdfcRepository = hdfcRepository ?? throw new ArgumentNullException(nameof(hdfcRepository));
        }

        /// <summary>
        /// AddLeadAddress
        /// </summary>
        /// <param name="leadDetail"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns> 
        public async Task<bool> AddLeadAddress(CreateLeadModel leadDetail, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("LeadID", leadDetail.LeadID, DbType.String, ParameterDirection.Input);
            parameters.Add("ComAddress1", leadDetail.CommunicationAddress?.Address1, DbType.String, ParameterDirection.Input);
            parameters.Add("ComAddress2", leadDetail.CommunicationAddress?.Address2, DbType.String, ParameterDirection.Input);
            parameters.Add("ComAddress3", leadDetail.CommunicationAddress?.Address3, DbType.String, ParameterDirection.Input);
            parameters.Add("ComPincode", leadDetail.CommunicationAddress?.Pincode, DbType.String, ParameterDirection.Input);
            parameters.Add("PermAddress1", leadDetail.PermanentAddress?.Address1, DbType.String, ParameterDirection.Input);
            parameters.Add("PermAddress2", leadDetail.PermanentAddress?.Address2, DbType.String, ParameterDirection.Input);
            parameters.Add("PermAddress3", leadDetail.PermanentAddress?.Address3, DbType.String, ParameterDirection.Input);
            parameters.Add("PermPincode", leadDetail.PermanentAddress?.Pincode, DbType.String, ParameterDirection.Input);
            _ = await connection.QueryAsync<VariantModel>("[dbo].[Insurance_InsertLeadAddress]",
                                                                 parameters,
                                                                 commandType: CommandType.StoredProcedure);

            return true;
        }

        /// <summary>
        /// AddLeadNominee
        /// </summary>
        /// <param name="leadDetail"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns> 
        public async Task<bool> AddLeadNominee(CreateLeadModel leadDetail, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("LeadID", leadDetail.LeadID, DbType.String, ParameterDirection.Input);
            parameters.Add("FirstName", leadDetail.Nominees?.FirstName, DbType.String, ParameterDirection.Input);
            parameters.Add("LastName", leadDetail.Nominees?.LastName, DbType.String, ParameterDirection.Input);
            parameters.Add("DOB", leadDetail.Nominees?.DOB, DbType.String, ParameterDirection.Input);
            parameters.Add("Age", leadDetail.Nominees?.Age, DbType.Int32, ParameterDirection.Input);
            parameters.Add("Relationship", leadDetail.Nominees?.Relationship, DbType.String, ParameterDirection.Input);
            _ = await connection.QueryAsync<VariantModel>("[dbo].[Insurance_InsertLeadNominee]",
                                                                 parameters,
                                                                 commandType: CommandType.StoredProcedure);

            return true;
        }

        /// <summary>
        /// GetProposalSummary
        /// </summary>
        /// <param name="leadDetailQuery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProposalFieldMasterModel>> GetProposalSummary(string insurerId,
                                                              string variantId,
                                                              string vehicleNumber,
                                                              string quoteTransactionId,
                                                              CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", insurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("VariantId", variantId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("QuoteTransactionId", quoteTransactionId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<string>("[dbo].[Insurance_GetProposalSummary]",
                parameters,
                commandType: CommandType.StoredProcedure);
            var proposalData = result.FirstOrDefault();

            var proposalFields = await GetProposalFields(insurerId, quoteTransactionId, cancellationToken);
            if (proposalData != null)
            {
                if (_insurerConfig.Bajaj.Equals(insurerId))
                {
                    BindBajajProposalData(proposalData, proposalFields);
                }
                if (_insurerConfig.ICICI.Equals(insurerId))
                {
                    BindICICIProposalData(proposalData, proposalFields);
                }
                if (_insurerConfig.GoDigit.Equals(insurerId))
                {
                    BindGoDigitProposalData(proposalData, proposalFields);
                }
                if (_insurerConfig.HDFC.Equals(insurerId))
                {
                    BindHDFCProposalData(proposalData, proposalFields);
                }
                if (_insurerConfig.Chola.Equals(insurerId))
                {
                    BindCholaProposalData(proposalData, proposalFields);
                }
                if (_insurerConfig.TATA.Equals(insurerId))
                {
                    BindTATAProposalData(proposalData, proposalFields);
                }
                if (_insurerConfig.Reliance.Equals(insurerId))
                {
                    BindRelianceProposalData(proposalData, proposalFields);
                }
                if (_insurerConfig.IFFCO.Equals(insurerId))
                {
                    BindIFFCOProposalData(proposalData, proposalFields);
                }
                if (_insurerConfig.Oriental.Equals(insurerId))
                {
                    BindOrientalProposalData(proposalData, proposalFields);
                }
                if (_insurerConfig.UnitedIndia.Equals(insurerId))
                {
                    BindUnitedIndiaProposalData(proposalData, proposalFields);
                }
            }
            return proposalFields;
        }

        /// <summary>
        /// GetProposalFields
        /// </summary>
        /// <param name="InsurerID"></param>
        /// <param name="quotetransactionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProposalFieldMasterModel>> GetProposalFields(string InsurerID, string quotetransactionId, CancellationToken cancellationToken)
        {
            var proposalFieldList = new List<ProposalFieldMasterModel>();

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerID", InsurerID, DbType.String, ParameterDirection.Input);
            parameters.Add("QuotetransactionId", quotetransactionId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetProposalFields]",
            parameters, commandType: CommandType.StoredProcedure);

            var dynamicFieldList = result.Read<ProposalFieldModel>();

            if (dynamicFieldList != null && dynamicFieldList.Any())
            {
                proposalFieldList = dynamicFieldList.Select(item => new ProposalFieldMasterModel
                {
                    FieldKey = item.FieldKey,
                    FieldText = item.FieldText,
                    FieldType = item.FieldType,
                    Section = item.Section,
                    IsMaster = item.IsMaster,
                    Validation = item.Validation == null ? null : JsonConvert.DeserializeObject<List<NameValueModel>>(item.Validation),
                    IsMandatory = item.IsMandatory,
                    MasterRef = item.MasterRef,
                    ColumnRef = item.ColumnRef,
                    DefaultValue = item.DefaultValue
                }).ToList();

                var masterList = proposalFieldList.Where(x => x.IsMaster).ToList();
                foreach (var item in masterList)
                {
                    if (!string.IsNullOrWhiteSpace(item.MasterRef))
                    {
                        var res = await connection.QueryAsync<NameValueModel>(item.MasterRef).ConfigureAwait(false);
                        proposalFieldList.Where(x => x.FieldKey == item.FieldKey).ToList()
                            .ForEach(d => d.MasterData = res);

                        foreach (var values in res)
                        {
                            if (values.ValidationObject != null)
                            {
                                values.Validation = JsonConvert.DeserializeObject<List<NameValueModel>>(values.ValidationObject);
                            }
                            values.ValidationObject = null;
                        }
                    }
                }
            }
            return proposalFieldList;
        }

        /// <summary>
        /// ProposalRequest
        /// </summary>
        /// <param name="createLeadModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SaveUpdateLeadVm> ProposalRequest(ProposalRequestModel proposalRequestModel, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("QuoteTransactionID", proposalRequestModel.QuoteTransactionID, DbType.String, ParameterDirection.Input);
            parameters.Add("RequestBody", proposalRequestModel.RequestBody, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleNumber", proposalRequestModel.VehicleNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("VariantId", proposalRequestModel.VariantId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_SaveUpdateProposal]", parameters,
                     commandType: CommandType.StoredProcedure);

            SaveUpdateLeadVm resProposal = result.Read<SaveUpdateLeadVm>()?.FirstOrDefault();

            _ = await UpdateProposalLeadDetails(resProposal.InsurerId, proposalRequestModel.QuoteTransactionID, proposalRequestModel.RequestBody, proposalRequestModel.IsProposal);

            return resProposal;
        }

        private async Task<string> UpdateProposalLeadDetails(string insurerId, string quoteTransactionId, string proposalRequestModel, bool isProposal)
        {
            ProposalLeadDetails proposalLeadDetails = new ProposalLeadDetails();

            switch (insurerId)
            {
                case ("78190CB2-B325-4764-9BD9-5B9806E99621"):
                    ProposalRequest godigitProposalRequest = JsonConvert.DeserializeObject<ProposalRequest>(proposalRequestModel);
                    proposalLeadDetails = _mapper.Map<ProposalLeadDetails>(godigitProposalRequest);
                    break;
                case ("0A326B77-AFD5-44DA-9871-1742624CFF16"):
                    HDFCProposalRequest hdfcProposalRequest = JsonConvert.DeserializeObject<HDFCProposalRequest>(proposalRequestModel);
                    proposalLeadDetails = _mapper.Map<ProposalLeadDetails>(hdfcProposalRequest);
                    break;
                case ("FD3677E5-7938-46C8-9CD2-FAE188A1782C"):
                    ICICIProposalRequest iCICIProposalRequest = JsonConvert.DeserializeObject<ICICIProposalRequest>(proposalRequestModel);
                    proposalLeadDetails = _mapper.Map<ProposalLeadDetails>(iCICIProposalRequest);
                    break;
                case ("16413879-6316-4C1E-93A4-FF8318B14D37"):
                    BajajProposalRequest BajajProposalRequest = JsonConvert.DeserializeObject<BajajProposalRequest>(proposalRequestModel);
                    proposalLeadDetails = _mapper.Map<ProposalLeadDetails>(BajajProposalRequest);
                    break;
                case ("77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA"):
                    CholaProposalRequest cholaProposalRequest = JsonConvert.DeserializeObject<CholaProposalRequest>(proposalRequestModel);
                    proposalLeadDetails = _mapper.Map<ProposalLeadDetails>(cholaProposalRequest);
                    break;
                case ("372B076C-D9D9-48DC-9526-6EB3D525CAB6"):
                    RelianceProposalRequest relianceProposalRequest = JsonConvert.DeserializeObject<RelianceProposalRequest>(proposalRequestModel);
                    proposalLeadDetails = _mapper.Map<ProposalLeadDetails>(relianceProposalRequest);
                    break;
                case ("E656D5D1-5239-4E48-9048-228C67AE3AC3"):
                    IFFCOProposalDynamicDetails iFFCOProposalRequest = JsonConvert.DeserializeObject<IFFCOProposalDynamicDetails>(proposalRequestModel);
                    proposalLeadDetails = _mapper.Map<ProposalLeadDetails>(iFFCOProposalRequest);
                    break;
                case ("85F8472D-8255-4E80-B34A-61DB8678135C"):
                    TATAProposalRequest tataProposalRequest = JsonConvert.DeserializeObject<TATAProposalRequest>(proposalRequestModel);
                    proposalLeadDetails = _mapper.Map<ProposalLeadDetails>(tataProposalRequest);
                    break;
                case ("5A97C9A3-1CFA-4052-8BA2-6294248EF1E9"):
                    OrientalProposalDynamicDetail orientalProposalDynamicDetail = JsonConvert.DeserializeObject<OrientalProposalDynamicDetail>(proposalRequestModel);
                    proposalLeadDetails = _mapper.Map<ProposalLeadDetails>(orientalProposalDynamicDetail);
                    break;
                case ("DC874A12-6667-41AB-A7A1-3BB832B59CEB"):
                    UnitedProposalDynamicDetail unitedProposalDynamicDetail = JsonConvert.DeserializeObject<UnitedProposalDynamicDetail>(proposalRequestModel);
                    proposalLeadDetails = _mapper.Map<ProposalLeadDetails>(unitedProposalDynamicDetail);
                    break;
                default:
                    break;
            }

            string dateOfBirth = string.IsNullOrEmpty(proposalLeadDetails.DOB) ?
                null : Convert.ToDateTime(proposalLeadDetails.DOB).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);


            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("QuoteTransactionID", quoteTransactionId, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", insurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("FirstName", proposalLeadDetails.FirstName, DbType.String, ParameterDirection.Input);
            parameters.Add("MiddleName", proposalLeadDetails.MiddleName, DbType.String, ParameterDirection.Input);
            parameters.Add("LastName", proposalLeadDetails.LastName, DbType.String, ParameterDirection.Input);
            parameters.Add("PhoneNumber", proposalLeadDetails.PhoneNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("Email", proposalLeadDetails.Email, DbType.String, ParameterDirection.Input);
            parameters.Add("CompanyName", proposalLeadDetails.CompanyName, DbType.String, ParameterDirection.Input);
            parameters.Add("GSTNumber", proposalLeadDetails.GSTNo, DbType.String, ParameterDirection.Input);
            parameters.Add("DOB", dateOfBirth, DbType.String, ParameterDirection.Input);
            parameters.Add("Gender", proposalLeadDetails.Gender, DbType.String, ParameterDirection.Input);
            parameters.Add("NomineeFirstName", proposalLeadDetails.NomineeFirstName, DbType.String, ParameterDirection.Input);
            parameters.Add("NomineeLastName", proposalLeadDetails.NomineeLastName, DbType.String, ParameterDirection.Input);
            parameters.Add("NomineeDOB", proposalLeadDetails.NomineeDOB, DbType.String, ParameterDirection.Input);
            parameters.Add("NomineeAge", string.IsNullOrEmpty(proposalLeadDetails.NomineeAge) ? 0 : Convert.ToInt32(proposalLeadDetails.NomineeAge), DbType.String, ParameterDirection.Input);
            parameters.Add("NomineeRelation", proposalLeadDetails.NomineeRelation, DbType.String, ParameterDirection.Input);
            parameters.Add("AddressType", proposalLeadDetails.AddressType, DbType.String, ParameterDirection.Input);
            parameters.Add("Perm_Address1", proposalLeadDetails.Perm_Address1, DbType.String, ParameterDirection.Input);
            parameters.Add("Perm_Address2", proposalLeadDetails.Perm_Address2, DbType.String, ParameterDirection.Input);
            parameters.Add("Perm_Address3", proposalLeadDetails.Perm_Address3, DbType.String, ParameterDirection.Input);
            parameters.Add("Perm_Pincode", proposalLeadDetails.Perm_Pincode, DbType.String, ParameterDirection.Input);
            parameters.Add("Perm_City", proposalLeadDetails.Perm_City, DbType.String, ParameterDirection.Input);
            parameters.Add("Perm_State", proposalLeadDetails.Perm_State, DbType.String, ParameterDirection.Input);
            parameters.Add("IsFinancier", proposalLeadDetails.IsFinancier, DbType.String, ParameterDirection.Input);
            parameters.Add("FinancierName", proposalLeadDetails.FinancierName, DbType.String, ParameterDirection.Input);
            parameters.Add("FinancierBranch", proposalLeadDetails.FinancierBranch, DbType.String, ParameterDirection.Input);
            parameters.Add("EngineNumber", proposalLeadDetails.EngineNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("ChassisNumber", proposalLeadDetails.ChassisNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("IsProposal", isProposal, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("PanNumber", proposalLeadDetails.PanNumber, DbType.String, ParameterDirection.Input);

            var saveData = await connection.QueryMultipleAsync("[dbo].[Insurance_UpdateProposalLeadDetails]", parameters,
                         commandType: CommandType.StoredProcedure);

            var response = saveData.Read<string>().FirstOrDefault();
            return response;
        }
        /// <summary>
        /// GetQuoteTransactionDetails
        /// </summary>
        /// <param name="transactionID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<QuoteTransactionDbModel> GetQuoteTransactionDetails(string transactionID, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("TransactionID", transactionID, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetQuoteTransactionDetails]",
                parameters,
                commandType: CommandType.StoredProcedure);

            QuoteTransactionDbModel quoteTransactionDbModel = new QuoteTransactionDbModel();

            quoteTransactionDbModel.QuoteTransactionRequest = result.Read<QuoteTransactionRequest>()?.FirstOrDefault();
            quoteTransactionDbModel.LeadDetail = result.Read<CreateLeadModel>().FirstOrDefault();
            quoteTransactionDbModel.ProposalRequestBody = result.Read<string>()?.FirstOrDefault();
            quoteTransactionDbModel.CKYCRequestBody = result.Read<string>()?.FirstOrDefault();
            quoteTransactionDbModel.LeadDetail.VehicleCubicCapacity = result?.Read<string>()?.FirstOrDefault();
            quoteTransactionDbModel.LeadDetail.kyc_id = result.Read<string>()?.FirstOrDefault();
            quoteTransactionDbModel.QuoteConfirmDetailsModel = result.Read<QuoteConfirmDetailsModel>()?.FirstOrDefault();
            return quoteTransactionDbModel;
        }

        /// <summary>
        /// GetQuoteConfirmDetails
        /// </summary>
        /// <param name="transactionID"></param>
        /// <param name="quoteConfirmCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> GetQuoteConfirmDetails(string transactionID, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
        {
            var response = (dynamic)null;
            string oldResponse = string.Empty;

            var getTransaction = await GetQuoteTransactionDetails(transactionID, cancellationToken);

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("PolicyTypeId", quoteConfirmCommand.PolicyDates.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleNumber", quoteConfirmCommand.VehicleNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("PreviousSAODInsurerId", quoteConfirmCommand.PreviousPolicy.SAODInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("PreviousSATPInsurerId", quoteConfirmCommand.PreviousPolicy.TPInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("NCBId", quoteConfirmCommand.PreviousPolicy.NCBId, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", getTransaction.QuoteTransactionRequest.InsurerId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<QuoteConfirmDetailsModel>("[dbo].[Insurance_GetQuoteConfirmDetails]",
                parameters,
                commandType: CommandType.StoredProcedure);

            getTransaction.QuoteConfirmDetailsModel = result.FirstOrDefault();


            switch (getTransaction.QuoteTransactionRequest.InsurerId)
            {
                case ("FD3677E5-7938-46C8-9CD2-FAE188A1782C"):
                    response = await _iCICIService.QuoteConfirmDetails(getTransaction, quoteConfirmCommand, cancellationToken);
                    break;
                case ("78190CB2-B325-4764-9BD9-5B9806E99621"):
                    response = await _godigitService.QuoteConfirmDetails(getTransaction, quoteConfirmCommand, cancellationToken);
                    break;
                case ("77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA"):
                    response = await _cholaService.QuoteConfirmDetails(getTransaction, quoteConfirmCommand, cancellationToken);
                    break;
                case ("16413879-6316-4C1E-93A4-FF8318B14D37"):
                    response = await _bajajService.QuoteConfirmDetails(getTransaction, quoteConfirmCommand, cancellationToken);
                    break;
                case ("0A326B77-AFD5-44DA-9871-1742624CFF16"):
                    response = await _hDFCService.QuoteConfirmDetails(getTransaction, quoteConfirmCommand, cancellationToken);
                    break;
            }

            oldResponse = getTransaction.QuoteTransactionRequest.CommonResponse;
            QuoteResponseModel getOldResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(oldResponse);


            response.Item1.NewPremium = RoundOffValue(response.Item1.NewPremium);
            response.Item1.OldPremium = RoundOffValue(getOldResponse.GrossPremium);
            response.Item1.Logo = response.Item2.InsurerLogo;

            return response;
        }
        private static string RoundOffValue(string _val)
        {
            decimal val = Math.Round(Convert.ToDecimal(_val));
            return val.ToString();
        }

        /// <summary>
        /// GetCKYCFields
        /// </summary>
        /// <param name="InsurerID"></param>
        /// <param name="isPOI"></param>
        /// <param name="isCompany"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProposalFieldMasterModel>> GetCKYCFields(string InsurerID, bool isPOI, bool isCompany, bool isDocumentUpload, CancellationToken cancellationToken)
        {
            var cKYCFieldList = new List<ProposalFieldMasterModel>();

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerID", InsurerID, DbType.String, ParameterDirection.Input);
            parameters.Add("IsPOI", isPOI, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("IsCompany", isCompany, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("IsDocumentUpload", isDocumentUpload, DbType.Boolean, ParameterDirection.Input);

            var result = await connection.QueryAsync<ProposalFieldModel>("[dbo].[Insurance_GetCKYCFields]",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (result != null && result.Any())
            {
                cKYCFieldList = result.Select(item => new ProposalFieldMasterModel
                {
                    FieldKey = item.FieldKey,
                    FieldText = item.FieldText,
                    FieldType = item.FieldType,
                    Section = item.Section,
                    IsMaster = item.IsMaster,
                    Validation = item.Validation == null || item.FieldKey == "poaDocumentUpload" || item.FieldKey == "poiDocumentUpload" || item.FieldKey == "poaDocumentUploadFront" || item.FieldKey == "poaDocumentUploadBack" ? null : JsonConvert.DeserializeObject<List<NameValueModel>>(item.Validation),
                    IsMandatory = item.IsMandatory,
                    MasterRef = item.MasterRef,
                    ColumnRef = item.ColumnRef,
                    AcceptedFormat = item.FieldKey == "poaDocumentUpload" || item.FieldKey == "poiDocumentUpload" || item.FieldKey == "poaDocumentUploadFront" || item.FieldKey == "poaDocumentUploadBack" ? item.Validation : null
                }).ToList();

                var masterList = cKYCFieldList.Where(x => x.IsMaster).ToList();
                foreach (var item in masterList)
                {
                    using var dbConnection = _context.CreateConnection();
                    var dbParameters = new DynamicParameters();
                    var data = item.MasterRef.Split(" ");
                    dbParameters.Add("Type", data[1], DbType.String, ParameterDirection.Input);
                    dbParameters.Add("InsurerID", InsurerID, DbType.String, ParameterDirection.Input);
                    dbParameters.Add("IsCompany", isCompany, DbType.Boolean, ParameterDirection.Input);

                    var res = await dbConnection.QueryAsync<NameValueModel>(data[0],
                    dbParameters,
                    commandType: CommandType.StoredProcedure);

                    cKYCFieldList.Where(x => x.FieldKey == item.FieldKey).ToList()
                        .ForEach(d => d.MasterData = res);

                    foreach (var values in res)
                    {
                        if (values.ValidationObject != null)
                        {
                            values.Validation = JsonConvert.DeserializeObject<List<NameValueModel>>(values.ValidationObject);
                        }
                        values.ValidationObject = null;
                    }
                }
                foreach (var item in cKYCFieldList)
                {
                    if (item.FieldKey.Equals("ispoliticallyExposedPerson"))
                    {
                        item.DefaultValue = "false";
                    }
                }
            }
            return cKYCFieldList;
        }

        /// <summary>
        /// GetCKYCDocumenFields
        /// </summary>
        /// <param name="insurerId"></param>
        /// <param name="documentName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProposalFieldMasterModel>> GetCKYCDocumenFields(string insurerId, string documentName, CancellationToken cancellationToken)
        {
            var cKYCFieldList = new List<ProposalFieldMasterModel>();

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerID", insurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("DocumentName", documentName, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<ProposalFieldModel>("[dbo].[Insurance_GetCKYCDocumentField]",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (result != null && result.Any())
            {
                cKYCFieldList = result.Select(item => new ProposalFieldMasterModel
                {
                    FieldKey = item.FieldKey,
                    FieldText = item.FieldText,
                    FieldType = item.FieldType,
                    Section = item.Section,
                    IsMaster = item.IsMaster,
                    Validation = item.Validation == null ? null : JsonConvert.DeserializeObject<List<NameValueModel>>(item.Validation),
                    IsMandatory = item.IsMandatory,
                    MasterRef = item.MasterRef,
                    ColumnRef = item.ColumnRef
                }).ToList();

                var masterList = cKYCFieldList.Where(x => x.IsMaster).ToList();
                foreach (var item in masterList)
                {
                    using var dbConnection = _context.CreateConnection();
                    var dbParameters = new DynamicParameters();
                    var data = item.MasterRef.Split(" ");
                    dbParameters.Add("Type", data[1], DbType.String, ParameterDirection.Input);
                    dbParameters.Add("InsurerID", insurerId, DbType.String, ParameterDirection.Input);

                    var res = await dbConnection.QueryAsync<NameValueModel>(data[0],
                    dbParameters,
                    commandType: CommandType.StoredProcedure);

                    cKYCFieldList.Where(x => x.FieldKey == item.FieldKey).ToList()
                        .ForEach(d => d.MasterData = res);
                }
            }
            return cKYCFieldList;
        }
        public async Task<SaveCKYCResponse> SaveLeadDetails(string insurerId, string quoteTransactionId, string kycRequest, string kycResponse, string stage, CreateLeadModel createLeadModel, CancellationToken cancellationToken)
        {

            string dateOfBirth = string.IsNullOrEmpty(createLeadModel.DOB) ?
               null : Convert.ToDateTime(createLeadModel.DOB).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dateOfIncorporation = string.IsNullOrEmpty(createLeadModel.DateOfIncorporation) ?
                null : Convert.ToDateTime(createLeadModel.DateOfIncorporation).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("QuoteTransactionID", quoteTransactionId, DbType.String, ParameterDirection.Input);
            parameters.Add("RequestBody", kycRequest, DbType.String, ParameterDirection.Input);
            parameters.Add("ResponseBody", kycResponse, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", insurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("LeadName", createLeadModel.LeadName, DbType.String, ParameterDirection.Input);
            parameters.Add("PhoneNumber", createLeadModel.PhoneNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("Email", createLeadModel.Email, DbType.String, ParameterDirection.Input);
            parameters.Add("CompanyName", createLeadModel.CompanyName, DbType.String, ParameterDirection.Input);
            parameters.Add("DOI", dateOfIncorporation, DbType.String, ParameterDirection.Input);
            parameters.Add("GSTNo", createLeadModel.GSTNo, DbType.String, ParameterDirection.Input);
            parameters.Add("MiddleName", createLeadModel.MiddleName, DbType.String, ParameterDirection.Input);
            parameters.Add("LastName", createLeadModel.LastName, DbType.String, ParameterDirection.Input);
            parameters.Add("DOB", dateOfBirth, DbType.String, ParameterDirection.Input);
            parameters.Add("Gender", createLeadModel.Gender, DbType.String, ParameterDirection.Input);
            parameters.Add("PANNumber", createLeadModel.PANNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("AadhaarNumber", createLeadModel.AadharNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("PhotoId", null, DbType.String, ParameterDirection.Input);//check
            parameters.Add("AddressType", createLeadModel.PermanentAddress?.AddressType, DbType.String, ParameterDirection.Input);
            parameters.Add("Address1", createLeadModel.PermanentAddress?.Address1, DbType.String, ParameterDirection.Input);
            parameters.Add("Address2", createLeadModel.PermanentAddress?.Address2, DbType.String, ParameterDirection.Input);
            parameters.Add("Address3", createLeadModel.PermanentAddress?.Address3, DbType.String, ParameterDirection.Input);
            parameters.Add("Pincode", createLeadModel.PermanentAddress?.Pincode, DbType.String, ParameterDirection.Input);
            parameters.Add("Stage", stage, DbType.String, ParameterDirection.Input);
            parameters.Add("CKYCNumber", createLeadModel.ckycNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("KYCId", createLeadModel.kyc_id, DbType.String, ParameterDirection.Input);
            parameters.Add("CKYCStatus", createLeadModel.CKYCstatus, DbType.String, ParameterDirection.Input);
            parameters.Add("UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("City", createLeadModel.City, DbType.String, ParameterDirection.Input);
            parameters.Add("State", createLeadModel.State, DbType.String, ParameterDirection.Input);
            parameters.Add("Salutation", createLeadModel.Salutation, DbType.String, ParameterDirection.Input);

            var saveData = await connection.QueryMultipleAsync("[dbo].[Insurance_SaveCKYCDetails]", parameters,
                         commandType: CommandType.StoredProcedure);

            var response = saveData.Read<SaveCKYCResponse>().FirstOrDefault();
            return response;
        }
        private static void BindHDFCProposalData(string proposalData, IEnumerable<ProposalFieldMasterModel> proposalFields)
        {
            var hdfcProposalRequestBody = JsonConvert.DeserializeObject<HDFCProposalRequest>(proposalData);
            if (proposalFields != null)
            {
                foreach (var field in proposalFields)
                {
                    if (field.FieldKey.Equals("salutation"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.PersonalDetails?.salutation;
                    }
                    else if (field.FieldKey.Equals("firstName"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.PersonalDetails?.firstName;
                    }
                    else if (field.FieldKey.Equals("lastName"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.PersonalDetails?.lastName;
                    }
                    else if (field.FieldKey.Equals("dateOfBirth"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.PersonalDetails?.dateOfBirth;
                    }
                    else if (field.FieldKey.Equals("gender"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.PersonalDetails?.gender;
                    }
                    else if (field.FieldKey.Equals("mobile"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.PersonalDetails?.mobile;
                    }
                    else if (field.FieldKey.Equals("emailId"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.PersonalDetails?.emailId;
                    }
                    else if (field.FieldKey.Equals("panNumber"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.PersonalDetails?.panNumber;
                    }
                    else if (field.FieldKey.Equals("perm_address"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.AddressDetails?.perm_address;
                    }
                    else if (field.FieldKey.Equals("perm_state"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.AddressDetails?.perm_state;
                    }
                    else if (field.FieldKey.Equals("perm_city"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.AddressDetails?.perm_city;
                    }
                    else if (field.FieldKey.Equals("perm_pincode"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.AddressDetails?.perm_pincode;
                    }
                    else if (field.FieldKey.Equals("perm_mail_addCheck"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.AddressDetails?.perm_mail_addCheck;
                    }
                    else if (field.FieldKey.Equals("mail_address"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.AddressDetails?.mail_address;
                    }
                    else if (field.FieldKey.Equals("mail_state"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.AddressDetails?.mail_state;
                    }
                    else if (field.FieldKey.Equals("mail_city"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.AddressDetails?.mail_city;
                    }
                    else if (field.FieldKey.Equals("mail_pincode"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.AddressDetails?.mail_pincode;
                    }
                    else if (field.FieldKey.Equals("nomineeName"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.NomineeDetails?.nomineeName;
                    }
                    else if (field.FieldKey.Equals("nomineeAge"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.NomineeDetails?.nomineeAge;
                    }
                    else if (field.FieldKey.Equals("nomineeRelation"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.NomineeDetails?.nomineeRelation;
                    }
                    else if (field.FieldKey.Equals("engineNumber"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.VehicleDetails?.engineNumber;
                    }
                    else if (field.FieldKey.Equals("chassisNumber"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.VehicleDetails?.chassisNumber;
                    }
                    else if (field.FieldKey.Equals("isFinancier"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.VehicleDetails?.isFinancier;
                    }
                    else if (field.FieldKey.Equals("financer"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.VehicleDetails?.financer;
                    }
                    else if (field.FieldKey.Equals("branch"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.VehicleDetails?.branch;
                    }
                    else if (field.FieldKey.Equals("agreementType"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.VehicleDetails?.agreementType;
                    }
                    else if (field.FieldKey.Equals("vehicleNumber"))
                    {
                        field.DefaultValue = hdfcProposalRequestBody?.VehicleDetails?.vehicleNumber;
                    }
                }
            }
        }
        private static void BindBajajProposalData(string proposalData, IEnumerable<ProposalFieldMasterModel> proposalFields)
        {
            var bajajProposalRequestBody = JsonConvert.DeserializeObject<BajajProposalRequest>(proposalData);
            if (proposalFields != null)
            {
                foreach (var field in proposalFields)
                {
                    if (field.FieldKey.Equals("companyName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.PersonalDetails?.companyName;
                    }
                    if (field.FieldKey.Equals("firstName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.PersonalDetails?.firstName;
                    }
                    else if (field.FieldKey.Equals("middleName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.PersonalDetails?.middleName;
                    }
                    else if (field.FieldKey.Equals("lastName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.PersonalDetails?.lastName;
                    }
                    else if (field.FieldKey.Equals("dateOfBirth") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.PersonalDetails?.dateOfBirth;
                    }
                    else if (field.FieldKey.Equals("dateOfIncorporation") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.PersonalDetails?.dateOfIncorporation;
                    }
                    else if (field.FieldKey.Equals("gender") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.PersonalDetails?.gender;
                    }
                    else if (field.FieldKey.Equals("mobile") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.PersonalDetails?.mobile;
                    }
                    else if (field.FieldKey.Equals("emailId") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.PersonalDetails?.emailId;
                    }
                    else if (field.FieldKey.Equals("addLine1"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.AddressDetails?.addLine1;
                    }
                    else if (field.FieldKey.Equals("addLine2"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.AddressDetails?.addLine2;
                    }
                    else if (field.FieldKey.Equals("city"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.AddressDetails?.city;
                    }
                    else if (field.FieldKey.Equals("state"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.AddressDetails?.state;
                    }
                    else if (field.FieldKey.Equals("pincode"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.AddressDetails?.pincode;
                    }
                    else if (field.FieldKey.Equals("isFinancier"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.VehicleDetails?.isFinancier;
                    }
                    else if (field.FieldKey.Equals("financer"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.VehicleDetails?.financer;
                    }
                    else if (field.FieldKey.Equals("engineNumber"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.VehicleDetails?.engineNumber;
                    }
                    else if (field.FieldKey.Equals("chassisNumber"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.VehicleDetails?.chassisNumber;
                    }
                    else if (field.FieldKey.Equals("financierBranch"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.VehicleDetails?.financierBranch;
                    }
                    else if (field.FieldKey.Equals("nomineeFirstName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.NomineeDetails?.nomineeFirstName;
                    }
                    else if (field.FieldKey.Equals("middleName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.NomineeDetails?.middleName;
                    }
                    else if (field.FieldKey.Equals("nomineeLastName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.NomineeDetails?.nomineeLastName;
                    }
                    else if (field.FieldKey.Equals("nomineeDateOfBirth") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.NomineeDetails?.nomineeDateOfBirth;
                    }
                    else if (field.FieldKey.Equals("nomineeRelation") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.NomineeDetails?.nomineeRelation;
                    }
                    else if (field.FieldKey.Equals("nomineeAge") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.NomineeDetails?.nomineeAge;
                    }
                    else if (field.FieldKey.Equals("vehicleNumber"))
                    {
                        field.DefaultValue = bajajProposalRequestBody?.VehicleDetails?.vehicleNumber;
                    }
                }
            }
        }
        private static void BindGoDigitProposalData(string proposalData, IEnumerable<ProposalFieldMasterModel> proposalFields)
        {
            var godigitProposalRequestBody = JsonConvert.DeserializeObject<ProposalRequest>(proposalData);
            if (proposalFields != null)
            {
                foreach (var field in proposalFields)
                {
                    if (field.FieldKey.Equals("companyName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.PersonalDetails?.companyName;
                    }
                    else if (field.FieldKey.Equals("gstno") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.PersonalDetails?.gstno;
                    }
                    else if (field.FieldKey.Equals("firstName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.PersonalDetails?.firstName;
                    }
                    else if (field.FieldKey.Equals("middleName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.PersonalDetails?.middleName;
                    }
                    else if (field.FieldKey.Equals("lastName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.PersonalDetails?.lastName;
                    }
                    else if (field.FieldKey.Equals("dateOfBirth") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.PersonalDetails?.dateOfBirth;
                    }
                    else if (field.FieldKey.Equals("gender") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.PersonalDetails?.gender;
                    }
                    else if (field.FieldKey.Equals("mobile") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.PersonalDetails?.mobile;
                    }
                    else if (field.FieldKey.Equals("emailId") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.PersonalDetails?.emailId;
                    }
                    else if (field.FieldKey.Equals("flatNumber"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.AddressDetails?.flatNumber;
                    }
                    else if (field.FieldKey.Equals("streetNumber"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.AddressDetails?.streetNumber;
                    }
                    else if (field.FieldKey.Equals("street"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.AddressDetails?.street;
                    }
                    else if (field.FieldKey.Equals("city"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.AddressDetails?.city;
                    }
                    else if (field.FieldKey.Equals("state"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.AddressDetails?.state;
                    }
                    else if (field.FieldKey.Equals("pincode"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.AddressDetails?.pincode;
                    }
                    else if (field.FieldKey.Equals("district"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.AddressDetails?.district;
                    }
                    else if (field.FieldKey.Equals("country"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.AddressDetails?.country;
                    }
                    else if (field.FieldKey.Equals("isFinancier"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.VehicleDetails?.isFinancier;
                    }
                    else if (field.FieldKey.Equals("financer"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.VehicleDetails?.financer;
                    }
                    else if (field.FieldKey.Equals("engineNumber"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.VehicleDetails?.engineNumber;
                    }
                    else if (field.FieldKey.Equals("chassisNumber"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.VehicleDetails?.chassisNumber;
                    }
                    else if (field.FieldKey.Equals("vehicleNumber"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.VehicleDetails?.vehicleNumber;
                    }
                    else if (field.FieldKey.Equals("nomineeFirstName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.NomineeDetails?.nomineeFirstName;
                    }
                    else if (field.FieldKey.Equals("middleName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.NomineeDetails?.middleName;
                    }
                    else if (field.FieldKey.Equals("nomineeLastName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.NomineeDetails?.nomineeLastName;
                    }
                    else if (field.FieldKey.Equals("nomineeDateOfBirth") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.NomineeDetails?.nomineeDateOfBirth;
                    }
                    else if (field.FieldKey.Equals("nomineeRelation") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.NomineeDetails?.nomineeRelation;
                    }
                    else if (field.FieldKey.Equals("personType") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = godigitProposalRequestBody?.NomineeDetails?.personType;
                    }
                }
            }
        }
        private static void BindICICIProposalData(string proposalData, IEnumerable<ProposalFieldMasterModel> proposalFields)
        {
            var iCICIProposalRequestBody = JsonConvert.DeserializeObject<ICICIProposalRequest>(proposalData);
            if (proposalFields != null)
            {
                foreach (var field in proposalFields)
                {
                    if (field.FieldKey.Equals("customerName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.PersonalDetails?.customerName;
                    }
                    else if (field.FieldKey.Equals("dateOfBirth") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.PersonalDetails?.dateOfBirth;
                    }
                    else if (field.FieldKey.Equals("panNumber") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.PersonalDetails?.panNumber;
                    }
                    else if (field.FieldKey.Equals("aadharNumbrer") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.PersonalDetails?.aadharNumbrer;
                    }
                    else if (field.FieldKey.Equals("emailId") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.PersonalDetails?.emailId;
                    }
                    else if (field.FieldKey.Equals("mobile") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.PersonalDetails?.mobile;
                    }
                    else if (field.FieldKey.Equals("addressLine1"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.AddressDetails?.addressLine1;
                    }
                    else if (field.FieldKey.Equals("city"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.AddressDetails?.city;
                    }
                    else if (field.FieldKey.Equals("state"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.AddressDetails?.state;
                    }
                    else if (field.FieldKey.Equals("pincode"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.AddressDetails?.pincode;
                    }
                    else if (field.FieldKey.Equals("country"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.AddressDetails?.country;
                    }
                    else if (field.FieldKey.Equals("isFinancier"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.VehicleDetails?.isFinancier;
                    }
                    else if (field.FieldKey.Equals("financer"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.VehicleDetails?.financer;
                    }
                    else if (field.FieldKey.Equals("branch"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.VehicleDetails?.branch;
                    }
                    else if (field.FieldKey.Equals("engineNumber"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.VehicleDetails?.engineNumber;
                    }
                    else if (field.FieldKey.Equals("chassisNumber"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.VehicleDetails?.chassisNumber;
                    }
                    else if (field.FieldKey.Equals("vehicleNumber"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.VehicleDetails?.vehicleNumber;
                    }
                    else if (field.FieldKey.Equals("nomineeName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.NomineeDetails?.nomineeName;
                    }
                    else if (field.FieldKey.Equals("nomineeAge") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.NomineeDetails?.nomineeAge;
                    }
                    else if (field.FieldKey.Equals("nomineeRelation") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = iCICIProposalRequestBody?.NomineeDetails?.nomineeRelation;
                    }
                }
            }
        }
        private static void BindTATAProposalData(string proposalData, IEnumerable<ProposalFieldMasterModel> proposalFields)
        {
            var tataProposalRequestBody = JsonConvert.DeserializeObject<TATAProposalRequest>(proposalData);
            if (proposalFields != null)
            {
                foreach (var field in proposalFields)
                {
                    if (field.FieldKey.Equals("salutation"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.salutation;
                    }
                    else if (field.FieldKey.Equals("firstName"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.firstName;
                    }
                    else if (field.FieldKey.Equals("lastName"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.lastName;
                    }
                    else if (field.FieldKey.Equals("dateOfBirth"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.dateOfBirth;
                    }
                    else if (field.FieldKey.Equals("gender"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.gender;
                    }
                    else if (field.FieldKey.Equals("mobile"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.mobile;
                    }
                    else if (field.FieldKey.Equals("emailId"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.emailId;
                    }
                    else if (field.FieldKey.Equals("maritalStatus"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.maritalStatus;
                    }
                    else if (field.FieldKey.Equals("occupation"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.occupation;
                    }
                    else if (field.FieldKey.Equals("panNumber"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.panNumber;
                    }
                    else if (field.FieldKey.Equals("addressLine1"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.AddressDetails?.addressLine1;
                    }
                    else if (field.FieldKey.Equals("pincode"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.AddressDetails?.pincode;
                    }
                    else if (field.FieldKey.Equals("state"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.AddressDetails?.state;
                    }
                    else if (field.FieldKey.Equals("city"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.AddressDetails?.city;
                    }
                    else if (field.FieldKey.Equals("nomineeName"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.NomineeDetails?.nomineeName;
                    }
                    else if (field.FieldKey.Equals("nomineeRelation"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.NomineeDetails?.nomineeRelation;
                    }
                    else if (field.FieldKey.Equals("nomineeAge"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.NomineeDetails?.nomineeAge;
                    }
                    else if (field.FieldKey.Equals("isFinancier"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.VehicleDetails?.isFinancier;
                    }
                    else if (field.FieldKey.Equals("financer"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.VehicleDetails?.financer;
                    }
                    else if (field.FieldKey.Equals("agreementType"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.VehicleDetails?.agreementType;
                    }
                    else if (field.FieldKey.Equals("chassisNumber"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.VehicleDetails?.chassisNumber;
                    }
                    else if (field.FieldKey.Equals("engineNumber"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.VehicleDetails?.engineNumber;
                    }
                    else if (field.FieldKey.Equals("vehicleNumber"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.VehicleDetails?.vehicleNumber;
                    }
                    else if (field.FieldKey.Equals("company"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.companyName;
                    }
                    else if (field.FieldKey.Equals("gstno"))
                    {
                        field.DefaultValue = tataProposalRequestBody?.PersonalDetails?.gstno;
                    }
                }
            }
        }
        private static void BindCholaProposalData(string proposalData, IEnumerable<ProposalFieldMasterModel> proposalFields)
        {
            var cholaProposalRequestBody = JsonConvert.DeserializeObject<CholaProposalRequest>(proposalData);
            if (proposalFields != null)
            {
                foreach (var field in proposalFields)
                {
                    if (field.FieldKey.Equals("companyName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.PersonalDetails?.companyName;
                    }
                    else if (field.FieldKey.Equals("gstno") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.PersonalDetails?.gstno;
                    }
                    else if (field.FieldKey.Equals("firstName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.PersonalDetails?.firstName;
                    }
                    else if (field.FieldKey.Equals("middleName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.PersonalDetails?.middleName;
                    }
                    else if (field.FieldKey.Equals("lastName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.PersonalDetails?.lastName;
                    }
                    else if (field.FieldKey.Equals("dateOfBirth") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.PersonalDetails?.dateOfBirth;
                    }
                    else if (field.FieldKey.Equals("dateOfIncorporation") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.PersonalDetails?.dateOfIncorporation;
                    }
                    else if (field.FieldKey.Equals("gender") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.PersonalDetails?.gender;
                    }
                    else if (field.FieldKey.Equals("mobile") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.PersonalDetails?.mobile;
                    }
                    else if (field.FieldKey.Equals("emailId") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.PersonalDetails?.emailId;
                    }
                    else if (field.FieldKey.Equals("aadharNumbrer") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.PersonalDetails?.aadharNumbrer;
                    }
                    else if (field.FieldKey.Equals("addressLine1"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.AddressDetails?.addressLine1;
                    }
                    else if (field.FieldKey.Equals("pincode"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.AddressDetails?.pincode;
                    }
                    else if (field.FieldKey.Equals("city"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.AddressDetails?.city;
                    }
                    else if (field.FieldKey.Equals("state"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.AddressDetails?.state;
                    }
                    else if (field.FieldKey.Equals("communication_address"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.AddressDetails?.communication_address;
                    }
                    else if (field.FieldKey.Equals("communication_state"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.AddressDetails?.communication_state;
                    }
                    else if (field.FieldKey.Equals("communication_city"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.AddressDetails?.communication_city;
                    }
                    else if (field.FieldKey.Equals("communication_pincode"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.AddressDetails?.communication_pincode;
                    }
                    else if (field.FieldKey.Equals("isFinancier"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.VehicleDetails?.isFinancier;
                    }
                    else if (field.FieldKey.Equals("financer"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.VehicleDetails?.financer;
                    }
                    else if (field.FieldKey.Equals("engineNumber"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.VehicleDetails?.engineNumber;
                    }
                    else if (field.FieldKey.Equals("chassisNumber"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.VehicleDetails?.chassisNumber;
                    }
                    else if (field.FieldKey.Equals("nomineeName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.NomineeDetails?.nomineeName;
                    }
                    else if (field.FieldKey.Equals("nomineeDateOfBirth") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.NomineeDetails?.nomineeDateOfBirth;
                    }
                    else if (field.FieldKey.Equals("nomineeRelation") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.NomineeDetails?.nomineeRelation;
                    }
                    else if (field.FieldKey.Equals("branch"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.VehicleDetails?.branch;
                    }
                    else if (field.FieldKey.Equals("vehicleNumber"))
                    {
                        field.DefaultValue = cholaProposalRequestBody?.VehicleDetails?.vehicleNumber;
                    }
                }
            }
        }
        private static void BindRelianceProposalData(string proposalData, IEnumerable<ProposalFieldMasterModel> proposalFields)
        {
            var relianceProposalRequestBody = JsonConvert.DeserializeObject<RelianceProposalRequest>(proposalData);
            if (proposalFields != null)
            {
                foreach (var field in proposalFields)
                {
                    if (field.FieldKey.Equals("companyName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.companyName;
                    }
                    else if (field.FieldKey.Equals("gstno") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.gstno;
                    }
                    else if (field.FieldKey.Equals("salutation") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.salutation;
                    }
                    else if (field.FieldKey.Equals("occupation") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.occupation;
                    }
                    else if (field.FieldKey.Equals("firstName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.firstName;
                    }
                    else if (field.FieldKey.Equals("middleName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.middleName;
                    }
                    else if (field.FieldKey.Equals("lastName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.lastName;
                    }
                    else if (field.FieldKey.Equals("dateOfBirth") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.dateOfBirth;
                    }
                    else if (field.FieldKey.Equals("dateOfIncorporation") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.dateOfIncorporation;
                    }
                    else if (field.FieldKey.Equals("gender") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.gender;
                    }
                    else if (field.FieldKey.Equals("mobile") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.mobile;
                    }
                    else if (field.FieldKey.Equals("emailId") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.emailId;
                    }
                    else if (field.FieldKey.Equals("aadharNumbrer") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.PersonalDetails?.aadharNumbrer;
                    }
                    else if (field.FieldKey.Equals("addressLine1"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.AddressDetails?.addressLine1;
                    }
                    else if (field.FieldKey.Equals("pincode"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.AddressDetails?.pincode;
                    }
                    else if (field.FieldKey.Equals("city"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.AddressDetails?.city;
                    }
                    else if (field.FieldKey.Equals("state"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.AddressDetails?.state;
                    }
                    else if (field.FieldKey.Equals("communication_address"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.AddressDetails?.communication_address;
                    }
                    else if (field.FieldKey.Equals("communication_state"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.AddressDetails?.communication_state;
                    }
                    else if (field.FieldKey.Equals("communication_city"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.AddressDetails?.communication_city;
                    }
                    else if (field.FieldKey.Equals("communication_pincode"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.AddressDetails?.communication_pincode;
                    }
                    else if (field.FieldKey.Equals("isFinancier"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.VehicleDetails?.isFinancier;
                    }
                    else if (field.FieldKey.Equals("financer"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.VehicleDetails?.financer;
                    }
                    else if (field.FieldKey.Equals("engineNumber"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.VehicleDetails?.engineNumber;
                    }
                    else if (field.FieldKey.Equals("chassisNumber"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.VehicleDetails?.chassisNumber;
                    }
                    else if (field.FieldKey.Equals("nomineeName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.NomineeDetails?.nomineeName;
                    }
                    else if (field.FieldKey.Equals("nomineeDateOfBirth") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.NomineeDetails?.nomineeDateOfBirth;
                    }
                    else if (field.FieldKey.Equals("nomineeRelation") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.NomineeDetails?.nomineeRelation;
                    }
                    else if (field.FieldKey.Equals("branch"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.VehicleDetails?.branch;
                    }
                    else if (field.FieldKey.Equals("vehicleNumber"))
                    {
                        field.DefaultValue = relianceProposalRequestBody?.VehicleDetails?.vehicleNumber;
                    }
                }
            }
        }
        private static void BindIFFCOProposalData(string proposalData, IEnumerable<ProposalFieldMasterModel> proposalFields)
        {
            var iffcoProposalRequestBody = JsonConvert.DeserializeObject<IFFCOProposalDynamicDetails>(proposalData);
            if (proposalFields != null)
            {
                foreach (var field in proposalFields)
                {
                    if (field.FieldKey.Equals("companyName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.companyName;
                    }
                    else if (field.FieldKey.Equals("gstno") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.gstno;
                    }
                    else if (field.FieldKey.Equals("salutation") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.salutation;
                    }
                    else if (field.FieldKey.Equals("maritalStatus") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.maritalStatus;
                    }
                    else if (field.FieldKey.Equals("firstName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.firstName;
                    }
                    else if (field.FieldKey.Equals("lastName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.lastName;
                    }
                    else if (field.FieldKey.Equals("dateOfBirth") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.dateOfBirth;
                    }
                    else if (field.FieldKey.Equals("gender") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.gender;
                    }
                    else if (field.FieldKey.Equals("mobile") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.mobile;
                    }
                    else if (field.FieldKey.Equals("emailId") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.emailId;
                    }
                    else if (field.FieldKey.Equals("dateOfIncorporation") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.dateOfIncorporation;
                    }
                    else if (field.FieldKey.Equals("occupation") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.PersonalDetails?.occupation;
                    }
                    else if (field.FieldKey.Equals("street"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.AddressDetails?.street;
                    }
                    else if (field.FieldKey.Equals("city"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.AddressDetails?.city;
                    }
                    else if (field.FieldKey.Equals("state"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.AddressDetails?.state;
                    }
                    else if (field.FieldKey.Equals("pincode"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.AddressDetails?.pincode;
                    }
                    else if (field.FieldKey.Equals("isFinancier"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.VehicleDetails?.isFinancier;
                    }
                    else if (field.FieldKey.Equals("financer"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.VehicleDetails?.financer;
                    }
                    else if (field.FieldKey.Equals("engineNumber"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.VehicleDetails?.engineNumber;
                    }
                    else if (field.FieldKey.Equals("chassisNumber"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.VehicleDetails?.chassisNumber;
                    }
                    else if (field.FieldKey.Equals("nomineeName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.NomineeDetails?.nomineeName;
                    }
                    else if (field.FieldKey.Equals("nomineeRelation") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = iffcoProposalRequestBody?.NomineeDetails?.nomineeRelation;
                    }
                }
            }
        }
        private static void BindOrientalProposalData(string proposalData, IEnumerable<ProposalFieldMasterModel> proposalFields)
        {
            var orientalProposalDynamicDetail = JsonConvert.DeserializeObject<OrientalProposalDynamicDetail>(proposalData);
            if (proposalFields != null)
            {
                foreach (var field in proposalFields)
                {
                    if (field.FieldKey.Equals("customerName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.PersonalDetails?.customerName;
                    }
                    else if (field.FieldKey.Equals("emailId") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.PersonalDetails?.emailId;
                    }
                    else if (field.FieldKey.Equals("mobile") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.PersonalDetails?.mobile;
                    }
                    else if (field.FieldKey.Equals("companyName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.PersonalDetails?.companyName;
                    }
                    else if (field.FieldKey.Equals("dateOfIncorporation") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.PersonalDetails?.dateOfIncorporation;
                    }
                    else if (field.FieldKey.Equals("dateOfBirth") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.PersonalDetails?.dateOfBirth;
                    }
                    else if (field.FieldKey.Equals("gender") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.PersonalDetails?.gender;
                    }
                    else if (field.FieldKey.Equals("documentType") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.PersonalDetails?.documentType;
                    }
                    else if (field.FieldKey.Equals("documentId") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.PersonalDetails?.documentId;
                    }
                    else if (field.FieldKey.Equals("addressLine1"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.AddressDetails?.addressLine1;
                    }
                    else if (field.FieldKey.Equals("city"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.AddressDetails?.city;
                    }
                    else if (field.FieldKey.Equals("state"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.AddressDetails?.state;
                    }
                    else if (field.FieldKey.Equals("pincode"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.AddressDetails?.pincode;
                    }
                    else if (field.FieldKey.Equals("isFinancier"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.VehicleDetails?.isFinancier;
                    }
                    else if (field.FieldKey.Equals("financer"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.VehicleDetails?.financer;
                    }
                    else if (field.FieldKey.Equals("branch"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.VehicleDetails?.branch;
                    }
                    else if (field.FieldKey.Equals("engineNumber"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.VehicleDetails?.engineNumber;
                    }
                    else if (field.FieldKey.Equals("chassisNumber"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.VehicleDetails?.chassisNumber;
                    }
                    else if (field.FieldKey.Equals("vehicleColour"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.VehicleDetails?.vehicleColour;
                    }
                    else if (field.FieldKey.Equals("vehicleNumber"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.VehicleDetails?.vehicleNumber;
                    }
                    else if (field.FieldKey.Equals("financierAddressLine1"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.VehicleDetails?.financierAddressLine1;
                    }
                    else if (field.FieldKey.Equals("financierpincode"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.VehicleDetails?.financierpincode;
                    }
                    else if (field.FieldKey.Equals("financierstate"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.VehicleDetails?.financierstate;
                    }
                    else if (field.FieldKey.Equals("financiercity"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.VehicleDetails?.financiercity;
                    }
                    else if (field.FieldKey.Equals("nomineeName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.NomineeDetails?.nomineeName;
                    }
                    else if (field.FieldKey.Equals("nomineeDateOfBirth") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.NomineeDetails?.nomineeDateOfBirth;
                    }
                    else if (field.FieldKey.Equals("nomineeRelation") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = orientalProposalDynamicDetail?.NomineeDetails?.nomineeRelation;
                    }
                }
            }
        }
        private static void BindUnitedIndiaProposalData(string proposalData, IEnumerable<ProposalFieldMasterModel> proposalFields)
        {
            var unitedProposalDynamicDetail = JsonConvert.DeserializeObject<UnitedProposalDynamicDetail>(proposalData);
            if (proposalFields != null)
            {
                foreach (var field in proposalFields)
                {
                    if (field.FieldKey.Equals("customerName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.PersonalDetails?.customerName;
                    }
                    else if (field.FieldKey.Equals("emailId") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.PersonalDetails?.emailId;
                    }
                    else if (field.FieldKey.Equals("mobile") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.PersonalDetails?.mobile;
                    }
                    else if (field.FieldKey.Equals("companyName") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.PersonalDetails?.companyName;
                    }
                    else if (field.FieldKey.Equals("dateOfIncorporation") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.PersonalDetails?.dateOfIncorporation;
                    }
                    else if (field.FieldKey.Equals("gstno") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.PersonalDetails?.gstno;
                    }
                    else if (field.FieldKey.Equals("dateOfBirth") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.PersonalDetails?.dateOfBirth;
                    }
                    else if (field.FieldKey.Equals("gender") && field.Section.Equals("PersonalDetails"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.PersonalDetails?.gender;
                    }
                    else if (field.FieldKey.Equals("panNumber"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.PersonalDetails?.panNumber;
                    }
                    else if (field.FieldKey.Equals("aadharNumbrer"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.PersonalDetails?.aadharNumbrer;
                    }
                    else if (field.FieldKey.Equals("addressLine1"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.AddressDetails?.addressLine1;
                    }
                    else if (field.FieldKey.Equals("pincode"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.AddressDetails?.pincode;
                    }
                    else if (field.FieldKey.Equals("isFinancier"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.VehicleDetails?.isFinancier;
                    }
                    else if (field.FieldKey.Equals("financer"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.VehicleDetails?.financer;
                    }
                    else if (field.FieldKey.Equals("branch"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.VehicleDetails?.branch;
                    }
                    else if (field.FieldKey.Equals("engineNumber"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.VehicleDetails?.engineNumber;
                    }
                    else if (field.FieldKey.Equals("chassisNumber"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.VehicleDetails?.chassisNumber;
                    }
                    else if (field.FieldKey.Equals("vehicleNumber"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.VehicleDetails?.vehicleNumber;
                    }
                    else if (field.FieldKey.Equals("financierAddressLine1"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.VehicleDetails?.financierAddressLine1;
                    }
                    else if (field.FieldKey.Equals("nomineeName") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.NomineeDetails?.nomineeName;
                    }
                    else if (field.FieldKey.Equals("nomineeRelation") && field.Section.Equals("NomineeDetails"))
                    {
                        field.DefaultValue = unitedProposalDynamicDetail?.NomineeDetails?.nomineeRelation;
                    }
                }
            }
        }
        public async Task<PaymentCKCYResponseModel> InsertPaymentTransaction(QuoteResponseModel proposalResponse, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("QuoteTransactionID", proposalResponse.TransactionID, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", proposalResponse.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("ApplicationId", proposalResponse.ApplicationId, DbType.String, ParameterDirection.Input);
            parameters.Add("ProposalNumber", proposalResponse.ProposalNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("Amount", proposalResponse.GrossPremium, DbType.String, ParameterDirection.Input);
            parameters.Add("Status", proposalResponse.PaymentStatus, DbType.String, ParameterDirection.Input);
            parameters.Add("PaymentTransactionNumber", proposalResponse.PaymentTransactionNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("CKYCStatus", proposalResponse.CKYCStatus, DbType.String, ParameterDirection.Input);
            parameters.Add("Type", proposalResponse.Type, DbType.String, ParameterDirection.Input);
            parameters.Add("CKYCLink", proposalResponse.CKYCLink, DbType.String, ParameterDirection.Input);
            parameters.Add("CKYCFailReason", proposalResponse.CKYCFailReason, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyDocumentLink", proposalResponse.PolicyDocumentLink, DbType.String, ParameterDirection.Input);
            parameters.Add("DocumentId", proposalResponse.DocumentId, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyNumber", proposalResponse.PolicyNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("CustomerId", proposalResponse.CustomerId, DbType.String, ParameterDirection.Input);
            parameters.Add("IsTP", proposalResponse.IsTP, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("BreakinId", proposalResponse.BreakinId, DbType.String, ParameterDirection.Input);
            parameters.Add("PaymentLink", proposalResponse.PaymentURLLink, DbType.String, ParameterDirection.Input);
            parameters.Add("BankName", proposalResponse.BankName, DbType.String, ParameterDirection.Input);
            parameters.Add("BankPaymentRefNum", proposalResponse.BankPaymentRefNum, DbType.String, ParameterDirection.Input);
            parameters.Add("PaymentDate", proposalResponse.PaymentDate, DbType.String, ParameterDirection.Input);
            parameters.Add("UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("PaymentCorrelationId", proposalResponse.PaymentCorrelationId, DbType.String, ParameterDirection.Input);
            parameters.Add("BreakinInspectionURL", proposalResponse.BreakinInspectionURL, DbType.String, ParameterDirection.Input);

            var saveData = await connection.QueryMultipleAsync("[dbo].[Insurance_InsertPaymentTransaction]", parameters,
                     commandType: CommandType.StoredProcedure);

            var response = saveData.Read<PaymentCKCYResponseModel>().FirstOrDefault();
            return response;
        }

        public async Task<PaymentCKCYResponseModel> GetPaymentStatus(PaymentStatusRequestModel paymentStatusRequest, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("QuotetransactionId", paymentStatusRequest.QuoteTransactionId, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", paymentStatusRequest.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("ApplicationId", paymentStatusRequest.ApplicationId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<PaymentCKCYResponseModel>("[dbo].[Insurance_GetPaymentDetails]",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task<string> UploadPolicyDocumentMangoDB(byte[] policyDocumentBase64, CancellationToken cancellationToken)
        {
            try
            {
                MemoryStream stream = new MemoryStream(policyDocumentBase64);
                //await File.WriteAllBytesAsync("C:\\Hero_Insurance\\Document.pdf", policyDocumentBase64);

                string documentId = await _mongoDBService.MongoUpload("Policy Document PDF", stream, policyDocumentBase64);

                if (documentId != null)
                {
                    return documentId;
                }
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("UploadPolicyDocumentMangoDB exception {message}", ex.Message);
            }
            return default;
        }

        public async Task<string> GetPolicyDocumentData(PaymentStatusRequestModel policyDocumentRequestModel, CancellationToken cancellationToken)
        {
            byte[] documentBase64 = null;
            string document = string.Empty;
            PolicyDocumentModel docModel = new PolicyDocumentModel();
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("QuotetransactionId", policyDocumentRequestModel.QuoteTransactionId, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", policyDocumentRequestModel.InsurerId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetPolicyDocumentDetails]",
                parameters,
                commandType: CommandType.StoredProcedure);

            var response = result.Read<PaymentCKCYResponseModel>().FirstOrDefault();
            var policyDocReqData = result.Read<PolicyDocumentDownloadRequest>().FirstOrDefault();

            if (response != null && !string.IsNullOrEmpty(response.DocumentId))
            {
                document = policyDocumentRequestModel.InsurerId.Equals(_insurerConfig.Reliance) ? response.PolicyDocumentLink : await DownloadPolicyDocumentMangoDB(response.DocumentId, cancellationToken);
                return document;
            }
            else if (response != null && string.IsNullOrEmpty(response.DocumentId))
            {
                switch (policyDocumentRequestModel.InsurerId)
                {
                    case ("78190CB2-B325-4764-9BD9-5B9806E99621"):
                        var policyDocumentRes = await _godigitService.GetPolicyDocumentPDF(response.LeadId, response.ApplicationId, cancellationToken);
                        if (policyDocumentRes != null)
                        {
                            docModel.PolicyDocumentLink = policyDocumentRes.schedulePath;
                            documentBase64 = await _godigitService.GetDocumentPDFBase64(policyDocumentRes.schedulePath, cancellationToken);
                        }
                        break;
                    case ("16413879-6316-4C1E-93A4-FF8318B14D37"):
                        documentBase64 = await _bajajService.GeneratePolicy(response.LeadId, response.PolicyNumber, response.IsTP);
                        break;
                    case ("FD3677E5-7938-46C8-9CD2-FAE188A1782C"):
                        var proposalRequest = JsonConvert.DeserializeObject<ICICIProposalRequestDto>(response.RequestBody);
                        ICICIPaymentTaggingResponse taggingResponse = new ICICIPaymentTaggingResponse()
                        {
                            correlationId = response.ApplicationId,
                        };
                        ICICIPaymentTaggingResponseDto iCICIPaymentTaggingResponseDto = new ICICIPaymentTaggingResponseDto()
                        {
                            CustomerId = response.CustomerId,
                            iCICIPaymentTaggingResponse = taggingResponse,
                            PolicyNumber = response.PolicyNumber,
                            DealId = proposalRequest.DealId,
                            VehicleTypeId = policyDocReqData.VehicleTypeId,
                            LeadId = response?.LeadId,
                            ProductCode = proposalRequest.ProductCode
                        };

                        ICICIPOSPDetails iCICIPOSPDetails = await GetPOPSDetais(response.UserId);
                        documentBase64 = await _iCICIService.GeneratePolicy(iCICIPaymentTaggingResponseDto, iCICIPOSPDetails);

                        break;
                    case ("77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA"):
                        if (response.ProposalNumber != null)
                        {
                            string documentURL = _cholaConfig.PolicyDocumentURL + response.ProposalNumber + "&user_code=" + _cholaConfig.UserCode;
                            documentBase64 = await _cholaService.GetDocumentPDFBase64(response.LeadId, documentURL, response.ProposalNumber, cancellationToken);
                        }
                        break;
                    case ("0A326B77-AFD5-44DA-9871-1742624CFF16"):
                        HDFCPolicyRequestModel hdfcFields = new HDFCPolicyRequestModel()
                        {
                            BankName = response.BankName,
                            GrossPremium = response.Amount,
                            ApplicationId = response.ApplicationId,
                            VehicleTypeId = policyDocReqData?.VehicleTypeId,
                            PolicyTypeId = policyDocReqData?.PolicyTypeId,
                            TransactionId = policyDocReqData?.TransactionId,
                            ProposalNumber = response.ProposalNumber,
                            LeadId = response.LeadId,
                            CategoryId = policyDocReqData.CVCategoryId
                        };
                        var policyDocData = await _hDFCService.GetPolicyDocument(hdfcFields, cancellationToken);
                        if (policyDocData != null && policyDocData.InsurerStatusCode.Equals(200))
                        {
                            response.CustomerId = policyDocData.CustomerId;
                            response.PolicyNumber = policyDocData.PolicyNumber;
                            documentBase64 = Convert.FromBase64String(policyDocData.PolicyDocumentBase64);
                        }
                        break;
                    case ("E656D5D1-5239-4E48-9048-228C67AE3AC3"):
                        if (response.ProposalNumber != null)
                        {
                            IFFCOPaymentResponseModel iFFCOPaymentResponseModel = new IFFCOPaymentResponseModel()
                            {
                                ProposalNumber = policyDocReqData?.TransactionId,
                                PolicyNumber = response.PolicyNumber,
                                Product = policyDocReqData.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler) ? "TWP" : "PCP",
                                LeadId = response?.LeadId
                            };
                            var policyDocumentURLResponse = await _IFFCOService.GetPolicyDownloadURL(iFFCOPaymentResponseModel, cancellationToken);
                            if (policyDocumentURLResponse != null && policyDocumentURLResponse.policyDownloadLink != null)
                            {
                                docModel.PolicyDocumentLink = policyDocumentURLResponse.policyDownloadLink;
                                documentBase64 = await _IFFCOService.PolicyDownload(response?.LeadId, policyDocumentURLResponse.policyDownloadLink, cancellationToken);
                            }
                        }
                        break;
                    case ("85F8472D-8255-4E80-B34A-61DB8678135C"):
                        var policyDocTATA = await _tataService.GetPolicyDocument(response.PaymentTransactionNumber, policyDocReqData.VehicleTypeId, response.LeadId, cancellationToken);
                        if (policyDocTATA != null && policyDocTATA.InsurerStatusCode.Equals(200) && policyDocTATA.PolicyDocumentBase64 != null)
                        {
                            documentBase64 = Convert.FromBase64String(policyDocTATA.PolicyDocumentBase64);
                        }
                        break;
                }

                if (documentBase64 != null)
                {
                    var documentId = await UploadPolicyDocumentMangoDB(documentBase64, cancellationToken);
                    if (documentId != null)
                    {
                        docModel.DocumentId = documentId;

                        using var connections = _context.CreateConnection();
                        var updateParameters = new DynamicParameters();
                        updateParameters.Add("QuotetransactionId", policyDocumentRequestModel.QuoteTransactionId, DbType.String, ParameterDirection.Input);
                        updateParameters.Add("InsurerId", policyDocumentRequestModel.InsurerId, DbType.String, ParameterDirection.Input);
                        updateParameters.Add("PolicyDocumentLink", docModel.PolicyDocumentLink, DbType.String, ParameterDirection.Input);
                        updateParameters.Add("DocumentId", docModel.DocumentId, DbType.String, ParameterDirection.Input);
                        updateParameters.Add("CustomerId", response.CustomerId, DbType.String, ParameterDirection.Input);
                        updateParameters.Add("PolicyNumber", response.PolicyNumber, DbType.String, ParameterDirection.Input);

                        var docResult = await connection.QueryMultipleAsync("[dbo].[Insurance_UpdatePolicyDocumentDetails]",
                            updateParameters,
                            commandType: CommandType.StoredProcedure);

                        var docResponse = docResult.Read<string>().FirstOrDefault();

                        if (docResponse != null)
                        {
                            document = await DownloadPolicyDocumentMangoDB(docResponse, cancellationToken);
                            return document;
                        }
                    }
                }
            }
            return default;
        }

        public async Task<string> DownloadPolicyDocumentMangoDB(string documentId, CancellationToken cancellationToken)
        {
            string policyDocumentPDF = await _mongoDBService.MongoDownload(documentId);
            if (policyDocumentPDF != null)
            {
                return policyDocumentPDF;
            }
            return default;
        }

        public async Task<QuotConfirmDataResponseModel> QuoteConfirmTransaction(QuoteConfirmDataModel quoteConfirmDataModel, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var commonResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(quoteConfirmDataModel.CommonResponse);
            DataTable dataTable = new();
            dataTable.Columns.Add("CoverId", typeof(string));
            dataTable.Columns.Add("IsChecked", typeof(bool));

            foreach (var item in quoteConfirmDataModel.CoverMasterDetails)
            {
                dataTable.Rows.Add(item.CoverId, item.IsChecked);
            }
            var parameters = new DynamicParameters();
            parameters.Add("CoverMasterData", dataTable.AsTableValuedParameter("dbo.CoverMasterDetailsType"));
            parameters.Add("InsurerId", quoteConfirmDataModel.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("ResponseBody", quoteConfirmDataModel.ResponseBody, DbType.String, ParameterDirection.Input);
            parameters.Add("RequestBody", quoteConfirmDataModel.RequestBody, DbType.String, ParameterDirection.Input);
            parameters.Add("CommonResponse", quoteConfirmDataModel.CommonResponse, DbType.String, ParameterDirection.Input);
            parameters.Add("Stage", quoteConfirmDataModel.Stage, DbType.String, ParameterDirection.Input);
            parameters.Add("LeadId", quoteConfirmDataModel.LeadId, DbType.String, ParameterDirection.Input);
            parameters.Add("MaxIDV", quoteConfirmDataModel.MaxIDV, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("MinIDV", quoteConfirmDataModel.MinIDV, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("RecommendedIDV", quoteConfirmDataModel.RecommendedIDV, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("TransactionId", quoteConfirmDataModel.TransactionId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleTypeId", quoteConfirmDataModel.VehicleTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("QuoteTransactionId", quoteConfirmDataModel.QuoteTransactionId, DbType.String, ParameterDirection.Input);
            parameters.Add("UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleNumber", quoteConfirmDataModel.VehicleNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("ManufacturingMonthYear", quoteConfirmDataModel.ManufacturingMonthYear, DbType.String, ParameterDirection.Input);
            parameters.Add("RegistrationDate", quoteConfirmDataModel.RegistrationDate, DbType.String, ParameterDirection.Input);
            parameters.Add("CustomerType", quoteConfirmDataModel.Customertype, DbType.String, ParameterDirection.Input);
            parameters.Add("IsPreviousPolicy", quoteConfirmDataModel.PreviousPolicy?.IsPreviousPolicy, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("PreviousPolicyTypeId", quoteConfirmDataModel.PreviousPolicy?.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("SAODPolicyExpiryDate", quoteConfirmDataModel.PreviousPolicy?.SAODPolicyExpiryDate, DbType.String, ParameterDirection.Input);
            parameters.Add("SAODPolicyNumber", quoteConfirmDataModel.PreviousPolicy?.PreviousPolicyNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("SATPPolicyExpiryDate", quoteConfirmDataModel.PreviousPolicy?.TPPolicyExpiryDate, DbType.String, ParameterDirection.Input);
            parameters.Add("SATPPolicyNumber", quoteConfirmDataModel.PreviousPolicy?.PreviousPolicyNumberSATP, DbType.String, ParameterDirection.Input);
            parameters.Add("IsPreviousYearClaim", quoteConfirmDataModel.PreviousPolicy?.IsPreviousYearClaim, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("PreviousNCBId", quoteConfirmDataModel.PreviousPolicy?.NCBId, DbType.String, ParameterDirection.Input);
            parameters.Add("IsPACover", quoteConfirmDataModel.IsPACover, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("PACoverTenure", quoteConfirmDataModel.PACoverTenure, DbType.String, ParameterDirection.Input);
            parameters.Add("IsBrandNew", quoteConfirmDataModel.IsBrandNewVehicle, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("CompanyName", quoteConfirmDataModel.CompanyName, DbType.String, ParameterDirection.Input);
            parameters.Add("DateOfIncorporation", quoteConfirmDataModel.DOI, DbType.String, ParameterDirection.Input);
            parameters.Add("GSTNumber", quoteConfirmDataModel.GSTNo, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyStartDate", quoteConfirmDataModel.PolicyDates?.PolicyStartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyEndDate", quoteConfirmDataModel.PolicyDates?.PolicyEndDate, DbType.String, ParameterDirection.Input);
            parameters.Add("IsBreakin", quoteConfirmDataModel.IsBreakin, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("IsQuoteDeviation", quoteConfirmDataModel.IsQuoteDeviation, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("IsApprovalRequired", quoteConfirmDataModel.IsApprovalRequired, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("TotalPremium", commonResponse?.TotalPremium, DbType.String, ParameterDirection.Input);
            parameters.Add("GrossPremiume", commonResponse?.GrossPremium, DbType.String, ParameterDirection.Input);
            parameters.Add("Tax", JsonConvert.SerializeObject(commonResponse?.Tax), DbType.String, ParameterDirection.Input);
            parameters.Add("NCBPercentage", commonResponse?.NCB, DbType.String, ParameterDirection.Input);
            parameters.Add("IsSelfInspection", quoteConfirmDataModel.IsSelfInspection, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("isPolicyExpired", quoteConfirmDataModel.IsPolicyExpired, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("RTOCode", quoteConfirmDataModel.VehicleNumber.Substring(0, 4), DbType.String, ParameterDirection.Input);
            parameters.Add("PreviousSAODInsurer", quoteConfirmDataModel.ConfirmCommand?.PreviousPolicy.SAODInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("PreviousSATPInsurer", quoteConfirmDataModel.ConfirmCommand?.PreviousPolicy.TPInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("ProposalId", quoteConfirmDataModel.ProposalId, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyId", quoteConfirmDataModel.PolicyId, DbType.String, ParameterDirection.Input);
            parameters.Add("PreviousSATPPolicyStartDate", quoteConfirmDataModel.PolicyDates?.TPPolicyStartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("PreviousSAODPolicyStartDate", quoteConfirmDataModel.PolicyDates?.ODPolicyStartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("ResponseReferanceFlag", quoteConfirmDataModel.ResponseReferanceFlag, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_InsertQuoteConfirmTransaction]", parameters,
                commandType: CommandType.StoredProcedure);

            var quotConfirmDataResponseModel = result.Read<QuotConfirmDataResponseModel>().FirstOrDefault();
            return quotConfirmDataResponseModel;
        }

        public async Task<QuoteTransactionDbModel> GetQuoteConfirmDetailsDB(string transactionID, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
        {
            var getTransaction = await GetQuoteTransactionDetails(transactionID, cancellationToken);

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("PolicyTypeId", quoteConfirmCommand.PolicyDates.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleNumber", quoteConfirmCommand.VehicleNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("PreviousSAODInsurerId", quoteConfirmCommand.PreviousPolicy.SAODInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("PreviousSATPInsurerId", quoteConfirmCommand.PreviousPolicy.TPInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("NCBId", quoteConfirmCommand.PreviousPolicy.NCBId, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", getTransaction.QuoteTransactionRequest.InsurerId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<QuoteConfirmDetailsModel>("[dbo].[Insurance_GetQuoteConfirmDetails]",
                parameters,
                commandType: CommandType.StoredProcedure);

            getTransaction.QuoteConfirmDetailsModel = result.FirstOrDefault();

            return getTransaction;
        }
        public async Task SaveQuoteTransaction(SaveQuoteTransactionModel saveQuoteTransactionModel, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", saveQuoteTransactionModel.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("ResponseBody", saveQuoteTransactionModel.ResponseBody, DbType.String, ParameterDirection.Input);
            parameters.Add("RequestBody", saveQuoteTransactionModel.RequestBody, DbType.String, ParameterDirection.Input);
            parameters.Add("CommonResponse", JsonConvert.SerializeObject(saveQuoteTransactionModel.quoteResponseModel), DbType.String, ParameterDirection.Input);
            parameters.Add("Stage", saveQuoteTransactionModel.Stage, DbType.String, ParameterDirection.Input);
            parameters.Add("LeadId", saveQuoteTransactionModel.LeadId, DbType.String, ParameterDirection.Input);
            parameters.Add("MaxIDV", saveQuoteTransactionModel.MaxIDV, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("MinIDV", saveQuoteTransactionModel.MinIDV, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("RecommendedIDV", saveQuoteTransactionModel.RecommendedIDV, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("TransactionId", saveQuoteTransactionModel.TransactionId, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyNumber", saveQuoteTransactionModel.PolicyNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyId", saveQuoteTransactionModel.PolicyId, DbType.String, ParameterDirection.Input);
            parameters.Add("QuoteTransactionId", saveQuoteTransactionModel.QuoteTransactionId, DbType.String, ParameterDirection.Input);
            parameters.Add("CreatedBy", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("SelectedIDV", saveQuoteTransactionModel.SelectedIDV, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyTypeId", saveQuoteTransactionModel.PolicyTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("IsPreviousPolicy", saveQuoteTransactionModel.IsPreviousPolicy, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("SAODInsurerId", saveQuoteTransactionModel.SAODInsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("SATPInsurerId", saveQuoteTransactionModel.SATPInsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("SAODPolicyStartDate", saveQuoteTransactionModel.SAODPolicyStartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("SAODPolicyExpiryDate", saveQuoteTransactionModel.SAODPolicyExpiryDate, DbType.String, ParameterDirection.Input);
            parameters.Add("SATPPolicyStartDate", saveQuoteTransactionModel.SATPPolicyStartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("SATPPolicyExpiryDate", saveQuoteTransactionModel.SATPPolicyExpiryDate, DbType.String, ParameterDirection.Input);
            parameters.Add("IsPreviousYearClaim", saveQuoteTransactionModel.IsPreviousYearClaim, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("PreviousYearNCB", saveQuoteTransactionModel.PreviousYearNCB, DbType.String, ParameterDirection.Input);
            parameters.Add("IsBrandNew", saveQuoteTransactionModel.IsBrandNew, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("VehicleNumber", saveQuoteTransactionModel.VehicleNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("RegistrationDate", saveQuoteTransactionModel.RegistrationDate, DbType.String, ParameterDirection.Input);
            parameters.Add("IsSharePaymentLink", saveQuoteTransactionModel.IsSharePaymentLink, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("TotalPremium", saveQuoteTransactionModel.TotalPremium, DbType.String, ParameterDirection.Input);
            parameters.Add("GrossPremiume", saveQuoteTransactionModel.GrossPremiume, DbType.String, ParameterDirection.Input);
            parameters.Add("Tax", saveQuoteTransactionModel.Tax, DbType.String, ParameterDirection.Input);
            parameters.Add("RTOId", saveQuoteTransactionModel.RTOId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_SaveQuoteTransaction]", parameters,
                         commandType: CommandType.StoredProcedure);

            var insurerInfo = result.Read<InsurerInfo>();
            var cashlessList = result.Read<CashlessGarage>();
            var premiumBasicDetailList = result.Read<PremiumBasicDetails>();
            var PremiumBasicSubtitleDetailList = result.Read<PremiumBasicSubDetails>();
            var quoteransactionId = result.Read<string>();

            saveQuoteTransactionModel.quoteResponseModel.InsurerLogo = _logoConfig.InsurerLogoURL + insurerInfo?.FirstOrDefault()?.Logo;
            saveQuoteTransactionModel.quoteResponseModel.SelfVideoClaim = insurerInfo?.FirstOrDefault()?.SelfVideoClaims;
            saveQuoteTransactionModel.quoteResponseModel.InsurerDescription = insurerInfo?.FirstOrDefault()?.SelfDescription;
            saveQuoteTransactionModel.quoteResponseModel.IsRecommended = insurerInfo.FirstOrDefault().IsRecommended;
            saveQuoteTransactionModel.quoteResponseModel.RecommendedDescription = insurerInfo?.FirstOrDefault()?.RecommendedDescription;
            saveQuoteTransactionModel.quoteResponseModel.TransactionID = quoteransactionId.FirstOrDefault();
            saveQuoteTransactionModel.quoteResponseModel.GarageDescription = !string.IsNullOrEmpty(insurerInfo?.FirstOrDefault()?.GarageDescription) ? insurerInfo?.FirstOrDefault()?.GarageDescription : string.Empty;

            foreach (var item in premiumBasicDetailList)
            {
                var subtitleModel = PremiumBasicSubtitleDetailList
                    .Where(x => x.PremiumBasicDetailId.Equals(item.PremiumBasicDetailsId))
                    .Select(d => new PremiumBasicSubDetails
                    {
                        PremiumBasicDetailId = d.PremiumBasicDetailId,
                        SubtitleId = d.SubtitleId,
                        Subtitle = d.Subtitle,
                        Description = d.Description,
                        Icon = _logoConfig.PremiumDetailsURL + d.Icon
                    }).ToList();
                item.SubDetailsList = subtitleModel;
            }
            saveQuoteTransactionModel.quoteResponseModel.CashlessGarageList = cashlessList;
            saveQuoteTransactionModel.quoteResponseModel.PremiumBasicDetailsList = premiumBasicDetailList;
            saveQuoteTransactionModel.quoteResponseModel.CachlessGarageCount = cashlessList.Count();
        }
        public async Task<GetLeadDetailsModel> GetLeadDetails(string leadId, string stageId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("LeadId", leadId, DbType.String, ParameterDirection.Input);
            parameters.Add("StageId", stageId, DbType.String, ParameterDirection.Input);

            var reader = await connection.QueryMultipleAsync("[dbo].[Insurance_GetLeadDetails]", parameters, commandType: CommandType.StoredProcedure);
            IEnumerable<GetLeadDetailsModel> result = await reader.ReadAsync<GetLeadDetailsModel>();
            if (result.Any())
            {
                result.FirstOrDefault().LeadPreviousCoverDetails = (await reader.ReadAsync<LeadPreviousCoverDetails>()).ToList();
            }
            if (!string.IsNullOrEmpty(result.FirstOrDefault().RegistrationDate) && !string.IsNullOrEmpty(result.FirstOrDefault().MakeMonthYear))
            {
                result.FirstOrDefault().RegistrationDate = Convert.ToDateTime(result.FirstOrDefault().RegistrationDate).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                result.FirstOrDefault().MakeMonthYear = Convert.ToDateTime(result.FirstOrDefault().MakeMonthYear).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            return result.FirstOrDefault();
        }
        public async Task UpdateLeadDetails(CreateLeadModel createLeadModel, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("LeadId", createLeadModel.LeadID, DbType.String, ParameterDirection.Input);
            parameters.Add("QuoteTransactionId", createLeadModel.QuoteTransactionID, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyNumber", createLeadModel.PolicyNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("PaymentLink", createLeadModel.PaymentLink, DbType.String, ParameterDirection.Input);
            parameters.Add("Stage", createLeadModel.Stage, DbType.String, ParameterDirection.Input);
            parameters.Add("BreakInStatus", createLeadModel.BreakInStatus, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("InsurerId", createLeadModel.InsurerId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync("[dbo].[Insurance_UpdateBreakinDetails]", parameters,
                         commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// GetCKYCFields
        /// </summary>
        /// <param name="InsurerID"></param>
        /// <param name="isPOI"></param>
        /// <param name="isCompany"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CKYCStatusModel> GetCKYCStstus(string insurerID, string quoteTransactionId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerID", insurerID, DbType.String, ParameterDirection.Input);
            parameters.Add("QuoteTransactionId", quoteTransactionId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<CKYCStatusModel>("[dbo].[Insurance_GetCKYCStatus]", parameters, commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }
        public async Task<ICICIPOSPDetails> GetPOPSDetais(string userId)
        {
            _logger.LogInformation("GetPOPSDetais userId {userId}", userId);
            using var connection = _identityApplicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<ICICIPOSPDetails>("[dbo].[Identity_GetICICIPOSPDetails]",
                parameters, commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }
        public async Task<BreakInPaymentDetailsDBModel> GetBreakInPaymentDetails(PaymentStatusRequestModel policyRequest, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", policyRequest.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("QuoteTransactionId", policyRequest.QuoteTransactionId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetBreakInPaymentDetails]",
                parameters,
                commandType: CommandType.StoredProcedure);

            var response = result.Read<BreakInPaymentDetailsDBModel>().FirstOrDefault();
            if (response != null)
            {
                return response;
            }
            return default;
        }
        public async Task<string> UpdateLeadPaymentLink(string insurerId, string quoteTransactionId, string paymentLink, string paymentCorrelationId, CancellationToken cancellationToken)
        {
            using var connections = _context.CreateConnection();
            var parameter = new DynamicParameters();
            parameter.Add("InsurerId", insurerId, DbType.String, ParameterDirection.Input);
            parameter.Add("QuoteTransactionId", quoteTransactionId, DbType.String, ParameterDirection.Input);
            parameter.Add("PaymentLink", paymentLink, DbType.String, ParameterDirection.Input);
            parameter.Add("UpdatedBy", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameter.Add("PaymentCorrelationId", paymentCorrelationId, DbType.String, ParameterDirection.Input);

            var updatePaymentLink = await connections.QueryMultipleAsync("[dbo].[Insurance_UpdateLeadPaymentLink]",
                parameter,
                commandType: CommandType.StoredProcedure);

            var updatePaymentLinkResponse = updatePaymentLink.Read<string>().FirstOrDefault();
            if (updatePaymentLinkResponse != null)
            {
                return updatePaymentLinkResponse;
            }
            return default;
        }
        public async Task<GetProposalDetailsForPaymentResponceModel> GetProposalDetailsForPayment(string insurerId, string quoteTransactionId, CancellationToken cancellationToken)
        {
            using var connections = _context.CreateConnection();
            var parameter = new DynamicParameters();
            parameter.Add("InsurerId", insurerId, DbType.String, ParameterDirection.Input);
            parameter.Add("QuoteTransactionId", quoteTransactionId, DbType.String, ParameterDirection.Input);

            var preoposalDetails = await connections.QueryAsync<GetProposalDetailsForPaymentResponceModel>("[dbo].[Insurance_GetProposalDetailsForPayment]", parameter, commandType: CommandType.StoredProcedure);

            var result = preoposalDetails.FirstOrDefault();
            if (result is not null)
            {
                return result;
            }
            return default;
        }

        public async Task<string> GetUserIddDetails(string insurerId, string proposalNumber, CancellationToken cancellationToken)
        {
            using var connections = _context.CreateConnection();
            var parameter = new DynamicParameters();
            parameter.Add("InsurerId", insurerId, DbType.String, ParameterDirection.Input);
            parameter.Add("ProposalNumber", proposalNumber, DbType.String, ParameterDirection.Input);

            var iuserIdDetails = await connections.QueryAsync<string>("[dbo].[Insurance_GetUserIdDetails]",
                parameter,
                commandType: CommandType.StoredProcedure);

            var result = iuserIdDetails.FirstOrDefault();
            if (result != null)
            {
                return result;
            }
            return default;
        }

        public async Task<GetPreviousCoverResponseModel> GetPreviousCoverMaster(string insurerID, string vehicalTypeId, string policyTypeId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", insurerID, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicalTypeId", vehicalTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyTypeId", policyTypeId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetPreviousCover]",
                parameters,
                commandType: CommandType.StoredProcedure);

            GetPreviousCoverResponseModel getPreviousCoverResponseModel = new()
            {
                getPreviousCoverRecords = result.Read<GetPreviousCoverRecords>()
            };

            return getPreviousCoverResponseModel;
        }

        public async Task<CreateLeadModel> InsertBreakInDetails(InsertBreakInDetailsModel breakInDetailsModel, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("LeadId", breakInDetailsModel.LeadId, DbType.String, ParameterDirection.Input);
            parameters.Add("IsBreakIn", breakInDetailsModel.IsBreakIn, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("PolicyNumber", breakInDetailsModel.PolicyNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("BreakinId", breakInDetailsModel.BreakInId, DbType.String, ParameterDirection.Input);
            parameters.Add("BreakinInspectionURL", breakInDetailsModel.BreakinInspectionURL, DbType.String, ParameterDirection.Input);
            parameters.Add("BreakInInspectionAgency", breakInDetailsModel.BreakInInspectionAgency, DbType.String, ParameterDirection.Input);
            parameters.Add("UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);


            var result = await connection.QueryAsync<CreateLeadModel>("[dbo].[Insurance_InsertBreakinDetails]",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task<CreateLeadModel> GetLeadDetailsByApplicationIdOrQuoteTransactionId(GetLeadDetailsByApplicationOrQuoteTransactionIdModel request, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("ApplicationId", request.ApplicationId, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", request.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("QuoteTransactionId", request.QuoteTransactionId, DbType.String, ParameterDirection.Input);


            var result = await connection.QueryAsync<CreateLeadModel>("[dbo].[Insurance_GetLeadDetailsByApplicationId]",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task InsertQuoteRequest(string request, string leadId, string stage, string quoteTransactionId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("RequestBody", request, DbType.String, ParameterDirection.Input);
            parameters.Add("LeadId", leadId, DbType.String, ParameterDirection.Input);
            parameters.Add("Stage", stage, DbType.String, ParameterDirection.Input);
            parameters.Add("UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("QuoteTransactionId", quoteTransactionId, DbType.String, ParameterDirection.Input);

            var result = await connection.ExecuteAsync("[dbo].[Insurance_InsertQuoteRequest]",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        public async Task<int> InsertICLogs(string insurerId, string requestBody, string responseBody, string leadId, string api, string token, string header, string stage)
        {
            var logsModel = new LogsModel
            {
                InsurerId = insurerId,
                RequestBody = requestBody,
                ResponseBody = responseBody,
                API = api,
                UserId = _applicationClaims.GetUserId(),
                Token = token,
                Headers = header,
                LeadId = leadId,
                Stage = stage
            };

            var id = await _commonService.InsertQuoteLogs(logsModel);
            return id;
        }
        public async Task<List<string>> VehicleNumberSplit(string vehicleNumber)
        {
            var words = new List<StringBuilder> { new StringBuilder() };
            for (var i = 0; i < vehicleNumber.Length; i++)
            {
                words[words.Count - 1].Append(vehicleNumber[i]);
                if (i + 1 < vehicleNumber.Length && char.IsLetter(vehicleNumber[i]) != char.IsLetter(vehicleNumber[i + 1]))
                {
                    words.Add(new StringBuilder());
                }
            }
            return words.Select(x => x.ToString()).ToList();
        }

        public async Task<CKYCStatusModel> InsertKYCDetailsAfterProposal(KYCDetailsModel kycDetailsModel, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("LeadId", kycDetailsModel.LeadId, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", kycDetailsModel.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("QuoteTransactionId", kycDetailsModel.QuoteTransactionId, DbType.String, ParameterDirection.Input);
            parameters.Add("RequestBody", kycDetailsModel.RequestBody, DbType.String, ParameterDirection.Input);
            parameters.Add("ResponseBody", kycDetailsModel.ResponseBody, DbType.String, ParameterDirection.Input);
            parameters.Add("PhotoId", kycDetailsModel.PhotoId, DbType.String, ParameterDirection.Input);
            parameters.Add("Stage", kycDetailsModel.Stage, DbType.String, ParameterDirection.Input);
            parameters.Add("KYCId", kycDetailsModel.KYCId, DbType.String, ParameterDirection.Input);
            parameters.Add("CKYCNumber", kycDetailsModel.CKYCNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("CKYCStatus", kycDetailsModel.CKYCStatus, DbType.String, ParameterDirection.Input);
            parameters.Add("UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<CKYCStatusModel>("[dbo].[Insurance_InsertKYCDetailsAfterProposal]",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }
    }
}
