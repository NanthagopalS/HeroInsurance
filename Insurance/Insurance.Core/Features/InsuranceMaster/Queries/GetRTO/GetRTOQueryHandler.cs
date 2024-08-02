using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.InsuranceMaster;

/// <summary>
/// GetMakeModelQuery
/// </summary>
public record GetRTOQuery : IRequest<HeroResult<StateCityRTOYearVm>>
{
}

public class GetRTOQueryHandler : IRequestHandler<GetRTOQuery, HeroResult<StateCityRTOYearVm>>
{
    private readonly IInsuranceMasterRepository _quoteRepository;
    private readonly IMapper _mapper;

    public GetRTOQueryHandler(IInsuranceMasterRepository quoteRepository, IMapper mapper)
    {
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<HeroResult<StateCityRTOYearVm>> Handle(GetRTOQuery request, CancellationToken cancellationToken)
    {
        var rtoResult = await _quoteRepository.GetRTO(cancellationToken);
        if (rtoResult != null)
        {
            var stateCityRTOYearVm = new StateCityRTOYearVm();
            var stateList = _mapper.Map<IEnumerable<StateVm>>(rtoResult.StateList);
            foreach (var state in stateList)
            {
                state.CityVms = rtoResult.CityList.Where(x => x.StateId.Equals(state.StateId))
                    .Select(d => new CityVm
                    {
                        CityId = d.CityId,
                        CityName = d.CityName,
                        RTOVms = rtoResult.RTOList.Where(f => f.CityId.Equals(d.CityId))
                        .Select(r => new RTOVm
                        {
                            RTOCode = r.RTOCode,
                            RTOId = r.RTOId,
                        })
                    });
            }

            var yearMappedResult = _mapper.Map<IEnumerable<YearVm>>(rtoResult.YearList);

            stateCityRTOYearVm.StateVms = stateList;
            stateCityRTOYearVm.YearVms = yearMappedResult;
            return HeroResult<StateCityRTOYearVm>.Success(stateCityRTOYearVm);
        }

        return HeroResult<StateCityRTOYearVm>.Fail("No record found");
    }
}
