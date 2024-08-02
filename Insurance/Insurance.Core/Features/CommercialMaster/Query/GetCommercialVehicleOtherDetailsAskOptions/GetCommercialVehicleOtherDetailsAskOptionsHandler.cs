using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.CommercialMaster.Query.GetCommercialVehicleOtherDetailsAskOptions
{
    public class GetCommercialVehicleOtherDetailsAskOptionsQuery : IRequest<HeroResult<GetCommercialVehicleOtherDetailsAskOptionsVm>>
    {
        public string variantid { get; set; }
    }

    public class GetCommercialVehicleOtherDetailsAskOptionsHandler : IRequestHandler<GetCommercialVehicleOtherDetailsAskOptionsQuery, HeroResult<GetCommercialVehicleOtherDetailsAskOptionsVm>>
    {
        private readonly ICommercialMasterRepository _quoteRepository;
        private readonly IMapper _mapper;
        public GetCommercialVehicleOtherDetailsAskOptionsHandler(ICommercialMasterRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<GetCommercialVehicleOtherDetailsAskOptionsVm>> Handle(GetCommercialVehicleOtherDetailsAskOptionsQuery getCommercialVehicleOtherDetailsAskOptionsQuery, CancellationToken cancellationToken)
        {
            var insuranceModel = await _quoteRepository.GetCommercialVehicleOtherDetailsAskOptions(getCommercialVehicleOtherDetailsAskOptionsQuery, cancellationToken);
            if (insuranceModel is null)
            {
                return HeroResult<GetCommercialVehicleOtherDetailsAskOptionsVm>.Fail("No record found");
            }

            var insuranceMappedList = _mapper.Map<GetCommercialVehicleOtherDetailsAskOptionsVm>(insuranceModel);
            return HeroResult<GetCommercialVehicleOtherDetailsAskOptionsVm>.Success(insuranceMappedList);
        }

    }

}
