using Insurance.Domain.IFFCO;
using Insurance.Domain.Reliance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Insurance.Domain.IFFCO
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class IFFCOEnvelope
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public IFFCOBody Body { get; set; }
        public IFFCOCVBody CVBody { get; set; }
    }
    public class IFFCOBody
    {
        [XmlElement(ElementName = "getVehicleIdvResponse")]
        public GetVehicleIdvResponse GetVehicleIdvResponse { get; set; }
        [XmlElement(ElementName = "getVehicleIdv")]
        public GetVehicleIdv GetVehicleIdv { get; set; }
        [XmlElement(ElementName = "getNewVehiclePremium")]
        public GetNewVehiclePremium GetNewVehiclePremium { get; set; }

        [XmlElement(ElementName = "getNewVehiclePremiumResponse")]
        public GetNewVehiclePremiumResponse GetNewVehiclePremiumResponse { get; set; }
        [XmlElement(ElementName = "getMotorPremiumResponse")]
        public GetMotorPremiumResponse GetMotorPremiumResponse { get; set; }
        [XmlElement(ElementName = "getVehicleCVIIdv")]
        public GetVehicleCVIIdv GetVehicleCVIIdv { get; set; }
        [XmlElement(ElementName = "getVehicleCVIIdvResponse")]
        public GetVehicleCVIIdvResponse GetVehicleCVIIdvResponse { get; set; }

    }

    public class IFFCOCVBody
    {
        [XmlElement(ElementName = "getMotorPremium")]
        public GetCVMotorPremium GetCVMotorPremium { get; set; }
    }

}
