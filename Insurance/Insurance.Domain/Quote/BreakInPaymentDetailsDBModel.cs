using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class BreakInPaymentDetailsDBModel
    {
        public string ProposalRequest { get; set; }
        public string ProposalResponse { get; set; }
        public string ApplicationId { get; set;}
        public string GrossPremium { get; set; }
        public string PaymentLink { get; set; }
        public string PaymentLinkCreatedDate { get; set; }
        public string VehicleTypeId { get; set; }
        public bool IsBreakin { get; set; }
        public bool IsBreakinApproved { get; set; }
        public string LeadId { get; set; }
        public string PANNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LeadFirstName { get; set; }
        public string LeadLastName { get; set; }
        public string CustomerId { get; set; }
        public string ProposalNumber { get; set; }
        public string CustomerType { get; set; }
        public string CompanyName { get; set; }

    }
}
