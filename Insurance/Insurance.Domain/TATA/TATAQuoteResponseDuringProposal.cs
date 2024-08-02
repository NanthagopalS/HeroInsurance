using Insurance.Domain.GoDigit.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATAQuoteResponseDuringProposal
    {
        public int InsurerStatusCode { get; set; }
        public string ValidationMessage { get; set; }
        public string ProposalId { get; set; }
        public string TransactionId { get; set; }
        public string TotalPremium { get; set; }
        public string GrossPremium { get; set; }
        public ServiceTax ServiceTax { get; set; }
    }
}
