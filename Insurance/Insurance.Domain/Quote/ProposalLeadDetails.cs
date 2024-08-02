using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class ProposalLeadDetails
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string GSTNo { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }       
        public string NomineeFirstName { get; set; }
        public string NomineeLastName { get; set; }
        public string NomineeDOB { get; set; }
        public string NomineeRelation { get; set; }
        public string NomineeAge { get; set; }
        public string AddressType { get; set; }
        public string Perm_Address1 { get; set; }
        public string Perm_Address2 { get; set; }
        public string Perm_Address3 { get; set; }
        public string Perm_Pincode { get; set; }
        public string Perm_City { get; set; }
        public string Perm_State { get; set; }
        public string IsFinancier { get; set; }
        public string FinancierName { get; set; }
        public string FinancierBranch { get; set; }
        public string EngineNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string PanNumber { get; set; }
        public string VehicleColour { get; set; }
    }
}
