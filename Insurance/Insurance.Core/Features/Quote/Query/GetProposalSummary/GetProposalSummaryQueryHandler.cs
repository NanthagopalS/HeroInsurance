using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Quote.Query.GetProposalField;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.Quote.Query.GetProposalSummary
{
    public record GetProposalSummaryQuery : IRequest<HeroResult<List<ProposalFieldVm>>>
    {
        public string InsurerId { get; set; }
        public string VehicleNumber { get; set; }
        public string VariantId { get; set; }
        public string QuoteTransactionId { get; set; }
    }
    internal class GetProposalSummaryQueryHandler : IRequestHandler<GetProposalSummaryQuery, HeroResult<List<ProposalFieldVm>>>
    {
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;

        public GetProposalSummaryQueryHandler(IMapper mapper, IQuoteRepository quoteRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        }

        public async Task<HeroResult<List<ProposalFieldVm>>> Handle(GetProposalSummaryQuery lead, CancellationToken cancellationToken)
        {
            var result = await _quoteRepository.GetProposalSummary(lead.InsurerId,
                                                                   lead.VariantId,
                                                                   lead.VehicleNumber,
                                                                   lead.QuoteTransactionId,
                                                                   cancellationToken);

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
