using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCCkycTokenResponseModel
    {
        public int InsurerStatusCode { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string token { get; set; }
        public string expiry { get; set; }
    }

}
