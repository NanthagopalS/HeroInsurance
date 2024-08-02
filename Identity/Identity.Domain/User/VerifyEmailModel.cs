using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class VerifyEmailModel
    {
        public string UserId { get; set; }
        public string EmailId { get; set; }
    }
}
