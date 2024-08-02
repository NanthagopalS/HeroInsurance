using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Bajaj
{
    public class BajajBreakInPinGenerationRequestModel
    {
        public string userName { get; set; }
        public string transactionId { get; set; }
        public string regNoPart1 { get; set; }
        public string regNoPart2 { get; set; }
        public string regNoPart3 { get; set; }
        public string regNoPart4 { get; set; }
        public string flag { get; set; }
    }
}
