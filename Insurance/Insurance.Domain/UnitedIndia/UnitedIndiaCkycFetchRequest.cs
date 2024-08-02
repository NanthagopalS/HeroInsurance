using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.UnitedIndia
{
    public record UnitedIndiaCkycFetchRequest
    {
        public string aadhar_last_four_digits { get; set; }
        public string ckyc_no { get; set; }
        public string customer_name { get; set; }
        public string customer_type { get; set; }
        public string dob { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string mobile_no { get; set; }
        public string oem_unique_identifier { get; set; }
        public string pan { get; set; }
        public string pincode { get; set; }
        public string tieup_name { get; set; }
        public string transactionid { get; set; }
        public string redirecturl { get; set; }

    }
}
