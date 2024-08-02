using FluentValidation;
using Insurance.Core.Features.Bajaj.Command.CKYC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.Chola.Command.CKYC
{
    public class SaveCKYCValidator : AbstractValidator<CholaCKYCCommand>
    {
        public SaveCKYCValidator()
        {
            RuleFor(v => v.QuoteTransactionId)
                .NotEmpty()
                .WithMessage("Quote Transaciton ID is not valid");

            RuleFor(v => v.InsurerId)
                .NotEmpty()
                .WithMessage("InsurerId is not valid");
        }
    }
}
