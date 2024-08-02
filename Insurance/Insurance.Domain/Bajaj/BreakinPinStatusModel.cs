namespace Insurance.Domain.Bajaj
{
    public class BreakinPinStatusModel
    {
        public string VehicleNumber { get; set; }
        public string LeadId { get; set; }
    }

    public class BreakinPinStatusResponse
    {
        public string PinStatus { get; set; }
    }

    public class BreakinPinGeneration
    {
        public string pinNumber { get; set; }
        public string pinStatus { get; set; }
        public VehicledetailsResponse vehicleDetails { get; set; }
    }

}
