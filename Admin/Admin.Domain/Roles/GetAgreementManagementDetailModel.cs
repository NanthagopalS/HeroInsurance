using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class GetAgreementManagementDetailModel
    {
        public IEnumerable<AgreementDetailListModel>? AgreementDetailListModel { get; set; }
        public IEnumerable<AgreementDetailPagingModel>? AgreementDetailPagingModel { get; set; }
    }

    public class AgreementDetailListModel
    {
        public string? UserId { get; set; }
        public string? StampId { get; set; }
        public string? StampData { get; set; }
        public string? POSPId { get; set; }
        public string? UserName { get; set; }
        public string? MobileNo { get; set; }
        public string? AgreementSignDate { get; set; }
        public string? AgreementStatus { get; set; }

    }

    public class AgreementDetailPagingModel
    {
        public int CurrentPageIndex { get; set; }
        public int PreviousPageIndex { get; set; }
        public int NextPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public int TotalRecord { get; set; }
    }
}
