using FluentValidation;

namespace Identity.Core.Features.User.Commands.UserCreation;
public class UserCreationValidator : AbstractValidator<UserCreationCommand>
{
    /// <summary>
    /// Initialize
    /// </summary>
    public UserCreationValidator()
    {

       // RuleFor(v => v.UserName)
       //.NotNull().When(x => !string.IsNullOrWhiteSpace(x.UserName)).WithMessage("E-10007");
       // //.When(x => !string.IsNullOrWhiteSpace(x.UserName)).WithMessage("User Name is not valid");
       // //.When(x => !string.IsNullOrWhiteSpace(x.UserName)).WithMessage("E-100");

       // RuleFor(v => v.EmailId)
       //     .NotNull()
       //     .When(x => !string.IsNullOrWhiteSpace(x.EmailId)).WithMessage("E-10001");  
        
        
       // RuleFor(v => v.MobileNo)
       //     .NotNull()
       //     .When(x => !string.IsNullOrWhiteSpace(x.MobileNo)).WithMessage("E-10004");

    }
}
