using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;
using System.Data;

namespace Identity.Core.Features.User.Commands.RoleModulePermission
{

    public record RoleModulePermissionCommand : IRequest<HeroResult<bool>>
    {
        public int RoleTypeID { get; set; }
        public string RoleTitleName { get; set; }
        public int BUID { get; set; }
        public int RoleLevelID { get; set; }
        public string CreatedBy { get; set; }
        public IList<RoleModulePermissionCommandInsert> RoleModulePermissionCommandInsert { get; set; }
    }
    public record RoleModulePermissionCommandInsert 
    {
       // public string RoleID { get; set; }
        public int ModuleID { get; set; }
       // public int RoletypeID { get; set; }
     //   public int IdentityRoleId { get; set; }        
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public string CreatedBy { get; set; }

    }
    /*
    public record RoleModulePermissionCommand : IRequest<HeroResult<bool>>
    {
        public string RoleID { get; set; }
        public int ModuleID { get; set; }
        public int RoletypeID { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public string CreatedBy { get; set; }

    }
    */
    public class RoleModulePermissionCommandHandler : IRequestHandler<RoleModulePermissionCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _rolePermissionRepository;
        private readonly IMapper _mapper;

        public RoleModulePermissionCommandHandler(IUserRepository rolePermissionRepository, IMapper mapper)
        {
            _rolePermissionRepository = rolePermissionRepository ?? throw new ArgumentNullException(nameof(rolePermissionRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(RoleModulePermissionCommand roleModulePermissionCommand, CancellationToken cancellationToken)
        {
            var roleModel = _mapper.Map<RoleModulePermissionModel>(roleModulePermissionCommand);
            var result = await _rolePermissionRepository.RoleModulePermissionMapping(roleModel, cancellationToken);

            return HeroResult<bool>.Success(result);
        }

    }
}
