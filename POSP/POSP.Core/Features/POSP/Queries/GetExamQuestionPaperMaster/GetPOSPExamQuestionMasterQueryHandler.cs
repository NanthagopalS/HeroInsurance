using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetExamQuestionPaperMaster
{
    public class GetPOSPExamQuestionMasterQuery : IRequest<HeroResult<IEnumerable<GetPOSPExamQuestionPaperMasterVm>>>
    {

    }

    public class GetPOSPExamQuestionMasterQueryHandler : IRequestHandler<GetPOSPExamQuestionMasterQuery, HeroResult<IEnumerable<GetPOSPExamQuestionPaperMasterVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPExamQuestionMasterQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPExamQuestionPaperMasterVm>>> Handle(GetPOSPExamQuestionMasterQuery request, CancellationToken cancellationToken)
        {
            var POSPConfigurationDetail = await _pospRepository.GetPOSPExamQuestionPaperMaster(cancellationToken).ConfigureAwait(false);
            if (POSPConfigurationDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPExamQuestionPaperMasterVm>>(POSPConfigurationDetail);
                return HeroResult<IEnumerable<GetPOSPExamQuestionPaperMasterVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPExamQuestionPaperMasterVm>>.Fail("No Record Found");
        }
    }
}
