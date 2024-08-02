using FluentValidation;
using Insurance.Core.Features.GoDigit.Command.CKYC;
using SharpCompress.Archives;

namespace Insurance.Core.Features.GoDigit.Command.SaveCKYC;

public class SaveCKYCValidator : AbstractValidator<GoDigitCKYCCommand>
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
