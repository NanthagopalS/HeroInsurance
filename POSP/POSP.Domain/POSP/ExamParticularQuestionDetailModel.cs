using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class ExamParticularQuestionDetailModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// ExamId
        /// </summary>
        public string? ExamId { get; set; }


        /// <summary>
        /// QuestionNo
        /// </summary>
        public int? QuestionNo { get; set; }
    }
}
