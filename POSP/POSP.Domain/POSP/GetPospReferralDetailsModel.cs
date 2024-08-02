using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{

    public class GetPospReferralDetailsModel
    {
        public IEnumerable<ReferralTypeModel>? ReferralTypeModel { get; set; }
        public IEnumerable<RefferalMode>? RefferalMode { get; set; }
        public IEnumerable<RefferalDetails>? RefferalDetails { get; set; }

    }

    public class ReferralTypeModel
    {
        public string? RefererralTypeId { get; set; }
        public string? RefererralType { get; set; }
        public string? ImageURL { get; set; }
        public string? ReferralBaseURL { get; set; }
        public string? PriorityIndex { get; set; }
       
    }

    public class RefferalMode
    {
        public string? ReferralModeId { get; set; }
        public string? ReferralModeType { get; set; }
        public string? ImageUrl { get; set; }
        public string? PriorityIndex { get; set; }
    }

    public class RefferalDetails
    {
        public string? ReferralId { get; set; }
        
    }
}


