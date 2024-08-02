using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class ExamQuestionAsweredDetailModel
    {
        public string UserId { get; set; }
        public string ExamId { get; set; }
        public string QuestionNo { get; set; }
        public string QuestionId { get; set; }
        public string AnswerOptionId { get; set; }

    }
}
