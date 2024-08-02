using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.InsertUserRoleMappingDetaill
{
    public class InsertUserRoleMappingDetaillValidator<T> : AbstractValidator<T> where T : InsertUserRoleMappingDetaillCommand
    {
        public InsertUserRoleMappingDetaillValidator()
        {
           
        }
    }
}
