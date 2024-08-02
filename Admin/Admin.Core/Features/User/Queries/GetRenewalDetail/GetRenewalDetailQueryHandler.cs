using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetRenewalDetail
{
    public record GetRenewalDetailQuery : IRequest<HeroResult<IEnumerable<GetRenewalDetailVm>>>
    {
        public string? POSPId { get; set; }
        public string? PolicyNo { get; set; }
        public string? CustomerName { get; set; }       
        public string? PolicyType { get; set; }        
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int PageIndex { get; set; }

    }
    public class GetRenewalDetailQueryHandler : IRequestHandler<GetRenewalDetailQuery, HeroResult<IEnumerable<GetRenewalDetailVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetRenewalDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetRenewalDetailVm>>> Handle(GetRenewalDetailQuery request, CancellationToken cancellationToken)
        {
            var getLeadManagementDetail = await _userRepository.GetRenewalDetail(request.POSPId, request.PolicyNo, request.CustomerName, request.PolicyType, request.StartDate, request.EndDate, request.PageIndex, cancellationToken).ConfigureAwait(false);
            if (getLeadManagementDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetRenewalDetailVm>>(getLeadManagementDetail);
                return HeroResult<IEnumerable<GetRenewalDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetRenewalDetailVm>>.Fail("No Record Found");
        }
    }
}
