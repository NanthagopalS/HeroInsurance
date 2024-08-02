using FluentValidation;
using Identity.Core.Features.Benefits.Commands.BenefitsDetail;

namespace Identity.Core.Features.User.Commands.BenefitsDetail;
public class BenefitsDetailValidator<T> : AbstractValidator<T> where T : BenefitsDetailCommand
{
    /// <summary>
    /// Initialize
    /// </summary>
    public BenefitsDetailValidator()
    {
        RuleFor(v => v.BenefitsTitle)
            .MaximumLength(200)
            .MinimumLength(1)
            .NotEmpty()
            .NotNull()
            .Equal(string.Empty)
            .When(x => !string.IsNullOrWhiteSpace(x.BenefitsTitle));

        RuleFor(v => v.BenefitsDescription)
            .MaximumLength(200)
            .MinimumLength(1)
            .NotEmpty()
            .NotNull()
            .Equal(string.Empty)
            .When(x => !string.IsNullOrWhiteSpace(x.BenefitsDescription));


        
    }
}
