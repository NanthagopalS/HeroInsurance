using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.User;
using Admin.Core.Responses;
using Admin.Domain.Roles;
using MediatR;

namespace Admin.Core.Features.User.Commands.UserRoleUpdateModel
{
    public record UserRoleMappingUpdateCommand: IRequest<HeroResult<bool>>
    {
        public string? UserRoleMappingId { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? EmpID { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public string? Gender { get; set; }
        public string? DOB { get; set; }
        public string? ProfilePictureID { get; set; }
        public string? RoleTypeId { get; set; }
        public string? BUId { get; set; }
        public string? RoleId { get; set; }
        public string? ReportingIdentityRoleId { get; set; }
        public string? ReportingUserId { get; set; }
        public string? CategoryId { get; set; }
        public string? CreatedBy { get; set; }
        public byte[]? ImageStream { get; set; }
        public string? DocumentId { get; set; }
        public bool IsProfilePictureChange { get; set; }
        public bool IsActive { get; set; }
    }
    public class UserRoleMappingUpdateCommandHandler : IRequestHandler<UserRoleMappingUpdateCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userMappingRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userPersonalDetailRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserRoleMappingUpdateCommandHandler(IUserRepository userMappingRepository, IMapper mapper)
        {
            _userMappingRepository = userMappingRepository ?? throw new ArgumentNullException(nameof(userMappingRepository));
            _mapper = mapper;
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<bool>> Handle(UserRoleMappingUpdateCommand userMappingCommand, CancellationToken cancellationToken)
        {
            var userMappingModel = _mapper.Map<UserRoleUpdateModels>(userMappingCommand);
            var result = await _userMappingRepository.UpdateUserAndRoleMapping(userMappingModel, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
   
}
