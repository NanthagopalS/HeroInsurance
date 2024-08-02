using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Customer
{
    public class GetRenewalDetailsByIdResponceModel
    {
        public string LeadName { get; set; }
        public string InsuranceName { get; set; }
        public string InsuranceType { get; set; }
        public string Days { get; set; }
        public string InsurerName { get; set; }
        public string ModelName { get; set; }  
        public string MakeName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PolicyNumber { get; set; }
        public string QuoteTransactionID { get; set; }
        public string InsurerId { get; set; }
        public string VariantName { get; set; }
    }
}
