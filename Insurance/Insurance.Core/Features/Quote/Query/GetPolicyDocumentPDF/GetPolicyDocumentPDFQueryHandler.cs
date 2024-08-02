using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.Quote.Query.GetPolicyDocumentPDF
{
    public record GetPolicyDocumentPdfQuery : IRequest<HeroResult<GetPolicyDocumentPdfModel>>
    {
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
    }
    public class GetPolicyDocumentPdfQueryHandler : IRequestHandler<GetPolicyDocumentPdfQuery, HeroResult<GetPolicyDocumentPdfModel>>
    {
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;

        public GetPolicyDocumentPdfQueryHandler(IMapper mapper, IQuoteRepository quoteRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        }
        public async Task<HeroResult<GetPolicyDocumentPdfModel>> Handle(GetPolicyDocumentPdfQuery getPaymentStatusQuery, CancellationToken cancellationToken)
        {
            var policyDocumentRequestModel = _mapper.Map<PaymentStatusRequestModel>(getPaymentStatusQuery);
            var document = await _quoteRepository.GetPolicyDocumentData(policyDocumentRequestModel, cancellationToken);
            GetPolicyDocumentPdfModel getPolicyDocumentPdf = new GetPolicyDocumentPdfModel();
            if (document != null)
            {
                getPolicyDocumentPdf.PDF = document;
                return HeroResult<GetPolicyDocumentPdfModel>.Success(getPolicyDocumentPdf);
            }
            return HeroResult<GetPolicyDocumentPdfModel>.Fail("Something went wrong, please try again");
        }
    }
}

