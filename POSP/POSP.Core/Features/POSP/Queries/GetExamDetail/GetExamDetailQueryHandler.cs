using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetExamDetail;

public class GetExamDetailQueryHandler : IRequest<HeroResult<IEnumerable<GetExamDetailVm>>>
{
    public string Id { get; set; }
    public string UserId { get; set; }
}


public class GetExamInstructionsDetailQueryHandler : IRequestHandler<GetExamDetailQueryHandler, HeroResult<IEnumerable<GetExamDetailVm>>>
{
    private readonly IPOSPRepository _pospRepository;
    private readonly IMapper _mapper;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="pospRepository"></param>
    /// <param name="mapper"></param>
    public GetExamInstructionsDetailQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
    {
        _pospRepository = pospRepository;
        _mapper = mapper;
    }

    public async Task<HeroResult<IEnumerable<GetExamDetailVm>>> Handle(GetExamDetailQueryHandler request, CancellationToken cancellationToken)
    {
        var getExamInstructionsDetail = await _pospRepository.GetExamResultDetail(request.Id, request.UserId, cancellationToken).ConfigureAwait(false);
        if (getExamInstructionsDetail.Any())
        {
            var listInsurer = _mapper.Map<IEnumerable<GetExamDetailVm>>(getExamInstructionsDetail);
            return HeroResult<IEnumerable<GetExamDetailVm>>.Success(listInsurer);
        }

        return HeroResult<IEnumerable<GetExamDetailVm>>.Fail("No Record Found");
    }
}

