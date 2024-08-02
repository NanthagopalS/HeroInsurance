using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Queries.GetTrainingMaterial;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetManagePOSPTraining
{

    public record GetPOSPTrainingQuery : IRequest<HeroResult<IEnumerable<GetPOSPTrainingVm>>>
    {

    }

    public class GetPOSPTrainingQueryHandler : IRequestHandler<GetPOSPTrainingQuery, HeroResult<IEnumerable<GetPOSPTrainingVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPTrainingQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPTrainingVm>>> Handle(GetPOSPTrainingQuery request, CancellationToken cancellationToken)
        {
            var POSPConfigurationDetail = await _pospRepository.GetPOSPTraining(cancellationToken).ConfigureAwait(false);
            if (POSPConfigurationDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPTrainingVm>>(POSPConfigurationDetail);
                return HeroResult<IEnumerable<GetPOSPTrainingVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPTrainingVm>>.Fail("No Record Found");
        }
    }
}
