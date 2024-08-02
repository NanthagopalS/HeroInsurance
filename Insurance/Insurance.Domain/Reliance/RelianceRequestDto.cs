
using System.Xml.Serialization;

[XmlRoot(ElementName = "ClientDetails")]
public class ClientDetails
{

    [XmlElement(ElementName = "ClientType")]
    public int ClientType { get; set; }
}

[XmlRoot(ElementName = "Policy")]
public class Policy
{

    [XmlElement(ElementName = "BusinessType")]
    public int BusinessType { get; set; }

    [XmlElement(ElementName = "Cover_From")]
    public string Cover_From { get; set; }

    [XmlElement(ElementName = "Cover_To")]
    public string Cover_To { get; set; }

    [XmlElement(ElementName = "Branch_Code")]
    public string Branch_Code { get; set; }

    [XmlElement(ElementName = "AgentName")]
    public string AgentName { get; set; }

    [XmlElement(ElementName = "productcode")]
    public string productcode { get; set; }

    [XmlElement(ElementName = "OtherSystemName")]
    public int OtherSystemName { get; set; }

    [XmlElement(ElementName = "isMotorQuote")]
    public bool isMotorQuote { get; set; }

    [XmlElement(ElementName = "isMotorQuoteFlow")]
    public bool isMotorQuoteFlow { get; set; }
}

[XmlRoot(ElementName = "Risk")]
public class Risk
{

    [XmlElement(ElementName = "VehicleMakeID")]
    public int VehicleMakeID { get; set; }

    [XmlElement(ElementName = "VehicleModelID")]
    public int VehicleModelID { get; set; }

    [XmlElement(ElementName = "CubicCapacity")]
    public int CubicCapacity { get; set; }

    [XmlElement(ElementName = "NoOfWheels")]
    public int NoOfWheels { get; set; }

    [XmlElement(ElementName = "Colour")]
    public string Colour { get; set; }

    [XmlElement(ElementName = "RTOLocationID")]
    public int RTOLocationID { get; set; }

    [XmlElement(ElementName = "Rto_RegionCode")]
    public string Rto_RegionCode { get; set; }

    [XmlElement(ElementName = "ExShowroomPrice")]
    public int ExShowroomPrice { get; set; }

    [XmlElement(ElementName = "DateOfPurchase")]
    public string DateOfPurchase { get; set; }

    [XmlElement(ElementName = "ManufactureMonth")]
    public string ManufactureMonth { get; set; }

    [XmlElement(ElementName = "ManufactureYear")]
    public int ManufactureYear { get; set; }

    [XmlElement(ElementName = "VehicleVariant")]
    public string VehicleVariant { get; set; }

    [XmlElement(ElementName = "StateOfRegistrationID")]
    public int StateOfRegistrationID { get; set; }

    [XmlElement(ElementName = "IsVehicleHypothicated")]
    public bool IsVehicleHypothicated { get; set; }

    [XmlElement(ElementName = "FinanceType")]
    public string FinanceType { get; set; }

    [XmlElement(ElementName = "FinancierName")]
    public string FinancierName { get; set; }

    [XmlElement(ElementName = "FinancierAddress")]
    public string FinancierAddress { get; set; }

    [XmlElement(ElementName = "FinancierCity")]
    public string FinancierCity { get; set; }

    [XmlElement(ElementName = "IDV")]
    public string IDV { get; set; }

    [XmlElement(ElementName = "BodyType")]
    public string BodyType { get; set; }

    [XmlElement(ElementName = "OtherColour")]
    public string OtherColour { get; set; }

    [XmlElement(ElementName = "GrossVehicleWeight")]
    public string GrossVehicleWeight { get; set; }

    [XmlElement(ElementName = "Zone")]
    public string Zone { get; set; }

    [XmlElement(ElementName = "LicensedCarryingCapacity")]
    public string LicensedCarryingCapacity { get; set; }

    [XmlElement(ElementName = "PurposeOfUsage")]
    public string PurposeOfUsage { get; set; }

    [XmlElement(ElementName = "EngineNo")]
    public string EngineNo { get; set; }

    [XmlElement(ElementName = "Chassis")]
    public string Chassis { get; set; }

    [XmlElement(ElementName = "TrailerIDV")]
    public string TrailerIDV { get; set; }

    [XmlElement(ElementName = "IsRegAddressSameasCommAddress")]
    public bool IsRegAddressSameasCommAddress { get; set; }

    [XmlElement(ElementName = "IsRegAddressSameasPermanentAddress")]
    public bool IsRegAddressSameasPermanentAddress { get; set; }

    [XmlElement(ElementName = "IsPermanentAddressSameasCommAddress")]
    public bool IsPermanentAddressSameasCommAddress { get; set; }

    [XmlElement(ElementName = "IsInspectionAddressSameasCommAddress")]
    public bool IsInspectionAddressSameasCommAddress { get; set; }

    [XmlElement(ElementName = "SalesManagerCode")]
    public string SalesManagerCode { get; set; }

    [XmlElement(ElementName = "SalesManagerName")]
    public string SalesManagerName { get; set; }

    [XmlElement(ElementName = "BodyIDV")]
    public string BodyIDV { get; set; }

    [XmlElement(ElementName = "ChassisIDV")]
    public string ChassisIDV { get; set; }

    [XmlElement(ElementName = "Rto_State_City")]
    public string Rto_State_City { get; set; }
}

[XmlRoot(ElementName = "Vehicle")]
public class Vehicle
{

    [XmlElement(ElementName = "Registration_Number")]
    public string Registration_Number { get; set; }

    [XmlElement(ElementName = "Registration_date")]
    public string Registration_date { get; set; }

