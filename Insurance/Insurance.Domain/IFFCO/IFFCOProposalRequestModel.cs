using System.Xml.Serialization;

namespace Insurance.Domain.IFFCO;


[XmlRoot(ElementName = "Policy")]
public class IFFCOProposalPolicy
{

    [XmlElement(ElementName = "Product")]
    public string Product { get; set; }

    [XmlElement(ElementName = "CreatedDate")]
    public string CreatedDate { get; set; }

    [XmlElement(ElementName = "InceptionDate")]
    public string InceptionDate { get; set; }

    [XmlElement(ElementName = "UniqueQuoteId")]
    public string UniqueQuoteId { get; set; }

    [XmlElement(ElementName = "CorporateClient")]
    public string CorporateClient { get; set; }

    [XmlElement(ElementName = "ExpiryDate")]
    public string ExpiryDate { get; set; }

    [XmlElement(ElementName = "PreviousPolicyEnddate")]
    public string PreviousPolicyEnddate { get; set; }

    [XmlElement(ElementName = "PreviousPolicyStartdate")]
    public string PreviousPolicyStartdate { get; set; }

    [XmlElement(ElementName = "TpInceptionDate")]
    public string TpInceptionDate { get; set; }

    [XmlElement(ElementName = "TpExpiryDate")]
    public string TpExpiryDate { get; set; }

    [XmlElement(ElementName = "TpInsurerName")]
    public string TpInsurerName { get; set; }

    [XmlElement(ElementName = "TpPolicyNo")]
    public string TpPolicyNo { get; set; }

    [XmlElement(ElementName = "PreviousPolicyInsurer")]
    public string PreviousPolicyInsurer { get; set; }

    [XmlElement(ElementName = "PreviousPolicyNo")]
    public string PreviousPolicyNo { get; set; }

    [XmlElement(ElementName = "GeneralPage")]
    public string GeneralPage { get; set; }

    [XmlElement(ElementName = "OdDiscountLoading")]
    public string OdDiscountLoading { get; set; }

    [XmlElement(ElementName = "OdDiscountAmt")]
    public string OdDiscountAmt { get; set; }

    [XmlElement(ElementName = "OdSumDisLoad")]
    public string OdSumDisLoad { get; set; }

    [XmlElement(ElementName = "TpSumDisLoad")]
    public string TpSumDisLoad { get; set; }

    [XmlElement(ElementName = "GrossPremium")]
    public string GrossPremium { get; set; }

    [XmlElement(ElementName = "ServiceTax")]
    public string IffcoServiceTax { get; set; }

    [XmlElement(ElementName = "NetPremiumPayable")]
    public string NetPremiumPayable { get; set; }

    [XmlElement(ElementName = "TotalSumInsured")]
    public string TotalSumInsured { get; set; }

    [XmlElement(ElementName = "ExternalBranch")]
    public string ExternalBranch { get; set; }

    [XmlElement(ElementName = "ExternalSubBranch")]
    public string ExternalSubBranch { get; set; }

    [XmlElement(ElementName = "ExternalServiceConsumer")]
    public string ExternalServiceConsumer { get; set; }

    [XmlElement(ElementName = "Nominee")]
    public string IffcoNominee { get; set; }

    [XmlElement(ElementName = "NomineeRelationship")]
    public string NomineeRelationship { get; set; }

    [XmlElement(ElementName = "PartnerType")]
    public string PartnerType { get; set; }

    [XmlElement(ElementName = "POSPanNumber")]
    public string POSPanNumber { get; set; }
    [XmlElement(ElementName = "BreakInofMorethan90days")]
    public string BreakInofMorethan90days { get; set; }
}

[XmlRoot(ElementName = "Coverage")]
public class IFFCOProposalCoverage
{

    [XmlElement(ElementName = "Code")]
    public string Code { get; set; }

    [XmlElement(ElementName = "Number")]
    public string Number { get; set; }

    [XmlElement(ElementName = "SumInsured")]
    public string SumInsured { get; set; }

    [XmlElement(ElementName = "ODPremium")]
    public string ODPremium { get; set; }

    [XmlElement(ElementName = "TPPremium")]
    public string TPPremium { get; set; }
}

[XmlRoot(ElementName = "Vehicle")]
public class IFFCOProposalVehicle
{

    [XmlElement(ElementName = "Capacity")]
    public string Capacity { get; set; }

    [XmlElement(ElementName = "EngineCapacity")]
    public string EngineCapacity { get; set; }

    [XmlElement(ElementName = "GrossVehicleWeight")]
    public string GrossVehicleWeight { get; set; }

