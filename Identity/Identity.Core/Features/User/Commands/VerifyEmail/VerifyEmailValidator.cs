using FluentValidation;
using Identity.Core.Features.User.Commands.UserEmailId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Commands.VerifyEmail
{
    public class VerifyEmailValidator<T> : AbstractValidator<T> where T : VerifyEmailCommand
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public VerifyEmailValidator()
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
