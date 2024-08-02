using FluentValidation;

namespace Identity.Core.Features.User.Commands.UserAddressDetail;
public class UserAddressDetail<T> : AbstractValidator<T> where T : UserAddressDetailCommand
{
    /// <summary>
    /// Initialize
    /// </summary>
    public UserAddressDetail()
    {
        //RuleFor(v => v.UserId)
        //    .MaximumLength(100)
        //    .MinimumLength(1)
        //    .NotEmpty()
        //    .NotNull()
        //    .Equal(string.Empty)
        //    .When(x => !string.IsNullOrWhiteSpace(x.UserId)).WithMessage("User Id is not valid");

        //RuleFor(v => v.AddressLine1)
        //    .MaximumLength(200)
        //    .MinimumLength(1)
        //    .NotEmpty()
        //    .NotNull()
        //    .Equal(string.Empty)
        //    .When(x => !string.IsNullOrWhiteSpace(x.AddressLine1));

        //RuleFor(v => v.AddressLine2)
        //    .MaximumLength(200)
        //    .MinimumLength(1)
        //    .NotEmpty()
        //    .NotNull()
        //    .Equal(string.Empty)
        //    .When(x => !string.IsNullOrWhiteSpace(x.AddressLine2));

        //RuleFor(v => v.CityId)
        //    .MaximumLength(100)
        //    .MinimumLength(1)
        //    .NotEmpty()
        //    .NotNull()
        //    .Equal(string.Empty)
        //    .When(x => !string.IsNullOrWhiteSpace(x.CityId)).WithMessage("CityId Id is not valid");

        //RuleFor(v => v.StateId)
        //    .MaximumLength(100)
        //    .MinimumLength(1)
        //    .NotEmpty()
        //    .NotNull()
        //    .Equal(string.Empty)
        //    .When(x => !string.IsNullOrWhiteSpace(x.StateId)).WithMessage("StateId Id is not valid");
    }
}
