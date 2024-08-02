using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain;
using MediatR;

namespace Insurance.Core.Features.InsuranceMaster.Queries.GetCity
{
    public record GetCityQuery : IRequest<HeroResult<IEnumerable<CityModel>>>
    {

    }
    public class GetCityQueryHandler : IRequestHandler<GetCityQuery, HeroResult<IEnumerable<CityModel>>>
    {
        private readonly IInsuranceMasterRepository _insuranceRepository;
        private readonly IMapper _mapper;

        public GetCityQueryHandler(IInsuranceMasterRepository insuranceRepository, IMapper mapper)
        {
            _insuranceRepository = insuranceRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<CityModel>>> Handle(GetCityQuery request, CancellationToken cancellationToken)
        {
            var cityResult = await _insuranceRepository.GetCity(cancellationToken).ConfigureAwait(false);
            if (cityResult.Any())
            {
                var listCity = _mapper.Map<IEnumerable<CityModel>>(cityResult);
                return HeroResult<IEnumerable<CityModel>>.Success(listCity);
            }

            return HeroResult<IEnumerable<CityModel>>.Fail("No Record Found");

        }
    }
}
