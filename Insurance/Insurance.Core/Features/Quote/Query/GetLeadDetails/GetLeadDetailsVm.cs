using Insurance.Core.Models;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.Quote;

namespace Insurance.Core.Features.Quote.Query.GetLeadDetails;

public class GetLeadDetailsVm
{
    public string InsurerId { get; set; }
    public string VehicleNumber { get; set; }
    public string MakeMonthYear { get; set; }
    public string RegistrationDate { get; set; }
    public string LeadName { get; set; }
    public bool IsPrevPolicy { get; set; }
    public string PrevPolicyNumber { get; set; }
    public string PrevPolicyExpiryDate { get; set; }
    public string PrevPolicyClaims { get; set; }
    public bool IsPrevPolicyClaim { get; set; }
    public string PrevPolicyNCB { get; set; }
    public string PreviousPolicyNumberSAOD { get; set; }
    public string PreviousPolicyExpirySAOD { get; set; }
    public bool IsPACover { get; set; }
    public string Tenure { get; set; }
    public string PolicyTypeId { get; set; }
    public string QuoteTransactionID { get; set; }
    public string TransactionId { get; set; }
    public bool IsSAODDateMandatory { get; set; }
    public bool IsSATPDateMandatory { get; set; }
    public string CompanyName { get; set; }
    public string TotalPremium { get; set; }
    public string GrossPremium { get; set; }
    public string Tax { get; set; }
    public string NCBPercentage { get; set; }
    public string GSTNo { get; set; }
    public string DOB { get; set; }
    public string DateOfIncorporation { get; set; }
    public string PrevPolicyNCBPercentage { get; set; }
    public string Logo { get; set; }
    public string VariantName { get; set; }
    public string ModelName { get; set; }
    public string MakeName { get; set; }
    public string FuelName { get; set; }
    public string IDV { get; set; }
    public string MinIDV { get; set; }
    public string MaxIDV { get; set; }
    public string CashlessGarageCount { get; set; }
    public string CommonResponse { get; set; }
    public string VariantId { get; set; }
    public QuoteResponseModel QuoteResponse { get; set; }
    public QuoteBaseCommand QuoteBaseRequest { get; set; }
    public ServiceTax TaxResponse { get; set; }
    public string BreakinStatus { get; set; }
    public string BreakInMessage { get; set; }
    public string CurrentNCBId { get; set; }
    public string VehicleTypeId { get; set; }
    public bool IsVehicleNumberPresent { get; set; }
    public string RegistrationYear { get; set; }
    public bool IsBrandNew { get; set; }
    public bool isPolicyExpired { get; set; }
    public string SelectedIDV { get; set; }
    public string RTOId { get; set; }
    public string PreviousSAODInsurer { get; set; }
    public string PreviousSATPInsurer { get; set; }
    public string PaymentURL { get; set; }
    public string CubicCapacity { get; set; }
    public string CustomerType { get; set; }
    public string BrandId { get; set; }
    public string ModelId { get; set; }
    public string FuelId { get; set; }
    public string SelfVideoClaims { get; set; }
    public string SelfDescription { get; set; }
    public string GarageDescription { get; set; }
    public List<LeadPreviousCoverDetails> LeadPreviousCoverDetails { get; set; }
    //For Commercial Vehicle Details
    public string CatagoryId { get; set; }
    public string SubCatagoryId { get; set; }
    public string CarrierTypeId { get; set; }
    public string BodyTypeId { get; set; }
    public string IsHazardous { get; set; }
    public string IsTrailer { get; set; }
    public string UsageNatureId { get; set; }
    public string UsageTypeId { get; set; }
    public string PCVVehicleCatagoryId { get; set; }
}

