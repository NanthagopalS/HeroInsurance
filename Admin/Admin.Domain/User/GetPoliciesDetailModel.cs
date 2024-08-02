using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class GetPoliciesDetailModel
    {
        public string POSPId { get; set; }
        public string PolicyNo { get; set; }
        public string PolicyType { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string Premium { get; set; }       
    }
}
