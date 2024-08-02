using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class PolicyDatesRequest
    {
        public string RegistrationYear { get; set; }
        public string VehicleType { get; set; }
        public bool IsPreviousPolicy { get; set; }
        public string ODPolicyExpiry { get; set; }
        public string TPPolicyExpiry { get; set; }


    }
}
