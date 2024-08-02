using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class UserRoleMappingDetailPermissionModel
    {        
        public string RoleTypeId { get; set; }
        public string RoleName { get; set; }
        public string BUId { get; set; }
        public string RoleLevelId { get; set; }       
        public IList<UserRoleMappingDetailCommandInsertModel> UserRoleMappingDetailCommandInsert { get; set; }
    }   
    public record UserRoleMappingDetailCommandInsertModel
    {
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }       
    }
}
