using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Commands.UserRoleGetMapping
{
    public class GetRoleLevelByBUIdforRoleCreation : IRequest<HeroResult<IEnumerable<GetRoleLevelByBUIdforRoleCreationVM>>>
    {
        public string? BUId { get; set; }
    }
    public class GetRoleLevelByBUIdforRoleCreationHandler : IRequestHandler<GetRoleLevelByBUIdforRoleCreation, HeroResult<IEnumerable<GetRoleLevelByBUIdforRoleCreationVM>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetRoleLevelByBUIdforRoleCreationHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<GetRoleLevelByBUIdforRoleCreationVM>>> Handle(GetRoleLevelByBUIdforRoleCreation request, CancellationToken cancellationToken)
        {
            var mapInput = _mapper.Map<GetRoleLevelByBUIdforRoleCreationInputModel>(request);
            var modelResult = await _userRepository.GetRoleLevelByBUIdforRoleCreation(mapInput, cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listUserRoleModel = _mapper.Map<IEnumerable<GetRoleLevelByBUIdforRoleCreationVM>>(modelResult);
                return HeroResult<IEnumerable<GetRoleLevelByBUIdforRoleCreationVM>>.Success(listUserRoleModel);
            }
            return HeroResult<IEnumerable<GetRoleLevelByBUIdforRoleCreationVM>>.Fail("No Record Found");
        }

    }
}
