using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetExamInstructionsDetail
{
    public record GetExamInstructionsDetailQuery : IRequest<HeroResult<IEnumerable<GetExamInstructionsDetailVm>>>
    {

    }


    public class GetExamInstructionsDetailQueryHandler : IRequestHandler<GetExamInstructionsDetailQuery, HeroResult<IEnumerable<GetExamInstructionsDetailVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetExamInstructionsDetailQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetExamInstructionsDetailVm>>> Handle(GetExamInstructionsDetailQuery request, CancellationToken cancellationToken)
        {
            var getExamInstructionsDetail = await _pospRepository.GetExamInstructionsDetail(cancellationToken).ConfigureAwait(false);
            if (getExamInstructionsDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetExamInstructionsDetailVm>>(getExamInstructionsDetail);
                return HeroResult<IEnumerable<GetExamInstructionsDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetExamInstructionsDetailVm>>.Fail("No Record Found");
        }
    }
}
