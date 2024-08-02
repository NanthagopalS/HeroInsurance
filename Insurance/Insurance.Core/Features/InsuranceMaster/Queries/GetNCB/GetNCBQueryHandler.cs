using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.InsuranceMaster;

public record GetNCBQuery : IRequest<HeroResult<IEnumerable<NCBVm>>>
{
    public string PolicyExpiryDate { get; set; }
    public bool IsPreviousPolicy { get; set; }
}
public class GetNCBQueryHandler : IRequestHandler<GetNCBQuery, HeroResult<IEnumerable<NCBVm>>>
{
    private readonly IInsuranceMasterRepository _quoteRepository;
    private readonly IMapper _mapper;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="quoteRepository"></param>
    /// <param name="mapper"></param>
    public GetNCBQueryHandler(IInsuranceMasterRepository quoteRepository, IMapper mapper)
    {
        _quoteRepository = quoteRepository;
        _mapper = mapper;
    }

    public async Task<HeroResult<IEnumerable<NCBVm>>> Handle(GetNCBQuery request, CancellationToken cancellationToken)
    {
        var insurerResult = await _quoteRepository.GetNCB(request.PolicyExpiryDate,
                                                          request.IsPreviousPolicy,
                                                          cancellationToken).ConfigureAwait(false);
        if (insurerResult.Any())
        {
            var listInsurer = _mapper.Map<IEnumerable<NCBVm>>(insurerResult);
            return HeroResult<IEnumerable<NCBVm>>.Success(listInsurer);
        }

        return HeroResult<IEnumerable<NCBVm>>.Fail("No Record Found");
    }
}