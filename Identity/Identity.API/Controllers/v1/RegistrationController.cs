using Identity.API.Models;
using Identity.Core.Features.Authenticate.Commands.ResetPasswordAdmin;
using Identity.Core.Features.Registration.Commands.CreateAdmin;
using Identity.Core.Features.Registration.Commands.ResetUserAccountDetail;
using Identity.Core.Features.Registration.Commands.UpdateAdmin;
using Identity.Core.Features.User;
using Identity.Core.Features.User.Commands.UpdateUserPasswordFromUserLinkCommand;
using Identity.Core.Features.User.Commands.UserCreation;
using Identity.Core.Features.User.Queries.PanVerificationDetails;
using Identity.Core.Helpers;
using Identity.Domain.User;
using Identity.Domain.UserCreation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Helpers;

namespace Identity.API.Controllers.v1;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[ServiceFilter(typeof(ResponseCaptureFilter))]
public class RegistrationController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public RegistrationController(IMediator mediatr, IHttpContextAccessor httpContextAccessor)
    {
        _mediatr = mediatr;
        _httpContextAccessor = httpContextAccessor;
    }

    #region - Create User -
    /// <summary>
    /// Create a new User
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpPost]
    [Route("CreateUser")]
    [ProducesResponseType(typeof(UserCreateResponseModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<UserCreateResponseModel>> CreateUser(UserCreationCommand userCreationCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(userCreationCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("User already exist");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    #endregion

    #region - Login - 
    /// <summary>
    /// Login
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>  
    //[AllowAnonymous]
    [HttpPost]
    [Route("Login")]
    [ProducesResponseType(typeof(AdminUserResponseModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<AdminVm>> Login(CreateAdminCommand createAdminCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(createAdminCommand, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    #endregion

    #region - Change Password -
    /// <summary>
    /// Change Password
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>  
    [HttpPost]
    [Route("ChangePassword")]
    [ProducesResponseType(typeof(AdminUpdateUserResponseModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<AdminUpdateVM>> ChangePassword(UpdateAdminCommand updateAdminCommand, CancellationToken cancellationToken)
    {
        var validatePassword = CheckPasswordValidation(updateAdminCommand.OldPassWord, updateAdminCommand.NewPassWord, updateAdminCommand.ConfirmPassWord);
        if (validatePassword is not null)
        {
            return NotFound(validatePassword);
        }
        var result = await _mediatr.Send(updateAdminCommand, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Failed to change password");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    #endregion


    #region - Change Password With Email Link (Guid REQ) -
    /// <summary>
    /// Change Password With Email Link (Guid REQ)
    /// </summary>
    /// <param name="cancellationToken"></param>
    [HttpPost]
    [Route("UpdateUserPasswordFromUserLink")]
    [ProducesResponseType(typeof(UpdateUserPasswordFromUserLinkResponceModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<UpdateUserPasswordFromUserLinkVm>> UpdateUserPasswordFromUserLink(UpdateUserPasswordFromUserLinkCommand updateAdminCommand, CancellationToken cancellationToken)
    {
        // Call Password Validation Method
        var validatePassword = CheckPasswordValidation(null, updateAdminCommand.NewPassWord, updateAdminCommand.ConfirmPassWord);
        if (validatePassword != null)
        {
            return NotFound(validatePassword);
        }
        var result = await _mediatr.Send(updateAdminCommand, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Failed to change password");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    #endregion

    #region - Reset Account Details -
    /// <summary>
    /// Reset Account Details
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>  
    [HttpPost]
    [Route("ResetUserAccountDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> ResetUserAccountDetail(string MobileNo, CancellationToken cancellationToken)
    {
        var req = new ResetUserAccountDetailCommand
        {
            MobileNo = MobileNo,
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("User reset account process failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    #endregion

    #region - Reset Password -
    /// <summary>
    /// Reset Password
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>  
    [HttpPost]
    [Route("ResetPassword")]
    [ProducesResponseType(typeof(ResetPasswordResponseModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ResetPasswordVM>> ResetPassword(ResetPasswordCommand resetPasswordCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(resetPasswordCommand, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    #endregion

    #region - Check Password Validation Method - 

    [HttpGet]
    [Route("CheckPasswordValidation")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ResultModel CheckPasswordValidation(string OldPassWord, string NewPassWord, string ConfirmPassWord)
    {
        if (!string.IsNullOrWhiteSpace(OldPassWord) && OldPassWord == NewPassWord)
        {
            var problemDetails = new ResultModel()
            {
                Data = null,
                Message = "Old password and new password should not same",
                StatusCode = ((int)HttpStatusCode.NotFound).ToString()
            };
            return problemDetails;
        }
        if (ConfirmPassWord != NewPassWord)
        {
            var problemDetails = new ResultModel()
            {
                Data = null,
                Message = "Confirm password should same as new password",
                StatusCode = ((int)HttpStatusCode.NotFound).ToString()
            };
            return problemDetails;
        }
        return null;
    }
    #endregion

    #region - Logout -
    /// <summary>
    /// Logout
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost]
    [Route("Logout")]
    [ProducesResponseType(typeof(LogoutResponseModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<LogoutVM>> Logout(string userId, CancellationToken cancellationToken)
    {
        var logoutCommand = new LogoutCommand
        {
            UserId = userId
        };
        var result = await _mediatr.Send(logoutCommand, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Failed to logout");
            return NotFound(problemDetails);
        }
        if (!result.Result.LogoutStatus)
        {
            var problemDetails = new ResultModel()
            {
                Data = null,
                Message = "Failed to logout",
                StatusCode = ((int)HttpStatusCode.NotFound).ToString()
            };
            return NotFound(problemDetails);
        }
        // Remove Auth token code
        HttpContext.Request.Headers.Remove("authorization");
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    #endregion

    #region - Reset Password Verification -

    [HttpGet]
    [Route("ResetPasswordVerification")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ResetPasswordVerificationVM>> ResetPasswordVerification(string userId, string guId, CancellationToken cancellationToken)
    {
        var req = new ResetPasswordVerificationQuery
        {
            UserId = userId,
            GuId = guId
        };
        var result = await _mediatr.Send(req, cancellationToken);

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion


    [HttpPost("ResetPasswordAdmin")]
    [ProducesResponseType(typeof(ResetPasswordAdminVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ResetPasswordAdminVm>> ResetPasswordAdmin(ResetPasswordAdminQuery resetPassword, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(resetPassword, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateValidationError(new List<string> { "Failed to Reset Password" });
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

}
