using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetCustomersDetail
{
    public class GetCustomersDetailQuery : IRequest<HeroResult<GetCustomersDetailVm>>
    {
        public string? CustomerType { get; set; }
        public string? SearchText { get; set; }
        public string? PolicyType { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? CurrentPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
        public IEnumerable<GetCustomersDetailModel>? GetCustomersDetailModel { get; set; }
        public IEnumerable<CustomersDetailPagingModel>? CustomersDetailPagingModel { get; set; }
    }
    public class GetCustomersDetailQueryHandler : IRequestHandler<GetCustomersDetailQuery, HeroResult<GetCustomersDetailVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetCustomersDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<GetCustomersDetailVm>> Handle(GetCustomersDetailQuery request, CancellationToken cancellationToken)
        {
            var roleDetailMapInput = _mapper.Map<GetCustomersDetailInputModel>(request);
            var modelResult = await _userRepository.GetCustomersDetail(roleDetailMapInput, cancellationToken).ConfigureAwait(false);
            if (modelResult != null)
            {
                var listUserRoleModel = _mapper.Map<GetCustomersDetailVm>(modelResult);
                return HeroResult<GetCustomersDetailVm>.Success(listUserRoleModel);
            }
            return HeroResult<GetCustomersDetailVm>.Fail("No Record Found");
        }

    }
}
