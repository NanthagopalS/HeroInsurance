using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundJobs.Repository.Models
{
	public class TATABreakInStatusModel
	{
        public string LeadId { get; set; }
		public string ProposalNo { get; set; }
		public string TicketId { get; set; }
		public string PaymentId { get; set;}
		public string VehicleTypeId { get; set; }
		public string QuoteTransactionId {  get; set; }

    }
}
