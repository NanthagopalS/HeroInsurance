using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using MediatR;

namespace Insurance.Core.Features.Magma.Queries.GetQuote;
public class GetMagmaQuoteQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{

}

public class GetQuoteQueryHandler : IRequestHandler<GetMagmaQuoteQuery, HeroResult<QuoteResponseModel>>
{
    private readonly IMagmaRepository _magmaRepository;

    public GetQuoteQueryHandler(IMagmaRepository magmaRepository)
    {
        _magmaRepository = magmaRepository ?? throw new ArgumentNullException(nameof(magmaRepository));
    }

    public async Task<HeroResult<QuoteResponseModel>> Handle(GetMagmaQuoteQuery request, CancellationToken cancellationToken)
    {
        var quoteData = await _magmaRepository.GetQuote(request, cancellationToken).ConfigureAwait(false);
        if (quoteData is not null)
        {
            return HeroResult<QuoteResponseModel>.Success(quoteData);
        }

        return HeroResult<QuoteResponseModel>.Fail("No record found");
    }
}
