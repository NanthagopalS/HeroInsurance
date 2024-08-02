using AutoMapper;
using Insurance.API.Helpers;
using Insurance.API.Models;
using Insurance.Core.Features.Bajaj.Command;
using Insurance.Core.Features.Bajaj.Command.CKYC;
using Insurance.Core.Features.Bajaj.Command.QuoteConfirm;
using Insurance.Core.Features.Bajaj.Command.UploadCKYCDocument;
using Insurance.Core.Features.Bajaj.Queries.GetPaymentLink;
using Insurance.Core.Features.Bajaj.Queries.GetQuote;
using Insurance.Core.Features.Chola.Command.CKYC;
using Insurance.Core.Features.Chola.Command.CreateProposal;
using Insurance.Core.Features.Chola.Command.QuoteConfirm;
using Insurance.Core.Features.Chola.Queries.GetPaymentLink;
using Insurance.Core.Features.Chola.Queries.GetQuote;
using Insurance.Core.Features.GoDigit.Command;
using Insurance.Core.Features.GoDigit.Command.CKYC;
using Insurance.Core.Features.GoDigit.Command.QuoteConfirm;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentLink;
using Insurance.Core.Features.GoDigit.Queries.GetQuote;
using Insurance.Core.Features.HDFC.Command.CKYC;
using Insurance.Core.Features.HDFC.Command.CreateProposal;
using Insurance.Core.Features.HDFC.Command.QuoteConfirm;
using Insurance.Core.Features.HDFC.Queries.GetPaymentLink;
using Insurance.Core.Features.HDFC.Queries.GetQuote;
using Insurance.Core.Features.ICICI.Command;
using Insurance.Core.Features.ICICI.Command.CKYC;
using Insurance.Core.Features.ICICI.Command.QuoteConfirm;
using Insurance.Core.Features.ICICI.Command.UploadICICICKYCDocument;
using Insurance.Core.Features.ICICI.Queries.GetPaymentLink;
using Insurance.Core.Features.ICICI.Queries.GetQuote;
using Insurance.Core.Features.IFFCO.Command.CKYC;
using Insurance.Core.Features.IFFCO.Command.CreateCKYC;
using Insurance.Core.Features.IFFCO.Command.CreateProposal;
using Insurance.Core.Features.IFFCO.Command.QuoteConfirm;
using Insurance.Core.Features.IFFCO.Queries.GetPaymentLink;
using Insurance.Core.Features.IFFCO.Queries.GetQuote;
using Insurance.Core.Features.Oriental.Command.CKYC;
using Insurance.Core.Features.Oriental.Command.CreateProposal;
using Insurance.Core.Features.Oriental.Command.QuoteConfirm;
using Insurance.Core.Features.Oriental.Queries.GetQuote;
using Insurance.Core.Features.Quote.Command.InsertQuoteConfirmRequest;
using Insurance.Core.Features.Quote.Command.InsertQuoteRequest;
using Insurance.Core.Features.Quote.Command.SaveForLater;
using Insurance.Core.Features.Quote.Command.SaveUpdateLead;
using Insurance.Core.Features.Quote.Query.CKYCDocumentField;
using Insurance.Core.Features.Quote.Query.GetCKYCField;
using Insurance.Core.Features.Quote.Query.GetLeadDetails;
using Insurance.Core.Features.Quote.Query.GetPaymentStatus;
using Insurance.Core.Features.Quote.Query.GetPolicyDocumentPDF;
using Insurance.Core.Features.Quote.Query.GetPreviousCoverMaster;
using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails;
using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetailsQueryHandler;
using Insurance.Core.Features.Quote.Query.GetPropoalField;
using Insurance.Core.Features.Quote.Query.GetProposalDetails;
using Insurance.Core.Features.Quote.Query.GetProposalField;
using Insurance.Core.Features.Quote.Query.GetProposalSummary;
using Insurance.Core.Features.Quote.Query.GetUserIdDetails;
using Insurance.Core.Features.Reliance.Command.CKYC;
using Insurance.Core.Features.Reliance.Command.CreateProposal;
using Insurance.Core.Features.Reliance.Command.GetQuote;
using Insurance.Core.Features.Reliance.Command.QuoteConfirm;
using Insurance.Core.Features.Reliance.Queries.GetPaymentLink;
using Insurance.Core.Features.TATA.Command.CKYC;
using Insurance.Core.Features.TATA.Command.CreateProposal;
using Insurance.Core.Features.TATA.Command.DocumentUploadPospProposal;
using Insurance.Core.Features.TATA.Command.QuoteConfirm;
using Insurance.Core.Features.TATA.Queries.GetPaymentLink;
using Insurance.Core.Features.TATA.Queries.GetQuote;
using Insurance.Core.Features.UnitedIndia.Command;
using Insurance.Core.Features.UnitedIndia.Command.CreateProposal;
using Insurance.Core.Features.UnitedIndia.Command.QuoteConfirm;
using Insurance.Core.Features.UnitedIndia.Queries.GetPaymentLink;
using Insurance.Core.Features.UnitedIndia.Queries.GetQuote;
using Insurance.Core.Helpers;
using Insurance.Core.Models;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Nodes;
using ThirdPartyUtilities.Helpers;

