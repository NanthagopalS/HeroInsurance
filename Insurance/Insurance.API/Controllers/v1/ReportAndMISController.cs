using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Org.BouncyCastle.Ocsp;
using System.Net;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;
using ThirdPartyUtilities.Models.SMS;
using Insurance.Core.Features.AllReportAndMIS.Query.GetAllReportAndMIS;
using Insurance.Core.Helpers;

namespace Insurance.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]

	public class ReportAndMISController : Controller
    {

        private readonly IMediator _mediatr;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        public ReportAndMISController(IMediator mediatr, IConfiguration configuration, ISmsService smsServ, IEmailService emailServ)
        {
            _mediatr = mediatr;
            _config = configuration;
            _emailService = emailServ;
            _smsService = smsServ;
        }

        #region - Get Report And MIS Details -
        /// <summary>
        /// Get Report And MIS Details
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AllReportAndMIS")]
        [ProducesResponseType(typeof(AllReportAndMISVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<AllReportAndMISVm>> AllReportAndMIS(AllReportAndMISQuery req, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion
    }

}
