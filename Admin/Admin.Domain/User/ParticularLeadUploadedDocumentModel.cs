using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class ParticularLeadUploadedDocumentModel
    {
        public string? UserId { get; set; }
        public string? DocumentTypeId { get; set; }
        public string? DocumentFilename { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentId { get; set; }
        public string? ShortDescription { get; set; }
        public string? CreatedOn { get; set; }
        public string? VerifyOn { get; set; }
        public string? IsVerify { get; set; }
        public string? IsApprove { get; set; }
        public bool? IsActive { get; set; }
        public string? VerifyByUserId { get; set; }
        public string? FileSize { get; set; }
        public string? BackOfficeRemark { get; set; }
    }
}
