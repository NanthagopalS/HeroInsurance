namespace Insurance.Domain.Customer
{
    public class GetCustomersResponseModel
    {
        public IEnumerable<GetCustomersListModel> GetCustomersListModel { get; set; }
        public int TotalRecords { get; set; }
    }
    public record GetCustomersListModel
    {
        public string UserId { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string PolicyNo { get; set; }
        public string PolicyType { get; set; }
        public string ProductType { get; set; }
        public string PolicyIssueDate { get; set; }
        public string Premium { get; set; }
        public string GeneratedOn { get; set; }
        public string ExpiringOn { get; set; }
        public string LeadId { get; set; }
        public string InsurerId { get; set; }
        public string QuoteTransactionID { get; set; }

        public string InsurerName { get; set; }
        public int SerialNumber { get; set; }
        public int TotalRecord { get; set; }
        public string ODTPExp { get; set; }

        public string RTOId { get; set; }
        public string PolicyTypeId { get; set; }
        public string RegNo { get; set; }
        public string RegDate { get; set; }
        public string Model { get; set; }
        public string VehicleType { get; set; }
        public string VehicleTypeId { get; set; }
        public string RegAuthority { get; set; }
        public string VariantName { get; set; }
        public string VariantId { get; set; }
        public string FuelId { get; set; }
        public string FuelName { get; set; }
        public string Maker { get; set; }
        public string PreviousSAODInsurer { get; set; }
        public string PrevPolicyTypeId { get; set; }
        public string IsPrevPolicy { get; set; }
        public string PreviousSATPInsurer { get; set; }
        public string PreviousPolicyNumberSAOD { get; set; }
        public string PreviousPolicyExpirySAOD { get; set; }
        public string PrevPolicyNCB { get; set; }
        public string PrevPolicyClaims { get; set; }
        public string IsPolicyExpired { get; set; }
        public string PrevPolicyExpiryDate { get; set; }
        public bool IsBrandNew { get; set; }
        public string RTOCode { get; set; }
        public string PolicyStartDate { get; set; }
        public string PolicyEndDate { get; set; }
        public string PolicyStatus { get; set; }
        public string TotalPremium { get; set; }
        public string PolicySource { get; set; }
        public string PolicyNature { get; set; }

    }
}
