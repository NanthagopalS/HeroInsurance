using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class UserRoleSearchInputModel
    {
        public string EMPID { get; set; }
        public string RoleTypeName { get; set; }
       public string Status { get; set; }
        public string CreatedFrom { get; set; }
        public string CreatedTo { get; set; }

    }
}
