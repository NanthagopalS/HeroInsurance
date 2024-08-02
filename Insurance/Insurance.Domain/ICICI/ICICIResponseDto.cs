namespace Insurance.Domain.ICICI.Response;
public class ICICITokenResponse
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public string expires_in { get; set; }
}

public class GeneralInformation
{
    public string vehicleModel { get; set; }
    public string manufacturerName { get; set; }
    public string manufacturingYear { get; set; }
    public string vehicleDescription { get; set; }
    public string rtoLocation { get; set; }
    public double showRoomPrice { get; set; }
    public string chassisPrice { get; set; }
    public string bodyPrice { get; set; }
    public string seatingCapacity { get; set; }
    public string carryingCapacity { get; set; }
    public string policyInceptionDate { get; set; }
    public string policyEndDate { get; set; }
    public string transactionType { get; set; }
    public string cubicCapacity { get; set; }
    public string proposalDate { get; set; }
    public string referenceProposalDate { get; set; }
    public decimal depriciatedIDV { get; set; }
    public string tenure { get; set; }
    public string tpTenure { get; set; }
    public string registrationDate { get; set; }
    public string percentageOfDepriciation { get; set; }
    public string proposalNumber { get; set; }
    public string referenceProposalNo { get; set; }
    public string customerId { get; set; }
    public string customerType { get; set; }
    public string rtoLocationCode { get; set; }
    public string discountType { get; set; }
    public string discountLoadName { get; set; }
    public double imtDiscountOrLoadingValue { get; set; }
    public string bodyTypeDescription { get; set; }
    public string quoteId { get; set; }
}

public class RiskDetails
{
    public string breakinLoadingAmount { get; set; }
    public double garageCash { get; set; }
    public string biFuelKitOD { get; set; }
    public string biFuelKitTP { get; set; }
    public double fibreGlassFuelTank { get; set; }
    public double emiProtect { get; set; }
    public string basicOD { get; set; }
    public double geographicalExtensionOD { get; set; }
    public string electricalAccessories { get; set; }
    public string nonElectricalAccessories { get; set; }
    public string consumables { get; set; }
    public string zeroDepreciation { get; set; }
    public string returnToInvoice { get; set; }
    public string roadSideAssistance { get; set; }
    public string engineProtect { get; set; }
    public string tyreProtect { get; set; }
    public string keyProtect { get; set; }
    public string lossOfPersonalBelongings { get; set; }
    public string voluntaryDiscount { get; set; }
    public string antiTheftDiscount { get; set; }
    public string automobileAssociationDiscount { get; set; }
    public string handicappedDiscount { get; set; }
    public double emeCover { get; set; }
    public string basicTP { get; set; }
    public double paidDriver { get; set; }
    public double employeesOfInsured { get; set; }
    public double geographicalExtensionTP { get; set; }
    public string paCoverForUnNamedPassenger { get; set; }
    public string paCoverForOwnerDriver { get; set; }
    public string paCoverForPadiDriver { get; set; }
    public double tppD_Discount { get; set; }
    public string bonusDiscount { get; set; }
    public bool paCoverWaiver { get; set; }
    public decimal ncbPercentage { get; set; }
}

public class ICICIResponseDto
{
    public RiskDetails riskDetails { get; set; }
    public double totalOwnDamagePremium { get; set; }
    public string packagePremium { get; set; }
    public string roadSideAssistanceService { get; set; }
    public string deviationMessage { get; set; }
    public bool isQuoteDeviation { get; set; }
    public bool breakingFlag { get; set; }
    public string proposalStatus { get; set; }
    public bool isApprovalRequired { get; set; }
    public double floaterDiscountPercent { get; set; }
    public double floaterDiscountAmount { get; set; }
    public string correlationId { get; set; }
    public GeneralInformation generalInformation { get; set; }
    public string totalLiabilityPremium { get; set; }
    public double specialDiscount { get; set; }
    public double totalTax { get; set; }
    public string finalPremium { get; set; }
    public string message { get; set; }
    public string status { get; set; }
    public double dealersExceptionalResponseValue { get; set; }
    public double dealersExceptionalSubjectToMax { get; set; }
}

public class IDVCalculationResponse
{
    public string VehicleModelStatus { get; set; }
    public double VehicleAge { get; set; }
    public double IDVDepreciationPercent { get; set; }
    public double MinExShowroomDeviationLimit { get; set; }
    public double MaxExShowroomDeviationLimit { get; set; }
    public double MaximumPrice { get; set; }
    public double MinimumPrice { get; set; }
    public double MaxIDV { get; set; }
    public double MinIDV { get; set; }
    public double VehicleSellingPrice { get; set; }
    public bool Status { get; set; }
    public string StatusMessage { get; set; }
    public string ErrorMessage { get; set; }
    public string CorrelationId { get; set; }
}
public class QuoteResponseBody
{
    public string ResponseBody { get; set; }
    public string LeadId { get; set; }
}

