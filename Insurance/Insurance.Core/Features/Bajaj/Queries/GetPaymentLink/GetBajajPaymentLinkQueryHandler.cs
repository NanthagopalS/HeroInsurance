using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.Bajaj.Queries.GetPaymentLink
{
    public class GetBajajPaymentLinkQuery : IRequest<HeroResult<string>>
    {
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
    }
    public class GetBajajPaymentLinkQueryHandler : IRequestHandler<GetBajajPaymentLinkQuery, HeroResult<string>>
    {
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IBajajRepository _bajajRepository;

        public GetBajajPaymentLinkQueryHandler(IMapper mapper, IQuoteRepository quoteRepository, IBajajRepository bajajRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _bajajRepository = bajajRepository;
        }
        public async Task<HeroResult<string>> Handle(GetBajajPaymentLinkQuery request, CancellationToken cancellationToken)
        {
            var requestModel = _mapper.Map<PaymentStatusRequestModel>(request);
            var breakinPaymentDetails = await _quoteRepository.GetBreakInPaymentDetails(requestModel, cancellationToken);
            if (breakinPaymentDetails != null)
            {
                var quoteId = await _bajajRepository.GetQuoteTransactionId(requestModel.QuoteTransactionId, cancellationToken);
                var quotedetails = await _quoteRepository.GetQuoteTransactionDetails(quoteId, cancellationToken);
                var response = await _bajajRepository.GetPaymentLink(quotedetails, cancellationToken);
                if (response != null)
                {
                    var paymentLink = await _quoteRepository.UpdateLeadPaymentLink(request.InsurerId, request.QuoteTransactionId, response.quoteResponseModel.PaymentURLLink, string.Empty,cancellationToken);
                    if (!string.IsNullOrEmpty(paymentLink))
                    {
                        response.QuoteTransactionId = quoteId;
                        await _quoteRepository.SaveQuoteTransaction(response, cancellationToken);
                        return HeroResult<string>.Success(paymentLink);
                    }
                }
            }
            return HeroResult<string>.Fail("Something went wrong, please try again");
        }
    }
}
