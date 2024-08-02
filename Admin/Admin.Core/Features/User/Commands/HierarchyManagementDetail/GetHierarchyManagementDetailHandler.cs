using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using Admin.Core.Responses;
using MediatR;
using ThirdPartyUtilities.Abstraction;

namespace Admin.Core.Features.User.Commands.UserRoleGetMapping
{
    
    public class GetHierarchyManagementDetail : IRequest<HeroResult<HierarchyManagementDetailVM>>
    {
        public string? RoleTypeId { get; set; }
        public string? RoleId { get; set; }
        public string? ParentUserId { get; set; }
        public string? ParentUserRoleId { get; set; }

        public IEnumerable<UserListModel> UserList { get; set; }
        public IEnumerable<ParentListModel> ParentList { get; set; }
    }

    public class GetHierarchyManagementDetailHandler : IRequestHandler<GetHierarchyManagementDetail, HeroResult<HierarchyManagementDetailVM>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMongoDBService _mongodbService;

        public GetHierarchyManagementDetailHandler(IUserRepository userRepository, IMapper mapper, IMongoDBService mongodbService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mongodbService = mongodbService;
        }

        public async Task<HeroResult<HierarchyManagementDetailVM>> Handle(GetHierarchyManagementDetail request, CancellationToken cancellationToken)
        {
            var modelResult = await _userRepository.GetHierarchyManagementDetail(request.RoleId, request.RoleTypeId, request.ParentUserId, request.ParentUserRoleId, cancellationToken).ConfigureAwait(false);
            if (modelResult != null)
            {
                try
                {
                    // Code For Geting Profile Picture From Mongo DB
                    if (modelResult.UserList != null && modelResult.UserList.Any())
                    {
                        foreach (var item in modelResult.UserList)
                        {
                            if (!string.IsNullOrWhiteSpace(item.ProfilePictureStream))
                            {
                                item.ProfilePictureStream = await _mongodbService.MongoDownload(item.ProfilePictureStream);
                            }
                        }
                    }
                    // End Code
                    var result = _mapper.Map<HierarchyManagementDetailVM>(modelResult);
                    return HeroResult<HierarchyManagementDetailVM>.Success(result);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return HeroResult<HierarchyManagementDetailVM>.Fail("No Record Found");
        }
    }
}
