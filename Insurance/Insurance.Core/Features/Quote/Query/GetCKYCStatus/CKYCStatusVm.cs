using Insurance.Domain.Quote;

namespace Insurance.Core.Features.Quote.Query.GetCKYCField
{
    public class CKYCStatusVm
    {
        public string SectionName { get; set; }
        public List<ProposalFieldMasterModel> SectionDetails { get; set; }
    }
}
