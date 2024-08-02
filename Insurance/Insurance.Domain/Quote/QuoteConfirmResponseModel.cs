using Insurance.Domain.GoDigit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class QuoteConfirmResponseModel
    {
        public QuoteConfirmDetailsResponseModel quoteConfirmResponse { get; set; }
        public QuoteResponseModel quoteResponse { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string LeadId { get; set; }
        public string TransactionId { get; set; }
        public string ResponseReferanceFlag { get; set; }
    }
}
