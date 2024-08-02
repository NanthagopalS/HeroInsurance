using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.Registration.Commands.ResetUserAccountDetail
{
    public class ResetUserAccountDetailValidator<T> : AbstractValidator<T> where T : ResetUserAccountDetailCommand
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public ResetUserAccountDetailValidator()
        {
           // RuleFor(v => v.MobileNo)
           //.Length(10)
           //.NotEmpty()
           //.When(x => !string.IsNullOrWhiteSpace(x.MobileNo)).WithMessage("Contact number is not valid");
        }
    }
}
