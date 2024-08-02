using Admin.Domain.Notification;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Contracts.Persistence
{
    public interface INotificationRepository
    {
        Task<IEnumerable<GetAdminAlertTypeModel>> GetAdminAlertType(CancellationToken cancellationToken);
        Task<IEnumerable<GetAdminRecipientTypeModel>> GetAdminRecipientType(CancellationToken cancellationToken);
        Task<IEnumerable<GetNotificationDetailByIdModel>> GetNotificationDetailById(string? NotificationId, CancellationToken cancellationToken);
        Task<IEnumerable<GetNotificationRecordByIdModel>> GetNotificationRecordById(string? NotificationId, CancellationToken cancellationToken);
        Task<GetNotificationByIdAndTypeModel> GetNotificationByIdAndType(string? UserId, string? NotificationCategory, CancellationToken cancellationToken);
        Task<bool> UpdateNotificationViewStatus(string? NotificationBoradcastId, CancellationToken cancellationToken);
    }
}
