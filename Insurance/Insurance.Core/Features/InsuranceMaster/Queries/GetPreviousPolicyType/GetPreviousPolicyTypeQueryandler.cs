using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.InsuranceMaster;

public record GetPreviousPolicyTypeQuery : IRequest<HeroResult<IEnumerable<PreviousPolicyTypeVm>>>
{
    public string RegDate { get; set; }
    public bool IsBrandNew { get; set; }
    public string VehicleTypeId { get; set; }
}

public class GetPreviousPolicyTypeQueryandler : IRequestHandler<GetPreviousPolicyTypeQuery, HeroResult<IEnumerable<PreviousPolicyTypeVm>>>
{
    private readonly IInsuranceMasterRepository _quoteRepository;
    private readonly IMapper _mapper;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="quoteRepository"></param>
    /// <param name="mapper"></param>
    public GetPreviousPolicyTypeQueryandler(IInsuranceMasterRepository quoteRepository, IMapper mapper)
    {
        _quoteRepository = quoteRepository;
        _mapper = mapper;
    }

    public async Task<HeroResult<IEnumerable<PreviousPolicyTypeVm>>> Handle(GetPreviousPolicyTypeQuery request, CancellationToken cancellationToken)
    {
        var insurerResult = await _quoteRepository.GetPreviousPolicyType(request.RegDate, request.IsBrandNew, request.VehicleTypeId,
                                                          cancellationToken).ConfigureAwait(false);
        if (insurerResult.Any())
        {
            var previousPolicyTypes = _mapper.Map<IEnumerable<PreviousPolicyTypeVm>>(insurerResult);
            return HeroResult<IEnumerable<PreviousPolicyTypeVm>>.Success(previousPolicyTypes);
        }

        return HeroResult<IEnumerable<PreviousPolicyTypeVm>>.Fail("No Record Found");
    }
}
