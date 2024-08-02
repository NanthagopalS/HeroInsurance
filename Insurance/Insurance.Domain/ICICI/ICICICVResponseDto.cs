namespace Insurance.Domain.ICICI.Response
{
    public class ICICICVResponseDto
    {
        public cvRiskdetails riskDetails { get; set; }
        public Generalinformation generalInformation { get; set; }
        public Premiumdetails premiumDetails { get; set; }
        public string deviationMessage { get; set; }
        public bool isQuoteDeviation { get; set; }
        public bool breakingFlag { get; set; }
        public bool isApprovalRequired { get; set; }
        public string proposalStatus { get; set; }
        //public object garageCashDetails { get; set; }
        //public int earlyPayRate { get; set; }
        public float systemDiscount { get; set; }
        public Taxdetails taxDetails { get; set; }
        //public object riskCoverRates { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public string statusMessage { get; set; }
        public string correlationId { get; set; }
    }

    public class cvRiskdetails
    {
        public float ncbProtect { get; set; }
        public float imT23OD { get; set; }
        public float imtDiscountOrLoadingValue { get; set; }
        public float legalLiabilityforNonFarePass { get; set; }
        public float overTurningLoading { get; set; }
        public float privateUseOD { get; set; }
        public float privateUseTP { get; set; }
        public float breakinLoadingAmount { get; set; }
        public float garageCash { get; set; }
        public float biFuelKitOD { get; set; }
        public float biFuelKitTP { get; set; }
        public float tyreProtect { get; set; }
        public float fibreGlassFuelTank { get; set; }
        public float trailersLiability { get; set; }
        public float paCoverCleanersAndConductors { get; set; }
        public float trailorOD { get; set; }
        public string legalLiabilityforCCC { get; set; }
        public float basicOD { get; set; }
        public float geographicalExtensionOD { get; set; }
        public float electricalAccessories { get; set; }
        public float nonElectricalAccessories { get; set; }
        public float consumables { get; set; }
        public float zeroDepreciation { get; set; }
        public float returnToInvoice { get; set; }
        public float roadSideAssistance { get; set; }
        public float engineProtect { get; set; }
        public float keyProtect { get; set; }
        public float lossOfPersonalBelongings { get; set; }
        public float voluntaryDiscount { get; set; }
        public float antiTheftDiscount { get; set; }
        public float automobileAssociationDiscount { get; set; }
        public float handicappedDiscount { get; set; }
        public float emeCover { get; set; }
        public float basicTP { get; set; }
        public float paidDriver { get; set; }
        public float employeesOfInsured { get; set; }
        public float geographicalExtensionTP { get; set; }
        public float paCoverForUnNamedPassenger { get; set; }
        public float paCoverForOwnerDriver { get; set; }
        public float paCoverForPaidDriver { get; set; }
        public float tppD_Discount { get; set; }
        public float bonusDiscount { get; set; }
        public bool paCoverWaiver { get; set; }
        public float ncbPercentage { get; set; }
        public float batteryProtect { get; set; }
    }

    public class Generalinformation
    {
        public string vehicleModel { get; set; }
        public string manufacturerName { get; set; }
        public string manufacturingYear { get; set; }
        public string vehicleDescription { get; set; }
        public string rtoLocation { get; set; }
        public float showRoomPrice { get; set; }
        public float chassisPrice { get; set; }
        public float bodyPrice { get; set; }
        public int seatingCapacity { get; set; }
        public float carryingCapacity { get; set; }
        public string policyInceptionDate { get; set; }
        public string policyEndDate { get; set; }
        public string transactionType { get; set; }
        public string cubicCapacity { get; set; }
        public string proposalDate { get; set; }
        public string referenceProposalDate { get; set; }
        public float depriciatedIDV { get; set; }
        public string tenure { get; set; }
        public string tpTenure { get; set; }
        public string registrationDate { get; set; }
        public string percentageOfDepriciation { get; set; }
        public string proposalNumber { get; set; }
        public string referenceProposalNo { get; set; }
        public string customerId { get; set; }
        public string customerType { get; set; }
        public string rtoLocationCode { get; set; }
        public object discountType { get; set; }
        public object discountLoadName { get; set; }
        public float imtDiscountOrLoadingValue { get; set; }
        public string bodyTypeDescription { get; set; }
        public string quoteId { get; set; }
    }

    public class Premiumdetails
    {
        public float totalOwnDamagePremium { get; set; }
        public float totalLiabilityPremium { get; set; }
        public string packagePremium { get; set; }
        public float specialDiscount { get; set; }
        public float totalTax { get; set; }
        public float finalPremium { get; set; }
        public float bonusDiscount { get; set; }
    }

    public class Taxdetails
    {
        public float igstRate { get; set; }
        public float igstAmount { get; set; }
        public float sgstRate { get; set; }
        public float sgstAmount { get; set; }
        public float cgstRate { get; set; }
        public float cgstAmount { get; set; }
        public float utgstRate { get; set; }
        public float utgstAmount { get; set; }
    }

}