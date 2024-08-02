using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class UserRoleSearchResponseModel
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string ReportingUserId { get; set; }
        public string CategoryId { get; set; }
        public string BUId { get; set; }
        public string RoleTypeId { get; set; }
        public bool IsActive { get; set; }
        public string UserRoleId { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string EmpID { get; set; }
        public string DOB { get; set; }
        public string UserCategoryName { get; set; }
        public string BUName { get; set; }
        public string RoleTypeName { get; set; }
        public string RoleName { get; set; }
    }
}

