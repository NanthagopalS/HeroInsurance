using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetExamQuestionStatusMaster
{
    public record GetExamQuestionStatusMasterQuery : IRequest<HeroResult<IEnumerable<GetExamQuestionStatusMasterVm>>>
    {

    }


    public class GetExamQuestionStatusMasterQueryHandler : IRequestHandler<GetExamQuestionStatusMasterQuery, HeroResult<IEnumerable<GetExamQuestionStatusMasterVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetExamQuestionStatusMasterQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetExamQuestionStatusMasterVm>>> Handle(GetExamQuestionStatusMasterQuery request, CancellationToken cancellationToken)
        {
            var getExamQuestionStatusMaster = await _pospRepository.GetExamQuestionStatusMaster(cancellationToken).ConfigureAwait(false);
            if (getExamQuestionStatusMaster.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetExamQuestionStatusMasterVm>>(getExamQuestionStatusMaster);
                return HeroResult<IEnumerable<GetExamQuestionStatusMasterVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetExamQuestionStatusMasterVm>>.Fail("No Record Found");
        }
    }
}
