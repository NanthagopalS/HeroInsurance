using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class UserRoleSearchResponseModel
    {
        public string UserID { get; set; }
        public string RoleID { get; set; }
        public string ReportingUserID { get; set; }
        public int CategoryID { get; set; }
        public int BUID { get; set; }
        public int RoleTypeID { get; set; }
        public bool IsActive { get; set; }
        public int UserRoleID { get; set; }
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

