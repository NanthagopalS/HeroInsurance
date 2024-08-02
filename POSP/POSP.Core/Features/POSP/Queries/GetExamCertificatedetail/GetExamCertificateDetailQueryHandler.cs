using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetExamCertificatedetail;


public class GetExamCertificateDetailQuery : IRequest<HeroResult<IEnumerable<GetExamCertificateDetailVm>>>
{
    public string? UserId { get; set; }
}



public class GetExamCertificateDetailQueryHandler : IRequestHandler<GetExamCertificateDetailQuery, HeroResult<IEnumerable<GetExamCertificateDetailVm>>>
{
    private readonly IPOSPRepository _pospRepository;
    private readonly IMapper _mapper;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="pospRepository"></param>
    /// <param name="mapper"></param>
    public GetExamCertificateDetailQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
    {
        _pospRepository = pospRepository;
        _mapper = mapper;
    }

    public async Task<HeroResult<IEnumerable<GetExamCertificateDetailVm>>> Handle(GetExamCertificateDetailQuery request, CancellationToken cancellationToken)
    {
        var getExamInstructionsDetail = await _pospRepository.GetExamCertificatDetail(request.UserId, cancellationToken).ConfigureAwait(false);
        if (getExamInstructionsDetail.Any())
        {
            var listInsurer = _mapper.Map<IEnumerable<GetExamCertificateDetailVm>>(getExamInstructionsDetail);
            return HeroResult<IEnumerable<GetExamCertificateDetailVm>>.Success(listInsurer);
        }

        return HeroResult<IEnumerable<GetExamCertificateDetailVm>>.Fail("No Record Found");
    }
}