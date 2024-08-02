using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetPoliciesDetail
{
    public record GetPoliciesDetailQuery : IRequest<HeroResult<IEnumerable<GetPoliciesDetailVm>>>
    {
        public string? POSPId { get; set; }
        public string? PolicyNo { get; set; }
        public string? CustomerName { get; set; }
        public string? Mobile { get; set; }
        public string? PolicyMode { get; set; }

        public string? PolicyType { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int PageIndex { get; set; }

    }
    public class GetPoliciesDetailQueryHandler : IRequestHandler<GetPoliciesDetailQuery, HeroResult<IEnumerable<GetPoliciesDetailVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPoliciesDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPoliciesDetailVm>>> Handle(GetPoliciesDetailQuery request, CancellationToken cancellationToken)
        {
            var getLeadManagementDetail = await _userRepository.GetPoliciesDetail(request.POSPId, request.PolicyNo, request.CustomerName, request.Mobile, request.PolicyMode, request.PolicyType, request.StartDate, request.EndDate, request.PageIndex, cancellationToken).ConfigureAwait(false);
            if (getLeadManagementDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPoliciesDetailVm>>(getLeadManagementDetail);
                return HeroResult<IEnumerable<GetPoliciesDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPoliciesDetailVm>>.Fail("No Record Found");
        }
    }
}
