using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATAPaymentResponseModel
    {
        public string QuoteTransactionId { get; set; }
        public string ProposalNumber { get; set; }
        public string PolicyNumber { get; set; }
    }
}
