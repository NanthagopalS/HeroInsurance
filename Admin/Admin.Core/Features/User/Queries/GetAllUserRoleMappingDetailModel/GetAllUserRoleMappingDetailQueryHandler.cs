using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using Admin.Domain.Roles;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.User.Queries.GetAllUserRoleMappingDetailModel
{
    public class GetAllUserRoleMappingDetailQuery : IRequest<HeroResult<GetAllUserRoleMappingDetailVm>>
    {
        public string? EmployeeIdorName { get; set; }
        public string? RoleTypeId { get; set; }
        public bool? StatusId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? CurrentPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
        public IEnumerable<UserRoleMappingDataModel>? UserRoleMappingDataModel { get; set; }
        public IEnumerable<UserRoleMappingPaginationModel>? UserRoleMappingPaginationModel { get; set; }
    }
    public class GetAllUserRoleMappingDetailQueryHandler : IRequestHandler<GetAllUserRoleMappingDetailQuery, HeroResult<GetAllUserRoleMappingDetailVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetAllUserRoleMappingDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<GetAllUserRoleMappingDetailVm>> Handle(GetAllUserRoleMappingDetailQuery request, CancellationToken cancellationToken)
        {
            var roleDetailMapInput = _mapper.Map<GetAllUserRoleMappingInputModel>(request);
            var modelResult = await _userRepository.GetAllUserRoleMappingDetail(roleDetailMapInput, cancellationToken).ConfigureAwait(false);
            if (modelResult != null)
            {
                var listUserRoleModel = _mapper.Map<GetAllUserRoleMappingDetailVm>(modelResult);
                return HeroResult<GetAllUserRoleMappingDetailVm>.Success(listUserRoleModel);
            }
            return HeroResult<GetAllUserRoleMappingDetailVm>.Fail("No Record Found");
        }

    }
}
