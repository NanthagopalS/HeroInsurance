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

namespace POSP.Core.Features.POSP.Commands.UpdateTrainingMaterialDetail
{
    public record UpdateTrainingMaterialDetailCommand : IRequest<HeroResult<bool>>
    {
        public string Id { get; set; }
        public string TrainingModuleType { get; set; }
        public string MaterialFormatType { get; set; }
        public string? VideoDuration { get; set; }
        public string LessonNumber { get; set; }
        public string LessonTitle { get; set; }
        public string DocumentFileName { get; set; }
        public int PriorityIndex { get; set; }

    }
    public class UpdateTrainingMaterialDetailCommandHandler : IRequestHandler<UpdateTrainingMaterialDetailCommand, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateTrainingMaterialDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateTrainingMaterialDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="updateTrainingMaterialDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateTrainingMaterialDetailCommand updateTrainingMaterialDetailCommand, CancellationToken cancellationToken)
        {
            var trainingMaterialDetailModel = _mapper.Map<TrainingMaterialDetailModel>(updateTrainingMaterialDetailCommand);
            var result = await _pospRepository.UpdateTrainingMaterialDetail(trainingMaterialDetailModel, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
