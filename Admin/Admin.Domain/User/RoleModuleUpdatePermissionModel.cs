using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class RoleModuleUpdatePermissionModel
    {
        public string RoleId { get; set; }
        public string ModuleId { get; set; }
        public string RoletypeId { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public string UpdatedBy { get; set; }
        public bool isActive { get; set; }

        
    }
}
