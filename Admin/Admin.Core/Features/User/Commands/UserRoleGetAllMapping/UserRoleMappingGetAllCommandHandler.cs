using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetModel;
using Admin.Core.Features.User.Queries.GetRoleBULevel;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Commands.UserRoleGetAllMapping
{
    public record UserRoleMappingGetAllCommand : IRequest<HeroResult<IEnumerable<UserRoleGetAllVM>>>
    {
    }
    public class UserRoleMappingGetAllCommandHandler : IRequestHandler<UserRoleMappingGetAllCommand, HeroResult<IEnumerable<UserRoleGetAllVM>>>
    {


        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public UserRoleMappingGetAllCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<UserRoleGetAllVM>>> Handle(UserRoleMappingGetAllCommand request, CancellationToken cancellationToken)
        {
            var roleResult = await _userRepository.GetUserandRoleMappingAll(cancellationToken).ConfigureAwait(false);
            if (roleResult.Any())
            {
                var listUserModel = _mapper.Map<IEnumerable<UserRoleGetAllVM>>(roleResult);
                return HeroResult<IEnumerable<UserRoleGetAllVM>>.Success(listUserModel);
            }
            return HeroResult<IEnumerable<UserRoleGetAllVM>>.Fail("No Record Found");
        }
    }
}
