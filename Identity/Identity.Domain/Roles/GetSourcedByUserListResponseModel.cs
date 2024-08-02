using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Roles
{
    public class GetSourcedByUserListResponseModel
    {
        public IEnumerable<SourcedByUser> SourcedByUser { get; set; }
        public IEnumerable<ServicedByUser> ServicedByUser { get; set; }
        
    }

    public class SourcedByUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }

    public class ServicedByUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
