using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Insurance.Core.Features.ManualPolicy.Query
{
    public record GetManualPolicyListQuery : IRequest<HeroResult<GetManualPolicyListVM>>
    {
        public string Product { get; set; }
        public string PolicyType { get; set; }
        public string Moter { get; set; }
        public string PolicySource { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SearchText { get; set; }
        public int CurrentPageSize { get; set; }
        public int CurrentPageIndex { get; set; }

        
    }
    public class GetManualPolicyListQueryHandler : IRequestHandler<GetManualPolicyListQuery, HeroResult<GetManualPolicyListVM>>
    {
        private readonly IManualPolicyRepository _manualPolicyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetManualPolicyListQueryHandler> _logger;

        public GetManualPolicyListQueryHandler(IManualPolicyRepository manualPolicyRepository, IMapper mapper, ILogger<GetManualPolicyListQueryHandler> logger)
        {
            _manualPolicyRepository = manualPolicyRepository ?? throw new ArgumentNullException(nameof(manualPolicyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HeroResult<GetManualPolicyListVM>> Handle(GetManualPolicyListQuery request, CancellationToken cancellationToken)
        {
            var ManualPolicyList = await _manualPolicyRepository.GetManualPolicyList(request, cancellationToken);
            if (ManualPolicyList is not null)
            {
                var listInsurer = _mapper.Map<GetManualPolicyListVM>(ManualPolicyList);
                return HeroResult<GetManualPolicyListVM>.Success(listInsurer);
            }
            return HeroResult<GetManualPolicyListVM>.Fail("No Record Found");
        }
    }
}
