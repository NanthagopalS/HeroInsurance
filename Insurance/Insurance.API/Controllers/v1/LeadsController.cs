using AutoMapper;
using Insurance.Core.Features.Leads;
using Insurance.Core.Features.Leads.GetPaymentStatus;
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
public class LeadsController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public LeadsController(IMediator mediatr, IMapper mapper)
    {
        _mediatr = mediatr;
        _mapper = mapper;
    }

    #region - Get Dashboard Lead Details -
    /// <summary>
    /// Get Dashboard Lead Details
    /// </summary>       
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("GetDashboardLeadDetails")]
    [ProducesResponseType(typeof(GetLeadManagementDetailVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetLeadManagementDetailVm>>> GetDashboardLeadDetails(GetLeadManagementDetailQuery req, CancellationToken cancellationToken)
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


    #region - Get Dashboard Lead Details -
    /// <summary>
    /// Get Dashboard Lead Details
    /// </summary>       
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetPaymentStatusList")]
    [ProducesResponseType(typeof(GetPaymentStatusListVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(GetPaymentStatusListVm), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPaymentStatusListVm>>> GetPaymentStatusList(CancellationToken cancellationToken)
    {
        GetPaymentStatusListQuery req = new GetPaymentStatusListQuery();
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
