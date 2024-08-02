namespace Insurance.Domain.IFFCO;

public class IFFCOPaymentResponseModel
{
    public string TransactionId { get; set; }
    public string PolicyNumber { get; set; }
    public string PremiumPayable { get; set; }
    public string Product { get; set; }
    public string StatusMessage { get; set; }
    public string ProposalNumber { get; set; }
    public string LeadId { get; set; }
}
