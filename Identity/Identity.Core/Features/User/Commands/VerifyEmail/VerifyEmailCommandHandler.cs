using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Commands.UserEmailId;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Commands.VerifyEmail
{
    public record VerifyEmailCommand : IRequest<HeroResult<VerifyEmailResponseModel>>
    {
        public string UserId { get; set; }

        /// <summary>
        /// Email Id
        /// </summary>
        public string EmailId { get; set; }
    }

    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, HeroResult<VerifyEmailResponseModel>>
    {
        private readonly IUserRepository _userEmailIdRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userPersonalDetailRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public VerifyEmailCommandHandler(IUserRepository userEmailIdRepository, IMapper mapper)
        {
            _userEmailIdRepository = userEmailIdRepository ?? throw new ArgumentNullException(nameof(userEmailIdRepository));
            _mapper = mapper;
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<VerifyEmailResponseModel>> Handle(VerifyEmailCommand userEmailIdCommand, CancellationToken cancellationToken)
        {
            var userModel = _mapper.Map<VerifyEmailModel>(userEmailIdCommand);
            var result = await _userEmailIdRepository.VerfyIdentityAdminEmail(userModel.UserId, userModel.EmailId, cancellationToken);

            return HeroResult<VerifyEmailResponseModel>.Success(result);
        }
    }
}
