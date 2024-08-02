using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Commands.BUDetails;
using Identity.Core.Features.User.Commands.UserRoleSearch;
using Identity.Domain.Roles;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Commands.UserRoleGetMapping
{
    public class UserRoleMappingGetCommand : IRequest<HeroResult<IEnumerable<UserRoleGetVM>>>
    {
    
        public string EMPID { get; set; }
        public string RoleTypeName { get; set; }
        public string isActive { get; set; }
        public string CreatedFrom { get; set; }
        public string CreatedTo { get; set; }
    }
    public class UserRoleMappingGetCommandHandler : IRequestHandler<UserRoleMappingGetCommand, HeroResult<IEnumerable<UserRoleGetVM>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public UserRoleMappingGetCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<UserRoleGetVM>>> Handle(UserRoleMappingGetCommand request, CancellationToken cancellationToken)
        {
            var userRoleMapInput = _mapper.Map<RoleMappingInputModel>(request);
            var modelResult = await _userRepository.GetUserandRoleMapping(userRoleMapInput, cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listUserRoleModel = _mapper.Map<IEnumerable<UserRoleGetVM>>(modelResult);
                return HeroResult<IEnumerable<UserRoleGetVM>>.Success(listUserRoleModel);
            }
            return HeroResult<IEnumerable<UserRoleGetVM>>.Fail("No Record Found");
        }

    }
}
