using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class GetExamParticularQuestionDetailModel
    {
        public string? OptionId { get; set; }
        public int? OptionIndex { get; set; }
        public string? OptionValue { get; set; }
        public bool? IsCorrectAnswer { get; set; }
        
    }
}
