using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetHtmlDocumentId;


public class GetHtmlDocumentDetailQuery : IRequest<HeroResult<GetHtmlDocumentDetailVm>>
{
    public string ConfigurationKey { get; set; }
}


public  class GetHtmlDocumentDetailQueryHandler : IRequestHandler<GetHtmlDocumentDetailQuery, HeroResult<GetHtmlDocumentDetailVm>>
{
    private readonly IPOSPRepository _pospRepository;
    private readonly IMapper _mapper;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="pospRepository"></param>
    /// <param name="mapper"></param>
    public GetHtmlDocumentDetailQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
    {
        _pospRepository = pospRepository;
        _mapper = mapper;
    }

    public async Task<HeroResult<GetHtmlDocumentDetailVm>> Handle(GetHtmlDocumentDetailQuery request, CancellationToken cancellationToken)
    {
        var getExamInstructionsDetail = await _pospRepository.GetHtmldocuentId(request.ConfigurationKey,cancellationToken).ConfigureAwait(false);
        if (getExamInstructionsDetail != null)
        {
            var listInsurer = _mapper.Map<GetHtmlDocumentDetailVm>(getExamInstructionsDetail);
            return HeroResult<GetHtmlDocumentDetailVm>.Success(listInsurer);
        }

        return HeroResult<GetHtmlDocumentDetailVm>.Fail("No Record Found");
    }
}
