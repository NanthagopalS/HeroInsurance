using Admin.Domain.UserAddressDetail;
using Admin.Domain.UserBankDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class UserProfileDetailModel
    {
        public IEnumerable<UserModel> UserList { get; set; }
        public IEnumerable<UserDetailModel> UserDetailList { get; set; }
        public IEnumerable<UserBankDetailModel> UserBankDetailList { get; set; }
        public IEnumerable<UserAddressDetailModel> UserAddressDetailList { get; set; }
        public IEnumerable<DocumentTypeModel> DocumentTypeList { get; set; }     
    }
}
