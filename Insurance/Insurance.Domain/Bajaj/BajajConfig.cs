using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Bajaj
{
    public class BajajConfig
    {
        public string BaseURL { get; set; }
        public string QuoteURL { get; set; }
        public string TPQuoteURL { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string TPPassword { get; set; }
        public string InsurerId { get; set; }
        public string InsurerName { get; set; }
        public string InsurerLogo { get; set; }
        public string CKYCURL { get; set; }
        public string CKYCDocumentUploadURL { get; set; }
        public string ProposalURL { get; set; }
        public string TPProposalURL { get; set; }
        public string PaymentURL { get; set; }
        public string ReturnURL { get; set; }
        public string TPReturnURL { get; set; }
        public string TPUserID { get; set; }
        public string PaymentUserName { get; set; }
        public string PaymentPassword { get; set; }
        public string ApplicationId { get; set; }
        public string SourceName { get; set; }
        public string TPGeneratePolicyURL { get; set; }
        public string GeneratePolicy { get; set; }
        public string TPPDFMode { get; set; }
        public string PDFMode { get; set; }
        public string PaymentMode { get; set; }
        public string BreakInPinGenerateURL { get; set; }
        public string BreakinPinStatusUrl { get; set; }
        public BajajCKYCRequestDto CKYCData { get; set; }
        public string KYCEncryptionKey { get; set; }
        public string KYCEncryptionIV { get; set; }
        public string KYCBusinessCorelationId { get; set; }
        public string KYCUserId { get; set; }
        public bool IsTPEnable { get; set; }
    }
}
