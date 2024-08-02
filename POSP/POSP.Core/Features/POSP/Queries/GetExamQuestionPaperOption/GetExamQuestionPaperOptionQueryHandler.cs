using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetExamQuestionPaperOption;
public record GetExamQuestionPaperOptionlQuery : IRequest<HeroResult<IEnumerable<GetExamQuestionPaperOptionMasterVm>>>
{

}
public class GetExamQuestionPaperOptionQueryHandler : IRequestHandler<GetExamQuestionPaperOptionlQuery, HeroResult<IEnumerable<GetExamQuestionPaperOptionMasterVm>>>
{
    private readonly IPOSPRepository _pospRepository;
    private readonly IMapper _mapper;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="pospRepository"></param>
    /// <param name="mapper"></param>
    public GetExamQuestionPaperOptionQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
    {
        _pospRepository = pospRepository;
        _mapper = mapper;
    }

    public async Task<HeroResult<IEnumerable<GetExamQuestionPaperOptionMasterVm>>> Handle(GetExamQuestionPaperOptionlQuery request, CancellationToken cancellationToken)
    {
        var POSPConfigurationDetail = await _pospRepository.GetExamQuestionPaperOption(cancellationToken).ConfigureAwait(false);
        if (POSPConfigurationDetail.Any())
        {
            var listInsurer = _mapper.Map<IEnumerable<GetExamQuestionPaperOptionMasterVm>>(POSPConfigurationDetail);
            return HeroResult<IEnumerable<GetExamQuestionPaperOptionMasterVm>>.Success(listInsurer);
        }

        return HeroResult<IEnumerable<GetExamQuestionPaperOptionMasterVm>>.Fail("No Record Found");
    }
}
