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

namespace POSP.Core.Features.POSP.Commands.InsertExamCertificate;


/// <summary>
/// 
/// </summary>
public record ExamCertificateUploadCommand : IRequest<HeroResult<bool>>
{
    /// <summary>
    /// Id
    /// </summary>
    public string? Id { get; set; }

    public string? BannerFileName { get; set; }

    /// <summary>
    /// BannnerImage
    /// </summary>
    public byte[]? BannnerImage { get; set; }

    public string? DocumentId { get; set; }

    public string? Image64 { get; set; }

}



public class ExamCertificateUploadCommandHandler : IRequestHandler<ExamCertificateUploadCommand, HeroResult<bool>>
{
    private readonly IPOSPRepository _posprepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userDocumentRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ExamCertificateUploadCommandHandler(IPOSPRepository posprepository, IMapper mapper)
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
    public async Task<HeroResult<bool>> Handle(ExamCertificateUploadCommand examcertificateuploadcommand, CancellationToken cancellationToken)
    {
        var certificatedetailmodel = _mapper.Map<ExamCertificateModel>(examcertificateuploadcommand);
        var result = await _posprepository.ExamCertificateUpload(certificatedetailmodel);
        return HeroResult<bool>.Success(result);
    }

}