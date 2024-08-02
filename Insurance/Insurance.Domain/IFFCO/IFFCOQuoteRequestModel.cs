using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Insurance.Domain.IFFCO
{
    [XmlRoot(ElementName = "getNewVehiclePremium")]
    public class GetNewVehiclePremium
    {
        [XmlElement(ElementName = "policyHeader")]
        public PolicyHeader PolicyHeader { get; set; }
        [XmlElement(ElementName = "policy")]
        public IFFCOPolicy IFFCOPolicy { get; set; }
        [XmlElement(ElementName = "partner")]
        public Partner Partner { get; set; }
    }
    public class PolicyHeader
    {
        [XmlElement(ElementName = "messageId")]
        public string MessageId { get; set; }
    }
    public class Partner
    {
        [XmlElement(ElementName = "partnerBranch")]
        public string PartnerBranch { get; set; }
        [XmlElement(ElementName = "partnerCode")]
        public string PartnerCode { get; set; }
        [XmlElement(ElementName = "partnerSubBranch")]
        public string PartnerSubBranch { get; set; }
    }
    public class IFFCOPolicy
    {
        [XmlElement(ElementName = "contractType")]
        public string ContractType { get; set; }
        [XmlElement(ElementName = "expiryDate")]
        public string ExpiryDate { get; set; }
        [XmlElement(ElementName = "inceptionDate")]
        public string InceptionDate { get; set; }
        [XmlElement(ElementName = "previousPolicyEndDate")]
        public string PreviousPolicyEndDate { get; set; }
        [XmlElement(ElementName = "vehicle")]
        public IFFCOVehicle IFFCOVehicle { get; set; }
    }
    public class IFFCOVehicle
    {
        [XmlElement(ElementName = "aaiExpiryDate")]
        public string AaiExpiryDate { get; set; }
        [XmlElement(ElementName = "aaiNo")]
        public string AaiNo { get; set; }
        [XmlElement(ElementName = "capacity")]
        public string Capacity { get; set; }
        [XmlElement(ElementName = "engineCpacity")]
        public string EngineCpacity { get; set; }
        [XmlElement(ElementName = "grossVehicleWeight")]
        public string GrossVehicleWt { get; set; }
        [XmlElement(ElementName = "itgiRiskOccupationCode")]
        public string ItgiRiskOccupationCode { get; set; }
        [XmlElement(ElementName = "itgiZone")]
        public string ItgiZone { get; set; }
        [XmlElement(ElementName = "make")]
        public string Make { get; set; }
        [XmlElement(ElementName = "regictrationCity")]
        public string RegictrationCity { get; set; }
        [XmlElement(ElementName = "registrationDate")]
        public string RegistrationDate { get; set; }
        [XmlElement(ElementName = "seatingCapacity")]
        public string SeatingCapacity { get; set; }
        [XmlElement(ElementName = "newVehicleFlag")]
        public string NewVehicleFlag { get; set; }
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "vehicleBody")]
        public string VehicleBody { get; set; }
        [XmlElement(ElementName = "vehicleClass")]
        public string VehicleClass { get; set; }
        [XmlElement(ElementName = "vehicleCoverage")]
        public VehicleCoverage VehicleCoverage { get; set; }
        [XmlElement(ElementName = "vehicleSubclass")]
        public string VehicleSubclass { get; set; }
        [XmlElement(ElementName = "yearOfManufacture")]
        public string YearOfManufacture { get; set; }
        [XmlElement(ElementName = "zcover")]
        public string Zcover { get; set; }
    }
    public class VehicleCoverage
    {
        [XmlElement(ElementName = "item")]
        public List<Item> item { get; set; }
    }
    public class Item
    {
        [XmlElement(ElementName = "coverageId")]
        public string CoverageId { get; set; }
        [XmlElement(ElementName = "number")]
        public string Number { get; set; }
        [XmlElement(ElementName = "sumInsured")]
        public string SumInsured { get; set; }
    }
}
