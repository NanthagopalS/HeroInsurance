using Insurance.Domain.ICICI;
using Insurance.Domain.Magma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Insurance.Domain.IFFCO
{
    [XmlRoot(ElementName = "getNewVehiclePremiumResponse")]
    public class GetNewVehiclePremiumResponse
    {
        [XmlElement(ElementName = "getNewVehiclePremiumReturn", Namespace = "http://premiumwrapper.motor.itgi.com", Type = typeof(GetNewVehiclePremiumReturn))]
        public List<GetNewVehiclePremiumReturn> GetNewVehiclePremiumReturn { get; set; }
    }

    public class GetNewVehiclePremiumReturn
    {
        [XmlElement(ElementName = "autocoverage")]
        public string autocoverage { get; set; }
        [XmlElement(ElementName = "discountLoading")]
        public string discountLoading { get; set; }
        [XmlElement(ElementName = "discountLoadingAmt")]
        public string discountLoadingAmt { get; set; }
        [XmlElement(ElementName = "gstAmount")]
        public string gstAmount { get; set; }
        [XmlElement(ElementName = "gstDetails")]
        public GstDetail gstDetail { get; set; }
        [XmlElement(ElementName = "inscoverageResponse")]
        public InscoverageResponse inscoverageResponse { get; set; }
        [XmlElement(ElementName = "premiumPayable")]
        public string premiumPayable { get; set; }
        [XmlElement(ElementName = "premiumpBreakUp")]
        public PremiumpBreakUp premiumpBreakUp { get; set; }
        [XmlElement(ElementName = "totalBreakInLoading")]
        public string totalBreakInLoading { get; set; }
        [XmlElement(ElementName = "totalODPremium")]
        public string totalODPremium { get; set; }
        [XmlElement(ElementName = "totalPremimAfterDiscLoad")]
        public string totalPremimAfterDiscLoad { get; set; }
        [XmlElement(ElementName = "totalPremiumBfDiscLoad")]
        public string totalPremiumBfDiscLoad { get; set; }
        [XmlElement(ElementName = "totalTPPremium")]
        public string totalTPPremium { get; set; }
        [XmlElement(ElementName = "totalodMktDiscOrLoading")]
        public string totalodMktDiscOrLoading { get; set; }
        [XmlElement(ElementName = "error")]
        public IFFCOError iFFCOError { get; set; }
    }
    public class IFFCOError
    {
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
    public class GstDetail
    {
        public string cess1Amount { get; set; }
        public string cess1Percent { get; set; }
        public string cess2Amount { get; set; }
        public string cess2Percent { get; set; }
        public string cess3Amount { get; set; }
        public string cess3Percent { get; set; }
        public string cess4Amount { get; set; }
        public string cess4Percent { get; set; }
        public string cess5Amount { get; set; }
        public string cess5Percent { get; set; }
        public string odCgstAmount { get; set; }
        public string odCgstPercent { get; set; }
        public string odIgstAmount { get; set; }
        public string odSgstAmount { get; set; }
        public string odSgstPercent { get; set; }
        public string odUgstAmount { get; set; }
        public string odUgstPercent { get; set; }
        public string totalTaxAmount { get; set; }
        public string tpCgstAmount { get; set; }
        public string tpCgstPercent { get; set; }
        public string tpIgstAmount { get; set; }
        public string tpIgstPercent { get; set; }
        public string tpSgstAmount { get; set; }
        public string tpSgstPercent { get; set; }
        public string tpUgstAmount { get; set; }
        public string tpUgstPercent { get; set; }
    }
    public class InscoverageResponse
    {
        public CoverageResponse coverageResponse { get; set; }
    }
    public class PremiumpBreakUp
    {
        public PremiumbreakUpDetail premiumbreakUpDetail { get; set; }
    }
    [XmlRoot(ElementName = "coverageResponse")]
    public class CoverageResponse
    {
        [XmlElement(ElementName = "coverageResponse", Type = typeof(IIFCOCoverageResponse))]
        public List<IIFCOCoverageResponse> coverageResponse { get; set; }
    }
    public class IIFCOCoverageResponse
    {
        public string LIMIT1 { get; set; }
        public string LIMIT2 { get; set; }
        public string LIMIT3 { get; set; }
        public string LIMIT4 { get; set; }
        public string LIMIT5 { get; set; }
        public string OD1 { get; set; }
        public string OD2 { get; set; }
        public string OD3 { get; set; }
        public string OD4 { get; set; }
        public string OD5 { get; set; }
        public string TP1 { get; set; }
        public string TP2 { get; set; }
        public string TP3 { get; set; }
        public string TP4 { get; set; }
        public string TP5 { get; set; }
        public string coverageCode { get; set; }
        public string coverageRowId { get; set; }
        public string coverageType { get; set; }
    }
    public class PremiumbreakUpDetail
    {
        [XmlElement(ElementName = "premiumbreakUpDetail", Type = typeof(IFFCOPremiumBreakUpDetail))]
        public List<IFFCOPremiumBreakUpDetail> IFFCOPremiumBreakUpDetail { get; set; }
    }
    public class IFFCOPremiumBreakUpDetail
    {
        public string cess1Amount { get; set; }
        public string cess1Percent { get; set; }
        public string cess2Amount { get; set; }
        public string cess2Percent { get; set; }
        public string cess3Amount { get; set; }
        public string cess3Percent { get; set; }
        public string cess4Amount { get; set; }
        public string cess4Percent { get; set; }
        public string cess5Amount { get; set; }
        public string cess5Percent { get; set; }
        public string odCgstAmount { get; set; }
        public string odCgstPercent { get; set; }
        public string odIgstAmount { get; set; }
        public string odSgstAmount { get; set; }
        public string odSgstPercent { get; set; }
        public string odUgstAmount { get; set; }
        public string odUgstPercent { get; set; }
        public string totalODPremium { get; set; }
        public string totalOdMktDiscOrLoading { get; set; }
        public string totalPremiumBfDiscLoading { get; set; }
        public string totalPremiumPayable { get; set; }
        public string totalTPPremium { get; set; }
        public string totalTaxAmount { get; set; }
        public string totalpremiumAftDiscLoad { get; set; }
        public string tpCgstAmount { get; set; }
        public string tpCgstPercent { get; set; }
        public string tpIgstAmount { get; set; }
        public string tpIgstPercent { get; set; }
        public string tpSgstAmount { get; set; }
        public string tpSgstPercent { get; set; }
        public string tpUgstAmount { get; set; }
        public string tpUgstPercent { get; set; }
        public string year { get; set; }
    }

    [XmlRoot(ElementName = "getMotorPremiumResponse")]
    public class GetMotorPremiumResponse
    {
        [XmlElement(ElementName = "getMotorPremiumReturn", Namespace = "http://premiumwrapper.motor.itgi.com", Type = typeof(GetMotorPremiumReturn))]
        public List<GetMotorPremiumReturn> GetMotorPremiumReturn { get; set; }
    }

    public class GetMotorPremiumReturn
    {

        public string autocoverage { get; set; }

        [XmlElement(ElementName = "coveragePremiumDetail")]
        public List<CoveragePremiumDetail> coveragePremiumDetail { get; set; }
        public string discountLoading { get; set; }
        public string discountLoadingAmt { get; set; }
        public string gst { get; set; }
        public string premiumPayable { get; set; }
        public string serviceTax { get; set; }
        public string totalODPremium { get; set; }
        public string totalPremimAfterDiscLoad { get; set; }
        public string totalPremium { get; set; }
        public string totalTPPremium { get; set; }
        [XmlElement(ElementName = "error")]
        public IFFCOError iFFCOError { get; set; }

    }
    public class CoveragePremiumDetail
    {
        public string coverageName { get; set; }
        public string coveragePremium { get; set; }
        public string odPremium { get; set; }
        public string tpPremium { get; set; }
    }
}
