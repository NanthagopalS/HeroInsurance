namespace Insurance.Domain.ManualPolicy
{
    public class GetManualPolicyListModel
    {
        public IEnumerable<ManualPolicyList> ManualPolicyList { get; set; }

        public int TotalRecords { get; set; }

    }

    public class ManualPolicyList
    {
        public string MotorType { get; set; }
        public string PolicyType { get; set; }
        public string PolicyCategory { get; set; }
        public string BasicOD { get; set; }
        public string BasicTP { get; set; }
        public string TotalPremium { get; set; }
        public string NetPremium { get; set; }
        public string GST { get; set; }
        public string PolicyNumber { get; set; }
        public string EngineNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string RegistrationNo { get; set; }
        public string IDV { get; set; }
        public string Insurer { get; set; }
        public string Make { get; set; }
        public string Fuel { get; set; }
        public string Variant { get; set; }
        public string Month { get; set; }
        public string LeadName { get; set; }
        public string CreatedOn { get; set; }
        public string PolicyStartDate { get; set; }
        public string PolicyEndDate { get; set; }
        public string BusinessType { get; set; }
        public string NCBPercentage { get; set; }
        public string PaymentTxnNumber { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Email { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string Year { get; set; }
        public string PrevPolicyNCB { get; set; }
        public string CubicCapacity { get; set; }
        public string RTOCode { get; set; }
        public string PrevPolicyNumber { get; set; }
        public string CPA { get; set; }
        public string Tenure { get; set; }
        public string InsuranceType { get; set; }
        public string AddOns { get; set; }
        public string NilDep { get; set; }
        public string IsPOSPProduct { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string STATE { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string PinCode { get; set; }
        public string DOB { get; set; }
        public string PANNumber { get; set; }
        public string GrossDiscount { get; set; }
        public string GrossPremium { get; set; }
        public string TotalTP { get; set; }
        public string GVW { get; set; }
        public string SeatingCapacity { get; set; }
        public int TotalRecords { get; set; }


    }

}
