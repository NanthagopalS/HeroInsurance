using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class InsertNotificationResponseModel
    {
        public string? AlertTypeId { get; set; }
        public string? RecipientId { get; set; }
        public string? RecipientUserids { get; set; }
        public string? NotificationCategory { get; set; }
        public string? NotificationOrigin { get; set; }
        public string? NotificationTitle { get; set; }
        public string? Description { get; set; }
        public string? NotificationEventId { get; set; }
        public bool? IsPublished { get; set; }
    }
}
