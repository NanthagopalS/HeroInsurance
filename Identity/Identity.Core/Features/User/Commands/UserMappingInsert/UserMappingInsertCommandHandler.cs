using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Commands.UserEmailId;
using Identity.Domain.Roles;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Commands.UserMappingInsert
{
    public record UserMappingInsertCommand : IRequest<HeroResult<bool>>
    {
        public string UserID { get; set; }
        public string RoleID { get; set; }
        public string ReportingUserID { get; set; }
        public int CategoryID { get; set; }
        public int BUID { get; set; }
        public int RoleTypeID { get; set; }
        public bool IsActive { get; set; }
    }
    public class UserMappingInsertCommandHandler : IRequestHandler<UserMappingInsertCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userMappingRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userPersonalDetailRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserMappingInsertCommandHandler(IUserRepository userMappingRepository, IMapper mapper)
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
        public async Task<HeroResult<bool>> Handle(UserMappingInsertCommand userMappingCommand, CancellationToken cancellationToken)
        {
            var userMappingModel = _mapper.Map<UserMappingInsertInputModel>(userMappingCommand);
            var result = await _userMappingRepository.UserRoleMappingInsert(userMappingModel, cancellationToken);

            return HeroResult<bool>.Success(result);
        }
    }
}
