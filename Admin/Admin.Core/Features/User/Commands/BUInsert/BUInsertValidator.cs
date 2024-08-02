using FluentValidation;
using Admin.Core.Features.User.Commands.UserEmailId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.BUInsert
{
    public class BUInsertValidator<T> : AbstractValidator<T> where T :  BUInsertCommand
    {
        public BUInsertValidator()
        {
            //RuleFor(v => v.BUName)
            //    .MaximumLength(100)
            //    .MinimumLength(2)
            //    .NotEmpty()
            //    .NotNull()
            //    .Equal(string.Empty)
            //    .When(x => !string.IsNullOrWhiteSpace(x.BUName)).WithMessage("BUName is not valid");

            //RuleFor(v => v.Roletypeid)               
            //    .NotEmpty()
            //    .NotNull()               
            //    .When(x => !string.IsNullOrWhiteSpace(x.Roletypeid.ToString())).WithMessage("Roletypeid is Mandatory");

        }
    }
}
