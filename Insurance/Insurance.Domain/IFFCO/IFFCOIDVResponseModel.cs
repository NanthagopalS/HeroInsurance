using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Insurance.Domain.IFFCO
{
    public class IFFCOIDVResponseModel
    {
        public int Idv { get; set; }
        public int maximumIdvAllowed { get; set; }
        public int minimumIdvAllowed { get; set; }
        public int StatusCode { get; set; }
        public string erorMessage { get; set; }
    }
    public class GetVehicleIdvResponse
    {
        [XmlElement(ElementName = "getVehicleIdvReturn", Namespace = "http://premiumwrapper.motor.itgi.com")]
        public GetVehicleIdvReturn GetVehicleIdvReturn { get; set; }
    }

    public class GetVehicleIdvReturn
    {
        public string erorMessage { get; set; }
        [XmlElement(ElementName = "idv")]
        public string Idv { get; set; }
        public string minimumIdvAllowed { get; set; }
        public string maximumIdvAllowed { get; set; }

    }
}
