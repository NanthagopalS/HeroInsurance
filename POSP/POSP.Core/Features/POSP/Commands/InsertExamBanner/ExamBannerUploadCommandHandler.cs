using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.InsertExamBanner;



/// <summary>
/// 
/// </summary>
public record ExamBannerUploadCommand : IRequest<HeroResult<bool>>
{
    /// <summary>
    /// Id
    /// </summary>
    public string? Id { get; set; }


    /// <summary>
    /// BannerFileName
    /// </summary>
    public string? BannerFileName { get; set; }
    //public List<string> DocumentTypeId { get; set; }

    /// <summary>
    /// BannerStoragePath
    /// </summary>
    public string? BannerStoragePath { get; set; }

    /// <summary>
    /// BannerType
    /// </summary>
    public string? BannerType { get; set; }

    public byte[]? BannnerImage { get; set; }

}
public class ExamBannerUploadCommandHandler : IRequestHandler<ExamBannerUploadCommand, HeroResult<bool>>
{
    private readonly IPOSPRepository _posprepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userDocumentRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ExamBannerUploadCommandHandler(IPOSPRepository posprepository, IMapper mapper)
    {
        _posprepository = posprepository ?? throw new ArgumentNullException(nameof(posprepository));
        _mapper = mapper;
    }

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<HeroResult<bool>> Handle(ExamBannerUploadCommand banneruploadcommand, CancellationToken cancellationToken)
    {
        var bannerdetailmodel = _mapper.Map<ExamBannerDetailModel>(banneruploadcommand);
        var result = await _posprepository.ExamBannerUpload(bannerdetailmodel);
        return HeroResult<bool>.Success(result);
    }

}
