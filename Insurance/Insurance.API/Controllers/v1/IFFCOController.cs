using AutoMapper;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.IFFCO.Command.SavePaymentStatus;
using Insurance.Core.Features.IFFCO.Queries.GetBreakinStatus;
using Insurance.Core.Features.IFFCO.Queries.GetQuote;
using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails;
using Insurance.Core.Helpers;
using Insurance.Domain.GoDigit;
using Insurance.Domain.IFFCO;
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
	public class IFFCOController : ControllerBase
    {
        private readonly IMediator _mediatR;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize and set the dependencies
        /// </summary>
        /// <param name="mediatR"></param>
        /// 
        public IFFCOController(IMediator mediator,IMapper mapper)
        {
            _mediatR = mediator;
            _mapper = mapper;
        }
        [HttpPost("GetQuote")]
        [ProducesResponseType(typeof(IEnumerable<QuoteResponseModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<QuoteResponseModel>>> GetQuote(GetIFFCOQuery getIFFCOQuery, CancellationToken cancellationToken)
        {
            var policyDateQuery = _mapper.Map<GetPolicyDatesQuery>(getIFFCOQuery);
            var policyDateResponse = await _mediatR.Send(policyDateQuery, cancellationToken);
            if (policyDateResponse.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Policy date is not correct");
                return NotFound(problemDetails);
            }


            getIFFCOQuery.PolicyDates = policyDateResponse.Result;
            try
            {
               var  result = await _mediatR.Send(getIFFCOQuery, cancellationToken);
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

        [HttpPost("SavePaymentStatus")]
        [ProducesResponseType(typeof(IEnumerable<PaymentDetailsVm>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PaymentDetailsVm>> SavePaymentStatus(IFFCOPaymentResponseModel iFFCOPaymentResponseModel, CancellationToken cancellationToken)
        {
            var request = _mapper.Map<SavePaymentStatusCommand>(iFFCOPaymentResponseModel);

            var result = await _mediatR.Send(request, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError(result.Messages);
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        [HttpPost("GetBreakinStatus/{quotetransactionId}/{breakinId}/{proposalNumber}")]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<string>> BreakinPinStatus([FromRoute] string quotetransactionId, [FromRoute] string breakinId, [FromRoute] string proposalNumber, CancellationToken cancellationToken)
        {
            GetIFFCOBreakinStatusQuery query = new GetIFFCOBreakinStatusQuery()
            {
                QuoteTransactionId = quotetransactionId,
                BreakinId = breakinId,
                ProposalNumber = proposalNumber
            };
            var result = await _mediatR.Send(query, cancellationToken);
            if (result != null)
            {
                var res = Result.CreateSuccess(result.Messages, (int)HttpStatusCode.OK);
                return Ok(res);
            }
            var problemDetails = Result.CreateNotFoundError("Data not found");
            return NotFound(problemDetails);
        }
    }
}
