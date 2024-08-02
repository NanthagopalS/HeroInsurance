using FluentValidation;


namespace Insurance.Core.Features.GoDigit.Queries.GetQuote;

public class GetQuoteValidator : AbstractValidator<GetGoDigitQuery>
{
    public GetQuoteValidator()
    {
        RuleFor(x => x.VariantId)
        .NotEmpty()
        .NotNull()
        .WithMessage("VariantId is not valid");

        RuleFor(x => x.VehicleTypeId)
        .NotEmpty()
        .NotNull()
        .WithMessage("VehicleTypeId is not valid");

    }
}
