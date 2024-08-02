using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Insurance.Domain.Reliance
{
    [XmlRoot(ElementName = "GenerateScheduleRequest")]
    public class ReliancePolicyDocumentRequestResponse
    {
        [XmlElement(ElementName = "PolicyNumber")]
        public string PolicyNumber { get; set; }

        [XmlElement(ElementName = "SourceSystemID")]
        public string SourceSystemID { get; set; }

        [XmlElement(ElementName = "SecureAuthToken")]
        public string SecureAuthToken { get; set; }

        [XmlElement(ElementName = "EndorsementNo")]
        public string EndorsementNo { get; set; }
    }

    public class ReliancePolicyDocumentResponse
    {
        public GenerateScheduleResponse GenerateScheduleResponse { get; set; }
    }

    public class GenerateScheduleResponse
    {
        [JsonProperty("@xmlns:xsd")]
        public string xmlnsxsd { get; set; }

        [JsonProperty("@xmlns:xsi")]
        public string xmlnsxsi { get; set; }
        public string PolicyNumber { get; set; }
        public string SourceSystemID { get; set; }
        public string SecureAuthToken { get; set; }
        public object EndorsementNo { get; set; }
        public string DownloadLink { get; set; }
        public string ContainsValidationErrors { get; set; }
        public string ValidationErrors { get; set; }
    }
}
