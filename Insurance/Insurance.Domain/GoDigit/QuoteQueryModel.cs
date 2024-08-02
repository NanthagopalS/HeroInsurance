using Insurance.Domain.InsuranceMaster;

namespace Insurance.Domain.GoDigit;
public class QuoteQueryModel
{
    public string Token { get; set; }
    public string LeadId { get; set; }
    public string TransactionId { get; set; }
    public string ProductCode { get; set; }
    public string PolicyTypeId { get; set; }
    public bool IsBrandNewVehicle { get; set; }
    public string VehicleNumber { get; set; }
    public int VariantId { get; set; }
    public int RTOId { get; set; }
    public decimal RecommendedIDV { get; set; }
    public decimal MinIDV { get; set; }
    public decimal MaxIDV { get; set; }
    public decimal IDVValue { get; set; }
    public int SelectedIDV { get; set; }
    public string RegistrationYear { get; set; }
    public int VehicleODTenure { get; set; }
    public int VehicleTPTenure { get; set; }
    public string PolicyStartDate { get; set; }
    public string PolicyEndDate { get; set; }
    public string CurrentPolicyType { get; set; }
    public string CurrentNCBPercentage { get; set; }
    public string CurrentPolicyTypeId { get; set; }
    public string RegistrationDate { get; set; }
    public AddOns AddOns { get; set; }
    public Discounts Discounts { get; set; }
    public Accessories Accessories { get; set; }
    public PACover PACover { get; set; }
    public PreviousPolicy PreviousPolicyDetails { get; set; }
    public VehicleDetails VehicleDetails { get; set; }
    public string State_Id { get; set; }
    public string CityName { get; set; }
    public IEnumerable<ConfigNameValueModel> ConfigNameValueModels { get; set; }
    public string RTOLocationCode { get; set; }
    public string RTOLocationName { get; set; }
    public decimal ExShowRoomPrice { get; set; }
    public decimal ChassisPrice { get; set; }
    public string GSTToState { get; set; }
    public string CityDescription { get; set; }
    public string VoluntaryExcess { get; set; }
    public string GeogExtension { get; set; }
    public string GeogExtensionCode { get; set; }
    public string UnnamedPassangerAndPillonRider { get; set; }
    public string VehicleTypeId { get; set; }
    public string RegistrationStateCode { get; set; }
    public string RegistrationRTOCode { get; set; }
    public string RTOCityName { get; set; }
    public string RTOStateName { get; set; }
    public string PlanType { get; set; }
    public string CountryCode { get; set; }
    public string StateCode { get; set; }
    public string CityCode { get; set; }
    public bool IsSAODMandatry { get; set; }
    public bool IsSATPMandatory { get; set; }
    public Guid CorrelationId { get; set; }
    public string BusinessType { get; set; }
    public string BusinessTypeId { get; set; }
    public string CategoryId { get; set; }
    public string DealID { get; set; }
    public GeoAreaCountries GeoAreaCountries { get; set; } 
    public string DiscountPercentage { get; set; }
}


public class PreviousPolicy
{
    public bool IsPreviousInsurerKnown { get; set; }
    public string PreviousInsurerCode { get; set; }
    public string PreviousPolicyNumber { get; set; }
    public string PreviousPolicyNumberTP { get; set; }
    public string PreviousPolicyStartDateSAOD { get; set; }
    public string PreviousPolicyStartDateSATP { get; set; }
    public string PreviousPolicyExpiryDateSAOD { get; set; }
    public string PreviousPolicyExpiryDateSATP { get; set; }
    public bool IsClaimInLastYear { get; set; }
    public string OriginalPreviousPolicyType { get; set; }
    public string PreviousPolicyType { get; set; }
    public string PreviousNoClaimBonus { get; set; }
    public string PreviousNoClaimBonusValue { get; set; }
    public string PreviousSAODInsurer { get; set; }
    public string PreviousSATPInsurer { get; set; }
}

public class AddOns
{
    public bool IsZeroDebt { get; set; }
    public bool IsRoadSideAssistanceRequired { get; set; }
    public bool IsEngineProtectionRequired { get; set; }
    public bool IsGeoAreaExtension { get; set; }
    public bool IsTyreProtectionRequired { get; set; }
    public bool IsRimProtectionRequired { get; set; }
    public bool IsKeyAndLockProtectionRequired { get; set; }
    public bool IsPersonalBelongingRequired { get; set; }
    public bool IsInvoiceCoverRequired { get; set; }
    public bool IsConsumableRequired { get; set; }
    public bool IsNCBRequired { get; set; }
    public bool IsDailyAllowance { get; set; }
    public bool IsReturnToInvoiceRequired { get; set; }
    public bool IsLossOfDownTimeRequired { get; set; }
    public bool IsRoadSideAssistanceWiderRequired { get; set; }
    public bool IsRoadSideAssistanceAdvanceRequired { get; set; }
    public bool IsTowingRequired { get; set; }
    public bool IsEMIProtectorRequired { get; set; }
    public bool IsLimitedOwnPremisesRequired { get; set; }
    public bool IsGlassFiberRepair { get; set; }
    public bool IsEmergencyTranspotationAndHotelExp { get; set; }
    public bool IsIMT23 { get; set; }
    public string ZeroDebtId { get; set; }
    public string RoadSideAssistanceId { get; set; }
    public string EngineProtectionId { get; set; }
    public string GeoAreaExtensionId { get; set; }
    public string TyreProtectionId { get; set; }
    public string RimProtectionId { get; set; }
    public string KeyAndLockProtectionId { get; set; }
    public string PersonalBelongingId { get; set; }
    public string ConsumableId { get; set; }
    public string NCBId { get; set; }
    public string DailyAllowanceId { get; set; }
    public string ReturnToInvoiceIdId { get; set; }
    public string LossOfDownTimeId { get; set; }
    public string RoadSideAssistanceWiderId { get; set; }
    public string RoadSideAssistanceAdvanceId { get; set; }
    public string TowingId { get; set; }
    public string EMIProtectorId { get; set; }
    public string LimitedOwnPremisesId { get; set; }
    public string IMT23Id { get; set; }
    public bool IsGeoAreaExtentionRequired { get; set; }
    public string GeoAreaExtentionCountryName { get; set; }
    public string PackageName { get; set; }
    public string PackageFlag { get; set; }
}

