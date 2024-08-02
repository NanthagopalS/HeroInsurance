using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATAVerifyCKYCResponseDto
    {
        public int status { get; set; }
        public string message_txt { get; set; }
        public KYC_Data data { get; set; }
    }

    public class KYC_Data
    {
        public string req_id { get; set; }
        public bool success { get; set; }
        public string error_message { get; set; }
        public string ckyc_remarks { get; set; }
        public Result result { get; set; }
        public bool verified { get; set; }
        public string id_num { get; set; }
        public string id_type { get; set; }
        public string customer_type { get; set; }
        public DateTime verified_at { get; set; }
        public string client_id { get; set; }
        public bool otp_sent { get; set; }
        public bool if_number { get; set; }
        public bool valid_aadhaar { get; set; }
        public string status { get; set; }
    }

    public class Result
    {
        public string FATHERS_NAME { get; set; }
        public string KYC_DATE { get; set; }
        public string UPDATED_DATE { get; set; }
        public string registered_name { get; set; }
        public string age { get; set; }
        public string ckyc_number { get; set; }
        public P_Address p_address { get; set; }
        public C_Address c_address { get; set; }
    }

    public class P_Address
    {
        public string country { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
    }

    public class C_Address
    {
        public string country { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
    }

}
