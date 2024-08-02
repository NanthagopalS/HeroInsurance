using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;

namespace Insurance.Domain.Quote;

public class SaveQuoteTransactionModel
{
    public QuoteResponseModel quoteResponseModel { get; set; }
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public string Stage { get; set; }
    public string InsurerId { get; set; }
    public string LeadId { get; set; }
    public decimal MinIDV { get; set; }
    public decimal MaxIDV { get; set; }
    public decimal RecommendedIDV { get; set; }
    public string TransactionId { get; set; }
    public string PolicyNumber { get; set; }
    public string QuoteTransactionId { get; set; }
    public string PolicyId { get; set; }
    public string SelectedIDV { get; set; }
    public string CustomIDV { get; set; }
    public string PolicyTypeId { get; set; }
    public bool IsPreviousPolicy { get; set; }
    public bool IsBrandNew { get; set; }
    public bool IsPreviousYearClaim { get; set; }
    public string SAODInsurerId { get; set; }
    public string SATPInsurerId { get; set; }
    public string SAODPolicyStartDate { get; set; }
    public string SAODPolicyExpiryDate { get; set; }
    public string SATPPolicyStartDate { get; set; }
    public string SATPPolicyExpiryDate { get; set; }
    public string PreviousYearNCB { get; set; }
    public string RegistrationDate { get; set; }
    public string VehicleNumber { get; set; }
    public bool IsSharePaymentLink { get; set; }
    public string BreakinInspectionURL { get; set; }
    public string CategoryId { get; set; }  
    public string TotalPremium { get; set; }
    public string GrossPremiume { get; set; }
    public string Tax { get; set; }
    public string RTOId { get; set; }
    public string CustomerId { get; set; }
}
