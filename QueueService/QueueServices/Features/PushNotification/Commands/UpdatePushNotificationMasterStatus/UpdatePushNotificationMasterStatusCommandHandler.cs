using QueueServices.Contracts.Persistence;

namespace QueueServices.Features.PushNotification.Commands.UpdatePushNotificationMasterStatus
{
    public class UpdatePushNotificationMasterStatusCommand
    {
        public string NotificationId { get; set; }
    }

    public class UpdatePushNotificationMasterStatusCommandHandler
    {
        private readonly IQueueRepository _BURepository;

        public UpdatePushNotificationMasterStatusCommandHandler(IQueueRepository buRepository)
        {
            _BURepository = buRepository ?? throw new ArgumentNullException(nameof(buRepository));
        }
        public async Task<bool> Handle(UpdatePushNotificationMasterStatusCommand buInsertCommand, CancellationToken cancellationToken)
        {
            var result = await _BURepository.UpdatePushNotificationMasterStatus(buInsertCommand, cancellationToken);
            return result;
        }

    }
}
