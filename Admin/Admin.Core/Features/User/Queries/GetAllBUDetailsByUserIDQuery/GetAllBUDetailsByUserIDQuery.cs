using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.User.Queries.GetAllBUDetailsByUserIDQuery
{
    public class GetAllBUDetailsByUserIDQuery : IRequest<HeroResult<IEnumerable<GetAllBUDetailsByUserIDVm>>>
    {
        public string UserID { get; set; }
    }
    public class GetAllBUDetailsByUserIDQueryHandler : IRequestHandler<GetAllBUDetailsByUserIDQuery, HeroResult<IEnumerable<GetAllBUDetailsByUserIDVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GetAllBUDetailsByUserIDQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        /// <summary>
        /// handle
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<IEnumerable<GetAllBUDetailsByUserIDVm>>> Handle(GetAllBUDetailsByUserIDQuery request, CancellationToken cancellationToken)
        {
            var getBuDetail = await _userRepository.GetAllBuDetailByUserID(request, cancellationToken);
            if (getBuDetail is not null)
            {
                var listInsurer = _mapper.Map<IEnumerable<GetAllBUDetailsByUserIDVm>>(getBuDetail);
                return HeroResult<IEnumerable<GetAllBUDetailsByUserIDVm>>.Success(listInsurer);
            }
            return HeroResult<IEnumerable<GetAllBUDetailsByUserIDVm>>.Fail("No Record Found");
        }

    }
}
