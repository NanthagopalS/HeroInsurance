using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.TicketManagement
{
    public class GetTicketManagementDetailByIdModel
    {
        public string? TicketId { get; set; }
        public string? POSPId { get; set; }
        public string? POSPName { get; set; }
        public string? ConcernType { get; set; }
        public string? SubconcernType { get; set; }
        public string? Status { get; set; }
        public string? Summary { get; set; }
        public string? SummaryDetails { get; set; }
        public string? Description { get; set; }
        public string[]? DocumentIdArray { get; set; }
        public string? DocumentId { get; set; }
        public List<string>? DocumentB64String { get; set; }

    }
}
