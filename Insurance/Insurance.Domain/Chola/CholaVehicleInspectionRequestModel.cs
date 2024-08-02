using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Chola
{
    public class CholaVehicleInspectionRequestModel
    {
        public string QuoteID { get; set; }
        public string CustomerName { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string Productcode { get; set; }
        public string Vehiclemodelcode { get; set; }
        public string RegistrationNumber { get; set; }
        public string Intermediary_Code { get; set; }
        public string TieupFlag { get; set; }
    }
}
