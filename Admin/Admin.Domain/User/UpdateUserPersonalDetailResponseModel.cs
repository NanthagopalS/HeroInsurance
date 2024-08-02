using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class UpdateUserPersonalDetailResponseModel
    {
        public int UserId { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Pincode { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? EducationalQualification { get; set; }
        public string? POSPSourceMode { get; set; }
        public string? InsuranceSellingExperience { get; set; }
        public string? ICName { get; set; }
        public string? PremiumSold { get; set; }
        public bool IsSelling { get; set; }
        public string? BankName { get; set; }
        public string? AccountNo { get; set; }
        public string? IFSC { get; set; }
        public string? BankUsername { get; set; }
        public string? DocumentName { get; set; }

    }
}
