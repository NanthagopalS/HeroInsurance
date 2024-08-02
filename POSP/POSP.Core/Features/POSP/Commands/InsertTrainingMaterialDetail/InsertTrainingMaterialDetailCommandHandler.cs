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

namespace POSP.Core.Features.POSP.Commands.InsertTrainingMaterialDetail
{
    public record InsertTrainingMaterialDetailCommand : IRequest<HeroResult<bool>>
    {
        public string TrainingModuleType { get; set; }
        public string MaterialFormatType { get; set; }
        public string? VideoDuration { get; set; }
        public string LessonNumber { get; set; }
        public string LessonTitle { get; set; }
        public string DocumentFileName { get; set; }
        public int PriorityIndex { get; set; }

    }

    public class InsertTrainingMaterialDetailCommandHandler : IRequestHandler<InsertTrainingMaterialDetailCommand, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// InsertTrainingMaterialDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertTrainingMaterialDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="insertTrainingMaterialDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(InsertTrainingMaterialDetailCommand insertTrainingMaterialDetailCommand, CancellationToken cancellationToken)
        {
            var trainingMaterialDetailModel = _mapper.Map<TrainingMaterialDetailModel>(insertTrainingMaterialDetailCommand);
            var result = await _pospRepository.InsertTrainingMaterialDetail(trainingMaterialDetailModel, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
