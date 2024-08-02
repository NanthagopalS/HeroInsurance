using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.GetPOSPConfigurationDetail
{
    public record GetPOSPConfigurationDetailQuery : IRequest<HeroResult<IEnumerable<GetPOSPConfigurationDetailVm>>>
    {

    }
    public class GetPOSPConfigurationDetailQueryHandler : IRequestHandler<GetPOSPConfigurationDetailQuery, HeroResult<IEnumerable<GetPOSPConfigurationDetailVm>>>
    {
        private readonly IUserRepository _quoteRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPConfigurationDetailQueryHandler(IUserRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPConfigurationDetailVm>>> Handle(GetPOSPConfigurationDetailQuery request, CancellationToken cancellationToken)
        {
            var POSPConfigurationDetail = await _quoteRepository.GetPOSPConfigurationDetail(cancellationToken).ConfigureAwait(false);
            if (POSPConfigurationDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPConfigurationDetailVm>>(POSPConfigurationDetail);
                return HeroResult<IEnumerable<GetPOSPConfigurationDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPConfigurationDetailVm>>.Fail("No Record Found");
        }
    }
}
