using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using Identity.Domain.User;
using MediatR;
namespace Identity.Core.Features.Registration.Commands.AuthenticateAdmin
{
    public record AuthenticateAdminCommand : IRequest<HeroResult<AuthenticateVM>>
    {
        /// <summary>
        /// Email
        /// </summary>
        public string EmailId { get; set; }

        public string Password { get; set; }

    }
    public record AuthenticateAdminCommandHandler : IRequestHandler<AuthenticateAdminCommand, HeroResult<AuthenticateVM>>
    {
        private readonly IAuthenticateRepository _authenticateRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="authCreationRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AuthenticateAdminCommandHandler(IAuthenticateRepository authCreationRepository, IMapper mapper)
        {
            _authenticateRepository = authCreationRepository ?? throw new ArgumentNullException(nameof(authCreationRepository));
            _mapper = mapper;
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<AuthenticateVM>> Handle(AuthenticateAdminCommand authenticateCommand, CancellationToken cancellationToken)
        {
            var authCreationModel = _mapper.Map<AdminEmailModel>(authenticateCommand);

            var result = await _authenticateRepository.AuthenticateAdminUser(authCreationModel.EmailId, authCreationModel.Password, cancellationToken);
            if (result != null)
            {
                var adminCreationModel = _mapper.Map<AuthenticateVM>(result);
                return HeroResult<AuthenticateVM>.Success(adminCreationModel);
            }
            return HeroResult<AuthenticateVM>.Fail("Invalid Credentials");
        }
    }
}
