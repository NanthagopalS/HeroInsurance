using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdPartyUtilities.Models.SMS
{
    public record RenewalSMSModel
    {
        public string mobileNumber { get; set; }
        public string daysLeft { get; set; }
        public string vechicleMaker { get; set; }
        public string vechicleModel { get; set; }
        public string leadName { get; set; }
        public string insuranceName { get; set; }
        public string renewalURL { get;set; }
        public string vechicleVariant { get;set; }
    }
}
