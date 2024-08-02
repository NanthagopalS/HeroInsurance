using Insurance.Domain.Quote;

namespace Insurance.Core.Features.Quote.Query.GetProposalField;
public class ProposalFieldVm
{
    public string SectionName { get; set; }
    public List<ProposalFieldMasterModel> SectionDetails { get; set; }
}