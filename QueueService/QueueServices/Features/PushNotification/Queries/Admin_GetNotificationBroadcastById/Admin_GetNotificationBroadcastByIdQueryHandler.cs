using AutoMapper;
using MediatR;
using QueueServices.Contracts.Persistence;
using QueueServices.Models.Queue;
using QueueServices.Models.Quque;

namespace QueueServices.Features.PushNotification.Queries.Admin_GetNotificationBroadcastById;

public record Admin_GetNotificationBroadcastByIdQuery : IRequest<IEnumerable<Admin_Admin_GetPushNotificationByIdVm>>
{
    public string? NotificationId { get; set; }
}
public class Admin_GetNotificationBroadcastByIdQueryHandler 
{
    private readonly IQueueRepository _queueRepository;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="pospRepository"></param>
    /// <param name="mapper"></param>
    public Admin_GetNotificationBroadcastByIdQueryHandler(IQueueRepository queueRepository)
    {
        _queueRepository = queueRepository;
    }

    public async Task<IEnumerable<Admin_GetPushNotificationBroadcastByIdResponseModel>> Handle(Admin_GetNotificationBroadcastByIdQuery request, CancellationToken cancellationToken)
    {
        var modelResult = await _queueRepository.Admin_GetBroadcastNotificationDetailById(request.NotificationId,cancellationToken).ConfigureAwait(false);
        
        return modelResult;
    }
}