    [XmlElement(ElementName = "TypeOfFuel")]
    public int TypeOfFuel { get; set; }

    [XmlElement(ElementName = "SeatingCapacity")]
    public int SeatingCapacity { get; set; }

    [XmlElement(ElementName = "MiscTypeOfVehicleID")]
    public string MiscTypeOfVehicleID { get; set; }

    [XmlElement(ElementName = "IsNewVehicle")]
    public bool IsNewVehicle { get; set; }

    [XmlElement(ElementName = "BodyIDV")]
    public int BodyIDV { get; set; }

    [XmlElement(ElementName = "ChassisIDV")]
    public int ChassisIDV { get; set; }
    
    [XmlElement(ElementName = "BodyPrice")]
    public long BodyPrice { get; set; }
    
    [XmlElement(ElementName = "ChassisPrice")]
    public long ChassisPrice { get; set; }

    [XmlElement(ElementName = "PCVVehicleCategory")]
    public int PCVVehicleCategory { get; set; }

    [XmlElement(ElementName = "PCVVehicleUsageType")]
    public int PCVVehicleUsageType { get; set; }

    [XmlElement(ElementName = "PCVVehicleSubUsageType")]
    public int PCVVehicleSubUsageType { get; set; }
    
    [XmlElement(ElementName = "ISmanufacturerfullybuild")]
    public bool ISmanufacturerfullybuild { get; set; } = true;
}

[XmlRoot(ElementName = "ElectricalItems")]
public class ElectricalItems
{

    [XmlElement(ElementName = "ElectricalItemsID")]
    public string ElectricalItemsID { get; set; }

    [XmlElement(ElementName = "PolicyId")]
    public string PolicyId { get; set; }

    [XmlElement(ElementName = "SerialNo")]
    public string SerialNo { get; set; }

    [XmlElement(ElementName = "MakeModel")]
    public string MakeModel { get; set; }

    [XmlElement(ElementName = "ElectricPremium")]
    public string ElectricPremium { get; set; }

    [XmlElement(ElementName = "Description")]
    public string Description { get; set; }

    [XmlElement(ElementName = "ElectricalAccessorySlNo")]
    public string ElectricalAccessorySlNo { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }
}

[XmlRoot(ElementName = "ElectricItems")]
public class ElectricItems
{

    [XmlElement(ElementName = "ElectricalItems")]
    public ElectricalItems ElectricalItems { get; set; }
}

[XmlRoot(ElementName = "NonElectricalItems")]
public class NonElectricalItems
{

    [XmlElement(ElementName = "NonElectricalItemsID")]
    public string NonElectricalItemsID { get; set; }

    [XmlElement(ElementName = "PolicyID")]
    public string PolicyID { get; set; }

    [XmlElement(ElementName = "SerialNo")]
    public string SerialNo { get; set; }

    [XmlElement(ElementName = "MakeModel")]
    public string MakeModel { get; set; }

    [XmlElement(ElementName = "NonElectricPremium")]
    public string NonElectricPremium { get; set; }

    [XmlElement(ElementName = "Description")]
    public string Description { get; set; }

    [XmlElement(ElementName = "Category")]
    public string Category { get; set; }

    [XmlElement(ElementName = "NonElectricalAccessorySlNo")]
    public string NonElectricalAccessorySlNo { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }
}

[XmlRoot(ElementName = "NonElectricItems")]
public class NonElectricItems
{

    [XmlElement(ElementName = "NonElectricalItems")]
    public NonElectricalItems NonElectricalItems { get; set; }
}

[XmlRoot(ElementName = "BasicODCoverage")]
public class BasicODCoverage
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "basicODCoverage")]
    public BasicODCoverage basicODCoverage { get; set; }
}

[XmlRoot(ElementName = "GeographicalExtension")]
public class GeographicalExtension
{

    [XmlElement(ElementName = "Countries")]
    public string Countries { get; set; }

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "geographicalExtension")]
    public GeographicalExtension geographicalExtension { get; set; }
}

[XmlRoot(ElementName = "BifuelKit")]
public class BifuelKits
{

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "PolicyCoverDetailsID")]
    public string PolicyCoverDetailsID { get; set; }

    [XmlElement(ElementName = "Fueltype")]
    public string Fueltype { get; set; }

    [XmlElement(ElementName = "ISLpgCng")]
    public bool ISLpgCng { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public int SumInsured { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "BifuelKit")]
    public BifuelKits BifuelKit { get; set; }
}

[XmlRoot(ElementName = "DrivingTuitionCoverage")]
public class DrivingTuitionCoverage
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "drivingTuitionCoverage")]
    public DrivingTuitionCoverage drivingTuitionCoverage { get; set; }
}

[XmlRoot(ElementName = "FibreGlassFuelTank")]
public class FibreGlassFuelTank
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "fibreGlassFuelTank")]
    public FibreGlassFuelTank fibreGlassFuelTank { get; set; }
}

[XmlRoot(ElementName = "AdditionalTowingCoverage")]
public class AdditionalTowingCoverage
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "additionalTowingCoverage")]
    public AdditionalTowingCoverage additionalTowingCoverage { get; set; }
}

[XmlRoot(ElementName = "VoluntaryDeductible")]
public class VoluntaryDeductibles
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "VoluntaryDeductible")]
    public VoluntaryDeductibles VoluntaryDeductible { get; set; }
}

[XmlRoot(ElementName = "AntiTheftDeviceDiscount")]
public class AntiTheftDeviceDiscount
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "antiTheftDeviceDiscount")]
    public AntiTheftDeviceDiscount antiTheftDeviceDiscount { get; set; }
}

