using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{

    public class TATATwoWheelerProposalResponseDto
    {
        public int status { get; set; }
        public string message_txt { get; set; }
        public TW_Datum[] data { get; set; }
    }

    public class TW_Datum
    {
        public string quote_id { get; set; }
        public string quote_no { get; set; }
        public string proposal_no { get; set; }
        public string proposal_id { get; set; }
        public string policy_id { get; set; }
        public string product_id { get; set; }
        public string discount_id { get; set; }
        public string nstp_id { get; set; }
        public string payment_id { get; set; }
        public string refferal { get; set; }
        public string tagic_emp_code { get; set; }
        public string mobile_no { get; set; }
        public string email_id { get; set; }
        public double premium_value { get; set; }
        public string document_id { get; set; }
        public string self_inspection_link { get; set; }
        public string inspectionFlag { get; set; }
        public string stage { get; set; }
        public string pol_start_date { get; set; }
        public string payment_stage { get; set; }
        public string policy_stage { get; set; }
        public string proposal_stage { get; set; }
        public string quote_stage { get; set; }
    }

}
