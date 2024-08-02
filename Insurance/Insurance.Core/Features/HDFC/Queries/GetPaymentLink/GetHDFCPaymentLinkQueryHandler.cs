using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.HDFC.Queries.GetPaymentLink
{

    public class GetHDFCPaymentLinkQuery : IRequest<HeroResult<string>>
    {
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
    }
    public class GetHDFCPaymentLinkQueryHandler : IRequestHandler<GetHDFCPaymentLinkQuery, HeroResult<string>>
    {
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IHDFCRepository _hdfcRepository;

        public GetHDFCPaymentLinkQueryHandler(IMapper mapper, IQuoteRepository quoteRepository, IHDFCRepository hdfcRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _hdfcRepository = hdfcRepository;
        }
        public async Task<HeroResult<string>> Handle(GetHDFCPaymentLinkQuery request, CancellationToken cancellationToken)
        {
            var requestModel = _mapper.Map<PaymentStatusRequestModel>(request);
            var breakinPaymentDetails = await _quoteRepository.GetBreakInPaymentDetails(requestModel, cancellationToken);
            if (breakinPaymentDetails != null)
            {
                var paymentLinkResponse = await _hdfcRepository.GetPaymentLink(breakinPaymentDetails, requestModel.QuoteTransactionId, cancellationToken);
                if (!string.IsNullOrEmpty(paymentLinkResponse))
                {
                    var paymentLink = await _quoteRepository.UpdateLeadPaymentLink(request.InsurerId, request.QuoteTransactionId, paymentLinkResponse, string.Empty, cancellationToken);
                    if (!string.IsNullOrEmpty(paymentLink))
                    {
                        return HeroResult<string>.Success(paymentLink);
                    }
                }
            }
            return HeroResult<string>.Fail("Something went wrong, please try again");
        }
    }
}
