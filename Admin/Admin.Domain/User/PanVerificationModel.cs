using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class PanVerificationModel
    {
        public string PanVerificationId { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string DOB { get; set; }
        public string InstanceId { get; set; }
        public string InstanceCallBackUrl { get; set; }

    }
}
