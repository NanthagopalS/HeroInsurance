using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.Registration.Commands.UpdateAdmin
{
    public record ResetPasswordCommand : IRequest<HeroResult<ResetPasswordVM>>
    {
        public string EmailId { get; set; }
        public string Environment { get; set; }
    }
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, HeroResult<ResetPasswordVM>>
    {
        private readonly IUserRepository _userCreationRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userCreationRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ResetPasswordCommandHandler(IUserRepository userCreationRepository, IMapper mapper)
        {
            _userCreationRepository = userCreationRepository ?? throw new ArgumentNullException(nameof(userCreationRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<ResetPasswordVM>> Handle(ResetPasswordCommand resetPassword, CancellationToken cancellationToken)
        {
            var resultResetPwd = await _userCreationRepository.ResetPassword(resetPassword.EmailId, resetPassword.Environment, cancellationToken);
            if (resultResetPwd is not null && resultResetPwd.IsValidate)
            {
                var mapModel = _mapper.Map<ResetPasswordVM>(resultResetPwd);
                return HeroResult<ResetPasswordVM>.Success(mapModel);
            }
            return HeroResult<ResetPasswordVM>.Fail(resultResetPwd.Message);
        }
    }
}