[XmlRoot(ElementName = "SpeciallyDesignedforChallengedPerson")]
public class SpeciallyDesignedforChallengedPerson
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "speciallyDesignedforChallengedPerson")]
    public SpeciallyDesignedforChallengedPerson speciallyDesignedforChallengedPerson { get; set; }
}

[XmlRoot(ElementName = "AutomobileAssociationMembershipDiscount")]
public class AutomobileAssociationMembershipDiscount
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "automobileAssociationMembershipDiscount")]
    public AutomobileAssociationMembershipDiscount automobileAssociationMembershipDiscount { get; set; }
}

[XmlRoot(ElementName = "UseOfVehiclesConfined")]
public class UseOfVehiclesConfined
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "useOfVehiclesConfined")]
    public UseOfVehiclesConfined useOfVehiclesConfined { get; set; }
}

[XmlRoot(ElementName = "TotalCover")]
public class TotalCover
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; } 

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "totalCover")]
    public TotalCover totalCover { get; set; }
}

[XmlRoot(ElementName = "RegistrationCost")]
public class RegistrationCost
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }

    [XmlElement(ElementName = "registrationCost")]
    public RegistrationCost registrationCost { get; set; }
}

[XmlRoot(ElementName = "RoadTax")]
public class RoadTax
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "roadTax")]
    public RoadTax roadTax { get; set; }
}

[XmlRoot(ElementName = "InsurancePremium")]
public class InsurancePremium
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }

    [XmlElement(ElementName = "insurancePremium")]
    public InsurancePremium insurancePremium { get; set; }
}

[XmlRoot(ElementName = "NilDepreciationCoverage")]
public class NilDepreciationCoverages
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "ApplicableRate")]
    public string ApplicableRate { get; set; }

    [XmlElement(ElementName = "NilDepreciationCoverage")]
    public NilDepreciationCoverages NilDepreciationCoverage { get; set; }
}

[XmlRoot(ElementName = "SecurePlus")]
public class SecurePluss
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "ApplicableRate")]
    public string ApplicableRate { get; set; }

    [XmlElement(ElementName = "SecurePlus")]
    public SecurePluss SecurePlus { get; set; }
}

[XmlRoot(ElementName = "SecurePremium")]
public class SecurePremiums
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "ApplicableRate")]
    public string ApplicableRate { get; set; }

    [XmlElement(ElementName = "SecurePremium")]
    public SecurePremiums SecurePremium { get; set; }
}

[XmlRoot(ElementName = "BasicLiability")]
public class BasicLiabilitys
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "BasicLiability")]
    public BasicLiabilitys BasicLiability { get; set; }
}

[XmlRoot(ElementName = "TPPDCover")]
public class TPPDCovers
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "TPPDCover")]
    public TPPDCovers TPPDCover { get; set; }
}

[XmlRoot(ElementName = "PACoverToOwner")]
public class PACoverToOwners
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "CPAcovertenure")]
    public int CPAcovertenure { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "AppointeeName")]
    public string AppointeeName { get; set; }

    [XmlElement(ElementName = "NomineeName")]
    public string NomineeName { get; set; }

    [XmlElement(ElementName = "NomineeDOB")]
    public string NomineeDOB { get; set; }

    [XmlElement(ElementName = "NomineeRelationship")]
    public string NomineeRelationship { get; set; }

    [XmlElement(ElementName = "NomineeAddress")]
    public string NomineeAddress { get; set; }

    [XmlElement(ElementName = "OtherRelation")]
    public string OtherRelation { get; set; }

    [XmlElement(ElementName = "PACoverToOwner")]
    public PACoverToOwners PACoverToOwner { get; set; }
}

[XmlRoot(ElementName = "PAToNamedPassenger")]
public class PAToNamedPassengers
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }

    [XmlElement(ElementName = "PassengerName")]
    public string PassengerName { get; set; }

    [XmlElement(ElementName = "NomineeName")]
    public string NomineeName { get; set; }

    [XmlElement(ElementName = "NomineeDOB")]
    public string NomineeDOB { get; set; }

    [XmlElement(ElementName = "NomineeRelationship")]
    public string NomineeRelationship { get; set; }

    [XmlElement(ElementName = "NomineeAddress")]
    public string NomineeAddress { get; set; }

    [XmlElement(ElementName = "OtherRelation")]
    public string OtherRelation { get; set; }

    [XmlElement(ElementName = "AppointeeName")]
    public string AppointeeName { get; set; }

    [XmlElement(ElementName = "PAToNamedPassenger")]
    public PAToNamedPassengers PAToNamedPassenger { get; set; }
}

[XmlRoot(ElementName = "PAToUnNamedPassenger")]
public class PAToUnNamedPassengers
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public int NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public int SumInsured { get; set; }

    [XmlElement(ElementName = "PAToUnNamedPassenger")]
    public PAToUnNamedPassengers PAToUnNamedPassenger { get; set; }
}

[XmlRoot(ElementName = "PAToPaidDriver")]
public class PAToPaidDrivers
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }

    [XmlElement(ElementName = "PAToPaidDriver")]
    public PAToPaidDrivers PAToPaidDriver { get; set; }
}

[XmlRoot(ElementName = "PAToPaidCleaner")]
public class PAToPaidCleaners
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public int SumInsured { get; set; }

    [XmlElement(ElementName = "PAToPaidCleaner")]
    public PAToPaidCleaners PAToPaidCleaner { get; set; }
}

[XmlRoot(ElementName = "LiabilityToPaidDriver")]
public class LiabilityToPaidDrivers
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "LiabilityToPaidDriver")]
    public LiabilityToPaidDrivers LiabilityToPaidDriver { get; set; }
}

