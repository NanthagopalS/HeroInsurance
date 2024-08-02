using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class RoleDetailUpdateInputModel
    {
        public string? RoleId { get; set; }
        public string? RoleTypeId { get; set; }
        public string? RoleName { get; set; }
        public string? BUId { get; set; }
        public string? RoleLevelId { get; set; }
        public bool IsActive { get; set; }
        public string? UpdatedBy { get; set; }
        public List<RoleDetailPermissionUpdateModel>? RoleDetailPermissionUpdate { get; set; }

    }

    public class RoleDetailPermissionUpdateModel
    {
        public string? ModuleId { get; set; }
        public string? RoleTypeId { get; set; }
        public bool? AddPermission { get; set; }
        public bool? EditPermission { get; set; }
        public bool? ViewPermission { get; set; }
        public bool? DeletePermission { get; set; }
        public bool? DownloadPermission { get; set; }
        public string? CreatedBy { get; set; }

    }
}