    [XmlElement(ElementName = "Make")]
    public string Make { get; set; }

    [XmlElement(ElementName = "RegistrationNumber1")]
    public string RegistrationNumber1 { get; set; }

    [XmlElement(ElementName = "RegistrationNumber2")]
    public string RegistrationNumber2 { get; set; }

    [XmlElement(ElementName = "RegistrationNumber3")]
    public string RegistrationNumber3 { get; set; }

    [XmlElement(ElementName = "RegistrationNumber4")]
    public string RegistrationNumber4 { get; set; }

    [XmlElement(ElementName = "PolicyType")]
    public string PolicyType { get; set; }

    [XmlElement(ElementName = "ManufacturingYear")]
    public string ManufacturingYear { get; set; }

    [XmlElement(ElementName = "Zone")]
    public string Zone { get; set; }

    [XmlElement(ElementName = "RiskOccupationCode")]
    public string RiskOccupationCode { get; set; }

    [XmlElement(ElementName = "VehicleBody")]
    public string VehicleBody { get; set; }

    [XmlElement(ElementName = "EngineNumber")]
    public string EngineNumber { get; set; }

    [XmlElement(ElementName = "ChassisNumber")]
    public string ChassisNumber { get; set; }

    [XmlElement(ElementName = "SeatingCapacity")]
    public string SeatingCapacity { get; set; }

    [XmlElement(ElementName = "RegistrationDate")]
    public string RegistrationDate { get; set; }

    [XmlElement(ElementName = "RTOCity")]
    public string RTOCity { get; set; }

    [XmlElement(ElementName = "NewVehicleFlag")]
    public string NewVehicleFlag { get; set; }

    [XmlElement(ElementName = "ImposedExcessPartialLoss")]
    public string ImposedExcessPartialLoss { get; set; }

    [XmlElement(ElementName = "ImposedExcessTotalLoss")]
    public string ImposedExcessTotalLoss { get; set; }

    [XmlElement(ElementName = "Zcover")]
    public string Zcover { get; set; }
    [XmlElement(ElementName = "ValidDrivingLicence")]
    public string ValidDrivingLicence { get; set; }
    [XmlElement(ElementName = "AlternatePACover")]
    public string AlternatePACover { get; set; }
}

[XmlRoot(ElementName = "Contact")]
public class IFFCOProposalContact
{

    [XmlElement(ElementName = "DOB")]
    public string DOB { get; set; }

    [XmlElement(ElementName = "PassPort")]
    public string PassPort { get; set; }

    [XmlElement(ElementName = "PAN")]
    public string PAN { get; set; }

    [XmlElement(ElementName = "SiebelContactNumber")]
    public string SiebelContactNumber { get; set; }

    [XmlElement(ElementName = "ExternalClientNo")]
    public string ExternalClientNo { get; set; }

    [XmlElement(ElementName = "ItgiClientNumber")]
    public string ItgiClientNumber { get; set; }

    [XmlElement(ElementName = "Salutation")]
    public string Salutation { get; set; }

    [XmlElement(ElementName = "FirstName")]
    public string FirstName { get; set; }

    [XmlElement(ElementName = "LastName")]
    public string LastName { get; set; }

    [XmlElement(ElementName = "Sex")]
    public string Sex { get; set; }

    [XmlElement(ElementName = "AddressType")]
    public string AddressType { get; set; }

    [XmlElement(ElementName = "PinCode")]
    public string PinCode { get; set; }

    [XmlElement(ElementName = "State")]
    public string State { get; set; }

    [XmlElement(ElementName = "AddressLine1")]
    public string AddressLine1 { get; set; }

    [XmlElement(ElementName = "AddressLine2")]
    public string AddressLine2 { get; set; }

    [XmlElement(ElementName = "FaxNo")]
    public string FaxNo { get; set; }

    [XmlElement(ElementName = "Country")]
    public string Country { get; set; }

    [XmlElement(ElementName = "CountryOrigin")]
    public string CountryOrigin { get; set; }

    [XmlElement(ElementName = "Occupation")]
    public string Occupation { get; set; }

    [XmlElement(ElementName = "City")]
    public string City { get; set; }

    [XmlElement(ElementName = "Source")]
    public string Source { get; set; }

    [XmlElement(ElementName = "Nationality")]
    public string Nationality { get; set; }

    [XmlElement(ElementName = "Married")]
    public string Married { get; set; }

    [XmlElement(ElementName = "HomePhone")]
    public string HomePhone { get; set; }

    [XmlElement(ElementName = "OfficePhone")]
    public string OfficePhone { get; set; }