[XmlRoot(ElementName = "LiabilityToEmployee")]
public class LiabilityToEmployee
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "PackageName")]
    public string PackageName { get; set; }

    [XmlElement(ElementName = "PolicyCoverID")]
    public string PolicyCoverID { get; set; }

    [XmlElement(ElementName = "liabilityToEmployee")]
    public LiabilityToEmployee liabilityToEmployee { get; set; }
}

[XmlRoot(ElementName = "NFPPIncludingEmployees")]
public class NFPPIncludingEmployees
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "nFPPIncludingEmployees")]
    public NFPPIncludingEmployees nFPPIncludingEmployees { get; set; }
}

[XmlRoot(ElementName = "NFPPExcludingEmployees")]
public class NFPPExcludingEmployees
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "nFPPExcludingEmployees")]
    public NFPPExcludingEmployees nFPPExcludingEmployees { get; set; }
}

[XmlRoot(ElementName = "WorkmenCompensationExcludingDriver")]
public class WorkmenCompensationExcludingDriver
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "workmenCompensationExcludingDriver")]
    public WorkmenCompensationExcludingDriver workmenCompensationExcludingDriver { get; set; }
}

[XmlRoot(ElementName = "PAToConductor")]
public class PAToConductor
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }

    [XmlElement(ElementName = "pAToConductor")]
    public PAToConductor pAToConductor { get; set; }
}

[XmlRoot(ElementName = "LiabilityToConductor")]
public class LiabilityToConductor
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "liabilityToConductor")]
    public LiabilityToConductor liabilityToConductor { get; set; }
}

[XmlRoot(ElementName = "LiabilitytoCoolie")]
public class LiabilitytoCoolie
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "liabilitytoCoolie")]
    public LiabilitytoCoolie liabilitytoCoolie { get; set; }
}

[XmlRoot(ElementName = "IndemnityToHirer")]
public class IndemnityToHirer
{

    [XmlElement(ElementName = "IsMandatory")]
    public bool IsMandatory { get; set; }

    [XmlElement(ElementName = "IsChecked")]
    public bool IsChecked { get; set; }

    [XmlElement(ElementName = "NoOfItems")]
    public string NoOfItems { get; set; }

    [XmlElement(ElementName = "indemnityToHirer")]
    public IndemnityToHirer indemnityToHirer { get; set; }
}

[XmlRoot(ElementName = "TrailerInfo")]
public class TrailerInfo
{

    [XmlElement(ElementName = "MakeandModel")]
    public string MakeandModel { get; set; }

    [XmlElement(ElementName = "IDV")]
    public int IDV { get; set; }

    [XmlElement(ElementName = "Registration_No")]
    public string Registration_No { get; set; }

    [XmlElement(ElementName = "ChassisNumber")]
    public string ChassisNumber { get; set; }

    [XmlElement(ElementName = "ManufactureYear")]
    public string ManufactureYear { get; set; }

    [XmlElement(ElementName = "SerialNumber")]
    public string SerialNumber { get; set; }
}

[XmlRoot(ElementName = "TrailerDetails")]
public class TrailerDetails
{

    [XmlElement(ElementName = "TrailerInfo")]
    public TrailerInfo TrailerInfo { get; set; }
}

[XmlRoot(ElementName = "Cover")]
public class Cover
{

    [XmlElement(ElementName = "PACoverToNamedPassengerSI")]
    public int PACoverToNamedPassengerSI { get; set; }

    [XmlElement(ElementName = "IsPAToUnnamedPassengerCovered")]
    public bool IsPAToUnnamedPassengerCovered { get; set; }

    [XmlElement(ElementName = "NoOfUnnamedPassenegersCovered")]
    public int NoOfUnnamedPassenegersCovered { get; set; }

    [XmlElement(ElementName = "UnnamedPassengersSI")]
    public int UnnamedPassengersSI { get; set; }

    [XmlElement(ElementName = "IsRacingCovered")]
    public bool IsRacingCovered { get; set; }

    [XmlElement(ElementName = "IsLossOfAccessoriesCovered")]
    public bool IsLossOfAccessoriesCovered { get; set; }

    [XmlElement(ElementName = "IsVoluntaryDeductableOpted")]
    public bool IsVoluntaryDeductableOpted { get; set; }

    [XmlElement(ElementName = "VoluntaryDeductableAmount")]
    public int VoluntaryDeductableAmount { get; set; }

    [XmlElement(ElementName = "IsElectricalItemFitted")]
    public bool IsElectricalItemFitted { get; set; }

    [XmlElement(ElementName = "ElectricalItemsTotalSI")]
    public string ElectricalItemsTotalSI { get; set; }

    [XmlElement(ElementName = "IsNonElectricalItemFitted")]
    public bool IsNonElectricalItemFitted { get; set; }

    [XmlElement(ElementName = "NonElectricalItemsTotalSI")]
    public string NonElectricalItemsTotalSI { get; set; }

    [XmlElement(ElementName = "IsGeographicalAreaExtended")]
    public bool IsGeographicalAreaExtended { get; set; }

    [XmlElement(ElementName = "IsBiFuelKit")]
    public bool IsBiFuelKit { get; set; }

    [XmlElement(ElementName = "BiFuelKitSi")]
    public string BiFuelKitSi { get; set; }

    [XmlElement(ElementName = "IsAutomobileAssociationMember")]
    public bool IsAutomobileAssociationMember { get; set; }

    [XmlElement(ElementName = "IsVehicleMadeInIndia")]
    public bool IsVehicleMadeInIndia { get; set; }

    [XmlElement(ElementName = "IsUsedForDrivingTuition")]
    public bool IsUsedForDrivingTuition { get; set; }

