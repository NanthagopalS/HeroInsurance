using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.Quote.Query.GetCKYCField
{

    public record GetCKYCStatusQuery : IRequest<HeroResult<CKYCStatusModel>>
    {
        public string InsurerID { get; set; }
        public string QuoteTransactionId { get; set; }
       
    }
    public class GetCKYCStatusQueryHandler : IRequestHandler<GetCKYCStatusQuery, HeroResult<CKYCStatusModel>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly LogoConfig _logoConfig;
        public GetCKYCStatusQueryHandler(IQuoteRepository quoteRepository, IOptions<LogoConfig> options)
        {
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _logoConfig = options.Value;
        }

        public async Task<HeroResult<CKYCStatusModel>> Handle(GetCKYCStatusQuery request, CancellationToken cancellationToken)
        {
            var result = await _quoteRepository.GetCKYCStstus(request.InsurerID, request.QuoteTransactionId, cancellationToken);
            if (result != null)
            {
                result.Logo = _logoConfig.InsurerLogoURL + result.Logo;
                return HeroResult<CKYCStatusModel>.Success(result);
            }
            return HeroResult<CKYCStatusModel>.Fail("No record found");
        }
    }
}
