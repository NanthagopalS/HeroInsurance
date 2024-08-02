using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.GoDigit
{
    public class GoDigitProposalResponseDto
    {
        public string enquiryId { get; set; }
        public Contract contract { get; set; }
        public Vehicle vehicle { get; set; }
        public Previousinsurer previousInsurer { get; set; }
        public ProposalPospinfo pospInfo { get; set; }
        public Preinspection preInspection { get; set; }
        public ProposalPerson[] persons { get; set; }
        public ProposalNominee nominee { get; set; }
        public ProposalMotorquestions motorQuestions { get; set; }
        public Motorbreakin motorBreakIn { get; set; }
        public Kycstatus kycStatus { get; set; }
        public string applicationId { get; set; }
        public string policyNumber { get; set; }
        public string policyState { get; set; }
        public string policyStatus { get; set; }
        public string state { get; set; }
        public string netPremium { get; set; }
        public string grossPremium { get; set; }
        public ProposalDiscounts discounts { get; set; }
        public Loadings loadings { get; set; }
        public Servicetax serviceTax { get; set; }
        public string[] informationMessages { get; set; }
        public Error error { get; set; }
    }

    public class Kycstatus
    {
        public string policyNumber { get; set; }
        public string policyStatus { get; set; }
        public string paymentStatus { get; set; }
        public string kycVerificationStatus { get; set; }
        public string mismatchType { get; set; }
        public string referenceId { get; set; }
        public string idVerificationDocType { get; set; }
        public string addressVerificationDocType { get; set; }
        public string mode { get; set; }
        public string link { get; set; }
    }


    public class Contract
    {
        public string insuranceProductCode { get; set; }
        public string subInsuranceProductCode { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string policyHolderType { get; set; }
        public string currentNoClaimBonus { get; set; }
        public bool isNCBTransfer { get; set; }
        public string quotationDate { get; set; }
        public Coverages coverages { get; set; }
    }

    public class Coverages
    {
        public Thirdpartyliability thirdPartyLiability { get; set; }
        public Owndamage ownDamage { get; set; }
        public Fire fire { get; set; }
        public Theft theft { get; set; }
        public Personalaccident personalAccident { get; set; }
        public ProposalAccessories accessories { get; set; }
        public Addons addons { get; set; }
        public Legalliability legalLiability { get; set; }
        public Unnamedpa unnamedPA { get; set; }
        public bool isGeoExt { get; set; }
        public bool isTheftAndConversionRiskIMT43 { get; set; }
        public bool isIMT23 { get; set; }
        public bool isOverturningExclusionIMT47 { get; set; }
    }

    public class Thirdpartyliability
    {
        public bool selection { get; set; }
        public string netPremium { get; set; }
        public bool isTPPD { get; set; }
    }

    public class Owndamage
    {
        public bool selection { get; set; }
        public string netPremium { get; set; }
        public string traiffPremium { get; set; }
        public string netPremiumWithoutZeroDepWithoutPreInspection { get; set; }
        public string ncbDiscountWithoutZeroDepWithoutPreInspection { get; set; }
        public string policyNetPremiumWithoutZeroDepWithoutPreInspection { get; set; }
        public string policyGrossPremiumWithoutZeroDepWithoutPreInspection { get; set; }
        public ProposalDiscount discount { get; set; }
        public Surcharge surcharge { get; set; }
    }

    public class ProposalDiscount
    {
        public float userSpecialDiscountPercent { get; set; }
        public float defaultSpecialDiscountPercent { get; set; }
        public float defaultSpecialDiscountPercentWithoutPreInspection { get; set; }
        public int effectiveSpecialDiscountPercent { get; set; }
        public int effectiveSpecialDiscountPercentWithoutPreInspection { get; set; }
        public float minSpecialDiscountPercent { get; set; }
        public float maxSpecialDiscountPercent { get; set; }
        public float effectiveIncrementalCommissionPercent { get; set; }
        public float effectiveIncrementalCommissionPercentWithoutPreInspection { get; set; }
        public Discount1[] discounts { get; set; }
        public Incremental incremental { get; set; }
        public Discountswithoutpreinspection[] discountsWithoutPreInspection { get; set; }
        public Discountswithoutzerodep[] discountsWithoutZeroDep { get; set; }
    }

    public class Incremental
    {
        public bool isIncrementalApplicable { get; set; }
        public float defaultCommisionPercentage { get; set; }
        public float maximumIncrementalPercentage { get; set; }
    }

    public class Discount1
    {
        public string discountType { get; set; }
        public float discountPercent { get; set; }
        public string discountAmount { get; set; }
    }

    public class Discountswithoutpreinspection
    {
        public string discountType { get; set; }
        public float discountPercent { get; set; }
        public string discountAmount { get; set; }
    }

    public class Discountswithoutzerodep
    {
        public string discountType { get; set; }
        public float discountPercent { get; set; }
        public string discountAmount { get; set; }
    }

    public class Surcharge
    {
        public Loading[] loadings { get; set; }
    }
    public class Loading
    {
        public string loadingType { get; set; }
        public float loadingPercent { get; set; }
        public string loadingAmount { get; set; }
    }
    public class Fire
    {
        public bool selection { get; set; }
    }

    public class Theft
    {
        public bool selection { get; set; }
    }

    public class Personalaccident
    {
        public bool selection { get; set; }
        public int coverTerm { get; set; }
        public string coverAvailability { get; set; }
        public string netPremium { get; set; }
    }

    public class ProposalAccessories
    {
        public Cng cng { get; set; }
        public Electrical electrical { get; set; }
        public Nonelectrical nonElectrical { get; set; }
    }

    public class Cng
    {
        public bool selection { get; set; }
        public decimal insuredAmount { get; set; }
        public decimal minAllowed { get; set; }
        public decimal maxAllowed { get; set; }
    }

    public class Electrical
    {
        public bool selection { get; set; }
        public decimal insuredAmount { get; set; }
        public decimal minAllowed { get; set; }
        public decimal maxAllowed { get; set; }
    }

    public class Nonelectrical
    {
        public bool selection { get; set; }
        public decimal insuredAmount { get; set; }
        public decimal minAllowed { get; set; }
        public decimal maxAllowed { get; set; }
    }

    public class Addons
    {
        public Partsdepreciation partsDepreciation { get; set; }
        public Personalbelonging personalBelonging { get; set; }
        public Keyandlockprotect keyAndLockProtect { get; set; }
        public Roadsideassistance roadSideAssistance { get; set; }
        public Engineprotection engineProtection { get; set; }
        public Tyreprotection tyreProtection { get; set; }
        public Rimprotection rimProtection { get; set; }
        public Returntoinvoice returnToInvoice { get; set; }
        public Consumables consumables { get; set; }
        public Dailyconveyance dailyConveyance { get; set; }
    }

    public class Partsdepreciation
    {
        public string claimsCovered { get; set; }
        public bool selection { get; set; }
        public string coverAvailability { get; set; }
        public string coverOfferingType { get; set; }
        public string netPremium { get; set; }
    }

    public class Personalbelonging
    {
        public string claimsCovered { get; set; }
        public bool selection { get; set; }
        public string coverAvailability { get; set; }
        public string coverOfferingType { get; set; }
        public string netPremium { get; set; }
    }

    public class Keyandlockprotect
    {
        public string claimsCovered { get; set; }
        public bool selection { get; set; }
        public string coverAvailability { get; set; }
        public string coverOfferingType { get; set; }
        public string netPremium { get; set; }
    }

    public class Roadsideassistance
    {
        public bool selection { get; set; }
        public string coverAvailability { get; set; }
        public string coverOfferingType { get; set; }
        public string netPremium { get; set; }
    }

    public class Engineprotection
    {
        public bool selection { get; set; }
        public string coverAvailability { get; set; }
        public string coverOfferingType { get; set; }
        public string netPremium { get; set; }
    }

    public class Tyreprotection
    {
        public bool selection { get; set; }
        public string coverAvailability { get; set; }
        public string coverOfferingType { get; set; }
        public string netPremium { get; set; }
    }

    public class Rimprotection
    {
        public bool selection { get; set; }
        public string coverAvailability { get; set; }
        public string coverOfferingType { get; set; }
        public string netPremium { get; set; }
    }

    public class Returntoinvoice
    {
        public bool selection { get; set; }
        public string coverAvailability { get; set; }
        public string coverOfferingType { get; set; }
        public string netPremium { get; set; }
    }

    public class Consumables
    {
        public bool selection { get; set; }
        public string coverAvailability { get; set; }
        public string coverOfferingType { get; set; }
        public string netPremium { get; set; }
    }

    public class Dailyconveyance
    {
        public string planType { get; set; }
        public int planLimit { get; set; }
        public int planPeriod { get; set; }
        public int excessPeriod { get; set; }
        public bool selection { get; set; }
        public string coverAvailability { get; set; }
        public string coverOfferingType { get; set; }
        public string netPremium { get; set; }
    }

    public class Legalliability
    {
        public Paiddriverll paidDriverLL { get; set; }
        public Employeesll employeesLL { get; set; }
        public Unnamedpaxll unnamedPaxLL { get; set; }
        public Cleanersll cleanersLL { get; set; }
        public Nonfarepaxll nonFarePaxLL { get; set; }
        public Workerscompensationll workersCompensationLL { get; set; }
    }

    public class Paiddriverll
    {
        public bool selection { get; set; }
        public string netPremium { get; set; }
        public int insuredCount { get; set; }
    }

    public class Employeesll
    {
        public bool selection { get; set; }
        public string netPremium { get; set; }
        public int insuredCount { get; set; }
    }

    public class Unnamedpaxll
    {
        public bool selection { get; set; }
        public string netPremium { get; set; }
        public int insuredCount { get; set; }
    }

    public class Cleanersll
    {
        public bool selection { get; set; }
    }

    public class Nonfarepaxll
    {
        public bool selection { get; set; }
    }

    public class Workerscompensationll
    {
        public bool selection { get; set; }
    }

    public class Unnamedpa
    {
        public Unnamedpax unnamedPax { get; set; }
        public Unnamedpaiddriver unnamedPaidDriver { get; set; }
        public Unnamedhirer unnamedHirer { get; set; }
        public Unnamedpillionrider unnamedPillionRider { get; set; }
        public Unnamedcleaner unnamedCleaner { get; set; }
        public Unnamedconductor unnamedConductor { get; set; }
    }

    public class Unnamedpax
    {
        public bool selection { get; set; }
        public float insuredAmount { get; set; }
        public string netPremium { get; set; }
    }

    public class Unnamedpaiddriver
    {
        public bool selection { get; set; }
        public float insuredAmount { get; set; }
        public string netPremium { get; set; }
    }

    public class Unnamedhirer
    {
        public bool selection { get; set; }
    }

    public class Unnamedpillionrider
    {
        public bool selection { get; set; }
    }

    public class Unnamedcleaner
    {
        public bool selection { get; set; }
    }

    public class Unnamedconductor
    {
        public bool selection { get; set; }
    }

    public class Vehicle
    {
        public bool isVehicleNew { get; set; }
        public string vehicleMaincode { get; set; }
        public string licensePlateNumber { get; set; }
        public string registrationAuthority { get; set; }
        public string vehicleIdentificationNumber { get; set; }
        public string engineNumber { get; set; }
        public string manufactureDate { get; set; }
        public string registrationDate { get; set; }
        public Vehicleidv vehicleIDV { get; set; }
        public string[] trailers { get; set; }
        public float annualMileage { get; set; }
        public string make { get; set; }
        public string model { get; set; }
    }

    public class Vehicleidv
    {
        public decimal idv { get; set; }
        public decimal defaultIdv { get; set; }
        public decimal minimumIdv { get; set; }
        public decimal maximumIdv { get; set; }
    }

    public class Previousinsurer
    {
        public bool isPreviousInsurerKnown { get; set; }
        public string previousInsurerCode { get; set; }
        public string previousPolicyExpiryDate { get; set; }
        public bool isClaimInLastYear { get; set; }
        public string originalPreviousPolicyType { get; set; }
        public string previousNoClaimBonus { get; set; }
        public Currentthirdpartypolicy currentThirdPartyPolicy { get; set; }
        public int maxAllowedNCB { get; set; }
    }

    public class Currentthirdpartypolicy
    {
    }

    public class ProposalPospinfo
    {
        public bool isPOSP { get; set; }
    }

    public class Preinspection
    {
        public bool isPreInspectionOpted { get; set; }
        public bool isPreInspectionRequired { get; set; }
        public bool isPreInspectionEligible { get; set; }
        public bool isPreInspectionWaived { get; set; }
        public bool isSchoolBusWaiverEligibleWithTPlusFortyEight { get; set; }
        public string[] preInspectionReasons { get; set; }
        public string piType { get; set; }
    }

    public class ProposalNominee
    {
        public string personType { get; set; }
        public string[] communications { get; set; }
        public string[] identificationDocuments { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string dateOfBirth { get; set; }
        public string relation { get; set; }
    }

    public class ProposalMotorquestions
    {
        public bool selfInspection { get; set; }
        public bool whatsappCommunicationConstent { get; set; }
    }

    public class Motorbreakin
    {
        public bool isBreakin { get; set; }
        public string breakinExcess { get; set; }
        public string breakinComments { get; set; }
        public bool isPreInspectionWaived { get; set; }
        public bool isPreInspectionCompleted { get; set; }
        public bool isDocumentUploaded { get; set; }
    }

    public class ProposalDiscounts
    {
        public string specialDiscountAmount { get; set; }
        public Otherdiscount[] otherDiscounts { get; set; }
    }

    public class Otherdiscount
    {
        public string discountType { get; set; }
        public float discountPercent { get; set; }
        public string discountAmount { get; set; }
    }

    public class Loadings
    {
        public string totalLoadingAmount { get; set; }
        public Otherloading[] otherLoadings { get; set; }
    }
    public class Otherloading
    {
        public string loadingType { get; set; }
        public float loadingPercent { get; set; }
        public string loadingAmount { get; set; }
    }
    public class Servicetax
    {
        public string cgst { get; set; }
        public string sgst { get; set; }
        public string igst { get; set; }
        public string utgst { get; set; }
        public string totalTax { get; set; }
        public string taxType { get; set; }
    }

    public class Error
    {
        public int errorCode { get; set; }
        public int httpCode { get; set; }
        public string[] validationMessages { get; set; }
        public string errorLink { get; set; }
        public string errorStackTrace { get; set; }
    }

    public class ProposalPerson
    {
        public string personType { get; set; }
        public string partyId { get; set; }
        public ProposalAddress[] addresses { get; set; }
        public ProposalCommunication[] communications { get; set; }
        public ProposalIdentificationdocument[] identificationDocuments { get; set; }
        public bool isPolicyHolder { get; set; }
        public bool isVehicleOwner { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string dateOfBirth { get; set; }
        public string gender { get; set; }
        public bool isDriver { get; set; }
        public bool isInsuredPerson { get; set; }
    }

    public class ProposalAddress
    {
        public string addressId { get; set; }
        public string addressType { get; set; }
        public string flatNumber { get; set; }
        public string streetNumber { get; set; }
        public string street { get; set; }
        public string district { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string pincode { get; set; }
    }

    public class ProposalCommunication
    {
        public string communicationNote { get; set; }
        public string identifier { get; set; }
        public string communicationType { get; set; }
        public string communicationId { get; set; }
        public bool isPrefferedCommunication { get; set; }
        public string communicationChannel { get; set; }
    }

    public class ProposalIdentificationdocument
    {
        public string identificationDocumentId { get; set; }
        public string documentType { get; set; }
        public string documentId { get; set; }
        public string issuingAuthority { get; set; }
        public string issuingPlace { get; set; }
        public string issueDate { get; set; }
        public string expiryDate { get; set; }
    }

}
