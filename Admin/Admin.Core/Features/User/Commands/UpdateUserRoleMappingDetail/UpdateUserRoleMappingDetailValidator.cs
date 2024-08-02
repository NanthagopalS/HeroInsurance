using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.UpdateUserRoleMappingDetail
{
    public class UpdateUserRoleMappingDetailValidator<T> : AbstractValidator<T> where T : UpdateUserRoleMappingDetailCommand
    {
        public UpdateUserRoleMappingDetailValidator()
        {

        }
    
    }
}
