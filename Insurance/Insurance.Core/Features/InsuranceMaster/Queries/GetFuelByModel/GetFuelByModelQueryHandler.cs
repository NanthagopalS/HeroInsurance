using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.InsuranceMaster;

/// <summary>
/// GetMakeModelQuery
/// </summary>
public record GetFuelByModelQuery : IRequest<HeroResult<IEnumerable<FuelVm>>>
{
    public string ModelId { get; set; }
}

public class GetFuelByModelQueryHandler : IRequestHandler<GetFuelByModelQuery, HeroResult<IEnumerable<FuelVm>>>
{
    private readonly IInsuranceMasterRepository _quoteRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialize
    /// </summary>
    public GetFuelByModelQueryHandler(IInsuranceMasterRepository quoteRepository, IMapper mapper)
    {
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<HeroResult<IEnumerable<FuelVm>>> Handle(GetFuelByModelQuery request, CancellationToken cancellationToken)
    {
        var fuelModels = await _quoteRepository.GetFuelByModel(request.ModelId).ConfigureAwait(false);
        if (fuelModels == null)
        {
            return HeroResult<IEnumerable<FuelVm>>.Fail("No record found");
        }

        var fuelMappedList = _mapper.Map<IEnumerable<FuelVm>>(fuelModels);
        return HeroResult<IEnumerable<FuelVm>>.Success(fuelMappedList);
    }
}

