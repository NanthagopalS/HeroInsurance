using Insurance.API.Models;
using Insurance.Core.Features.InsuranceMaster;
using Insurance.Core.Features.InsuranceMaster.Command.LeadDetails;
using Insurance.Core.Features.InsuranceMaster.Queries.GetCity;
using Insurance.Core.Features.InsuranceMaster.Queries.GetFinancier;
using Insurance.Core.Features.InsuranceMaster.Queries.GetInsuranceMaster;
using Insurance.Core.Features.InsuranceMaster.Queries.GetQuoteMaster;
using Insurance.Core.Features.InsuranceMaster.Queries.GetStateCity;
using Insurance.Core.Features.InsuranceMaster.Queries.GetVehicleDetails;
using Insurance.Core.Helpers;
using Insurance.Domain.InsuranceMaster;
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
public class InsuranceMasterController : ControllerBase
{
    private readonly IMediator _mediatr;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public InsuranceMasterController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    /// <summary>
    /// GetInsuraneType
    /// </summary>
    /// <param name="insuranceType">InsuranceType: MOTOR/HEALTH/TERM</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetInsuranceType/{insuranceType}")]
    [ProducesResponseType(typeof(IEnumerable<InsuranceTypeVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<InsuranceTypeVm>>> GetInsuranceType(string insuranceType, CancellationToken cancellationToken)
    {
        var req = new GetInsuranceTypeQuery { InsuranceType = insuranceType };
        var result = await _mediatr.Send(req, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages.SingleOrDefault());
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }


    /// <summary>
    /// Get GetMakeModelFuel
    /// </summary>
    /// <param name="vehicleType"></param>
    /// <param name="CVCategoryId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetMakeModel")]
    [ProducesResponseType(typeof(IEnumerable<MakeModelVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<MakeModelVm>>> GetMakeModel([FromQuery] GetMakeModelQuery req , CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Make Model not found");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Get GetMakeModelFuel
    /// </summary>
    ///  <param name="vehicleType">VehicleType: 2W/4W/CV</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetFuelByModel/{modelId}")]
    [ProducesResponseType(typeof(IEnumerable<FuelVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<FuelVm>>> GetFuelByModel(string modelId, CancellationToken cancellationToken)
    {
        var req = new GetFuelByModelQuery { ModelId = modelId };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Fuel not found");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Get GetVariant
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="fuelId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetVariant/{modelId}/{fuelId}")]
    [ProducesResponseType(typeof(IEnumerable<VariantVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<VariantVm>>> GetVariant(string modelId, string fuelId, CancellationToken cancellationToken)
    {
        var req = new GetVariantQuery()
        {
            Model_Id = modelId,
            Fuel_Id = fuelId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Variant not found");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Get GetRTO
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetRTO")]
    [ProducesResponseType(typeof(StateCityRTOYearVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<StateCityRTOYearVm>> GetRTO(CancellationToken cancellationToken)
    {
        var req = new GetRTOQuery();
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("RTO not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Get GetInsurer
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetInsurer/{isCommercial:bool?}")]
    [ProducesResponseType(typeof(IEnumerable<InsurerVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<InsurerVm>>> GetInsurer(bool? isCommercial, CancellationToken cancellationToken)
    {
        var req = new GetInsurerQuery();
        req.IsCommercial = isCommercial;

        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Insurer not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Get NCB
    /// </summary>
    /// <param name="policyExpiryDate"></param>
    /// <param name="isPreviousPolicy"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPreviousPolicyType/{regDate}/{isBrandNew}/{vehcileTypeId}")]
    [ProducesResponseType(typeof(IEnumerable<PreviousPolicyTypeVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<PreviousPolicyTypeVm>>> GetPreviousPolicyType(string regDate, bool isBrandNew, string vehcileTypeId, CancellationToken cancellationToken)
    {
        var req = new GetPreviousPolicyTypeQuery
        {
            RegDate = regDate,
            IsBrandNew = isBrandNew,
            VehicleTypeId = vehcileTypeId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Previous policy type not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Get NCB
    /// </summary>
    /// <param name="policyExpiryDate"></param>
    /// <param name="isPreviousPolicy"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetNCB/{policyExpiryDate}/{isPreviousPolicy}")]
    [ProducesResponseType(typeof(IEnumerable<NCBVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<NCBVm>>> GetNCB(string policyExpiryDate, bool isPreviousPolicy, CancellationToken cancellationToken)
    {
        var req = new GetNCBQuery
        {
            PolicyExpiryDate = policyExpiryDate,
            IsPreviousPolicy = isPreviousPolicy
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("NCB not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// GetInsuraneType
    /// </summary>
    /// <param name="insuranceType">InsuranceType: MOTOR/HEALTH/TERM</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetVehicleDetails/{vehicleNumber}/{vehicleTypeId}")]
    [ProducesResponseType(typeof(VehicleDetailVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<VehicleDetailVm>> GetVehicleDetails(string vehicleNumber, string vehicleTypeId,bool IsQuote, CancellationToken cancellationToken)
    {
        var req = new GetVehicleDetailsQuery { VehicleNumber = vehicleNumber, VehicleTypeId = vehicleTypeId, IsQuote = IsQuote };
        var result = await _mediatr.Send(req, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            if (problemDetails?.Data?.Detail.Replace(" ", "") == "vehicleismorethan15yearsold")
            {
                problemDetails = Result.CreateValidationError(result.Messages);
                problemDetails.Data.Detail = "The vehicle is more than 15 years old.";
                return BadRequest(problemDetails);

            }
            else
            {
                problemDetails = Result.CreateNotFoundError("Oh! Sorry didn't found your car details");
            }
            //var problemDetails = Result.CreateNotFoundError("Oh! Sorry didn't found your car details");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Create Lead
    /// </summary>
    /// <param name="createLeadCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("CreateLead")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<CreateLeadVm>>> CreateLead(CreateLeadCommand createLeadCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(createLeadCommand, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Create Lead failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    [HttpGet("GetQuoteMaster/{vehicleTypeId}/{policytypeid}")]
    [ProducesResponseType(typeof(VehicleDetailVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<QuoteMasterVm>> GetQuote(string vehicleTypeId, string policytypeid, CancellationToken cancellationToken)
    {
        var req = new GetQuoteMasterQuery
        {
            VehicleTypeId = vehicleTypeId,
            PolicyTypeId = policytypeid
        };
        var result = await _mediatr.Send(req, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Get Quote Failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Get City
    /// </summary>
    /// <param name="policyExpiryDate"></param>
    /// <param name="isPreviousPolicy"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetCity")]
    [ProducesResponseType(typeof(IEnumerable<CitysVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<CitysVm>>> GetCity(CancellationToken cancellationToken)
    {
        var req = new GetCityQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("City not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    /// <summary>
    /// Get Financier
    /// </summary>
    /// <param name="policyExpiryDate"></param>
    /// <param name="isPreviousPolicy"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetFinancier")]
    [ProducesResponseType(typeof(IEnumerable<FinancierVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<FinancierVm>>> GetFinancier(CancellationToken cancellationToken)
    {
        var req = new GetFinancierQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Financier not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    [HttpGet("GetStateCity/{insurerId}/{pincode}/{state}")]
    [ProducesResponseType(typeof(IEnumerable<MasterCityModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<MasterCityModel>> GetStateCity(string insurerId, string pincode, string state, CancellationToken cancellationToken)
    {
        var req = new GeStateCityQuery
        {
            InsurerId = insurerId,
            Pincode = pincode,
            State = state,
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    [HttpGet("GetInsurerMasterData")]
    [ProducesResponseType(typeof(IEnumerable<GetInsuranceMasterVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetInsuranceMasterVm>>> GetInsuranceMaster(CancellationToken cancellationToken)
    {
        var req = new GetInsuranceMasterQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Financier not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

}
