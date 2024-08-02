using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class ParticularHelpAndSupportModel
    {
        public string? ConcernTypeId { get; set; }
        public string? SubConcernTypeId { get; set; }
        public string? SubjectText { get; set; }
        public string? DetailText { get; set; }
        public string? Description { get; set; }
        public string? DocumentId { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }
        public string? ConcernTypeName { get; set; }
        public string? SubConcernTypeName { get; set; }
        public string? CreatedOn { get; set; }
        public string? CreatedOnDate { get; set; }
        public string? CreatedOnTime { get; set; }

        public string[]? DocumentIdArray { get; set; }
        public List<string>? DocumentB64String { get; set; }
    }
}
