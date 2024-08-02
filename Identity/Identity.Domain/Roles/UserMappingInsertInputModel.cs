using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Roles
{
    public class UserMappingInsertInputModel
    {
        public string UserID { get; set; }
        public string RoleID { get; set; }
        public string ReportingUserID { get; set; }
        public int CategoryID { get; set; }
        public int BUID { get; set; }
        public int RoleTypeID { get; set; }
        public bool IsActive { get; set; }
    }
}

  