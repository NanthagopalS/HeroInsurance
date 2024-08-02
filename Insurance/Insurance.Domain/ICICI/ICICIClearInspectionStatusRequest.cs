namespace Insurance.Domain.ICICI;
public class ICICIClearInspectionStatusRequest
{
    public string InspectionId { get; set; }
    public string DealNo { get; set; }
    public string ReferenceDate { get; set; }
    public string InspectionStatus { get; set; }
    public string CorrelationId { get; set; }
    public string ReferenceNo { get; set; }
}
