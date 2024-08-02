using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.GoDigit
{
    public class GodigitPaymentCKYCReqModel
    {
        public string ApplicationId { get; set; }
        public string PaymentTransactionNumber { get; set; }
        public string PolicyNumber {  get; set; }
        public string LeadId { get; set; }
    }
}
