using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.InsuranceMaster
{
    public class InsuranceMasterDataModel
    {
        public string InsurerId { get; set; }
        public string InsurerName { get; set; }
        public string InsurerCode { get; set; }
        public string IsActive { get; set; }
        public string InsurerType { get; set; }
    }
}
