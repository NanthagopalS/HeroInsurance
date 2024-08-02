using System.Xml.Serialization;

namespace Insurance.Domain.IFFCO
{
    [XmlRoot(ElementName = "idvWebServiceRequest")]
    public class IdvWebServiceRequestCV
    {
        [XmlElement(ElementName = "contractType")]
        public string ContractType { get; set; }

        [XmlElement(ElementName = "dateOfRegistration")]
        public string DateOfRegistration { get; set; }

        [XmlElement(ElementName = "inceptionDate")]
        public string InceptionDate { get; set; }

        [XmlElement(ElementName = "makeCode")]
        public string MakeCode { get; set; }

        [XmlElement(ElementName = "model")]
        public string Model { get; set; }

        [XmlElement(ElementName = "rtoCity")]
        public string RtoCity { get; set; }
        [XmlElement(ElementName = "vehicleClass")]
        public string VehicleClass { get; set; }
        [XmlElement(ElementName = "vehicleSubClass")]
        public string VehicleSubClass { get; set; }
        [XmlElement(ElementName = "yearOfMake")]
        public string YearOfMake { get; set; }
    }
    [XmlRoot(ElementName = "getVehicleCVIIdv")]
    public class GetVehicleCVIIdv
    {

        [XmlElement(ElementName = "idvWebServiceRequest")]
        public IdvWebServiceRequestCV IdvWebServiceRequestCV { get; set; }
    }
}
