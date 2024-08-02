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

namespace POSP.Core.Features.POSP.Commands.InsertCertificateDocumentId;


/// <summary>
/// 
/// </summary>
public record CertificacateCommand : IRequest<HeroResult<ExamCertificateModel>>
{
    public string? UserId { get; set; }
    public string? DocumentId { get; set; }
}





public class CertificateDocumentIdCommandHandler : IRequestHandler<CertificacateCommand, HeroResult<ExamCertificateModel>>
{
    private readonly IPOSPRepository _posprepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userDocumentRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CertificateDocumentIdCommandHandler(IPOSPRepository posprepository, IMapper mapper)
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
    public async Task<HeroResult<ExamCertificateModel>> Handle(CertificacateCommand banneruploadcommand, CancellationToken cancellationToken)
    {
        var bannerdetailmodel = _mapper.Map<ExamCertificateModel>(banneruploadcommand);
        var result = await _posprepository.InsertDOcumentaId(bannerdetailmodel);
        return HeroResult<ExamCertificateModel>.Success(result);
        //return HeroResult<ExamCertificateModel>.Success(result);
    }

}