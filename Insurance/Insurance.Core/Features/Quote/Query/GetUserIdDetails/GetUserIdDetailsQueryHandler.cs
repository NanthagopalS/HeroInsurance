using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using MediatR;

namespace Insurance.Core.Features.Quote.Query.GetUserIdDetails;

public record GetUserIdDetailsQuery : IRequest<HeroResult<string>>
{
    public string ProposalNumber { get; set; }
    public string InsurerId { get; set; }
}
public class GetUserIdDetailsQueryHandler : IRequestHandler<GetUserIdDetailsQuery, HeroResult<string>>
{
    private readonly IQuoteRepository _quoteRepository;
    public GetUserIdDetailsQueryHandler(IQuoteRepository quoteRepository)
    {
        _quoteRepository = quoteRepository;
    }
    public async Task<HeroResult<string>> Handle(GetUserIdDetailsQuery request, CancellationToken cancellationToken)
    {
        var userId = await _quoteRepository.GetUserIddDetails(request.InsurerId, request.ProposalNumber, cancellationToken);
        if(userId != null) 
        {
            return HeroResult<string>.Success(userId);
        }
        return HeroResult<string>.Fail("Fail to get UserId");
    }
}
