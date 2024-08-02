namespace Insurance.Domain.GoDigit;

public class QuoteConfirmDetailsResponseModel
{
    public int InsurerStatusCode { get; set; }
    public string NewPremium { get; set; }
    public string OldPremium { get; set; }
    public string PremiumDifference { get; set; }
    public string Logo { get; set; }
    public string InsurerName { get; set; }
    public string TransactionId { get; set; }
    public string InsurerId { get; set; }
    public int IDV { get; set; }
    public int MinIDV { get; set; }
    public int MaxIDV { get; set; }
    public string NCB { get; set; }
    public string TotalPremium { get; set; }
    public string GrossPremium { get; set; }
    public ServiceTaxModel Tax { get; set; }
    public string ValidationMessage { get; set; }
    public bool IsBreakin { get; set; }
    public bool IsSelfInspection { get; set; }
    public bool isNavigateToQuote { get; set; }
    public string CTA { get; set; }
    public bool IsSkipKYC { get; set; }
}
public class ServiceTaxModel
{
    public string cgst { get; set; }
    public string sgst { get; set; }
    public string igst { get; set; }
    public string utgst { get; set; }
    public string totalTax { get; set; }
    public string taxType { get; set; }
}