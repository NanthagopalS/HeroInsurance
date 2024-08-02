using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class HierarchyManagementDetailResponseModel
    {
        public IEnumerable<UserListModel>? UserList { get; set; }
        public IEnumerable<ParentListModel>? ParentList { get; set; }
       
    }

    public class ParentListModel
    {
        public string? ParentUserId { get; set; }
        public string? ParentUserRoleId { get; set; }
    }
    public class UserListModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePictureStream { get; set; }
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }

    }
}

