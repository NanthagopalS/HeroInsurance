using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.CommercialMaster.Query.GetCommercialMaster
{
	public class GetCommercialCategoryQuery : IRequest<HeroResult<CommercialVehicleCategoryVm>>
	{
	}
	public class GetCommercialCategoryQueryHandler : IRequestHandler<GetCommercialCategoryQuery, HeroResult<CommercialVehicleCategoryVm>>
	{
		private readonly ICommercialMasterRepository _quoteRepository;
		private readonly IMapper _mapper;

		/// <summary>
		/// Initialize
		/// </summary>
		public GetCommercialCategoryQueryHandler(ICommercialMasterRepository quoteRepository, IMapper mapper)
		{
			_quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task<HeroResult<CommercialVehicleCategoryVm>> Handle(GetCommercialCategoryQuery request, CancellationToken cancellationToken)
		{
			var insuranceModel = await _quoteRepository.GetCommercialCategory(cancellationToken);
			if (insuranceModel is null)
			{
				return HeroResult<CommercialVehicleCategoryVm>.Fail("No record found");
			}

			var insuranceMappedList = _mapper.Map<CommercialVehicleCategoryVm>(insuranceModel);
			return HeroResult<CommercialVehicleCategoryVm>.Success(insuranceMappedList);
		}
	}
}
