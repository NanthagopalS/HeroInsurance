using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using MediatR;
using System.Globalization;

namespace Insurance.Core.Features.ICICI.Queries.GetPaymentLink
{
    public class GetICICIPaymentLinkQuery : IRequest<HeroResult<string>>
    {
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
    }
    public class GetICICIPaymentLinkQueryHandler : IRequestHandler<GetICICIPaymentLinkQuery, HeroResult<string>>
    {
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IICICIRepository _iCICIRepository;

        public GetICICIPaymentLinkQueryHandler(IMapper mapper, IQuoteRepository quoteRepository, IICICIRepository iCICIRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _iCICIRepository = iCICIRepository;
        }
        public async Task<HeroResult<string>> Handle(GetICICIPaymentLinkQuery request, CancellationToken cancellationToken)
        {
            var requestModel = _mapper.Map<PaymentStatusRequestModel>(request);
            var breakinPaymentDetails = await _quoteRepository.GetBreakInPaymentDetails(requestModel, cancellationToken);
            if (breakinPaymentDetails != null)
            {
                var response = await _iCICIRepository.GetPaymentLink(breakinPaymentDetails, cancellationToken);
                if (!string.IsNullOrEmpty(response.Item1))
                {
                    var paymentLink = await _quoteRepository.UpdateLeadPaymentLink(request.InsurerId, request.QuoteTransactionId, response.Item1, response.Item2, cancellationToken);
                    if (!string.IsNullOrEmpty(paymentLink))
                    {
                        return HeroResult<string>.Success(paymentLink);
                    }
                }
            }
            return HeroResult<string>.Fail("Payment url expired, Please proceed with new journey");
        }
    }
}
