using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Chola
{
    public class CholaVehicleInspectionResponseModel
    {
        public string QuoteID { get; set; }
        public string Referencenumber { get; set;}
        public string Breakin_InspectionURL { get; set; }
        public string Flag { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}
