namespace Insurance.Core.Features.Quote.Command.SaveUpdateLead
{
    public class SaveUpdateLeadVm
    {
        public string InsurerId { get; set; }
        public string LeadID { get; set; }
        public string QuoteTransactionId { get; set;}
        public string ProposalRequestBody { get; set;}
    }
}
