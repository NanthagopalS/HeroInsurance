using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Commands.InsertPOSPRating;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetPOSPRating
{
    public class GetPOSPRatingQuery : IRequest<HeroResult<IEnumerable<GetPOSPRatingVm>>>
    {
        public string UserId { get; set; }

    }

    public class GetPOSPRatingQueryHandler : IRequestHandler<GetPOSPRatingQuery, HeroResult<IEnumerable<GetPOSPRatingVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPRatingQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPRatingVm>>> Handle(GetPOSPRatingQuery request, CancellationToken cancellationToken)
        {
            var POSPConfigurationDetail = await _pospRepository.GetPOSPRating(request.UserId, cancellationToken).ConfigureAwait(false);
            if (POSPConfigurationDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPRatingVm>>(POSPConfigurationDetail);
                return HeroResult<IEnumerable<GetPOSPRatingVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPRatingVm>>.Fail("No Record Found");
        }
    }

}
