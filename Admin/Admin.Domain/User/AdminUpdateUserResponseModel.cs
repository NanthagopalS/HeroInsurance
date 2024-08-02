using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class AdminUpdateUserResponseModel
    {
        public string UserId { get; set; }
        public bool IsUserExists { get; set; }
    }
}
