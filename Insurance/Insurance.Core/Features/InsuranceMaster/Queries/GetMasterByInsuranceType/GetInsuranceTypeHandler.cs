using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.InsuranceMaster;

/// <summary>
/// Query for Insurance Type
/// </summary>
public record GetInsuranceTypeQuery : IRequest<HeroResult<IEnumerable<InsuranceTypeVm>>>
{
    public string InsuranceType { get; set; }
}

/// <summary>
/// Handler for Insurance Type
/// </summary>
public class GetInsuranceTypeHandler : IRequestHandler<GetInsuranceTypeQuery, HeroResult<IEnumerable<InsuranceTypeVm>>>
{
    private readonly IInsuranceMasterRepository _insuranceMasterRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="quoteRepository"></param>
    /// <param name="mapper"></param>
    public GetInsuranceTypeHandler(IInsuranceMasterRepository quoteRepository, IMapper mapper)
    {
        _insuranceMasterRepository = quoteRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HeroResult<IEnumerable<InsuranceTypeVm>>> Handle(GetInsuranceTypeQuery request, CancellationToken cancellationToken)
    {
        var insurerResult = await _insuranceMasterRepository.GeInsuranceType(request.InsuranceType, cancellationToken).ConfigureAwait(false);
        if (insurerResult.Any())
        {
            var listInsurer = _mapper.Map<IEnumerable<InsuranceTypeVm>>(insurerResult);
            return HeroResult<IEnumerable<InsuranceTypeVm>>.Success(listInsurer);
        }

        return HeroResult<IEnumerable<InsuranceTypeVm>>.Fail("H-10001");
    }
}

