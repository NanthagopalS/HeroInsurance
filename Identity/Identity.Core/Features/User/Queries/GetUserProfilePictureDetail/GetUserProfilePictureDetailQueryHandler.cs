using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Queries.GetUserBreadcrumStatusDetail;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.GetUserProfilePictureDetail
{
    public record GetUserProfilePictureDetailQuery : IRequest<HeroResult<IEnumerable<GetUserProfilePictureDetailVm>>>
    {
        public string UserId { get; set; }
    }
    public class GetUserProfilePictureDetailQueryHandler : IRequestHandler<GetUserProfilePictureDetailQuery, HeroResult<IEnumerable<GetUserProfilePictureDetailVm>>>
    {
        private readonly IUserRepository _quoteRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public GetUserProfilePictureDetailQueryHandler(IUserRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetUserProfilePictureDetailVm>>> Handle(GetUserProfilePictureDetailQuery request, CancellationToken cancellationToken)
        {
            var userProfilePictureDetailResult = await _quoteRepository.GetUserProfilePictureDetail(request.UserId,cancellationToken).ConfigureAwait(false);
            if (userProfilePictureDetailResult.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetUserProfilePictureDetailVm>>(userProfilePictureDetailResult);
                return HeroResult<IEnumerable<GetUserProfilePictureDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetUserProfilePictureDetailVm>>.Fail("No Record Found");
        }
    }
}
