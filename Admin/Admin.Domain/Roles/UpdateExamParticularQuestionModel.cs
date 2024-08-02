using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class UpdateExamParticularQuestionModel
    {
        public string? QuestionId { get; set; }
        public string? OptionId1 { get; set; }
        public string? OptionValue1 { get; set; }
        public string? OptionId2 { get; set; }
        public string? OptionValue2 { get; set; }
        public string? OptionId3 { get; set; }
        public string? OptionValue3 { get; set; }
        public string? OptionId4 { get; set; }
        public string? OptionValue4 { get; set; }
        public int? CorrectAnswerIndex { get; set; }
    }
}
