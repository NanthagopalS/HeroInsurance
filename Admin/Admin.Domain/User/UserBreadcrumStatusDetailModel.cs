using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class UserBreadcrumStatusDetailModel
    {
        public int Number { get; set; }
        public string StatusName { get; set; }
        public string StatusValue { get; set; }
        public string StatusId { get; set; }
    }
}
