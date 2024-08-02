using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Queries.GetRoleType
{
    public record GetStampCountQuery : IRequest<HeroResult<StampCountVm>>;
    public class GetStampCountQueryHandler : IRequestHandler<GetStampCountQuery, HeroResult<StampCountVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetStampCountQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<StampCountVm>> Handle(GetStampCountQuery request, CancellationToken cancellationToken)
        {
            var stampCountResult = await _userRepository.GetStampCountQuery(cancellationToken).ConfigureAwait(false);
            if (stampCountResult != null)
            {
                var stamps = _mapper.Map<StampCountVm>(stampCountResult);
                return HeroResult<StampCountVm>.Success(stamps);
            }
            return HeroResult<StampCountVm>.Fail("No Record Found");
        }       
    }
}
