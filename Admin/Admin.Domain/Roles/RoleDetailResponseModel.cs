using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class RoleDetailResponseModel
    {
        public IEnumerable<RoleDetailModel>? RoleDetailModel { get; set; }
        public IEnumerable<RoleDetailPagingModel>? RoleDetailPagingModel { get; set; }
    }

    public class RoleDetailModel
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleTypeId { get; set; }
        public string? RoleTypeName { get; set; }
        public string? RoleLevelId { get; set; }
        public string? RoleLevelName { get; set; }
        public int? LevelNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string? BUID { get; set; }
        public string? BUName { get; set; }
    }

    public class RoleDetailPagingModel
    {
        public int CurrentPageIndex { get; set; }
        public int PreviousPageIndex { get; set; }
        public int NextPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public int TotalRecord { get; set; }
    }
}

