using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Commands.UserProfilePicture
{
    public record UserProfilePictureCommand : IRequest<HeroResult<bool>>
    {


        /// <summary>
        /// UserId
        /// </summary>
       //[Required]
        public string UserId { get; set; }

        /// <summary>
        /// ProfilePictureID
        /// </summary>
        public string ProfilePictureID { get; set; }

        /// <summary>
        /// ProfilePictureFileName
        /// </summary>
        public string ProfilePictureFileName { get; set; }


        /// <summary>
        /// ProfilePictureStoragePath
        /// </summary>
        public string ProfilePictureStoragePath { get; set; }


        /// <summary>
        /// ImageStream
        /// </summary>
        public byte[] ImageStream { get; set; }
    }
    public class UserProfilePictureCommandHandler : IRequestHandler<UserProfilePictureCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userProfilePictureRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userProfilePictureRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserProfilePictureCommandHandler(IUserRepository userProfilePictureRepository, IMapper mapper)
        {
            _userProfilePictureRepository = userProfilePictureRepository ?? throw new ArgumentNullException(nameof(userProfilePictureRepository));
            _mapper = mapper;
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<bool>> Handle(UserProfilePictureCommand userProfilePictureCommand, CancellationToken cancellationToken)
        {
            var userProfilePictureModel = _mapper.Map<UserProfilePictureModel>(userProfilePictureCommand);
            var result = await _userProfilePictureRepository.UserProfilePictureUpload(userProfilePictureModel);
            return HeroResult<bool>.Success(result);
        }
    }
}
