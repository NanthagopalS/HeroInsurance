using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.InsertNotification;
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

namespace Admin.Core.Features.User.Commands.EditNotification
{
    public record EditNotificationCommand : IRequest<HeroResult<bool>>
    {
        public string? NotificationId { get; set; }
        public string? AlertTypeId { get; set; }
        public string? RecipientId { get; set; }
        public string? UserIds { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
    internal class EditNotificationCommandHandler : IRequestHandler<EditNotificationCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public EditNotificationCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(EditNotificationCommand command, CancellationToken cancellationToken)
        {
            var notificationModel = _mapper.Map<EditNotificationResponseModel>(command);
            var result = await _userRepository.EditNotification(command, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
