using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCPolicyRequestModel
    {
        public string QuoteTransactionId { get; set; }
        public string ProposalNumber { get; set; }
        public string PolicyNumber { get; set; }
        public string TransactionId { get; set; }
        public string VehicleTypeId { get; set; }
        public string PolicyTypeId { get; set; }
        public string GrossPremium { get; set; }
        public string ApplicationId { get; set; }
        public string BankName { get; set; }
        public string PaymentDate { get; set; }
        public string LeadId { get; set; }
        public int CategoryId { get; set; }

    }
}
