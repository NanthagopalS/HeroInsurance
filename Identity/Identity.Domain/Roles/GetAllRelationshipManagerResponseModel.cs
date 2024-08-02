using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Roles
{
    public class GetAllRelationshipManagerResponseModel
    {
        public string UserId { get; set; }

        public string ServicedByUserId { get; set; }
        public string UserName { get; set; }
    }
}
