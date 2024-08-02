using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueServices.Models.Queue
{
    public class Admin_GetPushNotificationByIdResponseModel
    {
        public string? RecipientUserids { get; set; }

        public string? NotificationId { get; set; }
        public string? AlertTypeId { get; set; }
        public string? NotificationCategory { get; set; }
        public string? NotificationOrigin { get; set; }

        public string? NotificationTitle { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
        public string? NotificationEventId { get; set; }
        public string? EventTitle { get; set; }
        public string? EventDescription { get; set; }
        public string? IsPush { get; set; }
    }
}
