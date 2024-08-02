using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.GoDigit.Queries.GetPaymentLink
{
    public class GetGoDigitPaymentLinkQuery : IRequest<HeroResult<string>>
    {
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
    }
    public class GetGoDigitPaymentLinkQueryHandler : IRequestHandler<GetGoDigitPaymentLinkQuery, HeroResult<string>>
    {
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IGoDigitRepository _goDigitRepository;

        public GetGoDigitPaymentLinkQueryHandler(IMapper mapper, IQuoteRepository quoteRepository, IGoDigitRepository goDigitRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _goDigitRepository = goDigitRepository;
        }
        public async Task<HeroResult<string>> Handle(GetGoDigitPaymentLinkQuery request, CancellationToken cancellationToken)
        {
            var requestModel = _mapper.Map<PaymentStatusRequestModel>(request);
            var breakinPaymentDetails = await _quoteRepository.GetBreakInPaymentDetails(requestModel, cancellationToken);
            if(breakinPaymentDetails != null)
            {
                var godigitPaymentRes = await _goDigitRepository.GetPaymentLink(breakinPaymentDetails.LeadId,breakinPaymentDetails.ApplicationId, cancellationToken);
                if (godigitPaymentRes != null && godigitPaymentRes.InsurerStatusCode.Equals(200))
                {
                    var paymentLink = await _quoteRepository.UpdateLeadPaymentLink(request.InsurerId, request.QuoteTransactionId, godigitPaymentRes.PaymentURL, string.Empty, cancellationToken);
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
