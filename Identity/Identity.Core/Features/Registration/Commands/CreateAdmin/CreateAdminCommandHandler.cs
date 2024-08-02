using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using Identity.Domain.User;
using MediatR;
using ThirdPartyUtilities.Abstraction;

namespace Identity.Core.Features.Registration.Commands.CreateAdmin
{
    public record CreateAdminCommand : IRequest<HeroResult<AdminVm>>
    {

        /// <summary>
        /// Email
        /// </summary>
        public string EmailId { get; set; }

        /// <summary>
        /// Mobile No
        /// </summary>
        public string PassWord { get; set; }
    }
    public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, HeroResult<AdminVm>>
    {
        private readonly IUserRepository _userCreationRepository;
        private readonly IMapper _mapper;
        private readonly ICustomUtility _utility;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userCreationRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CreateAdminCommandHandler(IUserRepository userCreationRepository, IMapper mapper, ICustomUtility utility)
        {
            _userCreationRepository = userCreationRepository ?? throw new ArgumentNullException(nameof(userCreationRepository));
            _mapper = mapper;
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<AdminVm>> Handle(CreateAdminCommand logCreationCommand, CancellationToken cancellationToken)
        {
            logCreationCommand.EmailId = _utility.Base64DecodeForRequest(logCreationCommand.EmailId);
            logCreationCommand.PassWord = _utility.Base64DecodeForRequest(logCreationCommand.PassWord);
            //user model with decrypted Email and Password
            var userCreationModel = _mapper.Map<AdminUserModel>(logCreationCommand);
            if (string.IsNullOrWhiteSpace(userCreationModel.EmailId) || string.IsNullOrWhiteSpace(userCreationModel.PassWord))
            {
                return HeroResult<AdminVm>.Fail("Invalid Credentials");
            }
            var result = await _userCreationRepository.ValidateAdminLogin(userCreationModel, cancellationToken);
            if (result is not null)
            {
                var adminCreationModel = _mapper.Map<AdminVm>(result);
                return HeroResult<AdminVm>.Success(adminCreationModel);
            }
            return HeroResult<AdminVm>.Fail("Invalid Credentials");
        }
    }
}
