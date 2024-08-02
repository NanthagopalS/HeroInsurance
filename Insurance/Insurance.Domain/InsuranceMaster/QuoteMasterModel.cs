using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.InsuranceMaster
{
    public class QuoteMasterModel
    {
        public IEnumerable<DiscountModel> DiscountList { get; set; }
        public IEnumerable<AddonsModel> AddOnsList { get; set; }
        public IEnumerable<AccessoryModel> AccessoryList { get; set; }
        public IEnumerable<PACoverModel> PACoverList { get; set; }
        public IEnumerable<ConfigNameValueModel> ConfigNameValueList { get; set; }
    }

    public class DiscountModel
    {
        public string DiscountId { get; set; }
        public string DiscountName { get; set; }
        public List<DiscountExtensionModel> DiscountExtensionList { get; set; }
    }

    public class AddonsModel
    {
        public string AddOnId { get; set; }
        public string AddOns { get; set; }
        public bool IsRecommended { get; set; }
        public string Description { get; set; }
        public List<AddOnsExtensionModel> AddOnsExtensionList { get; set; }
    }

    public class AccessoryModel
    {
        public string AccessoryId { get; set; }
        public string Accessory { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
    }

    public class PACoverModel
    {
        public string PACoverId { get; set; }
        public string CoverName { get; set; }
        public List<PACoverExtensionModel> PACoverExtensionList { get; set; }
        public bool IsDefault { get; set; }
    }

    public class PACoverExtensionModel
    {
        public string PACoverExtensionId { get; set; }
        public string PACoverExtension { get; set; }
        public string PACoverId { get; set; }
    }
    public class AddOnsExtensionModel
    {
        public string AddOnsExtensionId { get; set; }
        public string AddOnsExtension { get; set; }
        public string AddOnsId { get; set; }
    }
    public class DiscountExtensionModel
    {
        public string DiscountExtensionId { get; set; }
        public string DiscountExtension { get; set; }
        public string DiscountId { get; set; }
        public string DiscountValueInWords { get; set; }
    }

    public class RTOVehiclePreviousInsurerModel
    {
        public string LicensePlateNumber { get; set; }
        public string RTOCode { get; set; }
        public string PreviousInsurerCode { get; set; }
        public string VehicleCode { get; set; }
        public string NCBValue { get; set; }
        public string NCBValueId { get; set; }
        public string State_Id { get; set; }
        public string CityName { get; set; }
        public string VehicleType { get; set; }
        public string VehicleMakeCode { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModelCode { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleSubTypeCode { get; set; }
        public string VehicleSubType { get; set; }
        public string VehicleBodyType { get; set; }
        public string VehicleVariant { get; set; }
        public string VehicleVariantCode { get; set; }
        public string Fuel { get; set; }
        public string FuelId { get; set; }
        public string VehicleSegment { get; set ; }
        public string NoOfWheels { get; set; }
        public string vehicleclass { get; set; }
        public string chassis { get; set; }
        public string engine { get; set; }
        public string vehicleColour { get; set; }
        public string regDate { get; set; }
        public string vehicleCubicCapacity { get; set; }
        public string vehicleSeatCapacity { get; set; }
        public string vehicleNumber { get; set; }
        public string RTOLocationCode { get; set; }
        public string RTOLocationName { get; set; }
        public double ExShowRoomPrice { get; set; }
        public string CurrentPolicyType { get; set; }
        public string CurrentPolicyTypeId { get; set; }
        public string GSTToState { get; set; }
        public string ManufactureDate { get; set; }
        public string PreviousPolicyType { get; set; }
        public string Zone { get; set; }
        public string CountryCode { get; set; }
        public string RegistrationStateCode { get; set; }
        public string CityCode { get; set; }
        public string RegistrationRTOCode { get; set; }
        public string RTOCityName { get; set; }
        public string RTOStateName { get; set; }
        public string SAODInsurer { get; set; }
        public string SATPInsurer { get; set; }
        public string PreviousInsurancePolicyNumber { get; set; }
        public string OriginalPreviousPolicyType { get; set; }
        public string PlanType { get; set; }
        public string ExtensionCountryCode { get; set; }
        public string GeogExtension { get; set; }
        public int IDVValue { get; set; }
        public int MinIdv { get; set; }
        public int MaxIdv { get; set; }
        public int RecommendedIdv { get; set; }
        public string BusinessType { get; set; }
        public string BusinessTypeId { get; set; }
        public string VoluntaryExcessCode { get; set; }
        public string DiscountPercentage { get; set; }
        public string GrossVehicleWeight { get; set; }
        public string IDVPercentage { get; set; }
        public double ChassisPrice { get; set; }
    }
    public class ConfigNameValueModel
    {
        public string ConfigName { get; set; }
        public string ConfigValue { get; set; }
        public int? UsageTypeId { get; set; }
        public int? SubUsageTypeId { get; set; }
        public int? VehicleCategoryId { get; set; }
    }

    public class PackageName
    {
        public string Package_Name { get; set; }
    }
}

