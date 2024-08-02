using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{

    public class IDVRequestModel
    {
        public string TransactionID { get; set; }
        public IDV_DETAILS IDV_DETAILS { get; set; }
    }

    public class IDV_DETAILS
    {
        public string ModelCode { get; set; }
        public string RTOCode { get; set; }
        public string Vehicle_Registration_Date { get; set; }
        public string Policy_Start_Date { get; set; }
    }

}
