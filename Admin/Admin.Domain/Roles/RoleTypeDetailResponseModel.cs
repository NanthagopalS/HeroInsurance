using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class RoleTypeDetailResponseModel
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleTypeName { get; set; }
        public string? IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

