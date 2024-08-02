namespace ThirdPartyUtilities.Models.SMS;

public class SendNotificationRequestModel
{
    public string LeadId { get; set; }
    public string MakeName { get; set; }
    public string ModelName { get; set; }
    public string VariantName { get; set; }
    public string Link { get; set; }
    public string GrossPremium { get; set; }
    public string InsurerName { get; set; }
    public string BreakinId { get; set; }
    public string ManufacturingYear { get; set; }
    public string CustomerName { get; set; }
    public string PolicyNumber { get; set; }
    public string SMSPhoneNumber { get; set; }
    public string EmailId { get; set; }
    public string Type { get; set; }
    public string POSPName { get; set; }
    public string POSPMobile { get; set; }
    public string PaymentLink { get; set; }
    public string QuoteTransactionId { get; set; }
    public string InsurerId { get; set; }
    public byte[] PolicyPDF { get; set; }
    public string DocumentId { get; set; }
    public string TransactionId { get; set; }
}
