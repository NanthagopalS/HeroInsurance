using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Commands.UserEmailId
{
    public class UserEmailIdValidator<T> : AbstractValidator<T> where T : UserEmailIdCommand
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public UserEmailIdValidator()
        {
            //RuleFor(v => v.UserId)
            //    .MaximumLength(100)
            //    .MinimumLength(1)
            //    .NotEmpty()
            //    .NotNull()
            //    .Equal(string.Empty)
            //    .When(x => !string.IsNullOrWhiteSpace(x.UserId)).WithMessage("User Id is not valid");

            //RuleFor(v => v.EmailId)
            //    .MaximumLength(100)
            //    .MinimumLength(1)
            //    .NotEmpty()
            //    .NotNull()
            //    .Equal(string.Empty)
            //    .When(x => !string.IsNullOrWhiteSpace(x.EmailId)).WithMessage("Email Id is not valid");
           
        }
    }
}
