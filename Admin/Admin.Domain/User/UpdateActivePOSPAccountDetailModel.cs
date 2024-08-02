using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class UpdateActivePOSPAccountDetailModel
    {
        public string? POSPUserId { get; set; }
        public string? PreSaleUserId { get; set; }
        public string? PostSaleUserId { get; set; }
        public string? MarketingUserId { get; set; }
        public string? ClaimUserId { get; set; }
        public string? SourcedBy { get; set; }
        public string? CreatedBy { get; set; }
        public string? ServicedBy { get; set; }

    }
}
