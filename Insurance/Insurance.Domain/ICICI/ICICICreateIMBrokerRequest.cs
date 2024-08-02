namespace Insurance.Domain.ICICI;

public class ICICICreateIMBrokerRequest
{
    public string PanNumber { get; set; }
    public string LicenseNo { get; set; }
    public string IlLocation { get; set; }
    public string CertificateNo { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string FatherHusbandName { get; set; }
    public string DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public string ContactPersonMobile { get; set; }
    public string ContactPersonEmailId { get; set; }
    public string Address { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
    public string CorrelationId { get; set; }
}
