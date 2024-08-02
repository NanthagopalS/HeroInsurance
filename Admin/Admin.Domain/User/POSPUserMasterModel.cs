using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class POSPUserMasterModel
    {
        public IEnumerable<BackgroundTypeMasterModel> BackgroundTypeMasterList { get; set; }
        public IEnumerable<InsurerCompanyMasterModel> InsurerCompanyMasterList { get; set; }
        public IEnumerable<POSPSourceTypeMasterModel> POSPSourceTypeMasterList { get; set; }
        public IEnumerable<PremiumRangeTypeMasterModel> PremiumRangeTypeMasterList { get; set; }
        public IEnumerable<CityModel> CityList { get; set; }  
        public IEnumerable<StateModel> StateList { get; set; }
        public IEnumerable<BankNameMasterModel> BankNameMasterList { get; set; }
        public IEnumerable<EducationQualificationMasterModel> EducationQualificationMasterList { get; set; }
        public IEnumerable<InsuranceProductsOfInterestModel> InsuranceProductsOfInterestMasterList { get; set; }

    }
}
