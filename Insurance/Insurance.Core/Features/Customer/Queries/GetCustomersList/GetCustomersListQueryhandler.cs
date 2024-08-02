using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.Customer.Queries.GetCustomersList
{
    public record GetCustomersListQuery : IRequest<HeroResult<GetCustomersListVm>>
    {
        public string CustomerType { get; set; }
        public string SearchText { get; set; }
        public string PolicyType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int CurrentPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public string PolicyStatus { get; set; }
        public string PolicyNature { get; set; }
    }
    public class GetCustomersListQueryhandler : IRequestHandler<GetCustomersListQuery, HeroResult<GetCustomersListVm>>
    {
        private readonly ICustomerRepository _userRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GetCustomersListQueryhandler(ICustomerRepository userRepository, IMapper mapper)
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
        public async Task<HeroResult<GetCustomersListVm>> Handle(GetCustomersListQuery request, CancellationToken cancellationToken)
        {
            var customerCollections = await _userRepository.GetCustomersList(request, cancellationToken);
            if (customerCollections is not null)
            {
                var listCustomer = _mapper.Map<GetCustomersListVm>(customerCollections);
                return HeroResult<GetCustomersListVm>.Success(listCustomer);
            }
            return HeroResult<GetCustomersListVm>.Fail("No Record Found");
        }

    }
}
