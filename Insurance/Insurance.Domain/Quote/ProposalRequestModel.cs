using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class ProposalRequestModel
    {
        public string RequestBody { get; set; }
        public string QuoteTransactionID { get; set; }
        public string LeadID { get; set; }
        public string VehicleNumber { get; set; }
        public string VariantId { get; set; }
        public string InsurerId { get; set; }
        public string VehicleTypeId { get; set; }
        public string TransactionId { get; set; }
        public bool IsProposal { get; set; }
        public bool IsSharePaymentLink { get; set; }
    }
}
