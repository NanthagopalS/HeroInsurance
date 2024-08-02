using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Chola
{
    public class CholaPaymentTaggingRequestModel
    {
        public string PaymentId { get; set; }
        public string Amount  { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionReferenceNumber { get; set; }
        public string LeadId { get; set; }
    }
}
