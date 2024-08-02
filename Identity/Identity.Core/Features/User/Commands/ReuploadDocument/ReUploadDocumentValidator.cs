using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Commands.ReuploadDocument;

public class ReUploadDocumentValidator<T> : AbstractValidator<T> where T : ReUploadDocumentCommand
{
    /// <summary>
    /// Initialize
    /// </summary>
    public ReUploadDocumentValidator()
    {
        //RuleFor(v => v.UserId)
        //    .MaximumLength(100)
        //    .MinimumLength(1)
        //    .NotEmpty()
        //    .NotNull()
        //    .Equal(string.Empty)
        //    .When(x => !string.IsNullOrWhiteSpace(x.UserId)).WithMessage("User Id is not valid");
    }
}