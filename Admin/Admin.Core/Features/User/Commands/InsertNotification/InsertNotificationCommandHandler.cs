using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.RoleModulePermission;
using Admin.Domain.Roles;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.InsertNotification
{
    public record InsertNotificationCommand : IRequest<HeroResult<bool>>
    {
        public string? AlertTypeId { get; set; }
        public string? RecipientId { get; set; }
        public string? RecipientUserids { get; set; }
        public string? NotificationCategory { get; set; }
        public string? NotificationOrigin { get; set; }
        public string? NotificationTitle { get; set; }
        public string? Description { get; set; }
        public string? NotificationEventId { get; set; }
        public bool? IsPublished { get; set; }
    }
    public class InsertNotificationCommandHandler : IRequestHandler<InsertNotificationCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public InsertNotificationCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(InsertNotificationCommand command, CancellationToken cancellationToken)
        {
            var notificationModel = _mapper.Map<InsertNotificationResponseModel>(command);
            var result = await _userRepository.InsertNotification(command, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
