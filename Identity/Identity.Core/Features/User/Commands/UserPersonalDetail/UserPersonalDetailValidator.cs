using FluentValidation;

namespace Identity.Core.Features.User.Commands.UserPersonalDetail;
public class UserPersonalDetailValidator : AbstractValidator<UserPersonalDetailCommand>
{
    /// <summary>
    /// Initialize
    /// </summary>
    public UserPersonalDetailValidator()
    {
        //RuleFor(v => v.UserId)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.UserId)).WithMessage("E-10002");

        //RuleFor(v => v.Gender)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.Gender)).WithMessage("E-10009");

        //RuleFor(v => v.AlternateContactNo)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.AlternateContactNo)).WithMessage("E-10010");

        //RuleFor(v => v.AadhaarNumber)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.AadhaarNumber)).WithMessage("E-10011");

        //RuleFor(v => v.NameDifferentOnDocument)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.NameDifferentOnDocument)).WithMessage("E-10012");

        ////RuleFor(v => v.Pincode)
        ////   .NotNull().When(x => !int.IsNullOrWhiteSpace(x.Pincode)).WithMessage("E-10013");

        //RuleFor(v => v.CityId)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.CityId)).WithMessage("E-10014");

        //RuleFor(v => v.StateId)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.StateId)).WithMessage("E-10015");

        //RuleFor(v => v.EducationQualificationTypeId)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.EducationQualificationTypeId)).WithMessage("E-10016");

        //RuleFor(v => v.InsuranceSellingExperienceRangeId)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.InsuranceSellingExperienceRangeId)).WithMessage("E-10017");

        //RuleFor(v => v.InsuranceProductsofInterestTypeId)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.InsuranceProductsofInterestTypeId)).WithMessage("E-10018");

    }
}
