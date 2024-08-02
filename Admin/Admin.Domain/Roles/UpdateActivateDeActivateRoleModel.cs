using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class UpdateActivateDeActivateRoleModel
    {
        public string RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}
