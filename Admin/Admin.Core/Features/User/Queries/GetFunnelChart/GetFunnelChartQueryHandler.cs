using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetFunnelChart
{
    public record GetFunnelChartQuery : IRequest<HeroResult<IEnumerable<GetFunnelChartVm>>>
    {
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? UserId { get; set; }


    }
    public class GetFunnelChartQueryHandler : IRequestHandler<GetFunnelChartQuery, HeroResult<IEnumerable<GetFunnelChartVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetFunnelChartQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetFunnelChartVm>>> Handle(GetFunnelChartQuery request, CancellationToken cancellationToken)
        {
            var getLeadManagementDetail = await _userRepository.GetFunnelChart(request.StartDate, request.EndDate, request.UserId, cancellationToken);
            if (getLeadManagementDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetFunnelChartVm>>(getLeadManagementDetail);
                return HeroResult<IEnumerable<GetFunnelChartVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetFunnelChartVm>>.Fail("No Record Found");
        }
    }
}
