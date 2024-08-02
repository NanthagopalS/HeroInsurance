using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.TicketManagement
{
    public class GetDeactivationTicketManagementDetailResponseModel
    {
        public IEnumerable<DeActivateTicketManagementDetailModel>? DeActivateTicketManagementDetailModel { get; set; }
        public IEnumerable<DeActivateTicketManagementDetailPagingModel>? DeActivateTicketManagementDetailPagingModel { get; set; }
    }
    public class DeActivateTicketManagementDetailModel
    {
        public string? DeActivateTicketId { get; set; }
        public string? POSPId { get; set; }
        public string? POSPName { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string? RelationshipManager { get; set; }
        public string? Status { get; set; }
        public string? Remark { get; set; }
        public string? PolicyType { get; set; }
        public string? CreatedOn { get; set; }
    }

    public class DeActivateTicketManagementDetailPagingModel
    {
        public int? CurrentPageIndex { get; set; }
        public int? PreviousPageIndex { get; set; }
        public int? NextPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
        public int? TotalRecord { get; set; }
    }
}
