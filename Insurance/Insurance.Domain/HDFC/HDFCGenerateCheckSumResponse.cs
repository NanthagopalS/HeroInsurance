using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Insurance.Domain.HDFC
{
    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (Envelope)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "GenerateRequestChecksumResponse")]
    public class GenerateRequestChecksumResponse
    {

        [XmlElement(ElementName = "GenerateRequestChecksumResult")]
        public string GenerateRequestChecksumResult { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Body")]
    public class Body
    {

        [XmlElement(ElementName = "GenerateRequestChecksumResponse",Namespace = "http://hdfcergo.com/")]
        public GenerateRequestChecksumResponse GenerateRequestChecksumResponse { get; set; }
    }

    [XmlRoot(ElementName = "Envelope",Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class HDFCGenerateCheckSumResponse
    {

        [XmlElement(ElementName = "Body")]
        public Body Body { get; set; }

        [XmlAttribute(AttributeName = "soap")]
        public string Soap { get; set; }

        [XmlAttribute(AttributeName = "xsi")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "xsd")]
        public string Xsd { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

}
