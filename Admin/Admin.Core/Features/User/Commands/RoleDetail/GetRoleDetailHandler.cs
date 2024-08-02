using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Commands.UserRoleGetMapping
{
    public class GetRoleDetail : IRequest<HeroResult<RoleDetailVM>>
    {
        public string? RoleName { get; set; }
        public string? RoleTypeId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? CurrentPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
        public bool? IsActive { get; set; }
        public IEnumerable<RoleDetailModel>? RoleDetailModel { get; set; }
        public IEnumerable<RoleDetailPagingModel>? RoleDetailPagingModel { get; set; }
    }
    public class GetRoleDetailHandler : IRequestHandler<GetRoleDetail, HeroResult<RoleDetailVM>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetRoleDetailHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<RoleDetailVM>> Handle(GetRoleDetail request, CancellationToken cancellationToken)
        {
            var roleDetailMapInput = _mapper.Map<RoleDetailInputModel>(request);
            var modelResult = await _userRepository.GetRoleDetail(roleDetailMapInput, cancellationToken).ConfigureAwait(false);
            if (modelResult != null)
            {
                var listUserRoleModel = _mapper.Map<RoleDetailVM>(modelResult);
                return HeroResult<RoleDetailVM>.Success(listUserRoleModel);
            }
            return HeroResult<RoleDetailVM>.Fail("No Record Found");
        }

    }
}
