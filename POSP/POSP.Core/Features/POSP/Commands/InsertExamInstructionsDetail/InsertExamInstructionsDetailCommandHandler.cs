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

namespace POSP.Core.Features.POSP.Commands.InsertExamInstructionsDetail
{
    public record InsertExamInstructionsDetailCommand : IRequest<HeroResult<bool>>
    {
        public string InstructionDetail { get; set; }
        public int PriorityIndex { get; set; }

    }

    public class InsertExamInstructionsDetailCommandHandler : IRequestHandler<InsertExamInstructionsDetailCommand, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// InsertExamInstructionsDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertExamInstructionsDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
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
        public async Task<HeroResult<bool>> Handle(InsertExamInstructionsDetailCommand insertExamInstructionsDetailCommand, CancellationToken cancellationToken)
        {
            var examInstructionsDetailModel = _mapper.Map<ExamInstructionsDetailModel>(insertExamInstructionsDetailCommand);
            var result = await _pospRepository.InsertExamInstructionsDetail(examInstructionsDetailModel, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }

}
