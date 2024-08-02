using Admin.API.Helpers;
using Admin.Core.Features.Notification.Command;
using Admin.Core.Features.Notification.Queries.GetAdminAlertType;
using Admin.Core.Features.Notification.Queries.GetAdminRecipientType;
using Admin.Core.Features.Notification.Queries.GetNotificationByIdAndType;
using Admin.Core.Features.Notification.Queries.GetNotificationDetailById;
using Admin.Core.Features.Notification.Queries.GetNotificationRecordById;
using Admin.Core.Features.User.Commands.PublishNotification;
using Admin.Domain.Notification;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;
using ThirdPartyUtilities.Implementation;

namespace Admin.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly IMongoDBService _mongodbService;
        //IExcelDataReader reader;
        //ServicingContext context;
        public NotificationController(IMediator mediatr, IMongoDBService mongodbService)
        {
            _mediatr = mediatr;
            _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        }

        #region - Get Admin Alert Type -
        /// <summary>
        /// GetAdminAlertType
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAdminAlertType")]
        [ProducesResponseType(typeof(GetAdminAlertTypeVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAdminAlertTypeVm>>> GetAdminAlertType(CancellationToken cancellationToken)
        {
            var req = new GetAdminAlertTypeQuery();

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

        #region - Get Admin Recipient Type -
        /// <summary>
        /// GetAdminRecipientType
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAdminRecipientType")]
        [ProducesResponseType(typeof(GetAdminRecipientTypeVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAdminRecipientTypeVm>>> GetAdminRecipientType(CancellationToken cancellationToken)
        {
            var req = new GetAdminRecipientTypeQuery();

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

        #region - Get Notification Detail By Id -
        /// <summary>
        /// GetNotificationDetailById
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetNotificationDetailById")]
        [ProducesResponseType(typeof(GetNotificationDetailByIdVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetNotificationDetailByIdVm>>> GetNotificationDetailById(string? NotificationId, CancellationToken cancellationToken)
        {
            var req = new GetNotificationDetailByIdQuery
            {
                NotificationId = NotificationId
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

        #region - Get Notification By Id And Type  -
        /// <summary>
        /// GetNotificationByIdAndType 
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetNotificationByIdAndType ")]
        [ProducesResponseType(typeof(GetNotificationByIdAndTypeVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetNotificationByIdAndTypeVm>> GetNotificationByIdAndType(string? UserId, string? NotificationCategory, CancellationToken cancellationToken)
        {
            var req = new GetNotificationByIdAndTypeQuery
            {
                UserId = UserId,
                NotificationCategory = NotificationCategory
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

        #region - PublishNotification -
        /// <summary>
        /// InsertNotification
        /// </summary>
        /// <param name="publishNotification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PublishNotification")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> PublishNotification(PublishNotificationCommand publishNotification, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(publishNotification, cancellationToken);
            // Metho call 
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Publishing of Notification failed");
                return NotFound(problemDetails);
            }
            else
            {
                string envType = AppConfiguration.GetConfiguration("Queue:enviourment");
                string rabbitMQHostName = AppConfiguration.GetConfiguration("QueueServers:hotName");

                GetNotificationRecordByIdQuery getOneRecord = new GetNotificationRecordByIdQuery() { NotificationId = publishNotification.NotificationId };

                var masterNotification = await _mediatr.Send(getOneRecord, cancellationToken);

                if (masterNotification.Result.Count()>0)
                {
                    QueueServiceProducer producer = new QueueServiceProducer(rabbitMQHostName);
                    foreach (var notificationMaster in masterNotification.Result)
                    {
                        if (notificationMaster.AlertType=="Push")
                        {
                            JObject body = new JObject();
                            body["notification_id"]= notificationMaster.NotificationId;
                            producer.Produce(envType + "pushNotificationQueue", body);
                        }
                        
                    }
                }
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - UpdateNotificationViewStatus -
        /// <summary>
        /// UpdateTicketManagementDetailById
        /// </summary>       
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateNotificationViewStatus")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> UpdateNotificationViewStatus(string? NotificationBoradcastId, CancellationToken cancellationToken)
        {
            var req = new UpdateNotificationViewStatusCommand
            {
                NotificationBoradcastId = NotificationBoradcastId
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
    }
}
