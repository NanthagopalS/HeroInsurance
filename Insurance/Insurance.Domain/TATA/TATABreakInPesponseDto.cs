using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
 {
	    public class TATABreakInPesponseDto
	    {
		
			public int status { get; set; }
			public string message_txt { get; set; }
			public ResponseData[] data { get; set; }
	    }
		public class ResponseData
	{
			public string proposal_no { get; set; }
			public string inspection_no { get; set; }
			public string result { get; set; }
			public string inspection_status { get; set; }
			public Policy[] policy { get; set; }
		}

		public class Policy
		{
			public string policy_id { get; set; }
			public string quote_id { get; set; }
			public string proposal_id { get; set; }
			public string discount_id { get; set; }
			public string document_id { get; set; }
			public string nstp_id { get; set; }
			public string quote_no { get; set; }
			public string proposal_no { get; set; }
			public string payment_id { get; set; }
			public double premium_value { get; set; }
			public string pol_start_date { get; set; }
		}


}

