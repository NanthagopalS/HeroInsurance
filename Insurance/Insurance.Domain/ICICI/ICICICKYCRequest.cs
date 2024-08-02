using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.ICICI
{

    public class ICICICKYCRequest
    {
        public string correlationId { get; set; }
        public string certificate_type { get; set; }
        public bool pep_flag { get; set; }
        public Pan_Details pan_details { get; set; }
    }

    public class Pan_Details
    {
        public string pan { get; set; }
        public string dob { get; set; }
    }

}
