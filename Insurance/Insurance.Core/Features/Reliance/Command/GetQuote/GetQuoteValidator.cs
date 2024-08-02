using FluentValidation;


namespace Insurance.Core.Features.Reliance.Command.GetQuote;

public class GetQuoteValidator : AbstractValidator<GetRelianceQuery>
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
