using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.User.Queries.GetTotalSalesDetail
{
    public record GetTotalSalesDetailQuery : IRequest<HeroResult<IEnumerable<GetTotalSalesDetailVm>>>
    {
        public string UserId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    };
    public class GetTotalSalesDetailQueryHandler : IRequestHandler<GetTotalSalesDetailQuery, HeroResult<IEnumerable<GetTotalSalesDetailVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialized
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GetTotalSalesDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<IEnumerable<GetTotalSalesDetailVm>>> Handle(GetTotalSalesDetailQuery request, CancellationToken cancellationToken)
        {
            var totalSales = await _userRepository.GetTotalSalesDetail(request, cancellationToken);
            if (totalSales is not null)
            {
                var stamps = _mapper.Map<IEnumerable<GetTotalSalesDetailVm>>(totalSales);
                return HeroResult<IEnumerable<GetTotalSalesDetailVm>>.Success(stamps);
            }
            return HeroResult<IEnumerable<GetTotalSalesDetailVm>>.Fail("No Record Found");
        }
    }
}
