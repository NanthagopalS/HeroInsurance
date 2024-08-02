using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Admin.Domain.Roles
{
    public class RoleDetailInsertInputModel
    {
        public string? RoleTypeId { get; set; }
        public string? RoleName { get; set; }
        public string? BUId { get; set; }
        public string? RoleLevelId { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public List<RoleDetailPermissionInsertModel>? RoleDetailPermissionInsert { get; set; }

    }
    public class RoleDetailPermissionInsertModel
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
