using FluentValidation;

namespace Insurance.Core.Features.Quote.Command.SaveUpdateLead
{
    public class SaveUpdateLeadValidator : AbstractValidator<SaveUpdateLeadCommand>
    {
        public SaveUpdateLeadValidator()
        {
            RuleFor(v => v.QuoteTransactionID)
                .NotEmpty()
                .WithMessage("Quote Transaciton ID is not valid");

            RuleFor(v => v.RequestBody)
                .NotEmpty()
                .WithMessage("Request Body is not valid");

            RuleFor(v => v.QuoteTransactionID)
                .MaximumLength(50)
                .MinimumLength(30)
                .WithMessage("Quote Transaciton ID is not valid");
        }
    }
}
