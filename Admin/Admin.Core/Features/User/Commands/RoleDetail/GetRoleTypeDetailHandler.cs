using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Commands.UserRoleGetMapping
{
    public class GetRoleTypeDetail : IRequest<HeroResult<IEnumerable<RoleTypeDetailVM>>>
    {
        public string? RoleName { get; set; }
        public string? RoleTypeId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? PageIndex { get; set; }
    }
    public class GetRoleTypeDetailHandler : IRequestHandler<GetRoleTypeDetail, HeroResult<IEnumerable<RoleTypeDetailVM>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetRoleTypeDetailHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<RoleTypeDetailVM>>> Handle(GetRoleTypeDetail request, CancellationToken cancellationToken)
        {
            var roleTypeDetailMapInput = _mapper.Map<RoleTypeDetailInputModel>(request);
            var modelResult = await _userRepository.GetRoleTypeDetail(roleTypeDetailMapInput, cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listUserRoleModel = _mapper.Map<IEnumerable<RoleTypeDetailVM>>(modelResult);
                return HeroResult<IEnumerable<RoleTypeDetailVM>>.Success(listUserRoleModel);
            }
            return HeroResult<IEnumerable<RoleTypeDetailVM>>.Fail("No Record Found");
        }

    }
}