    [XmlElement(ElementName = "IsInsuredAnIndividual")]
    public bool IsInsuredAnIndividual { get; set; }

    [XmlElement(ElementName = "IsIndividualAlreadyInsured")]
    public bool IsIndividualAlreadyInsured { get; set; }

    [XmlElement(ElementName = "IsPAToOwnerDriverCoverd")]
    public bool IsPAToOwnerDriverCoverd { get; set; }

    [XmlElement(ElementName = "ISLegalLiabilityToDefenceOfficialDriverCovered")]
    public bool ISLegalLiabilityToDefenceOfficialDriverCovered { get; set; }

    [XmlElement(ElementName = "IsLiabilityToPaidDriverCovered")]
    public bool IsLiabilityToPaidDriverCovered { get; set; }

    [XmlElement(ElementName = "IsLiabilityToEmployeeCovered")]
    public bool IsLiabilityToEmployeeCovered { get; set; }

    [XmlElement(ElementName = "IsPAToDriverCovered")]
    public bool IsPAToDriverCovered { get; set; }

    [XmlElement(ElementName = "IsPAToPaidCleanerCovered")]
    public bool IsPAToPaidCleanerCovered { get; set; }

    [XmlElement(ElementName = "IsAdditionalTowingCover")]
    public bool IsAdditionalTowingCover { get; set; }

    [XmlElement(ElementName = "IsLegalLiabilityToCleanerCovered")]
    public bool IsLegalLiabilityToCleanerCovered { get; set; }

    [XmlElement(ElementName = "IsLegalLiabilityToNonFarePayingPassengersCovered")]
    public bool IsLegalLiabilityToNonFarePayingPassengersCovered { get; set; }

    [XmlElement(ElementName = "IsLegalLiabilityToCoolieCovered")]
    public bool IsLegalLiabilityToCoolieCovered { get; set; }

    [XmlElement(ElementName = "IsCoveredForDamagedPortion")]
    public bool IsCoveredForDamagedPortion { get; set; }

    [XmlElement(ElementName = "IsImportedVehicle")]
    public bool IsImportedVehicle { get; set; }

    [XmlElement(ElementName = "IsFibreGlassFuelTankFitted")]
    public bool IsFibreGlassFuelTankFitted { get; set; }

    [XmlElement(ElementName = "IsConfinedToOwnPremisesCovered")]
    public bool IsConfinedToOwnPremisesCovered { get; set; }

    [XmlElement(ElementName = "IsAntiTheftDeviceFitted")]
    public bool IsAntiTheftDeviceFitted { get; set; }

    [XmlElement(ElementName = "IsTPPDLiabilityRestricted")]
    public bool IsTPPDLiabilityRestricted { get; set; }

    [XmlElement(ElementName = "IsTPPDCover")]
    public bool IsTPPDCover { get; set; }

    [XmlElement(ElementName = "IsBasicODCoverage")]
    public bool IsBasicODCoverage { get; set; }

    [XmlElement(ElementName = "IsBasicLiability")]
    public bool IsBasicLiability { get; set; }

    [XmlElement(ElementName = "IsUseOfVehiclesConfined")]
    public bool IsUseOfVehiclesConfined { get; set; }

    [XmlElement(ElementName = "IsTotalCover")]
    public bool IsTotalCover { get; set; }

    [XmlElement(ElementName = "IsRegistrationCover")]
    public bool IsRegistrationCover { get; set; }

    [XmlElement(ElementName = "IsRoadTaxcover")]
    public bool IsRoadTaxcover { get; set; }

    [XmlElement(ElementName = "IsInsurancePremium")]
    public bool IsInsurancePremium { get; set; }

    [XmlElement(ElementName = "IsCoverageoFTyreBumps")]
    public bool IsCoverageoFTyreBumps { get; set; }

    [XmlElement(ElementName = "IsImportedVehicleCover")]
    public bool IsImportedVehicleCover { get; set; }

    [XmlElement(ElementName = "IsVehicleDesignedAsCV")]
    public bool IsVehicleDesignedAsCV { get; set; }

    [XmlElement(ElementName = "IsWorkmenCompensationExcludingDriver")]
    public bool IsWorkmenCompensationExcludingDriver { get; set; }

    [XmlElement(ElementName = "IsLiabilityForAccidentsInclude")]
    public bool IsLiabilityForAccidentsInclude { get; set; }

    [XmlElement(ElementName = "IsLiabilityForAccidentsExclude")]
    public bool IsLiabilityForAccidentsExclude { get; set; }

    [XmlElement(ElementName = "IsLiabilitytoCoolie")]
    public bool IsLiabilitytoCoolie { get; set; }

    [XmlElement(ElementName = "IsLiabilitytoCleaner")]
    public bool IsLiabilitytoCleaner { get; set; }

    [XmlElement(ElementName = "IsLiabilityToConductor")]
    public bool IsLiabilityToConductor { get; set; }

    [XmlElement(ElementName = "IsPAToConductorCovered")]
    public bool IsPAToConductorCovered { get; set; }

    [XmlElement(ElementName = "IsNFPPIncludingEmployees")]
    public bool IsNFPPIncludingEmployees { get; set; }

    [XmlElement(ElementName = "IsNFPPExcludingEmployees")]
    public bool IsNFPPExcludingEmployees { get; set; }

    [XmlElement(ElementName = "IsNCBRetention")]
    public string IsNCBRetention { get; set; }

    [XmlElement(ElementName = "IsHandicappedDiscount")]
    public bool IsHandicappedDiscount { get; set; }

    [XmlElement(ElementName = "IsTrailerAttached")]
    public bool IsTrailerAttached { get; set; }

