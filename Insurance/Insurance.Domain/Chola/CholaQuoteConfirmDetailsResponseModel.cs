using Insurance.Domain.GoDigit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Chola
{
    public class CholaQuoteConfirmDetailsResponseModel
    {
        public QuoteConfirmDetailsResponseModel quoteConfirmDetailsResponseModel { get; set; }
        public QuoteResponseModel quoteResponseModel { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string LeadId { get; set; }
        public string TransactionId { get; set; }
    }
}
