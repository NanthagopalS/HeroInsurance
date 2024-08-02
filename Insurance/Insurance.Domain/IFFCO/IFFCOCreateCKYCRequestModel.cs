namespace Insurance.Domain.IFFCO;

public class IFFCOCreateCKYCRequestModel
{
    public string clientType { get; set; }
    public string prefix { get; set; }
    public string firstName { get; set; }
    public string middleName { get; set; }
    public string lastName { get; set; }
    public string fatherPrefix { get; set; }
    public string fatherFirstName { get; set; }
    public string fatherMiddleName { get; set; }
    public string fatherLastName { get; set; }
    public string gender { get; set; }
    public string dateofBirth { get; set; }
    public string mobileNumber { get; set; }
    public string emailAddress { get; set; }
    public string addressLine1 { get; set; }
    public string pinCode { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string country { get; set; }
    public string district { get; set; }
    public string correspondenceAddressLine1 { get; set; }
    public string correspondencePinCode { get; set; }
    public string correspondenceCity { get; set; }
    public string correspondenceState { get; set; }
    public string correspondenceCountry { get; set; }
    public string correspondenceDistrict { get; set; }
    public Kycdocument[] kycDocuments { get; set; }
}

public class Kycdocument
{
    public string idType { get; set; }
    public string idName { get; set; }
    public string idNumber { get; set; }
    public string fileName { get; set; }
    public string fileExtension { get; set; }
    public string fileBase64 { get; set; }
}
