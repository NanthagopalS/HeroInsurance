using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class RoleSearchResponseAllModel
    {
        public string RoleTitleName { get; set; }
        public string RoleTypeName { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool isActive { get; set; }
        public int RoleModulePermissionID { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public int IdentityRoleId { get; set; }
        public string CreatedBy { get; set; }
        public int RoleTypeID { get; set; }
        public string BUID { get; set; }
        /*
               public string UpdatedBy { get; set; }

               public string UpdatedOn { get; set; }


       */
    }
}
