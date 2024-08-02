using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Commands.VerifyEmail;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Commands.SendCompletedRegisterationMail
{
    public class SendCompletedRegisterationMailQuery : IRequest<HeroResult<bool>>
    {
        public string UserId { get; set; }
    }
    public class SendCompletedRegisterationMailQueryHandler : IRequestHandler<SendCompletedRegisterationMailQuery, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public SendCompletedRegisterationMailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<bool>> Handle(SendCompletedRegisterationMailQuery request, CancellationToken cancellationToken)
        {
            var userModel = _mapper.Map<SendCompletedRegisterationMailResponseModel>(request);
            var result = await _userRepository.SendCompletedRegisterationMail(userModel.UserId, cancellationToken);

            return HeroResult<bool>.Success(result);
        }

    }
}
