using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Admin.Domain.Roles
{
    public class BUInsertInputModel
    {
        public string RoleTypeId { get; set; }
        public string BULevelId { get; set; }
        public string BUName { get; set; }
        public bool IsActive { get; set; }
        public string RoleId { get; set; }
        public string CreatedBy { get; set; }	

    }
}
