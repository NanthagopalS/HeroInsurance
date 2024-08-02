using Identity.Core.Features.Authenticate;
using Identity.Core.Features.Authenticate.Commands.ResetPasswordAdmin;
using Identity.Core.Features.Registration.Commands.AuthenticateAdmin;
using Identity.Core.Features.Registration.Commands.SendOTP;
using Identity.Core.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;

namespace Identity.API.Controllers.v1;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[ServiceFilter(typeof(ResponseCaptureFilter))]
public class AuthenticateController : ControllerBase
{
    private readonly IMediator _mediatr;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public AuthenticateController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    /// <summary>
    /// GetPanVerificationDetails
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpPost("user")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<AuthenticateVm>> AuthenticateUser(AuthenticateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(command, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateValidationError(result.Messages);
            return BadRequest(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// GetPanVerificationDetails
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpPost("SendOTP")]
    [ProducesResponseType(typeof(SendOTPVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<SendOTPVm>> SendOTP(SendOTPCommand sendOTPCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(sendOTPCommand, cancellationToken);
        if (result.Result.UserId == null)
        {
            var problemDetails = Result.CreateValidationError(new List<string> { "User does not exist" });
            return BadRequest(problemDetails);
        }

        if (result.Failed)
        {
            var problemDetails = Result.CreateValidationError(new List<string> { "Failed to send OTP" });
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    [HttpPost("AuthenticateAdmin")]
    [ProducesResponseType(typeof(AuthenticateVM), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> AuthenticateAdmin(AuthenticateAdminCommand authenticateCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(authenticateCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateValidationError(result.Messages);
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
}
