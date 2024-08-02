using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.Banner;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.Banner.Commands.InsertBanner;

/// <summary>
/// Command for BannerUpload
/// </summary>
public record BannerUploadCommand : IRequest<HeroResult<bool>>
{
    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; set; }


    /// <summary>
    /// BannerFileName
    /// </summary>
    public string BannerFileName { get; set; }
    //public List<string> DocumentTypeId { get; set; }

    /// <summary>
    /// BannerStoragePath
    /// </summary>
    public string BannerStoragePath { get; set; }

    /// <summary>
    /// BannerType
    /// </summary>
    public string BannerType { get; set; }

    public byte[] BannnerImage { get; set; }


}
internal class BannerUploadCommandHandler : IRequestHandler<BannerUploadCommand, HeroResult<bool>>
{
    private readonly IBannerRepository _bannerrepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userDocumentRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public BannerUploadCommandHandler(IBannerRepository bannerrepository, IMapper mapper)
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
    public async Task<HeroResult<bool>> Handle(BannerUploadCommand banneruploadcommand, CancellationToken cancellationToken)
    {
        var bannerdetailmodel = _mapper.Map<BannerDetailModel>(banneruploadcommand);
        var result = await _bannerrepository.BannerUpload(bannerdetailmodel);
        return HeroResult<bool>.Success(result);
    }

}