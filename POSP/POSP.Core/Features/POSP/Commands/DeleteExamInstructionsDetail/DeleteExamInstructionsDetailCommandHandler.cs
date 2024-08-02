using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.DeleteExamInstructionsDetail
{
    public record DeleteExamInstructionsDetailQuery : IRequest<HeroResult<bool>>
    {
        public string Id { get; set; }
    }

    public class DeleteExamInstructionsDetailCommand : IRequestHandler<DeleteExamInstructionsDetailQuery, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public DeleteExamInstructionsDetailCommand(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(DeleteExamInstructionsDetailQuery request, CancellationToken cancellationToken)
        {
            var result = await _pospRepository.DeleteExamInstructionsDetail(request.Id, cancellationToken).ConfigureAwait(false);

            return HeroResult<bool>.Success(result);
        }
    }
}
