using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Commands.UpdateExamInstructionsDetail;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.UpdatePOSPTrainingDetail
{

    public record UpdatePOSPTrainingDetailCommand : IRequest<HeroResult<POSPTrainingModel>>
    {
        public string UserId { get; set; }
        public string TrainingModuleType { get; set; }

    }
    public class UpdatePOSPTrainingDetailCommandHandler : IRequestHandler<UpdatePOSPTrainingDetailCommand, HeroResult<POSPTrainingModel>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateExamInstructionsDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdatePOSPTrainingDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
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
        public async Task<HeroResult<POSPTrainingModel>> Handle(UpdatePOSPTrainingDetailCommand updatePOSPTrainingDetailCommand, CancellationToken cancellationToken)
        {
            var pospTrainingDetailModel = _mapper.Map<POSPTrainingModel>(updatePOSPTrainingDetailCommand);
            var result = await _pospRepository.UpdatePOSPTrainingDetail(pospTrainingDetailModel, cancellationToken);
            return HeroResult<POSPTrainingModel>.Success(result);

        }
    }
}

