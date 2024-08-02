using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class CheckUserExistOrNotModel
    {
        public bool IsUserExists { get; set; }
        public bool IsEmpIdExists { get; set; }
        public bool IsMobileNoExists { get; set; }
        public bool IsEmailIdExists { get; set; }

    }
}
