using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }       
        public string RoleId { get; set; }
        public bool IsEmailVerified { get; set; }
        public string RoleName { get; set; }
        public string UserRoleMappingName { get; set; }
        public string POSPLeadId { get; set; }
        public string UserProfileStage { get; set; }
        public string POSPId { get; set; }
        public string Environment { get; set; }
        public string CreatedBy { get; set; }
        public string IsActive { get; set; }
        public string OnboardingDate { get; set; }

    }
}