    [XmlElement(ElementName = "MobilePhone")]
    public string MobilePhone { get; set; }

    [XmlElement(ElementName = "Pager")]
    public string Pager { get; set; }

    [XmlElement(ElementName = "MailId")]
    public string MailId { get; set; }

    [XmlElement(ElementName = "TaxId")]
    public string TaxId { get; set; }

    [XmlElement(ElementName = "StafFlag")]
    public string StafFlag { get; set; }

    [XmlElement(ElementName = "AddressLine3")]
    public string AddressLine3 { get; set; }

    [XmlElement(ElementName = "AddressLine4")]
    public string AddressLine4 { get; set; }

    [XmlElement(ElementName = "ItgiKYCReferenceNo")]
    public string ItgiKYCReferenceNo { get; set; }
}

[XmlRoot(ElementName = "Account")]
public class IFFCOProposalAccount
{

    [XmlElement(ElementName = "DOB")]
    public string DOB { get; set; }

    [XmlElement(ElementName = "PAN")]
    public string PAN { get; set; }

    [XmlElement(ElementName = "AccountNumber")]
    public string AccountNumber { get; set; }

    [XmlElement(ElementName = "ExternalAccountId")]
    public string ExternalAccountId { get; set; }

    [XmlElement(ElementName = "ClientNumber")]
    public string ClientNumber { get; set; }

    [XmlElement(ElementName = "Name")]
    public string Name { get; set; }

    [XmlElement(ElementName = "TaxId")]
    public string TaxId { get; set; }

    [XmlElement(ElementName = "PrimaryAccountStreetAddress")]
    public string PrimaryAccountStreetAddress { get; set; }

    [XmlElement(ElementName = "PrimaryAccountStreetAddress2")]
    public string PrimaryAccountStreetAddress2 { get; set; }

    [XmlElement(ElementName = "MainPhoneNumber")]
    public string MainPhoneNumber { get; set; }

    [XmlElement(ElementName = "MainFaxNumber")]
    public string MainFaxNumber { get; set; }

    [XmlElement(ElementName = "MailId")]
    public string MailId { get; set; }

    [XmlElement(ElementName = "EconomicActivity")]
    public string EconomicActivity { get; set; }

    [XmlElement(ElementName = "PaidCapital")]
    public string PaidCapital { get; set; }

    [XmlElement(ElementName = "PrimaryAccountPostalCode")]
    public string PrimaryAccountPostalCode { get; set; }

    [XmlElement(ElementName = "PrimaryAccountState")]
    public string PrimaryAccountState { get; set; }

    [XmlElement(ElementName = "PrimaryAccountCity")]
    public string PrimaryAccountCity { get; set; }

    [XmlElement(ElementName = "PrimaryAccountCountry")]
    public string PrimaryAccountCountry { get; set; }

    [XmlElement(ElementName = "Source")]
    public string Source { get; set; }

    [XmlElement(ElementName = "Licence")]
    public string Licence { get; set; }

    [XmlElement(ElementName = "PrimaryAccountStreetAddress3")]
    public string PrimaryAccountStreetAddress3 { get; set; }

    [XmlElement(ElementName = "PrimaryAccountStreetAddress4")]
    public string PrimaryAccountStreetAddress4 { get; set; }

    [XmlElement(ElementName = "AccountGSTIN")]
    public string AccountGSTIN { get; set; }
}

[XmlRoot(ElementName = "VehicleThirdParty")]
public class IFFCOProposalVehicleThirdParty
{

    [XmlElement(ElementName = "InterestedParty")]
    public string InterestedParty { get; set; }

    [XmlElement(ElementName = "InterestedPartyName")]
    public string InterestedPartyName { get; set; }

    [XmlElement(ElementName = "Relation")]
    public string Relation { get; set; }
}

[XmlRoot(ElementName = "Request")]
public class IFFCOProposalRequest
{

    [XmlElement(ElementName = "Policy")]
    public IFFCOProposalPolicy Policy { get; set; }

    [XmlElement(ElementName = "Coverage")]
    public List<IFFCOProposalCoverage> Coverage { get; set; }

    [XmlElement(ElementName = "Vehicle")]
    public IFFCOProposalVehicle Vehicle { get; set; }

    [XmlElement(ElementName = "Contact")]
    public IFFCOProposalContact Contact { get; set; }

    [XmlElement(ElementName = "Account")]
    public IFFCOProposalAccount Account { get; set; }

    [XmlElement(ElementName = "VehicleThirdParty")]
    public IFFCOProposalVehicleThirdParty VehicleThirdParty { get; set; }
}


