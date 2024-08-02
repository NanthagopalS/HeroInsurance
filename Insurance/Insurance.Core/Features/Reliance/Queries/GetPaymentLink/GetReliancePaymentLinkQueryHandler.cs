using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.Reliance.Queries.GetPaymentLink
{

    public class GetReliancePaymentLinkQuery : IRequest<HeroResult<string>>
    {
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
    }
    public class GetReliancePaymentLinkQueryHandler : IRequestHandler<GetReliancePaymentLinkQuery, HeroResult<string>>
    {
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IRelianceRepository _relianceRepository;

        public GetReliancePaymentLinkQueryHandler(IMapper mapper, IQuoteRepository quoteRepository, IRelianceRepository relianceRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _relianceRepository = relianceRepository;
        }
        public async Task<HeroResult<string>> Handle(GetReliancePaymentLinkQuery request, CancellationToken cancellationToken)
        {
            var requestModel = _mapper.Map<PaymentStatusRequestModel>(request);
            var breakinPaymentDetails = await _quoteRepository.GetBreakInPaymentDetails(requestModel, cancellationToken);
            if (breakinPaymentDetails != null)
            {
                var paymentLinkResponse =  _relianceRepository.PaymentURLGeneration(breakinPaymentDetails.ApplicationId, breakinPaymentDetails.GrossPremium, request.QuoteTransactionId,"","","","");
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
