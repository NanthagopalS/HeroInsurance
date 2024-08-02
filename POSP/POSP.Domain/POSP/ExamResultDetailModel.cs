using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class ExamResultDetailModel
    {
        public string ExamStartDateTime { get; set; }
        public string ExamEndDateTime { get; set; }
        public string CorrectAnswered { get; set; }
        public string InCorrectAnswered { get; set; }
        public string SkippedAnswered { get; set; }
        public string FinalResult { get; set; }
        public string IsCleared { get; set; }
    }
}
