using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.ManualPolicy.Query.GetManualPolicyNature
{
    public record GetManualPolicyNatureQuery : IRequest<HeroResult<IEnumerable<GetManualPolicyNatureVm>>>
	{

	}
	public record GetManualPolicyNatureQueryHandler : IRequestHandler<GetManualPolicyNatureQuery, HeroResult<IEnumerable<GetManualPolicyNatureVm>>>
	{
		private readonly IManualPolicyRepository _manualPolicyRepository;
		private readonly IMapper _mapper;

		public GetManualPolicyNatureQueryHandler(IManualPolicyRepository manualPolicyRepository, IMapper mapper)
		{
			_manualPolicyRepository = manualPolicyRepository ?? throw new ArgumentNullException(nameof(manualPolicyRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

		}
		public async Task<HeroResult<IEnumerable<GetManualPolicyNatureVm>>> Handle(GetManualPolicyNatureQuery request, CancellationToken cancellationToken)
		{
			var ManualPolicyList = await _manualPolicyRepository.GetManualPolicyNatureList(request, cancellationToken);
			if (ManualPolicyList is not null)
			{
				var listInsurer = _mapper.Map<IEnumerable<GetManualPolicyNatureVm>>(ManualPolicyList);
				return HeroResult<IEnumerable<GetManualPolicyNatureVm>>.Success(listInsurer);
			}
			return HeroResult<IEnumerable<GetManualPolicyNatureVm>>.Fail("No Record Found");
		}
	}
}
