namespace Insurance.Domain.ICICI;

public class ICICIPaymentTaggingRequestDto
{
    public string IntermediaryID { get; set; }
    public string CorrelationId { get; set; }
    public string dealid { get; set; }
    public bool isMappingRequired { get; set; }
    public bool isTaggingRequired { get; set; }
    public PaymentEntry PaymentEntry { get; set; }
    public PaymentMapping PaymentMapping { get; set; }
    public PaymentTagging PaymentTagging { get; set; }
}

public class PaymentEntry
{
    public OnlineDAEntry onlineDAEntry { get; set; }
}

public class OnlineDAEntry
{
    public string AuthCode { get; set; }
    public string MerchantID { get; set; }
    public string TransactionId { get; set; }
    public float PaymentAmount { get; set; }
    public string InstrumentDate { get; set; }
    public string CustomerID { get; set; }
}

public class PaymentMapping
{
    public CustomerProposal[] customerProposal { get; set; }
}

public class PaymentTagging
{
    public CustomerProposal[] customerProposal { get; set; }
}
public class CustomerProposal
{
    public string CustomerID { get; set; }
    public string ProposalNo { get; set; }
}
