using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.ResetUserDetail;
using Admin.Core.Features.User.Queries.GetAllPOSPBackOficePendingReport;
using Admin.Core.Features.User.Queries.GetAllPOSPCountDetail;
using Admin.Core.Features.User.Queries.GetPOSPOnboardingDetail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;

namespace Admin.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class AdminController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly IMongoDBService _mongodbService;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="mediatr"></param>
        /// <param name="mongodbService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AdminController(IMediator mediatr, IMongoDBService mongodbService)
        {
            _mediatr = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
            _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        }




        #region - Get ALL POSP Count  -
        /// <summary>
        /// All posp stage count
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllPOSPCountDetail")]
        [ProducesResponseType(typeof(GetPOSPOnboardingDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetAllPOSPCountDetailVM>> GetAllPOSPCountDetail(CancellationToken cancellationToken)
        {
            var req = new GetAllPOSPCountDetailQuery();
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError(result.Messages);
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion


        #region - Get ALL Back Ofice Pending Report  -
        /// <summary>
        /// All posp stage count
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllPOSPBackOficePendingReport")]
        [ProducesResponseType(typeof(GetPOSPOnboardingDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetAllPOSPBackOficePendingReportVM>> GetAllPOSPBackOficePendingReport(CancellationToken cancellationToken)
        {
            var req = new GetAllPOSPBackOficePendingReportQuery();
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError(result.Messages);
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion



        #region - Reset UserId -
        /// <summary>
        /// ResetUserIdDetail
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ResetUserIdDetail")]
        [ProducesResponseType(typeof(ResetUserAccountDetailVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ResetUserAccountDetailVM>> ResetUserIdDetail(ResetUserAccountDetailQuery req, CancellationToken cancellationToken)
        {
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


    }
}

