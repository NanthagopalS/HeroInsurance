using AutoMapper;
using Insurance.API.Helpers;
using Insurance.API.Models;
using Insurance.Core.Features.HDFC.Command.QuoteConfirm;
using Insurance.Core.Features.HDFC.Queries.GetQuote;
using Insurance.Core.Features.ICICI.Command.QuoteConfirm;
using Insurance.Core.Features.IFFCO.Command.QuoteConfirm;
using Insurance.Core.Features.IFFCO.Queries.GetQuote;
using Insurance.Core.Features.ICICI.Queries.GetQuote;
using Insurance.Core.Features.Quote.Command.InsertQuoteConfirmRequest;
using Insurance.Core.Features.Quote.Command.InsertQuoteRequest;
using Insurance.Core.Features.Quote.Command.SaveForLater;
using Insurance.Core.Features.Quote.Query.GetPolicyDatesCommercial;
using Insurance.Core.Models;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Helpers;
using Insurance.Core.Helpers;
using Insurance.Core.Features.Reliance.Command.GetQuote;
using Insurance.Core.Features.Reliance.Command.QuoteConfirm;

namespace Insurance.API.Controllers.v1;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[ApiController]
[ServiceFilter(typeof(ResponseCaptureFilter))]
public class CommercialVehicleController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public CommercialVehicleController(IMediator mediatr, IMapper mapper)
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
    [HttpPost("GetQuote/{insurerid}")]
    [ProducesResponseType(typeof(IEnumerable<QuoteResponseModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuoteResponseModel>>> GetQuote(string insurerid, QuoteBaseCommand request, CancellationToken cancellationToken)
    {
        var policyDateQuery = new GetPolicyDatesQueryCommercial();

        if (request?.PreviousPolicy is not null)
        {
            policyDateQuery.IsPreviousPolicy = request.PreviousPolicy.IsPreviousPolicy;
            policyDateQuery.ODPolicyExpiry = request.PreviousPolicy.SAODPolicyExpiryDate;
            policyDateQuery.TPPolicyExpiry = request.PreviousPolicy.TPPolicyExpiryDate;
            policyDateQuery.RegistrationYear = request.RegistrationYear;
            policyDateQuery.ManufacturingDate = request.RegistrationYear;
            policyDateQuery.VehicleType = request.VehicleTypeId;
            policyDateQuery.PreviousPolicyTypeId = request.PreviousPolicy.PreviousPolicyTypeId;
            policyDateQuery.IsBrandNewVehicle = request.IsBrandNewVehicle;
        }
        else
            policyDateQuery.IsPreviousPolicy = false;

        var policyDateResult = await _mediatr.Send(policyDateQuery, cancellationToken);
        request.PolicyDates = policyDateResult.Result;

        var insertQuoteRequest = _mapper.Map<InsertQuoteRequestCommand>(request);
        _ = await _mediatr.Send(insertQuoteRequest, cancellationToken);

        if (!string.IsNullOrEmpty(request.VehicleNumber) && !request.IsBrandNewVehicle && request.VehicleNumber.Length > 4)
        {
            var vehicleNumber = CommonUtilityHelper.VehicleNumberSplit(request.VehicleNumber);
            if (vehicleNumber.Any())
            {
                if (vehicleNumber[0].ToUpper().Equals("DL") && vehicleNumber[1].Length == 1)
                {
                    request.VehicleNumber = $"{vehicleNumber[0]}0{vehicleNumber[1]}{vehicleNumber[2]}{vehicleNumber[3]}";
                }
            }
        }
        //Need to check for RegistrationYear or RegistrationDate
        if (request.RegistrationYear?.ToCharArray().Length != 4)
        {
            request.RegistrationYear = Convert.ToString((Convert.ToDateTime(request.RegistrationYear).Year));
        }

        switch (insurerid)
        {
            case ("0A326B77-AFD5-44DA-9871-1742624CFF16"):
                var hdfcQuery = _mapper.Map<GetCommercialHDFCQuoteQuery>(request);
                var hdfcResponse = await _mediatr.Send(hdfcQuery, cancellationToken);
                if (hdfcResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(hdfcResponse.Messages);
                    return NotFound(problemDetails);
                }
                var hdfcResult = Result.CreateSuccess(hdfcResponse.Result, (int)HttpStatusCode.OK);
                return Ok(hdfcResult);
            case ("FD3677E5-7938-46C8-9CD2-FAE188A1782C"):
                var iciciQuery = _mapper.Map<GetCommercialICICIQuoteQuery>(request);
                var iciciResponse = await _mediatr.Send(iciciQuery, cancellationToken);
                if (iciciResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(iciciResponse.Messages);
                    return NotFound(problemDetails);
                }
                var iciciResult = Result.CreateSuccess(iciciResponse.Result, (int)HttpStatusCode.OK);
                return Ok(iciciResult);
            case ("E656D5D1-5239-4E48-9048-228C67AE3AC3"):
                var iffcoQuery = _mapper.Map<GetCommercialIFFCOQuoteQuery>(request);
                var iffcoResponse = await _mediatr.Send(iffcoQuery, cancellationToken);
                if (iffcoResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(iffcoResponse.Messages);
                    return NotFound(problemDetails);
                }
                var iffcoResult = Result.CreateSuccess(iffcoResponse.Result, (int)HttpStatusCode.OK);
                return Ok(iffcoResult);

            case ("372B076C-D9D9-48DC-9526-6EB3D525CAB6"):
                var relianceCVQuery = _mapper.Map<GetCVRelianceQuery>(request);
                var relianceCVResponse = await _mediatr.Send(relianceCVQuery, cancellationToken);
                if (relianceCVResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(relianceCVResponse.Messages);
                    return NotFound(problemDetails);
                }
                var relianceCVResult = Result.CreateSuccess(relianceCVResponse.Result, (int)HttpStatusCode.OK);
                return Ok(relianceCVResult);
            default:
                break;
        }
        return default;
    }

    /// <summary>
    /// QuoteConfirmDetails
    /// </summary>
    /// <param name="QuoteTranID"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("QuoteConfirmDetails/{insurerId}/{quoteTranId}")]
    [ProducesResponseType(typeof(IEnumerable<QuoteConfirmDetailsResponseModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuoteConfirmDetailsResponseModel>>> QuoteConfirmDetails(string insurerId, string quoteTranId, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        quoteConfirmCommand.InsurerId = insurerId;
        quoteConfirmCommand.QuoteTransactionId = quoteTranId;

        var policyDateQuery = new GetPolicyDatesQueryCommercial();

        if (quoteConfirmCommand?.PreviousPolicy != null)
        {
            policyDateQuery.IsPreviousPolicy = quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy;
            policyDateQuery.ODPolicyExpiry = quoteConfirmCommand.PreviousPolicy.SAODPolicyExpiryDate;
            policyDateQuery.TPPolicyExpiry = quoteConfirmCommand.PreviousPolicy.TPPolicyExpiryDate;
            policyDateQuery.RegistrationYear = quoteConfirmCommand.RegistrationDate;
            policyDateQuery.VehicleType = quoteConfirmCommand.VehicleTypeId;
            policyDateQuery.PreviousPolicyTypeId = quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId;
            policyDateQuery.IsBrandNewVehicle = quoteConfirmCommand.IsBrandNewVehicle;
            policyDateQuery.ManufacturingDate = quoteConfirmCommand.ManufacturingMonthYear;
        }
        else
            policyDateQuery.IsPreviousPolicy = false;

        var policyDateResult = await _mediatr.Send(policyDateQuery, cancellationToken);
        if (policyDateResult.Failed)
        {
            var problemDetails = Result.CreateValidationError(policyDateResult.Messages);
            return BadRequest(problemDetails);
        }

        quoteConfirmCommand.PolicyDates = policyDateResult.Result;
        var insertQuoteRequest = _mapper.Map<InsertQuoteConfirmRequestCommand>(quoteConfirmCommand);
        var response = await _mediatr.Send(insertQuoteRequest, cancellationToken);
        var result = (dynamic)null;
        switch (insurerId)
        {
            case ("0A326B77-AFD5-44DA-9871-1742624CFF16"):
                var hdfcQuoteConfirmCommand = _mapper.Map<HDFCQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(hdfcQuoteConfirmCommand, cancellationToken);
                break;

            case ("FD3677E5-7938-46C8-9CD2-FAE188A1782C"):
                var iciciQuoteConfirmCommand = _mapper.Map<ICICIQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(iciciQuoteConfirmCommand, cancellationToken);
                break;

            case ("E656D5D1-5239-4E48-9048-228C67AE3AC3"):
                var iffcoQuoteConfirmCommand = _mapper.Map<IffcoQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(iffcoQuoteConfirmCommand, cancellationToken);
                break;
            case ("372B076C-D9D9-48DC-9526-6EB3D525CAB6"):
                var relianceQuoteConfirmCommand = _mapper.Map<RelianceQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(relianceQuoteConfirmCommand, cancellationToken);
                break;
        }

        if (result.Failed)
        {
            var problemDetails = Result.CreateValidationError(result.Messages);
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    [HttpPost("SaveForLater/{insurerId}/{quoteTranId}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> SaveForLater(string insurerId, string quoteTranId, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        quoteConfirmCommand.InsurerId = insurerId;
        quoteConfirmCommand.QuoteTransactionId = quoteTranId;

        var policyDateQuery = new GetPolicyDatesQueryCommercial();

        if (quoteConfirmCommand?.PreviousPolicy != null)
        {
            policyDateQuery.IsPreviousPolicy = quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy;
            policyDateQuery.ODPolicyExpiry = quoteConfirmCommand.PreviousPolicy.SAODPolicyExpiryDate;
            policyDateQuery.TPPolicyExpiry = quoteConfirmCommand.PreviousPolicy.TPPolicyExpiryDate;
            policyDateQuery.RegistrationYear = quoteConfirmCommand.RegistrationDate;
            policyDateQuery.VehicleType = quoteConfirmCommand.VehicleTypeId;
            policyDateQuery.PreviousPolicyTypeId = quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId;
            policyDateQuery.IsBrandNewVehicle = quoteConfirmCommand.IsBrandNewVehicle;
            policyDateQuery.ManufacturingDate = quoteConfirmCommand.ManufacturingMonthYear;
        }
        else
            policyDateQuery.IsPreviousPolicy = false;

        var policyDateResult = await _mediatr.Send(policyDateQuery, cancellationToken);
        if (policyDateResult.Failed)
        {
            var problemDetails = Result.CreateValidationError(policyDateResult.Messages);
            return BadRequest(problemDetails);
        }

        quoteConfirmCommand.PolicyDates = policyDateResult.Result;

        var saveForLaterCommand = _mapper.Map<SaveForLaterCommand>(quoteConfirmCommand);
        var result = await _mediatr.Send(saveForLaterCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateValidationError(result.Messages);
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Messages, (int)HttpStatusCode.OK);
        return Ok(res);
    }
}
