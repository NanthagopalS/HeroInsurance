using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.Registration.Commands.UpdateAdmin
{
    public record LogoutCommand : IRequest<HeroResult<LogoutVM>>
    {
        public string UserId { get; set; }
    }
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, HeroResult<LogoutVM>>
    {
        private readonly IUserRepository _userCreationRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userCreationRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public LogoutCommandHandler(IUserRepository userCreationRepository, IMapper mapper)
        {
            _userCreationRepository = userCreationRepository ?? throw new ArgumentNullException(nameof(userCreationRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<LogoutVM>> Handle(LogoutCommand logoutCommand, CancellationToken cancellationToken)
        {
            var resultLogout = await _userCreationRepository.Logout(logoutCommand.UserId, cancellationToken);
            if (resultLogout != null)
            {
                var mapModel = _mapper.Map<LogoutVM>(resultLogout);
                return HeroResult<LogoutVM>.Success(mapModel);
            }
            return HeroResult<LogoutVM>.Fail("Failed to logout");
        }
    }
}

