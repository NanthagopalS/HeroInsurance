using Admin.Core.Contracts.Persistence;
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

namespace Admin.Core.Features.User.Commands.PublishNotification
{
    public class PublishNotificationCommand : IRequest<HeroResult<bool>>
    {
        public string? NotificationId { get; set; }
    }
    public class PublishNotificationCommandHandler : IRequestHandler<PublishNotificationCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public PublishNotificationCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(PublishNotificationCommand command, CancellationToken cancellationToken)
        {
            var publishModel = _mapper.Map<PublishNotificationResponseModel>(command);
            var result = await _userRepository.PublishNotification(command, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
