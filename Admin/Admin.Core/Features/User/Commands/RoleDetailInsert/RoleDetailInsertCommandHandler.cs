using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using Admin.Domain.Roles;
using AutoMapper;
using MediatR;


namespace Admin.Core.Features.User.Commands.RoleDetailInsert
{
    public class RoleDetailInsertCommand : IRequest<HeroResult<RoleDetailInsertVm>>
    {
        public string RoleTypeId { get; set; }
        public string RoleName { get; set; }
        public string BUId { get; set; }
        public string RoleLevelId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public List<RoleDetailPermissionInsertCmd> RoleDetailPermissionInsert { get; set; }
    }

    public class RoleDetailPermissionInsertCmd
    {
        public string ModuleId { get; set; }
        public string RoleTypeId { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }
        public string CreatedBy { get; set; }

    }

    public class RoleDetailInsertCommandHandler : IRequestHandler<RoleDetailInsertCommand, HeroResult<RoleDetailInsertVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="buRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RoleDetailInsertCommandHandler(IUserRepository buRepository, IMapper mapper)
        {
            _userRepository = buRepository ?? throw new ArgumentNullException(nameof(buRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        /// <summary>
        /// handle
        /// </summary>
        /// <param name="roleDetailInsertCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<RoleDetailInsertVm>> Handle(RoleDetailInsertCommand roleDetailInsertCommand, CancellationToken cancellationToken)
        {
            RoleDetailInsertVm RoleDetailInsertVm = new RoleDetailInsertVm() { Message = "Failed To Save." };
            bool IsDashboardAdded = false;
            foreach (var item in roleDetailInsertCommand.RoleDetailPermissionInsert)
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
            var roleInputModel = _mapper.Map<RoleDetailInsertInputModel>(roleDetailInsertCommand);
            var result = await _userRepository.InsertRoleDetails(roleInputModel, cancellationToken);
            RoleDetailInsertVm.Status = result;
            RoleDetailInsertVm.Message = "Role Created SuccessFully";
            return HeroResult<RoleDetailInsertVm>.Success(RoleDetailInsertVm);
        }

    }

}