    [XmlElement(ElementName = "cAdditionalCompulsoryExcess")]
    public int CAdditionalCompulsoryExcess { get; set; }

    [XmlElement(ElementName = "iNumberOfLegalLiabilityCoveredPaidDrivers")]
    public int INumberOfLegalLiabilityCoveredPaidDrivers { get; set; }

    [XmlElement(ElementName = "NoOfLiabilityCoveredEmployees")]
    public int NoOfLiabilityCoveredEmployees { get; set; }

    [XmlElement(ElementName = "PAToDriverSI")]
    public int PAToDriverSI { get; set; }

    [XmlElement(ElementName = "PAToCleanerSI")]
    public int PAToCleanerSI { get; set; }

    [XmlElement(ElementName = "NumberOfPACoveredPaidDrivers")]
    public int NumberOfPACoveredPaidDrivers { get; set; }

    [XmlElement(ElementName = "NoOfPAtoPaidCleanerCovered")]
    public int NoOfPAtoPaidCleanerCovered { get; set; }

    [XmlElement(ElementName = "AdditionalTowingCharge")]
    public int AdditionalTowingCharge { get; set; }

    [XmlElement(ElementName = "NoOfLegalLiabilityCoveredCleaners")]
    public int NoOfLegalLiabilityCoveredCleaners { get; set; }

    [XmlElement(ElementName = "NoOfLegalLiabilityCoveredNonFarePayingPassengers")]
    public int NoOfLegalLiabilityCoveredNonFarePayingPassengers { get; set; }

    [XmlElement(ElementName = "NoOfLegalLiabilityCoveredCoolies")]
    public int NoOfLegalLiabilityCoveredCoolies { get; set; }

    [XmlElement(ElementName = "iNoOfLegalLiabilityCoveredPeopleOtherThanPaidDriver")]
    public int INoOfLegalLiabilityCoveredPeopleOtherThanPaidDriver { get; set; }

    [XmlElement(ElementName = "ISLegalLiabilityToConductorCovered")]
    public bool ISLegalLiabilityToConductorCovered { get; set; }

    [XmlElement(ElementName = "NoOfLegalLiabilityCoveredConductors")]
    public int NoOfLegalLiabilityCoveredConductors { get; set; }

    [XmlElement(ElementName = "PAToConductorSI")]
    public int PAToConductorSI { get; set; }

    [XmlElement(ElementName = "CompulsoryDeductible")]
    public int CompulsoryDeductible { get; set; }

    [XmlElement(ElementName = "PACoverToOwnerDriver")]
    public int PACoverToOwnerDriver { get; set; }

    [XmlElement(ElementName = "ElectricItems")]
    public ElectricItems ElectricItems { get; set; }

    [XmlElement(ElementName = "NonElectricItems")]
    public NonElectricItems NonElectricItems { get; set; }

    [XmlElement(ElementName = "BasicODCoverage")]
    public BasicODCoverage BasicODCoverage { get; set; }

    [XmlElement(ElementName = "GeographicalExtension")]
    public GeographicalExtension GeographicalExtension { get; set; }

    [XmlElement(ElementName = "IsImt23LampOrTyreTubeOrHeadlightCover")]
    public bool IsImt23LampOrTyreTubeOrHeadlightCover { get; set; }

    [XmlElement(ElementName = "BifuelKit")]
    public BifuelKits BifuelKit { get; set; }

    [XmlElement(ElementName = "DrivingTuitionCoverage")]
    public DrivingTuitionCoverage DrivingTuitionCoverage { get; set; }

    [XmlElement(ElementName = "FibreGlassFuelTank")]
    public FibreGlassFuelTank FibreGlassFuelTank { get; set; }

    [XmlElement(ElementName = "AdditionalTowingCoverage")]
    public AdditionalTowingCoverage AdditionalTowingCoverage { get; set; }

    [XmlElement(ElementName = "VoluntaryDeductible")]
    public VoluntaryDeductibles VoluntaryDeductible { get; set; }

    [XmlElement(ElementName = "AntiTheftDeviceDiscount")]
    public AntiTheftDeviceDiscount AntiTheftDeviceDiscount { get; set; }

    [XmlElement(ElementName = "SpeciallyDesignedforChallengedPerson")]
    public SpeciallyDesignedforChallengedPerson SpeciallyDesignedforChallengedPerson { get; set; }

    [XmlElement(ElementName = "AutomobileAssociationMembershipDiscount")]
    public AutomobileAssociationMembershipDiscount AutomobileAssociationMembershipDiscount { get; set; }

    [XmlElement(ElementName = "UseOfVehiclesConfined")]
    public UseOfVehiclesConfined UseOfVehiclesConfined { get; set; }

    [XmlElement(ElementName = "TotalCover")]
    public TotalCover TotalCover { get; set; }

    [XmlElement(ElementName = "RegistrationCost")]
    public RegistrationCost RegistrationCost { get; set; }

    [XmlElement(ElementName = "RoadTax")]
    public RoadTax RoadTax { get; set; }

    [XmlElement(ElementName = "InsurancePremium")]
    public InsurancePremium InsurancePremium { get; set; }

    [XmlElement(ElementName = "NilDepreciationCoverage")]
    public NilDepreciationCoverages NilDepreciationCoverage { get; set; }

    [XmlElement(ElementName = "SecurePlus")]
    public SecurePluss SecurePlus { get; set; }

    [XmlElement(ElementName = "SecurePremium")]
    public SecurePremiums SecurePremium { get; set; }

    [XmlElement(ElementName = "BasicLiability")]
    public BasicLiabilitys BasicLiability { get; set; }

    [XmlElement(ElementName = "TPPDCover")]
    public TPPDCovers TPPDCover { get; set; }

