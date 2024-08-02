using System.Xml.Serialization;

namespace Insurance.Domain.ICICI;

public class HDFCCreateIMBrokerRequestDto
{
    public string PanNumber { get; set; }
    public string AadharNumber { get; set; }
    public string Name { get; set; }
    public string MobileNumber { get; set; }
    public string Gender { get; set; }
    public string EmailId { get; set; }
    public string DateofBirth { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string State { get; set; }
    public string CityName { get; set; }
    public string Pincode { get; set; }
    public string Country { get; set; }
    public string LastName { get; set; }
    public string FatherHusbandName { get; set; }
    public string CorrelationId { get; set; }
    public string PospId { get; set; }
    public string LeadId { get; set; }
}
