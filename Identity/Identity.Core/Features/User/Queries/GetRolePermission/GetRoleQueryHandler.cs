using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Commands.RoleModulePermission;
using Identity.Core.Features.User.Queries.GetModel;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Queries.GetRolePermission
{
    public class GetRoleQueryCommand : IRequest<HeroResult<IEnumerable<RoleSearchVM>>>
    {
        public string RoleTitleName { get; set; }
        public string RoleTypeName { get; set; }
        public string CreatedFrom { get; set; }
        public string CreatedTo { get; set; }
    }
    public class GetRoleQueryHandler : IRequestHandler<GetRoleQueryCommand, HeroResult<IEnumerable<RoleSearchVM>>>
    {
        public GetRoleQueryHandler() { }
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetRoleQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<RoleSearchVM>>> Handle(GetRoleQueryCommand request, CancellationToken cancellationToken)
        {
            var roleSearchInput = _mapper.Map<RoleSearchInputModel>(request);
            var modelResult = await _userRepository.GetPermissionMapping(roleSearchInput, cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listInsurerModel = _mapper.Map<IEnumerable<RoleSearchVM>>(modelResult);
                return HeroResult<IEnumerable<RoleSearchVM>>.Success(listInsurerModel);
            }
            return HeroResult<IEnumerable<RoleSearchVM>>.Fail("No Record Found");
        }
    }
}
