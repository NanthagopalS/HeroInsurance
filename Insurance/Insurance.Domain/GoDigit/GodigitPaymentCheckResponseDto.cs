using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.GoDigit
{
    public class GodigitPaymentCheckResponseDto
    {
        public string applicationId { get; set; }
        public string premiumAmount { get; set; }
        public string dateOfTransaction { get; set; }
        public string status { get; set; }
        public object paymentGatewayStatus { get; set; }
        public string transactionNumber { get; set; }
        public string applicationNumber { get; set; }
        public string lastModifiedDate { get; set; }
    }
}
