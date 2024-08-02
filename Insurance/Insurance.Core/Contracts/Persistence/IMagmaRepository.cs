using Insurance.Core.Features.Magma.Queries.GetQuote;
using Insurance.Domain.GoDigit;

namespace Insurance.Core.Contracts.Persistence;
public interface IMagmaRepository
{
    Task<QuoteResponseModel> GetQuote(GetMagmaQuoteQuery request, CancellationToken cancellationToken);
}
