using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.BUInsert;
using Admin.Core.Features.User.Commands.BUUpdate;
using Admin.Core.Features.User.Commands.CategoryInsert;
using Admin.Core.Features.User.Commands.DeleteExamManagementDetail;
using Admin.Core.Features.User.Commands.DeletePoliciesDetail;
using Admin.Core.Features.User.Commands.DeleteTrainingManagementDetail;
using Admin.Core.Features.User.Commands.DownloadAgreement;
using Admin.Core.Features.User.Commands.EditNotification;
using Admin.Core.Features.User.Commands.InsertBUDetail;
using Admin.Core.Features.User.Commands.InsertLeadDetails;
using Admin.Core.Features.User.Commands.InsertNotification;
using Admin.Core.Features.User.Commands.InsertStampData;
using Admin.Core.Features.User.Commands.InsertStampInstruction;
using Admin.Core.Features.User.Commands.InsertUserRoleMappingDetaill;
using Admin.Core.Features.User.Commands.ResetAdminUserAccountDetail;
using Admin.Core.Features.User.Commands.RoleDetailInsert;
using Admin.Core.Features.User.Commands.RoleModulePermission;
using Admin.Core.Features.User.Commands.RoleModuleUpdatePermission;
using Admin.Core.Features.User.Commands.UpdateActivateDeActivateBU;
using Admin.Core.Features.User.Commands.UpdateActivateDeActivateExamManagement;
using Admin.Core.Features.User.Commands.UpdateActivateDeActivateRole;
using Admin.Core.Features.User.Commands.UpdateActivateDeActivateTrainingManagement;
using Admin.Core.Features.User.Commands.UpdateActivePOSPAccountDetail;
using Admin.Core.Features.User.Commands.UpdateAgreementStatusByUserId;
using Admin.Core.Features.User.Commands.UpdateBUDetail;
using Admin.Core.Features.User.Commands.UpdateBUStatus;
using Admin.Core.Features.User.Commands.UpdateDocumentStatus;
using Admin.Core.Features.User.Commands.UpdateParticularPOSPDetailForIIBDashboard;
using Admin.Core.Features.User.Commands.UpdatePersonalDetails;
using Admin.Core.Features.User.Commands.UserRoleGetAllMapping;
using Admin.Core.Features.User.Commands.UserRoleGetMapping;
using Admin.Core.Features.User.Commands.UserRoleSearch;
using Admin.Core.Features.User.Commands.UserRoleUpdateModel;
using Admin.Core.Features.User.Queries.CheckForRole;
using Admin.Core.Features.User.Queries.CheckUserExistOrNot;
using Admin.Core.Features.User.Queries.GetAgreementManagementDetail;
using Admin.Core.Features.User.Queries.GetAllBUDetail;
using Admin.Core.Features.User.Queries.GetAllBUDetailsByUserIDQuery;
using Admin.Core.Features.User.Queries.GetAllBUUser;
using Admin.Core.Features.User.Queries.GetAllExamManagementDetail;
using Admin.Core.Features.User.Queries.GetAllPOSPDetailForIIBDashboard;
using Admin.Core.Features.User.Queries.GetAllSharedReportingRole;
using Admin.Core.Features.User.Queries.GetAllTrainingManagementDetails;
using Admin.Core.Features.User.Queries.GetAllUserRoleMappingDetailModel;
using Admin.Core.Features.User.Queries.GetAssistedBUDetails;
using Admin.Core.Features.User.Queries.GetBuDetail;
using Admin.Core.Features.User.Queries.GetBUHeadDetail;
using Admin.Core.Features.User.Queries.GetBUHierarchy;
using Admin.Core.Features.User.Queries.GetCardsDetail;
using Admin.Core.Features.User.Queries.GetCustomersDetail;
using Admin.Core.Features.User.Queries.GetDeactivatedUser;
using Admin.Core.Features.User.Queries.GetFunnelChart;
using Admin.Core.Features.User.Queries.GetInActivePOSPDetail;
using Admin.Core.Features.User.Queries.GetInsuranceType;
using Admin.Core.Features.User.Queries.GetLeadManagementDetail;
using Admin.Core.Features.User.Queries.GetLeadOverview;
using Admin.Core.Features.User.Queries.GetLeadStage;
using Admin.Core.Features.User.Queries.GetModel;
using Admin.Core.Features.User.Queries.GetNotificationList;
using Admin.Core.Features.User.Queries.GetParticularBUDetail;
using Admin.Core.Features.User.Queries.GetParticularLeadDetail;
using Admin.Core.Features.User.Queries.GetParticularLeadUploadedDocument;
using Admin.Core.Features.User.Queries.GetParticularPOSPDetailForIIBDashboard;
using Admin.Core.Features.User.Queries.GetParticularUserRoleMappingDetail;
using Admin.Core.Features.User.Queries.GetPoliciesDetail;
using Admin.Core.Features.User.Queries.GetPOSPOnboardingDetail;
using Admin.Core.Features.User.Queries.GetPOSPStageDetail;
using Admin.Core.Features.User.Queries.GetProductCategory;
using Admin.Core.Features.User.Queries.GetRecipientList;
using Admin.Core.Features.User.Queries.GetRelationshipManager;
using Admin.Core.Features.User.Queries.GetRenewalDetail;
using Admin.Core.Features.User.Queries.GetRoleBULevel;
using Admin.Core.Features.User.Queries.GetRoleLevel;
using Admin.Core.Features.User.Queries.GetRolePermission;
using Admin.Core.Features.User.Queries.GetRolePermissionAll;
using Admin.Core.Features.User.Queries.GetRoleType;
using Admin.Core.Features.User.Queries.GetSalesOverview;
using Admin.Core.Features.User.Queries.GetTotalSalesDetail;
using Admin.Core.Features.User.Queries.GetUserByBUId;
using Admin.Core.Features.User.Queries.GetUserCategory;
using Admin.Core.Responses;
using Admin.Domain.Roles;
using Admin.Domain.User;
using ClosedXML.Excel;
using CsvHelper;
using ExcelDataReader;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using System.Net;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Admin.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class RolesController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly IMongoDBService _mongodbService;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="mediatr"></param>
        /// <param name="mongodbService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RolesController(IMediator mediatr, IMongoDBService mongodbService)
        {
            _mediatr = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
            _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        }

        #region - GET Methods -

        #region - GetRoleType -
        /// <summary>
        /// Get Role Types
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRoleType")]
        [ApiExplorerSettings(IgnoreApi = true)]
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

        #region - GetModuleDetail -
        /// <summary>
        /// Get Model
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetModuleDetail")]
        [ProducesResponseType(typeof(ModelVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ModelVm>>> GetModel(string? moduleGroupName, CancellationToken cancellationToken)
        {
            var req = new GetModelQuery
            {
                ModuleGroupName = moduleGroupName
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

        #region - GetPermissionMapping -
        /// <summary>
        /// Get Permission Mapping
        /// </summary>
        /// <param name="RoleTitleName"></param>
        /// <param name="RoleTypeName"></param>
        /// <param name="CreatedFrom"></param>
        /// <param name="CreatedTo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPermissionMapping")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(RoleSearchVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
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

        #region - GetPermissionMappingAll -
        /// <summary>
        /// Get Permission mapping for all
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPermissionMappingAll")]
        [ApiExplorerSettings(IgnoreApi = true)]
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

        #region - GetRoleBULevel -
        /// <summary>
        /// Get Role at BU level
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRoleBULevel")]
        [ApiExplorerSettings(IgnoreApi = true)]
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

        #region - GetUserRoleMapping -
        /// <summary>
        /// Get user role mapping
        /// </summary>
        /// <param name="empID"></param>
        /// <param name="roleTypeName"></param>
        /// <param name="isActive"></param>
        /// <param name="createdFrom"></param>
        /// <param name="CreatedTo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserRoleMapping")]
        [ApiExplorerSettings(IgnoreApi = true)]
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

        #region - GetUserRoleMappingAll -
        /// <summary>
        /// Get all user role mapping 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserRoleMappingAll")]
        [ApiExplorerSettings(IgnoreApi = true)]
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

        #region - GetRoleLevelDetails -
        /// <summary>
        /// Get role level details
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        #region - GetRoleDetail -
        /// <summary>
        /// Get role details
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="roleTypeId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRoleDetail")]
        [ProducesResponseType(typeof(RoleDetailVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<RoleDetailVM>>> GetRoleDetail(string? roleName, string? roleTypeId, string? startDate, string? endDate, int? pageIndex, int? pageSize, bool? isActive, CancellationToken cancellationToken)
        {
            var req = new GetRoleDetail
            {
                RoleName = roleName,
                RoleTypeId = roleTypeId,
                StartDate = startDate,
                EndDate = endDate,
                CurrentPageIndex = pageIndex == null ? 1 : pageIndex,
                CurrentPageSize = pageSize == null ? 10 : pageSize,
                IsActive = isActive
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

        #region - GetRoleTypeDetail -
        /// <summary>
        /// Get role type details
        /// </summary>
        /// <param name="empID"></param>
        /// <param name="roleTypeName"></param>
        /// <param name="isActive"></param>
        /// <param name="createdFrom"></param>
        /// <param name="CreatedTo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRoleTypeDetail")]
        [ProducesResponseType(typeof(RoleDetailVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<RoleDetailVM>>> GetRoleTypeDetail(CancellationToken cancellationToken)
        {
            var req = new GetRoleTypeQuery();
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

        #region - GetAllBUDetails -
        /// <summary>
        /// Get role details
        /// </summary>
        /// <param name="buName"></param>
        /// <param name="roleTypeId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetAllBUDetails")]
        [ProducesResponseType(typeof(GetAllBUDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAllBUDetailVm>>> GetAllBuDetail(GetAllBUDetailQuery objBUDetails, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(objBUDetails, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }

        /// <summary>
        /// GetAllUserRoleMappingDetail
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllUserRoleMappingDetail")]
        [ProducesResponseType(typeof(GetAllUserRoleMappingDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAllUserRoleMappingDetailVm>>> GetAllUserRoleMappingDetail(string? EmployeeIdorName, string? RoleTypeId, bool? StatusId, string? StartDate, string? EndDate, int? PageIndex, int? PageSize, CancellationToken cancellationToken)
        {
            var req = new GetAllUserRoleMappingDetailQuery
            {
                EmployeeIdorName = EmployeeIdorName,
                RoleTypeId = RoleTypeId,
                StatusId = StatusId,
                StartDate = StartDate,
                EndDate = EndDate,
                CurrentPageIndex = PageIndex == null ? 1 : PageIndex,
                CurrentPageSize = PageSize == null ? 10 : PageSize
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

        #region - GetBUDetails -
        /// <summary>
        /// Get BU details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBUDetails")]
        [ProducesResponseType(typeof(GetBUDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetBUDetailVm>>> GetBUDetail(string? RoleTypeId, CancellationToken cancellationToken)
        {
            var req = new GetBUDetailQuery
            {
                RoleTypeId = RoleTypeId
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("BU Detail not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }

        #endregion

        #region - Get Particular Role Details -
        /// <summary>
        /// Get Particular Role Details
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="roleTypeId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetParticularRoleDetail")]
        [ProducesResponseType(typeof(ParticularRoleDetailVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ParticularRoleDetailVM>>> GetParticularRoleDetail(string roleId, CancellationToken cancellationToken)
        {
            var req = new GetParticularRoleDetail
            {
                RoleTypeId = roleId
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

        #region - Get Particular Role Details -
        /// <summary>
        /// Get Particular User Role Mapping detail API
        /// </summary>        
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetParticularUserRoleMappingDetail")]
        [ProducesResponseType(typeof(GetParticularUserRoleMappingDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetParticularUserRoleMappingDetailVm>>> GetParticularUserRoleMappingDetail(string UserRoleMappingId, CancellationToken cancellationToken)
        {
            var req = new GetParticularUserRoleMappingDetailQuery
            {
                UserRoleMappingId = UserRoleMappingId
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

        /// <summary>
        /// GetProductCategory
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProductCategory")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(GetProductCategoryVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetProductCategoryVm>>> GetProductCategory(CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new GetProductCategoryQuery(), cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get Hierarchy Management Detail -
        /// <summary>
        /// Get Hierarchy Management Detail
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="roleTypeId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetHierarchyManagementDetail")]
        [ProducesResponseType(typeof(HierarchyManagementDetailVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<HierarchyManagementDetailVM>> GetHierarchyManagementDetail(string roleId, string roleTypeId, string? parentUserId, string? parentUserRoleId, CancellationToken cancellationToken)
        {
            var req = new GetHierarchyManagementDetail
            {
                RoleTypeId = roleTypeId,
                RoleId = roleId,
                ParentUserId = parentUserId,
                ParentUserRoleId = parentUserRoleId
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

        #region - GetParticularBUDetail -
        /// <summary>
        /// Get Particular BU Detail
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetParticularBUDetail")]
        [ProducesResponseType(typeof(GetParticularBUDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetParticularBUDetailVm>>> GetParticularBUDetail(string BUId, CancellationToken cancellationToken)
        {
            var req = new GetParticularBUDetailQuery
            {
                BUId = BUId
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

        #region - GetAllPOSPDetailForIIBDashboard -
        /// <summary>
        /// Get All POSP Detail For IIB Dashboard
        /// </summary>
        /// <param name="searchtext"></param>
        /// <param name="createdby"></param>
        /// <param name="statustype"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllPOSPDetailForIIBDashboard")]
        [ProducesResponseType(typeof(GetAllPOSPDetailForIIBDashboardVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAllPOSPDetailForIIBDashboardVm>>> GetAllPOSPDetailForIIBDashboard(string? searchtext, string? createdby, string? statustype, string? startDate, string? endDate, int? pageIndex, int? pageSize, CancellationToken cancellationToken)
        {
            var req = new GetAllPOSPDetailForIIBDashboardQuery
            {
                Searchtext = searchtext,
                CreatedBy = createdby,
                StatusType = statustype,
                StartDate = startDate,
                EndDate = endDate,
                PageIndex = pageIndex == null ? 1 : pageIndex,
                PageSize = pageSize == null ? 10 : pageSize
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

        #region - GetUserByBUId -
        /// <summary>
        /// Get User by BUId
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="BUId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserByBUId")]
        [ProducesResponseType(typeof(GetUserByBUIdVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetUserByBUIdVm>>> GetUserByBUId(string BUId, string searchtext, CancellationToken cancellationToken)
        {
            var req = new GetUserByBUIdQuery
            {
                BUId = BUId,
                SearchText = searchtext
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
        #endregion


        #region - GetPOSPManagementDetail -
        /// <summary>
        /// Get POSP Management Detail
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="stageId"></param>
        /// <param name="relationManagerId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="pospStatus"></param>
        /// <param name="isExportExcel"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPOSPManagementDetail")]
        [ProducesResponseType(typeof(POSPManagementDetailVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<POSPManagementDetailVM>>> GetPOSPManagementDetail(string? searchText, string? stageId, string? relationManagerId, int? pageIndex, int? pageSize, string? createdBy, CancellationToken cancellationToken, int pospStatus, bool isExportExcel = false)
        {
            if (isExportExcel == true)
            {
                pageIndex = -1;
            }
            var req = new GetPOSPManagementDetail
            {
                SearchText = !string.IsNullOrWhiteSpace(searchText) ? searchText : string.Empty,
                POSPStatus = pospStatus,
                StageId = !string.IsNullOrWhiteSpace(stageId) ? stageId : string.Empty,
                RelationManagerId = !string.IsNullOrWhiteSpace(relationManagerId) ? relationManagerId : string.Empty,
                PageIndex = pageIndex == null ? 1 : pageIndex,
                PageSize = pageSize == null ? 10 : pageSize,
                CreatedBy = createdBy
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (isExportExcel == true && result != null && result.Result != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var pospIDList = new List<string>();
                var pospNameList = new List<string>();
                var mobileNumberList = new List<string>();
                var createdByList = new List<string>();
                var stageList = new List<string>();
                var statusList = new List<string>();
                var emailIdList = new List<string>();
                var roleManagerList = new List<string>();
                var taggedPolicyList = new List<string>();
                var workbook = new XLWorkbook();
                workbook.AddWorksheet("NewPOSP");
                var ws = workbook.Worksheet("NewPOSP");
                ws.ColumnWidth = 24;
                int row = 1;
                foreach (var pospDetail in result.Result.POSPManagementDataModel)
                {
                    pospIDList.Add(!string.IsNullOrWhiteSpace(pospDetail.POSPId) ? Convert.ToString(pospDetail.POSPId) : string.Empty);
                    pospNameList.Add(!string.IsNullOrWhiteSpace(pospDetail.POSPName) ? Convert.ToString(pospDetail.POSPName) : string.Empty);
                    mobileNumberList.Add(!string.IsNullOrWhiteSpace(pospDetail.MobileNumber) ? Convert.ToString(pospDetail.MobileNumber) : string.Empty);
                    emailIdList.Add(!string.IsNullOrWhiteSpace(pospDetail.EmailId) ? Convert.ToString(pospDetail.EmailId) : string.Empty);
                    roleManagerList.Add(!string.IsNullOrWhiteSpace(pospDetail.RelationManager) ? Convert.ToString(pospDetail.RelationManager) : string.Empty);
                    taggedPolicyList.Add(!string.IsNullOrWhiteSpace(pospDetail.TaggedPolicy) ? Convert.ToString(pospDetail.TaggedPolicy) : string.Empty);
                    createdByList.Add(!string.IsNullOrWhiteSpace(pospDetail.CreatedBy) ? Convert.ToString(pospDetail.CreatedBy) : string.Empty);
                    stageList.Add(!string.IsNullOrWhiteSpace(pospDetail.StageValue) ? Convert.ToString(pospDetail.StageValue) : string.Empty);
                    statusList.Add(!string.IsNullOrWhiteSpace(pospDetail.StatusValue) ? Convert.ToString(pospDetail.StatusValue) : string.Empty);
                }
                var cell4th = pospStatus == 1 ? emailIdList : (pospStatus == 2 ? emailIdList : createdByList);
                var cell5th = pospStatus == 1 ? roleManagerList : (pospStatus == 2 ? roleManagerList : stageList);
                var cell6th = pospStatus == 1 ? taggedPolicyList : (pospStatus == 2 ? taggedPolicyList : statusList);
                ws.Cell(row, 1).InsertTable(pospIDList);
                ws.Cell(row, 2).InsertTable(pospNameList);
                ws.Cell(row, 3).InsertTable(mobileNumberList);
                ws.Cell(row, 4).InsertTable(cell4th);
                ws.Cell(row, 5).InsertTable(cell5th);
                ws.Cell(row, 6).InsertTable(cell6th);
                // Set Column Header Value : Code Start
                var forthColumnLabel = pospStatus == 1 ? "Email ID" : (pospStatus == 2 ? "Email ID" : "Created By");
                var fifthColumnLabel = pospStatus == 1 ? "Rel Manager" : (pospStatus == 2 ? "Rel Manager" : "Stage");
                var sixthColumnLabel = pospStatus == 1 ? "Tagged Policy" : (pospStatus == 2 ? "Tagged Policy" : "Status");
                ws.Cell(row, 1).SetValue("POSP ID");
                ws.Cell(row, 2).SetValue("POSP Name").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(row, 3).SetValue("Mobile Number").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(row, 4).SetValue(forthColumnLabel).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(row, 5).SetValue(fifthColumnLabel).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(row, 6).SetValue(sixthColumnLabel).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                // Code End For Set Header Value
                workbook.SaveAs(Path.Combine(path, "POSPManagement.xlsx"), new ClosedXML.Excel.SaveOptions());
                // CSV 
                //var csvPath = Path.Combine(path, $"POSPManagement-{DateTime.Now.ToFileTime()}.csv");
                var csvPath = Path.Combine(path, $"POSPManagement.csv");
                using (var streamWriter = new StreamWriter(csvPath))
                {
                    using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                    {
                        if (pospStatus == 1)
                        {
                            var listWithoutCol = result.Result.POSPManagementDataModel.Select(x => new { x.POSPId, x.POSPName, x.MobileNumber, x.EmailId, x.RelationManager, x.TaggedPolicy }).ToList();
                            csvWriter.WriteRecords(listWithoutCol);
                        }
                        else
                        {
                            var listWithoutCol = result.Result.POSPManagementDataModel.Select(x => new { x.POSPId, x.POSPName, x.MobileNumber, x.CreatedBy, x.StageValue, x.StatusValue }).ToList();
                            csvWriter.WriteRecords(listWithoutCol);
                        }
                    }
                }
            }
            if (result != null && result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result?.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetInActivePOSPDetail -
        /// <summary>
        /// Get role details
        /// </summary>
        /// <param name="CriteriaType"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetInActivePOSPDetail")]
        [ProducesResponseType(typeof(GetInActivePOSPDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetInActivePOSPDetailVm>>> GetInActivePOSPDetail(string? CriteriaType, string? FromDate, string? ToDate, int? pageIndex, CancellationToken cancellationToken)
        {
            var req = new GetInActivePOSPDetailQuery
            {
                CriteriaType = CriteriaType,
                FromDate = FromDate,
                ToDate = ToDate,
                PageIndex = pageIndex == null ? 1 : pageIndex
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
        #region - DownloadPOSPManagementDetailExcel -
        /// <summary>
        /// Download POSP Management Detail Excel File
        /// </summary>
        [HttpGet]
        [Route("DownloadPOSPManagementDetailExcel")]
        public FileResult DownloadPOSPManagementDetailExcel(bool isCsv = false)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Storage/POSPManagement");
            var fileExtension = isCsv == true ? ".csv" : ".xlsx";
            var fileType = isCsv == true ? "text/csv" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(path + fileExtension), fileType)
            {
                FileDownloadName = $"POSPManagement-{DateTime.Now.ToFileTime()}"
            };
            return result;
        }
        #endregion

        #endregion

        #region - GetParticularPOSPDetailForIIBDashboard -
        /// <summary>
        /// Get User by BUId
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="BUId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetParticularPOSPDetailForIIBDashboard")]
        [ProducesResponseType(typeof(GetParticularPOSPDetailForIIBDashboardVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetParticularPOSPDetailForIIBDashboardVm>>> GetParticularPOSPDetailForIIBDashboard(string? userId, CancellationToken cancellationToken)
        {
            var req = new GetParticularPOSPDetailForIIBDashboardQuery
            {
                UserId = userId
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

        #region - GetLeadOverview -
        /// <summary>
        /// GetLeadOverview
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetLeadOverview")]
        [ProducesResponseType(typeof(GetLeadOverviewVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetLeadOverviewVm>>> GetLeadOverview(string? LeadType, string? UserId, string? StartDate, string? EndDate, CancellationToken cancellationToken)
        {
            var req = new GetLeadOverviewQuery
            {
                LeadType = LeadType,
                UserId = UserId,
                StartDate = StartDate,
                EndDate = EndDate
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

        #region - GetFunnelChart -
        /// <summary>
        /// GetFunnelChart
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetFunnelChart")]
        [ProducesResponseType(typeof(GetFunnelChartVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetFunnelChartVm>>> GetFunnelChart(string? StartDate, string? EndDate, string? UserId, CancellationToken cancellationToken)
        {
            var req = new GetFunnelChartQuery
            {

                StartDate = StartDate,
                EndDate = EndDate,
                UserId = UserId
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

        #region - GetSalesOverview -
        /// <summary>
        /// Get sales overview
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSalesOverview")]
        [ProducesResponseType(typeof(GetSalesOverviewVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetSalesOverviewVm>>> GetSalesOverview(string? UserId, string? startDate, string? endDate, CancellationToken cancellationToken)
        {
            var req = new GetSalesOverviewQuery
            {
                UserId = UserId,
                StartDate = startDate,
                EndDate = endDate
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

        #region - GetLeadManagementDetail -
        /// <summary>
        /// Get Lead Management Detail details
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetLeadManagementDetail")]
        [ProducesResponseType(typeof(GetLeadManagementDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetLeadManagementDetailVm>>> GetLeadManagementDetail(GetLeadManagementDetailQuery getLeadManagementDetailQuery, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(getLeadManagementDetailQuery, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetRenewalDetail -
        /// <summary>
        /// GetRenewalDetail
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRenewalDetail")]
        [ProducesResponseType(typeof(GetRenewalDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetRenewalDetailVm>>> GetRenewalDetail(string? POSPId, string? PolicyNo, string? CustomerName, string? PolicyType, string? StartDate, string? EndDate, int PageIndex, CancellationToken cancellationToken)
        {
            var req = new GetRenewalDetailQuery
            {
                POSPId = POSPId,
                PolicyNo = PolicyNo,
                CustomerName = CustomerName,
                PolicyType = PolicyType,
                StartDate = StartDate,
                EndDate = EndDate,
                PageIndex = PageIndex == null ? 1 : PageIndex

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

        #region - GetPoliciesDetail -
        /// <summary>
        /// GetPoliciesDetail
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPoliciesDetail")]
        [ProducesResponseType(typeof(GetPoliciesDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetPoliciesDetailVm>>> GetPoliciesDetail(string? POSPId, string? PolicyNo, string? CustomerName, string? Mobile, string? PolicyMode, string? PolicyType, string? StartDate, string? EndDate, int PageIndex, CancellationToken cancellationToken)
        {
            var req = new GetPoliciesDetailQuery
            {
                POSPId = POSPId,
                PolicyNo = PolicyNo,
                CustomerName = CustomerName,
                Mobile = Mobile,
                PolicyMode = PolicyMode,
                PolicyType = PolicyType,
                StartDate = StartDate,
                EndDate = EndDate,
                PageIndex = PageIndex == null ? 1 : PageIndex

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

        #region - GetAllExamManagementDetail -
        /// <summary>
        /// Get All Exam Management Detail details
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllExamManagementDetail")]
        [ProducesResponseType(typeof(GetAllExamManagementDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAllExamManagementDetailVm>>> GetAllExamManagementDetail(string? Searchtext, string? Category, string? StartDate, string? EndDate, int? PageIndex, int? pageSize, CancellationToken cancellationToken)
        {
            var req = new GetAllExamManagementDetailQuery
            {
                Searchtext = Searchtext,
                Category = Category,
                StartDate = StartDate,
                EndDate = EndDate,
                PageIndex = PageIndex == null ? 1 : PageIndex,
                PageSize = pageSize == null ? 10 : pageSize
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

        #region - GetAgreementManagementDetail -
        /// <summary>
        /// Get Agreement Management Detail details
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAgreementManagementDetail")]
        [ProducesResponseType(typeof(GetAgreementManagementDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAgreementManagementDetailVm>>> GetAgreementManagementDetail(string? searchText, string? statusId, string? StartDate, string? EndDate, int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            var req = new GetAgreementManagementDetailQuery
            {
                SearchText = searchText,
                StatusId = statusId,
                StartDate = StartDate,
                EndDate = EndDate,
                PageIndex = pageIndex == 0 ? 1 : pageIndex,
                PageSize = pageSize == 0 ? 10 : pageSize

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

        #region - Get All Training Management Details -
        /// <summary>
        /// Get All POSP Detail For IIB Dashboard
        /// </summary>
        /// <param name="searchtext"></param>
        /// <param name="category"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllTrainingManagementDetails")]
        [ProducesResponseType(typeof(GetAllTrainingManagementDetailsVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAllTrainingManagementDetailsVm>>> GetAllTrainingManagementDetails(string? searchtext, string? category, string? startDate, string? endDate, int? pageIndex, int? pageSize, CancellationToken cancellationToken)
        {
            var req = new GetAllTrainingManagementDetailsQuery
            {
                Searchtext = searchtext,
                Category = category,
                StartDate = startDate,
                EndDate = endDate,
                PageIndex = pageIndex == null ? 1 : pageIndex,
                PageSize = pageSize == null ? 10 : pageSize
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

        #region - Get BU Head Detail -
        /// <summary>
        /// Get BU Head Detail
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserList")]
        [ProducesResponseType(typeof(GetUserListVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetUserListVm>> GetUserList(string? RoleId, CancellationToken cancellationToken)
        {
            var req = new GetUserListQuery()
            {
                RoleId = RoleId
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

        #region - Get BU Hirerachy Detail -
        /// <summary>
        /// Get BU Hirerachy Detail
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBUHierarchy")]
        [ProducesResponseType(typeof(GetBUHierarchyVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetUserListVm>> GetBUHierarchy(CancellationToken cancellationToken)
        {
            var req = new GetBUHierarchyQuery();
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

        #region - Get POSP Onboarded Detail -
        /// <summary>
        /// Get POSP Onboarded Detail
        /// </summary>
        /// <param name="POSPId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPOSPOnboardingDetail")]
        [ProducesResponseType(typeof(GetPOSPOnboardingDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetPOSPOnboardingDetailVm>> GetPOSPOnboardingDetail(CancellationToken cancellationToken)
        {
            var req = new GetPOSPOnboardingDetailQuery();
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

        #region - Get Particular Lead Uploaded Document -
        /// <summary>
        /// Get Particular Lead Uploaded Document
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetParticularLeadUploadedDocument")]
        [ProducesResponseType(typeof(GetParticularLeadUploadedDocumentVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetParticularLeadUploadedDocumentVm>> GetParticularLeadUploadedDocument(string? UserId, CancellationToken cancellationToken)
        {
            var req = new GetParticularLeadUploadedDocumentQuery()
            {
                UserId = UserId
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError("No document uploaded yet.");
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get POSP Document By ID -
        /// <summary>
        /// Get POSP Document By ID
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPOSPDocumentByID")]
        [ProducesResponseType(typeof(GetPOSPDocumentByIdVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetPOSPDocumentByIdVm>> GetPOSPDocumentByID(string? documentId, CancellationToken cancellationToken)
        {
            var req = new GetPOSPDocumentByIdQuery()
            {
                DocumentId = documentId
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

        #region - Get Role Level By BUId for Role Creation -
        /// <summary>
        /// Get Role Level By BUId for Role Creation
        /// </summary>
        /// <param name="bUId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRoleLevelByBUIdforRoleCreation")]
        [ProducesResponseType(typeof(RoleDetailVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetRoleLevelByBUIdforRoleCreationVM>>> GetRoleLevelByBUIdforRoleCreation(string? buId, CancellationToken cancellationToken)
        {
            var req = new GetRoleLevelByBUIdforRoleCreation
            {
                BUId = buId
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

        #region - Get Stamp Count For Agreement Management -
        /// <summary>
        /// Get Stamp Count For Agreement Management 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStampCountForAgreementManagement")]
        [ProducesResponseType(typeof(StampCountVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<StampCountVm>> GetStampCountForAgreementManagement(CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new GetStampCountQuery(), cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get Particular Lead Detail -
        /// <summary>
        /// Get Particular Lead Detail
        /// </summary>
        /// <param name="POSPId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetParticularLeadDetail")]
        [ProducesResponseType(typeof(GetParticularLeadDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetParticularLeadDetailVm>> GetParticularLeadDetail(string? LeadId, CancellationToken cancellationToken)
        {
            var req = new GetParticularLeadDetailQuery()
            {
                LeadId = LeadId
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

        #region - CheckUserExistOrNot -
        /// <summary>
        /// CheckUserExistOrNot
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckUserExistOrNot")]
        [ProducesResponseType(typeof(GetLeadOverviewVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<CheckUserExistOrNotVm>>> CheckUserExistOrNot(string? UserId, string? EmpId, string? MobileNo, string? EmailId, CancellationToken cancellationToken)
        {
            var req = new CheckUserExistOrNotQuery
            {
                UserId = UserId,
                EmpId = EmpId,
                MobileNo = MobileNo,
                EmailId = EmailId
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

        #region - GetPOSPStageDetail -
        /// <summary>
        /// GetPOSPStageDetail
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPOSPStageDetail")]
        [ProducesResponseType(typeof(GetPOSPStageDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetPOSPStageDetailVm>>> GetPOSPStageDetail(CancellationToken cancellationToken)
        {
            var req = new GetPOSPStageDetailQuery();

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


        #region - Get Exam Particular Question Detail -
        /// <summary>
        /// Get Exam Particular Question Detail
        /// </summary>        
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetExamParticularQuestionDetail")]
        [ProducesResponseType(typeof(GetExamParticularQuestionDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetExamParticularQuestionDetailVm>>> GetExamParticularQuestionDetail(string questionId, CancellationToken cancellationToken)
        {
            var req = new GetExamParticularQuestionDetailQuery
            {
                QuestionId = questionId
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

        #region - GetCustomersDetail -
        /// <summary>
        /// Get Customers details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCustomersDetail")]
        [ProducesResponseType(typeof(GetCustomersDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetCustomersDetailVm>>> GetCustomersDetail(string? CustomerType, string? SearchText, string? PolicyType, string? startDate, string? endDate, int? CurrentPageIndex, int? CurrentPageSize, CancellationToken cancellationToken)
        {
            var req = new GetCustomersDetailQuery
            {
                CustomerType = CustomerType,
                SearchText = SearchText,
                PolicyType = PolicyType,
                StartDate = startDate,
                EndDate = endDate,
                CurrentPageIndex = CurrentPageIndex == null ? 1 : CurrentPageIndex,
                CurrentPageSize = CurrentPageSize == null ? 10 : CurrentPageSize
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
        /// <summary>
        /// Insert Role Module Permission
        /// </summary>
        /// <param name="rolePermissionCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertRoleModulePermission")]
        // [ApiExplorerSettings(IgnoreApi = true)]
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

        #region - BUInsertDetails -
        /// <summary>
        /// BU Insert Details
        /// </summary>
        /// <param name="buInsertCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("BUInsertDetails")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> BUInsertDetail(BUInsertCommand buInsertCommand, CancellationToken cancellationToken)
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

        #region - CategoryInsertDetails -
        /// <summary>
        /// Category Insert Details
        /// </summary>
        /// <param name="catInsertCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CategoryInsertDetails")]
        [ApiExplorerSettings(IgnoreApi = true)]
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

        #region - InsertUserRoleMappingDetail -
        /// <summary>
        /// InsertUserRoleMappingDetail 
        /// </summary>
        /// <param name="insertUserRoleMappingDetaillCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertUserRoleMappingDetail")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> InsertUserRoleMappingDetail(IFormFile? files, [FromForm] InsertUserRoleMappingDetaillCommand insertUserRoleMappingDetaillCommand, CancellationToken cancellationToken)
        {
            if (files != null)
            {
                if (files.ContentType.Contains("jpeg") || files.ContentType.Contains("jpg") || files.ContentType.Contains("png") || files.ContentType.Contains("pdf"))
                {

                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Services/Files");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var fileNameWithPath = Path.Combine(path, files.FileName);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        files.CopyTo(stream);
                        var filesstream = stream;
                    }

                    byte[] byteimage = System.IO.File.ReadAllBytes(fileNameWithPath);

                    var fileSize = files.Length;
                    long fileSizeibMbs = fileSize / (1024 * 1024);

                    System.IO.File.Delete(fileNameWithPath);
                    var dname = Path.GetDirectoryName(fileNameWithPath);
                    Directory.Delete(dname, true);
                    insertUserRoleMappingDetaillCommand.ProfilePictureID = files.FileName;
                    insertUserRoleMappingDetaillCommand.ImageStream = byteimage;
                    var result = await _mediatr.Send(insertUserRoleMappingDetaillCommand, cancellationToken);
                    if (result.Failed)
                    {
                        var problemDetails = Result.CreateValidationError(new List<string> { "Invalid document" });
                        return BadRequest(problemDetails);
                    }
                    else
                    {
                        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
                        return Ok(res);
                    }
                }
                else
                {
                    var problemDetails = Result.CreateValidationError(new List<string> { "Invalid document" });
                    return BadRequest(problemDetails);
                }
            }
            else
            {
                insertUserRoleMappingDetaillCommand.ProfilePictureID = null;
                insertUserRoleMappingDetaillCommand.ImageStream = null;
                var result = await _mediatr.Send(insertUserRoleMappingDetaillCommand, cancellationToken);
                if (result.Failed)
                {
                    var problemDetails = Result.CreateValidationError(new List<string> { "Invalid document" });
                    return BadRequest(problemDetails);
                }
                else
                {
                    var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
                    return Ok(res);
                }
            }
        }


        #endregion

        #region - UserRoleMappingUpdate -
        /// <summary>
        /// Update user role mapping
        /// </summary>
        /// <param name="userInsertCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateUserRoleMappingDetail")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UserRoleMappingUpdate(IFormFile? files, [FromForm] UserRoleMappingUpdateCommand userRoleMappingUpdateCommand, CancellationToken cancellationToken)
        {
            if (userRoleMappingUpdateCommand.IsProfilePictureChange)
            {

                if (files.ContentType.Contains("jpeg") || files.ContentType.Contains("jpg") || files.ContentType.Contains("png") || files.ContentType.Contains("pdf"))
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Services/Files");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var fileNameWithPath = Path.Combine(path, files.FileName);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        files.CopyTo(stream);
                        var filesstream = stream;
                    }

                    byte[] byteimage = System.IO.File.ReadAllBytes(fileNameWithPath);

                    var fileSize = files.Length;
                    long fileSizeibMbs = fileSize / (1024 * 1024);

                    System.IO.File.Delete(fileNameWithPath);
                    var dname = Path.GetDirectoryName(fileNameWithPath);
                    Directory.Delete(dname, true);
                    userRoleMappingUpdateCommand.ProfilePictureID = files.FileName;
                    userRoleMappingUpdateCommand.ImageStream = byteimage;
                    var result = await _mediatr.Send(userRoleMappingUpdateCommand, cancellationToken);
                    if (result.Failed)
                    {
                        var problemDetails = Result.CreateValidationError(new List<string> { "Invalid document" });
                        return BadRequest(problemDetails);
                    }
                    else
                    {
                        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
                        return Ok(res);
                    }
                }
                else
                {
                    var problemDetails = Result.CreateValidationError(new List<string> { "Invalid document" });
                    return BadRequest(problemDetails);
                }

            }

            else
            {
                var insertRole = await _mediatr.Send(userRoleMappingUpdateCommand, cancellationToken);
                if (!insertRole.Result)
                {
                    var problemDetails = Result.CreateNotFoundError("Updating role Details failed");
                    return NotFound(problemDetails);
                }
                var res = Result.CreateSuccess(insertRole.Result, (int)HttpStatusCode.OK);
                return Ok(res);
            }

        }


        //var result = await _mediatr.Send(userRoleMappingUpdateCommand, cancellationToken);

        //if (result.Failed)
        //{
        //    var problemDetails = Result.CreateNotFoundError("Updating UserRoleMapping failed");
        //    return NotFound(problemDetails);
        //}

        //var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        //return Ok(res);

        #endregion

        #region - Insert Role Detail -
        /// <summary>
        /// Insert Role Detail
        /// </summary>
        /// <param name="roleDetailInsertCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertRoleDetail")]
        [ApiExplorerSettings(IgnoreApi = false)]
        [ProducesResponseType(typeof(RoleDetailInsertVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<RoleDetailInsertVm>> InsertRoleDetail(RoleDetailInsertCommand roleDetailInsertCommand, CancellationToken cancellationToken)
        {
            var insertRole = await _mediatr.Send(roleDetailInsertCommand, cancellationToken);
            if (insertRole.Failed)
            {
                var problemDetails = Result.CreateNotFoundError(insertRole.Messages);
                return BadRequest(problemDetails);
            }
            var res = Result.CreateSuccess(insertRole.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - InsertBUDetails -
        /// <summary>
        /// Insert BU details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertBUDetails")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> InsertBUDetails(InsertBUDetailCommand insertBUDetailCommand, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(insertBUDetailCommand.BUName) && string.IsNullOrWhiteSpace(insertBUDetailCommand.BUHeadId) && string.IsNullOrWhiteSpace(insertBUDetailCommand.HierarchyLevelId))
            {
                var problemDetails = Result.CreateNotFoundError("BUName, BUHeadId and HierarchyLevelId is required");
                return NotFound(problemDetails);
            }
            var result = await _mediatr.Send(insertBUDetailCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("BU Creation failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Insert Stamp Instruction -
        /// <summary>
        /// Insert Stamp Instruction
        /// </summary>
        /// <param name="files"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertStampInstruction")]
        [ApiExplorerSettings(IgnoreApi = false)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> InsertStampInstruction(IFormFile files, CancellationToken cancellationToken)
        {
            List<InsertStampInstructionModel> usersList = new List<InsertStampInstructionModel>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                files.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(0))) && !string.IsNullOrEmpty(Convert.ToString(reader.GetValue(1))))

                        {
                            usersList.Add(new InsertStampInstructionModel
                            {
                                //Id = reader.GetValue(1).ToString(),
                                SrNo = reader.GetValue(0).ToString(),
                                Instruction = reader.GetValue(1).ToString()
                            });
                        }
                    }
                    usersList.RemoveAt(0);
                }
                var result = new HeroResult<bool>();
                foreach (var user in usersList)
                {
                    var req = new InsertStampInstructionCommand
                    {
                        SrNo = user.SrNo,
                        Instruction = user.Instruction
                    };
                    result = await _mediatr.Send(req, cancellationToken);
                }
                if (result.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError("Data not found");
                    return NotFound(problemDetails);
                }
                var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
                return Ok(res);
            }
        }
        #endregion

        #region - Insert Stamp Data -
        /// <summary>
        /// Insert Stamp Data
        /// </summary>
        /// <param name="files"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertStampData")]
        [ApiExplorerSettings(IgnoreApi = false)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> InsertStampData(IFormFile files, CancellationToken cancellationToken)
        {
            List<InsertStampDataModel> usersList = new List<InsertStampDataModel>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                files.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(0))) && !string.IsNullOrEmpty(Convert.ToString(reader.GetValue(1))))

                        {
                            usersList.Add(new InsertStampDataModel
                            {
                                SrNo = reader.GetValue(0).ToString(),
                                StampData = reader.GetValue(1).ToString()
                            });
                        }
                    }
                    usersList.RemoveAt(0);
                }
                var result = new HeroResult<bool>();
                foreach (var user in usersList)
                {
                    var req = new InsertStampDataCommand
                    {
                        SrNo = user.SrNo,
                        StampData = user.StampData
                    };
                    result = await _mediatr.Send(req, cancellationToken);
                }
                if (result.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError("Data not found");
                    return NotFound(problemDetails);
                }
                var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
                return Ok(res);
            }
        }
        #endregion

        #region - Bulk Upload IIB Data -
        /// <summary>
        /// Bulk Upload IIB Data
        /// </summary>
        /// <param name="files"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("BulkUploadIIBData")]
        [ApiExplorerSettings(IgnoreApi = false)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> BulkUploadIIBData(IFormFile files, CancellationToken cancellationToken)
        {
            List<IIBBulkUploadDocument> usersList = new List<IIBBulkUploadDocument>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                files.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(1))) && !string.IsNullOrEmpty(Convert.ToString(reader.GetValue(11)))
                            && !string.IsNullOrEmpty(Convert.ToString(reader.GetValue(13))))
                        {
                            usersList.Add(new IIBBulkUploadDocument
                            {
                                UserId = reader.GetValue(1).ToString(),
                                IIBStatus = reader.GetValue(11).ToString(),
                                IIBUploadStatus = reader.GetValue(13).ToString()
                            });
                        }
                    }
                    usersList.RemoveAt(0);
                }
                var result = new HeroResult<bool>();
                foreach (var user in usersList)
                {
                    var req = new UpdateParticularPOSPDetailForIIBDashboardCommand
                    {
                        UserId = user.UserId,
                        IIBStatus = user.IIBStatus,
                        IIBUploadStatus = user.IIBUploadStatus
                    };
                    result = await _mediatr.Send(req, cancellationToken);
                }
                if (result.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError("Data not found");
                    return NotFound(problemDetails);
                }
                var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
                return Ok(res);
            }
        }
        #endregion

        #region - Update BU Status -
        /// <summary>
        /// UpdateBUStatus
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateBUStatus")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateBUStatus(string BUId, bool IsActive, CancellationToken cancellationToken)
        {
            var req = new UpdateBUStatusComand
            {
                BUId = BUId,
                IsActive = IsActive
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

        #region - Update Particular POSP Detail For IIB Dashboard -
        /// <summary>
        /// BU Insert Details
        /// </summary>
        /// <param name="buInsertCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateParticularPOSPDetailForIIBDashboard")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateParticularPOSPDetailForIIBDashboard(string? UserId, string? IIBStatus, string? IIBUploadStatus, CancellationToken cancellationToken)
        {
            var req = new UpdateParticularPOSPDetailForIIBDashboardCommand
            {
                UserId = UserId,
                IIBStatus = IIBStatus,
                IIBUploadStatus = IIBUploadStatus
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

        #region - UpdateBUDetails -
        /// <summary>
        /// Update BU details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateBUDetails")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateBUDetail(UpdateBUDetailCommand updatebUDetailModel, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(updatebUDetailModel, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Updation of BU failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }

        #endregion

        #region - Exam Bulk Upload -
        /// <summary>
        /// Exam Bulk Upload
        /// </summary>
        /// <param name="examModule">General Insurance / Life Insurance</param>
        /// <param name="files"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ExamBulkUpload")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<string>> ExamBulkUpload(IFormFile files, string? examModule, string? ExamTitle, string? createdBy, CancellationToken cancellationToken)
        {
            // Excel Code For Binding Models : Code Start
            List<ExamBulkUploadModel> questionModelList = new List<ExamBulkUploadModel>();
            List<ExamBulkUploadCommandInsertModel> optionModelList = new List<ExamBulkUploadCommandInsertModel>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                files.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    while (reader.Read())
                    {
                        // Foreach Logic For Options

                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(2))))
                        {
                            optionModelList.Add(new ExamBulkUploadCommandInsertModel
                            {
                                OptionValue = reader.GetValue(2).ToString(),
                                SrNo = reader.GetValue(0).ToString(),
                                IsCorrectAnswer = Convert.ToString(reader.GetValue(6)) == Convert.ToString(reader.GetValue(2)) ? true : false,
                            });
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(3))))
                        {
                            optionModelList.Add(new ExamBulkUploadCommandInsertModel
                            {
                                OptionValue = reader.GetValue(3).ToString(),
                                SrNo = reader.GetValue(0).ToString(),
                                IsCorrectAnswer = Convert.ToString(reader.GetValue(6)) == Convert.ToString(reader.GetValue(3)) ? true : false,
                            });
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(4))))
                        {
                            optionModelList.Add(new ExamBulkUploadCommandInsertModel
                            {
                                OptionValue = reader.GetValue(4).ToString(),
                                SrNo = reader.GetValue(0).ToString(),
                                IsCorrectAnswer = Convert.ToString(reader.GetValue(6)) == Convert.ToString(reader.GetValue(4)) ? true : false,
                            });
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(5))))
                        {
                            optionModelList.Add(new ExamBulkUploadCommandInsertModel
                            {
                                OptionValue = reader.GetValue(5).ToString(),
                                SrNo = reader.GetValue(0).ToString(),
                                IsCorrectAnswer = Convert.ToString(reader.GetValue(6)) == Convert.ToString(reader.GetValue(5)) ? true : false,
                            });
                        }
                        //
                        optionModelList.RemoveAll(T => T.SrNo == "S.No.");
                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(1))))
                        {
                            questionModelList.Add(new ExamBulkUploadModel
                            {
                                QuestionValue = reader.GetValue(1).ToString(),
                                SrNo = reader.GetValue(0).ToString()
                            });
                        }
                        questionModelList.RemoveAll(T => T.SrNo == "S.No.");
                    }

                }
            }
            var result = new HeroResult<bool>();
            foreach (var item in questionModelList)
            {
                item.ExamBulkUploadCommandInsertModel = optionModelList.FindAll(T => T.SrNo == item.SrNo);
                var req = new ExamBulkUploadCommand();
                req.SequenceNo = item.SequenceNo;
                req.ExamModuleType = examModule;
                req.ExamTitle = !string.IsNullOrWhiteSpace(ExamTitle) ? ExamTitle : "";
                req.QuestionValue = item.QuestionValue;
                req.IsActive = true;
                req.CreatedBy = createdBy;
                req.ExamBulkUploadCommandInsertModel = item.ExamBulkUploadCommandInsertModel;
                result = await _mediatr.Send(req, cancellationToken);
            }
            // Code End

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Exam Bulk Upload Failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Reset Admin Account Details -
        /// <summary>
        /// Reset Admin Account Details
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Dashboard data</returns>  
        [HttpPost]
        [Route("ResetAdminUserAccountDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> ResetAdminUserAccountDetail(string UserId, CancellationToken cancellationToken)
        {
            var req = new ResetAdminUserAccountDetailCommand
            {
                UserId = UserId,
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Admin User reset account process failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Upload Training File For Training Management -
        /// <summary>
        /// Upload Training File For Training Management
        /// </summary>
        /// <param name="examModule">General Insurance / Life Insurance</param>
        /// <param name="files"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadTrainingFileForTrainingManagement")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UploadTrainingFileForTrainingManagement(IFormFile files, string? trainingModuleType, string? materialFormatType, string? videoDuration, string? lessonNumber, string? lessionTitle, string? createdBy, CancellationToken cancellationToken)
        {
            try
            {
                var req = new UploadTrainingFileCommand
                {
                    TrainingModuleType = trainingModuleType,
                    LessionTitle = lessionTitle,
                    MaterialFormatType = materialFormatType,
                    VideoDuration = videoDuration,
                    LessonNumber = lessonNumber,
                    CreatedBy = createdBy
                };
                if (files != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Services/Files");
                    string mongoDbId;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var fileNameWithPath = Path.Combine(path, files.FileName);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        files.CopyTo(stream);
                    }
                    byte[] byteimage = System.IO.File.ReadAllBytes(fileNameWithPath);
                    Stream fileStream = new MemoryStream(byteimage);
                    System.IO.File.Delete(fileNameWithPath);
                    var dname = Path.GetDirectoryName(fileNameWithPath);
                    if (!string.IsNullOrWhiteSpace(dname))
                        Directory.Delete(dname, true);
                    mongoDbId = await _mongodbService.MongoUpload(files.FileName, fileStream, byteimage);
                    req.DocumentFileName = files.FileName;
                    req.ImageStream = byteimage;
                    req.DocumentId = !string.IsNullOrWhiteSpace(mongoDbId) ? mongoDbId : "";
                }
                var result = await _mediatr.Send(req, cancellationToken);
                if (result.Failed)
                {
                    var problemDetails = Result.CreateNotFoundError("Error while uploading file");
                    return NotFound(problemDetails);
                }
                var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
                return Ok(res);
            }
            catch (Exception)
            {
                var problemDetails = Result.CreateNotFoundError("Error while uploading file");
                return NotFound(problemDetails);
            }
        }
        #endregion

        #region - Request For Edit Profile -
        /// <summary>
        /// Request For Edit Profile
        /// </summary>
        /// <param name="roleDetailInsertCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RequestForEditProfile")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> RequestForEditProfile(RequestForEditProfileCommand command, CancellationToken cancellationToken)
        {
            var profileRequest = await _mediatr.Send(command, cancellationToken);
            if (!profileRequest.Result)
            {
                var problemDetails = Result.CreateNotFoundError("Inserting Role Details failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(profileRequest.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #endregion

        #region - PUT Methods -

        #region - UpdateRoleModulePermission -
        /// <summary>
        /// Update Role Module Permission
        /// </summary>
        /// <param name="rolePermissionCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateRoleModulePermission")]
        [ApiExplorerSettings(IgnoreApi = false)]
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

        #region - BUUpdateDetails -
        /// <summary>
        /// BU Update details
        /// </summary>
        /// <param name="buUpdateCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("BUUpdateDetails")]
        [ApiExplorerSettings(IgnoreApi = true)]
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

        #region - Role Detail Update -
        /// <summary>
        /// Role Detail Update
        /// </summary>
        /// <param name="roleDetailUpdateCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("RoleDetailUpdate")]
        [ApiExplorerSettings(IgnoreApi = false)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> RoleDetailUpdate(RoleDetailUpdateCommand roleDetailUpdateCommand, CancellationToken cancellationToken)
        {
            var UpdateBU = await _mediatr.Send(roleDetailUpdateCommand, cancellationToken);
            if (UpdateBU.Failed)
            {
                var problemDetails = Result.CreateNotFoundError(UpdateBU.Messages);
                return BadRequest(problemDetails);
            }
            var res = Result.CreateSuccess(UpdateBU.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - UpdateActivateDeActivateRole -
        /// <summary>
        /// UpdateActivateDeActivateRole
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateActivateDeActivateRole")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateActivateDeActivateRole(UpdateActivateDeActivateRoleCommand UpdateActivateDeActivateRoleCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(UpdateActivateDeActivateRoleCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Updation of Role failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - UpdateActivateDeActivateBU -
        /// <summary>
        /// UpdateActivateDeActivateBU
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateActivateDeActivateBU")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateActivateDeActivateBU(UpdateActivateDeActivateBUCommand UpdateActivateDeActivateBUCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(UpdateActivateDeActivateBUCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Updation of BU failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - UpdateActivateDeActivateTrainingManagement -
        /// <summary>
        /// UpdateActivateDeActivateTrainingManagement
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateActivateDeActivateTrainingManagement")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateActivateDeActivateTrainingManagement(UpdateActivateDeActivateTrainingManagementCommand UpdateActivateDeActivateTrainingManagementCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(UpdateActivateDeActivateTrainingManagementCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Updation of BU failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - UpdateActivateDeActivateExamManagement -
        /// <summary>
        /// UpdateActivateDeActivateTrainingManagement
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateActivateDeActivateExamManagement")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateActivateDeActivateExamManagement(UpdateActivateDeActivateExamManagementCommand UpdateActivateDeActivateExamManagementCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(UpdateActivateDeActivateExamManagementCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Updation of BU failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Update Document Status -
        /// <summary>
        /// Update Document Status
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateDocumentStatus")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateDocumentStatus(UpdateDocumentStatusCommand updateDocumentStatusCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(updateDocumentStatusCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Updation of BU failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Update User Personal Detail -
        /// <summary>
        /// Update User (POSP) Personal Detail
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Dashboard data</returns>
        [HttpPut]
        [Route("UpdateUserPersonalDetail")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateUserPersonalDetail(UpdatePersonalDetailsCommand userPersonalDetailCommand, CancellationToken cancellationToken)
        {

            var result = await _mediatr.Send(userPersonalDetailCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("User Personal Detail Update failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - UpdateAgreementStatusByUserId -
        /// <summary>
        /// UpdateAgreementStatusByUserId
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateAgreementStatusByUserId")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateAgreementStatusByUserId(UpdateAgreementStatusByUserIdCommand UpdateAgreementStatusByUserIdCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(UpdateAgreementStatusByUserIdCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Updation of BU failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Update Exam Particular Question Detail -
        /// <summary>
        /// Update Exam Particular Question Detail
        /// </summary>
        /// <param name="updateExamParticularQuestionCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateExamParticularQuestionDetail")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateExamParticularQuestionDetail(UpdateExamParticularQuestionCommand updateExamParticularQuestionCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(updateExamParticularQuestionCommand, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Updation Failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #endregion

        #region - DELETE Methods -

        #region - Delete Training Management Detail - 
        /// <summary>
        /// DeleteTrainingManagementDetail
        /// </summary>
        /// <param name="TrainingMaterialId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteTrainingManagementDetail")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> DeleteTrainingManagementDetail(string TrainingMaterialId, CancellationToken cancellationToken)
        {
            var req = new DeleteTrainingManagementDetailCommand
            {
                TrainingMaterialId = TrainingMaterialId
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Exam Instructions Detail deleted failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Delete Exam Management Detail - 
        /// <summary>
        /// DeleteExamManagementDetail
        /// </summary>
        /// <param name="QuestionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteExamManagementDetail")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> DeleteExamManagementDetail(string QuestionId, CancellationToken cancellationToken)
        {
            var req = new DeleteExamManagementDetailCommand
            {
                QuestionId = QuestionId
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Exam Instructions Detail deleted failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Delete Policies Detail - 
        /// <summary>
        /// DeletePoliciesDetail
        /// </summary>
        /// <param name="POSPId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeletePoliciesDetail")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> DeletePoliciesDetail(string POSPId, CancellationToken cancellationToken)
        {
            var req = new DeletePoliciesDetailCommand
            {
                POSPId = POSPId
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Exam Instructions Detail deleted failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion


        #region - GetMyTotalSales -
        /// <summary>
        /// GetTotalSales
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTotalSales")]
        [ProducesResponseType(typeof(GetTotalSalesDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetTotalSalesDetailVm>>> GetTotalSalesDetail(string? UserId, string? startDate, string? endDate, CancellationToken cancellationToken)
        {
            var req = new GetTotalSalesDetailQuery()
            {
                UserId = UserId,
                StartDate = startDate,
                EndDate = endDate,
            };

            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError("Total Sales not found");
                return BadRequest(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetCardsDetail -
        /// <summary>
        /// GetCardsDetail
        /// </summary>       
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCardsDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(GetCardsDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetCardsDetailVm>>> GetCardsDetail(string? userId, CancellationToken cancellationToken)
        {
            var req = new GetCardsDetailQuery()
            {
                UserId = userId
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

        #region - GetRelationshipManager -
        /// <summary>
        /// GetCardsDetail
        /// </summary>       
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRelationshipManager")]
        [ProducesResponseType(typeof(GetRelationshipManagerVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetRelationshipManagerVm>>> GetRelationshipManager(string? userId, CancellationToken cancellationToken)
        {
            var req = new GetRelationshipManagerQuery()
            {
                UserId = userId
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


        #region - UpdateActivePOSPAccountDetail -
        /// <summary>
        /// Update Active POSPAccount Detail
        /// </summary>
        /// <param name="rolePermissionCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateActivePOSPAccountDetail")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateActivePOSPAccountDetail(UpdateActivePOSPAccountDetailCommand updateActivePOSPAccountDetailCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(updateActivePOSPAccountDetailCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Update POSP Account Detail failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get Insurance Type -
        /// <summary>
        /// Get Insurance Type
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetInsuranceType")]
        [ProducesResponseType(typeof(GetInsuranceTypeVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetInsuranceTypeVm>> GetInsuranceType(CancellationToken cancellationToken)
        {
            var req = new GetInsuranceTypeQuery() { };
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

        #region - Get Lead Stage -
        /// <summary>
        /// Get Lead Stage
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetLeadStage")]
        [ProducesResponseType(typeof(GetLeadStageVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetLeadStageVm>> GetLeadStage(CancellationToken cancellationToken)
        {
            var req = new GetLeadStageQuery() { };
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

        #region - GetRecipientList -
        /// <summary>
        /// Get Recipient List
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRecipientList")]
        [ProducesResponseType(typeof(GetRecipientListVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetRecipientListVm>>> GetRecipientList(string? searchtext, string? RecipientType, CancellationToken cancellationToken)
        {
            var req = new GetRecipientListQuery()
            {
                SearchText = searchtext,
                RecipientType = RecipientType
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


        #region - Insert Lead Details - 
        /// <summary>
        /// InsertLeadDetails
        /// </summary>
        /// <param name="updatePOSPTrainingDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertLeadDetails")]
        [ProducesResponseType(typeof(InsertLeadDetailsModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<InsertLeadDetailsModel>> InsertLeadDetails(string? LeadName, string? LeadPhoneNumber, string? LeadEmailId, string? UserId, CancellationToken cancellationToken)
        {
            var req = new InsertLeadDetailsCommand
            {
                LeadName = LeadName,
                LeadPhoneNumber = LeadPhoneNumber,
                LeadEmailId = LeadEmailId,
                UserId = UserId
            };
            var result = await _mediatr.Send(req, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Training Detail Update failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetNotificationList -
        /// <summary>
        /// Get GeNotificationList
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetNotificationList")]
        [ProducesResponseType(typeof(GetNotificationListVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetNotificationListResponseModel>> GetNotificationList(string? SearchText, string? RecipientTypeId, string? StartDate, string? EndDate, int? CurrentPageIndex, int? CurrentPageSize, CancellationToken cancellationToken)
        {
            var req = new GetNotificationListQuery
            {
                SearchText = SearchText,
                RecipientTypeId = RecipientTypeId,
                StartDate = StartDate,
                EndDate = EndDate,
                CurrentPageIndex = CurrentPageIndex == null ? 1 : CurrentPageIndex,
                CurrentPageSize = CurrentPageSize == null ? 10 : CurrentPageSize
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

        #region - InsertNotification -
        /// <summary>
        /// InsertNotification
        /// </summary>
        /// <param name="insertNotification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertNotification")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> InsertNotification(InsertNotificationCommand insertNotificationCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(insertNotificationCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Insertion of Notification failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - EditNotification -
        /// <summary>
        /// EditNotification
        /// </summary>
        /// <param name="editNotification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("EditNotification")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> EditNotification(EditNotificationCommand editNotificationCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(editNotificationCommand, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Edit Notification failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - DownloadAgreement -
        /// <summary>
        /// EditNotification
        /// </summary>
        /// <param name="editNotification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("DownloadAgreement")]
        [ProducesResponseType(typeof(DownloadAgreementResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<DownloadAgreementResponse>>> DownloadAgreement(string? POSPId, CancellationToken cancellationToken)
        {
            var req = new DownloadAgreementCommand
            {
                POSPId = POSPId
            };
            var result = await _mediatr.Send(req, cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Edit Notification failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetUserCategory -
        /// <summary>
        /// Get Role Types
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserCategory")]
        [ProducesResponseType(typeof(GetUserCategoryVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetUserCategoryVm>>> GetUserCategory(CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new GetUserCategoryQuery(), cancellationToken);

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetAssistedBUDetails -
        /// <summary>
        /// Get Assisted BU Details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAssistedBUDetails")]
        [ProducesResponseType(typeof(GetAssistedBUDetailsResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetAssistedBUDetailsResponseModel>> GetAssistedBUDetails(string? RoleId, string? UserId, CancellationToken cancellationToken)
        {
            var req = new GetAssistedBUDetailsQuery
            {
                RoleId = RoleId,
                UserId = UserId
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("BU Detail not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion


        #region - GetDeactivatedUser -
        /// <summary>
        /// GetDeactivatedUser
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDeactivatedUser")]
        [ProducesResponseType(typeof(GetDeactivatedUserVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetDeactivatedUserReponseModel>> GetDeactivatedUser(GetDeactivatedUserRequestModel getDeactivatedUserQuery, CancellationToken cancellationToken)
        {
            GetDeactivatedUserQuery req = new GetDeactivatedUserQuery
            {
                CurrentPageIndex = getDeactivatedUserQuery.CurrentPageIndex,
                CurrentPageSize = getDeactivatedUserQuery.CurrentPageSize,
                RelationManagerId = getDeactivatedUserQuery.RelationManagerId,
                EndDate = getDeactivatedUserQuery.EndDate,
                SearchText = getDeactivatedUserQuery.SearchText,
                StartDate = getDeactivatedUserQuery.StartDate,
            };

            int index = 1;
            int reqStarter = getDeactivatedUserQuery.CurrentPageIndex == 1 ? 0 : Convert.ToInt32(getDeactivatedUserQuery.CurrentPageIndex - 1) * Convert.ToInt32(getDeactivatedUserQuery.CurrentPageSize);
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Result != null)
            {
                foreach (var item in result.Result.POSPUserDetail)
                {
                    item.SerialNumber = index + reqStarter;
                    index++;
                }
            }

            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetAllBUUser -
        /// <summary>
        /// GetAllBUUser
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllBUUserRole/{BUId}")]
        [ProducesResponseType(typeof(GetAllBUUserRoleVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAllBUUserRoleVm>>> GetAllBUUserRole([FromRoute] string? BUId, CancellationToken cancellationToken)
        {
            GetAllBUUserRoleQuery req = new GetAllBUUserRoleQuery
            {
                BUId = BUId
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("BU Detail not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get All Shared Reporting Role -
        /// <summary>
        /// Get All Shared Reporting Role
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllSharedReportingRole")]
        [ProducesResponseType(typeof(GetAllSharedReportingRoleVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetAllSharedReportingRoleVm>> GetAllSharedReportingRole(CancellationToken cancellationToken)
        {
            var req = new GetAllSharedReportingRoleQuery() { };
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


        #region - CheckForRole -
        /// <summary>
        /// CheckForRole
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckForRole/{UserId}")]
        [ProducesResponseType(typeof(CheckForRoleVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<CheckForRoleVm>>> CheckForRole([FromRoute] string UserId, CancellationToken cancellationToken)
        {
            CheckForRoleQuery req = new CheckForRoleQuery
            {
                UserId = UserId
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("BU Detail not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetAllBUDetails By UserID -
        /// <summary>
        /// Get role details
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetAllBUDetailsByUserID")]
        [ProducesResponseType(typeof(GetAllBUDetailsByUserIDVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAllBUDetailsByUserIDVm>>> GetAllBUDetailsByUserID(GetAllBUDetailsByUserIDQuery objBUDetails, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(objBUDetails, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion
    }
}

