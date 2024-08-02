using Insurance.Domain.HDFC;
using System;

namespace Insurance.Domain.ICICI.Request;


public class ICICICVRequestDto
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
    //public decimal ExShowRoomPrice { get; set; } --N
    public bool IsValidDrivingLicense { get; set; }
    public bool IsMoreThanOneVehicle { get; set; }
    public bool IsNoPrevInsurance { get; set; }
    public PreviousPolicyDetailsCV PreviousPolicyDetails { get; set; }
    public bool IsTransferOfNCB { get; set; }
    public int TransferOfNCBPercent { get; set; }
    public bool IsLegalLiabilityToPaidDriver { get; set; }
    public bool IsPACoverOwnerDriver { get; set; }
    public bool IsPACoverWaiver { get; set; }
    public bool IsVehicleHaveLPG { get; set; }
    public bool IsVehicleHaveCNG { get; set; }
    //public int SIVehicleHaveLPG_CNG { get; set; }
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
    //public bool IsLegaLiabilityToWorkmen { get; set; } --N
    public int NoOfWorkmen { get; set; }
    public bool IsFiberGlassFuelTank { get; set; }
    public bool IsVoluntaryDeductible { get; set; }
    public string VoluntaryDeductiblePlanName { get; set; }
    public bool IsAutomobileAssocnFlag { get; set; }
    public bool IsAntiTheftDisc { get; set; }
    //public bool IsHandicapDisc { get; set; } --N
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
    //public int Tenure { get; set; } --N
    //public int TPTenure { get; set; } --N
    public int PACoverTenure { get; set; } //--N
    public bool IsTyreProtect { get; set; }
    public bool IsEngineProtectPlus { get; set; }
    public string KeyProtectPlan { get; set; }

    //public string TPStartDate { get; set; } --N
    //public string TPEndDate { get; set; } --N
    //public string TPPolicyNo { get; set; } --N
    //public string TPInsurerName { get; set; } --N
    public string EngineNumber { get; set; } //--N
    public string ChassisNumber { get; set; } //--N
    public string RegistrationNumber { get; set; } //--N
    public CustomerDetails CustomerDetails { get; set; }



    //------New CV Paramaters
    public string ProductCode { get; set; }
    public double VehiclebodyPrice { get; set; }
    public double VehiclechassisPrice { get; set; }
    public int SI_VehicleLPGCNG_KIT { get; set; } // Refer SIVehicleHaveLPG_CNG
    public bool IsPrivateUse { get; set; }
    public bool IsLimitedToOwnPremises { get; set; }
    public bool IsNonFarePayingPassengers { get; set; }
    public Int32 NoOfNonFarePayingPassenger { get; set; }
    public bool IsHirerOrHirersEmployee { get; set; }
    public Int32 NoOfHirerOrHirersEmployee { get; set; }
    public bool InclusionOfIMT { get; set; }
    public Int32 NoOfCleanerOrConductor { get; set; }
    public Int32 NoOfTrailerTowed { get; set; }
    public string TrailerType { get; set; }
    public bool IsLegalLiabilityToWorkmen { get; set; }
    public bool OverTurningLoading { get; set; }
    public int VehicleCarryingcapacity { get; set; }
    public int VehicleSeatingCapacity { get; set; }
    public bool IsSchoolBus { get; set; } = false;
    public bool IsNCBProtect { get; set; } = false;
    public string NcbProtectPlanName { get; set; }
    public string CategoryId { get; set; }
}

public class PreviousPolicyDetailsCV
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

    //------New CV Paramaters
    public string TotalNoOfODClaims { get; set; }
    public Int32 PreviousPolicyTenure { get; set; }

}


