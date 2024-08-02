using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class RoleModulePermissionModel
    {
        public string RoleTypeId { get; set; }
        public string RoleTitleName { get; set; }
        public string BUId { get; set; }
        public string RoleLevelId { get; set; }
        public string CreatedBy { get; set; }
        public IList<RoleModulePermissionCommandInsertModel> RoleModulePermissionCommandInsert { get; set; }
    }
    public record RoleModulePermissionCommandInsertModel
    {
        public string ModuleId { get; set; }
        public string RoleTypeId { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public string CreatedBy { get; set; }

    }
   
}
