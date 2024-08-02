using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Queries.GetBenefitsDetail;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.GetBenefitsDetail
{
    public record GetBenefitsDetailQuery : IRequest<HeroResult<IEnumerable<GetBenefitsDetailvm>>>;
    public class GetBenefitsDetailQueryHandler : IRequestHandler<GetBenefitsDetailQuery, HeroResult<IEnumerable<GetBenefitsDetailvm>>>
    {
        private readonly IUserRepository _benifitsRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public GetBenefitsDetailQueryHandler(IUserRepository quoteRepository, IMapper mapper)
        {
            _benifitsRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetBenefitsDetailvm>>> Handle(GetBenefitsDetailQuery request, CancellationToken cancellationToken)
        {
            var benifitsDetailResult = await _benifitsRepository.GetBenefitsDetail(cancellationToken).ConfigureAwait(false);
            if (benifitsDetailResult.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetBenefitsDetailvm>>(benifitsDetailResult);
                return HeroResult<IEnumerable<GetBenefitsDetailvm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetBenefitsDetailvm>>.Fail("No Record Found");
        }
    }
}
