namespace Insurance.Domain.ICICI;

public class ICICISubmitPOSPCertificateRequest
{
    public string IRDALicNo { get; set; }
    public string CertificateNo { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string PanNumber { get; set; }
    public string CertificateUserName { get; set; }
    public string CertificateStatus { get; set; }
    public string Gender { get; set; }
    public string AadhaarNo { get; set; }
    public string CorrelationId { get; set; }
}
