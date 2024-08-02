using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;
namespace Identity.Core.Features.User.Commands.UserRoleMapping
{
    public record UserRoleMappingInsertCommand :   IRequest<HeroResult<bool>>
    {
        public string UserName { get; set; }
        public string EmpID { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string RoleId { get; set; }
        public string CreatedBy { get; set; }
        public bool StatusUser { get; set; }

        public int RoleTypeID { get; set; }
        public int IdentityRoleId { get; set; }
        public int ReportingIdentityRoleId { get; set; }
        //  public string UserID { get; set; }
        public string ReportingUserID { get; set; }
        public int CategoryID { get; set; }
        public bool StatusRoleUser { get; set; }
    }
    public class UserRoleMappingInsertCommandHandler : IRequestHandler<UserRoleMappingInsertCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userMappingRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userPersonalDetailRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserRoleMappingInsertCommandHandler(IUserRepository userMappingRepository, IMapper mapper)
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
        public async Task<HeroResult<bool>> Handle(UserRoleMappingInsertCommand userMappingCommand, CancellationToken cancellationToken)
        {
            var userMappingModel = _mapper.Map<UserRoleModel>(userMappingCommand);
            var result = await _userMappingRepository.InsertUserAndRoleMapping(userMappingModel, cancellationToken);

            return HeroResult<bool>.Success(result);
        }
    }
}
