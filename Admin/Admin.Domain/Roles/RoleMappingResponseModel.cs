using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class RoleMappingResponseModel
    {
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string EmpId { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string RoleTypeName { get; set; }
        public string RoleTypeId { get; set; }
        public string UserId { get; set; }
        public string IdentityRoleId { get; set; }
        public string RoleTitleName { get; set; }
        public string CategoryId { get; set; }
        public string UserCategoryName { get; set; }
        public string IsActive { get; set; }
        public string UserRoleId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

