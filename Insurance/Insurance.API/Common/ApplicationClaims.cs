using Insurance.Core.Contracts.Common;
using System.Security.Claims;
using ThirdPartyUtilities.Models.JWT;

namespace Insurance.API.Common;

public class ApplicationClaims : IApplicationClaims
{
    public IHttpContextAccessor HttpContextAccessor { get; }
    public ApplicationClaims(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }
    public string GetPOSPId()
    {
        return Convert.ToString(HttpContextAccessor.HttpContext.User.FindFirstValue(ApiClaimTypes.POSPId));
    }
    public string GetUserId()
    {
        return Convert.ToString(HttpContextAccessor.HttpContext.User.FindFirstValue(ApiClaimTypes.UserId));
    }
    public string GetUserName()
    {
        return Convert.ToString(HttpContextAccessor.HttpContext.User.Identity.Name);
    }
    public string GetDOB()
    {
        return Convert.ToString(HttpContextAccessor.HttpContext.User.FindFirstValue(ApiClaimTypes.DOB));
    }
    public string EmailId()
    {
        return Convert.ToString(HttpContextAccessor.HttpContext.User.FindFirstValue(ApiClaimTypes.EmailId));
    }
    public string GetMobileNo()
    {
        return Convert.ToString(HttpContextAccessor.HttpContext.User.FindFirstValue(ApiClaimTypes.MobileNo));
    }
    public string GetAadhaarNumber()
    {
        return Convert.ToString(HttpContextAccessor.HttpContext.User.FindFirstValue(ApiClaimTypes.AadhaarNumber));
    }
    public string GetPAN()
    {
        return Convert.ToString(HttpContextAccessor.HttpContext.User.FindFirstValue(ApiClaimTypes.PAN));
    }
    public string GetLocation()
    {
        return Convert.ToString(HttpContextAccessor.HttpContext.User.FindFirstValue(ApiClaimTypes.CityName));
    }
    public string GetRole()
    {
        return Convert.ToString(HttpContextAccessor.HttpContext.User.FindFirstValue(ApiClaimTypes.RoleName));
    }
}
