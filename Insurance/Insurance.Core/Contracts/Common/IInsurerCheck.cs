using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Contracts.Common;

public interface IInsurerCheck
{
    bool CheckPreviousCurrentInsurer(bool isBrandNew,
                                bool isPreviousPolicy,
                                string previousPolicyTypeId,
                                string insurerId,
                                string odInsurerId,
                                string tpInurerId);
}

