using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class UserMappingInsertInputModel
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string ReportingUserId { get; set; }
        public string CategoryId { get; set; }
        public string BUId { get; set; }
        public string RoleTypeId { get; set; }
        public bool IsActive { get; set; }
    }
}

  