namespace Insurance.API.Controllers.v1;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[ApiController]
[ServiceFilter(typeof(ResponseCaptureFilter))]
public class QuoteController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public QuoteController(IMediator mediatr, IMapper mapper)
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
    [HttpPost("Quote/{insurerid}")]
    [ProducesResponseType(typeof(IEnumerable<QuoteResponseModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuoteResponseModel>>> Quote(string insurerid, QuoteBaseCommand request, CancellationToken cancellationToken)
    {
        var policyDateQuery = new GetPolicyDatesQuery();

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
            policyDateQuery.PACoverList = request.PACoverList;
        }
        else
            policyDateQuery.IsPreviousPolicy = false;

        var policyDateResult = await _mediatr.Send(policyDateQuery, cancellationToken);
        request.PolicyDates = policyDateResult.Result;
        request.IsDefaultPACoverRequired = policyDateResult.Result.IsDefaultPACoverRequired;

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
            case ("78190CB2-B325-4764-9BD9-5B9806E99621"):
                var goDigitQuery = _mapper.Map<GetGoDigitQuery>(request);
                var goDigitResponse = await _mediatr.Send(goDigitQuery, cancellationToken);
                if (goDigitResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError("Failed to fetch the quotation");
                    return NotFound(problemDetails);
                }
                var godigitResult = Result.CreateSuccess(goDigitResponse.Result, (int)HttpStatusCode.OK);
                return Ok(godigitResult);
            case ("0A326B77-AFD5-44DA-9871-1742624CFF16"):
                var hdfcQuery = _mapper.Map<GetHdfcQuoteQuery>(request);
                var hdfcResponse = await _mediatr.Send(hdfcQuery, cancellationToken);
                if (hdfcResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(hdfcResponse.Messages);
                    return NotFound(problemDetails);
                }
                var hdfcResult = Result.CreateSuccess(hdfcResponse.Result, (int)HttpStatusCode.OK);
                return Ok(hdfcResult);
            case ("16413879-6316-4C1E-93A4-FF8318B14D37"):
                var bajajQuery = _mapper.Map<GetBajajQuoteQuery>(request);
                var bajajResponse = await _mediatr.Send(bajajQuery, cancellationToken);
                if (bajajResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(bajajResponse.Messages);
                    return NotFound(problemDetails);
                }
                var bajajResult = Result.CreateSuccess(bajajResponse.Result, (int)HttpStatusCode.OK);
                return Ok(bajajResult);
            case ("FD3677E5-7938-46C8-9CD2-FAE188A1782C"):
                var iciciQuery = _mapper.Map<GetIciciQuoteQuery>(request);
                var iciciResponse = await _mediatr.Send(iciciQuery, cancellationToken);
                if (iciciResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(iciciResponse.Messages);
                    return NotFound(problemDetails);
                }
                var iciciResult = Result.CreateSuccess(iciciResponse.Result, (int)HttpStatusCode.OK);
                return Ok(iciciResult);
            case ("77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA"):
                var cholaQuery = _mapper.Map<GetCholaQuery>(request);
                var cholaResponse = await _mediatr.Send(cholaQuery, cancellationToken);
                if (cholaResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(cholaResponse.Messages);
                    return NotFound(problemDetails);
                }
                var cholaResult = Result.CreateSuccess(cholaResponse.Result, (int)HttpStatusCode.OK);
                return Ok(cholaResult);
            case ("372B076C-D9D9-48DC-9526-6EB3D525CAB6"):
                var relianceQuery = _mapper.Map<GetRelianceQuery>(request);
                var relianceResponse = await _mediatr.Send(relianceQuery, cancellationToken);
                if (relianceResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(relianceResponse.Messages);
                    return NotFound(problemDetails);
                }
                var relianceResult = Result.CreateSuccess(relianceResponse.Result, (int)HttpStatusCode.OK);
                return Ok(relianceResult);
            case ("E656D5D1-5239-4E48-9048-228C67AE3AC3"):
                var iffcoQuery = _mapper.Map<GetIFFCOQuery>(request);

                var iffcoResponse = await _mediatr.Send(iffcoQuery, cancellationToken);
                if (iffcoResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(iffcoResponse.Messages);
                    return NotFound(problemDetails);
                }
                var iffcoResult = Result.CreateSuccess(iffcoResponse.Result, (int)HttpStatusCode.OK);
                return Ok(iffcoResult);
            case ("85F8472D-8255-4E80-B34A-61DB8678135C"):
                var tataQuery = _mapper.Map<GetTATAQuoteQuery>(request);
                var tataResponse = await _mediatr.Send(tataQuery, cancellationToken);
                if (tataResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(tataResponse.Messages);
                    return NotFound(problemDetails);
                }
                var tataResult = Result.CreateSuccess(tataResponse.Result, (int)HttpStatusCode.OK);
                return Ok(tataResult);
            case ("5A97C9A3-1CFA-4052-8BA2-6294248EF1E9"):
                var orientalQuery = _mapper.Map<GetOrientalQuoteQuery>(request);
                var orientalResponse = await _mediatr.Send(orientalQuery, cancellationToken);
                if (orientalResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(orientalResponse.Messages);
                    return NotFound(problemDetails);
                }
                var orientalResult = Result.CreateSuccess(orientalResponse.Result, (int)HttpStatusCode.OK);
                return Ok(orientalResult);
            case ("DC874A12-6667-41AB-A7A1-3BB832B59CEB"):
                var unitedIndiaQuery = _mapper.Map<GetUnitedIndiaQuoteQuery>(request);
                var unitedIndiaResponse = await _mediatr.Send(unitedIndiaQuery, cancellationToken);
                if (unitedIndiaResponse.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError(unitedIndiaResponse.Messages);
                    return NotFound(problemDetails);
                }
                var unitedIndiaResult = Result.CreateSuccess(unitedIndiaResponse.Result, (int)HttpStatusCode.OK);
                return Ok(unitedIndiaResult);
            default:
                break;
        }
        return default;
    }


    /// <summary>
    /// SaveUpdateLead
    /// </summary>
    /// <param name="leadDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("SaveProposal/{quotetransactionid}/{vehiclenumber}/{variantid}/{isProposal}")]
    [ProducesResponseType(typeof(SaveUpdateLeadVmResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<SaveUpdateLeadVmResponse>> SaveProposal(string quotetransactionid, string vehiclenumber, string variantid, bool isProposal, [FromBody] JsonObject jobj, CancellationToken cancellationToken)
    {
        SaveUpdateLeadCommand leadDetails = new SaveUpdateLeadCommand()
        {
            QuoteTransactionID = quotetransactionid,
            RequestBody = jobj.ToJsonString(),
            VariantId = variantid,
            VehicleNumber = vehiclenumber,
            IsProposal = isProposal
        };

        var result = await _mediatr.Send(leadDetails, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Lead Data not Saved");
            return NotFound(problemDetails);
        }
        var responseBody = System.Text.Json.JsonSerializer.Deserialize(result.Result.ProposalRequestBody, typeof(object));
        SaveUpdateLeadVmResponse resultobject = new SaveUpdateLeadVmResponse()
        {
            QuoteTransactionId = result.Result.QuoteTransactionId,
            LeadID = result.Result.LeadID,
            ProposalRequestBody = responseBody
        };
        var res = Result.CreateSuccess(resultobject, (int)HttpStatusCode.OK);
        return Ok(res);
    }


    /// <summary>
    /// GetLeadDetails
    /// </summary>
    /// <param name="leadid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetProposalSummary/{insurerid}/{vehiclenumber}/{variantid}/{quotetransactionid}")]
    [ProducesResponseType(typeof(SaveUpdateLeadVmResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<SaveUpdateLeadVmResponse>> GetProposalSummary(string insurerid, string vehiclenumber, string variantid, string quotetransactionid, CancellationToken cancellationToken)
    {
        GetProposalSummaryQuery leadDetails = new GetProposalSummaryQuery()
        {
            InsurerId = insurerid,
            VehicleNumber = vehiclenumber,
            VariantId = variantid,
            QuoteTransactionId = quotetransactionid
        };
        var result = await _mediatr.Send(leadDetails, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Proposal Summary not found");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }


    /// <summary>
    /// GetProposalFields
    /// </summary>
    /// <param name="InsurerID"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("ProposalField/{InsurerID}/{quotetransactionId}")]
    [ProducesResponseType(typeof(CreateLeadModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<List<ProposalFieldVm>>> ProposalField(string InsurerID, string quotetransactionId, CancellationToken cancellationToken)
    {
        GetProposalFieldsQuery proposalFieldInsurer = new GetProposalFieldsQuery()
        {
            InsurerID = InsurerID,
            QuoteTransactionId = quotetransactionId
        };
        var result = await _mediatr.Send(proposalFieldInsurer, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Proposal Fields Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }



    /// <summary>
    /// GetPreviousCoverMaster
    /// </summary>
    /// <param name="InsurerID"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPreviousCoverMaster/{InsurerID}/{VehicalTypeId}/{PolicyTypeId}")]
    [ProducesResponseType(typeof(CreateLeadModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ProposalFieldVm>> GetPreviousCoverMaster(string InsurerID, string VehicalTypeId, string PolicyTypeId, CancellationToken cancellationToken)
    {
        GetPreviousCoverMasterQuery previousCoverMaster = new GetPreviousCoverMasterQuery()
        {
            InsurerID = InsurerID,
            VehicalTypeId = VehicalTypeId,
            PolicyTypeId = PolicyTypeId
        };
        var result = await _mediatr.Send(previousCoverMaster, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Previous Covers Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }



    /// <summary>
    /// Proposal
    /// </summary>
    /// <param name="insuranceType">InsuranceType: MOTOR/HEALTH/TERM</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpPost("CreateProposal/{insurerid}/{quotetransactionid}/{vehicletypeid}/{isSharePaymentLink}")]
    [ProducesResponseType(typeof(IEnumerable<QuoteResponseModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<QuoteResponseModel>> CreateProposal(string insurerid, string quotetransactionid, string vehicletypeid, bool isSharePaymentLink, CancellationToken cancellationToken)
    {
        var result = (dynamic)null;
        switch (insurerid)
        {
            case ("16413879-6316-4C1E-93A4-FF8318B14D37"):
                BajajProposalCommand bajajProposalCommand = new BajajProposalCommand()
                {
                    QuoteTransactionID = quotetransactionid,
                    InsurerId = insurerid,
                    IsSharePaymentLink = isSharePaymentLink
                };
                result = await _mediatr.Send(bajajProposalCommand, cancellationToken);
                break;
            case ("FD3677E5-7938-46C8-9CD2-FAE188A1782C"):
                ICICICreateProposalCommand iciciCreateProposalCommand = new ICICICreateProposalCommand()
                {
                    QuoteTransactionID = quotetransactionid,
                    InsurerId = insurerid,
                    VehicleTypeId = vehicletypeid,
                    IsSharePaymentLink = isSharePaymentLink
                };
                result = await _mediatr.Send(iciciCreateProposalCommand, cancellationToken);
                break;

            case ("78190CB2-B325-4764-9BD9-5B9806E99621"):
                GodigitCreateProposalCommand godigitCreateProposalCommand = new GodigitCreateProposalCommand()
                {
                    QuoteTransactionID = quotetransactionid,
                    InsurerId = insurerid,
                    IsSharePaymentLink = isSharePaymentLink
                };
                result = await _mediatr.Send(godigitCreateProposalCommand, cancellationToken);
                break;

            case ("77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA"):
                CholaCreateProposalCommand cholaCreateProposalCommand = new CholaCreateProposalCommand()
                {
                    QuoteTransactionID = quotetransactionid,
                    InsurerId = insurerid,
                    VehicleTypeId = vehicletypeid,
                    IsSharePaymentLink = isSharePaymentLink
                };
                result = await _mediatr.Send(cholaCreateProposalCommand, cancellationToken);
                break;
            case ("0A326B77-AFD5-44DA-9871-1742624CFF16"):
                HDFCCreateProposalCommand hdfcCreateProposalCommand = new HDFCCreateProposalCommand()
                {
                    QuoteTransactionID = quotetransactionid,
                    InsurerId = insurerid,
                    VehicleTypeId = vehicletypeid,
                    IsSharePaymentLink = isSharePaymentLink
                };
                result = await _mediatr.Send(hdfcCreateProposalCommand, cancellationToken);
                break;
            case ("372B076C-D9D9-48DC-9526-6EB3D525CAB6"):
                RelianceCreateProposalCommand relianceCreateProposalCommand = new RelianceCreateProposalCommand()
                {
                    QuoteTransactionID = quotetransactionid,
                    InsurerId = insurerid,
                    VehicleTypeId = vehicletypeid,
                    IsSharePaymentLink = isSharePaymentLink
                };
                result = await _mediatr.Send(relianceCreateProposalCommand, cancellationToken);
                break;
            case ("E656D5D1-5239-4E48-9048-228C67AE3AC3"):
                IFFCOCreateProposalCommand iFFCOCreateProposalCommand = new IFFCOCreateProposalCommand()
                {
                    QuoteTransactionID = quotetransactionid,
                    InsurerId = insurerid,
                    VehicleTypeId = vehicletypeid,
                    IsSharePaymentLink = isSharePaymentLink
                };
                result = await _mediatr.Send(iFFCOCreateProposalCommand, cancellationToken);
                break;
            case ("85F8472D-8255-4E80-B34A-61DB8678135C"):
                TATACreateProposalCommand tataCreateProposalCommand = new TATACreateProposalCommand()
                {
                    QuoteTransactionID = quotetransactionid,
                    InsurerId = insurerid,
                    VehicleTypeId = vehicletypeid,
                    IsSharePaymentLink = isSharePaymentLink
                };
                result = await _mediatr.Send(tataCreateProposalCommand, cancellationToken);
                break;
            case ("5A97C9A3-1CFA-4052-8BA2-6294248EF1E9"):
                OrientalCreateProposalCommand orientalCreateProposalCommand = new()
                {
                    QuoteTransactionID = quotetransactionid,
                    InsurerId = insurerid,
                    VehicleTypeId = vehicletypeid,
                    IsSharePaymentLink = isSharePaymentLink
                };
                result = await _mediatr.Send(orientalCreateProposalCommand, cancellationToken);
                break;
            case ("DC874A12-6667-41AB-A7A1-3BB832B59CEB"):
                UIICCreateProposalCommand uIICCreateProposalCommand = new()
                {
                    QuoteTransactionID = quotetransactionid,
                    InsurerId = insurerid,
                    VehicleTypeId = vehicletypeid,
                    IsSharePaymentLink = isSharePaymentLink
                };
                result = await _mediatr.Send(uIICCreateProposalCommand, cancellationToken);
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

        var policyDateQuery = new GetPolicyDatesQuery();

        if (quoteConfirmCommand?.PreviousPolicy is not null)
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
        _ = await _mediatr.Send(insertQuoteRequest, cancellationToken);
        var result = (dynamic)null;
        switch (insurerId)
        {
            case ("16413879-6316-4C1E-93A4-FF8318B14D37"):
                var bajajQuoteConfirmCommand = _mapper.Map<BajajQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(bajajQuoteConfirmCommand, cancellationToken);
                break;
            case ("FD3677E5-7938-46C8-9CD2-FAE188A1782C"):
                var iciciQuoteConfirmCommand = _mapper.Map<ICICIQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(iciciQuoteConfirmCommand, cancellationToken);
                break;
            case ("78190CB2-B325-4764-9BD9-5B9806E99621"):
                var godigitQuoteConfirmCommand = _mapper.Map<GodigitQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(godigitQuoteConfirmCommand, cancellationToken);
                break;
            case ("0A326B77-AFD5-44DA-9871-1742624CFF16"):
                var hdfcQuoteConfirmCommand = _mapper.Map<HDFCQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(hdfcQuoteConfirmCommand, cancellationToken);
                break;
            case ("77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA"):
                var cholaQuoteConfirmCommand = _mapper.Map<CholaQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(cholaQuoteConfirmCommand, cancellationToken);
                break;
            case ("372B076C-D9D9-48DC-9526-6EB3D525CAB6"):
                var relianceQuoteConfirmCommand = _mapper.Map<RelianceQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(relianceQuoteConfirmCommand, cancellationToken);
                break;
            case ("E656D5D1-5239-4E48-9048-228C67AE3AC3"):
                var iffcoQuoteConfirmCommand = _mapper.Map<IffcoQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(iffcoQuoteConfirmCommand, cancellationToken);
                break;
            case ("85F8472D-8255-4E80-B34A-61DB8678135C"):
                var tataQuoteConfirmCommand = _mapper.Map<TATAQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(tataQuoteConfirmCommand, cancellationToken);
                break;
            case ("5A97C9A3-1CFA-4052-8BA2-6294248EF1E9"):
                var orientalConfirmCommand = _mapper.Map<OrientalQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(orientalConfirmCommand, cancellationToken);
                break;
            case ("DC874A12-6667-41AB-A7A1-3BB832B59CEB"):
                var unitedIndiaConfirmCommand = _mapper.Map<UnitedIndiaQuoteConfirmCommand>(quoteConfirmCommand);
                result = await _mediatr.Send(unitedIndiaConfirmCommand, cancellationToken);
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

    /// <summary>
    /// GetCKYCFields
    /// </summary>
    /// <param name="InsurerID"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("CKYCField/{insurerID}/{isPOI}/{isCompany}/{isDocumentUpload}")]
    [ProducesResponseType(typeof(CKYCFieldVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<List<ProposalFieldMasterModel>>> CKYCField(string insurerID, bool isPOI, bool isCompany, bool isDocumentUpload, CancellationToken cancellationToken)
    {
        GetCKYCFieldsQuery cKYCFieldInsurer = new GetCKYCFieldsQuery()
        {
            InsurerID = insurerID,
            IsPOI = isPOI,
            IsCompany = isCompany,
            IsDocumentUpload = isDocumentUpload
        };
        var result = await _mediatr.Send(cKYCFieldInsurer, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("CKYC Fields Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    /// <summary>
    /// SaveCKYC
    /// </summary>
    /// <param name="quotetransactionId"></param>
    /// <param name="vehicleNumber"></param>
    /// <param name="variantId"></param>
    /// <param name="jobj"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("SaveCKYC/{insurerId}/{quotetransactionId}")]
    [ProducesResponseType(typeof(SaveCKYCResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<SaveCKYCResponse>> SaveCKYC(string insurerId, string quotetransactionId, [FromBody] JsonObject jobj, CancellationToken cancellationToken)
    {
        var result = (dynamic)null;

        switch (insurerId)
        {
            case "16413879-6316-4C1E-93A4-FF8318B14D37":
                BajajCKYCCommand bajajCKYCCommand = JsonConvert.DeserializeObject<BajajCKYCCommand>(jobj.ToString());

                bajajCKYCCommand.QuoteTransactionId = quotetransactionId;
                bajajCKYCCommand.InsurerId = insurerId;

                result = await _mediatr.Send(bajajCKYCCommand, cancellationToken);
                break;

            case ("FD3677E5-7938-46C8-9CD2-FAE188A1782C"):
                ICICICKYCCommand iCICICKYCCommand = JsonConvert.DeserializeObject<ICICICKYCCommand>(jobj.ToString());

                iCICICKYCCommand.QuoteTransactionId = quotetransactionId;
                iCICICKYCCommand.InsurerId = insurerId;

                result = await _mediatr.Send(iCICICKYCCommand, cancellationToken);
                break;

            case ("78190CB2-B325-4764-9BD9-5B9806E99621"):
                GoDigitCKYCCommand goDigitCKYCCommand = JsonConvert.DeserializeObject<GoDigitCKYCCommand>(jobj.ToString());
                goDigitCKYCCommand.QuoteTransactionId = quotetransactionId;
                goDigitCKYCCommand.InsurerId = insurerId;
                result = await _mediatr.Send(goDigitCKYCCommand, cancellationToken);
                break;

            case ("77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA"):
                CholaCKYCCommand cholaCKYCCommand = JsonConvert.DeserializeObject<CholaCKYCCommand>(jobj.ToString());

                cholaCKYCCommand.QuoteTransactionId = quotetransactionId;
                cholaCKYCCommand.InsurerId = insurerId;

                result = await _mediatr.Send(cholaCKYCCommand, cancellationToken);
                break;
            case ("0A326B77-AFD5-44DA-9871-1742624CFF16"):
                HDFCCKYCCommand hdfcCKYCCommand = JsonConvert.DeserializeObject<HDFCCKYCCommand>(jobj.ToString());
                hdfcCKYCCommand.QuoteTransactionId = quotetransactionId;
                hdfcCKYCCommand.InsurerId = insurerId;
                result = await _mediatr.Send(hdfcCKYCCommand, cancellationToken);
                break;
            case ("E656D5D1-5239-4E48-9048-228C67AE3AC3"):
                IFFCOCKYCCommand iffcoCKYCCommand = JsonConvert.DeserializeObject<IFFCOCKYCCommand>(jobj.ToString());
                iffcoCKYCCommand.QuoteTransactionId = quotetransactionId;
                iffcoCKYCCommand.InsurerId = insurerId;
                result = await _mediatr.Send(iffcoCKYCCommand, cancellationToken);
                break;
            case ("372B076C-D9D9-48DC-9526-6EB3D525CAB6"):
                RelianceCKYCCommand relianceCKYCCommand = JsonConvert.DeserializeObject<RelianceCKYCCommand>(jobj.ToString());
                relianceCKYCCommand.QuoteTransactionId = quotetransactionId;
                relianceCKYCCommand.InsurerId = insurerId;
                result = await _mediatr.Send(relianceCKYCCommand, cancellationToken);
                break;
            //case ("5A97C9A3-1CFA-4052-8BA2-6294248EF1E9"):
            //    OrientalCKYCCommand orientalCKYCCommand = JsonConvert.DeserializeObject<OrientalCKYCCommand>(jobj.ToString());
            //    orientalCKYCCommand.QuoteTransactionId = quotetransactionId;
            //    orientalCKYCCommand.InsurerId= insurerId;
            //    result = await _mediatr.Send(orientalCKYCCommand, cancellationToken);
            //    break;

            case ("DC874A12-6667-41AB-A7A1-3BB832B59CEB"):
                UnitedIndiaCkycCommand unitedIndiaCommand = JsonConvert.DeserializeObject<UnitedIndiaCkycCommand>(jobj.ToString());
                unitedIndiaCommand.QuoteTransactionId = quotetransactionId;
                unitedIndiaCommand.InsurerId = insurerId;
                result = await _mediatr.Send(unitedIndiaCommand, cancellationToken);
                break;
        }

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Save CKYC Failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    [HttpPut("UploadCKYCDocument/{insurerId}/{quotetransactionId}")]
    [ProducesResponseType(typeof(UploadCKYCDocumentResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<UploadCKYCDocumentResponse>> UploadCKYCDocument(string insurerId, string quotetransactionId, [FromBody] JsonObject jobj, CancellationToken cancellationToken)
    {
        var result = (dynamic)null;

        switch (insurerId)
        {
            case "16413879-6316-4C1E-93A4-FF8318B14D37":
                UploadBajajCKYCDocumentCommand uploadBajajCKYCDocument = JsonConvert.DeserializeObject<UploadBajajCKYCDocumentCommand>(jobj.ToString());

                uploadBajajCKYCDocument.QuoteTransactionId = quotetransactionId;
                uploadBajajCKYCDocument.InsurerId = insurerId;

                result = await _mediatr.Send(uploadBajajCKYCDocument, cancellationToken);
                break;

            case ("FD3677E5-7938-46C8-9CD2-FAE188A1782C"):
                UploadICICICKYCDocumentCommand iCICICKYCCommand = JsonConvert.DeserializeObject<UploadICICICKYCDocumentCommand>(jobj.ToString());

                iCICICKYCCommand.QuoteTransactionId = quotetransactionId;
                iCICICKYCCommand.InsurerId = insurerId;

                result = await _mediatr.Send(iCICICKYCCommand, cancellationToken);
                break;
            case ("E656D5D1-5239-4E48-9048-228C67AE3AC3"):
                CreateIFFCOCKYCCommand createCkyc = JsonConvert.DeserializeObject<CreateIFFCOCKYCCommand>(jobj.ToString());

                createCkyc.QuoteTransactionId = quotetransactionId;
                createCkyc.InsurerId = insurerId;

                result = await _mediatr.Send(createCkyc, cancellationToken);
                break;
        }

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Save CKYC Document Failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// CKYCDocumentFields
    /// </summary>
    /// <param name="insurerId"></param>
    /// <param name="documentName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("CKYCDocumentField/{insurerId}/{documentName}")]
    [ProducesResponseType(typeof(ProposalFieldMasterModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<List<ProposalFieldMasterModel>>> CKYCDocumentFields(string insurerId, string documentName, CancellationToken cancellationToken)
    {
        GetCKYCDocumentFieldCommand cKYCFieldInsurer = new GetCKYCDocumentFieldCommand()
        {
            InsurerId = insurerId,
            DocumentName = documentName
        };
        var result = await _mediatr.Send(cKYCFieldInsurer, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("CKYC Fields Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }


    /// <summary>
    /// GetLeadDetails
    /// </summary>
    /// <param name="leadid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPaymentStatus/{insurerid}/{quotetransactionid}")]
    [ProducesResponseType(typeof(PaymentDetailsVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PaymentDetailsVm>> GetPaymentStatus(string insurerid, string quotetransactionid, CancellationToken cancellationToken)
    {
        GetPaymentStatusQuery getPaymentStatusQuery = new GetPaymentStatusQuery()
        {
            InsurerId = insurerid,
            QuoteTransactionId = quotetransactionid
        };
        var result = await _mediatr.Send(getPaymentStatusQuery, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Payment status not found");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// GetLeadDetails
    /// </summary>
    /// <param name="leadid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPolicyDocument/{insurerid}/{quotetransactionid}")]
    [ProducesResponseType(typeof(PaymentDetailsVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<GetPolicyDocumentPdfModel>> GetPolicyDocument(string insurerid, string quotetransactionid, CancellationToken cancellationToken)
    {
        GetPolicyDocumentPdfQuery getPolicyDocumentPDFQuery = new GetPolicyDocumentPdfQuery()
        {
            InsurerId = insurerid,
            QuoteTransactionId = quotetransactionid
        };
        var result = await _mediatr.Send(getPolicyDocumentPDFQuery, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    [HttpGet("GetLeadDetails")]
    [ProducesResponseType(typeof(GetLeadDetailsVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<GetLeadDetailsVm>> GetLeadDetails([FromQuery] string leadId, [FromQuery] string stageId, CancellationToken cancellationToken)
    {
        GetLeadDetailsQuery getPolicyDocumentPDFQuery = new GetLeadDetailsQuery()
        {
            LeadId = leadId,
            StageId = stageId
        };
        var result = await _mediatr.Send(getPolicyDocumentPDFQuery, cancellationToken);
        if (result.Failed)
        {
            var failedResult = Result.CreateNotFoundError("Data Not Found");
            return NotFound(failedResult);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// GetCKYCStatus
    /// </summary>
    /// <param name="insurerID"></param>
    /// <param name="quoteTransactionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetCKYCStatus/{insurerID}/{quoteTransactionId}")]
    [ProducesResponseType(typeof(CKYCStatusVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<CKYCStatusModel>> GetCKYCStatus(string insurerID, string quoteTransactionId, CancellationToken cancellationToken)
    {
        GetCKYCStatusQuery cKYCStatus = new GetCKYCStatusQuery()
        {
            InsurerID = insurerID,
            QuoteTransactionId = quoteTransactionId
        };
        var result = await _mediatr.Send(cKYCStatus, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("CKYC Status Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// GetLeadDetails
    /// </summary>
    /// <param name="Payment Link Genrate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPaymentLink/{insurerId}/{quoteTransactionId}")]
    [ProducesResponseType(typeof(PaymentDetailsVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> GetPaymentLink(string insurerId, string quoteTransactionId, CancellationToken cancellationToken)
    {
        var result = (dynamic)null;
        switch (insurerId)
        {
            case "78190CB2-B325-4764-9BD9-5B9806E99621":
                GetGoDigitPaymentLinkQuery getGoDigitPaymentLinkQuery = new GetGoDigitPaymentLinkQuery()
                {
                    InsurerId = insurerId,
                    QuoteTransactionId = quoteTransactionId
                };
                result = await _mediatr.Send(getGoDigitPaymentLinkQuery, cancellationToken);
                break;
            case "16413879-6316-4C1E-93A4-FF8318B14D37":
                GetBajajPaymentLinkQuery getBajajPaymentLinkQuery = new GetBajajPaymentLinkQuery()
                {
                    InsurerId = insurerId,
                    QuoteTransactionId = quoteTransactionId
                };
                result = await _mediatr.Send(getBajajPaymentLinkQuery, cancellationToken);
                break;

            case ("FD3677E5-7938-46C8-9CD2-FAE188A1782C"):
                GetICICIPaymentLinkQuery getICICIPaymentLinkQuery = new GetICICIPaymentLinkQuery()
                {
                    InsurerId = insurerId,
                    QuoteTransactionId = quoteTransactionId
                };
                result = await _mediatr.Send(getICICIPaymentLinkQuery, cancellationToken);
                break;
            case ("77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA"):
                GetCholaPaymentLinkQuery getCholaPaymentLinkQuery = new GetCholaPaymentLinkQuery()
                {
                    InsurerId = insurerId,
                    QuoteTransactionId = quoteTransactionId
                };
                result = await _mediatr.Send(getCholaPaymentLinkQuery, cancellationToken);
                break;
            case ("0A326B77-AFD5-44DA-9871-1742624CFF16"):
                GetHDFCPaymentLinkQuery getHDFCPaymentLinkQuery = new GetHDFCPaymentLinkQuery()
                {
                    InsurerId = insurerId,
                    QuoteTransactionId = quoteTransactionId
                };
                result = await _mediatr.Send(getHDFCPaymentLinkQuery, cancellationToken);
                break;
            case ("372B076C-D9D9-48DC-9526-6EB3D525CAB6"):
                GetReliancePaymentLinkQuery getreliancePaymentLinkQuery = new GetReliancePaymentLinkQuery()
                {
                    InsurerId = insurerId,
                    QuoteTransactionId = quoteTransactionId
                };
                result = await _mediatr.Send(getreliancePaymentLinkQuery, cancellationToken);
                break;
            case ("E656D5D1-5239-4E48-9048-228C67AE3AC3"):
                GetIFFCOPaymentLinkQuery getIFFCOPaymentLinkQuery = new GetIFFCOPaymentLinkQuery()
                {
                    InsurerId = insurerId,
                    QuoteTransactionId = quoteTransactionId
                };
                result = await _mediatr.Send(getIFFCOPaymentLinkQuery, cancellationToken);
                break;
            case ("85F8472D-8255-4E80-B34A-61DB8678135C"):
                TATAGetPaymentLinkQuery getTATAPaymentLinkQuery = new TATAGetPaymentLinkQuery()
                {
                    InsurerId = insurerId,
                    QuoteTransactionId = quoteTransactionId
                };
                result = await _mediatr.Send(getTATAPaymentLinkQuery, cancellationToken);
                break;
            case ("DC874A12-6667-41AB-A7A1-3BB832B59CEB"):
                UnitedIndiaGetPaymentLinkQuery unitedIndiaGetPaymentLinkQuery = new UnitedIndiaGetPaymentLinkQuery()
                {
                    InsurerId = insurerId,
                    QuoteTransactionId = quoteTransactionId
                };
                result = await _mediatr.Send(unitedIndiaGetPaymentLinkQuery, cancellationToken);
                break;
        }
        if (result.Failed)
        {
            var problemDetails = Result.CreateValidationError(result.Messages);
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Messages, (int)HttpStatusCode.OK);
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

        var policyDateQuery = new GetPolicyDatesQuery();

        if (quoteConfirmCommand?.PreviousPolicy is not null)
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

    [HttpGet("GetProposalDetails/{insurerId}/{quoteTransactionId}")]
    [ProducesResponseType(typeof(GetProposalDetailsForPaymentVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<GetProposalDetailsForPaymentVm>> GetProposalDetails(string insurerId, string quoteTransactionId, CancellationToken cancellationToken)
    {
        GetProposalDetailsQuery getproposalDetails = new GetProposalDetailsQuery()
        {
            InsurerId = insurerId,
            QuoteTransactionId = quoteTransactionId
        };

        var result = await _mediatr.Send(getproposalDetails, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateValidationError(result.Messages);
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result, (int)HttpStatusCode.OK);

        return Ok(res);
    }

    [HttpGet("GetUserId/{insurerId}/{proposalNumber}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> GetUserId(string insurerId, string proposalNumber, CancellationToken cancellationToken)
    {
        GetUserIdDetailsQuery getUserIdDetailsQuery = new GetUserIdDetailsQuery()
        {
            InsurerId = insurerId,
            ProposalNumber = proposalNumber
        };

        var result = await _mediatr.Send(getUserIdDetailsQuery, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateValidationError(result.Messages);
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Messages, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// POIAfterProposal
    /// </summary>
    /// <param name="quotetransactionId"></param>
    /// <param name="vehicleNumber"></param>
    /// <param name="variantId"></param>
    /// <param name="jobj"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("POIAfterProposal/{insurerId}/{quotetransactionId}/{isPOI}")]
    [ProducesResponseType(typeof(SaveCKYCResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<SaveCKYCResponse>> POIAfterProposal(string insurerId, string quotetransactionId, bool isPOI, bool isCompany, [FromBody] JsonObject jobj, CancellationToken cancellationToken)
    {
        var result = (dynamic)null;

        switch (insurerId)
        {

            case ("85F8472D-8255-4E80-B34A-61DB8678135C"):
                TATACKYCCommand tataCKYCCommand = JsonConvert.DeserializeObject<TATACKYCCommand>(jobj.ToString());
                tataCKYCCommand.QuoteTransactionId = quotetransactionId;
                tataCKYCCommand.IsPOI = isPOI;
                tataCKYCCommand.IsCompany = isCompany;
                result = await _mediatr.Send(tataCKYCCommand, cancellationToken);
                break;

        }

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Save CKYC Failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// DocUploadfterProposal
    /// </summary>
    /// <param name="insurerId"></param>
    /// <param name="quotetransactionId"></param>
    /// <param name="jobj"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("DocUploadfterProposal/{insurerId}/{quotetransactionId}")]
    [ProducesResponseType(typeof(SaveCKYCResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<SaveCKYCResponse>> DocUploadfterProposal(string insurerId, string quotetransactionId, [FromBody] JsonObject jobj, CancellationToken cancellationToken)
    {
        var result = (dynamic)null;
        switch (insurerId)
        {
            case ("85F8472D-8255-4E80-B34A-61DB8678135C"):
                TataDocumentUploadPospProposalCommand tataCKYCCommand = JsonConvert.DeserializeObject<TataDocumentUploadPospProposalCommand>(jobj.ToString());
                tataCKYCCommand.QuoteTransactionId = quotetransactionId;
                result = await _mediatr.Send(tataCKYCCommand, cancellationToken);
                break;
            case ("5A97C9A3-1CFA-4052-8BA2-6294248EF1E9"):
                OrientalCKYCCommand orientalCKYCCommand = JsonConvert.DeserializeObject<OrientalCKYCCommand>(jobj.ToString());
                orientalCKYCCommand.QuoteTransactionId = quotetransactionId;
                result = await _mediatr.Send(orientalCKYCCommand, cancellationToken);
                break;
        }

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Document Upload After Proposal failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

}
