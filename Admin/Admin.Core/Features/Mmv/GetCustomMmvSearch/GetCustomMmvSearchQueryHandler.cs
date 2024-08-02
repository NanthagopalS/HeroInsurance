
using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.Mmv.VariantMappingStatus;
public class GetCustomMmvSearchQuery : IRequest<HeroResult<IEnumerable<GetCustomMmvSearchVm>>>
{
	public string InsurerId { get; set; }
	public string MakeName { get; set; }
    public string VariantSearch { get; set; }
}
public class GetCustomMmvSearchQueryHandler : IRequestHandler<GetCustomMmvSearchQuery, HeroResult<IEnumerable<GetCustomMmvSearchVm>>>
{
	private readonly IMmvRepository _immvRepository;
	private readonly IMapper _mapper;
	public GetCustomMmvSearchQueryHandler(IMmvRepository mmvRepository, IMapper mapper)
	{
		_immvRepository = mmvRepository ?? throw new ArgumentNullException(nameof(mmvRepository));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
	}

	public async Task<HeroResult<IEnumerable<GetCustomMmvSearchVm>>> Handle(GetCustomMmvSearchQuery request, CancellationToken cancellationToken)
	{
		var HeroVariants = await _immvRepository.GetAllVariantForCustomModel(request, cancellationToken);
		if (HeroVariants is not null)
		{
			var HeroVariantslist = _mapper.Map<IEnumerable<GetCustomMmvSearchVm>>(HeroVariants);
			return HeroResult<IEnumerable<GetCustomMmvSearchVm>>.Success(HeroVariantslist);
		}
		return HeroResult<IEnumerable<GetCustomMmvSearchVm>>.Fail("No Record Found");
	}
}
