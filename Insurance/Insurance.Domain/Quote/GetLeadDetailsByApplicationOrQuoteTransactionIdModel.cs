using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class GetLeadDetailsByApplicationOrQuoteTransactionIdModel
    {
        public string ApplicationId { get; set; }
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
    }
}
