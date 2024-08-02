using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.UserAddressDetail
{
    public class UserAddressDetailModel
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// AddressLine1
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// AddressLine2
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Pincode
        /// </summary>
        public string Pincode { get; set; }

        /// <summary>
        /// CityId
        /// </summary>
        public string CityId { get; set; }
        /// <summary>
        /// CityName
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// StateId
        /// </summary>
        public string StateId { get; set; }       

        /// <summary>
        /// StateName
        /// </summary>
        public string StateName { get; set; }


    }
}
