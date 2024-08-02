using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class UserRoleModel
    { 

        public string UserName { get; set; }
        public string EmpID { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string ProfilePictureID { get; set; }
        public string RoleTypeId { get; set; }
        public string BUId { get; set; }
        public string RoleId { get; set; }
        public string ReportingIdentityRoleId { get; set; }
        public string ReportingUserId { get; set; }
        public string CategoryId { get; set; }
        public string CreatedBy { get; set; }
        public byte[] ImageStream { get; set; }
        public string DocumentId { get; set; }




        // public byte[] ImageStream { get; set; }
        // public string Image64 { get; set; }
        // public bool StatusUser { get; set; }

        // public string IdentityRoleId { get; set; }
        //// public string UserID { get; set; }
        // public bool StatusRoleUser { get; set; }


    }

}
