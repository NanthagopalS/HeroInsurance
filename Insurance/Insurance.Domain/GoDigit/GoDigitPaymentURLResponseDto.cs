using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.GoDigit
{
    public class GoDigitPaymentURLResponseDto
    {
        public int InsurerStatusCode { get; set; }
        public string timestamp { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public string PaymentURL { get; set; }
        public string ValidationMessage { get; set; }
    }
}
