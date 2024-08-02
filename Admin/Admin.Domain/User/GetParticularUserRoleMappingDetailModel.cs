using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class GetParticularUserRoleMappingDetailModel
    {
        public string? UserRoleMappingId { get; set; }
        public string? UserId { get; set; }
        public string? Password { get; set; }
        public string? RoleTypeId { get; set; }
        public string? RoleTypeName { get; set; }
        public string? BUId { get; set; }
        public string? BUName { get; set; }
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? ReportingIdentityRoleId { get; set; }
        public string? ReportingIdentityRoleName { get; set; }
        public string? ReportingUserId { get; set; }
        public string? ReportingUserName { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CreatedBy { get; set; }
        public string? UserName { get; set; }
        public string? EmpID { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public string? Gender { get; set; }
        public string? DOB { get; set; }
        public string? ProfilePictureID { get; set; }
        public string? DocumentId { get; set; }
        public string? ImageStream { get; set; }
        public bool? IsActiveStatus { get; set; }
        public string? StatusName { get; set; }
        public bool IsHead { get; set; }
        public string DisplayBUName { get; set;}
    }
}
