using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Magma
{
    public class MagmaServiceRequestModel
    {
        public string BusinessType { get; set; }
        public string PolicyProductType { get; set; }
        public string ProposalDate { get; set; }
        public Vehicledetails VehicleDetails { get; set; }
        public Generalproposalinformation GeneralProposalInformation { get; set; }
        public bool PAOwnerCoverApplicable { get; set; }
        public Paownercoverdetails PAOwnerCoverDetails { get; set; }
        public bool AddOnsPlanApplicable { get; set; }
        public Addonsplanapplicabledetails AddOnsPlanApplicableDetails { get; set; }
        public bool OptionalCoverageApplicable { get; set; }
        public Optionalcoveragedetails OptionalCoverageDetails { get; set; }
        public Nomineedetails NomineeDetails { get; set; }
        public string CompulsoryExcessAmount { get; set; }
        public string VoluntaryExcessAmount { get; set; }
        public string ImposedExcessAmount { get; set; }
        public Customerdetails CustomerDetails { get; set; }
        public bool FinancierDetailsApplicable { get; set; }
        public Financierdetails FinancierDetails { get; set; }
        public bool IsPrevPolicyApplicable { get; set; }
        public Prevpolicydetails PrevPolicyDetails { get; set; }
        public bool IsTPPolicyApplicable { get; set; }
        public string PrevTPPolicyDetails { get; set; }
        public string IIBClaimSearchDetails { get; set; }
    }
}

public class Financierdetails
{
    public string FinancierName { get; set; }
    public string FinancierCode { get; set; }
    public string FinancierAddress { get; set; }
    public string AgreementType { get; set; }
    public string BranchName { get; set; }
    public string CityCode { get; set; }
    public string CityName { get; set; }
    public string DistrictCode { get; set; }
    public string DistrictName { get; set; }
    public string Pincode { get; set; }
    public string PincodeLocality { get; set; }
    public string StateCode { get; set; }
    public string StateName { get; set; }
    public string FinBusinessType { get; set; }
    public string LoanAccountNumber { get; set; }
}

public class Vehicledetails
{
    public string RegistrationDate { get; set; }
    public string TempRegistrationDate { get; set; }
    public bool IsVehicleBharatRegistered { get; set; }
    public string BharatVehicleOwnBy { get; set; }
    public string RegistrationNumber { get; set; }
    public string MonthOfManufacture { get; set; }
    public string YearOfManufacture { get; set; }
    public string ChassisNumber { get; set; }
    public string EngineNumber { get; set; }
    public string RTOCode { get; set; }
    public string RTOName { get; set; }
    public string ManufactureCode { get; set; }
    public string ManufactureName { get; set; }
    public string ModelCode { get; set; }
    public string ModelName { get; set; }
    public string HPCC { get; set; }
    public string VehicleClassCode { get; set; }
    public string VehicleSubClassCode { get; set; }
    public string SeatingCapacity { get; set; }
    public string CarryingCapacity { get; set; }
    public string BodyTypeCode { get; set; }
    public string BodyTypeName { get; set; }
    public string FuelType { get; set; }
    public string SeagmentType { get; set; }
    public string TACMakeCode { get; set; }
    public string ElectricVehSpeed { get; set; }
    public string GVW { get; set; }
    public string TypeOfBus { get; set; }
    public string MiscTypeOfVehicle { get; set; }
    public string MiscTypeOfVehicleCode { get; set; }
    public bool IsVehicleUsedComPriPurposes { get; set; }
    public string ExShowroomPrice { get; set; }
    public string IDVofVehicle { get; set; }
    public string HigherIDV { get; set; }
    public string LowerIDV { get; set; }
    public string IDVofChassis { get; set; }
    public string Zone { get; set; }
    public bool IHoldValidPUC { get; set; }
    public bool InsuredHoldsValidPUC { get; set; }
    public Iibclaimsearchdetails IIBClaimSearchDetails { get; set; }
}

public class Iibclaimsearchdetails
{
    public bool AcceptIIBResponse { get; set; }
    public bool RejectIIBResponse { get; set; }
    public string IIBResponseRemarks { get; set; }
    public string UniqueIIBID { get; set; }
}

public class Generalproposalinformation
{
    public string CustomerType { get; set; }
    public string BusineeChannelType { get; set; }
    public string BusinessSource { get; set; }
    public string IntermediaryCode { get; set; }
    public string IntermediaryName { get; set; }
    public string EntityRelationShipCode { get; set; }
    public string EntityRelationShipName { get; set; }
    public string ChannelNumber { get; set; }
    public string DisplayOfficeCode { get; set; }
    public string OfficeCode { get; set; }
    public string OfficeName { get; set; }
    public string BusinessSourceType { get; set; }
    public string SPCode { get; set; }
    public string SPName { get; set; }
    public string POSPCode { get; set; }
    public string POSPName { get; set; }
    public string DetariffLoad { get; set; }
    public string DetariffDis { get; set; }
    public string PolicyEffectiveFromDate { get; set; }
    public string PolicyEffectiveToDate { get; set; }
    public string PolicyEffectiveFromHour { get; set; }
    public string PolicyEffectiveToHour { get; set; }
}

public class Paownercoverdetails
{
    public string PAOwnerSI { get; set; }
    public string PAOwnerTenure { get; set; }
    public bool ValidDrvLicense { get; set; }
    public bool DoNotHoldValidDrvLicense { get; set; }
    public bool Ownmultiplevehicles { get; set; }
    public bool ExistingPACover { get; set; }
}

