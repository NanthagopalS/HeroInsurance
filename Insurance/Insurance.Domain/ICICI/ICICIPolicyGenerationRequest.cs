using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.ICICI
{
    public class ICICIPolicyGenerationRequest
    {
        public string CorrelationId { get; set; }
        public string policyNo { get; set; }
        public string customerId { get; set; }
        public string dealId { get; set; }
    }
}