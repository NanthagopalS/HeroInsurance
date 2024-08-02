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

namespace POSP.Core.Features.POSP.Commands.InsertPOSPTrainingProgressDetail
{
    public record InsertPOSPTrainingProgressDetailCommand : IRequest<HeroResult<bool>>
    {
        public string UserId { get; set; }
        public string TrainingMaterialId { get; set; }

    }

    public class InsertPOSPTrainingProgressDetailCommandHandler : IRequestHandler<InsertPOSPTrainingProgressDetailCommand, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// InsertPOSPTrainingProgressDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertPOSPTrainingProgressDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="insertPOSPTrainingProgressDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(InsertPOSPTrainingProgressDetailCommand insertPOSPTrainingProgressDetailCommand, CancellationToken cancellationToken)
        {
            var trainingProgressDetailModel = _mapper.Map<TrainingProgressDetailModel>(insertPOSPTrainingProgressDetailCommand);
            var result = await _pospRepository.InsertPOSPTrainingProgressDetail(trainingProgressDetailModel, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
