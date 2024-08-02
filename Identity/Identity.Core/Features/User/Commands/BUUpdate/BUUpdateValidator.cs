using FluentValidation;
using Identity.Core.Features.User.Commands.BUInsert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Commands.BUUpdate
{
    public class BUUpdateValidator<T> : AbstractValidator<T> where T : BUUpdateCommand
    {
        public BUUpdateValidator()
        {
            //RuleFor(v => v.BUID)
            //    .NotEmpty()
            //    .NotEqual(0)               
            //    //.When(x => !string.IsNullOrWhiteSpace(x.BUID.ToString())).WithMessage("BUID is not Null");
            //    .When(x => x.BUID==0).WithMessage("BUID is not Zero");


        }
    }
}
