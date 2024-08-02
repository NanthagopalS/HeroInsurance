using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class GetLeadManagementDetailModel
    {
        public IEnumerable<LeadDetailModelList>? LeadDetailModelList { get; set; }
        public IEnumerable<LeadDetailPagingModel>? LeadDetailPagingModel { get; set; }
    }
    public class LeadDetailModelList
    {
        public string? UserId { get; set; }
        public string? POSPId { get; set; }
        public string? LeadId { get; set; }
        public string? CustomerName { get; set; }
        public string? StageValue { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public string? PolicyType { get; set; }
        public string? GeneratedOn { get; set; }
        public string? ExpiringOn { get; set; }
        public string? Product { get; set; }
        public string? PolicyStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? CreatedBy { get; set; }
        public double? Amount { get; set; }
        public string? StageId { get; set; }
    }

    public class LeadDetailPagingModel
    {
        public int CurrentPageIndex { get; set; }
        public int PreviousPageIndex { get; set; }
        public int NextPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public int TotalRecord { get; set; }
    }
}
