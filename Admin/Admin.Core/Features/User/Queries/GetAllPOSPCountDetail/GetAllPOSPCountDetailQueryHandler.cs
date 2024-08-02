using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;
namespace Admin.Core.Features.User.Queries.GetAllPOSPCountDetail
{
    public record GetAllPOSPCountDetailQuery : IRequest<HeroResult<IEnumerable<GetAllPOSPCountDetailVM>>>;
    public class GetAllPOSPCountDetailQueryHandler : IRequestHandler<GetAllPOSPCountDetailQuery, HeroResult<IEnumerable<GetAllPOSPCountDetailVM>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetAllPOSPCountDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<IEnumerable<GetAllPOSPCountDetailVM>>> Handle(GetAllPOSPCountDetailQuery request, CancellationToken cancellationToken)
        {
            var AdminDashboard = await _userRepository.GetAllPOSPCountDetail(cancellationToken);
            if (AdminDashboard is not null)
            {
                var posplist = _mapper.Map<IEnumerable<GetAllPOSPCountDetailVM>>(AdminDashboard);
                return HeroResult<IEnumerable<GetAllPOSPCountDetailVM>>.Success(posplist);
            }
            return HeroResult<IEnumerable<GetAllPOSPCountDetailVM>>.Fail("No Record Found");
        }
    }
}

