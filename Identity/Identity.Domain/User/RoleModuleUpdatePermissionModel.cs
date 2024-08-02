using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class RoleModuleUpdatePermissionModel
    {
        public string RoleID { get; set; }
        public int ModuleID { get; set; }
        public int RoletypeID { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public string UpdatedBy { get; set; }
        public bool isActive { get; set; }

        
    }
}
