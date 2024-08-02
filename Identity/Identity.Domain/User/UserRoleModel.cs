using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class UserRoleModel
    { 

        public string UserName { get; set; }
        public string EmpID { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string RoleId { get; set; }
        public string CreatedBy { get; set; }
        public bool StatusUser { get; set; }

        public int RoleTypeID { get; set; }
        public int IdentityRoleId { get; set; }
        public int ReportingIdentityRoleId { get; set; }
       // public string UserID { get; set; }
        public string ReportingUserID { get; set; }
        public int CategoryID { get; set; }
        public bool StatusRoleUser { get; set; }
        

    }
   
}
