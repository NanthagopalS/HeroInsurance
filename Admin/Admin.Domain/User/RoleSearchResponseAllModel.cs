using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class RoleSearchResponseAllModel
    {
        public string RoleTitleName { get; set; }
        public string RoleTypeName { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool isActive { get; set; }
        public string RoleModulePermissionId { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public string IdentityRoleId { get; set; }
        public string CreatedBy { get; set; }
        public string RoleTypeId { get; set; }
        public string BUId { get; set; }
       
    }
}
