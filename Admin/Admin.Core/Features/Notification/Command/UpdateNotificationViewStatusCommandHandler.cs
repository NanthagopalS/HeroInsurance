using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.TicketManagement.Command;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.Notification.Command
{
    public class UpdateNotificationViewStatusCommand : IRequest<HeroResult<bool>>
    {
        public string? NotificationBoradcastId { get; set; }
    }
    public class UpdateNotificationViewStatusCommandHandler : IRequestHandler<UpdateNotificationViewStatusCommand, HeroResult<bool>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public UpdateNotificationViewStatusCommandHandler(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(UpdateNotificationViewStatusCommand command, CancellationToken cancellationToken)
        {
            var result = await _notificationRepository.UpdateNotificationViewStatus(command.NotificationBoradcastId, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
