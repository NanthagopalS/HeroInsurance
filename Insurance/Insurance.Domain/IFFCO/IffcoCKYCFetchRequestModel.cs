using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.IFFCO
{
    public class IffcoCKYCFetchRequestModel
    {
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string dateofBirth { get; set; }
        public string gender { get; set; }
        public string idType { get; set; }
        public string idNumber { get; set; }
        public string clientType { get; set; }
    }
}
