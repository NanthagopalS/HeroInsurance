using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class POSPRatingModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class POSPRatingResponseModel
    {
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
    }

    public class POSPRatingResponseGetModel
    {
        public bool IsPOSPRatingAvailable { get; set; }
        
    }
}
