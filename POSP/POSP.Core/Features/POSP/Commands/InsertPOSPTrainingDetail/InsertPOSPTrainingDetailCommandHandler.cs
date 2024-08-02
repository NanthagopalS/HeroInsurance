using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Commands.InsertExamInstructionsDetail;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.InsertPOSPTrainingDetail
{

    public record InsertPOSPTrainingDetailCommand : IRequest<HeroResult<POSPResponseTrainingModel>>
    {
        public string UserId { get; set; }
        public string TrainingStatus { get; set; }
        public string TrainingId { get; set; }

    }
    public class InsertPOSPTrainingDetailCommandHandler : IRequestHandler<InsertPOSPTrainingDetailCommand, HeroResult<POSPResponseTrainingModel>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// InsertExamInstructionsDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertPOSPTrainingDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="InsertPOSPTrainingDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<POSPResponseTrainingModel>> Handle(InsertPOSPTrainingDetailCommand insertPOSPTrainingDetailCommand, CancellationToken cancellationToken)
        {
            var pospTrainingDetailResponseModel = _mapper.Map<POSPResponseTrainingModel>(insertPOSPTrainingDetailCommand);
            var result = await _pospRepository.InsertPOSPTrainingDetail(insertPOSPTrainingDetailCommand.UserId, insertPOSPTrainingDetailCommand.TrainingStatus, insertPOSPTrainingDetailCommand.TrainingId, cancellationToken);
            return HeroResult<POSPResponseTrainingModel>.Success(result);
        }
    }

}

