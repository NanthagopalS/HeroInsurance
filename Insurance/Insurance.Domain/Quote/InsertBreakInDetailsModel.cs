using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class InsertBreakInDetailsModel
    {
        public string LeadId { get; set; }
        public bool IsBreakIn { get; set; }
        public string PolicyNumber { get; set; }
        public string BreakInId { get; set; }
        public string BreakinInspectionURL { get; set; }
        public string BreakInInspectionAgency { get; set; }

    }
}
