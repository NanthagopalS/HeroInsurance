using Insurance.Core.Features.Magma.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Magma;
using Microsoft.Extensions.Configuration;
using ThirdPartyUtilities.Models.Signzy;

namespace Insurance.Persistence.ICIntegration.Abstraction;
public interface IMagmaService
{
    Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel query, CancellationToken cancellationToken);
    //public Task<(string Token, string TransactionId, string ProductCode)> GetToken(QuoteQueryModel query, CancellationToken cancellationToken);
}
