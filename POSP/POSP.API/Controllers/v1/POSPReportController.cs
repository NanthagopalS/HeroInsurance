using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSP.Core.Features.Reports;
using POSP.Core.Helpers;
using System.Net;
using ThirdPartyUtilities.Helpers;

namespace POSP.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class POSPReportController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly ILogger<POSPReportController> _logger;
        /// <summary>
        /// controller for posp report
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="replogger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public POSPReportController(IMediator mediator, ILogger<POSPReportController> replogger)
        {
            _mediatr = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = replogger ?? throw new ArgumentNullException(nameof(replogger));
        }
        #region - NewAndOldPOSPReport -
        /// <summary>
        /// NewAndOldPOSPReport
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("NewAndOldPOSPReport")]
        [ProducesResponseType(typeof(NewAndOldPOSPReportVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<NewAndOldPOSPReportVm>> NewAndOldPOSPReport(NewAndOldPOSPReportQuery query, CancellationToken cancellationToken)
        {
            if (query.IsExportResponce == true)
            {
                query.CurrentPageSize = -1;
            }
            var result = await _mediatr.Send(query, cancellationToken);
            if (result is not null && result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError(result.Messages);
                return BadRequest(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - DownloadPOSPManagementDetailExcel -
        /// <summary>
        /// download new and old POSP records
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isCsv"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DownloadPOSPReport")]
        [ProducesResponseType(typeof(FileResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public FileResult DownloadPOSPReport(string fileName, bool isCsv = false)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Storage/" + fileName);
            var fileExtension = isCsv == true ? ".csv" : ".xlsx";
            var fileType = isCsv == true ? "text/csv" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            FileContentResult file = new FileContentResult(System.IO.File.ReadAllBytes(path + fileExtension), fileType)
            {
                FileDownloadName = fileName + DateTime.Now.ToFileTime()
            };
            return file;
        }
        #endregion
    }
}
