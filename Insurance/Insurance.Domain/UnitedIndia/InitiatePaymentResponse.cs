using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.UnitedIndia
{
    public class InitiatePaymentResponse
    {
        public InitiatePaymentResponseHead head { get; set; }
        public InitiatePaymentResponseBody body { get; set; }
    }

    public class InitiatePaymentResponseHead
    {
        public string responseTimestamp { get; set; }
        public string version { get; set; }
        public string clientId { get; set; }
        public string signature { get; set; }
    }

    public class InitiatePaymentResponseBody
    {
        public Resultinfo resultInfo { get; set; }
        public string txnToken { get; set; }
        public bool isPromoCodeValid { get; set; }
        public bool authenticated { get; set; }
    }

    public class Resultinfo
    {
        public string resultStatus { get; set; }
        public string resultCode { get; set; }
        public string resultMsg { get; set; }
    }

}
