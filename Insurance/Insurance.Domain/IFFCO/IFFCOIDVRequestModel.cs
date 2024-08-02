using System.Xml.Serialization;

namespace Insurance.Domain.IFFCO
{
    [XmlRoot(ElementName = "idvWebServiceRequest")]
    public class IdvWebServiceRequest
    {

        [XmlElement(ElementName = "dateOfRegistration")]
        public string DateOfRegistration { get; set; }

        [XmlElement(ElementName = "inceptionDate")]
        public string InceptionDate { get; set; }

        [XmlElement(ElementName = "makeCode")]
        public string MakeCode { get; set; }

        [XmlElement(ElementName = "rtoCity")]
        public string RtoCity { get; set; }
    }
    [XmlRoot(ElementName = "getVehicleIdv")]
    public class GetVehicleIdv
    {

        [XmlElement(ElementName = "idvWebServiceRequest")]
        public IdvWebServiceRequest IdvWebServiceRequest { get; set; }
    }
}
