using AutoMapper;
using Insurance.API.Models;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails;
using Insurance.Core.Features.TATA.Command.VerifyAadharOTP;
using Insurance.Core.Features.TATA.Queries.GetBreakIn;
using Insurance.Core.Features.TATA.Queries.GetPaymentStatus;
using Insurance.Core.Features.TATA.Queries.GetQuote;
using Insurance.Core.Helpers;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
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
public class TATAController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public TATAController(IMediator mediatr, IMapper mapper)
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
    public async Task<ActionResult<QuoteResponseModel>> GetQuote(GetTATAQuoteQuery request, CancellationToken cancellationToken)
    {
        var policyDateQuery = _mapper.Map<GetPolicyDatesQuery>(request);
        var policyDateResponse = await _mediatr.Send(policyDateQuery, cancellationToken);

        if (policyDateResponse.Failed)
        {
            var problemDetails = ThirdPartyUtilities.Helpers.Result.CreateNotFoundError("Policy date is not correct");
            return NotFound(problemDetails);
        }
        request.PolicyDates = policyDateResponse.Result;

        var result = await _mediatr.Send(request, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = ThirdPartyUtilities.Helpers.Result.CreateNotFoundError("Data not found");
            return NotFound(problemDetails);
        }
        var res = ThirdPartyUtilities.Helpers.Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    /// <summary>
    /// GetBreakinStatus
    /// </summary>
    ///// <param name="LeadId"></param>
    /////<param name="ProposalNo"></param>
    /////<param name="TicketId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("VerifyBreakIn")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ResultModel>> VerifyBreakIn([FromQuery] string VehicleTypeId, [FromQuery] string LeadId, [FromQuery] string ProposalNo, [FromQuery] string TicketId, [FromQuery] string QuoteTransactionId, CancellationToken cancellationToken)
	{
		GetTATABreakInVerificationQuery getTATABreakinVerificationQuery = new GetTATABreakInVerificationQuery()
		{
			LeadId = LeadId,
            ProposalNo = ProposalNo,
            TicketId = TicketId,
            VehicleTypeId = VehicleTypeId,
            QuoteTransactionId = QuoteTransactionId
		};
		var result = await _mediatr.Send(getTATABreakinVerificationQuery, cancellationToken);
		if (result.Failed)
		{
			var problemDetails = ThirdPartyUtilities.Helpers.Result.CreateNotFoundError("Get Breakin Status Data not found");
			return NotFound(problemDetails);
		}
		var res = ThirdPartyUtilities.Helpers.Result.CreateSuccess(result.Messages, (int)HttpStatusCode.OK);
		return Ok(res);
	}

    /// <summary>
    /// GetBreakinStatus
    /// </summary>
    ///// <param name="LeadId"></param>
    /////<param name="ProposalNo"></param>
    /////<param name="TicketId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("VerifyPaymentCronJob")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PaymentDetailsVm>> VerifyPaymentCronJob([FromQuery] string VehicleTypeId, [FromQuery] string LeadId, [FromQuery] string PaymentId, [FromQuery] string ProposalNumber, CancellationToken cancellationToken)
    {
        TATAVerifyPaymentStatusCronjobQuery getTATAVerifyPayment = new TATAVerifyPaymentStatusCronjobQuery()
        {
            LeadId = LeadId,
            PaymentId = PaymentId,
            VehicleTypeId = VehicleTypeId,
            ProposalNumber = ProposalNumber
        };
        var result = await _mediatr.Send(getTATAVerifyPayment, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = ThirdPartyUtilities.Helpers.Result.CreateNotFoundError("Get Payment Status Data not found");
            return NotFound(problemDetails);
        }
        var res = ThirdPartyUtilities.Helpers.Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// GetBreakinStatus
    /// </summary>
    ///// <param name="LeadId"></param>
    /////<param name="ProposalNo"></param>
    /////<param name="TicketId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpPost("SavePaymentStatus")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PaymentDetailsVm>> SavePaymentStatus(TATAPaymentResponseModel paymentResponseModel, CancellationToken cancellationToken)
    {
        var getTATAVerifyPayment = _mapper.Map<TATAVerifyPaymentStatus>(paymentResponseModel);

        var result = await _mediatr.Send(getTATAVerifyPayment, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = ThirdPartyUtilities.Helpers.Result.CreateNotFoundError("Get Payment Status Data not found");
            return NotFound(problemDetails);
        }
        var res = ThirdPartyUtilities.Helpers.Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// GetBreakinStatus
    /// </summary>
    ///// <param name="LeadId"></param>
    /////<param name="ProposalNo"></param>
    /////<param name="TicketId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpPut("VerifyAadharOTP/{quoteTransactionId}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<SaveCKYCResponse>> VerifyAadharOTP(string quoteTransactionId, POAAadharOTPSubmitRequestModel poaAadharOTPSubmitRequestModel, CancellationToken cancellationToken)
    {
        var getTATAVerifyPayment = _mapper.Map<TATAVerifyAadharOTPCommand>(poaAadharOTPSubmitRequestModel);
        getTATAVerifyPayment.QuoteTransactionId = quoteTransactionId;

        var result = await _mediatr.Send(getTATAVerifyPayment, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = ThirdPartyUtilities.Helpers.Result.CreateNotFoundError(result.Messages);
            return NotFound(problemDetails);
        }
        var res = ThirdPartyUtilities.Helpers.Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
}


