using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Queries.GetExamQuestionPaperMaster;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetExamParticularQuestionStatus
{
    public class GetPOSPExamParticularQuestionStatusQuery : IRequest<HeroResult<IEnumerable<GetPOSPExamParticularQuestionStatusVm>>>
    {
        public string ExamId { get; set; }

    }

    public class GetPOSPExamParticularQuestionStatusQueryHandler : IRequestHandler<GetPOSPExamParticularQuestionStatusQuery, HeroResult<IEnumerable<GetPOSPExamParticularQuestionStatusVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPExamParticularQuestionStatusQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPExamParticularQuestionStatusVm>>> Handle(GetPOSPExamParticularQuestionStatusQuery request, CancellationToken cancellationToken)
        {
            var POSPConfigurationDetail = await _pospRepository.GetPOSPExamParticularQuestionStatus(request.ExamId, cancellationToken).ConfigureAwait(false);
            if (POSPConfigurationDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPExamParticularQuestionStatusVm>>(POSPConfigurationDetail);
                return HeroResult<IEnumerable<GetPOSPExamParticularQuestionStatusVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPExamParticularQuestionStatusVm>>.Fail("No Record Found");
        }
    }
}
