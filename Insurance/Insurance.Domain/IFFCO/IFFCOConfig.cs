using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.IFFCO
{
    public class IFFCOConfig
    {
        public string BaseURL { get; set; }
        public string IdvURL { get; set; }
        public string QuoteURLForBrandNew { get; set; }
        public string QuoteUrlForODAndComp { get; set; }
        public string QuoteUrlForTP { get; set; }
        public string InsurerId { get; set; }
        public string InsurerName { get; set; }
        public string InsurerLogo { get; set; }
        public string PartnerBranch { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerSubBranch { get; set; }
        public string MessageId { get; set; }
        public string SAODPlanType { get; set; }
        public string SATPPlanType { get; set; }
        public string ComprehensivePlanType { get; set; }
        public BasicAuth BasicAuth { get; set; }
        public string VerifyCKYCURL { get; set; }
        public string CreateCKYC { get; set; }
        public string Duration { get; set; }
        public string PaymentURL { get; set; }
        public string PaymentResponseURL { get; set; }
        public string PolicyDownload { get; set; }
        public string NewVehiclePlanType { get; set; }
        public string ProposalSumbitFormURL { get; set; }
        public string BreakinURL { get; set; }
        public string BreakinToken { get; set; }
        public string BreakinVehicleType { get; set; }
        public string InspectionPurpose { get; set; }
        public string BreakinPaidBy { get; set; }
        public string BreakinStatusURL { get; set; }
        public string UniqIdConfig { get; set; }
        public string IdvCVUrl { get; set; }
        public string CVQuoteUrl { get; set; }
        public string CVPartnerCode { get; set; }
        public string CVPartnerBranch { get; set; }
        public string CVPartnerSubBranch { get; set; }
    }
    public class BasicAuth
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CVUsername { get; set; }
        public string CVPassword { get; set; }
    }
}
