namespace Admin.Domain.Notification
{
    public class GetNotificationRecordByIdModel
    {  
        public string? AlertType { get; set; }
        public string? IsInApp { get; set; }
        public string? IsWhatsApp { get; set; }
        public string? IsEmail { get; set; }
        public string? IsPush { get; set; }
        public string? IsSMS { get; set; }
        public string? NotificationId { get; set; }
        public string? AlertTypeId { get; set; }
        public string? NotificationTitle { get; set; }
        public string? Description { get; set; }
        public string? NotificationEventId { get; set; }
        public string? EventTitle { get; set; }
        public string? EventDescription { get; set; }
        public string? RecipientUserids { get; set; }

    }
}
