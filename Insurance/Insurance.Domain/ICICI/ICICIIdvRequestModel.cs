using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.ICICI
{
    public class ICICIIdvRequestModel
    {
        public int manufacturercode { get; set; }
        public string BusinessType { get; set; }
        public int rtolocationcode { get; set; }
        public string DeliveryOrRegistrationDate { get; set; }
        public string PolicyStartDate { get; set; }
        public string DealID { get; set; }
        public int vehiclemodelcode { get; set; }
        public Guid correlationId { get; set; }
    }
}
