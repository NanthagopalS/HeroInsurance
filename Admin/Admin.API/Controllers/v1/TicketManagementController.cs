using Admin.Core.Features.TicketManagement.Command;
using Admin.Core.Features.TicketManagement.Queries.GetDeactivationTicketManagementDetail;
using Admin.Core.Features.TicketManagement.Queries.GetPOSPDetailsByDeactiveTicketId;
using Admin.Core.Features.TicketManagement.Queries.GetPOSPDetailsByIDToDeActivate;
using Admin.Core.Features.TicketManagement.Queries.GetTicketManagementDetail;
using Admin.Core.Features.TicketManagement.Queries.GetTicketManagementDetailById;
using Admin.Core.Features.User.Commands.DeactivateUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;

namespace Admin.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class TicketManagementController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly IMongoDBService _mongodbService;
        private readonly IEmailService _emailService;
        //IExcelDataReader reader;
        //ServicingContext context;
        public TicketManagementController(IMediator mediatr, IMongoDBService mongodbService, IEmailService emailService)
        {
            _mediatr = mediatr;
            _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
            _emailService = emailService;   
        }

        #region - Get Ticket Management  -
        /// <summary>
        /// Get Ticket Management 
        /// </summary>
        /// <param name="SearchText"></param>
        /// <param name="PolicyType"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="CurrentPageSize"></param>
        /// <param name="CurrentPageIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTicketManagementDetail")]
        [ProducesResponseType(typeof(GetTicketManagementDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetTicketManagementDetailVm>>> GetTicketManagementDetail(string? TicketType, string? SearchText, string? RelationshipManagerIds, string? PolicyType, string? StartDate, string? EndDate, int? CurrentPageIndex, int? CurrentPageSize, CancellationToken cancellationToken)
        {
            var req = new GetTicketManagementDetailQuery
            {
                TicketType = TicketType,
                SearchText = SearchText,
                RelationshipManagerIds = RelationshipManagerIds,
                PolicyType = PolicyType,
                StartDate = StartDate,
                EndDate = EndDate,
                CurrentPageIndex = CurrentPageIndex == null ? 1 : CurrentPageIndex,
                CurrentPageSize = CurrentPageSize == null ? 10 : CurrentPageSize
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

        #region - GetTicketManagementDetailById -
        /// <summary>
        /// GetTicketManagementDetailById
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTicketManagementDetailById")]
        [ProducesResponseType(typeof(GetTicketManagementDetailByIdVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetTicketManagementDetailByIdVm>>> GetTicketManagementDetailById(string TicketId, CancellationToken cancellationToken)
        {
            var req = new GetTicketManagementDetailByIdQuery
            {
                TicketId = TicketId
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Result.Count()>0)
            {
                foreach (var rec in result.Result)
                {
                     if(rec.DocumentId != null && rec.DocumentId!="")
                    {
                        rec.DocumentIdArray = rec.DocumentId.Split(",");
                        foreach (var fileID in rec.DocumentIdArray)
                        {
                            rec.DocumentB64String.Add(await _mongodbService.MongoDownload(fileID));
                        }
                    }
                }
            }
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetDeactivationTicketManagementDetail -
        /// <summary>
        /// GetDeactivationTicketManagementDetail
        /// </summary>
        /// <param name="SearchText"></param>
        /// <param name="RelationshipManagerId"></param>
        /// <param name="PolicyType"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="CurrentPageSize"></param>
        /// <param name="CurrentPageIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDeactivationTicketManagementDetail")]
        [ProducesResponseType(typeof(GetDeactivationTicketManagementDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetDeactivationTicketManagementDetailVm>>> GetDeactivationTicketManagementDetail(string? SearchText, string? RelationshipManagerId, string? PolicyType, string? StartDate, string? EndDate, int? CurrentPageIndex, int? CurrentPageSize, CancellationToken cancellationToken)
        {
            var req = new GetDeactivationTicketManagementDetailQuery
            {
                SearchText = SearchText,
                RelationshipManagerId = RelationshipManagerId,
                PolicyType = PolicyType,
                StartDate = StartDate,
                EndDate = EndDate,
                CurrentPageIndex = CurrentPageIndex == null ? 1 : CurrentPageIndex,
                CurrentPageSize = CurrentPageSize == null ? 10 : CurrentPageSize
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

        #region - GetPOSPDetailsByIDToDeActivate -
        /// <summary>
        /// GetPOSPDetailsByIDToDeActivate
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPOSPDetailsByIDToDeActivate")]
        [ProducesResponseType(typeof(GetPOSPDetailsByIDToDeActivateVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetPOSPDetailsByIDToDeActivateVm>>> GetPOSPDetailsByIDToDeActivate(string POSPId, CancellationToken cancellationToken)
        {
            var req = new GetPOSPDetailsByIDToDeActivateQuery
            {
                POSPId = POSPId
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

        #region - UpdateTicketManagementDetailById -
        /// <summary>
        /// UpdateTicketManagementDetailById
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateTicketManagementDetailById")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateTicketManagementDetailById(string TicketId, string Description, string Status, CancellationToken cancellationToken)
        {
            var req = new UpdateTicketManagementDetailByIdCommand
            {
                TicketId = TicketId,
                Description = Description,
                Status = Status
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

        /*#region - GetDeactivationTicketManagementDetail -
        /// <summary>
        /// GetDeactivationTicketManagementDetail
        /// </summary>
        /// <param name="SearchText"></param>
        /// <param name="RelationshipManagerId"></param>
        /// <param name="PolicyType"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="CurrentPageSize"></param>
        /// <param name="CurrentPageIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDeactivationTicketManagementDetail")]
        [ProducesResponseType(typeof(GetDeactivationTicketManagementDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetDeactivationTicketManagementDetailVm>>> GetDeactivationTicketManagementDetail(string? SearchText, string? RelationshipManagerId, string? PolicyType, string? StartDate, string? EndDate, int? CurrentPageIndex, int? CurrentPageSize, CancellationToken cancellationToken)
        {
            var req = new GetDeactivationTicketManagementDetailQuery
            {
                SearchText = SearchText,
                RelationshipManagerId = RelationshipManagerId,
                PolicyType = PolicyType,
                StartDate = StartDate,
                EndDate = EndDate,
                CurrentPageIndex = CurrentPageIndex == null ? 1 : CurrentPageIndex,
                CurrentPageSize = CurrentPageSize == null ? 10 : CurrentPageSize
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
        #endregion*/

        /*#region - GetPOSPDetailsByIDToDeActivate -
        /// <summary>
        /// GetPOSPDetailsByIDToDeActivate
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPOSPDetailsByIDToDeActivate")]
        [ProducesResponseType(typeof(GetPOSPDetailsByIDToDeActivateVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetPOSPDetailsByIDToDeActivateVm>>> GetPOSPDetailsByIDToDeActivate(string? POSPId, CancellationToken cancellationToken)
        {
            var req = new GetPOSPDetailsByIDToDeActivateQuery
            {
                POSPId = POSPId
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
        #endregion*/


        #region - SendDeactivateTicketNOCEmail -
        /// <summary>
        /// UpdateTicketManagementDetailById
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendDeactivateTicketNOCEmail")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> SendDeactivateTicketNOCEmail(string deactivateID, CancellationToken cancellationToken)
        {
            var req = new GetPOSPDetailsByDeactiveTicketIdQuery
            {
                POSPId = deactivateID
            };
            var result = await _mediatr.Send(req, cancellationToken);

            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(errorDetails);
            }
            var deactivateCommand = new DeactivateUserByIdCommand
            {
                UserId = result.Result.FirstOrDefault().UserId,
                DeactivatePOSPId = deactivateID
            };
            var deactivate = await _mediatr.Send(deactivateCommand, cancellationToken);

            var res = Result.CreateSuccess(deactivate.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

    }
}
