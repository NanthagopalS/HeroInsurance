using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Insurance.Domain.UnitedIndia
{
    [XmlRoot(ElementName = "HEADER")]
    public class UIICPaymentInfoHEADER
    {
        [XmlElement(ElementName = "DAT_UTR_DATE")]
        public string DAT_UTR_DATE { get; set; }
        [XmlElement(ElementName = "NUM_PREMIUM_AMOUNT")]
        public string NUM_PREMIUM_AMOUNT { get; set; }
        [XmlElement(ElementName = "NUM_REFERENCE_NUMBER")]
        public string NUM_REFERENCE_NUMBER { get; set; }
        [XmlElement(ElementName = "NUM_UTR_PAYMENT_AMOUNT")]
        public string NUM_UTR_PAYMENT_AMOUNT { get; set; }
        [XmlElement(ElementName = "TXT_BANK_CODE")]
        public string TXT_BANK_CODE { get; set; }
        [XmlElement(ElementName = "TXT_BANK_NAME")]
        public string TXT_BANK_NAME { get; set; }
        [XmlElement(ElementName = "TXT_MERCHANT_ID")]
        public string TXT_MERCHANT_ID { get; set; }
        [XmlElement(ElementName = "TXT_TRANSACTION_ID")]
        public string TXT_TRANSACTION_ID { get; set; }
        [XmlElement(ElementName = "TXT_UTR_NUMBER")]
        public string TXT_UTR_NUMBER { get; set; }
    }

    [XmlRoot(ElementName = "ROOT")]
    public class UIICPaymentInfoROOT
    {
        [XmlElement(ElementName = "HEADER")]
        public UIICPaymentInfoHEADER HEADER { get; set; }
    }

    //[XmlRoot(ElementName = "paymentXml")]
    //public class UIICPaymentXml
    //{
    //    [XmlElement(ElementName = "ROOT")]
    //    public UIICPaymentInfoROOT ROOT { get; set; }
    //}

    [XmlRoot(ElementName = "paymentInfo")]
    public class UIICPaymentInfo
    {
        [XmlElement(ElementName = "application")]
        public string application { get; set; }
        [XmlElement(ElementName = "userid")]
        public string userid { get; set; }
        [XmlElement(ElementName = "password")]
        public string password { get; set; }
        [XmlElement(ElementName = "paymentXml")]
        public string paymentXml { get; set; }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class UIICPaymentInfoRequestBody
    {
        [XmlElement(ElementName = "paymentInfo")]
        public UIICPaymentInfo PaymentInfo { get; set; }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class UIICPaymentInfoEnvelope
    {
        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string Header { get; set; }
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public UIICPaymentInfoRequestBody Body { get; set; }
        //[XmlAttribute(AttributeName = "soapenv", Namespace = "http://www.w3.org/2000/xmlns/")]
        //public string Soapenv { get; set; }
        //[XmlAttribute(AttributeName = "ws", Namespace = "http://www.w3.org/2000/xmlns/")]
        //public string Ws { get; set; }
    }
}
