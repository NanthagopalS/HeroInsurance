using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Commands.UserRoleMapping;
using Identity.Domain.User;
using Identity.Core.Responses;
using Identity.Domain.Roles;
using MediatR;

namespace Identity.Core.Features.User.Commands.UserRoleUpdateModel
{
    public record UserRoleMappingUpdateCommand: IRequest<HeroResult<bool>>
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string EmpID { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string RoleId { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }

        public int UserRoleID { get; set; }
        public int RoleTypeID { get; set; }
        public int IdentityRoleId { get; set; }
        public int ReportingIdentityRoleId { get; set; }

        public string ReportingUserID { get; set; }
        public int CategoryID { get; set; }
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
