using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Queries.GetSourcedByUserList
{
    public record GetSourcedByUserListQuery : IRequest<HeroResult<GetSourcedByUserListVm>>
    {
        public string? BUId { get; set; }
    }
    public class GetSourcedByUserListQueryHandler : IRequestHandler<GetSourcedByUserListQuery, HeroResult<GetSourcedByUserListVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetSourcedByUserListQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetSourcedByUserListVm>> Handle(GetSourcedByUserListQuery request, CancellationToken cancellationToken)
        {
            var getBuDetail = await _userRepository.GetSourcedByUserList(request.BUId, cancellationToken);
            if (getBuDetail != null)
            {
                var listInsurer = _mapper.Map<GetSourcedByUserListVm>(getBuDetail);
                return HeroResult<GetSourcedByUserListVm>.Success(listInsurer);
            }

            return HeroResult<GetSourcedByUserListVm>.Fail("No Record Found");
        }
    }
}
