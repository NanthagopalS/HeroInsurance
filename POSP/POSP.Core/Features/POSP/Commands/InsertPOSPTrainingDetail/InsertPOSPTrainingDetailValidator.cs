using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace POSP.Core.Features.POSP.Commands.InsertPOSPTrainingDetail;
public class InsertPOSPTrainingDetailValidator<T> : AbstractValidator<T> where T : InsertPOSPTrainingDetailCommand
{
    /// <summary>
    /// Initialize
    /// </summary>
    public InsertPOSPTrainingDetailValidator()
    {
        //RuleFor(v => v.UserId)
        //.MaximumLength(3)
        //.MinimumLength(1)
        //.NotEmpty()
        //.NotNull()
        //.Equal(string.Empty)
        //.When(x => !string.IsNullOrWhiteSpace(x.UserId)).WithMessage("E-100");
    }        
}
