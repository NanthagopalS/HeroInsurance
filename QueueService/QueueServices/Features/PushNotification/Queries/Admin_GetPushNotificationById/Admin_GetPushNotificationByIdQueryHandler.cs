using MediatR;
using QueueServices.Contracts.Persistence;
using QueueServices.Models.Queue;

namespace QueueServices.Features.PushNotification.Queries.Admin_GetPushNotificationById;

public record Admin_GetPushNotificationByIdQuery: IRequest<IEnumerable<Admin_Admin_GetPushNotificationByIdVm>>
{
    public string? NotificationId { get; set; }
}
public class Admin_GetPushNotificationByIdQueryHandler
{
    private readonly IQueueRepository _queueRepository;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="pospRepository"></param>
    /// <param name="mapper"></param>
    public Admin_GetPushNotificationByIdQueryHandler(IQueueRepository queueRepository)
    {
        _queueRepository = queueRepository;
    }

    public async Task<Admin_GetPushNotificationByIdResponseModel> Handle(Admin_GetPushNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var modelResult = await _queueRepository.GetPushNotificationbyId(request.NotificationId,cancellationToken).ConfigureAwait(false);
        
        return modelResult;
    }
}
