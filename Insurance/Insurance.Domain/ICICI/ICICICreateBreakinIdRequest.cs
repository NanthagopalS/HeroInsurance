namespace Insurance.Domain.ICICI;

public class ICICICreateBreakinIdRequest
{
    public string CorrelationId { get; set; }
    public string BreakInType { get; set; }
    public int BreakInDays { get; set; }
    public string CustomerName { get; set; }
    public string CustomerAddress { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public long MobileNumber { get; set; }
    public string TypeVehicle { get; set; }
    public string VehicleMake { get; set; }
    public string VehicleModel { get; set; }
    public string ManufactureYear { get; set; }
    public string RegistrationNo { get; set; }
    public string EngineNo { get; set; }
    public string ChassisNo { get; set; }
    public string SubLocation { get; set; }
    public string DistributorInterID { get; set; }
    public string DistributorName { get; set; }
    public string InspectionType { get; set; }
    public string DealId { get; set; }
}

