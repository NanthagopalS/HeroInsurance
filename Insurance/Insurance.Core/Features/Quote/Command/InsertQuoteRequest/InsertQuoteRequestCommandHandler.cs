using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using MediatR;
using Newtonsoft.Json;

namespace Insurance.Core.Features.Quote.Command.InsertQuoteRequest;

public class InsertQuoteRequestCommand : QuoteBaseCommand, IRequest<HeroResult<string>>
{
}
public class InsertQuoteRequestCommandHandler : IRequestHandler<InsertQuoteRequestCommand, HeroResult<string>>
{
    private readonly IQuoteRepository _quoteRepository;

    public InsertQuoteRequestCommandHandler(IQuoteRepository quoteRepository)
    {
        _quoteRepository = quoteRepository;
    }
    public async Task<HeroResult<string>> Handle(InsertQuoteRequestCommand request, CancellationToken cancellationToken)
    {
        await _quoteRepository.InsertQuoteRequest(JsonConvert.SerializeObject(request),request.LeadId, "Quote", string.Empty,cancellationToken);
        return HeroResult<string>.Success("Quotation Stored");
    }
}
