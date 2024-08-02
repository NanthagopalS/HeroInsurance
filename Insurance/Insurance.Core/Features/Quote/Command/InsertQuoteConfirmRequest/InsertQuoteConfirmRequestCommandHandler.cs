using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using MediatR;
using Newtonsoft.Json;

namespace Insurance.Core.Features.Quote.Command.InsertQuoteConfirmRequest;

public class InsertQuoteConfirmRequestCommand : QuoteConfirmRequestModel, IRequest<HeroResult<string>>
{
}
public class InsertQuoteConfirmRequestCommandHandler : IRequestHandler<InsertQuoteConfirmRequestCommand, HeroResult<string>>
{
    private readonly IQuoteRepository _quoteRepository;

    public InsertQuoteConfirmRequestCommandHandler(IQuoteRepository quoteRepository)
    {
        _quoteRepository = quoteRepository;
    }
    public async Task<HeroResult<string>> Handle(InsertQuoteConfirmRequestCommand request, CancellationToken cancellationToken)
    {
        await _quoteRepository.InsertQuoteRequest(JsonConvert.SerializeObject(request), string.Empty, "QuoteConfirm", request.QuoteTransactionId, cancellationToken);
        return HeroResult<string>.Success("Quotation Stored");
    }
}
