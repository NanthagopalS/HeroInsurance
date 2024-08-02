using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Commands.UserProfilePicture
{
    public class UserProfilePictureValidator<T> : AbstractValidator<T> where T : UserProfilePictureCommand
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public UserProfilePictureValidator()
        {
           // RuleFor(v => v.UserId)
           //.NotNull().When(x => !string.IsNullOrWhiteSpace(x.UserId)).WithMessage("E-10002");

           // RuleFor(v => v.ProfilePictureID)
           //.NotNull().When(x => !string.IsNullOrWhiteSpace(x.ProfilePictureID)).WithMessage("E-10033");

           // RuleFor(v => v.ProfilePictureFileName)
           //.NotNull().When(x => !string.IsNullOrWhiteSpace(x.ProfilePictureFileName)).WithMessage("E-10034");

           // RuleFor(v => v.ProfilePictureStoragePath)
           //.NotNull().When(x => !string.IsNullOrWhiteSpace(x.ProfilePictureStoragePath)).WithMessage("E-10035");
        }
    }
}
