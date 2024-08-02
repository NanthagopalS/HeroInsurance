
namespace Insurance.Domain.Leads
{
    public class GetLeadManagementDetailModel
    {
        public IEnumerable<LeadDetailModelList> LeadDetailModelList { get; set; }
        public int TotalRecords { get; set; } = 0;
    }
    public class LeadDetailModelList
    {
        public string UserId { get; set; }
        public string POSPId { get; set; }
        public string LeadId { get; set; }
        public string CustomerName { get; set; }
        public string StageValue { get; set; }
        public string StageId { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string PANNumber { get; set; }
        public string AadharNumber { get; set; }
        public string PolicyType { get; set; }
        public string GeneratedOn { get; set; }
        public string ExpiringOn { get; set; }
        public string Product { get; set; }
        public string PolicyStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string CreatedBy { get; set; }
        public double Amount { get; set; }
        public string Gender { get; set; }
        public string Education { get; set; }
        public string Profession { get; set; }
        public string RegNo { get; set; }
        public string VehicleManufacturerName { get; set; }
        public string Model { get; set; }
        public string VechicleType { get; set; }
        public string RegDate { get; set; }
        public string AddressType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string AState { get; set; }
        public string Country { get; set; }
        public string PolicyNumber { get; set; }
        public string QuoteTransactionID { get; set; }
        public string InsurerId { get; set; }

        public string DOB { get; set; }
        public int TotalRecord { get; set; } = 0;
    }
}
