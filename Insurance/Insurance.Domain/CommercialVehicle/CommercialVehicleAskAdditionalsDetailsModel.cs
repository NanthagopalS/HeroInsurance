using Insurance.Domain.GoDigit;

namespace Insurance.Domain.CommercialVehicle
{
    public class CommercialVehicleAskAdditionalsDetailsModel
    {
        public List<CommercialVehicleCategoryModel> CommercialVehicleCategory { get; set; }
    }
    public class CommercialVehicleCategoryModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Ask { get; set; } = false;
        public string label { get; set; }
        public IEnumerable<NameValueModel> SubCategoryList { get; set; }
    }
    public class CommercialVehicleAskOptionsSwitch
    {
        public bool AskForHazardusVehicleUse { get; set; }
        public bool AskForIfTrailer { get; set; }
        public bool AskCarrierType { get; set; }
    }
}
