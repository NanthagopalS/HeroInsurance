namespace Insurance.Domain.GoDigit;
public class GoDigitConfig
{
    public string BaseURL { get; set; }
    public string QuoteURL { get; set; }
    public string AuthCode { get; set; }
    public string InsurerId { get; set; }
    public string InsurerName { get; set; }
    public string InsurerLogo { get; set; }
    public string ProposalURL { get; set; }
    public string PaymentLinkURL { get; set; }
    public string Authorization { get; set; }
    public string PGCancelURL { get; set; }
    public string PGSuccessURL { get; set; }
    public string PGRedirectionURL { get; set; }
    public string PaymentStatusCheckURL { get; set; }
    public string CKYCStatusCheckURL { get; set; }
    public string PolicyDocumentGenerationURL { get; set; }
    public string GetPolicyStatusURL { get; set; }   
}
