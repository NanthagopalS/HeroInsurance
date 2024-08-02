using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCSubmitPaymentRequestDto
    {
        public string TransactionID { get; set; }
        public bool GoGreen { get; set; }
        public string IsReadyToWait { get; set; }
        public string PolicyCertifcateNo { get; set; }
        public string PolicyNo { get; set; }
        public string Proposal_no { get; set; }
        public string Inward_no { get; set; }
        public string Request_IP { get; set; }
        public string Customer_Details { get; set; }
        public string Policy_Details { get; set; }
        public string Req_GCV { get; set; }
        public string Req_MISD { get; set; }
        public string Req_PCV { get; set; }
        public Payment_Detailss Payment_Details { get; set; }
        public string IDV_DETAILS { get; set; }
        public string Req_ExtendedWarranty { get; set; }
        public string Req_Policy_Document { get; set; }
        public string Req_PEE { get; set; }
        public string Req_TW { get; set; }
        public string Req_GHCIP { get; set; }
        public string Req_PolicyConfirmation { get; set; }
    }   
    
    public class Payment_Detailss
    {
        public string GC_PaymentID { get; set; }
        public string BANK_NAME { get; set; }
        public string BANK_BRANCH_NAME { get; set; }
        public string PAYMENT_MODE_CD { get; set; }
        public string PAYER_TYPE { get; set; }
        public decimal PAYMENT_AMOUNT { get; set; }
        public string INSTRUMENT_NUMBER { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string OTC_Transaction_No { get; set; }
        public int IsReserved { get; set; }
        public string IsPolicyIssued { get; set; }
        public string Elixir_bank_code { get; set; }
    }

}
