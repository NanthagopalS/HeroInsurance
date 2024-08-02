using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.DeleteTrainingInstructionsDetail
{
    public record DeleteTrainingInstructionsDetailQuery : IRequest<HeroResult<bool>>
    {
        public string Id { get; set; }
    }

    public class DeleteTrainingInstructionsDetailCommand : IRequestHandler<DeleteTrainingInstructionsDetailQuery, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public DeleteTrainingInstructionsDetailCommand(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(DeleteTrainingInstructionsDetailQuery request, CancellationToken cancellationToken)
        {
            var result = await _pospRepository.DeleteTrainingInstructionsDetail(request.Id, cancellationToken).ConfigureAwait(false);

            return HeroResult<bool>.Success(result);
        }
    }
}
