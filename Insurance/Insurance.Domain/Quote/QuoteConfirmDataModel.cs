using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class QuoteConfirmDataModel : QuoteConfirmRequestModel
    {
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string CommonResponse { get; set; }
        public QuoteConfirmRequestModel ConfirmCommand  { get; set; }
        public string Stage { get; set; }
        public string LeadId { get; set; }
        public decimal? MaxIDV { get; set; }
        public decimal? MinIDV { get; set; }
        public decimal? RecommendedIDV { get; set; }
        public string TransactionId { get; set; }
        public string UserId { get; set; }
        public bool IsPolicyExpired { get; set; }
        public string ProposalId { get; set; }
        public string PolicyId { get; set; }
        public string ResponseReferanceFlag { get; set; }
    }
}
