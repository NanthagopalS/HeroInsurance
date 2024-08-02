using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Oriental;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.Oriental.Command.CKYC
{
    public record OrientalCKYCCommand : IRequest<HeroResult<SaveCKYCResponse>>
    {
        public string QuoteTransactionId { get; set; }
        public string POADocumentUploadFront { get; set; }
        public string POADocumentUploadBack { get; set; }
        public string ProofOfAddress { get; set; }
        public string POADocumentUploadBackExtension { get; set; }
        public string POADocumentUploadFrontExtension { get; set; }
    }
    public class OrientalUploadCKYCCommandHandler : IRequestHandler<OrientalCKYCCommand, HeroResult<SaveCKYCResponse>>
    {
        private readonly IOrientalRepository _orientalRepository;
        private readonly OrientalConfig _orientalConfig;
        private readonly IQuoteRepository _quoteRepository;
        private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";

        public OrientalUploadCKYCCommandHandler(IOrientalRepository orientalRepository,
            IOptions<OrientalConfig> options,
            IQuoteRepository quoteRepository)
        {
            _orientalRepository = orientalRepository;
            _orientalConfig = options.Value;
            _quoteRepository = quoteRepository;
        }

        public async Task<HeroResult<SaveCKYCResponse>> Handle(OrientalCKYCCommand request, CancellationToken cancellationToken)
        {
            var leadDetails = await _orientalRepository.GetDetailsForKYCUpload(request.QuoteTransactionId, cancellationToken);
            var ckycResponse = await _orientalRepository.UploadCKYC(request, leadDetails, cancellationToken);

            if (ckycResponse is not null && ckycResponse.saveCKYCResponse is not null && ckycResponse.saveCKYCResponse.InsurerStatusCode.Equals(200))
            {
                KYCDetailsModel kycDetailsModel = new KYCDetailsModel()
                {
                    LeadId = leadDetails?.LeadID,
                    InsurerId = _orientalConfig.InsurerId,
                    QuoteTransactionId = request.QuoteTransactionId,
                    RequestBody = ckycResponse?.RequestBody,
                    ResponseBody = ckycResponse?.ResponseBody,
                    CKYCStatus = ckycResponse?.saveCKYCResponse?.KYC_Status
                };
                var kycInsertDetails = await _quoteRepository.InsertKYCDetailsAfterProposal(kycDetailsModel, cancellationToken);
                if (kycInsertDetails is not null)
                {
                    return HeroResult<SaveCKYCResponse>.Success(ckycResponse.saveCKYCResponse);
                }
            }
            return HeroResult<SaveCKYCResponse>.Success(ckycResponse.saveCKYCResponse);
        }
    }
}
