namespace Insurance.Domain.GoDigit.Request;
public class GoDigitRequestDto
{
    public string enquiryId { get; set; }
    public Contract contract { get; set; }
    public Vehicle vehicle { get; set; }
    public PreviousInsurer previousInsurer { get; set; }
    public PospInfo pospInfo { get; set; }
    public string pincode { get; set; }
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
    public int insuredCount { get; set; }
}

public class Cng
{
    public bool selection { get; set; }
    public int insuredAmount { get; set; }
    public int minAllowed { get; set; }
    public int maxAllowed { get; set; }
}

public class Consumables
{
    public bool selection { get; set; }
}

public class Contract
{
    public string insuranceProductCode { get; set; }
    public string subInsuranceProductCode { get; set; }
    public string startDate { get; set; }
    public string endDate { get; set; }
    public string policyHolderType { get; set; }
    public string externalPolicyNumber { get; set; }
    public bool isNCBTransfer { get; set; }
    public string quotationDate { get; set; }
    public Coverages coverages { get; set; }
}

public class Coverages
{
    public string voluntaryDeductible { get; set; }
    public ThirdPartyLiability thirdPartyLiability { get; set; }
    public OwnDamage ownDamage { get; set; }
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

public class Discount
{
    public int userSpecialDiscountPercent { get; set; }
    public double defaultSpecialDiscountPercent { get; set; }
    public double defaultSpecialDiscountPercentWithZeroDepWithPreInspection { get; set; }
    public double defaultSpecialDiscountPercentWithoutZeroDepWithPreInspection { get; set; }
    public double defaultSpecialDiscountPercentWithZeroDepWithoutPreInspection { get; set; }
    public double defaultSpecialDiscountPercentWithoutZeroDepWithoutPreInspection { get; set; }
    public int effectiveSpecialDiscountPercent { get; set; }
    public int effectiveSpecialDiscountPercentWithZeroDep { get; set; }
    public int effectiveSpecialDiscountPercentWithoutZeroDep { get; set; }
    public int effectiveSpecialDiscountPercentWithZeroDepWithPreInspection { get; set; }
    public int effectiveSpecialDiscountPercentWithoutZeroDepWithPreInspection { get; set; }
    public int effectiveSpecialDiscountPercentWithZeroDepWithoutPreInspection { get; set; }
    public int effectiveSpecialDiscountPercentWithoutZeroDepWithoutPreInspection { get; set; }
    public double minSpecialDiscountPercent { get; set; }
    public double maxSpecialDiscountPercent { get; set; }
    public List<Discount> discounts { get; set; }
}

public class Electrical
{
    public bool selection { get; set; }
    public int insuredAmount { get; set; }
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

public class NonElectrical
{
    public bool selection { get; set; }
    public int insuredAmount { get; set; }
}

public class NonFarePaxLL
{
    public bool selection { get; set; }
    public int insuredCount { get; set; }
}

public class OwnDamage
{
    public Discount discount { get; set; }
    public Surcharge surcharge { get; set; }
}

public class PaidDriverLL
{
    public bool selection { get; set; }
}

public class PartsDepreciation
{
    public string claimsCovered { get; set; }
    public bool selection { get; set; }
}

public class PersonalAccident
{
    public bool selection { get; set; }
    public int coverTerm { get; set; }
    public string coverAvailability { get; set; }
}

public class PospInfo
{
    public bool isPOSP { get; set; }
}

public class PreviousInsurer
{
    public bool isPreviousInsurerKnown { get; set; }
    public string previousInsurerCode { get; set; }
    public string previousPolicyNumber { get; set; }
    public string previousPolicyExpiryDate { get; set; }
    public bool isClaimInLastYear { get; set; }
    public string originalPreviousPolicyType { get; set; }
    public string previousPolicyType { get; set; }
    public string previousNoClaimBonus { get; set; }
    public string previousNoClaimBonusValue { get; set; }
    public CurrentThirdPartyPolicy currentThirdPartyPolicy { get; set; }
}

public class CurrentThirdPartyPolicy
{
    public bool isCurrentThirdPartyPolicyActive { get; set; }
    public string currentThirdPartyPolicyInsurerCode { get; set; }
    public string currentThirdPartyPolicyNumber { get; set; }
    public string currentThirdPartyPolicyStartDateTime { get; set; }
    public string currentThirdPartyPolicyExpiryDateTime { get; set; }
}

public class ReturnToInvoice
{
    public bool selection { get; set; }
}

public class RimProtection
{
    public bool selection { get; set; }
}

public class RoadSideAssistance
{
    public bool selection { get; set; }
}


public class Surcharge
{
    public List<string> loadings { get; set; }
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
}

public class UnnamedCleaner
{
    public bool selection { get; set; }
    public int insuredAmount { get; set; }
    public int insuredCount { get; set; }
}

public class UnnamedConductor
{
    public bool selection { get; set; }
    public int insuredAmount { get; set; }
    public int insuredCount { get; set; }
}

public class UnnamedHirer
{
    public bool selection { get; set; }
    public int insuredAmount { get; set; }
    public int insuredCount { get; set; }
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
    public int insuredAmount { get; set; }
    public int insuredCount { get; set; }
}

public class Vehicle
{
    public bool isVehicleNew { get; set; }
    public string vehicleMaincode { get; set; }
    public string licensePlateNumber { get; set; }
    public string vehicleIdentificationNumber { get; set; }
    public string engineNumber { get; set; }
    public string manufactureDate { get; set; }
    public string registrationDate { get; set; }
    public VehicleIDV vehicleIDV { get; set; }
    public string usageType { get; set; }
    public string permitType { get; set; }
    public string motorType { get; set; }
    public List<string> trailers { get; set; }
    public string make { get; set; }
    public string model { get; set; }
}

public class VehicleIDV
{
    public decimal idv { get; set; }
    public decimal defaultIdv { get; set; }
    public decimal minimumIdv { get; set; }
    public decimal maximumIdv { get; set; }
}

public class WorkersCompensationLL
{
    public bool selection { get; set; }
    public int insuredCount { get; set; }
}
