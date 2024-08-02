using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class DeactivateUserByIdModal
    {
        public string UserId { get; set; }
        public int IsActive { get; set; } = 0;
    }
}
