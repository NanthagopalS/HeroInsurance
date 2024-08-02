using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class SendCompletedRegisterationMailResponseModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string POSPId { get; set; }
        public string CreatedByUserName { get; set; }
    }
}
