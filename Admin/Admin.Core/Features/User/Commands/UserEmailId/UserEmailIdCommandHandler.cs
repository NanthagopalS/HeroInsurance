using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.User;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.UserEmailId
{
    public record UserEmailIdCommand : IRequest<HeroResult<bool>>
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Email Id
        /// </summary>
        public string EmailId { get; set; }

       

    }

    /// <summary>
    /// Handler for UserEmailId
    /// </summary>
    public class UserEmailIdCommandHandler : IRequestHandler<UserEmailIdCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userEmailIdRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userPersonalDetailRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserEmailIdCommandHandler(IUserRepository userEmailIdRepository, IMapper mapper)
        {
            _userEmailIdRepository = userEmailIdRepository ?? throw new ArgumentNullException(nameof(userEmailIdRepository));
            _mapper = mapper;
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<bool>> Handle(UserEmailIdCommand userEmailIdCommand, CancellationToken cancellationToken)
        {
            var userModel = _mapper.Map<UserModel>(userEmailIdCommand);
            var result = await _userEmailIdRepository.UpdateUserEmailIdDetail(userModel, cancellationToken);

            return HeroResult<bool>.Success(result);
        }
    }

}
