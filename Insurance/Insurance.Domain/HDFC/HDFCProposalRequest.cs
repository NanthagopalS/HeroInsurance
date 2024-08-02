using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCProposalRequest
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
        public string panNumber { get; set; }
        public string companyName { get; set; }
        public string gstNo { get; set; }
    }

    public class Addressdetails
    {
        public string perm_address { get; set; }
        public string perm_state { get; set; }
        public string perm_city { get; set; }
        public string perm_pincode { get; set; }
        public string perm_mail_addCheck { get; set; }
        public string mail_address { get; set; }
        public string mail_state { get; set; }
        public string mail_city { get; set; }
        public string mail_pincode { get; set; }
    }
    public class Nomineedetails
    {
        public string nomineeName { get; set; }
        public string nomineeAge { get; set; }
        public string nomineeRelation { get; set; }
    }
    public class Vehicledetails
    {
        public string engineNumber { get; set; }
        public string chassisNumber { get; set; }
        public string isFinancier { get; set; }
        public string financer { get; set; }
        public string branch { get; set; }
        public string agreementType { get; set; }
        public string vehicleNumber { get; set; }
    }

}
