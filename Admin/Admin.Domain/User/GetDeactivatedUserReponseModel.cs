using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{

    public class GetDeactivatedUserReponseModel
    {
        public IEnumerable<POSPUserDetail> POSPUserDetail { get; set; }
        public int TotalRecord { get; set; }
    }

    public class POSPUserDetail
    {

        public string UserId { get; set; }
        public string? POSPId { get; set; }
        public string? POSPName { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmailId { get; set; }
        public string? CreatedBy { get; set; }
        public string? StageValue { get; set; }
        public string? StatusValue { get; set; }
        public string? RelationManager { get; set; }
        public string? TaggedPolicy { get; set; }
        public bool IsActive { get; set; }
        public int TotalRecord { get; set; }
        public int SerialNumber { get; set; }
        

    }
}
