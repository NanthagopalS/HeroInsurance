using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATASubmitOTPResponseDto
    {
        public int status { get; set; }
        public string message_txt { get; set; }
        public OTP_Data data { get; set; }
    }

    public class OTP_Data
    {
        public bool success { get; set; }
        public string req_id { get; set; }
        public OTP_Result result { get; set; }
        public bool verified { get; set; }
        public string id_num { get; set; }
        public string id_type { get; set; }
        public string customer_type { get; set; }
        public DateTime verified_at { get; set; }
    }

    public class OTP_Result
    {
        public string registered_name { get; set; }
        public string age { get; set; }
        public string gender { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public OTP_P_Address p_address { get; set; }
        public OTP_C_Address c_address { get; set; }
    }

    public class OTP_P_Address
    {
        public string country { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
    }

    public class OTP_C_Address
    {
        public string country { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
    }

}
