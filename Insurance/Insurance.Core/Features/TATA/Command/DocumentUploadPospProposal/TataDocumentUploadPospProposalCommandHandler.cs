using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.TATA.Command.DocumentUploadPospProposal;

public class TataDocumentUploadPospProposalCommand : DocumentUploadModelPostProposal, IRequest<HeroResult<SaveCKYCResponse>>
{
    public string QuoteTransactionId { get; set; }
}
public class TataDocumentUploadPospProposalCommandHandler : IRequestHandler<TataDocumentUploadPospProposalCommand, HeroResult<SaveCKYCResponse>>
{
    private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";
    private readonly ITATARepository _tataRepository;
    private readonly IQuoteRepository _quoteRepository;
    private readonly TATAConfig _tataConfig;
    public TataDocumentUploadPospProposalCommandHandler(ITATARepository tATARepository, IQuoteRepository quoteRepository, IOptions<TATAConfig> options)
    {
        _tataRepository = tATARepository;
        _quoteRepository = quoteRepository;
        _tataConfig = options.Value;
    }

    public async Task<HeroResult<SaveCKYCResponse>> Handle(TataDocumentUploadPospProposalCommand request, CancellationToken cancellationToken)
    {
        string doc_type = "image";
        if(request.POADocumentUploadExtension != null && request.POADocumentUploadExtension.Contains("pdf"))
        {
            doc_type = "pdf";
        }

        var kycData = await _tataRepository.GetDetailsForKYCAfterProposal(request.QuoteTransactionId, cancellationToken);
        POADocumentUploadRequestModel tataCKYCRequestModel = new POADocumentUploadRequestModel()
        {
           LeadId = kycData?.LeadID,
           req_id = kycData?.KYC_RequestId,
           doc_base64 = request.POADocumentUpload,
           doc_type = doc_type,
           proposal_no = kycData?.PolicyNumber
        };
        var result = await _tataRepository.POADocumentUpdload(tataCKYCRequestModel, cancellationToken);

        if (result is not null && result.SaveCKYCResponse is not null && result.SaveCKYCResponse.InsurerStatusCode.Equals(200))
        {
            KYCDetailsModel kycDetailsModel = new KYCDetailsModel()
            {
                LeadId = kycData?.LeadID,
                InsurerId = _tataConfig.InsurerId,
                QuoteTransactionId = request.QuoteTransactionId,
                RequestBody = result?.RequestBody,
                ResponseBody = result?.ResponseBody,
                PhotoId = result?.SaveCKYCResponse?.PhotoId,
                KYCId = result?.SaveCKYCResponse?.KYCId,
                CKYCNumber = result?.SaveCKYCResponse?.CKYCNumber,
                CKYCStatus = result?.SaveCKYCResponse?.KYC_Status
            };
            var kycInsertDetails = await _quoteRepository.InsertKYCDetailsAfterProposal(kycDetailsModel, cancellationToken);
            if (kycInsertDetails is not null)
            {
                return HeroResult<SaveCKYCResponse>.Success(result.SaveCKYCResponse);
            }
        }
        return HeroResult<SaveCKYCResponse>.Fail(ValidationMessage);

    }


}

