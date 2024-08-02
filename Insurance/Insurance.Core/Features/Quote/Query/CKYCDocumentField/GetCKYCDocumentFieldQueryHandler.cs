using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Quote.Query.GetCKYCField;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.Quote.Query.CKYCDocumentField;

public record GetCKYCDocumentFieldCommand : IRequest<HeroResult<List<ProposalFieldMasterModel>>>
{
    public string InsurerId { get; set; }
    public string DocumentName { get; set; }
}
public class GetCKYCDocumentFieldQueryHandler : IRequestHandler<GetCKYCDocumentFieldCommand, HeroResult<List<ProposalFieldMasterModel>>>
{
    private readonly IQuoteRepository _quoteRepository;

    public GetCKYCDocumentFieldQueryHandler(IQuoteRepository quoteRepository)
    {
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
    }

    public async Task<HeroResult<List<ProposalFieldMasterModel>>> Handle(GetCKYCDocumentFieldCommand request, CancellationToken cancellationToken)
    {
        var result = await _quoteRepository.GetCKYCDocumenFields(request.InsurerId,request.DocumentName, cancellationToken);

        var cKYCFieldList = new List<CKYCFieldVm>();

        if (result != null && result.Any())
        {
            var distinctSection = result.DistinctBy(x => x.Section).ToList();
            if (distinctSection != null && distinctSection.Any())
            {
                foreach (var section in distinctSection)
                {
                    var sections = result.Where(x => x.Section == section.Section).ToList();
                    cKYCFieldList.Add(new CKYCFieldVm
                    {
                        SectionDetails = sections,
                        SectionName = section.Section,
                    });
                }
            }
            return HeroResult<List<ProposalFieldMasterModel>>.Success(result.ToList());
        }
        else
            return HeroResult<List<ProposalFieldMasterModel>>.Fail("No record found");
    }
}
