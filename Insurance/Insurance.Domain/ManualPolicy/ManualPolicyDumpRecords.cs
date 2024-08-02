namespace Insurance.Domain.ManualPolicy
{
    public class EmailManualPolicyValidationResponce
    {
        public IEnumerable<ManualPolicyDumpRecords> ManualPolicyDumpRecords { get; set; }
        public IEnumerable<ManualPolicyDumpErrors> ManualPolicyDumpErrors { get; set; }
        public ManualPolicyCount manualPolicyCounts { get; set; }
    }
    public class ManualPolicyDumpRecords
    {
        public string UserEmail { get; set; }
        public string MotorType { get; set; }
        public string PolicyType { get; set; }
        public string PolicyCategory { get; set; }
        public string BasicOD { get; set; }
        public string BasicTP { get; set; }
        public string TotalPremium { get; set; }
        public string NetPremium { get; set; }
        public string ServiceTax { get; set; }
        public string PolicyNo { get; set; }
        public string EngineNo { get; set; }
        public string ChasisNo { get; set; }
        public string VehicleNo { get; set; }
        public string IDV { get; set; }
        public string Insurer { get; set; }
        public string Make { get; set; }
        public string Fuel { get; set; }
        public string Variant { get; set; }
        public string ManufacturingMonth { get; set; }
        public string CustomerName { get; set; }
        public string PolicyIssueDate { get; set; }
        public string PolicyStartDate { get; set; }
        public string PolicyEndDate { get; set; }
        public string BusinessType { get; set; }
        public string NCB { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeDate { get; set; }
        public string ChequeBank { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
        public string ManufacturingYear { get; set; }
        public string PreviousNCB { get; set; }
        public string CubicCapacity { get; set; }
        public string RTOCode { get; set; }
        public string PreviousPolicyNo { get; set; }
        public string CPA { get; set; }
        public string Period { get; set; }
        public string InsuranceType { get; set; }
        public string AddOnPremium { get; set; }
        public string NilDep { get; set; }
        public string IsPOSPProduct { get; set; }
        public string CustomerAddress { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PhoneNo { get; set; }
        public string PinCode { get; set; }
        public string CustomerDOB { get; set; }
        public string PANNo { get; set; }
        public string GrossDiscount { get; set; }
        public string TotalTP { get; set; }
        public string GVW { get; set; }
        public string SeatingCapacity { get; set; }
    }
    public class ManualPolicyDumpErrors
    {
        public string DumpId { get; set; }
        public string PolicyId { get; set; }
        public string ErrorLog { get; set; }
        public string CreatedOn { get; set; }
    }
    public class ManualPolicyCount
    {
        public int policyuploadedsuccessful { get; set; }
        public int policyfailed { get; set; }
        public int totalpolicy { get; set; }
        public string EmailId { get; set; }
        public string UserName { get; set; }
    }

    public class ManualPolicyErrorLogsList
    {
        public ManualPolicyErrorLogsList()
        {
            manualPolicyDumps = new List<ManualPolicyDumpErrors>();
            manualPolicyValidations = new List<ManualPolicyDumpRecords>();
        }
        public List<ManualPolicyDumpRecords> manualPolicyValidations { get; set; }
        public List<ManualPolicyDumpErrors> manualPolicyDumps { get; set; }
    }
}
