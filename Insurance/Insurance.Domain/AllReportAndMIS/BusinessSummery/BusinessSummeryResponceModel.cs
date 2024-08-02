namespace Insurance.Domain.AllReportAndMIS.BusinessSummery
{
    /// <summary>
    /// business summery report responce model
    /// </summary>

    public record BusinessSummeryResponceModel
    {
        public IEnumerable<BusinessSummeryRecords> BusinessSummeryRecords { get; set; }
        public int TotalRecords { get; set; }
        public string FileName { get; set; }
    }
    /// <summary>
    /// attributes for business summery report 
    /// </summary>
    /// <summary>
    ///  TotalRecord type="int" accessibility="public" 
    ///  OD_Coverage type = "string" accessibility="public" 
    ///  TP_Coverage type = "string" accessibility="public" 
    ///  VehicleRegnStatus type = "string" accessibility="public" 
    ///  ChassisNo type = "string" accessibility="public" 
    ///  EngineNo type = "string" accessibility="public" 
    ///  CubicCapacity type = "string" accessibility="public" 
    ///  Fuel type = "string" accessibility="public" 
    ///  RTOCode type = "string" accessibility="public" 
    ///  RTOName type = "string" accessibility="public" 
    ///  CurrentNCB type = "string" accessibility="public" 
    ///  PreviousNCB type = "string" accessibility="public" 
    ///  ODD type = "string" accessibility="public" 
    ///  OD_PolicyStartDate type = "string" accessibility="public" 
    ///  OD_PolicyEndDate type = "string" accessibility="public" 
    ///  TP_PolicyStartDate type = "string" accessibility="public" 
    ///  TP_PolicyEndDate type = "string" accessibility="public" 
    ///  CoverNoteNo type = "string" accessibility="public" 
    ///  PolicyNo type = "string" accessibility="public" 
    ///  PolicyIssueDate type = "string" accessibility="public" 
    ///  PolicyReceiveDate type = "string" accessibility="public" 
    ///  BizType type = "string" accessibility="public" 
    ///  SumInsured type = "string" accessibility="public" 
    ///  ODNetPremium type = "string" accessibility="public" 
    ///  TpPrem type = "string" accessibility="public" 
    ///  TotalTp type = "string" accessibility="public" 
    ///  NetPremium type = "string" accessibility="public" 
    ///  GST type = "string" accessibility="public" 
    ///  GrossPremium type = "string" accessibility="public" 
    ///  PrevPolicyNO type = "string" accessibility="public" 
    ///  UserName type = "string" accessibility="public" 
    ///  Uploadtime type = "string" accessibility="public" 
    ///  Product type = "string" accessibility="public" 
    ///  POSP_Product type = "string" accessibility="public" 
    ///  SeatingCapacity type = "string" accessibility="public" 
    ///  CPA type = "string" accessibility="public" 
    ///  Discount_Applied type = "string" accessibility="public" 
    ///  NillDep type = "string" accessibility="public" 
    ///  MotorType type = "string" accessibility="public" 
    ///  BusinessFrom type = "string" accessibility="public" 
    ///  YearofManf type = "string" accessibility="public" 
    ///  Variant type = "string" accessibility="public" 
    ///  InsurerName type = "string" accessibility="public" 
    ///  POSPName type = "string" accessibility="public" 
    ///  POSPCode type = "string" accessibility="public" 
    ///  ReferalCode type = "string" accessibility="public" 
    ///  ReportingEmail type = "string" accessibility="public" 
    ///  RMNAme type = "string" accessibility="public" 
    ///  RMCode type = "string" accessibility="public" 
    ///  Fax type = "string" accessibility="public" 
    ///  PANNumber type = "string" accessibility="public" 
    ///  RegNo type = "string" accessibility="public" 
    ///  VehicleManufacturerName type = "string" accessibility="public" 
    ///  ModelName type = "string" accessibility="public" 
    ///  VechicleType type = "string" accessibility="public" 
    ///  RegDate type = "string" accessibility="public" 
    ///  AddressType type = "string" accessibility="public" 
    ///  Address1 type = "string" accessibility="public" 
    ///  Address2 type = "string" accessibility="public" 
    ///  Address3 type = "string" accessibility="public" 
    ///  Pincode type = "string" accessibility="public" 
    ///  City type = "string" accessibility="public" 
    ///  AState type = "string" accessibility="public" 
    ///  Country type = "string" accessibility="public" 
    ///  DOB type = "string" accessibility="public" 
    ///  Gender type = "string" accessibility="public" 
    ///  UserId type = "string" accessibility="public" 
    ///  POSPId type = "string" accessibility="public" 
    ///  OrderNumber type = "string" accessibility="public" 
    ///  CustomerName type = "string" accessibility="public" 
    ///  StageValue type = "string" accessibility="public" 
    ///  MobileNo type = "string" accessibility="public" 
    ///  EmailId type = "string" accessibility="public" 
    ///  PolicyType type = "string" accessibility="public" 
    ///  PolicyTypeId type = "string" accessibility="public" 
    ///  GeneratedOn type = "string" accessibility="public" 
    ///  ExpiringOn type = "string" accessibility="public" 
    ///  InsuranceProduct type = "string" accessibility="public" 
    ///  Amount type = "string" accessibility="public" 
    ///  PaymentStatus type = "string" accessibility="public" 
    ///  CreatedBy type = "string" accessibility="public" 
    ///  StageId type = "string" accessibility="public" 
    ///  VehicleTypeId type = "string" accessibility="public" 
    ///  IsActive type = "string" accessibility="public" 
    ///  VehicleType type = "string" accessibility="public" 
    ///  InsurerId type = "string" accessibility="public" 
    /// </summary>
    public record BusinessSummeryRecords
    {
        public int TotalRecord { get; set; }
        public string OD_Coverage { get; set; }
        public string TP_Coverage { get; set; }
        public string VehicleRegnStatus { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public string CubicCapacity { get; set; }
        public string Fuel { get; set; }
        public string RTOCode { get; set; }
        public string RTOName { get; set; }
        public string CurrentNCB { get; set; }
        public string PreviousNCB { get; set; }
        public string ODD { get; set; }
        public string PolicyStatus { get; set; }
        public string OD_PolicyStartDate { get; set; }
        public string OD_PolicyEndDate { get; set; }
        public string TP_PolicyStartDate { get; set; }
        public string TP_PolicyEndDate { get; set; }
        public string CoverNoteNo { get; set; }
        public string PolicyNo { get; set; }
        public string PolicyIssueDate { get; set; }
        public string PolicyReceiveDate { get; set; }
        public string BizType { get; set; }
        public string SumInsured { get; set; }
        public string ODNetPremium { get; set; }
        public string TpPrem { get; set; }
        public string TotalTp { get; set; }
        public string NetPremium { get; set; }
        public string GST { get; set; }
        public string GrossPremium { get; set; }
        public string PrevPolicyNO { get; set; }
        public string UserName { get; set; }
        public string Uploadtime { get; set; }
        public string Product { get; set; }
        public string POSP_Product { get; set; }
        public string SeatingCapacity { get; set; }
        public string CPA { get; set; }
        public string Discount_Applied { get; set; }
        public string NillDep { get; set; }
        public string MotorType { get; set; }
        public string BusinessFrom { get; set; }
        public string YearofManf { get; set; }
        public string Variant { get; set; }
        public string InsurerName { get; set; }
        public string POSPName { get; set; }
        public string POSPCode { get; set; }
        public string ReferalCode { get; set; }
        public string ReportingEmail { get; set; }
        public string RMNAme { get; set; }
        public string RMCode { get; set; }
        public string Fax { get; set; }
        public string PANNumber { get; set; }
        public string RegNo { get; set; }
        public string VehicleManufacturerName { get; set; }
        public string ModelName { get; set; }
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
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string UserId { get; set; }
        public string POSPId { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string StageValue { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string PolicyType { get; set; }
        public string PolicyTypeId { get; set; }
        public string GeneratedOn { get; set; }
        public string ExpiringOn { get; set; }
        public string InsuranceProduct { get; set; }
        public string Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string CreatedBy { get; set; }
        public string StageId { get; set; }
        public string VehicleTypeId { get; set; }
        public string IsActive { get; set; }
        public string VehicleType { get; set; }
        public string InsurerId { get; set; }
        public string Sno { get; set; }
        public string BrokerBranchCode { get; set; }
        public string BrokerBranch { get; set; }
        public string CustName { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Pin { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string PANNo { get; set; }
        public string ReferCode { get; set; }
        public string CSCCode { get; set; }
        public string Ref { get; set; }
        public string Insurer { get; set; }
        public string InsurerBranchAutoCode { get; set; }
        public string OD { get; set; }
        public string TP { get; set; }
        public string VehicleNo { get; set; }
        public string Make { get; set; }
        public string CubicCapicaty { get; set; }
        public string PCV { get; set; }
        public string Passenger { get; set; }
        public string BusPropDate { get; set; }
        public string OD_start { get; set; }
        public string OD_end { get; set; }
        public string TP_start { get; }
        public string TP_end { get; }
        public string PolRecvdFormat { get; set; }
        public string GSGrossPremiumT { get; set; }
        public string BankName { get; set; }
        public string POS { get; set; }
        public string TPPOS { get; set; }
        public string POSP { get; set; }
        public string Discount { get; set; }
        public string GVW { get; set; }



    }
}
