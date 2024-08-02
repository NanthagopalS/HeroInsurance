using Insurance.Domain.HDFC;

namespace Insurance.Domain.ICICI.Request;

public class ICICIRequestDto
{
    public string BusinessType { get; set; }
    public string CustomerType { get; set; }
    public string DealId { get; set; }
    public string PolicyStartDate { get; set; }
    public string PolicyEndDate { get; set; }
    public string VehicleMakeCode { get; set; }
    public string VehicleModelCode { get; set; }
    public string RTOLocationCode { get; set; }
    public string ManufacturingYear { get; set; }
    public string DeliveryOrRegistrationDate { get; set; }
    public string FirstRegistrationDate { get; set; }
    public decimal ExShowRoomPrice { get; set; }
    public bool IsValidDrivingLicense { get; set; }
    public bool IsMoreThanOneVehicle { get; set; }
    public bool IsNoPrevInsurance { get; set; }
    public bool IsTransferOfNCB { get; set; }
    public int TransferOfNCBPercent { get; set; }
    public bool IsLegalLiabilityToPaidDriver { get; set; }
    public bool IsPACoverOwnerDriver { get; set; }
    public bool IsPACoverWaiver { get; set; }
    public bool IsVehicleHaveLPG { get; set; }
    public bool IsVehicleHaveCNG { get; set; }
    public int SIVehicleHaveLPG_CNG { get; set; }
    public string TPPDLimit { get; set; }
    public bool IsHaveElectricalAccessories { get; set; }
    public bool IsHaveNonElectricalAccessories { get; set; }
    public int SIHaveElectricalAccessories { get; set; }
    public int SIHaveNonElectricalAccessories { get; set; }
    public bool IsPACoverUnnamedPassenger { get; set; }
    public decimal SIPACoverUnnamedPassenger { get; set; }
    public bool IsLegalLiabilityToPaidEmployee { get; set; }
    public int NoOfEmployee { get; set; }
    public int NoOfDriver { get; set; }
    public bool IsLegaLiabilityToWorkmen { get; set; }
    public int NoOfWorkmen { get; set; }
    public bool IsFiberGlassFuelTank { get; set; }
    public bool IsVoluntaryDeductible { get; set; }
    public string VoluntaryDeductiblePlanName { get; set; }
    public bool IsAutomobileAssocnFlag { get; set; }
    public bool IsAntiTheftDisc { get; set; }
    public bool IsHandicapDisc { get; set; }
    public bool IsExtensionCountry { get; set; }
    public string ExtensionCountryName { get; set; }
    public bool IsGarageCash { get; set; }
    public string GarageCashPlanName { get; set; }
    public string ZeroDepPlanName { get; set; }
    public string RSAPlanName { get; set; }
    public bool IsConsumables { get; set; }
    public string LossOfPersonalBelongingPlanName { get; set; }
    public bool IsRTIApplicableflag { get; set; }
    public double OtherLoading { get; set; }
    public double OtherDiscount { get; set; }
    public string GSTToState { get; set; }
    public Guid CorrelationId { get; set; }
    public int Tenure { get; set; }
    public int TPTenure { get; set; }
    public int PACoverTenure { get; set; }
    public bool IsTyreProtect { get; set; }
    public bool IsEngineProtectPlus { get; set; }
    public string KeyProtectPlan { get; set; }
    public PreviousPolicyDetails PreviousPolicyDetails { get; set; }
    public string TPStartDate { get; set; }
    public string TPEndDate { get; set; }
    public string TPPolicyNo { get; set; }
    public string TPInsurerName { get; set; }
    public string EngineNumber { get; set; }
    public string ChassisNumber { get; set; }
    public string RegistrationNumber { get; set; }
    public CustomerDetails CustomerDetails { get; set; }
}

public class PreviousPolicyDetails
{
    public string previousPolicyStartDate { get; set; }
    public string previousPolicyEndDate { get; set; }
    public int ClaimOnPreviousPolicy { get; set; }
    public int NoOfClaimsOnPreviousPolicy { get; set; }
    public string PreviousPolicyType { get; set; }
    public string BonusOnPreviousPolicy { get; set; }
    public string PreviousVehicleSaleDate { get; set; }
    public string PreviousPolicyNumber { get; set; }
    public string PreviousInsurerName { get; set; }
}
public class CustomerDetails
{
    public string CustomerType { get; set; }
    public string CustomerName { get; set; }
    public string DateOfBirth { get; set; }
    public string PinCode { get; set; }
    public string PANCardNo { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string AddressLine1 { get; set; }
    public int CountryCode { get; set; }
    public int StateCode { get; set; }
    public int CityCode { get; set; }
    public string AadharNumber { get; set; }
}