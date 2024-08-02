namespace Insurance.Domain.Oriental;

public class OrientalCKYCDetailsModel
{
    public string ProposalNumber { get; set; }
    public string LeadId { get; set; }
    public string CustomerType { get; set; }
    public OrientalProposalDynamicDetail orientalProposalDynamicDetail { get; set; }
}
