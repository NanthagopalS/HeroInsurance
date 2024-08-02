using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATAPaymentRequestModelDto
    {
        public string payment_mode { get; set; }
        public string online_payment_mode { get; set; }
        public string payer_type { get; set; }
        public string payer_id { get; set; }
        public string payer_pan_no { get; set; }
        public string payer_relationship { get; set; }
        public string payer_name { get; set; }
        public string email { get; set; }
        public string mobile_no { get; set; }
        public string deposit_in { get; set; }
        public string pan_no { get; set; }
        public string[] payment_id { get; set; }
        public string returnurl { get; set; }


    }

}
