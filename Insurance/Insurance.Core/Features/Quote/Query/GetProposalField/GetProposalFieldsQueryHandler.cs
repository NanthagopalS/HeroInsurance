using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Quote.Query.GetProposalField;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.Quote.Query.GetPropoalField
{
    public class GetProposalFieldsQuery : ProposalFieldVm, IRequest<HeroResult<List<ProposalFieldVm>>>
    {
        public string InsurerID { get; set; }
        public string QuoteTransactionId { get; set; }
    }

    internal class GetProposalFieldsQueryHandler : IRequestHandler<GetProposalFieldsQuery, HeroResult<List<ProposalFieldVm>>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;

        public GetProposalFieldsQueryHandler(IQuoteRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<List<ProposalFieldVm>>> Handle(GetProposalFieldsQuery request, CancellationToken cancellationToken)
        {
            var result = await _quoteRepository.GetProposalFields(request.InsurerID, request.QuoteTransactionId, cancellationToken);

            var proposalFieldList = new List<ProposalFieldVm>();

            if (result != null && result.Any())
            {
                var distinctSection = result.DistinctBy(x => x.Section).ToList();
                if (distinctSection != null && distinctSection.Any())
                {
                    foreach (var section in distinctSection)
                    {
                        var sections = result.Where(x => x.Section == section.Section).ToList();
                        proposalFieldList.Add(new ProposalFieldVm
                        {
                            SectionDetails = sections,
                            SectionName = section.Section,
                        });
                    }
                }
                return HeroResult<List<ProposalFieldVm>>.Success(proposalFieldList);
            }
            else
                return HeroResult<List<ProposalFieldVm>>.Fail("No record found");
        }
    }
}
