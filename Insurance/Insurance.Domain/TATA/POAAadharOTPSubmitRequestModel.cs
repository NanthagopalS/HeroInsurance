using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class POAAadharOTPSubmitRequestModel
    {
        public string ProposalNo { get; set; }
        public string CustomerName { get; set; }
        public string ClientId { get; set; }
        public string OTP { get; set; }
        public string IdNumber { get; set; }
        public string LeadId { get; set; }
    }
    public class POAAadharOTPSubmitRequestDto
    {
        public string proposal_no { get; set; }
        public string customer_name { get; set; }
        public string id_type { get; set; }
        public string id_num { get; set; }
        public string client_id { get; set; }
        public string otp { get; set; }
        public string product { get; set; }
    }


}
