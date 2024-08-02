using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.GetUserBreadcrumStatusDetail
{
    public record GetUserBreadcrumStatusDetailQuery : IRequest<HeroResult<IEnumerable<GetUserBreadcrumStatusDetailVm>>>
    {
        public string UserId { get; set; }
    }
    public class GetUserBreadcrumStatusDetailQueryHandler : IRequestHandler<GetUserBreadcrumStatusDetailQuery, HeroResult<IEnumerable<GetUserBreadcrumStatusDetailVm>>>
    {
        private readonly IUserRepository _quoteRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public GetUserBreadcrumStatusDetailQueryHandler(IUserRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetUserBreadcrumStatusDetailVm>>> Handle(GetUserBreadcrumStatusDetailQuery request, CancellationToken cancellationToken)
        {
            var userBreadcrumStatusDetailResult = await _quoteRepository.GetUserBreadcrumStatusDetail(request.UserId,
                                                              cancellationToken).ConfigureAwait(false);
            if (userBreadcrumStatusDetailResult.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetUserBreadcrumStatusDetailVm>>(userBreadcrumStatusDetailResult);
                return HeroResult<IEnumerable<GetUserBreadcrumStatusDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetUserBreadcrumStatusDetailVm>>.Fail("No Record Found");
        }
    }
}
