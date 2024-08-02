namespace Insurance.Domain.GoDigit.Response;
public class GoDigitResponseDto
{
    public string enquiryId { get; set; }
    public Contract contract { get; set; }
    public Vehicle vehicle { get; set; }
    public PreviousInsurer previousInsurer { get; set; }
    public PospInfo pospInfo { get; set; }
    public PreInspection preInspection { get; set; }
    public List<object> motorTransits { get; set; }
    public MotorBreakIn motorBreakIn { get; set; }
    public string netPremium { get; set; }
    public string grossPremium { get; set; }
    public Discount2 discounts { get; set; }
    public Loadings loadings { get; set; }
    public ServiceTax serviceTax { get; set; }
    public List<object> cesses { get; set; }
    public List<object> informationMessages { get; set; }
    public Error error { get; set; }
}
public class Accessories
{
    public Cng cng { get; set; }
    public Electrical electrical { get; set; }
    public NonElectrical nonElectrical { get; set; }
}

public class Addons
{
    public PartsDepreciation partsDepreciation { get; set; }
    public PersonalBelonging personalBelonging { get; set; }
    public KeyAndLockProtect keyAndLockProtect { get; set; }
    public RoadSideAssistance roadSideAssistance { get; set; }
    public EngineProtection engineProtection { get; set; }
    public TyreProtection tyreProtection { get; set; }
    public RimProtection rimProtection { get; set; }
    public ReturnToInvoice returnToInvoice { get; set; }
    public Consumables consumables { get; set; }
}

public class CleanersLL
{
    public bool selection { get; set; }
}

public class Cng
{
    public bool selection { get; set; }
    public double insuredAmount { get; set; }
    public int minAllowed { get; set; }
    public int maxAllowed { get; set; }
}

public class Consumables
{
    public bool selection { get; set; }
    public string coverAvailability { get; set; }
    public string coverOfferingType { get; set; }
    public string netPremium { get; set; }
    public string netPremiumWithZeroDepWithPreInspection { get; set; }
    public string netPremiumWithZeroDepWithoutPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithoutPreInspection { get; set; }
}

public class Contract
{
    public string insuranceProductCode { get; set; }
    public string subInsuranceProductCode { get; set; }
    public string startDate { get; set; }
    public string endDate { get; set; }
    public string policyHolderType { get; set; }
    public string currentNoClaimBonus { get; set; }
    public bool isNCBTransfer { get; set; }
    public string quotationDate { get; set; }
    public Coverages coverages { get; set; }
}

public class Coverages
{
    public ThirdPartyLiability thirdPartyLiability { get; set; }
    public OwnDamage ownDamage { get; set; }
    public Fire fire { get; set; }
    public Theft theft { get; set; }
    public PersonalAccident personalAccident { get; set; }
    public Accessories accessories { get; set; }
    public Addons addons { get; set; }
    public LegalLiability legalLiability { get; set; }
    public UnnamedPA unnamedPA { get; set; }
    public bool isGeoExt { get; set; }
    public bool isTheftAndConversionRiskIMT43 { get; set; }
    public bool isIMT23 { get; set; }
    public bool isOverturningExclusionIMT47 { get; set; }
}

public class CurrentThirdPartyPolicy
{
}