public class Addonsplanapplicabledetails
{
    public string PlanName { get; set; }
    public bool ReturnToInvoice { get; set; }
    public bool RoadSideAssistance { get; set; }
    public bool InconvenienceAllowance { get; set; }
    public string InconvenienceAllowanceDetails { get; set; }
    public bool ZeroDepreciation { get; set; }
    public bool NCBProtection { get; set; }
    public bool EngineProtector { get; set; }
    public bool KeyReplacement { get; set; }
    public bool LossOfPerBelongings { get; set; }
    public bool Consumables { get; set; }
    public bool TyreGuard { get; set; }
    public bool RimSafeguard { get; set; }
    public bool LossofIncome { get; set; }
    public string LossofIncomeDetails { get; set; }
    public bool EMIProtector { get; set; }
    public string EMIProtectorDetails { get; set; }
    public bool LossofDLorRC { get; set; }
}

public class Optionalcoveragedetails
{
    public bool ElectricalApplicable { get; set; }
    public Electricaldetail[] ElectricalDetails { get; set; }
    public bool NonElectricalApplicable { get; set; }
    public Nonelectricaldetail[] NonElectricalDetails { get; set; }
    public bool PAPaidDriverApplicable { get; set; }
    public Papaiddriverdetails PAPaidDriverDetails { get; set; }
    public bool NamedPACoverApplicable { get; set; }
    public string NamedPACoverDetails { get; set; }
    public bool UnnamedPACoverApplicable { get; set; }
    public Unnamedpacoverdetails UnnamedPACoverDetails { get; set; }
    public bool AAMembershipApplicable { get; set; }
    public string AAMembershipDetails { get; set; }
    public bool FiberFuelTankApplicable { get; set; }
    public string FiberFuelTankDetails { get; set; }
    public bool LLPaidDriverCleanerApplicable { get; set; }
    public string LLPaidDriverCleanerDetails { get; set; }
    public bool LLEmployeesApplicable { get; set; }
    public string LLEmployeesDetails { get; set; }
    public bool AdditionalTowingChargesApplicable { get; set; }
    public string AdditionalTowingChargesDetails { get; set; }
    public bool GeographicalExtensionApplicable { get; set; }
    public string GeographicalExtensionDetails { get; set; }
    public bool TheftAccessoriesApplicable { get; set; }
    public string TheftAccessoriesDetails { get; set; }
    public bool IsSideCarApplicable { get; set; }
    public string SideCarDetails { get; set; }
    public bool TPPDDiscountApplicable { get; set; }
    public bool ApprovedAntiTheftDevice { get; set; }
    public bool CertifiedbyARAI { get; set; }
    public bool IsVehicleforHandicapped { get; set; }
    public bool InbuiltCNGkitApplicable { get; set; }
    public bool InbuiltLPGkitApplicable { get; set; }
    public bool ExternalCNGkitApplicable { get; set; }
    public bool ExternalLPGkitApplicable { get; set; }
    public string ExternalCNGLPGkitDetails { get; set; }
}

public class Papaiddriverdetails
{
    public string NoofPADriver { get; set; }
    public string PAPaiddrvSI { get; set; }
}

public class Unnamedpacoverdetails
{
    public string NoOfPerunnamed { get; set; }
    public string UnnamedPASI { get; set; }
}

public class Electricaldetail
{
    public string Description { get; set; }
    public string ElectricalSI { get; set; }
    public string SerialNumber { get; set; }
    public string YearofManufacture { get; set; }
}

public class Nonelectricaldetail
{
    public string Description { get; set; }
    public string NonElectricalSI { get; set; }
    public string SerialNumber { get; set; }
    public string YearofManufacture { get; set; }
}

public class Nomineedetails
{
    public string NomineeName { get; set; }
    public string NomineeDOB { get; set; }
    public string NomineeRelationWithHirer { get; set; }
    public string PercentageOfShare { get; set; }
    public string GuardianName { get; set; }
    public string GuardianDOB { get; set; }
    public string RelationshoipWithGuardian { get; set; }
}

public class Customerdetails
{
    public string CustomerType { get; set; }
    public string Salutation { get; set; }
    public string CustomerName { get; set; }
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public string Nationality { get; set; }
    public string ContactNo { get; set; }
    public string EmailId { get; set; }
    public string DOB { get; set; }
    public string Gender { get; set; }
    public string MaritalStatus { get; set; }
    public string OccupationCode { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string CityDistrictCode { get; set; }
    public string CityDistrictName { get; set; }
    public string PinCode { get; set; }
    public string PincodeLocality { get; set; }
    public string StateCode { get; set; }
    public string StateName { get; set; }
    public string UIDNo { get; set; }
    public string PanNo { get; set; }
    public string AnnualIncome { get; set; }
    public string GSTNumber { get; set; }
}

public class Prevpolicydetails
{
    public string PrevInsurerCompanyCode { get; set; }
    public string PrevNCBPercentage { get; set; }
    public bool HavingClaiminPrevPolicy { get; set; }
    public string NoOfClaims { get; set; }
    public string PrevPolicyEffectiveFromDate { get; set; }
    public string PrevPolicyEffectiveToDate { get; set; }
    public string PrevPolicyNumber { get; set; }
    public string PrevPolicyType { get; set; }
    public bool PrevAddOnAvialable { get; set; }
    public string PrevPolicyTenure { get; set; }
    public string IIBStatus { get; set; }
    public string PrevInsuranceAddress { get; set; }
}
