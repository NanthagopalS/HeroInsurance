using AutoMapper;
using Insurance.API.Models;
using Insurance.Core.Features.Bajaj.Command.CheckBreakinPinStatus;
using Insurance.Core.Features.Bajaj.Queries.GetPayment;
using Insurance.Core.Features.Bajaj.Queries.GetQuote;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
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
public class BajajController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public BajajController(IMediator mediatr, IMapper mapper)
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
    public async Task<ActionResult<QuoteResponseModel>> GetQuote(GetBajajQuoteQuery request, CancellationToken cancellationToken)
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

    [HttpPost("PGSuccess/{applicationId}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDetailsVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PaymentDetailsVm>> PGSuccess([FromRoute] string applicationId,
        BajajPaymentResponseModel bajajPaymentResponseModel,
        CancellationToken cancellationToken)
    {
        GetPaymentQuery getPaymentQuery = new GetPaymentQuery()
        {
            Status = bajajPaymentResponseModel.p_pay_status,
            ReferenceNo = bajajPaymentResponseModel.policyref,
            TransactionNo = bajajPaymentResponseModel.p_policy_ref,
            TransactionId = applicationId,
            IsTP = false
        };
        var result = await _mediatr.Send(getPaymentQuery, cancellationToken);
        if (result != null)
        {
            var res = Result.CreateSuccess(result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        var problemDetails = Result.CreateNotFoundError("Data not found");
        return NotFound(problemDetails);
    }

    [HttpPost("TPPGSuccess/{applicationId}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDetailsVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PaymentDetailsVm>> TPPGSuccess([FromRoute] string applicationId,
        BajajPaymentResponseModel bajajPaymentResponseModel,
        CancellationToken cancellationToken)
    {
        GetPaymentQuery getPaymentQuery = new GetPaymentQuery()
        {
            Status = bajajPaymentResponseModel.status,
            Amount = bajajPaymentResponseModel.amt,
            TransactionNo = bajajPaymentResponseModel.txn,
            ReferenceNo = bajajPaymentResponseModel.referenceno,
            QuoteNo = bajajPaymentResponseModel.quoteno,
            TransactionId = applicationId,
            IsTP = true
        };
        var result = await _mediatr.Send(getPaymentQuery, cancellationToken);
        if (result != null)
        {
            var res = Result.CreateSuccess(result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        var problemDetails = Result.CreateNotFoundError("Data not found");
        return NotFound(problemDetails);
    }

    [HttpPost("BreakinPinStatus/{quotetransactionId}/{vehicleNumber}")]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> BreakinPinStatus([FromRoute] string vehicleNumber, [FromRoute] string quotetransactionId, [FromQuery] string leadId, CancellationToken cancellationToken)
    {
        BajajBreakinPinStatusCommand query = new BajajBreakinPinStatusCommand()
        {
            VehicleNumber = vehicleNumber,
            QuotetransactionId = quotetransactionId,
            LeadId = leadId
        };
        var result = await _mediatr.Send(query, cancellationToken);
        if (result != null)
        {
            var res = Result.CreateSuccess(result.Messages, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        var problemDetails = Result.CreateNotFoundError("Data not found");
        return NotFound(problemDetails);
    }
}
