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

namespace POSP.Core.Features.POSP.Commands.UpdateTrainingInstructionsDetail
{
    public record UpdateTrainingInstructionsDetailCommand : IRequest<HeroResult<bool>>
    {
        public string Id { get; set; }
        public string InstructionDetail { get; set; }
        public int PriorityIndex { get; set; }

    }

    public class UpdateTrainingInstructionsDetailCommandHandler : IRequestHandler<UpdateTrainingInstructionsDetailCommand, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateBenefitsDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateTrainingInstructionsDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="UpdateExamInstructionsDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateTrainingInstructionsDetailCommand UpdateTrainingInstructionsDetailCommand, CancellationToken cancellationToken)
        {
            var trainingInstructionsDetailModel = _mapper.Map<TrainingInstructionsDetailModel>(UpdateTrainingInstructionsDetailCommand);
            var result = await _pospRepository.UpdateTrainingInstructionsDetail(trainingInstructionsDetailModel, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
