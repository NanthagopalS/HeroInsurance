using Insurance.Core.Contracts.Persistence;
using Insurance.Domain.Leads;
using AutoMapper;
using Insurance.Core.Responses;
using MediatR;
using Newtonsoft.Json;

namespace Insurance.Core.Features.Leads
{
    public record GetLeadManagementDetailQuery : IRequest<HeroResult<GetLeadManagementDetailVm>>
    {
        public bool IsFromDashboard { get; set; }
        public string ViewLeadsType { get; set; }
        public string POSPId { get; set; }
        public string SearchText { get; set; }
        public string LeadType { get; set; }
        public string PolicyType { get; set; }
        public string PreQuote { get; set; }
        public string AllStatus { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string UserId { get; set; }
        public bool IsFromPaymentGatewayScreen { get; set; }

        [JsonIgnore]
        public IEnumerable<LeadDetailModelList> LeadDetailModelList { get; set; }
        //[JsonIgnore]
       // public IEnumerable<LeadDetailPagingModel> LeadDetailPagingModel { get; set; }

    }
    public class GetLeadManagementDetailQueryHandler : IRequestHandler<GetLeadManagementDetailQuery, HeroResult<GetLeadManagementDetailVm>>
    {
        private readonly ILeadsRepository _leadsRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetLeadManagementDetailQueryHandler(ILeadsRepository leadsRepository, IMapper mapper)
        {
            _leadsRepository = leadsRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetLeadManagementDetailVm>> Handle(GetLeadManagementDetailQuery request, CancellationToken cancellationToken)
        {
            var getLeadManagementDetail = await _leadsRepository.GetDashboardLeadDetails(request, cancellationToken).ConfigureAwait(false);
            if (getLeadManagementDetail is not null)
            {
                var listInsurer = _mapper.Map<GetLeadManagementDetailVm>(getLeadManagementDetail);
                return HeroResult<GetLeadManagementDetailVm>.Success(listInsurer);
            }
            return HeroResult<GetLeadManagementDetailVm>.Fail("No Record Found");
        }
    }
}
