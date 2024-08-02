using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class RequestForEditProfileInputModel
    {
        public string? UserId { get; set; }
        public string? RequestType { get; set; }
        public string? NewRequestTypeContent { get; set; }
        public string? CreatedBy { get; set; }
    }
}
