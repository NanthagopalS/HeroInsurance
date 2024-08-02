using System;

namespace QueueServices.Models.Quque
{
    public class Admin_GetPushNotificationBroadcastByIdResponseModel
    {
        public string? NotificationBoradcastId { get; set; }
        public string? Title { get; set; }
        public string? RecipientUserId { get; set; }
        public string? Description { get; set; }
        public string? BrowserId { get; set; }
        public string? MobileDeviceId { get; set; }

    }
}
