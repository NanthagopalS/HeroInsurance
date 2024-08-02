using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSP.Core.Features.Command.ImportPOSP;
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
	public class POSPMigrationController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly ILogger<POSPMigrationController> _logger;

        public POSPMigrationController(IMediator mediatr, ILogger<POSPMigrationController> logger)
        {
            _mediatr = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("POSPMigrationDump")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> POSPMigrationDump(IFormFile files, CancellationToken cancellationToken)
        {
            POSPMigrationDumpCommand cmd = new POSPMigrationDumpCommand();
            cmd.files = files;
            var result = await _mediatr.Send(cmd, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError(result.Messages);
                return BadRequest(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
    }
}
