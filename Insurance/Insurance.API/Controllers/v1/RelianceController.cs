using AutoMapper;
using Insurance.API.Models;
using Insurance.Core.Features.Reliance.Queries.GetCKYCPOA;
using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails;
using Insurance.Core.Features.Reliance.Command.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Helpers;
using Insurance.Core.Features.Reliance.Queries.GetPaymentWrapper;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Domain.Reliance;
using Insurance.Core.Helpers;

namespace Insurance.API.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[ApiController]
[ServiceFilter(typeof(ResponseCaptureFilter))]
public class RelianceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="mediator"></param>
    public RelianceController(IMediator mediator,IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("GetQuote")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuoteResponse>>> GetQuote(GetRelianceQuery getRelianceQuery, CancellationToken cancellationToken)
    {
        var policyDateQuery = _mapper.Map<GetPolicyDatesQuery>(getRelianceQuery);
        var policyDateResponse = await _mediator.Send(policyDateQuery, cancellationToken);

        if (policyDateResponse.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Policy date is not correct");
            return NotFound(problemDetails);
        }
        getRelianceQuery.PolicyDates = policyDateResponse.Result;

        var result = await _mediator.Send(getRelianceQuery, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Get POA Status
    /// </summary>
    ///// <param name="insuranceType">InsuranceType: MOTOR/HEALTH/TERM</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [AllowAnonymous]
    [HttpPost("GetPOAStatus/{quotetransactionId}")]
    [ProducesResponseType(typeof(IEnumerable<UploadCKYCDocumentResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<UploadCKYCDocumentResponse>> GetPOAStatus([FromBody] ReliancePOAResponseModel reliancePOAResponse, CancellationToken cancellationToken)
    {
        GetCKYCPOAQuery getCKYCQuery = new GetCKYCPOAQuery
        {
            QuoteTransactionId = reliancePOAResponse.QuoteTransactionId,
            TransactionId = reliancePOAResponse.TransactionId,
            Status = reliancePOAResponse.KYCProcess,
            KYCNumber = reliancePOAResponse.KYCNumber,
            RegisteredName = reliancePOAResponse.RegisteredName,
            FirstName = reliancePOAResponse.FirstName,
            MiddleName  = reliancePOAResponse.MiddleName,
            LastName = reliancePOAResponse.LastName,
            DOB = reliancePOAResponse.DOB,
            Gender = reliancePOAResponse.Gender,
            Mobile = reliancePOAResponse.Mobile,
            Email = reliancePOAResponse.Email,
            KYCProcess =    reliancePOAResponse.KYCProcess,
            CorrAddressLine1 = reliancePOAResponse.CorrAddressLine1,
            CorrAddressLine2 = reliancePOAResponse.CorrAddressLine2,
            CorrCity = reliancePOAResponse.CorrCity,
            CorrCountry = reliancePOAResponse.CorrCountry,
            CorrPinCode =   reliancePOAResponse.CorrPinCode,
            CorrState = reliancePOAResponse.CorrState,
            PerAddressLine1 = reliancePOAResponse.PerAddressLine1,
            PerAddressLine2 = reliancePOAResponse.PerAddressLine2,
            PerCity = reliancePOAResponse.PerCity,
            PerCountry = reliancePOAResponse.PerCountry,
            PerPinCode = reliancePOAResponse.PerPinCode,
            PerState = reliancePOAResponse.PerState,
            KYCVerified = reliancePOAResponse.KYCVerified,
            ProposalId = reliancePOAResponse.ProposalId,
            VerifiedAt = reliancePOAResponse.VerifiedAt,
            UDP1 = reliancePOAResponse.UDP1
        };

        var result = await _mediator.Send(getCKYCQuery, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Payment Wrapper
    /// </summary>
    /// <param name="cancellationToken"></param>
    [HttpPost("PGStatus/{QuoteTransactionId}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDetailsVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PaymentDetailsVm>> PGStatus([FromRoute] string QuoteTransactionId, ReliancePaymentResponseModel paymentModel, CancellationToken cancellationToken)
    {
        var request = _mapper.Map<GetReliancePaymentWrapperQuery>(paymentModel);
        request.QuoteTransactionId = QuoteTransactionId;
        var result = await _mediator.Send(request, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
}

