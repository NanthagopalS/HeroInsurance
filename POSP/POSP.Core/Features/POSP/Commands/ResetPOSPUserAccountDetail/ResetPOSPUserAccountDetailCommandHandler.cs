using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.ResetPOSPUserAccountDetail
{
    public record ResetPOSPUserAccountDetailCommand : IRequest<HeroResult<bool>>
    {
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }
    }
    public class ResetPOSPUserAccountDetailCommandHandler : IRequestHandler<ResetPOSPUserAccountDetailCommand, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ResetPOSPUserAccountDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<bool>> Handle(ResetPOSPUserAccountDetailCommand request, CancellationToken cancellationToken)
        {
            var result = await _pospRepository.ResetPOSPUserAccountDetail(request.UserId, cancellationToken).ConfigureAwait(false);

            return HeroResult<bool>.Success(result);
        }
    }
}
