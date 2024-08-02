using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.Quote.Command.SaveUpdateLead
{
    public class SaveUpdateLeadVmResponse
    {
        public string LeadID { get; set; }
        public string QuoteTransactionId { get; set; }
        public object ProposalRequestBody { get; set; }
    }
}
