namespace Insurance.Domain.GoDigit
{

    public class ProposalRequest
    {
        public Personaldetails PersonalDetails { get; set; }
        public AddressDetails AddressDetails { get; set; }
        public Nomineedetail NomineeDetails { get; set; }
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
        public string documentId { get; set; }
        public string companyName { get; set; }
        public string gstno { get; set; }
    }

    public class Nomineedetail
    {
        public string nomineeFirstName { get; set; }
        public string middleName { get; set; }
        public string nomineeLastName { get; set; }
        public string nomineeDateOfBirth { get; set; }
        public string nomineeRelation { get; set; }
        public string personType { get; set; }
    }
    public class Vehicledetails
    {
        public string chassisNumber { get; set; }
        public string financer { get; set; }
        public string engineNumber { get; set; }
        public string vehicleNumber { get; set; }
        public string isFinancier { get; set; }
        public bool selfInspection { get; set; }
    }
    public class AddressDetails
    {
        public string addressType { get; set; }
        public string flatNumber { get; set; }
        public string streetNumber { get; set; }
        public string pincode { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string district { get; set; }
        public string state { get; set; }
        public string country { get; set; }
    }

}
