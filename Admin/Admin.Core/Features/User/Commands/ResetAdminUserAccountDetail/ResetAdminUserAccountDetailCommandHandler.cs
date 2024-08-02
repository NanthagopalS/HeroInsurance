using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.ResetAdminUserAccountDetail
{
        public record ResetAdminUserAccountDetailCommand : IRequest<HeroResult<bool>>
        {
            /// <summary>
            /// UserId
            /// </summary>
            public string UserId { get; set; }
        }
        public class ResetAdminUserAccountDetailCommandHandler : IRequestHandler<ResetAdminUserAccountDetailCommand, HeroResult<bool>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;

            /// <summary>
            /// Initialization
            /// </summary>
            /// <param name="AdminRepository"></param>
            /// <param name="mapper"></param>
            /// <exception cref="ArgumentNullException"></exception>
            public ResetAdminUserAccountDetailCommandHandler(IUserRepository userRepository, IMapper mapper)
            {
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
                _mapper = mapper;
            }

            /// <summary>
            /// Handler
            /// </summary>
            /// <param name="request"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            public async Task<HeroResult<bool>> Handle(ResetAdminUserAccountDetailCommand request, CancellationToken cancellationToken)
            {
                var result = await _userRepository.ResetAdminUserAccountDetail(request.UserId, cancellationToken).ConfigureAwait(false);

                return HeroResult<bool>.Success(result);
            }
        }
    
}
