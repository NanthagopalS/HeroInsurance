using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Reliance
{
    public class ReliancePaymentResponseModel
    {
        public string QuoteTransactionId { get; set; }
        public string StatusID { get; set; }
        public string PoliCyNumber { get; set; }
        public string TransactionNumber { get; set; }
        public string OptionalValue { get; set; }
        public string GatewayName { get; set; }
        public string ProposalNumber { get; set; }
        public string TransactionStatus { get; set; }
        public string ProductCode { get; set; }
        public string PolicDocumentLink { get; set; }

    }
}
