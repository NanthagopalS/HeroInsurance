using Insurance.Domain.Bajaj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.ICICI
{
    public class ICICIConfig
    {
        public string BaseURL { get; set; }
        public string IdvURL { get; set; }
        public string QuoteURL4W { get; set; }
        public string ProposalURL4W { get; set; }
        public string ProposalURL2W { get; set; }
        public string QuoteURL2W { get; set; }
        public string QuoteURL2WTP { get; set; }
        public string QuoteURL4WTP { get; set; }
        public string SubmitPOSPCertificateURL { get; set; }
        public string CreateIMBrokerURL { get; set; }
        public string POSPScope { get; set; }
        public string QuoteScope { get; set; }
        public string IdvScope { get; set; }
        public string CKYCScope { get; set; }
        public string PaymentScope { get; set; }
        public string PolicyGenerationScope { get; set; }
        public string CKYCURL { get; set; }
        public string POADocumentUpload { get; set; }
        public string TokenURL { get; set; }
        public string InsurerId { get; set; }
        public string InsurerName { get; set; }
        public string InsurerLogo { get; set; }
        public TokenData Token { get; set; }
        public string CreateBaseTransaction { get; set; }
        public string AuthCode { get; set; }
        public string ApplicationId { get; set; }
        public string ReturnURL { get; set; }
        public string PaymentURL { get; set; }
        public string TransactionEnquiryURL { get; set; }
        public string PaymentTaggingURL { get; set; }
        public string PolicyGenerationURL { get; set; }
        public string FourWheeler { get; set; }
        public string TwoWheeler { get; set; }
        public string Commerical { get; set; }
        public string LicenseNo { get; set; }
        public string IlLocation { get; set; }
        public string CertificateStatus { get; set; }
        public string CreateBreakinId { get; set; }
        public string GetBreakinStatus { get; set; }
        public string ClearInspectionStatus { get; set; }
        public string BreakinType { get; set; }
        public string DistributorInterId { get; set; }
        public string DistributorName { get; set; }
        public string InspectionType { get; set; }
        public string TwoWheelerProdectCode { get; set; }
        public string FourWheelerProdectCode { get; set; }
        public string TwoWheelerTPProdectCode { get; set; }
        public string FourWheelerTPProdectCode { get; set; }
        public string VehicleTypeTwoWheeler { get; set; }
        public string VehicleTypeFourWheeler { get; set; }
        public string VehicleTypeGoodsCarrier { get; set; }
        public string VehicleTypePassengerCarrier { get; set; }
        public string VehicleTypeMisscellaneous { get; set; }
        public string ProposalTPURL4W { get; set; }
        public string ProposalTPURL2W { get; set; }
        public string QuoteURLCV { get; set; }
        public string ProposalURLCV { get; set; }
        public string GCVProdectCode { get; set; }
        public string PCVProdectCode { get; set; }
        public string MISCProdectCode { get; set; }
        public string GCVDealId { get; set; }
        public string PCVDealId { get; set; }
        public string MISCDealId { get; set; }
        public string GCVDealIdTP { get; set; }
        public string PCVDealIdTP { get; set; }
        public string MISCDealIdTP { get; set; }
        public string GCVTPProdectCode { get; set; }
        public string PCVTPProdectCode { get; set; }
        public string MISCTPProdectCode { get; set; }
        public string GCVVehicleType { get; set; }
        public string PCVVehicleType { get; set;}
        public string MISCVehicleType { get; set; }
    }
    public class TokenData
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
