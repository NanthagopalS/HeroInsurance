using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class ExamBulkUploadModel
    {
        public string? SrNo { get; set; }
        public int? SequenceNo { get; set; }
        public string? ExamModuleType { get; set; }
        public string? QuestionValue { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public List<ExamBulkUploadCommandInsertModel>? ExamBulkUploadCommandInsertModel { get; set; }
    }
    public record ExamBulkUploadCommandInsertModel
    {
        public string? SrNo { get; set; }
        public string? QuestionId { get; set; }
        public int? OptionIndex { get; set; }
        public string? OptionValue { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public bool IsActive { get; set; }

    }
   
}
