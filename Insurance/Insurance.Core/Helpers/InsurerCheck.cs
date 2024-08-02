using Insurance.Domain.InsuranceMaster;
using Microsoft.Extensions.Options;
using Insurance.Core.Contracts.Common;

namespace Insurance.Core.Helpers;

public class InsurerCheck : IInsurerCheck
{
    private readonly PolicyTypeConfig _policyTypeConfig;

    public InsurerCheck(IOptions<PolicyTypeConfig> policyTypeConfig)
    {
        _policyTypeConfig = policyTypeConfig.Value;
    }
    
    public bool CheckPreviousCurrentInsurer(bool isBrandNew, bool isPreviousPolicy, string previousPolicyTypeId, string insurerId, string odInsurerId, string tpInurerId)
    {
        if (previousPolicyTypeId.Equals(_policyTypeConfig.SATP))
        {
            return !isBrandNew && isPreviousPolicy && insurerId.Equals(tpInurerId);
        }
        else if (previousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive) || previousPolicyTypeId.Equals(_policyTypeConfig.SAOD))
        {
            return !isBrandNew && isPreviousPolicy && insurerId.Equals(odInsurerId);
        }
        return false;
    }
}
