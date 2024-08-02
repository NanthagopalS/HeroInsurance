using FluentValidation;

namespace Identity.Core.Features.User.Commands.UserInquiryDetail;
public class UserInquiryDetailValidator: AbstractValidator<UserInquiryDetailCommand> 
{
    /// <summary>
    /// Initialize
    /// </summary>
    public UserInquiryDetailValidator()
    {
        //RuleFor(v => v.UserName)
        //   .NotEmpty().WithMessage("E-10007");

        //RuleFor(v => v.PhoneNumber)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber)).WithMessage("E-10004");
        
    }
}
