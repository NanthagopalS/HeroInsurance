using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.GoDigit
{
    public class GodigitCKYCCheckResponseDto
    {
        public string policyNumber { get; set; }
        public string policyStatus { get; set; }
        public string paymentStatus { get; set; }
        public string kycVerificationStatus { get; set; }
        public string mismatchType { get; set; }
        public string referenceId { get; set; }
        public string idVerificationDocType { get; set; }
        public string addressVerificationDocType { get; set; }
        public string mode { get; set; }
        public string link { get; set; }
    }
}
