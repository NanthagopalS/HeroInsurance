using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCPaymentResponseModel
    {
        public string QuoteTransactionId { get; set; }
        public string MerchantId { get; set; }
        public string TransactionNo { get; set; }
        public string TransctionRefNo { get; set; }
        public string BankReferenceNo { get; set; }
        public string TxnAmount { get; set; }
        public string BankCode { get; set; }
        public string IsSIOpted { get; set; }
        public string PaymentMode { get; set; }
        public string PG_Remarks { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionDate { get; set; }
        public string AppID { get; set; }
        public string CheckSum { get; set; }
    }
}
