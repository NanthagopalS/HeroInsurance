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

namespace POSP.Core.Features.POSP.Commands.UpdatePOSPExamQuestionAsweredDetail
{
    public record UpdatePOSPExamQuestionAsweredDetailCommand : IRequest<HeroResult<bool>>
    {
        public string UserId { get; set; }
        public string ExamId { get; set; }
        public string QuestionNo { get; set; }
        public string QuestionId { get; set; }
        public string AnswerOptionId { get; set; }

    }
    public class UpdatePOSPExamQuestionAsweredDetailCommandHandler : IRequestHandler<UpdatePOSPExamQuestionAsweredDetailCommand, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdatePOSPExamQuestionAsweredDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdatePOSPExamQuestionAsweredDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="updatePOSPExamQuestionAsweredDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdatePOSPExamQuestionAsweredDetailCommand updatePOSPExamQuestionAsweredDetailCommand, CancellationToken cancellationToken)
        {
            var examQuestionAsweredDetailModel = _mapper.Map<ExamQuestionAsweredDetailModel>(updatePOSPExamQuestionAsweredDetailCommand);
            var result = await _pospRepository.UpdatePOSPExamQuestionAsweredDetail(examQuestionAsweredDetailModel, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
