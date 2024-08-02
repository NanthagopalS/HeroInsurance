namespace Insurance.Domain.Reliance
{


    public class Rootobject
    {
        public Motorpolicy MotorPolicy { get; set; }
    }

    public class Motorpolicy
    {
        public string NetPremium { get; set; }
        public string BasicPremium { get; set; }
        public string OriginalPremium { get; set; }
        public string EndorsedPremium { get; set; }
        public string FinalPremium { get; set; }
        public string ECessAmount { get; set; }
        public string HECessAmount { get; set; }
        public string TotalPremium { get; set; }
        public string TotalOD { get; set; }
        public string ServiceTaxRate { get; set; }
        public string EducationalCessRate { get; set; }
        public string HigherEducationalCessRate { get; set; }
        public string TotalScheduleODPremium { get; set; }
        public string TotalODPremium { get; set; }
        public string ServiceTaxAmount { get; set; }
        public string TotalLiabilityPremium { get; set; }
        public string TotalPackagePremium { get; set; }
        public string TotalAddonPremium { get; set; }
        public string TotalChangeAmount { get; set; }
        public string InspectionCharges { get; set; }
        public string InspectionChargesapplicable { get; set; }
        public string ErrorMessages { get; set; }
        public string IsEligible { get; set; }
        public string ReferalMessages { get; set; }
        public string status { get; set; }
        public string TraceID { get; set; }
        public string ErrorCode { get; set; }
        public string IDV { get; set; }
        public string BodyIDV { get; set; }
        public string ChassisIDV { get; set; }
        public string MinIDV { get; set; }
        public string MaxIDV { get; set; }
        public string MinBodyIDV { get; set; }
        public string MaxBodyIDV { get; set; }
        public string MinChassisIDV { get; set; }
        public string MaxChassisIDV { get; set; }
        public string DerivedVehicleIDV { get; set; }
        public string ProposalNo { get; set; }
        public string SalesTaxAmount { get; set; }
        public string SalesTaxRate { get; set; }
        public string SurchargeAmount { get; set; }
        public string SurchrgeRate { get; set; }
        public string IsClaimedInLastPolicy { get; set; }
        public string CurrentYearNCB { get; set; }
        public string Current2YearNCB { get; set; }
        public string Current3YearNCB { get; set; }
        public string InspectionErrorMessage { get; set; }
        public string SwachhBharatCess { get; set; }
        public string SwachhBharatCessRate { get; set; }
        public string KrishiKalyanCess { get; set; }
        public string KrishiKalyanCessRate { get; set; }
        public string InvoiceNo { get; set; }
        public string NetPremium2Year { get; set; }
        public string BasicPremium2Year { get; set; }
        public string FinalPremium2Year { get; set; }
        public string TotalOD2Year { get; set; }
        public string TotalODPremium2Year { get; set; }
        public string TotalLiabilityPremium2Year { get; set; }
        public string TotalPackagePremium2Year { get; set; }
        public string TotalAddonPremium2Year { get; set; }
        public string SecondYearBasicVehicleIDV { get; set; }
        public string NetPremium3Year { get; set; }
        public string BasicPremium3Year { get; set; }
        public string FinalPremium3Year { get; set; }
        public string TotalOD3Year { get; set; }
        public string TotalODPremium3Year { get; set; }
        public string TotalLiabilityPremium3Year { get; set; }
        public string TotalPackagePremium3Year { get; set; }
        public string TotalAddonPremium3Year { get; set; }
        public string ThirdYearBasicVehicleIDV { get; set; }
        public string FourthYearBasicVehicleIDV { get; set; }
        public string FifthYearBasicVehicleIDV { get; set; }
        public Lsttaxcomponentdetails LstTaxComponentDetails { get; set; }
        public Lstpricingresponse[] lstPricingResponse { get; set; }
        public string EndorsementNo { get; set; }
        public string QuoteNo { get; set; }
    }

    public class Lsttaxcomponentdetails
    {
        public Taxcomponent TaxComponent { get; set; }
        public Taxcomponent2year TaxComponent2Year { get; set; }
        public Taxcomponent3year TaxComponent3Year { get; set; }
    }

    public class Taxcomponent
    {
        public string TaxName { get; set; }
        public string Rate { get; set; }
        public string Amount { get; set; }
    }

    public class Taxcomponent2year
    {
        public string TaxName { get; set; }
        public string Rate { get; set; }
        public string Amount { get; set; }
    }

    public class Taxcomponent3year
    {
        public string TaxName { get; set; }
        public string Rate { get; set; }
        public string Amount { get; set; }
    }

    public class Lstpricingresponse
    {
        public string CoverID { get; set; }
        public string Premium { get; set; }
        public string Premium2Year { get; set; }
        public string Premium3Year { get; set; }
        public string CoverageName { get; set; }
        public string SumInsured { get; set; }
        public string OriginalPremium { get; set; }
        public string EndorsementPremium { get; set; }
        public string PremiumDifference { get; set; }
    }


}
