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

namespace POSP.Core.Features.POSP.Commands.UpdateExamInstructionsDetail
{
    public record UpdateExamInstructionsDetailCommand : IRequest<HeroResult<bool>>
    {
        public string Id { get; set; }
        public string InstructionDetail { get; set; }
        public int PriorityIndex { get; set; }

    }
    public class UpdateExamInstructionsDetailCommandHandler : IRequestHandler<UpdateExamInstructionsDetailCommand, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateExamInstructionsDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateExamInstructionsDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="updateExamInstructionsDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateExamInstructionsDetailCommand updateExamInstructionsDetailCommand, CancellationToken cancellationToken)
        {
            var examInstructionsDetailModel = _mapper.Map<ExamInstructionsDetailModel>(updateExamInstructionsDetailCommand);
            var result = await _pospRepository.UpdateExamInstructionsDetail(examInstructionsDetailModel, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
