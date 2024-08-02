using Admin.Core.Features.Banners.Queries.GetBannerDetail;
using Admin.Core.Features.HelpAndSupport.Queries.GetAllHelpAndSupport;
using Admin.Core.Features.HelpAndSupport.Queries.GetConcernType;
using Admin.Core.Features.HelpAndSupport.Queries.GetSubConcernType;
using Admin.Core.Features.User.Queries.GetAllTrainingManagementDetails;
using Admin.API.Models;
using Admin.Core.Features.HelpAndSupport.RaiseRequest;
using Admin.Core.Features.User.Commands.InsertNotification;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using Admin.Core.Features.User.Queries.GetRecipientList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using System.IO;
using System.Net;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;
using Admin.Core.Features.User.Commands.UserRoleGetMapping;
using Admin.Core.Features.User.Queries.GetParticularBUDetail;
using Admin.Core.Features.HelpAndSupport.Queries.DeleteHelpAndSupport;
using Admin.Domain.User;
using Admin.Core.Features.HelpAndSupport.InsertDeactivatePospDetails;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Admin.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class HelpAndSupportController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly IMongoDBService _mongodbService;

        public HelpAndSupportController(IMediator mediatr, IMongoDBService mongodbService)
        {
            _mediatr = mediatr;
            _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        }

        #region - GET Methods -

        #region - Get SubConcern Types  -
        /// <summary>
        /// SubConcernType
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSubConcernType")]
        [ProducesResponseType(typeof(GetSubConcernTypeVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetSubConcernTypeVm>>> GetSubConcernType(string? concernTypeId,CancellationToken cancellationToken)
        {
            var req = new GetSubConcernTypeQuery()
            {
                ConcernTypeId = concernTypeId
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

        #region - Get Concern Types  -
        /// <summary>
        /// GetConcernType
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetConcernType")]
        [ProducesResponseType(typeof(GetConcernTypeVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetConcernTypeVm>>> GetConcernType(CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new GetConcernTypeQuery(), cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get ALl Help and Support -
        /// <summary>
        /// Get ALl Help and Support
        /// </summary>
        /// <param name="searchtext"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllHelpAndSupport")]
        [ProducesResponseType(typeof(GetAllHelpAndSupportVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAllHelpAndSupportVm>>> GetAllHelpAndSupport(string? searchtext, string? UserId, string? startDate, string? endDate, int? currentpageIndex, int? currentpageSize, CancellationToken cancellationToken)
        {
            var req = new GetAllHelpAndSupportQuery
            {
                Searchtext = searchtext,
                UserId = UserId,
                StartDate = startDate,
                EndDate = endDate,
                CurrentPageIndex = currentpageIndex == null ? 1 : currentpageIndex,
                CurrentPageSize = currentpageSize == null ? 10 : currentpageSize
            };
            int  index = 1;
            int reqStarter = req.CurrentPageIndex == 1 ? 0 : Convert.ToInt32(req.CurrentPageIndex - 1) * Convert.ToInt32(req.CurrentPageSize);
            var result = await _mediatr.Send(req, cancellationToken);
            if(result.Result.GetAllHelpAndSupportModel.Count()>0)
            {
                foreach (var item in result.Result.GetAllHelpAndSupportModel)
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

        #region - GetParticularHelpAndSupport - 
        /// <summary>
        /// Get Particular Help And Support
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetParticularHelpAndSupport")]
        [ProducesResponseType(typeof(GetParticularHelpAndSupportVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetParticularHelpAndSupportVm>>> GetParticularHelpAndSupport(string requestId, CancellationToken cancellationToken)
        {
            var req = new GetParticularHelpAndSupportQuery
            {
                RequestId = requestId
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Result.Count() > 0)
            {
                foreach (var rec in result.Result)
                {
                    if (rec.DocumentId != null && rec.DocumentId != "")
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
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #endregion

        #region - POST Methods -

        #region - Raise Request -
        /// <summary>
        /// RaiseRequest
        /// </summary>
        /// <param name="files"></param>
        /// <param name="concernTypeId"></param>
        /// <param name="subConcernTypeId"></param>
        /// <param name="subjectText"></param>
        /// <param name="detailText"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RaiseRequest")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> RaiseRequest(List<IFormFile>? files, string concernTypeId, string subConcernTypeId,
            string subjectText, string detailText,string userId, CancellationToken cancellationToken)
        {
            var cmd = new RaiseRequestCommand
            {
                ConcernTypeId = concernTypeId,
                SubConcernTypeId = subConcernTypeId,
                SubjectText = subjectText,
                DetailText = detailText,
                UserId = userId
            };
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (!file.FileName.Contains(".pdf") && !file.FileName.Contains(".jpg") && !file.FileName.Contains(".jpeg"))
                    {
                        var problemDetails = new ResultModel()
                        {
                            Data = string.Empty,
                            Message = "Only PDF,JPG & JPEG formats allowed.",
                            StatusCode = ((int)HttpStatusCode.NotFound).ToString()
                        };
                        return NotFound(problemDetails);
                    }
                    if (file.Length > 2097152)
                    {
                        var problemDetails = new ResultModel()
                        {
                            Data = string.Empty,
                            Message = "File size should be less than 2MB.",
                            StatusCode = ((int)HttpStatusCode.NotFound).ToString()
                        };
                        return NotFound(problemDetails);
                    }
                    string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Services/Files");
                    string mongoDbId;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var fileNameWithPath = System.IO.Path.Combine(path, file.FileName);
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    byte[] byteimage = System.IO.File.ReadAllBytes(fileNameWithPath);
                    Stream fileStream = new MemoryStream(byteimage);
                    System.IO.File.Delete(fileNameWithPath);
                    var dname = System.IO.Path.GetDirectoryName(fileNameWithPath);
                    if (!string.IsNullOrWhiteSpace(dname))
                        Directory.Delete(dname, true);
                    mongoDbId = await _mongodbService.MongoUpload(file.FileName, fileStream, byteimage);
                    if (!string.IsNullOrWhiteSpace(cmd.DocumentId))
                        cmd.DocumentId = string.Join(",", cmd.DocumentId, mongoDbId);
                    else
                        cmd.DocumentId = mongoDbId;
                }
            }
            var result = await _mediatr.Send(cmd, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Raise Request Failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - InsertDeactivatePospDetails -
        /// <summary>
        /// InsertDeactivatePospDetails
        /// </summary>
        /// <param name="insertDeactivatePospDetailsCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertDeactivatePospDetails")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> InsertDeactivatePospDetails(IFormFile? EmailAttachmentDocument, IFormFile? BusinessTeamApprovalAttachmentDocument, string? POSPId, string? remark,string? Status, CancellationToken cancellationToken)
        {
            var cmd = new InsertDeactivatePospDetailsCommand()
            {
                POSPId = POSPId,
                Remark = remark,
                Status = Status
            };
            if (EmailAttachmentDocument != null)
            {
               
                    string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Services/Files");
                    string mongoDbId;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var fileNameWithPath = System.IO.Path.Combine(path, EmailAttachmentDocument.FileName);
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        EmailAttachmentDocument.CopyTo(stream);
                    }
                    byte[] byteimage = System.IO.File.ReadAllBytes(fileNameWithPath);
                    Stream fileStream = new MemoryStream(byteimage);
                    System.IO.File.Delete(fileNameWithPath);
                    var dname = System.IO.Path.GetDirectoryName(fileNameWithPath);
                    if (!string.IsNullOrWhiteSpace(dname))
                        Directory.Delete(dname, true);
                    mongoDbId = await _mongodbService.MongoUpload(EmailAttachmentDocument.FileName, fileStream, byteimage);
                    if (!string.IsNullOrWhiteSpace(cmd.DocumentId1))
                        cmd.DocumentId1 = string.Join(",", cmd.DocumentId1, mongoDbId);
                    else
                        cmd.DocumentId1 = mongoDbId;

                
            }
            if (BusinessTeamApprovalAttachmentDocument != null) 
            {

                    string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Services/Files");
                    string mongoDbId;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var fileNameWithPath = System.IO.Path.Combine(path, BusinessTeamApprovalAttachmentDocument.FileName);
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                    BusinessTeamApprovalAttachmentDocument.CopyTo(stream);
                    }
                    byte[] byteimage = System.IO.File.ReadAllBytes(fileNameWithPath);
                    Stream fileStream = new MemoryStream(byteimage);
                    System.IO.File.Delete(fileNameWithPath);
                    var dname = System.IO.Path.GetDirectoryName(fileNameWithPath);
                    if (!string.IsNullOrWhiteSpace(dname))
                        Directory.Delete(dname, true);
                    mongoDbId = await _mongodbService.MongoUpload(BusinessTeamApprovalAttachmentDocument.FileName, fileStream, byteimage);
                    if (!string.IsNullOrWhiteSpace(cmd.DocumentId2))
                        cmd.DocumentId2 = string.Join(",", cmd.DocumentId2, mongoDbId);
                    else
                        cmd.DocumentId2 = mongoDbId;

                
            }
                var insertRole = await _mediatr.Send(cmd, cancellationToken);
            if (!insertRole.Result)
            {
                var problemDetails = Result.CreateNotFoundError("Inserting Files failed");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(insertRole.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #endregion

        #region - DELETE Methods -

        #region - Delete Help And Support - 
        /// <summary>
        /// DeleteHelpAndSupport
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteHelpAndSupport")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> DeleteHelpAndSupport(string requestId, CancellationToken cancellationToken)
        {
            var req = new DeleteHelpAndSupportCommand
            {
                RequestId = requestId
            };
            var result = await _mediatr.Send(req, cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Delete Failed");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #endregion
    }
}

