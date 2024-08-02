using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Roles
{
    public class RoleTypeResponseModel
    {
        public string RoleTypeID { get; set; }

        public string RoleTypeName { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedOn { get; set; }
    }
}
