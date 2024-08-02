using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Commands.UserDocumentUpload;

public class UserDocumentUploadValidator<T> : AbstractValidator<T> where T : UserDocumentUploadCommand
{
    /// <summary>
    /// Initialize
    /// </summary>
    public UserDocumentUploadValidator()
    {
        //RuleFor(v => v.UserId)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.UserId)).WithMessage("E-10002");

        //RuleFor(v => v.DocumentTypeId)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.DocumentTypeId)).WithMessage("E-10029");

        //RuleFor(v => v.DocumentFileName)
        //   .NotNull().When(x => !string.IsNullOrWhiteSpace(x.DocumentFileName)).WithMessage("E-10030");
    }
}