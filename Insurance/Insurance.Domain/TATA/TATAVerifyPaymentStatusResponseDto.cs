using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATAVerifyPaymentStatusResponseDto
    {
        public int status { get; set; }
        public string message_txt { get; set; }
        public PolicyDownloadData data { get; set; }
    }

    public class PolicyDownloadData
    {
        public string policy_id { get; set; }
        public string policy_no { get; set; }
        public string payment_status { get; set; }
        public int reference_id { get; set; }
        public string encrypted_policy_id { get; set; }
        public string encrypted_policy_no { get; set; }
    }

}
