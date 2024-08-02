using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATAPaymentRequestModel
    {
        public string QuoteTransactionId { get; set; }
        public string TATAPaymentId { get; set; }
        public string Name { get; set; }
        public string PAN { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string LeadId { get; set; }
        public string VehicleTypeId { get; set; }
    }
}