public class Discount
{
    public double userSpecialDiscountPercent { get; set; }
    public double defaultSpecialDiscountPercent { get; set; }
    public double defaultSpecialDiscountPercentWithZeroDepWithPreInspection { get; set; }
    public double defaultSpecialDiscountPercentWithoutZeroDepWithPreInspection { get; set; }
    public double defaultSpecialDiscountPercentWithZeroDepWithoutPreInspection { get; set; }
    public double defaultSpecialDiscountPercentWithoutZeroDepWithoutPreInspection { get; set; }
    public double effectiveSpecialDiscountPercent { get; set; }
    public double effectiveSpecialDiscountPercentWithZeroDep { get; set; }
    public double effectiveSpecialDiscountPercentWithoutZeroDep { get; set; }
    public double effectiveSpecialDiscountPercentWithZeroDepWithPreInspection { get; set; }
    public double effectiveSpecialDiscountPercentWithoutZeroDepWithPreInspection { get; set; }
    public double effectiveSpecialDiscountPercentWithZeroDepWithoutPreInspection { get; set; }
    public double effectiveSpecialDiscountPercentWithoutZeroDepWithoutPreInspection { get; set; }
    public double minSpecialDiscountPercent { get; set; }
    public double maxSpecialDiscountPercent { get; set; }
    public List<Discount1> discounts { get; set; }
    public List<DiscountsWithZeroDep> discountsWithZeroDep { get; set; }
    public List<DiscountsWithoutZeroDep> discountsWithoutZeroDep { get; set; }
    public List<DiscountsWithZeroDepWithPreInspection> discountsWithZeroDepWithPreInspection { get; set; }
    public List<DiscountsWithoutZeroDepWithPreInspection> discountsWithoutZeroDepWithPreInspection { get; set; }
    public List<DiscountsWithZeroDepWithoutPreInspection> discountsWithZeroDepWithoutPreInspection { get; set; }
    public List<DiscountsWithoutZeroDepWithoutPreInspection> discountsWithoutZeroDepWithoutPreInspection { get; set; }
}

public class Discount2
{
    public string discountType { get; set; }
    public int discountPercent { get; set; }
    public string discountAmount { get; set; }
    public string specialDiscountAmount { get; set; }
    public List<OtherDiscount> otherDiscounts { get; set; }
}

public class DiscountsWithoutZeroDep
{
    public string discountType { get; set; }
    public double discountPercent { get; set; }
    public string discountAmount { get; set; }
}

public class DiscountsWithoutZeroDepWithoutPreInspection
{
    public string discountType { get; set; }
    public double discountPercent { get; set; }
    public string discountAmount { get; set; }
}

public class DiscountsWithoutZeroDepWithPreInspection
{
    public string discountType { get; set; }
    public double discountPercent { get; set; }
    public string discountAmount { get; set; }
}

public class DiscountsWithZeroDep
{
    public string discountType { get; set; }
    public double discountPercent { get; set; }
    public string discountAmount { get; set; }
}
public class Discount1
{
    public string discountType { get; set; }
    public double discountPercent { get; set; }
    public string discountAmount { get; set; }
}

public class DiscountsWithZeroDepWithoutPreInspection
{
    public string discountType { get; set; }
    public double discountPercent { get; set; }
    public string discountAmount { get; set; }
}

public class DiscountsWithZeroDepWithPreInspection
{
    public string discountType { get; set; }
    public double discountPercent { get; set; }
    public string discountAmount { get; set; }
}

public class Electrical
{
    public bool selection { get; set; }
    public double insuredAmount { get; set; }
    public int minAllowed { get; set; }
    public int maxAllowed { get; set; }
}

public class EmployeesLL
{
    public bool selection { get; set; }
    public string netPremium { get; set; }
    public int insuredCount { get; set; }
}

public class EngineProtection
{
    public bool selection { get; set; }
    public string coverAvailability { get; set; }
    public string coverOfferingType { get; set; }
    public string netPremium { get; set; }
    public string netPremiumWithZeroDepWithPreInspection { get; set; }
    public string netPremiumWithZeroDepWithoutPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithoutPreInspection { get; set; }
}

public class Error
{
    public int errorCode { get; set; }
    public int httpCode { get; set; }
    public List<object> validationMessages { get; set; }
    public object errorLink { get; set; }
    public object errorStackTrace { get; set; }
}

public class Fire
{
    public bool selection { get; set; }
}

public class KeyAndLockProtect
{
    public string claimsCovered { get; set; }
    public bool selection { get; set; }
    public string coverAvailability { get; set; }
    public string coverOfferingType { get; set; }
    public string netPremium { get; set; }
    public string netPremiumWithZeroDepWithPreInspection { get; set; }
    public string netPremiumWithZeroDepWithoutPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithoutPreInspection { get; set; }
}

public class LegalLiability
{
    public PaidDriverLL paidDriverLL { get; set; }
    public EmployeesLL employeesLL { get; set; }
    public UnnamedPaxLL unnamedPaxLL { get; set; }
    public CleanersLL cleanersLL { get; set; }
    public NonFarePaxLL nonFarePaxLL { get; set; }
    public WorkersCompensationLL workersCompensationLL { get; set; }
}

