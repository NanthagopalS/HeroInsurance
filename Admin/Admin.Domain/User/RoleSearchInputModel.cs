using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class RoleSearchInputModel
    {
        public string RoleTitleName { get; set; }
        public string RoleTypeName { get; set; }
        public string CreatedFrom { get; set; }
        public string CreatedTo { get; set; }
    }
}

