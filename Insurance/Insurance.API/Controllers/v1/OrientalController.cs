using AutoMapper;
using Insurance.API.Models;
using Insurance.Core.Features.Oriental.Queries.GetQuote;
using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails;
using Insurance.Core.Helpers;
using Insurance.Domain.GoDigit;
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
public class OrientalController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public OrientalController(IMediator mediatr, IMapper mapper)
    {
        _mediatr = mediatr;
        _mapper = mapper;
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
    public async Task<ActionResult<QuoteResponseModel>> GetQuote(GetOrientalQuoteQuery request, CancellationToken cancellationToken)
    {
        var policyDateQuery = _mapper.Map<GetPolicyDatesQuery>(request);
        var policyDateResponse = await _mediatr.Send(policyDateQuery, cancellationToken);

        if (policyDateResponse.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Policy date is not correct");
            return NotFound(problemDetails);
        }
        request.PolicyDates = policyDateResponse.Result;

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
