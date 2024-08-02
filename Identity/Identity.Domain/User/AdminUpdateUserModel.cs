using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class AdminUpdateUserModel
    {
        public string UserId { get; set; }
        public string NewPassWord { get; set; }
        public string ConfirmPassWord { get; set; }
        public string OldPassWord { get; set; }
    }
}
