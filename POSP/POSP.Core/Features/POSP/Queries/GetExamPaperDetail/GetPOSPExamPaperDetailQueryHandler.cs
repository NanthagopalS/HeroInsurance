using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Queries.GetExamLanguageMaster;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetExamPaperDetail
{
    public class GetPOSPExamPaperDetailQuery : IRequest<HeroResult<IEnumerable<GetPOSPExamPaperDetailVm>>>
    {

    }
    public class GetPOSPExamPaperDetailQueryHandler : IRequestHandler<GetPOSPExamPaperDetailQuery, HeroResult<IEnumerable<GetPOSPExamPaperDetailVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPExamPaperDetailQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPExamPaperDetailVm>>> Handle(GetPOSPExamPaperDetailQuery request, CancellationToken cancellationToken)
        {
            var POSPConfigurationDetail = await _pospRepository.GetPOSPExamPaperDetail(cancellationToken).ConfigureAwait(false);
            if (POSPConfigurationDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPExamPaperDetailVm>>(POSPConfigurationDetail);
                return HeroResult<IEnumerable<GetPOSPExamPaperDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPExamPaperDetailVm>>.Fail("No Record Found");
        }
    }
}
