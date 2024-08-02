using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCPolicyDocumentResponseModel
    {
        public int InsurerStatusCode { get; set; }
        public string CustomerId { get; set; }
        public string ProposalNumber { get; set; }
        public string PolicyNumber { get; set; }
        public string CustomerName { get; set; }
        public string PaymentID { get; set; }
        public string TransactionID { get; set; }
        public string PolicyDocumentBase64 { get; set; }
    }
}