public class Discounts
{
    public bool IsLimitedTPCoverage { get; set; }
    public bool IsVoluntarilyDeductible { get; set; }
    public bool IsAntiTheft { get; set; }
    public bool IsAAMemberShip { get; set; }    
    public bool IsHandicapDisc { get; set; }
    public string LimitedTPCoverageId { get; set; }
    public string VoluntarilyDeductibleId { get; set; }
    public string AntiTheftId { get; set; }
    public string AAMemberShipId { get; set; }
}

public class Accessories
{
    public bool IsElectrical { get; set; }
    public int ElectricalValue { get; set; }
    public bool IsNonElectrical { get; set; }
    public int NonElectricalValue { get; set; }
    public bool IsCNG { get; set; }
    public int CNGValue { get; set; }
    public string ElectricalId { get; set; }
    public string NonElectricalId { get; set; }
    public string CNGId { get; set; }
}

public class PACover
{
    public bool IsUnnamedHirer { get; set; }
    public int UnnamedHirerValue { get; set; }
    public bool IsUnnamedPillionRider { get; set; }
    public bool IsUnnamedHelper { get; set; }
    public int UnnamedPillonRiderValue { get; set; }
    public bool IsGeoAreaExtension { get; set; }
    public int GeoAreaExtensionValue { get; set; }
    public bool IsUnnamedCleaner { get; set; }
    public int UnnamedCleanerValue { get; set; }
    public bool IsUnnamedConductor { get; set; }
    public int UnnamedConductorValue { get; set; }
    public bool IsUnnamedPassenger { get; set; }
    public int UnnamedPassengerValue { get; set; }
    public bool IsPaidDriver { get; set; }
    public int UnnamedPaidDriverValue { get; set; }
    public bool IsUnnamedPersonalAccidentCover { get; set; }
    public int UnnamedPersonalAccidentCoverValue { get; set; }
    public bool IsUnnamedLLO { get; set; }
    public int UnnamedLLOValue { get; set; }
    public bool IsUnnamedOWNERDRIVER { get; set; }
    public int UnnamedOwnerDriverValue { get; set; }
    public string UnnamedOWNERDRIVERId { get; set; }
    public string UnnamedPassengerId { get; set; }
    public string PaidDriverId { get; set; }
    public string UnnamedPillionRiderId { get; set; }
    public string UnnamedCleanerId { get; set; }
    public string UnnamedConductorId { get; set; }
    public string UnnamedHelperId { get; set; }
    public string UnnamedHirerId { get; set; }
}

public class VehicleDetails
{
    public string IsVehicleNew { get; set; }
    public string VehicleMaincode { get; set; }
    public string LicensePlateNumber { get; set; }
    public string VehicleIdentificationNumber { get; set; }
    public string EngineNumber { get; set; }
    public string ManufactureDate { get; set; }
    public string RegistrationDate { get; set; }
    public VehicleIDV VehicleIDV { get; set; }
    public string UsageType { get; set; }
    public string PermitType { get; set; }
    public string MotorType { get; set; }
    public string VehicleType { get; set; }
    public string VehicleMakeCode { get; set; }
    public string VehicleMake { get; set; }
    public string VehicleModelCode { get; set; }
    public string VehicleModel { get; set; }
    public string VehicleSubTypeCode { get; set; }
    public string VehicleSubType { get; set; }
    public string VehicleVariant { get; set; }
    public string VehicleVariantCode { get; set; }
    public string Fuel { get; set; }
    public string FuelId { get; set; }
    public string VehicleId { get; set; }
    public string RegNo { get; set; }
    public string Zone { get; set; }

    public string VehicleClass { get; set; }
    public string Chassis { get; set; }
    //public string engine{ get; set; }
    public string VehicleColour { get; set; }
    public string RegDate { get; set; }
    public string VehicleCubicCapacity { get; set; }
    public string VehicleSeatCapacity { get; set; }
    public string VehicleSegment { get; set; }
    public bool IsTwoWheeler { get; set; }
    public bool IsFourWheeler { get; set; }
    public bool IsCommercialVehicle { get; set; }
    public string NoOfWheels { get; set; }
    public string GrossVehicleWeight { get; set; }
    public bool IsCommercial { get; set; }
}

public class VehicleIDV
{
    public int Idv { get; set; }
    public int DefaultIdv { get; set; }
    public int MinimumIdv { get; set; }
    public int MaximumIdv { get; set; }

}
public class GeoAreaCountries
{
    public bool IsBhutan { get; set; }
    public bool IsBangladesh { get; set; }
    public bool IsNepal { get; set; }
    public bool IsMaldives { get; set; }
    public bool IsSrilanka { get; set; }
    public bool IsPakistan { get; set; }
}
