using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class ExamParticularDetail 
    {
        public string? Id { get; set; }
		public string? ExamId { get; set; }
		public int? QuestionNo { get; set; }
        public string? ExamModuleType { get; set; }
		public string? QuestionId { get; set; }
		public string? QuestionValue { get; set; }
        public string? StatusId { get; set; }
        public string StatusValue { get; set; }
        public string? AnswerOptionId { get; set; }
        public string? OptionIndex { get; set; }
        public string? OptionValue { get; set; }
    }
    public class OptionResponse
    {
        public string? Id { get; set;}
        public int? OptionIndex { get; set; }
        public string? OptionValue { get; set; }
    }
    public class ExamParticularQuestionDetailResponseModel
    {
        public IEnumerable<ExamParticularDetail>? ExamParticularDetailslist { get; set; }
        public IEnumerable<OptionResponse>? OptionResponsesList { get; set; }

    }
}
