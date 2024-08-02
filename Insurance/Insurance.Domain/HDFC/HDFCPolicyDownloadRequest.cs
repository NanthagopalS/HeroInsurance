using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCPolicyDownloadRequest
    {
        public string TransactionId { get; set; }
        public string PolicyNumber { get; set; }
        public string VehicleTypeId { get; set; }
        public string PolicyTypeId { get; set; }

    }
}