    [XmlElement(ElementName = "PACoverToOwner")]
    public PACoverToOwners PACoverToOwner { get; set; }

    [XmlElement(ElementName = "PAToNamedPassenger")]
    public PAToNamedPassengers PAToNamedPassenger { get; set; }

    [XmlElement(ElementName = "PAToUnNamedPassenger")]
    public PAToUnNamedPassengers PAToUnNamedPassenger { get; set; }

    [XmlElement(ElementName = "PAToPaidDriver")]
    public PAToPaidDrivers PAToPaidDriver { get; set; }

    [XmlElement(ElementName = "PAToPaidCleaner")]
    public PAToPaidCleaners PAToPaidCleaner { get; set; }

    [XmlElement(ElementName = "LiabilityToPaidDriver")]
    public LiabilityToPaidDrivers LiabilityToPaidDriver { get; set; }

    [XmlElement(ElementName = "LiabilityToEmployee")]
    public LiabilityToEmployee LiabilityToEmployee { get; set; }

    [XmlElement(ElementName = "NFPPIncludingEmployees")]
    public NFPPIncludingEmployees NFPPIncludingEmployees { get; set; }

    [XmlElement(ElementName = "NFPPExcludingEmployees")]
    public NFPPExcludingEmployees NFPPExcludingEmployees { get; set; }

    [XmlElement(ElementName = "WorkmenCompensationExcludingDriver")]
    public WorkmenCompensationExcludingDriver WorkmenCompensationExcludingDriver { get; set; }

    [XmlElement(ElementName = "PAToConductor")]
    public PAToConductor PAToConductor { get; set; }

    [XmlElement(ElementName = "LiabilityToConductor")]
    public LiabilityToConductor LiabilityToConductor { get; set; }

    [XmlElement(ElementName = "LiabilitytoCoolie")]
    public LiabilitytoCoolie LiabilitytoCoolie { get; set; }

    [XmlElement(ElementName = "LegalLiabilitytoCleaner")]
    public string LegalLiabilitytoCleaner { get; set; }

    [XmlElement(ElementName = "IndemnityToHirer")]
    public IndemnityToHirer IndemnityToHirer { get; set; }

    [XmlElement(ElementName = "TrailerDetails")]
    public TrailerDetails TrailerDetails { get; set; }

    [XmlElement(ElementName = "IsSpeciallyDesignedForHandicapped")]
    public bool IsSpeciallyDesignedForHandicapped { get; set; }

    [XmlElement(ElementName = "IsPAToNamedPassenger")]
    public bool IsPAToNamedPassenger { get; set; }

    [XmlElement(ElementName = "IsOverTurningCovered")]
    public bool IsOverTurningCovered { get; set; }

    [XmlElement(ElementName = "IsLLToPersonsEmployedInOperations_PaidDriverCovered")]
    public bool IsLLToPersonsEmployedInOperations_PaidDriverCovered { get; set; }

    [XmlElement(ElementName = "NoOfLLToPersonsEmployedInOperations_PaidDriver")]
    public int NoOfLLToPersonsEmployedInOperations_PaidDriver { get; set; }

    [XmlElement(ElementName = "IsLLToPersonsEmployedInOperations_CleanerConductorCoolieCovered")]
    public bool IsLLToPersonsEmployedInOperations_CleanerConductorCoolieCovered { get; set; }

    [XmlElement(ElementName = "NoOfLLToPersonsEmployedInOperations_CleanerConductorCoolie")]
    public int NoOfLLToPersonsEmployedInOperations_CleanerConductorCoolie { get; set; }

    [XmlElement(ElementName = "IsLLUnderWCActForCarriageOfMoreThanSixEmpCovered")]
    public bool IsLLUnderWCActForCarriageOfMoreThanSixEmpCovered { get; set; }

    [XmlElement(ElementName = "NoOfLLUnderWCAct")]
    public int NoOfLLUnderWCAct { get; set; }

    [XmlElement(ElementName = "IsLLToNFPPNotWorkmenUnderWCAct")]
    public bool IsLLToNFPPNotWorkmenUnderWCAct { get; set; }

    [XmlElement(ElementName = "NoOfLLToNFPPNotWorkmenUnderWCAct")]
    public int NoOfLLToNFPPNotWorkmenUnderWCAct { get; set; }

    [XmlElement(ElementName = "IsIndemnityToHirerCovered")]
    public bool IsIndemnityToHirerCovered { get; set; }

    [XmlElement(ElementName = "IsAccidentToPassengerCovered")]
    public bool IsAccidentToPassengerCovered { get; set; }

    [XmlElement(ElementName = "NoOfAccidentToPassengerCovered")]
    public int NoOfAccidentToPassengerCovered { get; set; }

    [XmlElement(ElementName = "IsNilDepreciation")]
    public bool IsNilDepreciation { get; set; }

    [XmlElement(ElementName = "IsDetariffRateForOverturning")]
    public bool IsDetariffRateForOverturning { get; set; }

    [XmlElement(ElementName = "IsAddOnCoverforTowing")]
    public bool IsAddOnCoverforTowing { get; set; }

    [XmlElement(ElementName = "IsSecurePlus")]
    public bool IsSecurePlus { get; set; }

    [XmlElement(ElementName = "IsSecurePremium")]
    public bool IsSecurePremium { get; set; }

    [XmlElement(ElementName = "AddOnCoverTowingCharge")]
    public int AddOnCoverTowingCharge { get; set; }

    [XmlElement(ElementName = "NCBRetentiontRate")]
    public string NCBRetentiontRate { get; set; }

    [XmlElement(ElementName = "EMIprotectionCover")]
    public string EMIprotectionCover { get; set; }

