using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class ReferralNewUserDetailsModel
    {
        public string ReferralModeType { get; set; }
        public string Environment { get; set; }
        public string ReferralId { get; set; }
        public bool? IsEmailExist { get; set; }
        public bool? IsMobileExist { get; set; }
        public string ErrorMessage { get; set; }
        public string POSPId { get; set; }
        public string POSPName { get; set; }
    }
}
