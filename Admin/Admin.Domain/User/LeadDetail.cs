using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class LeadDetail
    {
        public IEnumerable<PersonalDetailModel> PersonalDetail { get; set; }
        public IEnumerable<VehicleDetailModel> VehicleDetail { get; set; }
        public IEnumerable<PolicyDetails> policyDetails { get; set; }
    }
    public class PersonalDetailModel
    {
        public string? LeadId { get; set; }
        public string? LeadName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? DOB { get; set; }
        public string? PANNumber { get; set; }
        public string? AadharNumber { get; set; }
        public string? Address { get; set; }
        public string? Education { get; set; }
        public string? Profession { get; set; }
        public string? NomineeName { get; set; }
        public string? NomineeAge { get; set; }
        public string? NomineeRelation { get; set; }
        public string? ProposalRequestBody { get; set; }
    }

    public class VehicleDetailModel
    {
       public string? regNo { get; set; }
        public string? regDate { get; set; }
        public string? Maker { get; set; }
        public string? Model { get; set; }
        public string? Variant { get; set; }
        public string? VehicleType { get; set; }
        public string? AccountHolder { get; set; }
        public string? EngineNumber { get; set; }
        public string? ChassisNumber { get; set; }

    }

    public class PolicyDetails
    {
        public string? PolicyStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PolicyType { get; set; }
        public string? InsuranceCompanyName { get; set; }
        public string? PolicyNumber { get; set; }
        public string? TransactionNumber { get; set; }
    }
}
