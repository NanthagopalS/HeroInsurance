using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class AllBUDetailModel
    {
        public string? BUName { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? CurrentPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
        public bool? IsActive { get; set; }
    }

    public class AllBUDetailResponseModel
    {
        public IEnumerable<BUDetailsModel>? BUDetailsModel { get; set; }
        public IEnumerable<BUDetailsPagingModel>? BUDetailsPagingModel { get; set; }
    }

    public class BUDetailsModel
    {
        public string? BUId { get; set; }
        public string? BUName { get; set; }
        public string? HierarchyLevelId { get; set; }
        public string? HierarchyLevelName { get; set; }
        public string? RoleTypeName { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedOn { get; set; }
        public bool? IsSales { get; set; }
    }
    public class BUDetailsPagingModel
    {
        public int? CurrentPageIndex { get; set; }
        public int? PreviousPageIndex { get; set; }
        public int? NextPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
        public int? TotalRecord { get; set; }
    }
}
