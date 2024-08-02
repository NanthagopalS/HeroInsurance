using System.Xml.Serialization;

namespace Insurance.Domain.Oriental;

[XmlRoot(ElementName = "GetQuoteMotorResult")]
public class GetQuoteMotorResult
{

    [XmlElement(ElementName = "TP_PREMIUM")]
    public string TPPREMIUM { get; set; }

    [XmlElement(ElementName = "OD_PREMIUM")]
    public string ODPREMIUM { get; set; }

    [XmlElement(ElementName = "ANNUAL_PREMIUM")]
    public string ANNUALPREMIUM { get; set; }

    [XmlElement(ElementName = "NCB_PERCENTAGE_OUT")]
    public string NCBPERCENTAGEOUT { get; set; }

    [XmlElement(ElementName = "NCB_AMOUNT")]
    public string NCBAMOUNT { get; set; }

    [XmlElement(ElementName = "SERVICE_TAX")]
    public string SERVICETAX { get; set; }

    [XmlElement(ElementName = "PROPOSAL_NO_OUT")]
    public string PROPOSALNOOUT { get; set; }

    [XmlElement(ElementName = "POLICY_SYS_ID")]
    public string POLICYSYSID { get; set; }

    [XmlElement(ElementName = "FLEX_01_OUT")]
    public string FLEX01OUT { get; set; }

    [XmlElement(ElementName = "FLEX_02_OUT")]
    public string FLEX02OUT { get; set; }

    [XmlElement(ElementName = "FLEX_03_OUT")]
    public string FLEX03OUT { get; set; }

    [XmlElement(ElementName = "FLEX_04_OUT")]
    public string FLEX04OUT { get; set; }

    [XmlElement(ElementName = "FLEX_05_OUT")]
    public string FLEX05OUT { get; set; }

    [XmlElement(ElementName = "ERROR_CODE")]
    public string ERRORCODE { get; set; }
}

[XmlRoot(ElementName = "GetQuoteMotorResponse")]
public class GetQuoteMotorResponse
{

    [XmlElement(ElementName = "GetQuoteMotorResult")]
    public GetQuoteMotorResult GetQuoteMotorResult { get; set; }

    [XmlAttribute(AttributeName = "xmlns")]
    public string Xmlns { get; set; }

    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "Body")]
public class QuoteResponseBody
{

    [XmlElement(ElementName = "GetQuoteMotorResponse", Namespace = "http://MotorService/")]
    public GetQuoteMotorResponse GetQuoteMotorResponse { get; set; }
}

[XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class QuoteResponseEnvelope
{

    [XmlElement(ElementName = "Body")]
    public QuoteResponseBody Body { get; set; }

    [XmlAttribute(AttributeName = "soap")]
    public string Soap { get; set; }

    [XmlAttribute(AttributeName = "xsi")]
    public string Xsi { get; set; }

    [XmlAttribute(AttributeName = "xsd")]
    public string Xsd { get; set; }

    [XmlText]
    public string Text { get; set; }
}

