using Admin.Domain.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class AllPOSPDetailForIIBDashboardModel
    {

        public IEnumerable<AllPOSPDetailDataModel>? AllPOSPDetailDataModel { get; set; }
        public IEnumerable<AllPOSPDetailDataPaginationModel>? AllPOSPDetailDataPaginationModel { get; set; }

    }
    public class AllPOSPDetailDataModel
    {

        public string? POSPId { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? MobileNumber { get; set; }
        public string? PANNumber { get; set; }
        public string? CreatedByMode { get; set; }
        public string? IIBStatus { get; set; }
        public string? IIBUploadStatus { get; set; }

    }

    public class AllPOSPDetailDataPaginationModel
    {
        public int? CurrentPageIndex { get; set; }
        public int? PreviousPageIndex { get; set; }
        public int? NextPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
        public int? TotalRecord { get; set; }
    }
}
