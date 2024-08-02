using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Insurance.Domain.Reliance
{

    public class RelianceServiceRequestModel
    {
        public PolicyDetails PolicyDetails { get; set; }
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
        public string Req_GCV { get; set; }
        public string Req_MISD { get; set; }
        public string Req_PCV { get; set; }
        public string Payment_Details { get; set; }
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

    //Proposal Request Framing Model Start
    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(PolicyDetails));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (PolicyDetails)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "CommunicationAddress")]
    public class CommunicationAddress
    {

        [XmlElement(ElementName = "AddressType")]
        public int AddressType { get; set; }

        [XmlElement(ElementName = "Address1")]
        public string Address1 { get; set; }

        [XmlElement(ElementName = "Address2")]
        public string Address2 { get; set; }

        [XmlElement(ElementName = "Address3")]
        public string Address3 { get; set; }

        [XmlElement(ElementName = "CityID")]
        public int CityID { get; set; }

        [XmlElement(ElementName = "DistrictID")]
        public int DistrictID { get; set; }

        [XmlElement(ElementName = "StateID")]
        public int StateID { get; set; }

        [XmlElement(ElementName = "Pincode")]
        public int Pincode { get; set; }

        [XmlElement(ElementName = "Country")]
        public string Country { get; set; }

        [XmlElement(ElementName = "NearestLandmark")]
        public string NearestLandmark { get; set; }
    }

    [XmlRoot(ElementName = "PermanentAddress")]
    public class PermanentAddress
    {

        [XmlElement(ElementName = "AddressType")]
        public int AddressType { get; set; }

        [XmlElement(ElementName = "Address1")]
        public string Address1 { get; set; }

        [XmlElement(ElementName = "Address2")]
        public string Address2 { get; set; }

        [XmlElement(ElementName = "Address3")]
        public string Address3 { get; set; }

        [XmlElement(ElementName = "CityID")]
        public int CityID { get; set; }

        [XmlElement(ElementName = "DistrictID")]
        public int DistrictID { get; set; }

        [XmlElement(ElementName = "StateID")]
        public int StateID { get; set; }

        [XmlElement(ElementName = "Pincode")]
        public int Pincode { get; set; }

        [XmlElement(ElementName = "Country")]
        public string Country { get; set; }

        [XmlElement(ElementName = "NearestLandmark")]
        public string NearestLandmark { get; set; }
    }

    [XmlRoot(ElementName = "RegistrationAddress")]
    public class RegistrationAddress
    {

        [XmlElement(ElementName = "AddressType")]
        public int AddressType { get; set; }

        [XmlElement(ElementName = "Address1")]
        public string Address1 { get; set; }

        [XmlElement(ElementName = "Address2")]
        public string Address2 { get; set; }

        [XmlElement(ElementName = "Address3")]
        public string Address3 { get; set; }

        [XmlElement(ElementName = "CityID")]
        public int CityID { get; set; }

        [XmlElement(ElementName = "DistrictID")]
        public int DistrictID { get; set; }

        [XmlElement(ElementName = "StateID")]
        public int StateID { get; set; }

        [XmlElement(ElementName = "Pincode")]
        public int Pincode { get; set; }

        [XmlElement(ElementName = "Country")]
        public string Country { get; set; }

        [XmlElement(ElementName = "NearestLandmark")]
        public string NearestLandmark { get; set; }
    }

    [XmlRoot(ElementName = "ClientAddress")]
    public class ClientAddress
    {

        [XmlElement(ElementName = "CommunicationAddress")]
        public CommunicationAddress CommunicationAddress { get; set; }

        [XmlElement(ElementName = "PermanentAddress")]
        public PermanentAddress PermanentAddress { get; set; }

        [XmlElement(ElementName = "RegistrationAddress")]
        public RegistrationAddress RegistrationAddress { get; set; }
    }

    [XmlRoot(ElementName = "ClientDetails")]
    public class ClientDetails
    {

        [XmlElement(ElementName = "ClientType")]
        public int ClientType { get; set; }

        [XmlElement(ElementName = "LastName")]
        public string LastName { get; set; }

        [XmlElement(ElementName = "MidName")]
        public string MidName { get; set; }

        [XmlElement(ElementName = "ForeName")]
        public string ForeName { get; set; }

        [XmlElement(ElementName = "CorporateName")]
        public string CorporateName { get; set; }

        [XmlElement(ElementName = "OccupationID")]
        public string OccupationID { get; set; }

        [XmlElement(ElementName = "DOB")]
        public string DOB { get; set; }

        [XmlElement(ElementName = "Gender")]
        public string Gender { get; set; }

        [XmlElement(ElementName = "PhoneNo")]
        public string PhoneNo { get; set; }

        [XmlElement(ElementName = "MobileNo")]
        public string MobileNo { get; set; }

        [XmlElement(ElementName = "GSTIN")]
        public string GSTIN { get; set; }

        [XmlElement(ElementName = "ClientAddress")]
        public ClientAddress ClientAddress { get; set; }

        [XmlElement(ElementName = "EmailID")]
        public string EmailID { get; set; }

        [XmlElement(ElementName = "Salutation")]
        public string Salutation { get; set; }

        [XmlElement(ElementName = "MaritalStatus")]
        public int MaritalStatus { get; set; }

        [XmlElement(ElementName = "Nationality")]
        public int Nationality { get; set; }
    }

    [XmlRoot(ElementName = "Policy")]
    public class Policy
    {

        [XmlElement(ElementName = "BusinessType")]
        public int BusinessType { get; set; }

        [XmlElement(ElementName = "POSType")]
        public string POSType { get; set; }

        [XmlElement(ElementName = "POSAadhaarNumber")]
        public string POSAadhaarNumber { get; set; }

        [XmlElement(ElementName = "POSPANNumber")]
        public string POSPANNumber { get; set; }

        [XmlElement(ElementName = "Cover_From")]
        public string CoverFrom { get; set; }

        [XmlElement(ElementName = "Branch_Code")]
        public string BranchCode { get; set; }

        [XmlElement(ElementName = "AgentName")]
        public string AgentName { get; set; }

        [XmlElement(ElementName = "productcode")]
        public string Productcode { get; set; }

        [XmlElement(ElementName = "OtherSystemName")]
        public int OtherSystemName { get; set; }

        [XmlElement(ElementName = "isMotorQuote")]
        public bool IsMotorQuote { get; set; }

        [XmlElement(ElementName = "isMotorQuoteFlow")]
        public bool IsMotorQuoteFlow { get; set; }
        public string TPPolicyNumber { get; set; }
        public string TPPolicyStartDate { get; set; }
        public string TPPolicyEndDate { get; set; }
        public string TPPolicyInsurer { get; set; }

        [XmlElement(ElementName = "Cover_To")]
        public string CoverTo { get; set; }
    }

    [XmlRoot(ElementName = "Risk")]
    public class Risk
    {

        [XmlElement(ElementName = "VehicleMakeID")]
        public int VehicleMakeID { get; set; }

        [XmlElement(ElementName = "VehicleModelID")]
        public int VehicleModelID { get; set; }

        [XmlElement(ElementName = "CubicCapacity")]
        public int CubicCapacity { get; set; }

        [XmlElement(ElementName = "Zone")]
        public string Zone { get; set; }

        [XmlElement(ElementName = "RTOLocationID")]
        public int RTOLocationID { get; set; }

        [XmlElement(ElementName = "ExShowroomPrice")]
        public int ExShowroomPrice { get; set; }

        [XmlElement(ElementName = "IDV")]
        public string IDV { get; set; }

        [XmlElement(ElementName = "DateOfPurchase")]
        public string DateOfPurchase { get; set; }

        [XmlElement(ElementName = "ManufactureMonth")]
        public string ManufactureMonth { get; set; }

        [XmlElement(ElementName = "ManufactureYear")]
        public int ManufactureYear { get; set; }

        [XmlElement(ElementName = "EngineNo")]
        public string EngineNo { get; set; }

        [XmlElement(ElementName = "Chassis")]
        public string Chassis { get; set; }

        [XmlElement(ElementName = "IsVehicleHypothicated")]
        public bool IsVehicleHypothicated { get; set; }

        [XmlElement(ElementName = "FinanceType")]
        public string FinanceType { get; set; }

        [XmlElement(ElementName = "FinancierName")]
        public string FinancierName { get; set; }

        [XmlElement(ElementName = "FinancierAddress")]
        public string FinancierAddress { get; set; }

        [XmlElement(ElementName = "FinancierCity")]
        public string FinancierCity { get; set; }

        [XmlElement(ElementName = "IsRegAddressSameasCommAddress")]
        public bool IsRegAddressSameasCommAddress { get; set; }

        [XmlElement(ElementName = "IsPermanentAddressSameasCommAddress")]
        public bool IsPermanentAddressSameasCommAddress { get; set; }

        [XmlElement(ElementName = "IsRegAddressSameasPermanentAddress")]
        public bool IsRegAddressSameasPermanentAddress { get; set; }

        [XmlElement(ElementName = "VehicleVariant")]
        public string VehicleVariant { get; set; }

        [XmlElement(ElementName = "StateOfRegistrationID")]
        public int StateOfRegistrationID { get; set; }

        [XmlElement(ElementName = "Rto_RegionCode")]
        public string RtoRegionCode { get; set; }
    }

    [XmlRoot(ElementName = "Vehicle")]
    public class Vehicle
    {

        [XmlElement(ElementName = "Registration_Number")]
        public string RegistrationNumber { get; set; }

        [XmlElement(ElementName = "ISNewVehicle")]
        public bool ISNewVehicle { get; set; }

        [XmlElement(ElementName = "Registration_date")]
        public string RegistrationDate { get; set; }

        [XmlElement(ElementName = "SeatingCapacity")]
        public int SeatingCapacity { get; set; }

        [XmlElement(ElementName = "TypeOfFuel")]
        public int TypeOfFuel { get; set; }

        [XmlElement(ElementName = "BodyIDV")]
        public int BodyIDV { get; set; }

        [XmlElement(ElementName = "ChassisIDV")]
        public int ChassisIDV { get; set; }

        [XmlElement(ElementName = "BodyPrice")]
        public long BodyPrice { get; set; }

        [XmlElement(ElementName = "ChassisPrice")]
        public long ChassisPrice { get; set; }
        [XmlElement(ElementName = "ISmanufacturerfullybuild")]
        public bool ISmanufacturerfullybuild { get; set; } = true;
    }

    [XmlRoot(ElementName = "PAToUnNamedPassenger")]
    public class PAToUnNamedPassengers
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public int NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "PolicyCoverID")]
        public string PolicyCoverID { get; set; }

        [XmlElement(ElementName = "SumInsured")]
        public int SumInsured { get; set; }

        [XmlElement(ElementName = "PAToUnNamedPassenger")]
        public PAToUnNamedPassengers PAToUnNamedPassenger { get; set; }
    }

    [XmlRoot(ElementName = "PACoverToOwner")]
    public class PACoverToOwners
    {

        [XmlElement(ElementName = "CPAcovertenure")]
        public int CPAcovertenure { get; set; }

        [XmlElement(ElementName = "NomineeName")]
        public string NomineeName { get; set; }

        [XmlElement(ElementName = "NomineeDOB")]
        public string NomineeDOB { get; set; }

        [XmlElement(ElementName = "NomineeRelationship")]
        public string NomineeRelationship { get; set; }

        [XmlElement(ElementName = "PACoverToOwner")]
        public PACoverToOwners PACoverToOwner { get; set; }
    }

    [XmlRoot(ElementName = "LiabilityToPaidDriver")]
    public class LiabilityToPaidDrivers
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public int NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "PolicyCoverID")]
        public string PolicyCoverID { get; set; }

        [XmlElement(ElementName = "LiabilityToPaidDriver")]
        public LiabilityToPaidDrivers LiabilityToPaidDriver { get; set; }
    }

    [XmlRoot(ElementName = "BifuelKit")]
    public class BifuelKits
    {

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "ISLpgCng")]
        public bool ISLpgCng { get; set; }

        [XmlElement(ElementName = "SumInsured")]
        public int SumInsured { get; set; }

        [XmlElement(ElementName = "Fueltype")]
        public string Fueltype { get; set; }

        [XmlElement(ElementName = "BifuelKit")]
        public BifuelKits BifuelKit { get; set; }
    }

    [XmlRoot(ElementName = "AntiTheftDeviceDiscount")]
    public class AntiTheftDeviceDiscounts
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public string NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "AntiTheftDeviceDiscount")]
        public AntiTheftDeviceDiscounts AntiTheftDeviceDiscount { get; set; }
    }

    [XmlRoot(ElementName = "AutomobileAssociationMembershipDiscount")]
    public class AutomobileAssociationMembershipDiscounts
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public string NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "AutomobileAssociationMembershipDiscount")]
        public AutomobileAssociationMembershipDiscounts AutomobileAssociationMembershipDiscount { get; set; }
    }

    [XmlRoot(ElementName = "VoluntaryDeductible")]
    public class VoluntaryDeductibles
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "PolicyCoverID")]
        public string PolicyCoverID { get; set; }

        [XmlElement(ElementName = "SumInsured")]
        public int SumInsured { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public string NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "VoluntaryDeductible")]
        public VoluntaryDeductibles VoluntaryDeductible { get; set; }
    }

    [XmlRoot(ElementName = "ElectricalItems")]
    public class ElectricalItems
    {

        [XmlElement(ElementName = "ElectricalItemsID")]
        public string ElectricalItemsID { get; set; }

        [XmlElement(ElementName = "PolicyId")]
        public string PolicyId { get; set; }

        [XmlElement(ElementName = "SerialNo")]
        public string SerialNo { get; set; }

        [XmlElement(ElementName = "MakeModel")]
        public string MakeModel { get; set; }

        [XmlElement(ElementName = "ElectricPremium")]
        public string ElectricPremium { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "ElectricalAccessorySlNo")]
        public string ElectricalAccessorySlNo { get; set; }

        [XmlElement(ElementName = "SumInsured")]
        public int SumInsured { get; set; }
    }

    [XmlRoot(ElementName = "ElectricItems")]
    public class ElectricItems
    {

        [XmlElement(ElementName = "ElectricalItems")]
        public ElectricalItems ElectricalItems { get; set; }
    }

    [XmlRoot(ElementName = "NonElectricalItems")]
    public class NonElectricalItems
    {

        [XmlElement(ElementName = "NonElectricalItemsID")]
        public string NonElectricalItemsID { get; set; }

        [XmlElement(ElementName = "PolicyID")]
        public string PolicyID { get; set; }

        [XmlElement(ElementName = "SerialNo")]
        public string SerialNo { get; set; }

        [XmlElement(ElementName = "MakeModel")]
        public string MakeModel { get; set; }

        [XmlElement(ElementName = "NonElectricPremium")]
        public string NonElectricPremium { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "Category")]
        public string Category { get; set; }

        [XmlElement(ElementName = "NonElectricalAccessorySlNo")]
        public string NonElectricalAccessorySlNo { get; set; }

        [XmlElement(ElementName = "SumInsured")]
        public string SumInsured { get; set; }
    }

    [XmlRoot(ElementName = "NonElectricItems")]
    public class NonElectricItems
    {

        [XmlElement(ElementName = "NonElectricalItems")]
        public NonElectricalItems NonElectricalItems { get; set; }
    }

    [XmlRoot(ElementName = "NilDepreciationCoverage")]
    public class NilDepreciationCoverages
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public string NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "PolicyCoverID")]
        public string PolicyCoverID { get; set; }

        [XmlElement(ElementName = "ApplicableRate")]
        public double ApplicableRate { get; set; }

        [XmlElement(ElementName = "NilDepreciationCoverage")]
        public NilDepreciationCoverages NilDepreciationCoverage { get; set; }
    }

    [XmlRoot(ElementName = "TotalCover")]
    public class TotalCovers
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public string NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "TotalCover")]
        public TotalCovers TotalCover { get; set; }
    }

    [XmlRoot(ElementName = "RegistrationCost")]
    public class RegistrationCosts
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public string NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "SumInsured")]
        public string SumInsured { get; set; }

        [XmlElement(ElementName = "RegistrationCost")]
        public RegistrationCosts RegistrationCost { get; set; }
    }

    [XmlRoot(ElementName = "RoadTax")]
    public class RoadTaxs
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public string NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "SumInsured")]
        public string SumInsured { get; set; }

        [XmlElement(ElementName = "PolicyCoverID")]
        public string PolicyCoverID { get; set; }

        [XmlElement(ElementName = "RoadTax")]
        public RoadTaxs RoadTax { get; set; }
    }

    [XmlRoot(ElementName = "InsurancePremium")]
    public class InsurancePremiums
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public string NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "SumInsured")]
        public string SumInsured { get; set; }

        [XmlElement(ElementName = "InsurancePremium")]
        public InsurancePremiums InsurancePremium { get; set; }
    }

    [XmlRoot(ElementName = "SecurePlus")]
    public class SecurePluss
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public string NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "PolicyCoverID")]
        public string PolicyCoverID { get; set; }

        [XmlElement(ElementName = "ApplicableRate")]
        public string ApplicableRate { get; set; }

        [XmlElement(ElementName = "SecurePlus")]
        public SecurePluss SecurePlus { get; set; }
    }

    [XmlRoot(ElementName = "SecurePremium")]
    public class SecurePremiums
    {

        [XmlElement(ElementName = "IsMandatory")]
        public bool IsMandatory { get; set; }

        [XmlElement(ElementName = "IsChecked")]
        public bool IsChecked { get; set; }

        [XmlElement(ElementName = "NoOfItems")]
        public string NoOfItems { get; set; }

        [XmlElement(ElementName = "PackageName")]
        public string PackageName { get; set; }

        [XmlElement(ElementName = "PolicyCoverID")]
        public string PolicyCoverID { get; set; }

        [XmlElement(ElementName = "ApplicableRate")]
        public string ApplicableRate { get; set; }

        [XmlElement(ElementName = "SecurePremium")]
        public SecurePremiums SecurePremium { get; set; }
    }

    [XmlRoot(ElementName = "Cover")]
    public class Cover
    {

        [XmlElement(ElementName = "IsNilDepreciation")]
        public bool IsNilDepreciation { get; set; }

        [XmlElement(ElementName = "IsTotalCover")]
        public bool IsTotalCover { get; set; }

        [XmlElement(ElementName = "IsRegistrationCover")]
        public bool IsRegistrationCover { get; set; }

        [XmlElement(ElementName = "IsRoadTaxcover")]
        public bool IsRoadTaxcover { get; set; }

        [XmlElement(ElementName = "IsInsurancePremium")]
        public bool IsInsurancePremium { get; set; }

        [XmlElement(ElementName = "IsPAToUnnamedPassengerCovered")]
        public bool IsPAToUnnamedPassengerCovered { get; set; }

        [XmlElement(ElementName = "NoOfUnnamedPassenegersCovered")]
        public int NoOfUnnamedPassenegersCovered { get; set; }

        [XmlElement(ElementName = "UnnamedPassengersSI")]
        public int UnnamedPassengersSI { get; set; }

        [XmlElement(ElementName = "PAToUnNamedPassenger")]
        public PAToUnNamedPassengers PAToUnNamedPassenger { get; set; }

        [XmlElement(ElementName = "IsPAToOwnerDriverCoverd")]
        public bool IsPAToOwnerDriverCoverd { get; set; }

        [XmlElement(ElementName = "PACoverToOwner")]
        public PACoverToOwners PACoverToOwner { get; set; }

        [XmlElement(ElementName = "IsLiabilityToPaidDriverCovered")]
        public bool IsLiabilityToPaidDriverCovered { get; set; }

        [XmlElement(ElementName = "LiabilityToPaidDriver")]
        public LiabilityToPaidDrivers LiabilityToPaidDriver { get; set; }

        [XmlElement(ElementName = "IsSecurePlus")]
        public bool IsSecurePlus { get; set; }

        [XmlElement(ElementName = "IsSecurePremium")]
        public bool IsSecurePremium { get; set; }

        [XmlElement(ElementName = "IsAntiTheftDeviceFitted")]
        public bool IsAntiTheftDeviceFitted { get; set; }

        [XmlElement(ElementName = "IsAutomobileAssociationMember")]
        public bool IsAutomobileAssociationMember { get; set; }

        [XmlElement(ElementName = "IsVoluntaryDeductableOpted")]
        public bool IsVoluntaryDeductableOpted { get; set; }

        [XmlElement(ElementName = "VoluntaryDeductableAmount")]
        public int VoluntaryDeductableAmount { get; set; }

        [XmlElement(ElementName = "IsElectricalItemFitted")]
        public bool IsElectricalItemFitted { get; set; }

        [XmlElement(ElementName = "ElectricalItemsTotalSI")]
        public int ElectricalItemsTotalSI { get; set; }

        [XmlElement(ElementName = "IsNonElectricalItemFitted")]
        public bool IsNonElectricalItemFitted { get; set; }

        [XmlElement(ElementName = "NonElectricalItemsTotalSI")]
        public int NonElectricalItemsTotalSI { get; set; }

        [XmlElement(ElementName = "IsBiFuelKit")]
        public bool IsBiFuelKit { get; set; }

        [XmlElement(ElementName = "BiFuelKitSi")]
        public int BiFuelKitSi { get; set; }

        [XmlElement(ElementName = "IsTPPDCover")]
        public bool IsTPPDCover { get; set; }

        [XmlElement(ElementName = "IsBasicODCoverage")]
        public bool IsBasicODCoverage { get; set; }

        [XmlElement(ElementName = "IsBasicLiability")]
        public bool IsBasicLiability { get; set; }

        [XmlElement(ElementName = "BifuelKit")]
        public BifuelKits BifuelKit { get; set; }

        [XmlElement(ElementName = "AntiTheftDeviceDiscount")]
        public AntiTheftDeviceDiscount AntiTheftDeviceDiscount { get; set; }

        [XmlElement(ElementName = "AutomobileAssociationMembershipDiscount")]
        public AutomobileAssociationMembershipDiscount AutomobileAssociationMembershipDiscount { get; set; }

        [XmlElement(ElementName = "VoluntaryDeductible")]
        public VoluntaryDeductibles VoluntaryDeductible { get; set; }

        [XmlElement(ElementName = "ElectricItems")]
        public ElectricItems ElectricItems { get; set; }

        [XmlElement(ElementName = "NonElectricItems")]
        public NonElectricItems NonElectricItems { get; set; }

        [XmlElement(ElementName = "NilDepreciationCoverage")]
        public NilDepreciationCoverages NilDepreciationCoverage { get; set; }

        [XmlElement(ElementName = "TotalCover")]
        public TotalCover TotalCover { get; set; }

        [XmlElement(ElementName = "RegistrationCost")]
        public RegistrationCost RegistrationCost { get; set; }

        [XmlElement(ElementName = "RoadTax")]
        public RoadTax RoadTax { get; set; }

        [XmlElement(ElementName = "InsurancePremium")]
        public InsurancePremium InsurancePremium { get; set; }

        [XmlElement(ElementName = "SecurePlus")]
        public SecurePluss SecurePlus { get; set; }

        [XmlElement(ElementName = "SecurePremium")]
        public SecurePremiums SecurePremium { get; set; }

        [XmlElement(ElementName = "IsGeographicalAreaExtended")]
        public bool IsGeographicalAreaExtended { get; set; }
    }

    [XmlRoot(ElementName = "PreviousInsuranceDetails")]
    public class PreviousInsuranceDetails
    {

        [XmlElement(ElementName = "PrevYearInsurer")]
        public string PrevYearInsurer { get; set; }

        [XmlElement(ElementName = "PrevYearPolicyNo")]
        public string PrevYearPolicyNo { get; set; }

        [XmlElement(ElementName = "PrevYearPolicyStartDate")]
        public string PrevYearPolicyStartDate { get; set; }

        [XmlElement(ElementName = "PrevYearPolicyEndDate")]
        public string PrevYearPolicyEndDate { get; set; }

        [XmlElement(ElementName = "IsClaimedLastYear")]
        public string IsClaimedLastYear { get; set; }

        [XmlElement(ElementName = "IsPreviousPolicyDetailsAvailable")]
        public string IsPreviousPolicyDetailsAvailable { get; set; }
    }

    [XmlRoot(ElementName = "NCBEligibility")]
    public class NCBEligibility
    {

        [XmlElement(ElementName = "IsNCBApplicable")]
        public bool IsNCBApplicable { get; set; }

        [XmlElement(ElementName = "NCBEligibilityCriteria")]
        public int NCBEligibilityCriteria { get; set; }

        [XmlElement(ElementName = "PreviousNCB")]
        public int PreviousNCB { get; set; }

        [XmlElement(ElementName = "CurrentNCB")]
        public int CurrentNCB { get; set; }
    }

    [XmlRoot(ElementName = "PolicyDetails")]
    public class PolicyDetails
    {

        [XmlElement(ElementName = "CoverDetails")]
        public string CoverDetails { get; set; }

        [XmlElement(ElementName = "TrailerDetails")]
        public string TrailerDetails { get; set; }

        [XmlElement(ElementName = "ClientDetails")]
        public ClientDetails ClientDetails { get; set; }

        [XmlElement(ElementName = "Policy")]
        public Policy Policy { get; set; }

        [XmlElement(ElementName = "Risk")]
        public Risk Risk { get; set; }

        [XmlElement(ElementName = "Vehicle")]
        public Vehicle Vehicle { get; set; }

        [XmlElement(ElementName = "Cover")]
        public Cover Cover { get; set; }

        [XmlElement(ElementName = "PreviousInsuranceDetails")]
        public PreviousInsuranceDetails PreviousInsuranceDetails { get; set; }

        [XmlElement(ElementName = "NCBEligibility")]
        public NCBEligibility NCBEligibility { get; set; }

        [XmlElement(ElementName = "UserID")]
        public string UserID { get; set; }

        [XmlElement(ElementName = "ProductCode")]
        public string ProductCode { get; set; }

        [XmlElement(ElementName = "SourceSystemID")]
        public string SourceSystemID { get; set; }

        [XmlElement(ElementName = "AuthToken")]
        public string AuthToken { get; set; }

        [XmlAttribute(AttributeName = "xsd")]
        public string Xsd { get; set; }

        [XmlAttribute(AttributeName = "xsi")]
        public string Xsi { get; set; }

        [XmlText]
        public string Text { get; set; }
    }



    //Proposal Request Framing Model End
}
