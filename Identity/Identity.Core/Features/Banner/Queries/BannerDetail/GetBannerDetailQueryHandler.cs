using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using Identity.Domain.Banner;
using MediatR;

namespace Identity.Core.Features.Banner.Queries.BannerDetail;

/// <summary>
/// Query for Get Banner
/// </summary>
public record GetBannerDetailQuery : IRequest<HeroResult<IEnumerable<BannerDetailModel>>>;
public class GetBannerDetailQueryHandler : IRequestHandler<GetBannerDetailQuery, HeroResult<IEnumerable<BannerDetailModel>>>
{
    private readonly IBannerRepository _bannerrepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userBankDetailRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GetBannerDetailQueryHandler(IBannerRepository bannerrepository, IMapper mapper)
    {
        _bannerrepository = bannerrepository ?? throw new ArgumentNullException(nameof(bannerrepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<HeroResult<IEnumerable<BannerDetailModel>>> Handle(GetBannerDetailQuery getbannerdetailquery, CancellationToken cancellationToken)
    {
        var result = await _bannerrepository.GetBannerDetail(cancellationToken).ConfigureAwait(false);

        return HeroResult<IEnumerable<BannerDetailModel>>.Success(result);
    }
}
