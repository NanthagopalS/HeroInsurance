using Identity.Core.Features.User.Commands.BUDetails;
using Identity.Core.Features.User.Commands.BUInsert;
using Identity.Core.Features.User.Commands.BUUpdate;
using Identity.Core.Features.User.Commands.CategoryInsert;
using Identity.Core.Features.User.Commands.RoleModulePermission;
using Identity.Core.Features.User.Commands.RoleModuleUpdatePermission;
using Identity.Core.Features.User.Commands.UserCreation;
using Identity.Core.Features.User.Commands.UserEmailId;
using Identity.Core.Features.User.Commands.UserMappingInsert;
using Identity.Core.Features.User.Commands.UserRoleGetAllMapping;
using Identity.Core.Features.User.Commands.UserRoleGetMapping;
using Identity.Core.Features.User.Commands.UserRoleMapping;
using Identity.Core.Features.User.Commands.UserRoleSearch;
using Identity.Core.Features.User.Commands.UserRoleUpdateModel;
using Identity.Core.Features.User.Queries.GetModel;
using Identity.Core.Features.User.Queries.GetRoleBULevel;
using Identity.Core.Features.User.Queries.GetRoleLevel;
using Identity.Core.Features.User.Queries.GetRolePermission;
using Identity.Core.Features.User.Queries.GetRolePermissionAll;
using Identity.Core.Features.User.Queries.GetRoleType;
using Identity.Core.Features.User.Queries.PanVerificationDetails;
using Identity.Core.Features.User.Queries.UserDocument;
using Identity.Core.Features.User.Querries.GetMasterType;
using Identity.Domain.Roles;
using Identity.Domain.User;
using Identity.Domain.UserCreation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net;
using ThirdPartyUtilities.Helpers;
using Identity.Core.Features.User.Queries.GetAllRelationshipManager;
using Identity.Core.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Identity.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class RolesController : ControllerBase
    {
        private readonly IMediator _mediatr;
        public RolesController(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        #region - GET Methods -

        #region - Get Role Type -
        [HttpGet]
        [Route("GetRoleType")]
        [ProducesResponseType(typeof(RoleTypeVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<RoleTypeVm>>> GetRoleType(CancellationToken cancellationToken)
        {


            var result = await _mediatr.Send(new GetRoleTypeQuery(), cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get Model -
        [HttpGet]
        [Route("GetModel")]
        [ProducesResponseType(typeof(ModelVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ModelVm>>> GetModel(CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new GetModelQuery(), cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get Permission Mapping All - 
        [HttpGet]
        [Route("GetPermissionMappingAll")]
        [ProducesResponseType(typeof(RoleSearchVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<RoleSearchAllVM>>> GetPermissionMappingAll(CancellationToken cancellationToken)
        {

            var result = await _mediatr.Send(new GetRoleQueryAllCommand(), cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get Role BU Level - 
        [HttpGet]
        [Route("GetRoleBULevel")]
        [ProducesResponseType(typeof(BULevelVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<BULevelVm>>> GetRoleBULevel(CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new GetRoleBULevelCommand(), cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get Permission Mapping - 
        [HttpGet]
        [Route("GetPermissionMapping")]
        [ProducesResponseType(typeof(RoleSearchVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        // public async Task<ActionResult<IEnumerable<RoleSearchVM>>> GetPermissionMapping([FromBody] string RoleName, string RoleTypeName, string CreatedFrom, string CreatedTo, CancellationToken cancellationToken)
        public async Task<ActionResult<IEnumerable<RoleSearchVM>>> GetPermissionMapping(string RoleTitleName, string RoleTypeName, string CreatedFrom, string CreatedTo, CancellationToken cancellationToken)
        {
            GetRoleQueryCommand requestRole = new GetRoleQueryCommand();
            requestRole.RoleTitleName = RoleTitleName;
            requestRole.RoleTypeName = RoleTypeName;
            // request1.RoleName = RoleName;
            requestRole.CreatedFrom = CreatedFrom;
            requestRole.CreatedTo = CreatedTo;

            var result = await _mediatr.Send(requestRole, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetUser Role Mapping - 
        [HttpGet]
        [Route("GetUserRoleMapping")]
        [ProducesResponseType(typeof(UserRoleVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserRoleVM>>> GetUserRoleMapping(string empID, string roleTypeName, string isActive, string createdFrom, string CreatedTo, CancellationToken cancellationToken)
        {

            var req = new UserRoleMappingGetCommand
            {
                EMPID = empID,
                RoleTypeName = roleTypeName,
                isActive = isActive,
                CreatedFrom = createdFrom,
                CreatedTo = CreatedTo

            };
            var result = await _mediatr.Send(req, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get User Role Mapping All - 
        [HttpGet]
        [Route("GetUserRoleMappingAll")]
        [ProducesResponseType(typeof(UserRoleGetAllVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserRoleGetAllVM>>> GetUserRoleMappingAll(CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new UserRoleMappingGetAllCommand(), cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get Role Level Details -
        [HttpGet]
        [Route("GetRoleLevelDetails")]
        [ProducesResponseType(typeof(RoleLevelVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<RoleLevelVM>>> GetRoleLevelDetails(CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new GetRoleLevelCommand(), cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion


        #region -GetAllRelationshipManager-
        [HttpGet]
        [Route("GetAllRelationshipManager")]
        [ProducesResponseType(typeof(GetAllRelationshipManagerVM),(int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]

        public async Task<ActionResult<IEnumerable<GetAllRelationshipManagerVM>>> GetAllRelationshipManagerResponseModel(string? UserId, CancellationToken cancellationToken)
        {
            var req = new GetAllRelationshipManagerQueries()
            {
              UserId = UserId,
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }

        #endregion

        #endregion

        #region - POST Methods -

        #region - InsertRoleModulePermission -
        [HttpPost]
        [Route("InsertRoleModulePermission")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<string>> InsertRoleModulePermission(RoleModulePermissionCommand rolePermissionCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(rolePermissionCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Inserting RoleModulePermission failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get BU Details -
        [HttpPost]
        [Route("GetBUDetails")]
        [ProducesResponseType(typeof(BUDetailsVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<BUDetailsVM>>> GetBUDetails(BUDetailsCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(request, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - BU Insert Details -
        [HttpPost]
        [Route("BUInsertDetails")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> BUInsertDetails(BUInsertCommand buInsertCommand, CancellationToken cancellationToken)
        {
            var insertBU = await _mediatr.Send(buInsertCommand, cancellationToken);

            if (!insertBU.Result)
            {
                var problemDetails = Result.CreateNotFoundError("Inserting BU failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(insertBU.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Category Insert Details -
        [HttpPost]
        [Route("CategoryInsertDetails")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<string>> CategoryInsertDetails(CategoryInsertCommand catInsertCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new UserRoleMappingGetAllCommand(), cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - UserRoleMappingInsert - 
        [HttpPost]
        [Route("UserRoleMappingInsert")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UserRoleMappingInsert(UserRoleMappingInsertCommand userInsertCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(userInsertCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Inserting UserRoleMapping failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - User Role Mapping Update -
        [HttpPost]
        [Route("UserRoleMappingUpdate")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UserRoleMappingUpdate(UserRoleMappingUpdateCommand userInsertCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(userInsertCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Updating UserRoleMapping failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #endregion

        #region - PUT Methods -

        #region - Update Role Module Permission - 

        [HttpPut]
        [Route("UpdateRoleModulePermission")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<string>> UpdateRoleModulePermission(UpdateRoleModulePermissionCommand rolePermissionCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(rolePermissionCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Inserting RoleModulePermission failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - BU Update Details -
        [HttpPut]
        [Route("BUUpdateDetails")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> BUUpdateDetails(BUUpdateCommand buUpdateCommand, CancellationToken cancellationToken)
        {
            var UpdateBU = await _mediatr.Send(buUpdateCommand, cancellationToken);

            if (!UpdateBU.Result)
            {
                var problemDetails = Result.CreateNotFoundError("Update BU failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(UpdateBU.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #endregion


    }
}

