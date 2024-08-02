using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetSalesOverview
{
    public record GetSalesOverviewQuery : IRequest<HeroResult<IEnumerable<GetSalesOverviewVm>>>
    {
        public string? UserId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

    }
    public class GetSalesOverviewQueryHandler : IRequestHandler<GetSalesOverviewQuery, HeroResult<IEnumerable<GetSalesOverviewVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetSalesOverviewQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetSalesOverviewVm>>> Handle(GetSalesOverviewQuery request, CancellationToken cancellationToken)
        {
            var getLeadManagementDetail = await _userRepository.GetSalesOverview(request.UserId,request.StartDate, request.EndDate,  cancellationToken);
            if (getLeadManagementDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetSalesOverviewVm>>(getLeadManagementDetail);
                return HeroResult<IEnumerable<GetSalesOverviewVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetSalesOverviewVm>>.Fail("No Record Found");
        }
    }
}
