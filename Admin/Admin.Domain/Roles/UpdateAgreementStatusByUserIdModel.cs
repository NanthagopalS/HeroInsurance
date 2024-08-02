using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class UpdateAgreementStatusByUserIdModel
    {
        public string UserId { get; set; }
        public string ProcessType { get; set; }
    }
}
