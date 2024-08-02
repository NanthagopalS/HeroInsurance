using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Queries.PanVerificationDetails;
/// <summary>
/// GetPanVerification Query
/// </summary>
/// 
public record GetPanVerificationQuery : IRequest<HeroResult<PanVerificationVM>>
{
    public string UserId { get; set; }
    public string PanNumber { get; set; }
}
public class PanVerificationQueryHandler : IRequestHandler<GetPanVerificationQuery, HeroResult<PanVerificationVM>>
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _panVerificationlRepository;

    public PanVerificationQueryHandler(IUserRepository panVerificationlRepositor, IMapper mapper)
    {
        _panVerificationlRepository = panVerificationlRepositor ?? throw new ArgumentNullException(nameof(panVerificationlRepositor));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<HeroResult<PanVerificationVM>> Handle(GetPanVerificationQuery request, CancellationToken cancellationToken)
    {
        var panVerificationResult = await _panVerificationlRepository.VerifyPanDetails(request.UserId,
                                                                                       request.PanNumber,
                                                                                       cancellationToken);
        if (panVerificationResult != null)
        {
            var listPan = _mapper.Map<PanVerificationVM>(panVerificationResult);
            return HeroResult<PanVerificationVM>.Success(listPan);
        }

        return HeroResult<PanVerificationVM>.Fail("No Record Found");
    }
}

