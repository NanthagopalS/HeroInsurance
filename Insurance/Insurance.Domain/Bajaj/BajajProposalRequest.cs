namespace Insurance.Domain.Bajaj;

public class BajajProposalRequest
{
    public Personaldetails PersonalDetails { get; set; }
    public AddressDetails AddressDetails { get; set; }
    public Nomineedetails NomineeDetails { get; set; }
    public Vehicledetails VehicleDetails { get; set; }
}

public class Personaldetails
{
    public string firstName { get; set; }
    public string middleName { get; set; }
    public string lastName { get; set; }
    public string dateOfBirth { get; set; }
    public string gender { get; set; }
    public string mobile { get; set; }
    public string emailId { get; set; }
    public string companyName { get; set; }
    public string dateOfIncorporation { get; set; }
}
public class Nomineedetails
{
    public string nomineeFirstName { get; set; }
    public string middleName { get; set; }
    public string nomineeLastName { get; set; }
    public string nomineeAge { get; set; }
    public string nomineeRelation { get; set; }
    public string nomineeDateOfBirth { get; set; }
}
public class VehicledetailsResponse
{
    public string chassisNumber { get; set; }
    public string engineNumber { get; set; }
    public string financierName { get; set; }
    public string financierBranch { get; set; }
    public string isFinancier { get; set; }
    public string stringval1 { get; set; }
    public string stringval2 { get; set; }
    public string stringval3 { get; set; }
    public string stringval4 { get; set; }
    public string stringval5 { get; set; }
    public string stringval6 { get; set; }
    public string stringval7 { get; set; }
    public string stringval8 { get; set; }
    public string stringval9 { get; set; }
    public string stringval10 { get; set; }
    public string stringval11 { get; set; }
    public string stringval12 { get; set; }
    public string stringval13 { get; set; }
    public string stringval14 { get; set; }
    public string stringval15 { get; set; }
    public string stringval16 { get; set; }
    public string stringval17 { get; set; }
    public string stringval18 { get; set; }
    public string stringval19 { get; set; }
    public string stringval20 { get; set; }
    public string stringval21 { get; set; }
    public string stringval22 { get; set; }
    public string stringval23 { get; set; }
    public string stringval24 { get; set; }
    public string stringval25 { get; set; }
    public string stringval26 { get; set; }
    public string stringval27 { get; set; }
    public string stringval28 { get; set; }
    public string stringval29 { get; set; }
    public string stringval30 { get; set; }
    public string stringval31 { get; set; }
    public string stringval32 { get; set; }
    public string stringval33 { get; set; }
    public string stringval34 { get; set; }
    public string stringval35 { get; set; }
    public string stringval36 { get; set; }
    public string stringval37 { get; set; }
    public string stringval38 { get; set; }
    public string stringval39 { get; set; }
    public string stringval40 { get; set; }
    public string stringval41 { get; set; }
    public string stringval42 { get; set; }
    public string stringval43 { get; set; }
    public string stringval44 { get; set; }
    public string stringval45 { get; set; }
    public string stringval46 { get; set; }
    public string stringval47 { get; set; }
    public string stringval48 { get; set; }
    public string stringval49 { get; set; }
    public string stringval50 { get; set; }
    public string stringval51 { get; set; }
    public string stringval52 { get; set; }
    public string stringval53 { get; set; }
    public string stringval54 { get; set; }
    public string stringval55 { get; set; }
    public string stringval56 { get; set; }
    public string stringval57 { get; set; }
    public string stringval58 { get; set; }
    public string stringval59 { get; set; }
    public string stringval60 { get; set; }
}

public class AddressDetails
{
    public string addLine1 { get; set; }
    public string addLine2 { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string pincode { get; set; }
}
public class Vehicledetails
{
    public string chassisNumber { get; set; }
    public string engineNumber { get; set; }
    public string financer { get; set; }
    public string financierBranch { get; set; }
    public string isFinancier { get; set; }
    public string vehicleNumber { get; set; }
}
