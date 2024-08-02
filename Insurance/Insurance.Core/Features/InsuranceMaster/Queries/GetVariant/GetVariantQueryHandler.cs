using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.InsuranceMaster;

/// <summary>
/// GetMakeModelQuery
/// </summary>
public record GetVariantQuery : IRequest<HeroResult<IEnumerable<VariantVm>>>
{
    public string Model_Id { get; set; }
    public string Fuel_Id { get; set; }
}

public class GetVariantQueryHandler : IRequestHandler<GetVariantQuery, HeroResult<IEnumerable<VariantVm>>>
{
    private readonly IInsuranceMasterRepository _quoteRepository;
    private readonly IMapper _mapper;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="quoteRepository"></param>
    /// <param name="mapper"></param>
    public GetVariantQueryHandler(IInsuranceMasterRepository quoteRepository, IMapper mapper)
    {
        _quoteRepository = quoteRepository;
        _mapper = mapper;
    }

    public async Task<HeroResult<IEnumerable<VariantVm>>> Handle(GetVariantQuery request, CancellationToken cancellationToken)
    {
        var variantResult = await _quoteRepository.GetVariant(request.Model_Id, request.Fuel_Id, cancellationToken).ConfigureAwait(false);
        if(variantResult.Any())
        {
            var listVariant = _mapper.Map<IEnumerable<VariantVm>>(variantResult);
            return HeroResult<IEnumerable<VariantVm>>.Success(listVariant);
        }

        return HeroResult<IEnumerable<VariantVm>>.Fail("No Record Found");
    }
}
