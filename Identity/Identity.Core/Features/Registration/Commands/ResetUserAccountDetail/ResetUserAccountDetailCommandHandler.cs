using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.UserCreation;
using Identity.Core.Responses;
using MediatR;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.Registration.Commands.ResetUserAccountDetail
{
    public record ResetUserAccountDetailCommand : IRequest<HeroResult<bool>>
    {
        /// <summary>
        /// Mobile No
        /// </summary>
        public string MobileNo { get; set; }
    }
    public class ResetUserAccountDetailCommandHandler : IRequestHandler<ResetUserAccountDetailCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userCreationRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userCreationRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ResetUserAccountDetailCommandHandler(IUserRepository userCreationRepository, IMapper mapper)
        {
            _userCreationRepository = userCreationRepository ?? throw new ArgumentNullException(nameof(userCreationRepository));
            _mapper = mapper;
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<bool>> Handle(ResetUserAccountDetailCommand request, CancellationToken cancellationToken)
        {
            var result = await _userCreationRepository.ResetUserAccountDetail(request.MobileNo, cancellationToken).ConfigureAwait(false);

            return HeroResult<bool>.Success(result);
        }
    }
}
