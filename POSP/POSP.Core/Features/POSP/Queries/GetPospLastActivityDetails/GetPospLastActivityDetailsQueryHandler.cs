using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Queries.GetExamParticularQuestionStatus;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetPospLastActivityDetails
{
    public class GetPospLastLogInDetailsQuery : IRequest<HeroResult<IEnumerable<GetPospLastLogInDetailsVm>>>
    {
        public string? UserId { get; set; }

    }
    public class GetPospLastLogInDetailsQueryHandler : IRequestHandler<GetPospLastLogInDetailsQuery, HeroResult<IEnumerable<GetPospLastLogInDetailsVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPospLastLogInDetailsQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPospLastLogInDetailsVm>>> Handle(GetPospLastLogInDetailsQuery request, CancellationToken cancellationToken)
        {
            var lastActivity = await _pospRepository.GetPospLastLogInDetails(request.UserId, cancellationToken).ConfigureAwait(false);
            if (lastActivity.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPospLastLogInDetailsVm>>(lastActivity);
                return HeroResult<IEnumerable<GetPospLastLogInDetailsVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPospLastLogInDetailsVm>>.Fail("No Record Found");
        }
    }
}
