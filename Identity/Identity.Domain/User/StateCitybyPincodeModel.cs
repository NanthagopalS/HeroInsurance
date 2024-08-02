using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class StateCitybyPincodeModel
    {
        public IEnumerable<StateModel> StateList { get; set; }
        public IEnumerable<CityModel> CityList { get; set; }

    }
}
