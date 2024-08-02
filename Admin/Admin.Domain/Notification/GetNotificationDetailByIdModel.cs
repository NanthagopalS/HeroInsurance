using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Notification
{
    public class GetNotificationDetailByIdModel
    {
        public string? NotificationId { get; set; }
        public string? Title { get; set; }
        public string? RecipientType { get; set; }
        public string? AlertType { get; set; }
        public string? CreatedOn { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
       
    }
}
