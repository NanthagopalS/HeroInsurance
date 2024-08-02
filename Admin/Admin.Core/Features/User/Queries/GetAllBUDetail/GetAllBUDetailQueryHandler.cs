using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using Admin.Domain.Roles;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.User.Queries.GetAllBUDetail
{
    public class GetAllBUDetailQuery : IRequest<HeroResult<GetAllBUDetailVm>>
    {
        public string BUName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int CurrentPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public bool IsActive { get; set; }
        
    }
    public class GetAllBUDetailQueryHandler : IRequestHandler<GetAllBUDetailQuery, HeroResult<GetAllBUDetailVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// initialize
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GetAllBUDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
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
        public async Task<HeroResult<GetAllBUDetailVm>> Handle(GetAllBUDetailQuery request, CancellationToken cancellationToken)
        {
            var allbuDetailMapInput = _mapper.Map<AllBUDetailModel>(request);
            var getBuDetail = await _userRepository.GetAllBuDetail(allbuDetailMapInput, cancellationToken).ConfigureAwait(false);
            if (getBuDetail != null)
            {
                var listInsurer = _mapper.Map<GetAllBUDetailVm>(getBuDetail);
                return HeroResult<GetAllBUDetailVm>.Success(listInsurer);
            }
            return HeroResult<GetAllBUDetailVm>.Fail("No Record Found");
        }

    }
}
