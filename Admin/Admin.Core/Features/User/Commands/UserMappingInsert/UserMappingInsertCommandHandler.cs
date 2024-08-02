using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.UserEmailId;
using Admin.Domain.Roles;
using Admin.Domain.User;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Commands.UserMappingInsert
{
    public record UserMappingInsertCommand : IRequest<HeroResult<bool>>
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string ReportingUserID { get; set; }
        public string CategoryId { get; set; }
        public string BUId { get; set; }
        public string RoleTypeId { get; set; }
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
