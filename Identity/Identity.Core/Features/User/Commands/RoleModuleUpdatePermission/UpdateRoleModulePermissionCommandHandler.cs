using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Commands.RoleModulePermission;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Commands.RoleModuleUpdatePermission
{
    public record UpdateRoleModulePermissionCommand : IRequest<HeroResult<bool>>
    {
        public string RoleID { get; set; }
        public int ModuleID { get; set; }
        public int RoletypeID { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public string UpdatedBy { get; set; }
        public bool isActive { get; set; }
        
    }
    public class UpdateRoleModulePermissionCommandHandler : IRequestHandler<UpdateRoleModulePermissionCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _rolePermissionRepository;
        private readonly IMapper _mapper;

        public UpdateRoleModulePermissionCommandHandler(IUserRepository rolePermissionRepository, IMapper mapper)
        {
            _rolePermissionRepository = rolePermissionRepository ?? throw new ArgumentNullException(nameof(rolePermissionRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(UpdateRoleModulePermissionCommand roleModulePermissionCommand, CancellationToken cancellationToken)
        {
            var roleModel = _mapper.Map<RoleModuleUpdatePermissionModel>(roleModulePermissionCommand);
            var result = await _rolePermissionRepository.UpdateRoleModulePermissionMapping(roleModel, cancellationToken);

            return HeroResult<bool>.Success(result);
        }

    }
}
