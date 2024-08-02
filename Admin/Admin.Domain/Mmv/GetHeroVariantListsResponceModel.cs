namespace Admin.Domain.Mmv
{
    public class HeroVariantLists
    {
        public string ICName { get; set; }
        public string MMVScore {get ; set;}
        public string HeroVarientID { get; set; }
        public string ICVehicleCode { get; set; }
        public string HEROMake { get; set; }
        public string HeroModel { get; set; }
        public string HeroVarient { get; set; }
        public string HeroSC { get; set; }
        public string HCC { get; set; }
        public string HeroFuel { get; set; }
        public string ICMake { get; set; }
        public string ICModel { get; set; }
        public string ICVarient { get; set; }
        public string ICSC { get; set; }
        public string CCC { get; set; }
        public string ICFuel { get; set; }
        public string HeroGVW { get; set; }
        public string ICGVW { get; set; }
        public bool IsManuallyMapped { get; set; }
        public bool IsManuallyMappedRecordSelected { get; set; } = false;

    }
    public class ICVariantPosibilityLists
    {
        public string HeroVarientID { get; set; }
        public string ICVehicleCode { get; set; }
        public string HEROMake { get; set; }
        public string HeroModel { get; set; }
        public string HeroVarient { get; set; }
        public string HeroSC { get; set; }
        public string HCC { get; set; }
        public string HeroFuel { get; set; }
        public string ICMake { get; set; }
        public string ICModel { get; set; }
        public string ICVarient { get; set; }
        public string ICSC { get; set; }
        public string CCC { get; set; }
        public string ICFuel { get; set; }
        public string HeroGVW { get; set; }
        public string ICGVW { get; set; }
        public bool IsManuallyMapped { get; set; }
    }
    public class GetHeroVariantListsResponceModel
    {
        public IEnumerable<HeroVariantLists> HeroVariantLists { get; set; }
        public IEnumerable<ICVariantPosibilityLists> ICVariantPosibilityLists { get; set; }
    }
}
