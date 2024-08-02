using FluentValidation;
using Identity.Core.Features.Registration.Commands.UpdateAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.Registration.Commands.AuthenticateAdmin
{
    internal class AuthenticateCreationValidator<T> : AbstractValidator<T> where T : UpdateAdminCommand
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public AuthenticateCreationValidator()
        {

            //RuleFor(v => v.EmailId)
            //    .MaximumLength(100)
            //    .EmailAddress()
            //    .NotEmpty()
            //    .NotNull()
            //    .When(x => !string.IsNullOrWhiteSpace(x.EmailId)).WithMessage("Email Id is not valid");

            //RuleFor(v => v.PassWord)
            // .MaximumLength(20)
            // .MinimumLength(6)
            // .NotEmpty()
            // .NotNull()
            // .Equal(string.Empty)
            // .When(x => !string.IsNullOrWhiteSpace(x.PassWord)).WithMessage("PassWord is not valid");


        }
    }
    
    
}
