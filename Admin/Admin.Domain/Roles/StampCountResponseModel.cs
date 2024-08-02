using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class StampCountResponseModel
    {
        public int? TotalStampCount { get; set; }
        public int? TotalUsedStamp { get; set; }
        public int? TotalBlockedStamp { get; set; }
        public int? TotalAvailableStamp { get; set; }
    }
}
