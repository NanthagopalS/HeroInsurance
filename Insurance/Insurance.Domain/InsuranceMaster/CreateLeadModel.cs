namespace Insurance.Domain.InsuranceMaster;

public class CreateLeadModel
{
    public string VehicleTypeId { get; set; }
    public string VehicleNumber { get; set; }
    public string VariantId { get; set; }
    public string YearId { get; set; }
    public string LeadName { get; set; } //
    public string PhoneNumber { get; set; }  //
    public string Email { get; set; }   //
    public string Gender { get; set; } //
    public string QuoteTransactionID { get; set; }
    public string MakeMonthYear { get; set; }
    public string RegistrationDate { get; set; }
    public string CarOwnedBy { get; set; }
    public bool IsPrevPolicy { get; set; }
    public string PrevPolicyNumber { get; set; }
    public string PreviousSATPPolicyStartDate { get; set; }
    public string PrevPolicyExpiryDate { get; set; }
    public string PreviousSATPInsurer { get; set; }
    public string PreviousSAODPolicyStartDate { get; set; }
    public string PreviousPolicyExpirySAOD { get; set; }
    public string PreviousPolicyNumberSAOD { get; set; }
    public string PreviousSAODInsurer { get; set; }
    public string PrevPolicyClaims { get; set; }
    public string PrevPolictyNCB { get; set; }
    public string PrevPolicyTypeId { get; set; }
    public string PolicyTypeId { get; set; }
    public bool IsPACover { get; set; }
    public int Tenure { get; set; }
    public string EngineNumber { get; set; }
    public string ChassisNumber { get; set; }
    public string VehicleCubicCapacity { get; set; }
    public string LoadProvidedCompany { get; set; }
    public string LoadProvidedCity { get; set; }
    public bool IsOwnershipChangeIn12Months { get; }
    public bool IsExternalCNGKit { get; }
    public bool IsCarOnLoan { get; set; }
    public string kyc_id { get; set; }
    public string redirect_link { get; set; }
    public string ckycNumber { get; set; }
    public string CompanyName { get; set; }
    public string DateOfIncorporation { get; set; }
    public string GSTNo { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string DOB { get; set; }
    public string PANNumber { get; set; }
    public string AadharNumber { get; set; }
    public string DrivingLicenceNumber { get; set; }
    public string VoterId { get; set; }
    public string PassportNo { get; set; }
    public string CIN { get; set; }
    public string Profession { get; set; }
    public string MaritalStatus { get; set; }
    public string LeadID { get; set; }
    public string RefLeadId { get; set; }
    public LeadAddressModel CommunicationAddress { get; set; }
    public LeadAddressModel PermanentAddress { get; set; }
    public LeadNomineeModel Nominees { get; set; }
    public string ProposalRequestBody { get; set; }
    public string CKYCstatus { get; set; }
    public bool IsBreakin { get; set; }
    public bool IsBreakinApproved { get; set; }
    public bool IsBrandNew { get; set; }
    public bool IsSelfInspection { get; set; }
    public string PolicyNumber { get; set; }
    public string PaymentLink { get; set; }
    public string Stage { get; set; }
    public string BreakinId { get; set; }
    public bool BreakInStatus { get; set; }
    public string GrossPremium { get; set; }
    public string InsurerId { get; set; }
    public string RTOId { get; set; }
    public string BreakinInspectionDate { get; set; }
    public string InspectionAgency { get; set; }
    public string MakeName { get; set; }
    public string ModelName { get; set; }
    public string RTOZone { get; set; }
    public string FinancierCode { get; set; }
    public string preInsurer { get; set; }
    public string ResponseReferanceFlag { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Salutation { get; set; }
    public string TransactionId { get; set; }
    public  string TotalPremium { get; set; }
    public string Tax { get; set; }
    public string KYC_RequestId { get; set; }
    public string CarrierType { get; set; }
    public string UsageNatureId { get; set; }
    public string VehicleBodyId { get; set; }
    public string HazardousVehicleUse { get; set; }
    public string IfTrailer { get; set; }
    public string TrailerIDV { get; set; }
    public string CategoryId { get; set; }
    public string SubCategoryId { get; set; }
    public string UsageType { get; set; }
    public string PCVVehicleCategory { get; set; }
}

