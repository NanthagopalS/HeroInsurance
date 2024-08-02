using AutoMapper;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails;
using Insurance.Core.Features.UnitedIndia.Command;
using Insurance.Core.Features.UnitedIndia.Queries;
using Insurance.Core.Features.UnitedIndia.Queries.GetCKYCPOAStatus;
using Insurance.Core.Features.UnitedIndia.Queries.GetFinancierBranch;
using Insurance.Core.Features.UnitedIndia.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.UnitedIndia;
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
public class UnitedIndiaController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediatR;
    public UnitedIndiaController(IMapper mapper, IMediator mediatR)
    {
        _mapper = mapper;
        _mediatR = mediatR;
    }
    [HttpPost("GetQuote")]
    [ProducesResponseType(typeof(IEnumerable<QuoteResponseModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuoteResponseModel>>> GetQuote(GetUnitedIndiaQuoteQuery getUnitedIndiaQuoteQuery, CancellationToken cancellationToken)
    {
        var policyDateQuery = _mapper.Map<GetPolicyDatesQuery>(getUnitedIndiaQuoteQuery);
        var policyDateResponse = await _mediatR.Send(policyDateQuery, cancellationToken);
        if (policyDateResponse.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Policy date is not correct");
            return NotFound(problemDetails);
        }

        getUnitedIndiaQuoteQuery.PolicyDates = policyDateResponse.Result;
        try
        {
            var result = await _mediatR.Send(getUnitedIndiaQuoteQuery, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return Ok(ex.Message);
        }
    }

    [HttpPost("InitiatePayment")]
    [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<string>>> InitiatePayment(InitiatePaymentCommand initiatePaymentCommand, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediatR.Send(initiatePaymentCommand, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            res.Data = result.Messages[0];
            return Ok(res);
        }
        catch (Exception ex)
        {
            return Ok(ex.Message);
        }
    }

    [HttpPost("SavePaymentStatus")]
    [ProducesResponseType(typeof(PaymentDetailsVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PaymentDetailsVm>> SavePaymentStatus(InitiatePaymentRequestDto paymentResponseModel, CancellationToken cancellationToken)
    {
        try
        {
            var getUnitedIndiaVerifyPayment = _mapper.Map<GetPaymentStatusQuery>(paymentResponseModel);

            var result = await _mediatR.Send(getUnitedIndiaVerifyPayment, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = ThirdPartyUtilities.Helpers.Result.CreateNotFoundError("Get Payment Status Data not found");
                return NotFound(problemDetails);
            }
            var res = ThirdPartyUtilities.Helpers.Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return Ok(ex.Message);
        }
    }

    /// <summary>
    /// GetQuote
    /// </summary>
    ///// <param name="insuranceType">InsuranceType: MOTOR/HEALTH/TERM</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [AllowAnonymous]
    [HttpPost("GetPOAStatus")]
    [ProducesResponseType(typeof(IEnumerable<CreateLeadModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<CreateLeadModel>> GetPOAStatus([FromBody] UnitedIndiaCKYCResponseModel unitedIndiaCKYCResponseModel, CancellationToken cancellationToken)
    {
        try
        {
            GetUnitedIndiaCKYCPOAQuery getUnitedIndiaCKYCPOAQuery = new GetUnitedIndiaCKYCPOAQuery
            {
                QuoteTransactionId = unitedIndiaCKYCResponseModel.QuoteTransactionId,
                UserId = unitedIndiaCKYCResponseModel.UserId,
                Status = unitedIndiaCKYCResponseModel.Status,
                TrannsactionId = unitedIndiaCKYCResponseModel.TrannsactionId
            };
            var result = await _mediatR.Send(getUnitedIndiaCKYCPOAQuery, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError(result.Messages);
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return Ok(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("GetFinancierBranch")]
    [ProducesResponseType(typeof(IEnumerable<UploadCKYCDocumentResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<NameValueModel>>> GetFinancierBranch(string financierCode, CancellationToken cancellationToken)
    {
        GetFinancierBranchQuery getFinancierBranchQuery = new GetFinancierBranchQuery
        {
            financierCode = financierCode,
        };

        var result = await _mediatR.Send(getFinancierBranchQuery, cancellationToken);
        if (result == null)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
}
