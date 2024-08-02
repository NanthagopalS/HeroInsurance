using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Roles
{
    public class RoleMappingResponseModel
    {
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string EmpID { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string RoleTypeName { get; set; }
        public int RoleTypeID { get; set; }
        public string UserID { get; set; }
        public int IdentityRoleId { get; set; }
        public string RoleTitleName { get; set; }
        public int CategoryID { get; set; }
        public string UserCategoryName { get; set; }
        public string IsActive { get; set; }

        public int UserRoleID { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

