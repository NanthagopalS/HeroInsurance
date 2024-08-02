using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
	public class TATABreakInPaymentRequestModel
	{
        public string ProposalNo { get; set; }
        public string TicketId { get; set; }
        public string LeadId { get; set; }
        public string VehicleTypeId { get; set; }
    }
}
