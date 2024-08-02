using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.InsuranceMaster;
public record GetInsurerQuery : IRequest<HeroResult<IEnumerable<InsurerVm>>>
{
    public bool? IsCommercial { get; set; }
}

public class GetInsurerQueryHandler : IRequestHandler<GetInsurerQuery, HeroResult<IEnumerable<InsurerVm>>>
{
    private readonly IInsuranceMasterRepository _quoteRepository;
    private readonly IMapper _mapper;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="quoteRepository"></param>
    /// <param name="mapper"></param>
    public GetInsurerQueryHandler(IInsuranceMasterRepository quoteRepository, IMapper mapper)
    {
        _quoteRepository = quoteRepository;
        _mapper = mapper;
    }

    public async Task<HeroResult<IEnumerable<InsurerVm>>> Handle(GetInsurerQuery request, CancellationToken cancellationToken)
    {
        var insurerResult = await _quoteRepository.GetInsurer(request.IsCommercial, cancellationToken).ConfigureAwait(false);
        if (insurerResult.Any())
        {
            var listInsurer = _mapper.Map<IEnumerable<InsurerVm>>(insurerResult);
            return HeroResult<IEnumerable<InsurerVm>>.Success(listInsurer);
        }

        return HeroResult<IEnumerable<InsurerVm>>.Fail("No Record Found");
    }
}
