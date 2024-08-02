using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.User.Queries.GetUserByBUId
{
    public class GetUserByBUIdQuery : IRequest<HeroResult<IEnumerable<GetUserByBUIdVm>>>
    {
        public string BUId { get; set; }
        public string SearchText { get; set; }
    }
    public class GetUserByBUIdQueryHandler : IRequestHandler<GetUserByBUIdQuery, HeroResult<IEnumerable<GetUserByBUIdVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetUserByBUIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<GetUserByBUIdVm>>> Handle(GetUserByBUIdQuery request, CancellationToken cancellationToken)
        {
            var userByBUId = await _userRepository.GetUserByBUId(request, cancellationToken).ConfigureAwait(false);
            if (userByBUId is not null)
            {
                var listInsurer = _mapper.Map<IEnumerable<GetUserByBUIdVm>>(userByBUId);
                return HeroResult<IEnumerable<GetUserByBUIdVm>>.Success(listInsurer);
            }
            return HeroResult<IEnumerable<GetUserByBUIdVm>>.Fail("No Record Found");
        }

    }
}
