using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Commands.UserRoleGetMapping
{
    public class GetPOSPManagementDetail : IRequest<HeroResult<POSPManagementDetailVM>>
    {
        
        public string? SearchText { get; set; }
        public int POSPStatus { get; set; }
        public string? StageId { get; set; }
        public string? RelationManagerId { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string? CreatedBy { get; set; }
        public IEnumerable<POSPManagementDataModel>? POSPManagementDataModel { get; set; }
        public IEnumerable<POSPManagementPaginationModel>? POSPManagementPaginationModel { get; set; }
    }
    public class GetPOSPManagementDetailHandler : IRequestHandler<GetPOSPManagementDetail, HeroResult<POSPManagementDetailVM>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetPOSPManagementDetailHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<POSPManagementDetailVM>> Handle(GetPOSPManagementDetail request, CancellationToken cancellationToken)
        {
            var roleDetailMapInput = _mapper.Map<POSPManagementInputModel>(request);
            var modelResult = await _userRepository.GetPOSPManagementDetail(roleDetailMapInput, cancellationToken).ConfigureAwait(false);
            if (modelResult != null)
            {
                var listUserRoleModel = _mapper.Map<POSPManagementDetailVM>(modelResult);
                return HeroResult<POSPManagementDetailVM>.Success(listUserRoleModel);
            }
            return HeroResult<POSPManagementDetailVM>.Fail("No Record Found");
        }

    }
}
