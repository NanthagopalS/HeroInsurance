namespace Insurance.Domain.ICICI;

public class ICICIClearInspectionStatusResponse
{
    public object referenceNo { get; set; }
    public string policyNo { get; set; }
    public string error_Id { get; set; }
    public string vehicleInspectionStatus { get; set; }
    public string message { get; set; }
    public bool status { get; set; }
    public string statusMessage { get; set; }
    public string correlationId { get; set; }
}