public class Loadings
{
    public string totalLoadingAmount { get; set; }
    public List<object> otherLoadings { get; set; }
}

public class MotorBreakIn
{
    public bool isBreakin { get; set; }
    public bool isPreInspectionWaived { get; set; }
    public bool isPreInspectionCompleted { get; set; }
}

public class NonElectrical
{
    public bool selection { get; set; }
    public double insuredAmount { get; set; }
    public int minAllowed { get; set; }
    public int maxAllowed { get; set; }
}

public class NonFarePaxLL
{
    public bool selection { get; set; }
}

public class OtherDiscount
{
    public string discountType { get; set; }
    public double? discountPercent { get; set; }
    public string discountAmount { get; set; }
}

public class OwnDamage
{
    public bool selection { get; set; }
    public string netPremium { get; set; }
    public string traiffPremium { get; set; }
    public string withZeroDepNetPremium { get; set; }
    public string ncbDiscountWithZeroDep { get; set; }
    public string policyNetPremiumWithZeroDep { get; set; }
    public string policyGrossPremiumWithZeroDep { get; set; }
    public string withoutZeroDepNetPremium { get; set; }
    public string ncbDiscountWithoutZeroDep { get; set; }
    public string policyNetPremiumWithoutZeroDep { get; set; }
    public string policyGrossPremiumWithoutZeroDep { get; set; }
    public string netPremiumWithZeroDepWithPreInspection { get; set; }
    public string ncbDiscountWithZeroDepWithPreInspection { get; set; }
    public string policyNetPremiumWithZeroDepWithPreInspection { get; set; }
    public string policyGrossPremiumWithZeroDepWithPreInspection { get; set; }
    public string netPremiumWithZeroDepWithoutPreInspection { get; set; }
    public string ncbDiscountWithZeroDepWithoutPreInspection { get; set; }
    public string policyNetPremiumWithZeroDepWithoutPreInspection { get; set; }
    public string policyGrossPremiumWithZeroDepWithoutPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithPreInspection { get; set; }
    public string ncbDiscountWithoutZeroDepWithPreInspection { get; set; }
    public string policyNetPremiumWithoutZeroDepWithPreInspection { get; set; }
    public string policyGrossPremiumWithoutZeroDepWithPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithoutPreInspection { get; set; }
    public string ncbDiscountWithoutZeroDepWithoutPreInspection { get; set; }
    public string policyNetPremiumWithoutZeroDepWithoutPreInspection { get; set; }
    public string policyGrossPremiumWithoutZeroDepWithoutPreInspection { get; set; }
    public Discount discount { get; set; }
    public Surcharge surcharge { get; set; }
}

public class PaidDriverLL
{
    public bool selection { get; set; }
    public string netPremium { get; set; }
    public int insuredCount { get; set; }
}

public class PartsDepreciation
{
    public string claimsCovered { get; set; }
    public bool selection { get; set; }
    public string coverAvailability { get; set; }
    public string coverOfferingType { get; set; }
    public string netPremium { get; set; }
    public string netPremiumWithZeroDepWithPreInspection { get; set; }
    public string netPremiumWithZeroDepWithoutPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithoutPreInspection { get; set; }
}

public class PersonalAccident
{
    public bool selection { get; set; }
    public int coverTerm { get; set; }
    public string coverAvailability { get; set; }
    public string netPremium { get; set; }
}

public class PersonalBelonging
{
    public string claimsCovered { get; set; }
    public bool selection { get; set; }
    public string coverAvailability { get; set; }
    public string coverOfferingType { get; set; }
    public string netPremium { get; set; }
    public string netPremiumWithZeroDepWithPreInspection { get; set; }
    public string netPremiumWithZeroDepWithoutPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithoutPreInspection { get; set; }
}

public class PospInfo
{
    public bool isPOSP { get; set; }
}

public class PreInspection
{
    public bool isPreInspectionOpted { get; set; }
    public bool isPreInspectionRequired { get; set; }
    public bool isPreInspectionEligible { get; set; }
    public bool isPreInspectionEligibleWithZeroDep { get; set; }
    public bool isPreInspectionEligibleWithoutZeroDep { get; set; }
    public bool isPreInspectionWaived { get; set; }
    public bool isSchoolBusWaiverEligibleWithTPlusFortyEight { get; set; }
    public List<string> preInspectionReasons { get; set; }
    public string piType { get; set; }
}

