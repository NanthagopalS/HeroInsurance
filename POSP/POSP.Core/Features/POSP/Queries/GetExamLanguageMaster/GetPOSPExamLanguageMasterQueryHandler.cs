using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Queries.GetExamQuestionPaperMaster;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetExamLanguageMaster
{
    public class GetPOSPExamLanguageMasterQuery : IRequest<HeroResult<IEnumerable<GetPOSPExamLanguageMasterVm>>>
    {

    }

    public class GetPOSPExamLanguageMasterQueryHandler : IRequestHandler<GetPOSPExamLanguageMasterQuery, HeroResult<IEnumerable<GetPOSPExamLanguageMasterVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPExamLanguageMasterQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPExamLanguageMasterVm>>> Handle(GetPOSPExamLanguageMasterQuery request, CancellationToken cancellationToken)
        {
            var POSPConfigurationDetail = await _pospRepository.GetPOSPExamLanguageMaster(cancellationToken).ConfigureAwait(false);
            if (POSPConfigurationDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPExamLanguageMasterVm>>(POSPConfigurationDetail);
                return HeroResult<IEnumerable<GetPOSPExamLanguageMasterVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPExamLanguageMasterVm>>.Fail("No Record Found");
        }
    }
}
