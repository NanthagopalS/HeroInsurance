﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.Registration.Commands.UpdateAdmin
{
    internal class AdminUpdateCreationValidator<T> : AbstractValidator<T> where T : UpdateAdminCommand
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public AdminUpdateCreationValidator()
        {

            //RuleFor(v => v.EmailId)
            //    .MaximumLength(100)
            //    .EmailAddress()
            //    .NotEmpty()
            //    .NotNull()
            //    .When(x => !string.IsNullOrWhiteSpace(x.EmailId)).WithMessage("Email Id is not valid");

            //RuleFor(v => v.PassWord)
            //   .MaximumLength(20)
            //   .MinimumLength(6)
            //   .NotEmpty()
            //   .NotNull()
            //   .Equal(string.Empty)
            //   .When(x => !string.IsNullOrWhiteSpace(x.PassWord)).WithMessage("PassWord is not valid");


        }
    }

}