public class PreviousInsurer
{
    public bool isPreviousInsurerKnown { get; set; }
    public string previousPolicyExpiryDate { get; set; }
    public bool isClaimInLastYear { get; set; }
    public string previousNoClaimBonus { get; set; }
    public CurrentThirdPartyPolicy currentThirdPartyPolicy { get; set; }
}

public class ReturnToInvoice
{
    public bool selection { get; set; }
    public string coverAvailability { get; set; }
    public string coverOfferingType { get; set; }
    public string netPremium { get; set; }
    public string netPremiumWithZeroDepWithPreInspection { get; set; }
    public string netPremiumWithZeroDepWithoutPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithoutPreInspection { get; set; }
}

public class RimProtection
{
    public bool selection { get; set; }
    public string coverAvailability { get; set; }
    public string coverOfferingType { get; set; }
    public string netPremium { get; set; }
    public string netPremiumWithZeroDepWithPreInspection { get; set; }
    public string netPremiumWithZeroDepWithoutPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithoutPreInspection { get; set; }
}

public class RoadSideAssistance
{
    public bool selection { get; set; }
    public string coverAvailability { get; set; }
    public string coverOfferingType { get; set; }
    public string netPremium { get; set; }
}



public class ServiceTax
{
    public string cgst { get; set; }
    public string sgst { get; set; }
    public string igst { get; set; }
    public string utgst { get; set; }
    public string totalTax { get; set; }
    public string taxType { get; set; }
}

public class Surcharge
{
    public List<object> loadings { get; set; }
}

public class Theft
{
    public bool selection { get; set; }
}

public class ThirdPartyLiability
{
    public bool selection { get; set; }
    public string netPremium { get; set; }
    public bool isTPPD { get; set; }
}

public class TyreProtection
{
    public bool selection { get; set; }
    public string coverAvailability { get; set; }
    public string coverOfferingType { get; set; }
    public string netPremium { get; set; }
    public string netPremiumWithZeroDepWithPreInspection { get; set; }
    public string netPremiumWithZeroDepWithoutPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithPreInspection { get; set; }
    public string netPremiumWithoutZeroDepWithoutPreInspection { get; set; }
}

public class UnnamedCleaner
{
    public bool selection { get; set; }
}

public class UnnamedConductor
{
    public bool selection { get; set; }
}

public class UnnamedHirer
{
    public bool selection { get; set; }
}

public class UnnamedPA
{
    public UnnamedPax unnamedPax { get; set; }
    public UnnamedPaidDriver unnamedPaidDriver { get; set; }
    public UnnamedHirer unnamedHirer { get; set; }
    public UnnamedPillionRider unnamedPillionRider { get; set; }
    public UnnamedCleaner unnamedCleaner { get; set; }
    public UnnamedConductor unnamedConductor { get; set; }
}

public class UnnamedPaidDriver
{
    public bool selection { get; set; }
    public double insuredAmount { get; set; }
    public string netPremium { get; set; }
}

public class UnnamedPax
{
    public bool selection { get; set; }
    public double insuredAmount { get; set; }
    public string netPremium { get; set; }
}

public class UnnamedPaxLL
{
    public bool selection { get; set; }
    public string netPremium { get; set; }
    public int insuredCount { get; set; }
}

public class UnnamedPillionRider
{
    public bool selection { get; set; }
}

public class Vehicle
{
    public bool isVehicleNew { get; set; }
    public string vehicleMaincode { get; set; }
    public string licensePlateNumber { get; set; }
    public string registrationAuthority { get; set; }
    public string vehicleIdentificationNumber { get; set; }
    public string manufactureDate { get; set; }
    public string registrationDate { get; set; }
    public VehicleIDV vehicleIDV { get; set; }
    public List<object> trailers { get; set; }
    public string make { get; set; }
    public string model { get; set; }
}

public class VehicleIDV
{
    public decimal idv { get; set; }
    public decimal defaultIdv { get; set; }
    public int minimumIdv { get; set; }
    public int maximumIdv { get; set; }
}

public class WorkersCompensationLL
{
    public bool selection { get; set; }
}