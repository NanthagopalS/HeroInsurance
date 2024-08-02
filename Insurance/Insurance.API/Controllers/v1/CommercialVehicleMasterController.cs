using Insurance.API.Models;
using Insurance.Core.Features.CommercialMaster.Query.GetCommercialMaster;
using Insurance.Core.Features.CommercialMaster.Query.GetCommercialVehicleOtherDetailsAskOptions;
using Insurance.Core.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Helpers;

namespace Insurance.API.Controllers.v1;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[ApiController]
[ServiceFilter(typeof(ResponseCaptureFilter))]
public class CommercialVehicleMasterController : Controller
{
    private readonly IMediator _mediatr;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public CommercialVehicleMasterController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    /// <summary>
    /// Get GetMakeModelFuel
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetCommercialCategorySubCategory")]
    [ProducesResponseType(typeof(IEnumerable<CommercialVehicleCategoryVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<CommercialVehicleCategoryVm>>> GetCommercialCategorySubCategory(CancellationToken cancellationToken)
    {
        var req = new GetCommercialCategoryQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Commercial Make Model not found");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// GetCommercialVehicleOtherDetailsAskOptions
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("GetCommercialVehicleOtherDetailsAskOptions")]
    [ProducesResponseType(typeof(IEnumerable<GetCommercialVehicleOtherDetailsAskOptionsVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetCommercialVehicleOtherDetailsAskOptionsVm>>> GetCommercialVehicleOtherDetailsAskOptions(GetCommercialVehicleOtherDetailsAskOptionsQuery req, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

}
