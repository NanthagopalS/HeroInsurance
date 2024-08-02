using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATANonPanVerifyCKYCIndividualRequestDto
    {
        public string proposal_no { get; set; }
        public string customer_name { get; set; }
        public string id_type { get; set; }
        public string id_num { get; set; }
        public string full_name { get; set; }
        public string dob { get; set; }
        public string gender { get; set; }
        public string req_id { get; set; }
    }

}
