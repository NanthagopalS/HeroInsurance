using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Commands.UpdateUserPasswordFromUserLinkCommand
{
    public record UpdateUserPasswordFromUserLinkCommand : IRequest<HeroResult<UpdateUserPasswordFromUserLinkVm>>
    {
        public string UserId { get; set; }
        public string NewPassWord { get; set; }
        public string ConfirmPassWord { get; set; }
        public string Guid { get; set; }
    }
    public class UpdateUserPasswordFromUserLinkModalCommand : IRequestHandler<UpdateUserPasswordFromUserLinkCommand, HeroResult<UpdateUserPasswordFromUserLinkVm>>
    {
        private readonly IUserRepository _userCreationRepository;
        private readonly IMapper _mapper;

        public UpdateUserPasswordFromUserLinkModalCommand(IUserRepository userCreationRepository, IMapper mapper)
        {
            _userCreationRepository = userCreationRepository ?? throw new ArgumentNullException(nameof(userCreationRepository));
            _mapper = mapper;
        }
        public async Task<HeroResult<UpdateUserPasswordFromUserLinkVm>> Handle(UpdateUserPasswordFromUserLinkCommand resetPassword, CancellationToken cancellationToken)
        {
            var resultResetPwd = await _userCreationRepository.UpdateUserPasswordFromUserLink(resetPassword, cancellationToken);
            if (resultResetPwd != null)
            {
                var responceModel = _mapper.Map<UpdateUserPasswordFromUserLinkVm>(resultResetPwd);
                return HeroResult<UpdateUserPasswordFromUserLinkVm>.Success(responceModel);
            }
            return HeroResult<UpdateUserPasswordFromUserLinkVm>.Fail("Failed to resetting password");
        }
    }
}
