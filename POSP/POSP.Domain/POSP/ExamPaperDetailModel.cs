using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class ExamPaperDetailModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// ExamId
        /// </summary>
        public string? ExamId { get; set; }


        /// <summary>
        /// QuestionNo
        /// </summary>
        public int? QuestionNo { get; set; }

        /// <summary>
        /// QuestionId
        /// </summary>
        public string? QuestionId { get; set; }


        /// <summary>
        /// StatusId
        /// </summary>
        public string? StatusId { get; set; }

        /// <summary>
        /// AnswerOptionId
        /// </summary>
        public string? AnswerOptionId { get; set; }

        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// CreatedBy
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// UpdatedBy
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// UpdatedOn
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }

    public class ExamPaperDetailResponseModel
    {

        /// <summary>
        /// QuestionNo
        /// </summary>
        public int? QuestionNo { get; set; }

        /// <summary>
        /// QuestionId
        /// </summary>
        public string? QuestionId { get; set; }


        /// <summary>
        /// StatusId
        /// </summary>
        public string? StatusId { get; set; }

        /// <summary>
        /// StatusValue
        /// </summary>
        public string? StatusValue { get; set; }
    }
}
