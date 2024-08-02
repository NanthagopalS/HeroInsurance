namespace Insurance.Domain.Oriental;

public class OrientalProposalDynamicDetail
{
    public Personaldetails PersonalDetails { get; set; }
    public Addressdetails AddressDetails { get; set; }
    public Nomineedetails NomineeDetails { get; set; }
    public Vehicledetails VehicleDetails { get; set; }
}

public class Personaldetails
{
    public string customerName { get; set; }
    public string mobile { get; set; }
    public string emailId { get; set; }
    public string companyName { get; set; }
    public string dateOfIncorporation { get; set; }
    public string dateOfBirth { get; set; }
    public string documentId { get; set; }
    public string documentType { get; set; }
    public string gender { get; set; }
}

public class Addressdetails
{
    public string addressLine1 { get; set; }
    public string state { get; set; }
    public string city { get; set; }
    public string pincode { get; set; }
}
public class Nomineedetails
{
    public string nomineeName { get; set; }
    public string nomineeDateOfBirth { get; set; }
    public string nomineeRelation { get; set; }
}

public class Vehicledetails
{
    public string engineNumber { get; set; }
    public string chassisNumber { get; set; }
    public string isFinancier { get; set; }
    public string financer { get; set; }
    public string branch { get; set; }
    public string vehicleNumber { get; set; }
    public string vehicleColour { get; set; }
    public string financierAddressLine1 { get; set; }
    public string financierpincode { get; set; }
    public string financierstate { get; set; }
    public string financiercity { get; set; }
}




