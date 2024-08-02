using AutoMapper;
using MediatR;
using QueueServices.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueServices.Features.PushNotification.Commands.UpdatePushNotificationBroadcastStatus
{
    public class UpdatePushNotificationBroadcastStatusCommand 
    {
        public string NotificationId { get; set; }
        public string FirebaseQueueId { get; set; }
    }

    public class UpdatePushNotificationBroadcastStatusCommandHandler 
    {
        private readonly IQueueRepository _BURepository;

        public UpdatePushNotificationBroadcastStatusCommandHandler(IQueueRepository buRepository)
        {
            _BURepository = buRepository ?? throw new ArgumentNullException(nameof(buRepository));
        }
        public async Task<bool> Handle(UpdatePushNotificationBroadcastStatusCommand buInsertCommand, CancellationToken cancellationToken)
        {
            var result = await _BURepository.UpdatePushNotificationBroadcastStatus(buInsertCommand, cancellationToken);
            return result;
        }

    }

}
