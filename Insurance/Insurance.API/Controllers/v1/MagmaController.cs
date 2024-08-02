using Insurance.API.Models;
using Insurance.Core.Features.Magma.Queries.GetQuote;
using Insurance.Core.Features.InsuranceMaster;
using Insurance.Domain.GoDigit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Helpers;
using Insurance.Core.Helpers;

namespace Insurance.API.Controllers.v1;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[ApiController]
[ServiceFilter(typeof(ResponseCaptureFilter))]
public class MagmaController : ControllerBase
{
    private readonly IMediator _mediatr;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public MagmaController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    /// <summary>
    /// GetQuote
    /// </summary>
    ///// <param name="insuranceType">InsuranceType: MOTOR/HEALTH/TERM</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpPost("GetQuote")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<QuoteResponseModel>> GetQuote(GetMagmaQuoteQuery request, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(request, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
}
