using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Commands.InsertPOSPTrainingDetail;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.InsertPOSPRating
{
    public record InsertPOSPRatingCommand : IRequest<HeroResult<POSPRatingResponseModel>>
    {
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }

    }

    public class InsertPOSPRatingCommandHandler : IRequestHandler<InsertPOSPRatingCommand, HeroResult<POSPRatingResponseModel>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// InsertBenefitsDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertPOSPRatingCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="insertPOSPRatingCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<POSPRatingResponseModel>> Handle(InsertPOSPRatingCommand insertPOSPRatingCommand, CancellationToken cancellationToken)
        {
            var pospTrainingDetailResponseModel = _mapper.Map<POSPRatingResponseModel>(insertPOSPRatingCommand);
            var result = await _pospRepository.InsertPOSPRating(insertPOSPRatingCommand.UserId, insertPOSPRatingCommand.Rating, insertPOSPRatingCommand.Description, cancellationToken);
            return HeroResult<POSPRatingResponseModel>.Success(result);
        }
    }
}
