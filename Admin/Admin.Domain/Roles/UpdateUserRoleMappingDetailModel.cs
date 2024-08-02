using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class UpdateUserRoleMappingDetailModel
    {
        public string RoleId { get; set; }
        public string RoleTypeId { get; set; }
        public string RoleName { get; set; }
        public string BUId { get; set; }
        public string RoleLevelId { get; set; }
        public IList<UserRoleMappingDetailCommandUpdateModel> UserRoleMappingDetailCommandUpdate { get; set; }
    }
    public record UserRoleMappingDetailCommandUpdateModel
    {
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
    }
}
