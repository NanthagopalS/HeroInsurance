using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.HDFC;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Core.Features.HDFC.Queries.GetCKYCPOA
{
    public class GetCKYCPOAQuery : IRequest<HeroResult<UploadCKYCDocumentResponse>>
    {
        public string QuoteTransactionId { get; set; }
        public string TxnId { get; set; }
        public string Status { get; set; }
        public string KYCId { get; set; }
        public string LeadId { get; set; }
    }
    public class GetCKYCPOAQueryHandler : IRequestHandler<GetCKYCPOAQuery, HeroResult<UploadCKYCDocumentResponse>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly IHDFCRepository _hdfcRepository;
        private readonly HDFCConfig _hdfcConfig;

        public GetCKYCPOAQueryHandler(IQuoteRepository quoteRepository, IMapper mapper, IHDFCRepository hdfcRepository, IOptions<HDFCConfig> options)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
            _hdfcRepository = hdfcRepository;
            _hdfcConfig = options.Value;
        }

        public async Task<HeroResult<UploadCKYCDocumentResponse>> Handle(GetCKYCPOAQuery request, CancellationToken cancellationToken)
        {
            var ckycReqModel = _mapper.Map<HDFCCkycPOAModel>(request);
            var kycVerifiedModel = new UploadCKYCDocumentResponse();
            string poaRequest = string.Empty;
            string poaResponse = string.Empty;
            CreateLeadModel createLeadModel = new CreateLeadModel();
            
            if(ckycReqModel != null && ckycReqModel.KYCId != null && ckycReqModel.Status.ToLower().Equals("approved"))
            {
                
                var res = await _hdfcRepository.GetCKYCPOAResponse(ckycReqModel.QuoteTransactionId,ckycReqModel.KYCId,ckycReqModel.LeadId, cancellationToken);
                poaRequest = res.RequestBody;
                poaResponse= res.ResponseBody;
                kycVerifiedModel = res.uploadCKYCDocumentResponse;
                kycVerifiedModel.InsurerId = _hdfcConfig.InsurerId;
                kycVerifiedModel.TransactionId = ckycReqModel.QuoteTransactionId;
                createLeadModel = res.createLeadModel;
                createLeadModel.CKYCstatus = "Success";
            }
            else
            {
                createLeadModel.kyc_id= ckycReqModel.KYCId;
                createLeadModel.CKYCstatus = "Pending";
            }
            var response = await _quoteRepository.SaveLeadDetails(_hdfcConfig.InsurerId, ckycReqModel.QuoteTransactionId, poaRequest, poaResponse,"POI", createLeadModel,cancellationToken);
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
