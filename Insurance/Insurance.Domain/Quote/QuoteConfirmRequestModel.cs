using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class QuoteConfirmRequestModel
    {
        public string QuoteTransactionId { get; set; }
        public string VariantId { get; set; }
        public string VehicleNumber { get; set; }
        public string ManufacturingMonthYear { get; set; }
        public string RegistrationDate { get; set; }
        public string Customertype { get; set; }
        public PreviousPolicyModels PreviousPolicy { get; set; }
        public bool IsPACover { get; set; }
        public string PACoverTenure { get; set; }
        public bool IsHavePACover { get; set; }
        public bool isPrevPolicyEngineCover { get; set; }
        public bool isPrevPolicyNilDeptCover { get; set; }
        public bool isPrevPolicyRTICover { get; set; }
        public bool isPrevPolicyConsumablesCover { get; set; }
        public bool isPrevPolicyTyreCover { get; set; }
        public bool isPrevPolicyCNGLPGCover { get; set; }
        public PolicyDatesResponse PolicyDates { get; set; }
        public string VehicleTypeId { get; set; }
        public bool IsBrandNewVehicle { get; set; }
        public string CompanyName { get; set; }
        public string DOI { get; set; }
        public string GSTNo { get; set; }
        public string InsurerId { get; set; }
        public bool IsBreakin { get; set; }
        public bool IsSelfInspection { get; set; }
        public bool IsApprovalRequired { get; set; }
        public bool IsQuoteDeviation { get; set; }
        public string CurrentNCBPercentage { get; set; }
        public string LeadId { get; set; }
        public string CategoryId { get; set; }
        public List<CoverMasterDetails> CoverMasterDetails { get; set; }

    }
    public class PreviousPolicyModels
    {
        public bool IsPreviousPolicy { get; set; }
        public string PreviousPolicyNumber { get; set; }
        public string PreviousPolicyTypeId { get; set; }
        public string SAODInsurer { get; set; }
        public string SAODPolicyExpiryDate { get; set; }
        public string TPInsurer { get; set; }
        public string TPPolicyExpiryDate { get; set; }
        public bool IsPolicyExpired { get; set; }
        public bool IsPreviousYearClaim { get; set; }
        public string NCBId { get; set; }
        public string PreviousPolicyNumberSATP { get; set; }

    }
    public class CoverMasterDetails
    {
        public string CoverId { get; set; }
        public bool IsChecked { get; set; }
    }
}

