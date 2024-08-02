using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Insurance.Domain.UnitedIndia
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    //[System.SerializableAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    //public partial class UnitedIndiaPaymentInfoResponseEnvelope
    //{
    //    /// <remarks/>
    //    public UnitedIndiaPaymentInfoResponseEnvelopeBody Body { get; set; }
    //}

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class UnitedIndiaPaymentInfoResponseEnvelope
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public UnitedIndiaPaymentInfoResponseEnvelopeBody Body { get; set; }
        [XmlAttribute(AttributeName = "S", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string S { get; set; }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class UnitedIndiaPaymentInfoResponseEnvelopeBody
    {
        [XmlElement(ElementName = "paymentInfoResponse", Namespace = "http://ws.uiic.com/")]
        public UnitedIndiapaymentInfoResponse paymentInfoResponse { get; set; }
    }
    ///// <remarks/>
    //[System.SerializableAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    //public partial class UnitedIndiaPaymentInfoResponseEnvelopeBody
    //{
    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://ws.uiic.com/")]
    //    public UnitedIndiapaymentInfoResponse paymentInfoResponse { get; set; }
    //}

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.uiic.com/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ws.uiic.com/", IsNullable = false)]
    public partial class UnitedIndiapaymentInfoResponse
    {
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
        public UnitedIndiaPaymentInfoResponsereturn @return { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class UnitedIndiaPaymentInfoResponsereturn
    {
        public UnitedIndiaPaymentInfoResponsereturnROOT ROOT { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class UnitedIndiaPaymentInfoResponsereturnROOT
    {
        public UnitedIndiaPaymentInfoResponsereturnROOTHEADER HEADER { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class UnitedIndiaPaymentInfoResponsereturnROOTHEADER
    {
        public string TXT_ERR_MSG { get; set; }

        /// <remarks/>
        public string NUM_REFERENCE_NUMBER { get; set; }

        /// <remarks/>
        public string DAT_REFERENCE_DATE { get; set; }

        /// <remarks/>
        public string TXT_NAME_OF_INSURED { get; set; }

        /// <remarks/>
        public string TXT_CUSTOMER_ID { get; set; }

        /// <remarks/>
        public string NUM_PAYMENT_AMOUNT { get; set; }

        /// <remarks/>
        public string TXT_NEW_POLICY_NUMBER { get; set; }
        /// <remarks/>
        public string DAT_DATE_ISSUE_OF_NEW_POLICY { get; set; }

        /// <remarks/>
        public string TXT_BANK_CODE { get; set; }

        /// <remarks/>
        public string TXT_BANK_NAME { get; set; }

        /// <remarks/>
        public string TXT_BANK_BRANCH_NAME { get; set; }

        /// <remarks/>
        public string DAT_RECEIPT_DATE { get; set; }

        /// <remarks/>
        public string NUM_COLLECTION_NO { get; set; }

        /// <remarks/>
        public string TXT_BUSINESS_LOCATION { get; set; }

        /// <remarks/>
        public string TXT_TRANSACTION_ID { get; set; }

        /// <remarks/>
        public string TXT_PAYMENT_NUMBER { get; set; }

        /// <remarks/>
        public string NUM_PAYMENT_ID { get; set; }

        /// <remarks/>
        public string TXT_INVOICE_NO { get; set; }

        /// <remarks/>
        public string TXT_INVOICE_DATE { get; set; }

        /// <remarks/>
        public string TXT_SAC_CODE { get; set; }

        /// <remarks/>
        public string SCHEDULE { get; set; }
    }


}
