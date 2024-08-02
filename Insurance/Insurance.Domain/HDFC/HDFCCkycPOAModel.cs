using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCCkycPOAModel
    {
        public string QuoteTransactionId { get; set; }
        public string TxnId { get; set; }
        public string Status { get; set; }
        public string KYCId { get; set; }
        public string LeadId { get; set; }
    }
}
