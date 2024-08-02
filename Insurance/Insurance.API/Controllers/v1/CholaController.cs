using AutoMapper;
using Insurance.API.Models;
using Insurance.Core.Features.Chola.Queries.GetBreakinStatus;
using Insurance.Core.Features.Chola.Queries.GetCKYCStatus;
using Insurance.Core.Features.Chola.Queries.GetPaymentStatus;
using Insurance.Core.Features.Chola.Queries.GetPaymentWrapper;
using Insurance.Core.Features.Chola.Queries.GetPOAStatus;
using Insurance.Core.Features.Chola.Queries.GetQuote;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.GoDigit.Queries.GetQuote;
using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails;
using Insurance.Core.Helpers;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Helpers;

namespace Insurance.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class CholaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize and set the dependencies
        /// </summary>
        /// <param name="mediator"></param>
        public CholaController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// GetQuote
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("GetQuote")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<QuoteResponseModel>> GetQuote(GetCholaQuery getCholaQuery, CancellationToken cancellationToken)
        {
            var policyDateQuery = _mapper.Map<GetPolicyDatesQuery>(getCholaQuery);
            var policyDateResponse = await _mediator.Send(policyDateQuery, cancellationToken);

            if (policyDateResponse.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Policy date is not correct");
                return NotFound(problemDetails);
            }
            getCholaQuery.PolicyDates = policyDateResponse.Result;
            var result = await _mediator.Send(getCholaQuery, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }

        /// <summary>
        /// GetCKYCStatus
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="appRefNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("GetCKYCStatus/{transactionId}/{appRefNo}/{quoteTransactionId}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<string>> GetCKYCStatus([FromRoute] string transactionId, [FromRoute] string appRefNo, [FromRoute] string quoteTransactionId, CancellationToken cancellationToken)
        {
            GetCholaCKYCStatusQuery cKYCStatus = new GetCholaCKYCStatusQuery()
            {
                TransactionID = transactionId,
                AppRefNo = appRefNo,
                QuoteTransactionId = quoteTransactionId,
            };
            var result = await _mediator.Send(cKYCStatus, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("CKYC Status Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess("CKYC Status Data found", (int)HttpStatusCode.OK);
            return Ok(res);
        }

        /// <summary>
        /// Payment Wrapper
        /// </summary>
        /// <param name="cancellationToken"></param>
        [HttpPost("PGStatus/{QuoteTransactionId}")]
        [ProducesResponseType(typeof(IEnumerable<PaymentDetailsVm>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PaymentDetailsVm>> PGStatus([FromRoute] string QuoteTransactionId, CholaPaymentResponseModel paymentModel, CancellationToken cancellationToken)
        {
            var request = _mapper.Map<GetPaymentWrapperQuery>(paymentModel);
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

        /// <summary>
        /// GetBreakinStatus
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("GetBreakinStatus/{referenceNumber}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<string>> GetBreakinStatus([FromRoute] string referenceNumber, [FromQuery] string LeadId, CancellationToken cancellationToken)
        {
            GetBreakinStatusQuery getBreakinStatusQuery = new GetBreakinStatusQuery()
            {
                ReferenceNumber = referenceNumber,
                LeadId = LeadId
            };
            var result = await _mediator.Send(getBreakinStatusQuery, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Get Breakin Status Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Messages, (int)HttpStatusCode.OK);
            return Ok(res);
        }

        /// <summary>
        /// Success URL
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("GetPGStatusCronJob/{applicationid}")]
        [ProducesResponseType(typeof(IEnumerable<CholaPaymentDetailsVm>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public ActionResult<CholaPaymentDetailsVm> GetPGStatusCronJob([FromRoute] string applicationId, CancellationToken cancellationToken)
        {
            GetCholaPaymentStatusQuery getPaymentStatusQuery = new GetCholaPaymentStatusQuery
            {
                ApplicationId = applicationId
            };
            var result = _mediator.Send(getPaymentStatusQuery, cancellationToken);
            if (result != null)
            {
                var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
                return Ok(res);
            }
            var problemDetails = Result.CreateNotFoundError("Data not found");
            return NotFound(problemDetails);
        }

        /// <summary>
        /// GetPOAStatus
        /// </summary>
        /// <param name="quoteTransactionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("GetPOAStatus/{quoteTransactionId}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(UploadCKYCDocumentResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<UploadCKYCDocumentResponse>> GetPOAStatus(string quoteTransactionId, CancellationToken cancellationToken)
        {
            GetPOAStatusQuery getPOAStatusQuery = new GetPOAStatusQuery()
            {
                QuoteTransactionId = quoteTransactionId
            };
            var result = await _mediator.Send(getPOAStatusQuery, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError(result.Messages);
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
    }
}
