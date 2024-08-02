using Insurance.Core.Features.ManualPolicy.Command;
using Insurance.Core.Features.ManualPolicy.Query;
using Insurance.Core.Features.ManualPolicy.Query.GetManualPolicyNature;
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

	public class PolicyController : Controller
    {

        private readonly IMediator _mediatr;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="mediatr"></param>
        public PolicyController(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        #region -Upload Manula Policy -
        /// <summary>
        /// Upload Manula Policy
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadManualPolicy")]
        [ProducesResponseType(typeof(ManualPolicyUploadCommandVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ManualPolicyUploadCommandVm>> UploadPolicy(IFormFile files, CancellationToken cancellationToken)
        {
            ManualPolicyUploadCommand cmd = new ManualPolicyUploadCommand();
            cmd.files = files;
            var result = await _mediatr.Send(cmd, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError(result.Messages);
                return BadRequest(errorDetails);
            }
            EmailPolicyValidationCommand EmailCommand = new EmailPolicyValidationCommand();
            _ = await _mediatr.Send(EmailCommand, cancellationToken);

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get Manual Policy List -
        /// <summary>
        /// Get Dashboard Lead Details
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetManualPolicyList")]
        [ProducesResponseType(typeof(GetManualPolicyListVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetManualPolicyListVM>>> GetManualPolicyList(GetManualPolicyListQuery required, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(required, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError(result.Messages);
                return BadRequest(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetManualPolicyNature -
        /// <summary>
        /// Get Dashboard Lead Details
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetManualPolicyNature")]
        [ProducesResponseType(typeof(GetManualPolicyNatureVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetManualPolicyNatureVm>>> GetManualPolicyNature(CancellationToken cancellationToken)
        {
            var req = new GetManualPolicyNatureQuery { };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError(result.Messages);
                return BadRequest(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion
    }

}
