using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Oriental
{
    public  class OrientalCKYCFetchRequestModel
    {
        public string idNo { get; set; }
        public string dob { get; set; }
        public string pincode { get; set; } 
        public string mobileNo { get; set; }
        public string idType { get; set; }
        public string returnOnlySearchResponse { get; set; }    
        public string entityType { get; set; }  
    }
}
