using QueueServices.Features.PushNotification.Commands.UpdatePushNotificationBroadcastStatus;
using QueueServices.Features.PushNotification.Commands.UpdatePushNotificationMasterStatus;
using QueueServices.Models.Queue;
using QueueServices.Models.Quque;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueServices.Contracts.Persistence
{
    public interface IQueueRepository
    {
        Task<Admin_GetPushNotificationByIdResponseModel> GetPushNotificationbyId(string? NotificationId, CancellationToken cancellationToken);
        Task<IEnumerable<Admin_GetPushNotificationBroadcastByIdResponseModel>> Admin_GetBroadcastNotificationDetailById(string? NotificationId, CancellationToken cancellationToken);

        Task<bool> UpdatePushNotificationBroadcastStatus(UpdatePushNotificationBroadcastStatusCommand updateBroadcastStatusCommand,CancellationToken ctoken);
        Task<bool> UpdatePushNotificationMasterStatus(UpdatePushNotificationMasterStatusCommand updateMasterStatusCommand,CancellationToken ctoken);
        Task<bool> InsertTriggerNotification(string? AlertTypeId, string? RecipientId, string? RecipientUserids,string? EventId, CancellationToken cancellationToken);

    }
}
