namespace Insurance.Domain.ICICI;

public class ICICIGetBreakinStatusResponseModel
{
    public string inspectionStatus { get; set; }
    public string breakInInsuranceID { get; set; }
    public string typeOfInspection { get; set; }
    public string typeofBreakIn { get; set; }
    public string message { get; set; }
    public bool status { get; set; }
    public string statusMessage { get; set; }
    public string correlationId { get; set; }
}
