using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATAProposalRequest
    {
        public Personaldetails PersonalDetails { get; set; }
        public Addressdetails AddressDetails { get; set; }
        public Nomineedetails NomineeDetails { get; set; }
        public Vehicledetails VehicleDetails { get; set; }
    }

    public class Personaldetails
    {
        public string salutation { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string dateOfBirth { get; set; }
        public string gender { get; set; }
        public string mobile { get; set; }
        public string emailId { get; set; }
        public string maritalStatus { get; set; }
        public string occupation { get; set; }
        public string panNumber { get; set; }
        public string companyName { get; set; }
        public string gstno { get; set; }
    }

    public class Addressdetails
    {
        public string addressLine1 { get; set; }
        public string pincode { get; set; }
        public string state { get; set; }
        public string city { get; set; }
    }
    public class Nomineedetails
    {
        public string nomineeName { get; set; }
        public string nomineeAge { get; set; }
        public string nomineeRelation { get; set; }
    }
    public class Vehicledetails
    {
        public string isFinancier { get; set; }
        public string financer { get; set; }
        public string agreementType { get; set; }
        public string chassisNumber { get; set; }
        public string engineNumber { get; set; }
        public string vehicleNumber { get; set; }

    }

}
