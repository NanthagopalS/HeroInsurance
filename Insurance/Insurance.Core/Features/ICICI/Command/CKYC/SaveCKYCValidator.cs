using FluentValidation;

namespace Insurance.Core.Features.ICICI.Command.CKYC;

public class SaveCKYCValidator : AbstractValidator<ICICICKYCCommand>
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
