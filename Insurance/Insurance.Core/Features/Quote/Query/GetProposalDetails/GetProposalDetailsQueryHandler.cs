using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.Quote.Query.GetProposalDetails;
public record GetProposalDetailsQuery : IRequest<HeroResult<GetProposalDetailsForPaymentVm>>
{
    public string InsurerId { get; set; }
    public string QuoteTransactionId { get; set; }
    public string RequestBody { get; set; }
}
public class GetProposalDetailsQueryHandler : IRequestHandler<GetProposalDetailsQuery, HeroResult<GetProposalDetailsForPaymentVm>>
{
    private readonly IQuoteRepository _quoteRepository;
    private readonly IMapper _mapper;
    public GetProposalDetailsQueryHandler(IQuoteRepository quoteRepository,IMapper mapper)
    {
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<HeroResult<GetProposalDetailsForPaymentVm>> Handle(GetProposalDetailsQuery request, CancellationToken cancellationToken)
    {
        var proposalRequest = await _quoteRepository.GetProposalDetailsForPayment(request.InsurerId, request.QuoteTransactionId, cancellationToken);

        var listReport = _mapper.Map<GetProposalDetailsForPaymentVm>(proposalRequest);

        if (listReport is not null)
        {
            return HeroResult<GetProposalDetailsForPaymentVm>.Success(listReport);
        }
        return HeroResult<GetProposalDetailsForPaymentVm>.Fail("Fail to get proposal Details");   
    }
}
