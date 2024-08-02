using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Queries.GetUserProfileDetail
{
    public record GetUserProfileDetailQuery : IRequest<HeroResult<GetUserProfileDetailVm>>
    {
        public string UserId { get; set; }
    }
    public class GetUserProfileDetailQueryHandler : IRequestHandler<GetUserProfileDetailQuery, HeroResult<GetUserProfileDetailVm>>
    {

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        public GetUserProfileDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<GetUserProfileDetailVm>> Handle(GetUserProfileDetailQuery request, CancellationToken cancellationToken)
        {
            var getUserProfileDetailResult = await _userRepository.GetUserProfileDetail(request.UserId,
                cancellationToken);

            if (getUserProfileDetailResult is not null)
            {
                var result = _mapper.Map<GetUserProfileDetailVm>(getUserProfileDetailResult);

                return HeroResult<GetUserProfileDetailVm>.Success(result);
            }

            return HeroResult<GetUserProfileDetailVm>.Fail("No record found");
        }
    }
}
