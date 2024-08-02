using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain
{
    public class CholaPOAStatusRequestModel
    {
        public string AppRefNo { get; set; }
        public string TransactionId { get; set; }
        public string LeadId { get; set; }
    }
}
