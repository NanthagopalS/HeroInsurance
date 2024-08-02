namespace Insurance.Web.Models;

public class PaymentDetailsVm
{
    public int InsurerStatusCode { get; set; }
    public string PaymentTransactionId { get; set; }
    public string InsurerId { get; set; }
    public string QuoteTransactionId { get; set; }
    public string ApplicationId { get; set; }
    public string LeadId { get; set; }
    public string LeadName { get; set; }
    public string ProposalNumber { get; set; }
    public string PaymentTransactionNumber { get; set; }
    public string Amount { get; set; }
    public string PaymentStatus { get; set; }
    public string CKYCStatus { get; set; }
    public string CKYCLink { get; set; }
    public string CKYCFailedReason { get; set; }
    public string PolicyType { get; set; }
    public string Logo { get; set; }
    public string PolicyDocumentLink { get; set; }
    public string DocumentId { get; set; }
    public string PolicyNumber { get; set; }
    public string CustomerId { get; set; }
}

public class Data
{
    public dynamic result { get; set; }
    public bool failed { get; set; }
    public List<object> messages { get; set; }
    public bool succeeded { get; set; }
    public List<object> validationFailures { get; set; }
}

public class HeroResult
{
    public string statusCode { get; set; }
    public string message { get; set; }
    public Data data { get; set; }
}
public class ICICIHeroResult
{
    public string statusCode { get; set; }
    public string message { get; set; }
    public PaymentDetailsVm data { get; set; }
}

public class UIICHeroResult
{
    public string statusCode { get; set; }
    public string message { get; set; }
    public LeadDetails data { get; set; }
}

public class GetDatailsHeroResult
{
    public string statusCode { get; set; }
    public string message { get; set; }
    public List<string> data { get; set; }
}
