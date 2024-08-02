using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class InsertUserDeviceDetailsModel
    {
        public string? UserId { get; set; }
        public string? MobileDeviceId { get; set; }
        public string? BrowserId { get; set; }
        public string? GfcToken { get; set; }
    }
}
