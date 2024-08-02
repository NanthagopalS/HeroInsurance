using AutoMapper;
using Insurance.API.Models;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.HDFC.Command.CreatePOSP;
using Insurance.Core.Features.HDFC.Queries.GetCKYCPOA;
using Insurance.Core.Features.HDFC.Queries.GetCKYCPOAStatus;
using Insurance.Core.Features.HDFC.Queries.GetPaymentStatus;
using Insurance.Core.Features.HDFC.Queries.GetQuote;
using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails;
using Insurance.Core.Helpers;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.Quote;
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
public class HDFCController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public HDFCController(IMediator mediatr, IMapper mapper)
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
    public async Task<ActionResult<QuoteResponseModel>> GetQuote(GetHdfcQuoteQuery request, CancellationToken cancellationToken)
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

    /// <summary>
    /// GetQuote
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
    public async Task<ActionResult<UploadCKYCDocumentResponse>> GetPOAStatus([FromBody] HDFCPOAResponseModel hdfcPOAResponse, CancellationToken cancellationToken)
    {
        GetCKYCPOAQuery getCKYCQuery = new GetCKYCPOAQuery
        {
            QuoteTransactionId= hdfcPOAResponse.Id,
            TxnId= hdfcPOAResponse.TxnId,
            Status= hdfcPOAResponse.Status,
            KYCId= hdfcPOAResponse.KYCId
        };

        var result = await _mediatr.Send(getCKYCQuery, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    [HttpGet("GetCKYCStatus/{transactionId}/{kycId}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDetailsVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(UploadCKYCDocumentResponse), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public  async Task<ActionResult<UploadCKYCDocumentResponse>> CKCYPOAStatus(string transactionId, string kycId, CancellationToken cancellationToken)
    {
        GetCKYCPOAStatusQuery getCKYCQuery = new GetCKYCPOAStatusQuery
        {
            QuoteTransactionId = transactionId,
            KYCId = kycId
        };

        var result = await _mediatr.Send(getCKYCQuery, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);   
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    [HttpPost("CreatePOSP")]
    [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> CreatePOSP(CreatePOSPRequestModel pospCreateModel, CancellationToken cancellationToken)
    {
        CreatePOSPCommand createIMBrokerCommand = new CreatePOSPCommand()
        {
            POSPId = pospCreateModel.PospId,
            AadharNumber= pospCreateModel.AadharNumber,
            EmailId= pospCreateModel.EmailId,
            MobileNumber= pospCreateModel.MobileNumber,    
            Name= pospCreateModel.Name,    
            PanNumber= pospCreateModel.PanNumber,
            State = pospCreateModel.State  
        };
        var result = await _mediatr.Send(createIMBrokerCommand, cancellationToken);
        if (!result.Failed)
        {
            var res = Result.CreateSuccess($"Successfully Updated HDFC POSPId for {pospCreateModel.PospId}", (int)HttpStatusCode.OK);
            return Ok(res);
        }
        var problemDetails = Result.CreateNotFoundError("Failed To Update HDFC POSPId");
        return NotFound(problemDetails);
    }

    /// <summary>
    /// Success URL
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("PGStatus/{quoteTransactionId}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDetailsVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ActionResult<PaymentDetailsVm> PGStatus([FromRoute] string quoteTransactionId, HDFCPaymentResponseModel hdfcPaymentModel, CancellationToken cancellationToken)
    {
        var request = _mapper.Map<GetPaymentQuery>(hdfcPaymentModel);
        request.QuoteTransactionId= quoteTransactionId;

        var result = _mediatr.Send(request, cancellationToken);

        if (result != null)
        {
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);

        }
        var problemDetails = Result.CreateNotFoundError("Data not found");
        return NotFound(problemDetails);
    }

    //[HttpGet("GetPolicyDocument/{transactionId}/{policyNumber}")]
    //[ProducesResponseType(typeof(IEnumerable<PaymentDetailsVm>), (int)HttpStatusCode.OK)]
    //[ProducesResponseType(typeof(UploadCKYCDocumentResponse), (int)HttpStatusCode.Unauthorized)]
    //[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    //public async Task<ActionResult<UploadCKYCDocumentResponse>> GetPolicyDocument(string transactionId, string policyNumber, CancellationToken cancellationToken)
    //{
    //    GetPolicyDocumentQuery getPolicyDocumentQuery = new GetPolicyDocumentQuery
    //    {
    //        QuoteTransactionId = transactionId,
    //        PolicyDocument = policyNumber
    //    };

    //    var result = await _mediatr.Send(getPolicyDocumentQuery, cancellationToken);

    //    if (result.Failed)
    //    {
    //        var problemDetails = Result.CreateNotFoundError(result.Messages);
    //        return NotFound(problemDetails);
    //    }
    //    var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
    //    return Ok(res);
    //}
}
