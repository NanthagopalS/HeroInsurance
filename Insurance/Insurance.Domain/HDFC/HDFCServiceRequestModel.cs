using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{

    public class HDFCServiceRequestModel
    {
        public string ApplicationNumber { get; set; }
        public string TransactionID { get; set; }
        public bool GoGreen { get; set; }
        public string IsReadyToWait { get; set; }
        public string PolicyCertifcateNo { get; set; }
        public string PolicyNo { get; set; }
        public string Proposal_no { get; set; }
        public string Inward_no { get; set; }
        public string Request_IP { get; set; }
        public Customer_Details Customer_Details { get; set; }
        public Policy_Details Policy_Details { get; set; }
        public Req_GCV Req_GCV { get; set; }
        public Req_MISD Req_MISD { get; set; }
        public Req_PCV Req_PCV { get; set; }
        public Payment_Details Payment_Details { get; set; }
        public string IDV_DETAILS { get; set; }
        public string Req_ExtendedWarranty { get; set; }
        public string Req_Policy_Document { get; set; }
        public string Req_PEE { get; set; }
        public Req_TW Req_TW { get; set; }
        public string Req_RE { get; set; }
        public string Req_Fire2111 { get; set; }
        public string Req_ClaimIntimation { get; set; }
        public string Req_ClaimStatus { get; set; }
        public string Req_Renewal { get; set; }
        public Req_Pvtcar Req_PvtCar { get; set; }
        public string Req_HInsurance { get; set; }
        public string Req_IPA { get; set; }
        public string Req_CI { get; set; }
        public string Req_HomeInsurance { get; set; }
        public string Req_RetailTravel { get; set; }
        public string Req_HCA { get; set; }
        public string Req_HF { get; set; }
        public string Req_HI { get; set; }
        public string Req_HSTPI { get; set; }
        public string Req_HSTPF { get; set; }
        public string Req_ST { get; set; }
        public string Req_WC { get; set; }
        public string Req_BSC { get; set; }
        public string Req_Discount { get; set; }
        public string Req_POSP { get; set; }
        public string Req_HSF { get; set; }
        public string Req_HSI { get; set; }
        public string Req_CustDec { get; set; }
        public string Req_TW_Multiyear { get; set; }
        public string Req_OptimaRestore { get; set; }
        public string Req_Aviation { get; set; }
        public string Req_NE { get; set; }
        public string Req_TravelXDFD { get; set; }
        public string Req_OptimaSenior { get; set; }
        public string Req_Energy { get; set; }
        public string Req_HW { get; set; }
        public string Req_EH { get; set; }
        public string Req_Ican { get; set; }
        public string Req_GetStatus { get; set; }
        public string Request_UploadDocument { get; set; }
        public string Req_PolicyDetails { get; set; }
        public string Req_AMIPA { get; set; }
        public string Req_PolicyStatus { get; set; }
        public string Req_MasterData { get; set; }
        public string Req_ChequeDetails { get; set; }
        public string Req_appstatus { get; set; }
        public string Req_OptimaSuper { get; set; }
        public string PaymentStatusDetails { get; set; }
        public string Req_PospCodeStatus { get; set; }
        public string Req_TvlSportify { get; set; }
        public string Request_Data_OS { get; set; }
        public string Req_GHCIP { get; set; }
        public string Req_PolicyConfirmation { get; set; }
        public string Req_MarineOpen { get; set; }
        public string Req_CyberSachet { get; set; }
    }

    public class Policy_Details
    {
        public string PolicyStartDate { get; set; }
        public string ProposalDate { get; set; }
        public string PolicyEndDate { get; set; }
        public string AgreementType { get; set; }
        public string FinancierCode { get; set; }
        public string BranchName { get; set; }
        public string PreviousPolicy_CorporateCustomerId_Mandatary { get; set; }
        public int PreviousPolicy_NCBPercentage { get; set; }
        public string PreviousPolicy_PolicyEndDate { get; set; }
        public string PreviousPolicy_PolicyStartDate { get; set; }
        public string PreviousPolicy_PolicyNo { get; set; }
        public string PreviousPolicy_PolicyClaim { get; set; }
        public string PreviousPolicy_PreviousPolicyType { get; set; }
        public string PreviousPolicy_TPENDDATE { get; set; }
        public string PreviousPolicy_TPSTARTDATE { get; set; }
        public string PreviousPolicy_TPINSURER { get; set; }
        public string PreviousPolicy_TPPOLICYNO { get; set; }
        public bool PreviousPolicy_IsZeroDept_Cover { get; set; }
        public bool PreviousPolicy_IsRTI_Cover { get; set; }
        public string BusinessType_Mandatary { get; set; }
        public string VehicleModelCode { get; set; }
        public string DateofDeliveryOrRegistration { get; set; }
        public string DateofFirstRegistration { get; set; }
        public string YearOfManufacture { get; set; }
        public string Registration_No { get; set; }
        public string EngineNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string RTOLocationCode { get; set; }
        public double Vehicle_IDV { get; set; }
        public string EndorsementEffectiveDate { get; set; }
        public int SumInsured { get; set; }
        public double Premium { get; set; }
        public string EXEMPTED_KERALA_FLOOD_CESS { get; set; }
        public string CUSTOMER_STATE_CD { get; set; }
        public string TXT_GIR_NO { get; set; }
        public string TSECode { get; set; }
        public string AVCode { get; set; }
        public string SMCode { get; set; }
        public string LGCode { get; set; }
        public string BankLocation { get; set; }
        public string ChannelName { get; set; }
        public string SPCode { get; set; }
        public string BANK_BRANCH_ID { get; set; }
        public string AV_SP_Code { get; set; }
        public string PB_Code { get; set; }
        public string Lead_ID { get; set; }
        public string AutoRenewal { get; set; }
        public string Type_of_payment { get; set; }
        public string PolicyType { get; set; }
        public string FamilyType { get; set; }
        public string TypeofPlan { get; set; }
        public int PolicyTenure { get; set; }
        public double Deductible { get; set; }
    }

    public class Req_TW
    {
        public string POSP_CODE { get; set; }
        public string POLICY_TYPE { get; set; }
        public int POLICY_TENURE { get; set; }
        public double ExtensionCountryCode { get; set; }
        public string ExtensionCountryName { get; set; }
        public bool Effectivedrivinglicense { get; set; }
        public int NumberOfEmployees { get; set; }
        public string BiFuelType { get; set; }
        public double BiFuel_Kit_Value { get; set; }
        public int Paiddriver { get; set; }
        public string Owner_Driver_Nominee_Name { get; set; }
        public int Owner_Driver_Nominee_Age { get; set; }
        public string Owner_Driver_Nominee_Relationship { get; set; }
        public string Owner_Driver_Appointee_Name { get; set; }
        public string Owner_Driver_Appointee_Relationship { get; set; }
        public int IsZeroDept_Cover { get; set; }
        public double ElecticalAccessoryIDV { get; set; }
        public double NonElecticalAccessoryIDV { get; set; }
        public double OtherLoadDiscRate { get; set; }
        public bool AntiTheftDiscFlag { get; set; }
        public bool HandicapDiscFlag { get; set; }
        public int IsNCBProtection_Cover { get; set; }
        public int IsRTI_Cover { get; set; }
        public int IsCOC_Cover { get; set; }
        public int IsEA_Cover { get; set; }
        public int NoofUnnamedPerson { get; set; }
        public double UnnamedPersonSI { get; set; }
        public double Voluntary_Excess_Discount { get; set; }
        public int IsLimitedtoOwnPremises { get; set; }
        public double TPPDLimit { get; set; }
        public int NoofnamedPerson { get; set; }
        public double namedPersonSI { get; set; }
        public string NamedPersons { get; set; }
        public string AutoMobile_Assoication_No { get; set; }
        public int CPA_Tenure { get; set; }
        public string service_type { get; set; }
    }

    public class Req_Pvtcar
    {
        public string POSP_CODE { get; set; }
        public string POLICY_TYPE { get; set; }
        public int POLICY_TENURE { get; set; }
        public double ExtensionCountryCode { get; set; }
        public string ExtensionCountryName { get; set; }
        public string BreakIN_ID { get; set; }
        public string BreakInStatus { get; set; }
        public string BreakInInspectionFlag { get; set; }
        public bool BreakinWaiver { get; set; }
        public string BreakinInspectionDate { get; set; }
        public bool Effectivedrivinglicense { get; set; }
        public int NumberOfEmployees { get; set; }
        public string BiFuelType { get; set; }
        public double BiFuel_Kit_Value { get; set; }
        public int LLPaiddriver { get; set; }
        public double PAPaiddriverSI { get; set; }
        public string Owner_Driver_Nominee_Name { get; set; }
        public int Owner_Driver_Nominee_Age { get; set; }
        public string Owner_Driver_Nominee_Relationship { get; set; }
        public string Owner_Driver_Appointee_Name { get; set; }
        public string Owner_Driver_Appointee_Relationship { get; set; }
        public int IsZeroDept_Cover { get; set; }
        public int IsTyreSecure_Cover { get; set; }
        public double ElecticalAccessoryIDV { get; set; }
        public double NonElecticalAccessoryIDV { get; set; }
        public double OtherLoadDiscRate { get; set; }
        public bool AntiTheftDiscFlag { get; set; }
        public bool HandicapDiscFlag { get; set; }
        public int IsNCBProtection_Cover { get; set; }
        public int IsRTI_Cover { get; set; }
        public int IsCOC_Cover { get; set; }
        public int IsEngGearBox_Cover { get; set; }
        public int IsLossofUseDownTimeProt_Cover { get; set; }
        public int IsEA_Cover { get; set; }
        public int IsEAW_Cover { get; set; }
        public int IsEAAdvance_Cover { get; set; }
        public int IsTowing_Cover { get; set; }
        public string Towing_Limit { get; set; }
        public int IsEMIProtector_Cover { get; set; }
        public string NoOfEmi { get; set; }
        public double EMIAmount { get; set; }
        public int NoofUnnamedPerson { get; set; }
        public double UnnamedPersonSI { get; set; }
        public double Voluntary_Excess_Discount { get; set; }
        public int IsLimitedtoOwnPremises { get; set; }
        public double TPPDLimit { get; set; }
        public int NoofnamedPerson { get; set; }
        public double namedPersonSI { get; set; }
        public string NamedPersons { get; set; }
        public string AutoMobile_Assoication_No { get; set; }
        public string fuel_type { get; set; }
        public int CPA_Tenure { get; set; }
        public bool PayAsYouDrive { get; set; }
        public int InitialOdometerReading { get; set; }
        public string InitialOdometerReadingDate { get; set; }
    }

    public class Customer_Details
    {
        public string GC_CustomerID { get; set; }
        public string IsCustomer_modify { get; set; }
        public string Company_Name { get; set; }
        public string Customer_Type { get; set; }
        public string Customer_FirstName { get; set; }
        public string Customer_MiddleName { get; set; }
        public string Customer_LastName { get; set; }
        public string Customer_DateofBirth { get; set; }
        public string Customer_Email { get; set; }
        public string Customer_Mobile { get; set; }
        public string Customer_Telephone { get; set; }
        public string Customer_PanNo { get; set; }
        public string Customer_Salutation { get; set; }
        public string Customer_Gender { get; set; }
        public string Customer_Perm_Address1 { get; set; }
        public string Customer_Perm_Address2 { get; set; }
        public string Customer_Perm_Apartment { get; set; }
        public string Customer_Perm_Street { get; set; }
        public string Customer_Perm_CityDistrictCode { get; set; }
        public string Customer_Perm_CityDistrict { get; set; }
        public string Customer_Perm_StateCode { get; set; }
        public string Customer_Perm_State { get; set; }
        public string Customer_Perm_PinCode { get; set; }
        public string Customer_Perm_PinCodeLocality { get; set; }
        public string Customer_Mailing_Address1 { get; set; }
        public string Customer_Mailing_Address2 { get; set; }
        public string Customer_Mailing_Apartment { get; set; }
        public string Customer_Mailing_Street { get; set; }
        public string Customer_Mailing_CityDistrictCode { get; set; }
        public string Customer_Mailing_CityDistrict { get; set; }
        public string Customer_Mailing_StateCode { get; set; }
        public string Customer_Mailing_State { get; set; }
        public string Customer_Mailing_PinCode { get; set; }
        public string Customer_Mailing_PinCodeLocality { get; set; }
        public string Customer_GSTIN_Number { get; set; }
        public string Customer_GSTIN_State { get; set; }
        public string Customer_Professtion { get; set; }
        public string Customer_MaritalStatus { get; set; }
        public string Customer_EIA_Number { get; set; }
        public string Customer_IDProof { get; set; }
        public string Customer_IDProofNo { get; set; }
        public string Customer_Nationality { get; set; }
        public string Customer_UniqueRefNo { get; set; }
        public string Customer_GSTDetails { get; set; }
        public string Customer_Pehchaan_id { get; set; }
    }

    public class Req_PCV
    {
        public string POSP_CODE { get; set; }
        public string BreakIN_ID { get; set; }
        public string BreakInStatus { get; set; }
        public string BreakinInspectionDate { get; set; }
        public double ExtensionCountryCode { get; set; }
        public string ExtensionCountryName { get; set; }
        public bool Effectivedrivinglicense { get; set; }
        public int NumberOfDrivers { get; set; }
        public int NumberOfEmployees { get; set; }
        public int NoOfCleanerConductorCoolies { get; set; }
        public string BiFuelType { get; set; }
        public double BiFuel_Kit_Value { get; set; }
        public double Paiddriver_Si { get; set; }
        public string Owner_Driver_Nominee_Name { get; set; }
        public int Owner_Driver_Nominee_Age { get; set; }
        public string Owner_Driver_Nominee_Relationship { get; set; }
        public string Owner_Driver_Appointee_Name { get; set; }
        public string Owner_Driver_Appointee_Relationship { get; set; }
        public int IsZeroDept_Cover { get; set; }
        public double ElecticalAccessoryIDV { get; set; }
        public double NonElecticalAccessoryIDV { get; set; }
        public int IsLimitedtoOwnPremises { get; set; }
        public int IsPrivateUseLoading { get; set; }
        public int IsInclusionofIMT23 { get; set; }
        public double OtherLoadDiscRate { get; set; }
        public string Bus_Type { get; set; }
        public int NoOfFPP { get; set; }
        public int NoOfNFPP { get; set; }
        public int IsCOC_Cover { get; set; }
        public int IsTowing_Cover { get; set; }
        public string Towing_Limit { get; set; }
        public int IsEngGearBox_Cover { get; set; }
        public int IsNCBProtection_Cover { get; set; }
        public int IsRTI_Cover { get; set; }
        public int IsEA_Cover { get; set; }
        public int IsEAW_Cover { get; set; }
        public string NoOfEmi { get; set; }
        public double EMIAmount { get; set; }
        public int Is_SchoolBus { get; set; }
        public double TPPDLimit { get; set; }
        public bool AntiTheftDiscFlag { get; set; }
    }
    public class Req_GCV
    {
        public string POSP_CODE { get; set; }
        public string BreakIN_ID { get; set; }
        public string BreakInStatus { get; set; }
        public string BreakinInspectionDate { get; set; }
        public double ExtensionCountryCode { get; set; }
        public string ExtensionCountryName { get; set; }
        public bool Effectivedrivinglicense { get; set; }
        public int NumberOfDrivers { get; set; }
        public int NumberOfEmployees { get; set; }
        public int NoOfCleanerConductorCoolies { get; set; }
        public string fuel_type { get; set; }
        public string BiFuelType { get; set; }
        public double BiFuel_Kit_Value { get; set; }
        public double Paiddriver_Si { get; set; }
        public string Owner_Driver_Nominee_Name { get; set; }
        public int Owner_Driver_Nominee_Age { get; set; }
        public string Owner_Driver_Nominee_Relationship { get; set; }
        public string Owner_Driver_Appointee_Name { get; set; }
        public string Owner_Driver_Appointee_Relationship { get; set; }
        public int IsZeroDept_Cover { get; set; }
        public int NoOfTrailers { get; set; }
        public string TrailerChassisNo { get; set; }
        public double TrailerIDV { get; set; }
        public double ElecticalAccessoryIDV { get; set; }
        public double NonElecticalAccessoryIDV { get; set; }
        public int IsLimitedtoOwnPremises { get; set; }
        public int IsPrivateUseLoading { get; set; }
        public int IsInclusionofIMT23 { get; set; }
        public int IsEngGearBox_Cover { get; set; }
        public int IsNCBProtection_Cover { get; set; }
        public double OtherLoadDiscRate { get; set; }
        public bool PrivateCarrier { get; set; }
        public int NoOfFPP { get; set; }
        public int NoOfNFPP { get; set; }
        public int ISHired_vehicles { get; set; }
        public int IsFibertank { get; set; }
        public int NoOfWorkmen { get; set; }
        public bool AntiTheftDiscFlag { get; set; }
        public double TPPDLimit { get; set; }
    }
    public class Req_MISD
    {
        public string POSP_CODE { get; set; }
        public string BreakIN_ID { get; set; }
        public string BreakInStatus { get; set; }
        public string BreakinInspectionDate { get; set; }
        public double ExtensionCountryCode { get; set; }
        public string ExtensionCountryName { get; set; }
        public bool Effectivedrivinglicense { get; set; }
        public int NumberOfDrivers { get; set; }
        public int NumberOfEmployees { get; set; }
        public int NoOfCleanerConductorCoolies { get; set; }
        public string BiFuelType { get; set; }
        public double BiFuel_Kit_Value { get; set; }
        public double Paiddriver_Si { get; set; }
        public string Owner_Driver_Nominee_Name { get; set; }
        public int Owner_Driver_Nominee_Age { get; set; }
        public string Owner_Driver_Nominee_Relationship { get; set; }
        public string Owner_Driver_Appointee_Name { get; set; }
        public string Owner_Driver_Appointee_Relationship { get; set; }
        public int IsZeroDept_Cover { get; set; }
        public int NoOfTrailers { get; set; }
        public string TrailerChassisNo { get; set; }
        public double TrailerIDV { get; set; }
        public double ElecticalAccessoryIDV { get; set; }
        public double NonElecticalAccessoryIDV { get; set; }
        public int IsLimitedtoOwnPremises { get; set; }
        public int IsPrivateUseLoading { get; set; }
        public int IsInclusionofIMT23 { get; set; }
        public int IsOverTurningLoading { get; set; }
        public double OtherLoadDiscRate { get; set; }
        public double TPPDLimit { get; set; }
        public bool AntiTheftDiscFlag { get; set; }
    }

    public class Payment_Details
    {
        public string GC_PaymentID { get; set; }
        public string BANK_NAME { get; set; }
        public string BANK_BRANCH_NAME { get; set; }
        public string PAYMENT_MODE_CD { get; set; }
        public string PAYER_TYPE { get; set; }
        public string PAYMENT_AMOUNT { get; set; }
        public string INSTRUMENT_NUMBER { get; set; }
        public string PAYMENT_DATE { get; set; }
    }
}
