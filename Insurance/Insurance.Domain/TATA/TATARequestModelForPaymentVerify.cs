using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATARequestModelForPaymentVerify
    {
        public string LeadId { get; set; }
        public string PaymentId { get; set; }
        public string VehicleTypeId { get; set; }
        public string ProposalNumber { get; set; }
    }
}
