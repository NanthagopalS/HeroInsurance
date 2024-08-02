using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class RoleModulePermissionModel
    {
        public int RoleTypeID { get; set; }
        public string RoleTitleName { get; set; }
        public int BUID { get; set; }
        public int RoleLevelID { get; set; }
        public string CreatedBy { get; set; }
        public IList<RoleModulePermissionCommandInsertModel> RoleModulePermissionCommandInsert { get; set; }
    }
    public record RoleModulePermissionCommandInsertModel
    {
       // public string RoleID { get; set; }
        public int ModuleID { get; set; }
       // public int RoletypeID { get; set; }
       // public int IdentityRoleId { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public string CreatedBy { get; set; }

    }
    /*
    public class RoleModulePermissionModel
    {
        public string RoleID { get; set; }
        public int ModuleID { get; set; }
        public int RoletypeID { get; set; }        
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public string CreatedBy { get; set; }       


    }
    */

}
