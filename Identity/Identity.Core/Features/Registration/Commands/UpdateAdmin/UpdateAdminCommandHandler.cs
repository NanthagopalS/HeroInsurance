using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using Identity.Domain.User;
using MediatR;

namespace Identity.Core.Features.Registration.Commands.UpdateAdmin
{
    public record UpdateAdminCommand : IRequest<HeroResult<AdminUpdateVM>>
    {
        public string UserId { get; set; }
        public string NewPassWord { get; set; }
        public string ConfirmPassWord { get; set; }
        public string OldPassWord { get; set; }
    }
    public class UpdateAdminCommandHandler : IRequestHandler<UpdateAdminCommand, HeroResult<AdminUpdateVM>>
    {
        private readonly IUserRepository _userCreationRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userCreationRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateAdminCommandHandler(IUserRepository userCreationRepository, IMapper mapper)
        {
            _userCreationRepository = userCreationRepository ?? throw new ArgumentNullException(nameof(userCreationRepository));
            _mapper = mapper;
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<AdminUpdateVM>> Handle(UpdateAdminCommand logUpdateCommand, CancellationToken cancellationToken)
        {
            var loggerUpdateModel = _mapper.Map<AdminUpdateUserModel>(logUpdateCommand);
            var result = await _userCreationRepository.UpdateAdminUserPasswordSelf(loggerUpdateModel, cancellationToken);
            if (result != null)
            {
                var updateUpdateModel = _mapper.Map<AdminUpdateVM>(result);
                return HeroResult<AdminUpdateVM>.Success(updateUpdateModel);
            }
            return HeroResult<AdminUpdateVM>.Fail("Failed to change password");
        }
    }
}