    [XmlElement(ElementName = "AutomobileAssociationName")]
    public string AutomobileAssociationName { get; set; }

    [XmlElement(ElementName = "AutomobileAssociationNo")]
    public string AutomobileAssociationNo { get; set; }

    [XmlElement(ElementName = "AutomobileAssociationExpiryDate")]
    public string AutomobileAssociationExpiryDate { get; set; }
}

[XmlRoot(ElementName = "PreviousInsuranceDetails")]
public class PreviousInsuranceDetails
{

    [XmlElement(ElementName = "PrevInsuranceID")]
    public string PrevInsuranceID { get; set; }

    [XmlElement(ElementName = "IsVehicleOfPreviousPolicySold")]
    public string IsVehicleOfPreviousPolicySold { get; set; }

    [XmlElement(ElementName = "IsNCBApplicable")]
    public bool IsNCBApplicable { get; set; }

    [XmlElement(ElementName = "PrevYearInsurer")]
    public string PrevYearInsurer { get; set; }

    [XmlElement(ElementName = "PrevYearPolicyNo")]
    public string PrevYearPolicyNo { get; set; }

    [XmlElement(ElementName = "PrevYearInsurerAddress")]  
    public string PrevYearInsurerAddress { get; set; }

    [XmlElement(ElementName = "DocumentProof")]
    public string DocumentProof { get; set; }

    [XmlElement(ElementName = "PrevYearPolicyType")]
    public string PrevYearPolicyType { get; set; }

    [XmlElement(ElementName = "PrevYearPolicyStartDate")]
    public string PrevYearPolicyStartDate { get; set; }

    [XmlElement(ElementName = "PrevYearPolicyEndDate")]
    public string PrevYearPolicyEndDate { get; set; }

    [XmlElement(ElementName = "MTAReason")]
    public string MTAReason { get; set; }

    [XmlElement(ElementName = "PrevYearNCB")]
    public int PrevYearNCB { get; set; }

    [XmlElement(ElementName = "IsInspectionDone")]
    public string IsInspectionDone { get; set; }

    [XmlElement(ElementName = "InspectionDate")]
    public string InspectionDate { get; set; }

    [XmlElement(ElementName = "Inspectionby")]
    public string Inspectionby { get; set; }

    [XmlElement(ElementName = "InspectorName")]
    public string InspectorName { get; set; }

    [XmlElement(ElementName = "IsNCBEarnedAbroad")]
    public string IsNCBEarnedAbroad { get; set; }

    [XmlElement(ElementName = "ODLoading")]
    public string ODLoading { get; set; }

    [XmlElement(ElementName = "IsClaimedLastYear")]
    public string IsClaimedLastYear { get; set; }

    [XmlElement(ElementName = "ODLoadingReason")]
    public string ODLoadingReason { get; set; }

    [XmlElement(ElementName = "PreRateCharged")]
    public string PreRateCharged { get; set; }

    [XmlElement(ElementName = "PreSpecialTermsAndConditions")]
    public string PreSpecialTermsAndConditions { get; set; }

    [XmlElement(ElementName = "IsTrailerNCB")]
    public string IsTrailerNCB { get; set; }

    [XmlElement(ElementName = "InspectionID")]
    public string InspectionID { get; set; }
}

[XmlRoot(ElementName = "NCBEligibility")]
public class NCBEligibility
{

    [XmlElement(ElementName = "NCBEligibilityCriteria")]
    public int NCBEligibilityCriteria { get; set; }

    [XmlElement(ElementName = "NCBReservingLetter")]
    public string NCBReservingLetter { get; set; }

    [XmlElement(ElementName = "PreviousNCB")]
    public int PreviousNCB { get; set; }

    [XmlElement(ElementName = "CurrentNCB")]
    public int CurrentNCB { get; set; }
}

[XmlRoot(ElementName = "PolicyDetails")]
public class PolicyDetails
{

    [XmlElement(ElementName = "CoverDetails")]
    public string CoverDetails { get; set; }

    [XmlElement(ElementName = "TrailerDetails")]
    public string TrailerDetails { get; set; }

    [XmlElement(ElementName = "ClientDetails")]
    public ClientDetails ClientDetails { get; set; }

    [XmlElement(ElementName = "Policy")]
    public Policy Policy { get; set; }

    [XmlElement(ElementName = "Risk")]
    public Risk Risk { get; set; }

    [XmlElement(ElementName = "Vehicle")]
    public Vehicle Vehicle { get; set; }

    [XmlElement(ElementName = "Cover")]
    public Cover Cover { get; set; }

    [XmlElement(ElementName = "PreviousInsuranceDetails")]
    public PreviousInsuranceDetails PreviousInsuranceDetails { get; set; }

    [XmlElement(ElementName = "ProductCode")]
    public string ProductCode { get; set; }

    [XmlElement(ElementName = "UserID")]
    public string UserID { get; set; }

    [XmlElement(ElementName = "NCBEligibility")]
    public NCBEligibility NCBEligibility { get; set; }

    [XmlElement(ElementName = "LstCoveragePremium")]
    public string LstCoveragePremium { get; set; }

    [XmlElement(ElementName = "SourceSystemID")]
    public string SourceSystemID { get; set; }

    [XmlElement(ElementName = "AuthToken")]
    public string AuthToken { get; set; }

    [XmlAttribute(AttributeName = "xsi")]
    public string Xsi { get; set; }

    [XmlAttribute(AttributeName = "xsd")]
    public string Xsd { get; set; }

    [XmlText]
    public string Text { get; set; }
}

public class RelianceRequestDto
{
    public PolicyDetails PolicyDetails { get; set; }
}

