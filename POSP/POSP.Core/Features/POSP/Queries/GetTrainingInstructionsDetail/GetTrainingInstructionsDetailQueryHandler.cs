using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetTrainingInstructionsDetail
{
    public record GetTrainingInstructionsDetailQuery : IRequest<HeroResult<IEnumerable<GetTrainingInstructionsDetailVm>>>
    {

    }


    public class GetTrainingInstructionsDetailQueryHandler : IRequestHandler<GetTrainingInstructionsDetailQuery, HeroResult<IEnumerable<GetTrainingInstructionsDetailVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetTrainingInstructionsDetailQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetTrainingInstructionsDetailVm>>> Handle(GetTrainingInstructionsDetailQuery request, CancellationToken cancellationToken)
        {
            var getExamInstructionsDetail = await _pospRepository.GetTrainingInstructionsDetail(cancellationToken).ConfigureAwait(false);
            if (getExamInstructionsDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetTrainingInstructionsDetailVm>>(getExamInstructionsDetail);
                return HeroResult<IEnumerable<GetTrainingInstructionsDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetTrainingInstructionsDetailVm>>.Fail("No Record Found");
        }
    }
}
