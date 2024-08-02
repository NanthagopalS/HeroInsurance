using AutoMapper;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.GoDigit.Queries.GetPolicyStatus;
using Insurance.Core.Features.GoDigit.Queries.GetQuote;
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
public class GoDigitController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public GoDigitController(IMediator mediatr, IMapper mapper)
    {
        _mediatr = mediatr;
        _mapper = mapper;
    }

    /// <summary>
    /// GetInsuraneType
    /// </summary>
    /// <param name="insuranceType">InsuranceType: MOTOR/HEALTH/TERM</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpPost("GetQuote")]
    [ProducesResponseType(typeof(IEnumerable<QuoteResponseModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuoteResponseModel>>> GetQuote(GetGoDigitQuery getGoDigitQuery, CancellationToken cancellationToken)
    {
        var policyDateQuery = _mapper.Map<GetPolicyDatesQuery>(getGoDigitQuery);
        var policyDateResponse = await _mediatr.Send(policyDateQuery, cancellationToken);

        if (policyDateResponse.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Policy date is not correct");
            return NotFound(problemDetails);
        }


        getGoDigitQuery.PolicyDates = policyDateResponse.Result;
        var result = await _mediatr.Send(getGoDigitQuery, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Success URL
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("PGSuccess/{applicationid}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDetailsVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ActionResult<PaymentDetailsVm> PGSuccess([FromRoute] string applicationId, [FromQuery] string LeadId, CancellationToken cancellationToken)
    {
        GetPaymentCKYCQuery getPaymentCKYCQuery = new GetPaymentCKYCQuery
        {
            ApplicationId = applicationId,
            LeadId = LeadId,
        };
        var result = _mediatr.Send(getPaymentCKYCQuery, cancellationToken);

        if (result != null)
        {
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);

        }
        var problemDetails = Result.CreateNotFoundError("Data not found");
        return NotFound(problemDetails);
    }

    /// <summary>
    /// Cancel URL
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("PGCancel/{applicationid}")]
    public ActionResult<string> PGCancel([FromRoute] string applicationId, [FromQuery] string transactionNumber, CancellationToken cancellationToken)
    {
        var res = Result.CreateSuccess("Failure", (int)HttpStatusCode.OK);
        return Ok("CancelURL");
    }

    [HttpGet("GetPolicyStatus/{quoteTransactionId}/{policyNumber}")]
    [ProducesResponseType(typeof(QuoteResponseModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<QuoteResponseModel>> GetPolicyStatus([FromRoute] string quoteTransactionId, [FromRoute] string policyNumber, [FromQuery] string LeadId, CancellationToken cancellationToken)
    {
        GetPolicyStatusQuery getPolicyStatusQuery = new GetPolicyStatusQuery()
        {
            PolicyNumber = policyNumber,
            QuoteTransactionId = quoteTransactionId,
            LeadId = LeadId
        };

        var response = await _mediatr.Send(getPolicyStatusQuery, cancellationToken);
        if(response.Failed)
        {
            var problemDetails = Result.CreateValidationError(response.Messages);
            return BadRequest(problemDetails);
        }
        
        var res = Result.CreateSuccess(response.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
}
