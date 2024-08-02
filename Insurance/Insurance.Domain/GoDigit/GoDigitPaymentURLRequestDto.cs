using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.GoDigit
{
    public class GoDigitPaymentURLRequestDto
    {
        public string applicationId { get; set; }
        public string cancelReturnUrl { get; set; }
        public string successReturnUrl { get; set; }
    }
}
