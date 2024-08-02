using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCCkycPOAStatusResponseModel
    {
        public bool success { get; set; }
        public CKYCData data { get; set; }
    }

    public class CKYCData
    {
        public int iskycVerified { get; set; }
        public string status { get; set; }
    }
}
