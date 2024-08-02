using Identity.Core.Contracts.Common;
using System.Security.Claims;
using ThirdPartyUtilities.Models.JWT;

namespace Identity.API.common;

public class ApplicationClaims : IApplicationClaims
{
	public IHttpContextAccessor HttpContextAccessor { get; }
	public ApplicationClaims(IHttpContextAccessor httpContextAccessor)
	{
		HttpContextAccessor = httpContextAccessor;
	}
	public string GetUserId()
	{
		return Convert.ToString(HttpContextAccessor.HttpContext.User.FindFirstValue(ApiClaimTypes.UserId));
	}
}
