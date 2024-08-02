using Newtonsoft.Json;

namespace Insurance.Domain.IFFCO;

public class IFFCOBreakinRequestModel
{
    [JsonProperty("customerName")]
    public string CustomerName { get; set; }
    [JsonProperty("customerPhoneNumber")]
    public string CustomerPhoneNumber { get; set; }
    [JsonProperty("vehicleNumber")]
    public string VehicleNumber { get; set; }
    [JsonProperty("vehicleType")]
    public string VehicleType { get; set; }
    [JsonProperty("quoteNumber")]
    public string QuoteNumber { get; set; }
    [JsonProperty("purposeOfInspection")]
    public string PurposeOfInspection { get; set; }
    [JsonProperty("inspectionNumber")]
    public string InspectionNumber { get; set; }
    [JsonProperty("chassisNumber")]
    public string ChassisNumber { get; set; }
    [JsonProperty("engineNumber")]
    public string EngineNumber { get; set; }
    [JsonProperty("paidBy")]
    public string PaidBy { get; set; }
}
