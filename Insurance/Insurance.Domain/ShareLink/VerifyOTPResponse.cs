using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.ShareLink
{
    public class VerifyOTPResponse
    {
        public bool IsValidOTP { get; set; }
        public string MobileNumber { get; set; }
        public string wrongOtpCount { get; set; }
    }
}
