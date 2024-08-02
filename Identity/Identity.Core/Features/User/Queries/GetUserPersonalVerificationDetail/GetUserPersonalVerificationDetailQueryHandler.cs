using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.GetUserPersonalVerificationDetail
{
    public record GetUserPersonalVerificationDetailQuery : IRequest<HeroResult<GetUserPersonalVerificationDetailVm>>
    {
        public string UserId { get; set; }
    }
    public class GetUserPersonalVerificationDetailQueryHandler : IRequestHandler<GetUserPersonalVerificationDetailQuery, HeroResult<GetUserPersonalVerificationDetailVm>>
    {

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        public GetUserPersonalVerificationDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<GetUserPersonalVerificationDetailVm>> Handle(GetUserPersonalVerificationDetailQuery request, CancellationToken cancellationToken)
        {
            var getUserPersonalVerificationDetailResult = await _userRepository.GetUserPersonalVerificationDetail(request.UserId,
                cancellationToken);

            if (getUserPersonalVerificationDetailResult != null)
            {
                var result = _mapper.Map<GetUserPersonalVerificationDetailVm>(getUserPersonalVerificationDetailResult);

                return HeroResult<GetUserPersonalVerificationDetailVm>.Success(result);
            }

            return HeroResult<GetUserPersonalVerificationDetailVm>.Fail("No record found");
        }
    }
}
