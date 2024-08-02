using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class PublishNotificationResponseModel
    {
        public string? NotificationId { get; set; }
        public string? AlertTypeId { get; set; }
        public string? RecipientUserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? MobileDeviceid { get; set; }
    }
}
