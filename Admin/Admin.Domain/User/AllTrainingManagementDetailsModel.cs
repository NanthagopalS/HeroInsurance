using Admin.Domain.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class AllTrainingManagementDetailsModel
    {
        public IEnumerable<AllTrainingManagementDetails>? AllTrainingManagementDetails { get; set; }
        public IEnumerable<TrainingManagementDetailsPagingModel>? TrainingManagementDetailsPagingModel { get; set; }
    }

    public class AllTrainingManagementDetails
    {

        public string? TrainingMaterialId { get; set; }
        public string? Category { get; set; }
        public string? DocumentType { get; set; }
        public string? Duration { get; set; }
        public string? LessionNumber { get; set; }
        public string? LessionTitle { get; set; }
        public string? Content { get; set; }
        public int? PriorityIndex { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedOn { get; set; }
       
    }
    public class TrainingManagementDetailsPagingModel
    {
        public int CurrentPageIndex { get; set; }
        public int PreviousPageIndex { get; set; }
        public int NextPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public int TotalRecord { get; set; }
    }
}
