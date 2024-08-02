using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class DocumentTypeModel
    {
        public string UserId { get; set; }
        public string DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public bool IsMandatory { get; set; }
        public string DocumentFileName { get; set; }
        public bool IsVerify { get; set; }
        public bool IsApprove { get; set; }

    }
}
