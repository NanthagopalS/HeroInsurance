using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class POSPInActiveDetailsModel
    {
        public string? POSPId { get; set; }
        public string? UserName { get; set; }
        public string? MobileNumber { get; set; }
        public string? ReportingManager { get; set; }
        public string? PolicySold { get; set; }
        public string? Premium { get; set; }
    }
}
