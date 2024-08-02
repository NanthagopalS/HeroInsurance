namespace Insurance.Domain.Customer
{
    public class GetCustomerLeadDetailModel
    {
        public IEnumerable<PersonalDetailModel> PersonalDetail { get; set; }
        public IEnumerable<VehicleDetailModel> VehicleDetail { get; set; }
    }
    public class PersonalDetailModel
    {
        public string LeadId { get; set; }
        public string LeadName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string PANNumber { get; set; }
        public string AadharNumber { get; set; }
        public string Address { get; set; }
        public string AddressType { get; set; }
        public string Education { get; set; }
        public string Profession { get; set; }

        public string NFirstName { get; set; }
        public string NLastName { get; set; }
        public string MaritalStatus { get; set; }
        public string AlternateMobileNumber { get; set; }
        public string Income { get; set; }
        public string TotalPremium { get; set; }
        public string InsurerName { get; set; }
        public string PolicyType { get; set; }
        public string PolicyStartDate { get; set; }
        public string PolicyEndDate { get; set; }
        public string PolicyNumber { get; set; }




    }

    public class VehicleDetailModel
    {
        public string regNo { get; set; }
        public string regDate { get; set; }
        public string Maker { get; set; }
        public string Model { get; set; }
        public string VehicleType { get; set; }
        public string AccountHolder { get; set; }
        public string VehicleNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string EngineNumber { get; set; }
        public string InsurerName { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyStartDate { get; set; }
        public string PolicyEndDate { get; set; }
        public string TotalPremium { get; set; }
        public string VariantName { get; set; }
        public string PolicySource { get; set; }
        public string PolicyType { get; set; }

    }
}
