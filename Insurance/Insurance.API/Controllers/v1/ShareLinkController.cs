using AutoMapper;
using Insurance.API.Models;
using Insurance.Core.Features.ShareLink.Command.AuthenticateUser;
using Insurance.Core.Features.ShareLink.Command.SendNotification;
using Insurance.Core.Features.ShareLink.Command.SendOTP;
using Insurance.Core.Features.ShareLink.Command.VerifyOTP;
using Insurance.Core.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Helpers;

namespace Insurance.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class ShareLinkController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize and set the dependencies
        /// </summary>
        /// <param name="mediator"></param>
        public ShareLinkController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Send OTP
        /// </summary>
        /// <param name="sendOTPCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("SendOTP")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> SendOTP(SendOTPCommand sendOTPCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(sendOTPCommand, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateValidationError(new List<string> { "Failed to send OTP" });
                return BadRequest(problemDetails);
            }
            var res = Result.CreateSuccess(result.Messages, (int)HttpStatusCode.OK);
            return Ok(res);
        }

        /// <summary>
        /// Verify OTP
        /// </summary>
        /// <param name="verifyOTPCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("VerifyOTP")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<string>> VerifyOTP(VerifyOTPCommand verifyOTPCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(verifyOTPCommand, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateValidationError(new List<string> { "Invalid OTP" });
                return BadRequest(problemDetails);
            }
            var res = Result.CreateSuccess(result.Messages, (int)HttpStatusCode.OK);
            return Ok(res);
        }

        /// <summary>
        /// Send Notification
        /// </summary>
        /// <param name="sendNotificationCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("SendNotification")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<string>> SendNotification(SendNotificationCommand sendNotificationCommand, CancellationToken cancellationToken)
        {
            var request = _mapper.Map<SendNotificationCommand>(sendNotificationCommand);
            var response = await _mediator.Send(request, cancellationToken);
            if (response.Failed)
            {
                var problemDetails = Result.CreateValidationError(new List<string> { "Failed to share plan" });
                return BadRequest(problemDetails);
            }
            var godigitResult = Result.CreateSuccess(response.Messages, (int)HttpStatusCode.OK);
            return Ok(godigitResult);
        }

        /// <summary>
        /// Aithenticate Link
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("AuthenticateUser")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<AuthenticateUserVm>> AuthenticateUser(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            if (response.Failed)
            {
                var problemDetails = Result.CreateValidationError(new List<string> { "Failed to authenticate" });
                return BadRequest(problemDetails);
            }
            var result = Result.CreateSuccess(response.Result, (int)HttpStatusCode.OK);
            return Ok(result);
        }

    }
}
