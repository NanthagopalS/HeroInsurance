using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.HelpAndSupport
{
    public class GetAllHelpAndSupportResponseModel
    {
        public IEnumerable<GetAllHelpAndSupportModel>? GetAllHelpAndSupportModel { get; set; }
        public IEnumerable<GetAllHelpAndSupportPagingModel>? GetAllHelpAndSupportPagingModel { get; set; }
    }

    public class GetAllHelpAndSupportModel
    {
        public string? Id { get; set; }
        public string? ConcernTypeId { get; set; }
        public string? ConcernType { get; set; }
        public string? SubConcernTypeId { get; set; }
        public string? SubConcernType { get; set; }
        public string? SubjectText { get; set; }
        public string? DetailText { get; set; }
        public string? Status { get; set; }
        public int? SerialNumber { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedOn { get; set; }
        public string? CreatedOnDate { get; set; }
        public string? CreatedOnTime { get; set; }

    }
    public class GetAllHelpAndSupportPagingModel
    {
        public int CurrentPageIndex { get; set; }
        public int PreviousPageIndex { get; set; }
        public int NextPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public int TotalRecord { get; set; }
    }
}
