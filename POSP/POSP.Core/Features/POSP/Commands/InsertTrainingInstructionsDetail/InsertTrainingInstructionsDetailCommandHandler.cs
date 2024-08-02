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

namespace POSP.Core.Features.POSP.Commands.InsertTrainingInstructionsDetail
{
    public record InsertTrainingInstructionsDetailCommand : IRequest<HeroResult<bool>>
    {
        public string InstructionDetail { get; set; }
        public int PriorityIndex { get; set; }

    }

    public class InsertTrainingInstructionsDetailCommandHandler : IRequestHandler<InsertTrainingInstructionsDetailCommand, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// InsertBenefitsDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertTrainingInstructionsDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="insertExamInstructionsDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(InsertTrainingInstructionsDetailCommand insertTrainingInstructionsDetailCommand, CancellationToken cancellationToken)
        {
            var trainingInstructionsDetailModel = _mapper.Map<TrainingInstructionsDetailModel>(insertTrainingInstructionsDetailCommand);
            var result = await _pospRepository.InsertTrainingInstructionsDetail(trainingInstructionsDetailModel, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
