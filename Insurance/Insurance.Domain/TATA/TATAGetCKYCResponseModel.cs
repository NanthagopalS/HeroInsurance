using Insurance.Domain.Quote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATAGetCKYCResponseModel
    {
        public CKYCStatusModel cKYCStatusModel { get; set; }
        public TATACKYCStatusResponseModel tATACKYCStatusResponseModel { get; set; }
    }
}
