using Admin.Domain.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class GetNotificationListResponseModel
    {
        public IEnumerable<NotificationListModel>? NotificationListModel { get; set; }
        public IEnumerable<NotificationPagingModel>? NotificationPagingModel { get; set; }
    }

    public class NotificationListModel 
    {
        public string? NotificationId { get; set; }
        public string? Title { get; set; }
        public string? RecipientType { get; set; }
        public string? AlertType { get; set; }
        public string? CreatedOn { get; set; }
        public string? IsPublished { get; set; }
        public string? Description { get; set; }
        public string? UserIds { get; set; }
    }
    public class NotificationPagingModel
    {
        public int? CurrentPageIndex { get; set; }
        public int? PreviousPageIndex { get; set; }
        public int? NextPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
        public int? TotalRecord { get; set; }
    }
}
