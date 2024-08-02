using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class GetRecipientListResponseModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? EmpId { get; set; }
        public string? POSPId { get; set; }
    }
}
