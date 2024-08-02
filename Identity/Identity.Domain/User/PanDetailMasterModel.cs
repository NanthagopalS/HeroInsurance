using Identity.Domain.UserAddressDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class PanDetailMasterModel
    {
        public IEnumerable<UserModel> UserList { get; set; }
        public IEnumerable<UserDetailModel> UserDetailList { get; set; }
        public IEnumerable<InsuranceProductModel> InsuranceProductModel { get; set; }
        public IEnumerable<InsuranceCompanyModel> InsuranceCompanyModel { get; set; }
        public IEnumerable<PanDetailsModel> PanDetailsList { get; set; }
        public IEnumerable<UserAddressDetailModel> UserAddressDetailList { get; set; }
    }
}
