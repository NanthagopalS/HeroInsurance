using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Reliance
{
    public class RelianceCkycFetchModel
    {
        public bool success { get; set; }
        public Datas data { get; set; }
    }

    public class Datas
    {
        public int iskycVerified { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string dob { get; set; }
        public string permanentAddress { get; set; }
        public string permanentAddress1 { get; set; }
        public string permanentAddress2 { get; set; }
        public string permanentAddress3 { get; set; }
        public string permanentCity { get; set; }
        public string permanentPincode { get; set; }
        public string correspondenceAddress { get; set; }
        public string correspondenceAddress1 { get; set; }
        public string correspondenceAddress2 { get; set; }
        public string correspondenceAddress3 { get; set; }
        public string correspondenceCity { get; set; }
        public string correspondencePincode { get; set; }
        public string email { get; set; }
        public string rejectionReason { get; set; }
        public string pan { get; set; }
        public string kyc_id { get; set; }
        public string status { get; set; }
        public bool form60 { get; set; }
        public string ckycNumber { get; set; }
        public string redirect_link { get; set; }
    }

}
