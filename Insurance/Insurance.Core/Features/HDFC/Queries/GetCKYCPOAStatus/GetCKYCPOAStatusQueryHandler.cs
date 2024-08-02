using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.HDFC;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.HDFC.Queries.GetCKYCPOAStatus
{
    public class GetCKYCPOAStatusQuery : IRequest<HeroResult<UploadCKYCDocumentResponse>>
    {
        public string QuoteTransactionId { get; set; }
        public string KYCId { get; set; }
    }
    public class GetCKYCPOAStatusQueryHandler :IRequestHandler<GetCKYCPOAStatusQuery,HeroResult<UploadCKYCDocumentResponse>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly IHDFCRepository _hdfcRepository;
        private readonly HDFCConfig _hdfcConfig;

        public GetCKYCPOAStatusQueryHandler(IQuoteRepository quoteRepository, IMapper mapper, IHDFCRepository hdfcRepository, IOptions<HDFCConfig> options)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
            _hdfcRepository = hdfcRepository;
            _hdfcConfig = options.Value;
        }

        public async Task<HeroResult<UploadCKYCDocumentResponse>> Handle(GetCKYCPOAStatusQuery request, CancellationToken cancellationToken)
        {
            var ckycReqModel = _mapper.Map<HDFCCkycPOAModel>(request);
            var kycVerifiedModel = new UploadCKYCDocumentResponse();
            string ckycRequest = string.Empty;
            string poaRequest = string.Empty;
            string poaResponse = string.Empty;
            CreateLeadModel createLeadModel = new CreateLeadModel();

            if (ckycReqModel != null)
            {
                var kycStatusRes = await _hdfcRepository.GetCKYCPOAStatus(ckycReqModel.KYCId,ckycReqModel.LeadId, cancellationToken);
                
                if (kycStatusRes != null && kycStatusRes.data != null && kycStatusRes.data.iskycVerified == 1 && kycStatusRes.data.status.ToLower().Equals("approved"))
                {
                    
                    var kycRes = await _hdfcRepository.GetCKYCPOAResponse(ckycReqModel.QuoteTransactionId,ckycReqModel.KYCId,ckycReqModel.LeadId, cancellationToken);
                    kycVerifiedModel = kycRes.uploadCKYCDocumentResponse;
                    kycVerifiedModel.CKYCStatus = "SUCCESS";
                    createLeadModel = kycRes.createLeadModel;
                    createLeadModel.CKYCstatus = "Success";
                    poaRequest = kycRes.RequestBody;
                    poaResponse = kycRes.ResponseBody;
                    await SaveLeadKYCData(ckycReqModel, kycVerifiedModel, poaRequest, poaResponse, createLeadModel, cancellationToken);

                    if (kycVerifiedModel != null)
                    {
                        return HeroResult<UploadCKYCDocumentResponse>.Success(kycVerifiedModel);
                    }
                    return HeroResult<UploadCKYCDocumentResponse>.Fail("Fail to Fetch CKYC POA Details");
                }
                else if(kycStatusRes != null && kycStatusRes.data != null && kycStatusRes.data.iskycVerified == 0 && kycStatusRes.data.status.ToLower().Equals("rejected"))
                {
                    kycVerifiedModel.CKYCStatus = "FAILED";
                    kycVerifiedModel.InsurerName = "HDFC ERGO";
                    createLeadModel.kyc_id = ckycReqModel.KYCId;
                    createLeadModel.CKYCstatus = "Rejected";
                    await SaveLeadKYCData(ckycReqModel, kycVerifiedModel, poaRequest, poaResponse, createLeadModel, cancellationToken);
                    return HeroResult<UploadCKYCDocumentResponse>.Success(kycVerifiedModel);
                }
                else if (kycStatusRes != null && kycStatusRes.data != null && kycStatusRes.data.iskycVerified == 0 && kycStatusRes.data.status.ToLower().Equals("pending for verification"))
                {
                    kycVerifiedModel.CKYCStatus = "PENDING";
                    kycVerifiedModel.InsurerName = "HDFC ERGO";
                    createLeadModel.kyc_id = ckycReqModel.KYCId;
                    createLeadModel.CKYCstatus = "Pending";
                    await SaveLeadKYCData(ckycReqModel, kycVerifiedModel, poaRequest, poaResponse, createLeadModel,cancellationToken);
                    return HeroResult<UploadCKYCDocumentResponse>.Success(kycVerifiedModel);
                }
            }
            return HeroResult<UploadCKYCDocumentResponse>.Fail("Please send the request parameters");
        }

        private async Task SaveLeadKYCData(HDFCCkycPOAModel ckycReqModel, UploadCKYCDocumentResponse kycVerifiedModel, string poaRequest, string poaResponse, CreateLeadModel createLeadModel,CancellationToken cancellationToken)
        {
            var response = await _quoteRepository.SaveLeadDetails(_hdfcConfig.InsurerId, ckycReqModel.QuoteTransactionId, poaRequest, poaResponse, "POI", createLeadModel, cancellationToken);
            if (response != null)
            {
                kycVerifiedModel.KYCId = response.KYCId;
                kycVerifiedModel.CKYCNumber = response.CKYCNumber;
                kycVerifiedModel.LeadID = response.LeadID;
            }
        }
    }
}
