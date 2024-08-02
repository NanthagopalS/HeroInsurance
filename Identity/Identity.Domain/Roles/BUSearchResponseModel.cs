using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Roles
{
    public class BUSearchResponseModel
    {

        public int BUID { get; set; }

        public int RoleTypeID { get; set; }
        public int BULevelID { get; set; }

        public string BUName { get; set; }

        public bool IsActive { get; set; }

        public string RoleTypeName { get; set; }

        public string BULevelName { get; set; }

        public int ULevelID { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string RoleId { get; set; }

        public string RoleName { get; set; }
    }

    
}


