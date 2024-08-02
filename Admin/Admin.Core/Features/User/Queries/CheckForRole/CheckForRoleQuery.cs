using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.User.Queries.CheckForRole
{
    public class CheckForRoleQuery : IRequest<HeroResult<IEnumerable<CheckForRoleVm>>>
    {
        public string? UserId { get; set; }
    }

    public class CheckForRoleQueryHandler : IRequestHandler<CheckForRoleQuery, HeroResult<IEnumerable<CheckForRoleVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CheckForRoleQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<IEnumerable<CheckForRoleVm>>> Handle(CheckForRoleQuery request, CancellationToken cancellationToken)
        {
            var getBuDetail = await _userRepository.CheckForRole(request, cancellationToken);
            if (getBuDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<CheckForRoleVm>>(getBuDetail);
                return HeroResult<IEnumerable<CheckForRoleVm>>.Success(listInsurer);
            }
            return HeroResult<IEnumerable<CheckForRoleVm>>.Fail("No Record Found");
        }
    }

}
