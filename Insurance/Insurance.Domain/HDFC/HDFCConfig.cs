using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCConfig
    {
        public string BaseURL { get; set; }
        public string IdvURL { get; set; }
        public string QuoteURL { get; set; }
        public string TokenURL { get; set; }
        public string CKYCTokenURL { get; set; }
        public string FetchKYCURL { get; set; }
        public string PGCKYCURL { get; set; }
        public string FetchKYCPOAURL { get; set; }
        public string GenerateCheckSumURL { get; set; }
        public string ProposalURL { get; set; }
        public string CommercialProposalURL { get; set; }
        public string SubmitPaymentDetailsURL { get; set; }
        public string InsurerId { get; set; }
        public string InsurerName { get; set; }
        public string InsurerLogo { get; set; }
        public string PaymentTransNo { get; set; }
        public string AppId { get; set; }
        public string SubscriptionId { get; set; }
        public string PC_ProductCode_NonTP { get; set; }
        public string PC_ProductCode_TP { get; set; }
        public string TW_ProductCode_NonTP { get; set; }
        public string TW_ProductCode_TP { get; set; }
        public TokenData CommanHeaderData { get; set; }
        public string LicenseNo { get; set; }
        public string IlLocation { get; set; }
        public string CreateIMBrokerURL { get; set; }
        public string CertificateStatus { get; set; }
        public string SubmitPOSPCertificateURL { get; set; }
        public string QuoteScope { get; set; }
        public string IdvScope { get; set; }
        public string CKYCScope { get; set; }
        public string PaymentScope { get; set; }
        public string PolicyGenerationScope { get; set; }
        public string POSPScope { get; set; }
        public string CreatePOSPURL { get; set; }
        public string Token { get; set; }
        public string TRANSACTIONID { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string CHANNEL_ID { get; set; }
        public string SOURCE { get; set; }
        public string MakePaymentURL { get; set; }
        public string PGStatusRedirectionURL { get; set; }
        public string PolicyDocumentURL { get; set; }
        public string PGSubmitPayment { get; set; }
        public string Kyc_Key { get; set; }
        public string PCV_ProductCode_NonTP { get; set; }
        public string PCV_ProductCode_TP { get; set; }
        public string GCV_ProductCode_NonTP { get; set; }
        public string GCV_ProductCode_TP { get; set; }
        public string MISDProductCode { get; set; }

    }
    public class TokenData
    {
        public string SOURCE { get; set; }
        public string CHANNEL_ID { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string CREDENTIAL { get; set; }
        public string TRANSACTIONID { get; set; }
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
