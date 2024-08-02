using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;
namespace Admin.Core.Features.User.Queries.GetAllPOSPBackOficePendingReport
{
    public record GetAllPOSPBackOficePendingReportQuery : IRequest<HeroResult<IEnumerable<GetAllPOSPBackOficePendingReportVM>>>;
    public class GetAllPOSPBackOficePendingReportQueryHandler : IRequestHandler<GetAllPOSPBackOficePendingReportQuery, HeroResult<IEnumerable<GetAllPOSPBackOficePendingReportVM>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mapper"></param>
        public GetAllPOSPBackOficePendingReportQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// All Posp Back office pending report
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<IEnumerable<GetAllPOSPBackOficePendingReportVM>>> Handle(GetAllPOSPBackOficePendingReportQuery request, CancellationToken cancellationToken)
        {
            var AdminDashboard = await _userRepository.GetAllPOSPBackOficePendingReport(cancellationToken);
            if (AdminDashboard is not null)
            {
                var posplist = _mapper.Map<IEnumerable<GetAllPOSPBackOficePendingReportVM>>(AdminDashboard);
                return HeroResult<IEnumerable<GetAllPOSPBackOficePendingReportVM>>.Success(posplist);
            }
            return HeroResult<IEnumerable<GetAllPOSPBackOficePendingReportVM>>.Fail("No Record Found");
        }
    }
}