using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class ExamQuestionPaperMasterModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string? Id { get; set; }

        ///// <summary>
        ///// QuestionId
        ///// </summary>
        //public string? QuestionId { get; set; }


        /// <summary>
        /// QuestionValue
        /// </summary>
        public string? QuestionValue { get; set; }

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
}
