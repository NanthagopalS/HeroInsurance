using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class ParticularRoleDetailResponseModel
    {
        public string? RoleTypeID { get; set; }
        public string? RoleName { get; set; }
        public string? RoleLevelID { get; set; }
        public string? BUId { get; set; }

        public bool? IsActive { get; set; }

        public IEnumerable<ParticularPermissionRoleDetailResponseModel> ParticularPermissions { get; set; }
       
    }

    public class ParticularPermissionRoleDetailResponseModel
    {
        public string? RoleModulePermissionId { get; set; }
        public string? RoleTypeId { get; set; }
        public string? ModuleId { get; set; }
        public bool? AddPermission { get; set; }
        public bool? EditPermission { get; set; }
        public bool? ViewPermission { get; set; }
        public bool? DeletePermission { get; set; }
        public bool? DownloadPermission { get; set; }
        public string? ModuleName { get; set; }
        public string? ModuleGroupName { get; set; }

    }
}

