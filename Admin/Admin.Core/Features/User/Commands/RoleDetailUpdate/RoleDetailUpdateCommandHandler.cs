using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.RoleDetailInsert;
using Admin.Core.Responses;
using Admin.Domain.Roles;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.User.Commands.BUUpdate
{
    public record RoleDetailUpdateCommand : IRequest<HeroResult<RoleDetailInsertVm>>
    {
        public string RoleId { get; set; }
        public string RoleTypeId { get; set; }
        public string RoleName { get; set; }
        public string BUId { get; set; }
        public string RoleLevelId { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public List<RoleDetailPermissionUpdateCmd> RoleDetailPermissionUpdate { get; set; }

    }

    public class RoleDetailPermissionUpdateCmd
    {
        public string RoleModulePermissionId { get; set; }
        public string ModuleId { get; set; }
        public string RoleTypeId { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public string UpdatedBy { get; set; }

    }

    public class RoleDetailUpdateCommandHandler : IRequestHandler<RoleDetailUpdateCommand, HeroResult<RoleDetailInsertVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public RoleDetailUpdateCommandHandler(IUserRepository buRepository, IMapper mapper)
        {
            _userRepository = buRepository ?? throw new ArgumentNullException(nameof(buRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<RoleDetailInsertVm>> Handle(RoleDetailUpdateCommand roledetailUpdateCommand, CancellationToken cancellationToken)
        {
            RoleDetailInsertVm RoleDetailInsertVm = new RoleDetailInsertVm() { Message = "Failed To Save." };
            bool IsDashboardAdded = false;
            foreach (var item in roledetailUpdateCommand.RoleDetailPermissionUpdate)
            {
                if ((item.ModuleId.Equals("BF4E0149-0C14-4E66-8391-D1E5F5630B2F") && item.ViewPermission) || (item.ModuleId.Equals("DF0805C1-0725-434C-86B8-0CA2160B69CE") && item.ViewPermission))
                {
                    IsDashboardAdded = true;
                }
            }
            if (!IsDashboardAdded)
            {
                return HeroResult<RoleDetailInsertVm>.Fail("At least one Dashboard should have View Permission");
            }
            var roleInput = _mapper.Map<RoleDetailUpdateInputModel>(roledetailUpdateCommand);
            var result = await _userRepository.UpdateRoleDetails(roleInput, cancellationToken);
            RoleDetailInsertVm.Status = result;
            RoleDetailInsertVm.Message = "Role Created SuccessFully";
            return HeroResult<RoleDetailInsertVm>.Success(RoleDetailInsertVm);
        }
    }

}
