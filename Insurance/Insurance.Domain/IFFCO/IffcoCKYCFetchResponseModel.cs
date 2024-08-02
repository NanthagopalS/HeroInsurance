using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.IFFCO
{
    public class IffcoCKYCFetchResponseModel
    {
        public string status { get; set; }
        public IffcoResult result { get; set; }
    }
    public class IffcoResult
    {
        public string itgiUniqueReferenceId { get; set; }
        public string status { get; set; }
        public string prefix { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        public string formSixty { get; set; }
        public string dateofBirth { get; set; }
        public DateTime kycDateOfDeclaration { get; set; }
        public string minor { get; set; }
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
        public DateTime createdOn { get; set; }
    }
}
