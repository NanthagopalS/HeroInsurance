using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class GetCustomersDetailResponseModel
    {
        public IEnumerable<GetCustomersDetailModel>? GetCustomersDetailModel { get; set; }
        public IEnumerable<CustomersDetailPagingModel>? CustomersDetailPagingModel { get; set; }
    }

    public class GetCustomersDetailModel
    {
        public string? UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public string? PolicyNo { get; set; }
        public string? PolicyType { get; set; }
        public string? PolicyIssueDate { get; set; }
        public string? Premium { get; set; }
        public string? GeneratedOn { get; set; }
        public string? ExpiringOn { get; set; }
        public string? RequestStatus { get; set; }
        public string? CreatedBy { get; set; }

    }

    public class CustomersDetailPagingModel
    {
        public int CurrentPageIndex { get; set; }
        public int PreviousPageIndex { get; set; }
        public int NextPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public int TotalRecord { get; set; }
    }

}
