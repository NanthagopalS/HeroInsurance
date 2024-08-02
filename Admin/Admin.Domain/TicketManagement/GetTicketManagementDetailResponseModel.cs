using Admin.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.TicketManagement
{
    public class GetTicketManagementDetailResponseModel
    {
        public IEnumerable<TicketManagementDetailModel>? TicketManagementDetailModel { get; set; }
        public IEnumerable<TicketManagementDetailPagingModel>? TicketManagementDetailPagingModel { get; set; }
    }
    public class TicketManagementDetailModel
    {
        public string? TicketId { get; set; }
        public string? POSPId { get; set; }
        public string? POSPName { get; set; }
        public string? MobileNumber { get; set; }
        public string? RelationshipManager { get; set; }
        public string? ConcernType { get; set; }
        public string? Status { get; set; }
        public string? CreatedOn { get; set; }
        public int? TicketAge { get; set; }
    }

    public class TicketManagementDetailPagingModel
    {
        public int? CurrentPageIndex { get; set; }
        public int? PreviousPageIndex { get; set; }
        public int? NextPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
        public int? TotalRecord { get; set; }
    }
}
