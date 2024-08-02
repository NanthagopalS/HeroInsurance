using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Insurance.Domain.IFFCO
{
    [XmlRoot(ElementName = "getMotorPremium", Namespace = "http://premiumwrapper.motor.itgi.com")]
    public class GetCVMotorPremium
    {
        [XmlElement(ElementName = "policy")]
        public IFFCOCVPolicy IFFCOCVPolicy { get; set; }
        [XmlElement(ElementName = "partner")]
        public CVPartner CVPartner { get; set; }
    }
    public class IFFCOCVPolicy
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
        public IFFCOCVVehicle IFFCOCVVehicle { get; set; }
    }
    public class IFFCOCVVehicle
    {
        [XmlElement(ElementName = "capacity")]
        public string Capacity { get; set; }
        [XmlElement(ElementName = "engineCpacity")]
        public string EngineCpacity { get; set; }
        [XmlElement(ElementName = "grossVehicleWt")]
        public string GrossVehicleWt { get; set; }
        [XmlElement(ElementName = "make")]
        public string Make { get; set; }
        [XmlElement(ElementName = "registrationDate")]
        public string RegistrationDate { get; set; }
        [XmlElement(ElementName = "seatingCapacity")]
        public string SeatingCapacity { get; set; }
        [XmlElement(ElementName = "regictrationCity")]
        public string RegictrationCity { get; set; }
        [XmlElement(ElementName = "yearOfManufacture")]
        public string YearOfManufacture { get; set; }
        [XmlElement(ElementName = "zcover")]
        public string Zcover { get; set; }
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "itgiRiskOccupationCode")]
        public string ItgiRiskOccupationCode { get; set; }
        [XmlElement(ElementName = "grossVehicleWeight")]
        public string GrossVehicleWt2 { get; set; } // double in request
        [XmlElement(ElementName = "validDrivingLicence")]
        public string ValidDrivingLicence { get; set; }
        [XmlElement(ElementName = "nofOfCarTrailers")]
        public int NofOfCarTrailers { get; set; }
        [XmlElement(ElementName = "noOfLuggageTrailers")]
        public int NoOfLuggageTrailers { get; set; }
        [XmlElement(ElementName = "luggageAverageIDV")]
        public int LuggageAverageIDV { get; set; }
        [XmlElement(ElementName = "vehicleCoverage")]
        public VehicleCVCoverage VehicleCVCoverage { get; set; }

        //[XmlElement(ElementName = "itgiZone")]
        //public string ItgiZone { get; set; }
        //[XmlElement(ElementName = "newVehicleFlag")]
        //public string NewVehicleFlag { get; set; }
        //[XmlElement(ElementName = "vehicleBody")]
        //public string VehicleBody { get; set; }
        //[XmlElement(ElementName = "vehicleClass")]
        //public string VehicleClass { get; set; }
        //[XmlElement(ElementName = "vehicleCoverage")]
        //public VehicleCVCoverage VehicleCVCoverage { get; set; }
        //[XmlElement(ElementName = "vehicleSubclass")]
        //public string VehicleSubclass { get; set; }
    }
    public class VehicleCVCoverage
    {
        [XmlElement(ElementName = "item")]
        public List<CVItem> CVitem { get; set; }
    }
    public class CVItem
    {
        [XmlElement(ElementName = "coverageId")]
        public string CoverageId { get; set; }
        [XmlElement(ElementName = "number")]
        public string Number { get; set; }
        [XmlElement(ElementName = "sumInsured")]
        public string SumInsured { get; set; }
    }
    public class CVPartner
    {
        [XmlElement(ElementName = "partnerBranch")]
        public string PartnerBranch { get; set; }
        [XmlElement(ElementName = "partnerCode")]
        public string PartnerCode { get; set; }
        [XmlElement(ElementName = "partnerSubBranch")]
        public string PartnerSubBranch { get; set; }
    }
}
