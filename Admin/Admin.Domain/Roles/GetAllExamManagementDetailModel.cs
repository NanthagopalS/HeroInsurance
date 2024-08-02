using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class GetAllExamManagementDetailModel
    {
        public IEnumerable<ExamManagementDetailModel>? ExamManagementDetailModel { get; set; }
        public IEnumerable<ExamManagementDetailPagingModel>? ExamManagementDetailPagingModel { get; set; }

    }
    public class ExamManagementDetailModel
    {
        public string? QuestionId { get; set; }
        public string? QuestionValue { get; set; }
        public string? Category { get; set; }
        public int? QuestionIndex { get; set; }
        public bool? IsActive { get; set; }
    }
    public class ExamManagementDetailPagingModel
    {
        public int CurrentPageIndex { get; set; }
        public int PreviousPageIndex { get; set; }
        public int NextPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public int TotalRecord { get; set; }
    }
}
