using System.Xml.Serialization;

namespace Insurance.Domain.Reliance;

[XmlRoot(ElementName = "ClientDetails")]
public class CoverageClientDetails
{

    [XmlElement(ElementName = "ClientType")]
    public int ClientType { get; set; }
}

[XmlRoot(ElementName = "PolicyTenure")]
public class CoveragePolicyTenure
{

    [XmlAttribute(AttributeName = "nil")]
    public bool Nil { get; set; }
}

[XmlRoot(ElementName = "Policy")]
public class CoveragePolicy
{

    [XmlElement(ElementName = "BusinessType")]
    public int BusinessType { get; set; }

    [XmlElement(ElementName = "Cover_From")]
    public string CoverFrom { get; set; }

    [XmlElement(ElementName = "Cover_To")]
    public string CoverTo { get; set; }

    [XmlElement(ElementName = "Branch_Code")]
    public string BranchCode { get; set; }

    [XmlElement(ElementName = "AgentName")]
    public string AgentName { get; set; }

    [XmlElement(ElementName = "productcode")]
    public string Productcode { get; set; }

    [XmlElement(ElementName = "OtherSystemName")]
    public int OtherSystemName { get; set; }

    [XmlElement(ElementName = "isMotorQuote")]
    public bool IsMotorQuote { get; set; }

    [XmlElement(ElementName = "isMotorQuoteFlow")]
    public bool IsMotorQuoteFlow { get; set; }

    [XmlElement(ElementName = "PolicyTenure")]
    public CoveragePolicyTenure PolicyTenure { get; set; }
}

[XmlRoot(ElementName = "Risk")]
public class CoverageRisk
{

    [XmlElement(ElementName = "VehicleMakeID")]
    public int VehicleMakeID { get; set; }

    [XmlElement(ElementName = "VehicleModelID")]
    public int VehicleModelID { get; set; }

    [XmlElement(ElementName = "CubicCapacity")]
    public int CubicCapacity { get; set; }

    [XmlElement(ElementName = "Zone")]
    public string Zone { get; set; }

    [XmlElement(ElementName = "RTOLocationID")]
    public int RTOLocationID { get; set; }

    [XmlElement(ElementName = "IDV")]
    public string IDV { get; set; }

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

    [XmlElement(ElementName = "Rto_RegionCode")]
    public string RtoRegionCode { get; set; }
}

[XmlRoot(ElementName = "Vehicle")]
public class CoverageVehicle
{

    [XmlElement(ElementName = "Registration_Number")]
    public string RegistrationNumber { get; set; }

    [XmlElement(ElementName = "Registration_date")]
    public string RegistrationDate { get; set; }

    [XmlElement(ElementName = "SeatingCapacity")]
    public int SeatingCapacity { get; set; }

    [XmlElement(ElementName = "TypeOfFuel")]
    public int TypeOfFuel { get; set; }
    [XmlElement(ElementName = "PCVVehicleCategory")]
    public int PCVVehicleCategory { get; set; }

    [XmlElement(ElementName = "PCVVehicleUsageType")]
    public int PCVVehicleUsageType { get; set; }

    [XmlElement(ElementName = "PCVVehicleSubUsageType")]
    public int PCVVehicleSubUsageType { get; set; }
    [XmlElement(ElementName = "BodyPrice")]
    public long BodyPrice { get; set; }

    [XmlElement(ElementName = "ChassisPrice")]
    public long ChassisPrice { get; set; }
    [XmlElement(ElementName = "ISmanufacturerfullybuild")]
    public bool ISmanufacturerfullybuild { get; set; } = true;
}

[XmlRoot(ElementName = "BifuelKit")]
public class CoverageBifuelKit
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

    [XmlElement(ElementName = "SumInsured")]
    public int SumInsured { get; set; }

    [XmlElement(ElementName = "BifuelKit")]
    public CoverageBifuelKit BifuelKits { get; set; }
}

[XmlRoot(ElementName = "Cover")]
public class CoverageCover
{

    [XmlElement(ElementName = "IsBasicODCoverage")]
    public bool IsBasicODCoverage { get; set; }

    [XmlElement(ElementName = "IsBasicLiability")]
    public bool IsBasicLiability { get; set; }

    [XmlElement(ElementName = "IsBiFuelKit")]
    public bool IsBiFuelKit { get; set; }

    [XmlElement(ElementName = "BifuelKit")]
    public CoverageBifuelKit BifuelKit { get; set; }
}

[XmlRoot(ElementName = "PreviousInsuranceDetails")]
public class CoveragePreviousInsuranceDetails
{

    [XmlElement(ElementName = "PrevYearInsurer")]
    public string PrevYearInsurer { get; set; }

    [XmlElement(ElementName = "PrevYearPolicyNo")]
    public string PrevYearPolicyNo { get; set; }

    [XmlElement(ElementName = "PrevYearInsurerAddress")]
    public string PrevYearInsurerAddress { get; set; }

    [XmlElement(ElementName = "PrevYearPolicyType")]
    public string PrevYearPolicyType { get; set; }

    [XmlElement(ElementName = "PrevYearPolicyStartDate")]
    public string PrevYearPolicyStartDate { get; set; }

    [XmlElement(ElementName = "PrevYearPolicyEndDate")]
    public string PrevYearPolicyEndDate { get; set; }
}

[XmlRoot(ElementName = "NCBEligibility")]
public class CoverageNCBEligibility
{

    [XmlElement(ElementName = "NCBEligibilityCriteria")]
    public int NCBEligibilityCriteria { get; set; }

    [XmlElement(ElementName = "PreviousNCB")]
    public int PreviousNCB { get; set; }

    [XmlElement(ElementName = "CurrentNCB")]
    public int CurrentNCB { get; set; }
}

[XmlRoot(ElementName = "PolicyDetails")]
public class CoveragePolicyDetails
{

    [XmlElement(ElementName = "CoverDetails")]
    public string CoverDetails { get; set; }

    [XmlElement(ElementName = "TrailerDetails")]
    public string TrailerDetails { get; set; }

    [XmlElement(ElementName = "ClientDetails")]
    public CoverageClientDetails ClientDetails { get; set; }

    [XmlElement(ElementName = "Policy")]
    public CoveragePolicy Policy { get; set; }

    [XmlElement(ElementName = "Risk")]
    public CoverageRisk Risk { get; set; }

    [XmlElement(ElementName = "Vehicle")]
    public CoverageVehicle Vehicle { get; set; }

    [XmlElement(ElementName = "Cover")]
    public CoverageCover Cover { get; set; }

    [XmlElement(ElementName = "PreviousInsuranceDetails")]
    public CoveragePreviousInsuranceDetails PreviousInsuranceDetails { get; set; }

    [XmlElement(ElementName = "productcode")]
    public string Productcode { get; set; }

    [XmlElement(ElementName = "NCBEligibility")]
    public CoverageNCBEligibility NCBEligibility { get; set; }

    [XmlElement(ElementName = "UserID")]
    public string UserID { get; set; }

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

