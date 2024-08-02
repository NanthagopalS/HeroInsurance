using FluentValidation;

namespace Identity.Core.Features.User.Commands.UserBankDetail;
public class UserBankDetailValidator<T> : AbstractValidator<T> where T : UserBankDetailCommand
{
    /// <summary>
    /// Initialize
    /// </summary>
    public UserBankDetailValidator()
    {
        //RuleFor(v => v.UserId)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.UserId)).WithMessage("E-10002");

        //RuleFor(v => v.BankId)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.BankId)).WithMessage("E-10021");

        //RuleFor(v => v.IFSC)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.IFSC)).WithMessage("E-10022");

        //RuleFor(v => v.AccountHolderName)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.AccountHolderName)).WithMessage("E-10023");

        //RuleFor(v => v.AccountNumber)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.AccountNumber)).WithMessage("E-10024");
    }
}
