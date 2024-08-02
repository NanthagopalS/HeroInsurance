using Insurance.Domain.GoDigit.Response;

namespace Insurance.Domain.GoDigit;
public class QuoteResponse
{
    public List<QuoteResponseModel> QuoteResponses { get; set; }
    public decimal? IDV { get; set; }
    public decimal? MinIDV { get; set; }
    public decimal? MaxIDV { get; set; }
}

public class QuoteResponseModel
{
    public string InsurerName { get; set; }
    public string InsurerId { get; set; }
    public int InsurerStatusCode { get; set; }
    public string InsurerLogo { get; set; }
    public int CachlessGarageCount { get; set; }
    public string SelfVideoClaim { get; set; }
    public string InsurerDescription { get; set; }
    public string TotalPremium { get; set; }
    public string GrossPremium { get; set; }
    public int SelectedIDV { get; set; }
    public decimal? IDV { get; set; }
    public decimal? MinIDV { get; set; }
    public decimal? MaxIDV { get; set; }
    public string NCB { get; set; }
    public bool IsRecommended { get; set; }
    public string RecommendedDescription { get; set; }
    public string PolicyStartDate { get; set; }
    public string Tenure { get; set; }
    public BasicCover BasicCover { get; set; }
    public PACovers PACovers { get; set; }
    public AddonCover AddonCover { get; set; }
    public Discount Discount { get; set; }
    public ServiceTax Tax { get; set; }
    public AccessoriesCover AccessoriesCover { get; set; }
    public IEnumerable<CashlessGarage> CashlessGarageList { get; set; }
    public IEnumerable<PremiumBasicDetails> PremiumBasicDetailsList { get; set; }
    public string TransactionID { get; set; }
    public string RTOCode { get; set; }
    public string PlanType { get; set; }
    public bool IsSATPDateMandatory { get; set; }
    public bool IsSAODDateMandatory { get; set; }
    public string RegistrationDate { get; set; }
    public string ManufacturingDate { get; set; }
    public string VehicleNumber { get; set; }
    public string ApplicationId { get; set; }
    public string ProposalNumber { get; set; }
    public string PaymentURLLink { get; set; }
    public bool IsBreakIn { get; set; }
    public bool IsSelfInspection { get; set; }
    public bool isQuoteDeviation { get; set; }
    public bool isApprovalRequired { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentTransactionNumber { get; set; }
    public string BankName { get; set; }
    public string BankPaymentRefNum { get; set; }
    public string PaymentMode { get; set; }
    public string PaymentDate { get; set; }
    public string CKYCStatus { get; set; }
    public string Type { get; set; }
    public string CKYCLink { get; set; }
    public string CKYCFailReason { get; set; }
    public string PolicyDocumentLink { get; set; }
    public string DocumentId { get; set; }
    public string PolicyNumber { get; set; }
    public string CustomerId { get; set; }
    public string ValidationMessage { get; set; }
    public string BreakinStatus { get; set; }
    public string InspectionId { get; set; }
    public bool IsTP { get; set; }
    public string BreakinId { get; set; }
    public bool IsHtml { get; set; }
    public string QuoteId { get; set; }
    public string ProposalId { get; set; }
    public string PolicyId { get; set; }
    public string BreakinInspectionURL { get; set; }
    public string PaymentCorrelationId { get; set; }
    public string GarageDescription { get; set; }
    public string LeadId { get; set; }
    public bool IsDocumentUpload { get; set; }
    public bool IsPOARedirectionURL { get; set; }
    public string NUM_REFERENCE_NUMBER { get; set; }// for UIIC

}

public class BasicCover
{
    public List<NameValueModel> CoverList { get; set; }
}
public class PACovers
{
    public List<NameValueModel> PACoverList { get; set; }
}
public class AddonCover
{
    public List<NameValueModel> AddonList { get; set; }

}

public class Discount
{
    public List<NameValueModel> DiscountList { get; set; }
}

public class AccessoriesCover
{
    public List<NameValueModel> AccessoryList { get; set; }
}

public class CashlessGarage
{
    public string GarageId { get; set; }
    public string WorkshopName { get; set; }
    public string FullAddress { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Pincode { get; set; }
    public string Latitude { get; set; }
    public string WorkLatitudeshopName { get; set; }
    public string ProductType { get; set; }
    public string EmailId { get; set; }
    public string MobileNumber { get; set; }
    public string ContactPerson { get; set; }
}
public class PremiumBasicDetails
{
    public string PremiumBasicDetailsId { get; set; }
    public string Title { get; set; }
    public List<PremiumBasicSubDetails> SubDetailsList { get; set; }
}

public class PremiumBasicSubDetails
{
    public string PremiumBasicDetailId { get; set; }
    public string SubtitleId { get; set; }
    public string Subtitle { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}

public class InsurerInfo
{
    public string Logo { get; set; }
    public string SelfVideoClaims { get; set; }
    public string SelfDescription { get; set; }
    public bool IsRecommended { get; set; }
    public string RecommendedDescription { get; set; }
    public string GarageDescription { get; set; }
}
public class NameValueModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public bool IsApplicable { get; set; }
    public List<NameValueModel> Validation { get; set; }
    public dynamic ValidationObject { get; set; }
}

public class QuoteResponseDetail
{
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public string Stage { get; set; }
    public string InsurerId { get; set; }
    public string LeadId { get; set; }
    public decimal MaxIDV { get; set; }
    public decimal MinIDV { get; set; }
    public decimal RecommendedIDV { get; set; }
}