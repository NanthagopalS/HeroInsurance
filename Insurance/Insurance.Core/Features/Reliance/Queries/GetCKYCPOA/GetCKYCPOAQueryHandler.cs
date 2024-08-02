using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Reliance;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;

namespace Insurance.Core.Features.Reliance.Queries.GetCKYCPOA
{
    public class GetCKYCPOAQuery : IRequest<HeroResult<UploadCKYCDocumentResponse>>
    {
        public string QuoteTransactionId { get; set; }
        public string UserId { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string UniqueId { get; set; }
        public string RegisteredName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string KYCNumber { get; set; }
        public string CorrAddressLine1 { get; set; }
        public string CorrAddressLine2 { get; set; }
        public string CorrCity { get; set; }
        public string CorrState { get; set; }
        public string CorrCountry { get; set; }
        public string CorrPinCode { get; set; }
        public string PerAddressLine1 { get; set; }
        public string PerAddressLine2 { get; set; }
        public string PerCity { get; set; }
        public string PerState { get; set; }
        public string PerCountry { get; set; }
        public string PerPinCode { get; set; }
        public string ProposalId { get; set; }
        public string KYCVerified { get; set; }
        public string VerifiedAt { get; set; }
        public string KYCProcess { get; set; }
        public string UDP1 { get; set; }
        public string UDP2 { get; set; }
        public string UDP3 { get; set; }
        public string UDP4 { get; set; }
        public string UDP5 { get; set; }
    }
    public class GetCKYCPOAQueryHandler : IRequestHandler<GetCKYCPOAQuery, HeroResult<UploadCKYCDocumentResponse>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly IRelianceRepository _relianceRepository;
        private readonly RelianceConfig _relianceConfig;

        public GetCKYCPOAQueryHandler(IQuoteRepository quoteRepository, IMapper mapper, IRelianceRepository relianceRepository, IOptions<RelianceConfig> options)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
            _relianceRepository = relianceRepository;
            _relianceConfig = options.Value;
        }

        public async Task<HeroResult<UploadCKYCDocumentResponse>> Handle(GetCKYCPOAQuery request, CancellationToken cancellationToken)
        {
            var kycVerifiedModel = new UploadCKYCDocumentResponse();
            string poaRequest = string.Empty;
            string poaResponse = string.Empty;
            CreateLeadModel createLeadModel = new CreateLeadModel();
            createLeadModel.LeadName = !string.IsNullOrEmpty(request.RegisteredName) ? request.RegisteredName : request.LastName + " " + request.FirstName;
            createLeadModel.LastName = request.LastName;
            createLeadModel.MiddleName = request.MiddleName;
            createLeadModel.DOB = !string.IsNullOrWhiteSpace(request.DOB) ? DateTime.ParseExact(request.DOB, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : string.Empty;
            createLeadModel.PhoneNumber = request.Mobile;
            createLeadModel.Email = request.Email;
            createLeadModel.ckycNumber = request.KYCNumber;
            createLeadModel.kyc_id = request.UniqueId;
            createLeadModel.CKYCstatus = request.KYCProcess;
            createLeadModel.Gender = request.Gender;
            createLeadModel.PermanentAddress = new LeadAddressModel
            {
                AddressType = "PRIMARY",
                Address1 = request.PerAddressLine1,
                Address2 = request.PerAddressLine2,
                Pincode = request.PerPinCode
            };
            createLeadModel.CommunicationAddress = new LeadAddressModel
            {
                AddressType = "SECONDARY",
                Address1 = request.CorrAddressLine1,
                Address2 = request.CorrAddressLine2,
                Pincode = request.CorrPinCode
            };
            if (request.KYCProcess.ToLower().Equals("complete"))
            {
                poaResponse= JsonConvert.SerializeObject(request);
                kycVerifiedModel.InsurerId = _relianceConfig.InsurerId;
                kycVerifiedModel.TransactionId = request.QuoteTransactionId;
                createLeadModel.CKYCstatus = "Success";
            }
            else
            {
                createLeadModel.kyc_id= request.KYCNumber;
                createLeadModel.CKYCstatus = "Pending";
            }
            var response = await _quoteRepository.SaveLeadDetails(_relianceConfig.InsurerId, request.QuoteTransactionId, poaRequest, poaResponse,"POI", createLeadModel,cancellationToken);
            if (response != null)
            {
                kycVerifiedModel.KYCId = response.KYCId;
                kycVerifiedModel.CKYCNumber = response.CKYCNumber;
                kycVerifiedModel.LeadID = response.LeadID;
            }

            if (kycVerifiedModel != null)
            {
                return HeroResult<UploadCKYCDocumentResponse>.Success(kycVerifiedModel);
            }
            return HeroResult<UploadCKYCDocumentResponse>.Fail("CKYC POA Fail");
        }
    }
}
