using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.TicketManagement
{
    public class GetPOSPDetailsByIDToDeActivateResponseModel
    {
        public string? POSPId { get; set; }
        public string? POSPName { get; set; }
        public string? MobileNumber { get; set; }
        public string? Aadhar { get; set; }
        public string? Pan { get; set; }
        public string? Address { get; set; }
        public string? EmailAttachmentDocumentId { get; set; }
        public string? EmailAttachment { get; set; }
        public string? BusinessTeamApprovalAttachmentDocumentId { get; set; }
        public string? BusinessTeamApprovalAttachment { get; set; }
        public string? Status { get; set; }
        public string? Remark { get; set; }
        public string? PolicyType { get; set; }
        public string? EmailId { get; set; }
        public bool? IsNocGenerated { get; set; }
    }
}
