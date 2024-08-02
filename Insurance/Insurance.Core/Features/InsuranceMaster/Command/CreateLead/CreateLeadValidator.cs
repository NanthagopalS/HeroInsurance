using FluentValidation;
using Insurance.Domain.InsuranceMaster;

namespace Insurance.Core.Features.InsuranceMaster.Command.LeadDetails;

public class CreateLeadValidator : AbstractValidator<CreateLeadCommand>
{
    /// <summary>
    /// Initialize
    /// </summary>
    public CreateLeadValidator()
    {
        //RuleFor(v => v.VehicleTypeId)
        //    .NotEmpty()
        //    .NotNull()
        //    .When(x => !string.IsNullOrWhiteSpace(x.VehicleTypeId)).WithMessage("VehicleTypeId is not valid");

        //RuleFor(v => v.VehicleNumber)
        //    .Length(10)
        //    .NotEmpty()
        //    .NotNull()
        //    .When(x => !string.IsNullOrWhiteSpace(x.VehicleNumber)).WithMessage("VehicleNumber is not valid");

        //RuleFor(v => v.VariantId)
        //    .NotEmpty()
        //    .NotNull()
        //    .When(x => !string.IsNullOrWhiteSpace(x.VariantId)).WithMessage("VariantId is not valid");

        //RuleFor(v => v.YearId)
        //    .NotEmpty()
        //    .NotNull()
        //    .When(x => !string.IsNullOrWhiteSpace(x.YearId)).WithMessage("YearId is not valid");

        //RuleFor(v => v.LeadName)
        //    .MaximumLength(40)
        //    .MinimumLength(3)
        //    .NotEmpty()
        //    .NotNull()
        //    .When(x => !string.IsNullOrWhiteSpace(x.LeadName)).WithMessage("Lead Name is not valid");

        //RuleFor(v => v.PhoneNumber)
        //    .Length(10)
        //    .NotEmpty()
        //    .NotNull()
        //    .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber)).WithMessage("Phone Number is not valid");

        //RuleFor(v => v.Email)
        //    .MaximumLength(100)
        //    .EmailAddress()
        //    .NotEmpty()
        //    .NotNull()
        //    .When(x => !string.IsNullOrWhiteSpace(x.Email)).WithMessage("Email Id is not valid");
    }
}
