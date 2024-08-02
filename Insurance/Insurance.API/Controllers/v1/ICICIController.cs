using AutoMapper;
using Insurance.API.Models;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.ICICI.Command.CreateIMBroker;
using Insurance.Core.Features.ICICI.Command.GetBreakinStatus;
using Insurance.Core.Features.ICICI.Command.Payment;
using Insurance.Core.Features.ICICI.Queries.GetQuote;
using Insurance.Core.Features.Quote.Query.GetPolicyDatesCommercial;
using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails;
using Insurance.Core.Helpers;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
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
	public class ICICIController : ControllerBase
    {
        private readonly IMediator _mediatR;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize and set the dependencies
        /// </summary>
        /// <param name="mediatR"></param>
        public ICICIController(IMediator mediatR, IMapper mapper)
        {
            _mediatR = mediatR;
            _mapper = mapper;
        }
        [HttpPost("GetQuote")]
        [ProducesResponseType(typeof(IEnumerable<QuoteResponseModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<QuoteResponseModel>>> GetQuote(GetIciciQuoteQuery getICICIQuery, CancellationToken cancellationToken)
        {
            var policyDateQuery = _mapper.Map<GetPolicyDatesQuery>(getICICIQuery);
            var policyDateResponse = await _mediatR.Send(policyDateQuery, cancellationToken);

            if (policyDateResponse.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Policy date is not correct");
                return NotFound(problemDetails);
            }


            getICICIQuery.PolicyDates = policyDateResponse.Result;
            var result = await _mediatR.Send(getICICIQuery, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }

        /// <summary>
        /// SuccessURL
        /// </summary>
        /// <param name="transactionNumber"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("PaymentMapping/{correlationId}/{transactionId}")]
        [ProducesResponseType(typeof(IEnumerable<PaymentDetailsVm>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PaymentDetailsVm>> PaymentMapping(string transactionId, string correlationId, CancellationToken cancellationToken)
        {
            PaymentCommand paymentResponse = new PaymentCommand()
            {
                TransactionId = transactionId,
                CorrelationId = correlationId,
            };
            var result = await _mediatR.Send(paymentResponse, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateValidationError(result.Messages);
                return BadRequest(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }

        [HttpPost("CreateIMBroker")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<string>> CreateIMBroker(ICICICreateIMBrokerModel iCICICreateIMBrokerModel, CancellationToken cancellationToken)
        {
            var createIMBroker = _mapper.Map<CreateIMBrokerCommand>(iCICICreateIMBrokerModel);

            var result = await _mediatR.Send(createIMBroker, cancellationToken);
            if (result != null)
            {
                var res = Result.CreateSuccess(result.Messages, (int)HttpStatusCode.OK);
                return Ok(res);
            }
            var problemDetails = Result.CreateNotFoundError("Failed to create IM broker");
            return NotFound(problemDetails);
        }

        [HttpGet("GetBreakinStatus")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<string>> GetBreakinStatus(CancellationToken cancellationToken)
        {
            var request = new GetBreakinStatusQuery();
            var result = await _mediatR.Send(request, cancellationToken);
            if (result != null)
            {
                var res = Result.CreateSuccess(result.Messages, (int)HttpStatusCode.OK);
                return Ok(res);
            }
            var problemDetails = Result.CreateNotFoundError("Failed to get breakin status");
            return NotFound(problemDetails);
        }

        #region Commercial
        /// <summary>
        /// GetQuote
        /// </summary>
        ///// <param name="insuranceType">InsuranceType: MOTOR/HEALTH/TERM</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Dashboard data</returns>
        [HttpPost("GetQuote/cv")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<QuoteResponseModel>> GetQuote(GetCommercialICICIQuoteQuery request, CancellationToken cancellationToken)
        {
            var policyDateQuery = new GetPolicyDatesQueryCommercial();

            if (request?.PreviousPolicy is not null)
            {
                policyDateQuery.IsPreviousPolicy = request.PreviousPolicy.IsPreviousPolicy;
                policyDateQuery.ODPolicyExpiry = request.PreviousPolicy.SAODPolicyExpiryDate;
                policyDateQuery.TPPolicyExpiry = request.PreviousPolicy.TPPolicyExpiryDate;
                policyDateQuery.RegistrationYear = request.RegistrationYear;
                policyDateQuery.VehicleType = request.VehicleTypeId;
                policyDateQuery.PreviousPolicyTypeId = request.PreviousPolicy.PreviousPolicyTypeId;
                policyDateQuery.IsBrandNewVehicle = request.IsBrandNewVehicle;
            }
            else
                policyDateQuery.IsPreviousPolicy = false;

            var policyDateResult = await _mediatR.Send(policyDateQuery, cancellationToken);
            request.PolicyDates = policyDateResult.Result;
            //

            var result = await _mediatR.Send(request, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }

        #endregion
    }
}
