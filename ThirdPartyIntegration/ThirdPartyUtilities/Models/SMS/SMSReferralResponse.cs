using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdPartyUtilities.Models.SMS
{
    public class SMSRsult
    {
        public string id { get; set; }
        public string phone { get; set; }
        public string details { get; set; }
        public string status { get; set; }
        public int OTP { get; set; }
        public int userName { get; set; }
        public int pOSPName { get; set; }
        public int referralId { get; set; }
    }
    public class SMSReferralResponse
    {
        public SMSRsult SMSResponse { get; set; }
    }
}
