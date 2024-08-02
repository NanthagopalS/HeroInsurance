using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.TicketManagement
{
    public class GetPOSPDetailsByDeactiveTicketIdResponceModel
    {
        public string UserId { get; set; }
        public string TicketId { get; set; }
        public string POSPName { get; set; }
        public string PanNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; } = "India";

        public string JoinDate { get; set; }

        public DateTime releavingDate { get; set; } = DateTime.Now;

        public DateTime effectiveFrom { get; set; } = DateTime.Now;
        public string EmailId { get; set; }
        public string POSPId { get; set; }

    }
}
