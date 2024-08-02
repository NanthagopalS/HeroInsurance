using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class GetRoleLevelByBUIdforRoleCreationResponseModel
    {
        public string? RoleLevelId { get; set; }
        public string? RoleLevelName { get; set; }
        public int? PriorityIndex { get; set; }
        public bool? IsEnable { get; set; }
    }
}

