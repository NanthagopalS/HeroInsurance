namespace Insurance.Domain.Chola;

public class CholaConfig
{
    public string BaseURL { get; set; }
    public string QuoteURLComprehensive { get; set; }
    public string QuoteURLSAOD { get; set; }
    public string QuoteURLTP { get; set; }
    public string TokenURL { get; set; }
    public string IdvURL { get; set; }
    public string InsurerId { get; set; }
    public string InsurerName { get; set; }
    public string InsurerLogo { get; set; }
    public string CKYCTokenURL { get; set; }
    public string PrivateKey { get; set; }
    public string VerifyCKYCURL { get; set; }
    public TokenData Token { get; set; }
    public string PaymentWrapperURL { get; set; }
    public string UserCode { get; set; }
    public string PolicyDocumentURL { get; set; }
    public string VerifyCKYCSTATUSURL { get; set; }
    public string ProposalWrapperURL { get; set; }
    public string MakePaymentURL { get; set; }
    public string PGStatusRedirectionURL { get; set; }
    public string AppId { get; set; }
    public string SubscriptionId  { get; set; }
    public string GenerateCheckSumURL { get; set; }
    public string BreakInURL { get; set; }
    public string Productcode { get; set; }
    public string PaymentGenerationURLLink { get; set; }
    public string MerchantId { get; set; }
    public string SecurityId { get; set; }
    public string CheckSumKey { get; set; }
    public string AdditionalInformation2 { get; set; }
    public string Currency { get; set; }
    public string Code1 { get; set; }
    public string Code2 { get; set; }
    public string TieupFlag { get; set; }
    public string IntermediaryCode { get; set; }
    public string BreakInStatusURL { get; set; }
    public string PGCKYCURL { get; set; }
}
public class TokenData
{
    public string grant_type { get; set; }
    public string username { get; set; }
    public string password { get; set; }
}