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

namespace POSP.Core.Features.POSP.Queries.GetExamParticularQuestionDetail
{
    public class GetPOSPExamParticularQuestionDetailQuery : IRequest<HeroResult<GetPOSPExamParticularQuestionDetailVm>>
    
    {
        public string UserId { get; set; }
        public string ExamId { get; set; }
        public int QuestionNo { get; set; }

    }

    public class GetPOSPExamParticularQuestionDetailQueryHandler : IRequestHandler<GetPOSPExamParticularQuestionDetailQuery, HeroResult<GetPOSPExamParticularQuestionDetailVm>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPExamParticularQuestionDetailQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<HeroResult<GetPOSPExamParticularQuestionDetailVm>> Handle(GetPOSPExamParticularQuestionDetailQuery request, CancellationToken cancellationToken)
        {
            var getPOSPExamParticularQuestionResult = await _pospRepository.GetPOSPExamParticularQuestionDetail(request.UserId, request.ExamId, request.QuestionNo,
                cancellationToken).ConfigureAwait(false);

            if (getPOSPExamParticularQuestionResult != null)
            {
                var result = _mapper.Map<GetPOSPExamParticularQuestionDetailVm>(getPOSPExamParticularQuestionResult);

                return HeroResult<GetPOSPExamParticularQuestionDetailVm>.Success(result);
            }

            return HeroResult<GetPOSPExamParticularQuestionDetailVm>.Fail("No record found");
        }
    }

}
