using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.GetUserDetail
{
    public record GetUserDetailQuery : IRequest<HeroResult<IEnumerable<GetUserDetailVm>>>
    {
        public string UserId { get; set; }
    }
    public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, HeroResult<IEnumerable<GetUserDetailVm>>>
    {
        private readonly IUserRepository _quoteRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public GetUserDetailQueryHandler(IUserRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetUserDetailVm>>> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
        {
            var userDetailResult = await _quoteRepository.GetUserDetail(request.UserId,
                                                              cancellationToken).ConfigureAwait(false);
            if (userDetailResult.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetUserDetailVm>>(userDetailResult);
                return HeroResult<IEnumerable<GetUserDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetUserDetailVm>>.Fail("No Record Found");
        }
    }
}
