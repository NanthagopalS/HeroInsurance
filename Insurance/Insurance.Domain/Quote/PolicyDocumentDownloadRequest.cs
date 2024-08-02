using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class PolicyDocumentDownloadRequest
    {
        public string TransactionId { get; set; }
        public string VehicleTypeId { get; set; }
        public string PolicyTypeId { get; set; }
        public string EngineNumber { get; set; }
        public string VehicleNumber { get; set; }
        public int CVCategoryId { get; set; }
    }
}
