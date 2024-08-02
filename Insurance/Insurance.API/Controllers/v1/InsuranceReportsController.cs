using Insurance.Core.Features.AllReportAndMIS.Query.BusinessSummery;
using Insurance.Core.Features.AllReportAndMIS.RequestandResponseH;
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
	public class InsuranceReportsController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly ILogger<InsuranceReportsController> _logger;
        /// <summary>
        /// Initialize and set the dependencies
        /// </summary>
        /// <param name="mediatr"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsuranceReportsController(IMediator mediatr, ILogger<InsuranceReportsController> logger)
        {
            _mediatr = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #region - BusinessSummeryReport -
        /// <summary>
        /// Get Business summery report
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="businessSummeryQuery"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("BusinessSummeryReport")]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BusinessSummeryVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<BusinessSummeryVm>>> BusinessSummeryReport(BusunessSummeryQuery businessSummeryQuery, CancellationToken cancellationToken)
        {
            if (businessSummeryQuery.IsExportExcel)
            {
                businessSummeryQuery.CurrentPageSize = -1;
            }
            var result = await _mediatr.Send(businessSummeryQuery, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateValidationError(result.Messages);
                return BadRequest(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - DownloadBusinessSummeryReport -
        /// <summary>
        /// download new and old POSP records
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isCsv"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DownloadBusinessSummeryReport")]
        [ProducesResponseType(typeof(FileResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public FileResult DownloadBusinessSummeryReport(string fileName, bool isCsv = false)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Storage/" + fileName);
            var fileExtension = isCsv == true ? ".csv" : ".xlsx";
            var fileType = isCsv == true ? "text/csv" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            FileContentResult file = new(System.IO.File.ReadAllBytes(path + fileExtension), fileType)
            {
                FileDownloadName = fileName
            };
            return file;
        }
        #endregion


        #region - RequestandResponseReport -
        /// <summary>
        /// Get Business summery report
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="requestandresponse"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RequestandResponseReport")]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(RequestandResponseVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<RequestandResponseVM>>> RequestandResponseReport(RequestandResponseQuery requestandResponseQuery, CancellationToken cancellationToken)
        {
            if (requestandResponseQuery.IsExportExcel)
            {
                requestandResponseQuery.CurrentPageSize = -1;
            }
            var result = await _mediatr.Send(requestandResponseQuery, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateValidationError(result.Messages);
                return BadRequest(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion


    }
}
