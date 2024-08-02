using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.UnitedIndia
{
    public class InitiatePaymentRequest
    {
        public Body body { get; set; }
        public Head head { get; set; }
    }

    public class Body
    {
        public string requestType { get; set; }
        public string mid { get; set; }
        public string websiteName { get; set; }
        public string orderId { get; set; }
        public Txnamount txnAmount { get; set; }
        public Userinfo userInfo { get; set; }
        public string callbackUrl { get; set; }
    }

    public class Txnamount
    {
        public string value { get; set; }
        public string currency { get; set; }
    }

    public class Userinfo
    {
        public string custId { get; set; }
        public string mobile { get; set; } //10-digit mobile number of user.Example: 9988000000 Note: Mandatory when the merchant wants to give Debit Card EMI as a payment option to its users.
    }

    public class Head
    {
        public string signature { get; set; }
    }

}
