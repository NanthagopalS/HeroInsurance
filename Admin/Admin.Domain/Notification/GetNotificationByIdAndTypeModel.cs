using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Notification
{
    public class GetNotificationByIdAndTypeModel
    {
        public IEnumerable<NotificationListNew>? NotificationListNew { get; set; }
        public IEnumerable<NotificationListOld>? NotificationListOld { get; set; }

    }

    public class NotificationListNew
    {
        public string? UserId { get; set; }
        public string? NotificationBoradcastId { get; set; }
        public string? NotificationId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CreatedOn { get; set; }
        public string? NotificationCategory { get; set; }
        public string? IsSeen { get; set; }
        public string? NotificationAge { get; set; }

    }

    public class NotificationListOld
    {
        public string? UserId { get; set; }
        public string? NotificationBoradcastId { get; set; }
        public string? NotificationId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CreatedOn { get; set; }
        public string? NotificationCategory { get; set; }
        public string? IsSeen { get; set; }
        public string? NotificationAge { get; set; }

    }
}
