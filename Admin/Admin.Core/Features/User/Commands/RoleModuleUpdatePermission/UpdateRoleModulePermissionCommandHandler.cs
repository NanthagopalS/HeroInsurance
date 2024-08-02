using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.RoleModulePermission;
using Admin.Domain.User;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Commands.RoleModuleUpdatePermission
{
    public record UpdateRoleModulePermissionCommand : IRequest<HeroResult<bool>>
    {
        public string RoleId { get; set; }
        public string ModuleId { get; set; }
        public string RoleTypeId { get; set; }
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
