using DinkToPdf;
using DinkToPdf.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSP.Core.Features.POSP.Commands.DeleteExamInstructionsDetail;
using POSP.Core.Features.POSP.Commands.DeleteTrainingInstructionsDetail;
using POSP.Core.Features.POSP.Commands.DeleteTrainingMaterialDetail;
using POSP.Core.Features.POSP.Commands.InsertAgreement;
using POSP.Core.Features.POSP.Commands.InsertAgreementId;
using POSP.Core.Features.POSP.Commands.InsertCertificateDocumentId;
using POSP.Core.Features.POSP.Commands.InsertExamBanner;
using POSP.Core.Features.POSP.Commands.InsertExamCertificate;
using POSP.Core.Features.POSP.Commands.InsertExamDetail;
using POSP.Core.Features.POSP.Commands.InsertExamInstructionsDetail;
using POSP.Core.Features.POSP.Commands.InsertPOSPRating;
using POSP.Core.Features.POSP.Commands.InsertPOSPTrainingDetail;
using POSP.Core.Features.POSP.Commands.InsertPOSPTrainingProgressDetail;
using POSP.Core.Features.POSP.Commands.InsertReferralNewUserDetails;
using POSP.Core.Features.POSP.Commands.InsertTrainingInstructionsDetail;
using POSP.Core.Features.POSP.Commands.InsertTrainingMaterialDetail;
using POSP.Core.Features.POSP.Commands.InsertUserDeviceDetails;
using POSP.Core.Features.POSP.Commands.ResetPOSPUserAccountDetail;
using POSP.Core.Features.POSP.Commands.UpdateExamInstructionsDetail;
using POSP.Core.Features.POSP.Commands.UpdatePOSPExamQuestionAsweredDetail;
using POSP.Core.Features.POSP.Commands.UpdatePOSPTrainingDetail;
using POSP.Core.Features.POSP.Commands.UpdateTrainingInstructionsDetail;
using POSP.Core.Features.POSP.Commands.UpdateTrainingMaterialDetail;
using POSP.Core.Features.POSP.Queries.GetExamBannerDetail;
using POSP.Core.Features.POSP.Queries.GetExamCertificatedetail;
using POSP.Core.Features.POSP.Queries.GetExamDetail;
using POSP.Core.Features.POSP.Queries.GetExamInstructionsDetail;
using POSP.Core.Features.POSP.Queries.GetExamLanguageMaster;
using POSP.Core.Features.POSP.Queries.GetExamPaperDetail;
using POSP.Core.Features.POSP.Queries.GetExamParticularQuestionDetail;
using POSP.Core.Features.POSP.Queries.GetExamParticularQuestionStatus;
using POSP.Core.Features.POSP.Queries.GetExamQuestionPaperMaster;
using POSP.Core.Features.POSP.Queries.GetExamQuestionPaperOption;
using POSP.Core.Features.POSP.Queries.GetExamQuestionStatusMaster;
using POSP.Core.Features.POSP.Queries.GetFeedbackList;
using POSP.Core.Features.POSP.Queries.GetManagePOSPTraining;
using POSP.Core.Features.POSP.Queries.GetPanRejectionReasons;
using POSP.Core.Features.POSP.Queries.GetPOSPAgreementDocument;
using POSP.Core.Features.POSP.Queries.GetPOSPButtonDetail;
using POSP.Core.Features.POSP.Queries.GetPOSPCardDetail;
using POSP.Core.Features.POSP.Queries.GetPOSPExamQuestionNavigatorDetail;
using POSP.Core.Features.POSP.Queries.GetPospLastActivityDetails;
using POSP.Core.Features.POSP.Queries.GetPOSPMessageDetail;
using POSP.Core.Features.POSP.Queries.GetPOSPRating;
using POSP.Core.Features.POSP.Queries.GetPospReferralDetails;
using POSP.Core.Features.POSP.Queries.GetProductCategory;
using POSP.Core.Features.POSP.Queries.GetTrainingInstructionsDetail;
using POSP.Core.Features.POSP.Queries.GetTrainingMaterial;
using POSP.Core.Helpers;
using POSP.Domain.POSP;
using System.Net;
using System.Text;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;

namespace POSP.API.Controllers.v1;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[ApiController]
[ServiceFilter(typeof(ResponseCaptureFilter))]

public class POSPController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly IMongoDBService _mongodbService;
    private readonly IConverter _converter;
    private readonly ICustomUtility _customUtility;
    private readonly ILogger<POSPController> _logger;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public POSPController(IMediator mediatr, IMongoDBService mongodbService, IConverter converter, ICustomUtility customUtility, ILogger<POSPController> logger)
    {
        _mediatr = mediatr;
        _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        _customUtility = customUtility ?? throw new ArgumentNullException(nameof(customUtility));
        _logger = logger;
    }

    #region - GET Methods -

    #region - Get Training Instructions Detail -
    /// <summary>
    /// GetTrainingInstructionsDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetTrainingInstructionsDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetTrainingInstructionsDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetTrainingInstructionsDetailVm>>> GetTrainingInstructionsDetail(CancellationToken cancellationToken)
    {
        var req = new GetTrainingInstructionsDetailQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {

            //ob.Information_error.(target)

            var problemDetails = Result.CreateNotFoundError("test");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Training Material Detail -
    /// <summary>
    /// Get TrainingMaterialDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetTrainingMaterialDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPTrainingMaterialDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPTrainingMaterialDetailVm>>> GetTrainingMaterialDetail(string ModuleType, string TrainingId, CancellationToken cancellationToken)
    {

        var req = new GetPOSPCPTrainingMaterialQuery
        {
            ModuleType = ModuleType,
            TrainingId = TrainingId


        };
        //var req = new GetPOSPCPTrainingMaterialQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP Training Material Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Exam Instructions Detail -
    /// <summary>
    /// GetExamInstructionsDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetExamInstructionsDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetExamInstructionsDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetExamInstructionsDetailVm>>> GetExamInstructionsDetail(CancellationToken cancellationToken)
    {
        var req = new GetExamInstructionsDetailQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP Exam Instructions Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Exam Question Status Master -
    /// <summary>
    /// GetExamQuestionStatusMaster
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetExamQuestionStatusMaster")]
    [ProducesResponseType(typeof(IEnumerable<GetExamQuestionStatusMasterVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetExamQuestionStatusMasterVm>>> GetExamQuestionStatusMaster(CancellationToken cancellationToken)
    {
        var req = new GetExamQuestionStatusMasterQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Exam Question status master Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Exam Question Paper Master - 
    /// <summary>
    /// Get ExamQuestionPaperMaster
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetExamQuestionMaster")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPExamQuestionPaperMasterVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPExamQuestionPaperMasterVm>>> GetPOSPExamQuestionMaster(CancellationToken cancellationToken)
    {
        var req = new GetPOSPExamQuestionMasterQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP Configuration Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Exam Language Master - 
    /// <summary>
    /// Get ExamLanguageMaster
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("ExamLanguageMaster")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPExamLanguageMasterVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPExamLanguageMasterVm>>> GetPOSPExamLanguageMaster(CancellationToken cancellationToken)
    {
        var req = new GetPOSPExamLanguageMasterQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP Configuration Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Exam Paper Detail - 
    /// <summary>
    /// Get ExamPaperDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("ExamPaperDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPExamPaperDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPExamPaperDetailVm>>> GetPOSPExamPaperDetail(CancellationToken cancellationToken)
    {
        var req = new GetPOSPExamPaperDetailQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP Configuration Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Exam Question Paper Option -
    /// <summary>
    /// Get ExamQuestionPaperOption
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetExamQuestionPaperOption")]
    [ProducesResponseType(typeof(IEnumerable<GetExamQuestionPaperOptionMasterVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetExamQuestionPaperOptionMasterVm>>> GetExamQuestionPaperOption(CancellationToken cancellationToken)
    {
        var req = new GetExamQuestionPaperOptionlQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP ExamQuestionPaperOption Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get POSPTraining - 
    /// <summary>
    /// Get POSPTraining
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPOSPTraining")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPTrainingVm>), (int)HttpStatusCode.OK)]

    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPTrainingVm>>> GetPOSPTraining(CancellationToken cancellationToken)
    {
        var req = new GetPOSPTrainingQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP Training Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Exam Result Detail -
    /// <summary>
    /// GetExamResultDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetExamResultDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetExamDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetExamDetailVm>>> GetExamResultDetail(string Id, string UserId, CancellationToken cancellationToken)
    {
        var req = new GetExamDetailQueryHandler
        {
            Id = Id,
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP Exam Result Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get POSP Exam Particular Question Detail -
    /// <summary>
    /// GetPOSPExamParticularQuestionDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPOSPExamParticularQuestionDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPExamParticularQuestionDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPExamParticularQuestionDetailVm>>> GetPOSPExamParticularQuestionDetail(string UserId, string ExamId, int QuestionNo, CancellationToken cancellationToken)
    {
        var req = new GetPOSPExamParticularQuestionDetailQuery
        {
            UserId = UserId,
            ExamId = ExamId,
            QuestionNo = QuestionNo,
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Exam Particular Question Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get POSP Exam Quetion Navigator Detail -
    /// <summary>
    /// GetPOSPExamQuetionNavigatorDetail
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="examId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPOSPExamQuestionNavigatorDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPExamQuestionNavigatorDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPExamQuestionNavigatorDetailVm>>> GetPOSPExamQuestionNavigatorDetail(string UserId, string ExamId, CancellationToken cancellationToken)
    {
        var req = new GetPOSPExamQuestionNavigatorDetailQuery
        {
            UserId = UserId,
            ExamId = ExamId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Exam Question Navigator Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get POSP Message Detail - 
    /// <summary>
    /// GetPOSPMessageDetail
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="examId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPOSPMessageDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPMessageDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPMessageDetailVm>>> GetPOSPMessageDetail(string MessageKey, CancellationToken cancellationToken)
    {
        var req = new GetPOSPMessageDetailQuery
        {
            MessageKey = MessageKey
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Message Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Exam Banner Details -
    /// <summary>
    /// GetExamBannerDetails
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet()]
    [Route("GetExamBannerDetails")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetExamBannerDetailVm>>> GetBannerDetails(CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(new GetExamBannerDetailQuery(), cancellationToken);
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get POSP Exam Particular Question Status -
    /// <summary>
    /// GetPOSPExamParticularQuestionStatus
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetExamParticularQuestionStatus")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPExamParticularQuestionStatusVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPExamParticularQuestionStatusVm>>> GetPOSPExamParticularQuestionStatus(string ExamId, CancellationToken cancellationToken)
    {
        var req = new GetPOSPExamParticularQuestionStatusQuery
        {
            ExamId = ExamId,
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Exam Particular Question status not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Training Material Detail -
    /// <summary>
    /// Get TrainingMaterialDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPOSPButtonDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPButtonDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<ButtonResponseModel>>> GetPOSPButtonDetail(string UserId, CancellationToken cancellationToken)
    {

        var req = new GetPOSPButtonDetailQuery
        {
            UserId = UserId,

        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP Button not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get POSP Rating -
    /// <summary>
    /// GetPOSPRating
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPOSPRating")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPRatingVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPRatingVm>>> GetPOSPRating(string UserId, CancellationToken cancellationToken)
    {
        var req = new GetPOSPRatingQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Posp Rating not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Reset POSP Account Details -
    /// <summary>
    /// Reset POSP Account Details
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>  
    [HttpGet]
    [Route("ExamCertificate")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> ExamCertificate(string UserId, CancellationToken cancellationToken)
    {
        CertificacateCommand certificacateCommand = new CertificacateCommand();
        var htmls = string.Empty;
        var req = new GetExamCertificateDetailQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        var userdata = result.Result.FirstOrDefault();
        if (userdata.IsCleared == true && userdata.DocumentId != null)
        {
            htmls = await _mongodbService.MongoCertificataeDownload(userdata.DocumentId);
        }
        else
        {

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), @"Pages\");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);


            string[] OldFiles = Directory.GetFiles(directoryPath);

            // Print the file names
            foreach (string oldTempFiles in OldFiles)
            {
                string[] resultArray = oldTempFiles.Split(".");
                if (resultArray.Length > 0 && resultArray[resultArray.Length - 1].ToLower() == "pdf")
                {
                    DateTime creationTime = System.IO.File.GetCreationTimeUtc(oldTempFiles);
                    TimeSpan timeDiff = DateTime.Now - creationTime;
                    if (timeDiff.TotalMinutes > 3600)
                    {
                        try
                        {
                            System.IO.File.Delete(oldTempFiles);
                        }
                        catch (Exception exC)
                        {
                            _logger.LogInformation("Error In Deleting Temp File:-", oldTempFiles.ToString());
                        }
                    }
                }
            }


            var path = directoryPath + Guid.NewGuid() + "Certificate" + ".pdf";

            string HTMLTemplate = GetRowCertificateTemplate(directoryPath);  // Get HTML String

            HTMLTemplate = HTMLTemplate.Replace("{Name}", userdata.UserName);
            HTMLTemplate = HTMLTemplate.Replace("{State}", userdata.StateName);
            HTMLTemplate = HTMLTemplate.Replace("{POSPId}", userdata.POSPId);
            HTMLTemplate = HTMLTemplate.Replace("{Date}", DateTime.Now.ToShortDateString());
            HTMLTemplate = HTMLTemplate.Replace("{Start Date}", DateTime.Now.ToShortDateString());
            HTMLTemplate = HTMLTemplate.Replace("{End Date}", DateTime.Now.AddYears(1).ToShortDateString());
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = DinkToPdf.Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Left = 17, Top = 30 },
                DocumentTitle = "",
                Out = path
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = HTMLTemplate,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "styles.css") },
                FooterSettings = { FontName = "Arial", FontSize = 1, Line = false, Right = "", Spacing = 0 }
                // HeaderSettings = { FontName = "Arial", FontSize = 10, Line = false },
                //  FooterSettings = { FontName = "Arial", FontSize = 10, Line = true, Right = "Page [page] of [toPage]", Spacing = 5 }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            _converter.Convert(pdf);

            string mongoDbId;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            FileStream to_stream = new FileStream(path, FileMode.Open);
            mongoDbId = await _mongodbService.MongoCertificateUpload("Certificate", to_stream, bytes);
            certificacateCommand.UserId = UserId;
            certificacateCommand.DocumentId = mongoDbId;
            var certificatedetail = await _mediatr.Send(certificacateCommand, cancellationToken);
            //certificacateCommand.DocumentId = certificatedetail.Result.DocumentId;
            htmls = await _mongodbService.MongoCertificataeDownload(mongoDbId);
        }
        var res = Result.CreatePdfSuccess(htmls, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Agreement Document -
    [HttpGet]
    [Route("GetAgreementDocument")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> GetAgreementDocument(string UserId, CancellationToken cancellationToken)
    {
        InsertAgreementIdCommand insertAgreementIdCommand = new InsertAgreementIdCommand();
        var htmls = string.Empty;
        var req = new GetPOSPAgreementDocumentQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        var userdata = result.Result.FirstOrDefault();
        if (userdata.IsRevoked == 1)
        {
            var revoked = Result.CreateSuccess("Agreement is Revoked.", (int)HttpStatusCode.OK);
            return Ok(revoked);
        }
        if (userdata.AgreementId != null)
        {
            htmls = await _mongodbService.MongoAgreementDownload(userdata.AgreementId);
        }
        else if (userdata.PreSignedAgreementId != null)
        {
            htmls = await _mongodbService.MongoAgreementDownload(userdata.PreSignedAgreementId);
        }
        else
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), @"Pages\");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            var path = directoryPath + Guid.NewGuid() + "-Agreement" + ".pdf";
            string HTMLTemplate = GetRowAgreementTemplate(directoryPath);  // Get HTML String
            int age = 0;
            string userSign = await _mongodbService.MongoAgreementSignatureDownload(userdata.SignatureDocumentId);

            if (!string.IsNullOrEmpty(userdata.DateofBirth))
            {
                DateTime dt = _customUtility.ConvertToDateTime(userdata.DateofBirth);
                age = new DateTime(DateTime.Now.Subtract(dt).Ticks).Year - 1;
            }
            string fatherName = string.Empty;
            if (!string.IsNullOrWhiteSpace(userdata.FatherName) && !string.IsNullOrWhiteSpace(userdata.Gender) && userdata.Gender.ToLower() == "male")
                fatherName = "Son Of " + userdata.FatherName;
            else if (!string.IsNullOrWhiteSpace(userdata.FatherName) && !string.IsNullOrWhiteSpace(userdata.Gender) && userdata.Gender.ToLower() == "female")
                fatherName = "Daughter Of" + userdata.FatherName;
            string aadharNumber = string.Empty;
            if (!string.IsNullOrWhiteSpace(userdata.AadhaarNumber))
            {
                aadharNumber = userdata.AadhaarNumber.Substring(0, 4) + '-' + userdata.AadhaarNumber.Substring(4, 4) + '-' + userdata.AadhaarNumber.Substring(8, 4);
            }
            string addressLine1 = !string.IsNullOrWhiteSpace(userdata.AddressLine1) ? userdata.AddressLine1.ToUpper() : string.Empty;
            string addressLine2 = !string.IsNullOrWhiteSpace(userdata.AddressLine2) ? userdata.AddressLine2.ToUpper() : string.Empty;
            string cityName = !string.IsNullOrWhiteSpace(userdata.CityName) ? userdata.CityName.ToUpper() : string.Empty;
            string stateName = !string.IsNullOrWhiteSpace(userdata.StateName) ? userdata.StateName.ToUpper() : string.Empty;

            HTMLTemplate = HTMLTemplate.Replace("{POSPSTAMPNUMBER}", !string.IsNullOrWhiteSpace(userdata.StampNumber) ? userdata.StampNumber : string.Empty);
            HTMLTemplate = HTMLTemplate.Replace("{PANNUMBER}", userdata.PAN);
            HTMLTemplate = HTMLTemplate.Replace("{USERSIGN}", userSign);
            HTMLTemplate = HTMLTemplate.Replace("{DD/MM/YYYY}", DateTime.Now.ToString("dd/MM/yyyy"));
            HTMLTemplate = HTMLTemplate.Replace("{AADHAARNUMBER}", aadharNumber);
            HTMLTemplate = HTMLTemplate.Replace("{ADDRESS}", addressLine1 + ", " + addressLine2 + ", " +
                cityName + " - " + userdata.PinCode + ", " + stateName);
            HTMLTemplate = HTMLTemplate.Replace("{Age}", age == 0 ? string.Empty : Convert.ToString(age));
            HTMLTemplate = HTMLTemplate.Replace("{POSP_Name}", userdata.POSP_Name.ToUpper());
            HTMLTemplate = HTMLTemplate.Replace("{FatherName}", !string.IsNullOrWhiteSpace(fatherName) ? fatherName.ToUpper() : string.Empty); // Father Name
            HTMLTemplate = HTMLTemplate.Replace("{POSPIdentificationNumber}", userdata.POSPId); //POSPID

            //string pdfName = "Agreement";
            //var path = @"D:\HeroInsurance\POSP\POSP.API\Pages\" + pdfName + ".pdf";
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = DinkToPdf.Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Left = 17 },
                DocumentTitle = "",
                Out = path
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = HTMLTemplate,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "styles.css") },
                FooterSettings = { FontName = "Arial", FontSize = 1, Line = false, Right = "", Spacing = 0 }
                // HeaderSettings = { FontName = "Arial", FontSize = 10, Line = false },
                //  FooterSettings = { FontName = "Arial", FontSize = 10, Line = true, Right = "Page [page] of [toPage]", Spacing = 5 }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            _converter.Convert(pdf);

            string mongoDbId;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            FileStream to_stream = new FileStream(path, FileMode.Open);
            mongoDbId = await _mongodbService.MongoAgreementUpload("Agreement", to_stream, bytes);
            insertAgreementIdCommand.UserId = UserId;
            insertAgreementIdCommand.AgreementId = mongoDbId;
            insertAgreementIdCommand.ProcessType = "PreSignAgreement";
            var certificatedetail = await _mediatr.Send(insertAgreementIdCommand, cancellationToken);
            htmls = await _mongodbService.MongoAgreementDownload(mongoDbId); //insertAgreementIdCommand.AgreementId
        }
        var res = Result.CreatePdfSuccess(htmls, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Product Category -
    /// <summary>
    /// GetProductCategory
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetProductCategory")]
    [ProducesResponseType(typeof(IEnumerable<GetProductCategoryVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetProductCategoryVm>>> GetProductCategory(CancellationToken cancellationToken)
    {
        var req = new GetProductCategoryQuery { };
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

    #region - Get Posp Referral Details -
    /// <summary>
    /// GetPospReferralDetails
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPospReferralDetails")]
    [ProducesResponseType(typeof(IEnumerable<GetPospReferralDetailsVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPospReferralDetailsVm>>> GetPospReferralDetails(string? UserId, CancellationToken cancellationToken)
    {

        var req = new GetPospReferralDetailsQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP Referral Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get POSP Card Detail -
    /// <summary>
    /// Get UserDetail
    /// </summary>
    /// <param name="userId"></param>   
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPOSPCardDetail")]
    [ProducesResponseType(typeof(GetPOSPCardDetailVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<GetPOSPCardDetailVm>> GetPOSPCardDetail(CancellationToken cancellationToken)
    {
        var req = new GetPOSPCardDetailQuery();
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get POSP Profile -
    /// <summary>
    /// GetPOSPTestimonials
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("GetPOSPTestimonials")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPTestimonialsVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPTestimonialsVm>>> GetPOSPTestimonials(CancellationToken cancellationToken)
    {
        var req = new GetPOSPTestimonialsQuery { };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #endregion

    #region - POST Methods -

    #region - Insert Exam Instructions Detail -
    /// <summary>
    /// InsertExamInstructionsDetail
    /// </summary>
    /// <param name="insertExamInstructionsDetail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("InsertExamInstructionsDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> InsertExamInstructionsDetail(InsertExamInstructionsDetailCommand insertExamInstructionsDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(insertExamInstructionsDetailCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Exam instructions Detail Create failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Insert Training Instructions Detail - 
    /// <summary>
    /// InsertTrainingInstructionsDetail
    /// </summary>
    /// <param name="insertExamInstructionsDetail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("InsertTrainingInstructionsDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> InsertExamInstructionsDetail(InsertTrainingInstructionsDetailCommand inserttrainingInstructionsDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(inserttrainingInstructionsDetailCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Training instructions Detail Create failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Insert Training Material Detail - 
    /// <summary>
    /// InsertTrainingMaterialDetail
    /// </summary>
    /// <param name="insertTrainingMaterialDetailCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("InsertTrainingMaterialDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> InsertTrainingMaterialDetail(InsertTrainingMaterialDetailCommand insertTrainingMaterialDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(insertTrainingMaterialDetailCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Training material Detail Create failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Insert Exam Detail -
    [HttpPost]
    [Route("InsertExamDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> InsertExamDetail(string UserId, string ExamStatus, CancellationToken cancellationToken)
    {

        var req = new InsertExamDetailCommand
        {
            UserId = UserId,
            ExamStatus = ExamStatus
        };

        var result = await _mediatr.Send(req, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Training instructions Detail Create failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Insert POSP Training Detail - 
    /// <summary>
    /// InsertPOSPTrainingDetail
    /// </summary>
    /// <param name="updatePOSPTrainingDetailCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("InsertPOSPTrainingDetail")]
    [ProducesResponseType(typeof(POSPResponseTrainingModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<POSPResponseTrainingModel>> InsertPOSPTrainingDetail(string UserId, string TrainingStatus, string TrainingId, CancellationToken cancellationToken)
    {
        var req = new InsertPOSPTrainingDetailCommand
        {
            UserId = UserId,
            TrainingStatus = TrainingStatus,
            TrainingId = TrainingId
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

    #region - Insert POSP Training Progress Detail - 
    /// <summary>
    /// InsertPOSPTrainingProgressDetail
    /// </summary>
    /// <param name="insertPOSPTrainingProgressDetailCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("InsertPOSPTrainingProgressDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> InsertPOSPTrainingProgressDetail(InsertPOSPTrainingProgressDetailCommand insertPOSPTrainingProgressDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(insertPOSPTrainingProgressDetailCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Training progress Detail Create failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Update POSP Training Detail -
    /// <summary>
    /// UpdatePOSPTrainingDetail
    /// </summary>
    /// <param name="updatePOSPTrainingDetailCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("UpdatePOSPTrainingDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UpdatePOSPTrainingDetail(UpdatePOSPTrainingDetailCommand updatePOSPTrainingDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(updatePOSPTrainingDetailCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Training Detail Update failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Update Training Material Detail -
    /// <summary>
    /// UpdateTrainingMaterialDetail
    /// </summary>
    /// <param name="updateTrainingMaterialDetailCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("UpdateTrainingMaterialDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UpdateTrainingMaterialDetail(UpdateTrainingMaterialDetailCommand updateTrainingMaterialDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(updateTrainingMaterialDetailCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Training material Detail Update failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Update ExamInstructionsDetail -
    /// <summary>
    /// Update ExamInstructionsDetail
    /// </summary>
    /// <param name="updatetrainingInstructionsDetail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("UpdateTrainingInstructionsDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UpdateExamInstructionsDetail(UpdateTrainingInstructionsDetailCommand updatetrainingInstructionsDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(updatetrainingInstructionsDetailCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Training instructions Detail Create failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Update Exam Instructions Detail -
    /// <summary>
    /// UpdateExamInstructionsDetail
    /// </summary>
    /// <param name="examInstructionsDetailCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("UpdateExamInstructionsDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UpdateExamInstructionsDetail(UpdateExamInstructionsDetailCommand updateExamInstructionsDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(updateExamInstructionsDetailCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Exam Instructions Detail Update failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Update POSP Exam Question Aswered Detail -
    /// <summary>
    /// UpdatePOSPExamQuestionAsweredDetail
    /// </summary>
    /// <param name="updatePOSPExamQuestionAsweredDetailCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("UpdatePOSPExamQuestionAsweredDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UpdatePOSPExamQuestionAsweredDetail(UpdatePOSPExamQuestionAsweredDetailCommand updatePOSPExamQuestionAsweredDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(updatePOSPExamQuestionAsweredDetailCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Exam Question Answered Detail Update failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Upload Exam Banner -
    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="banneruploadcommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost()]
    [Route("UploadExamBanner")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UploadExamBanner(IFormFile files, [FromForm] ExamBannerUploadCommand banneruploadcommand, CancellationToken cancellationToken)
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Services/Files");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        var fileNameWithPath = Path.Combine(path, files.FileName);
        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
        {
            files.CopyTo(stream);
        }
        byte[] byteimage = System.IO.File.ReadAllBytes(fileNameWithPath);

        System.IO.File.Delete(fileNameWithPath);
        var dname = Path.GetDirectoryName(fileNameWithPath);
        Directory.Delete(dname, true);
        banneruploadcommand.BannnerImage = byteimage;
        banneruploadcommand.BannerFileName = files.FileName;
        banneruploadcommand.BannerStoragePath = fileNameWithPath;
        var result = await _mediatr.Send(banneruploadcommand, cancellationToken);
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
    #endregion

    #region - Reset POSP Account Details -
    /// <summary>
    /// Reset POSP Account Details
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>  
    [HttpPost]
    [Route("ResetPOSPUserAccountDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> ResetPOSPUserAccountDetail(string UserId, CancellationToken cancellationToken)
    {
        var req = new ResetPOSPUserAccountDetailCommand
        {
            UserId = UserId,
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP User reset account process failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Insert POSP Rating -
    /// <summary>
    /// InsertPOSPRating
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="Rating"></param>
    /// <param name="Description"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("InsertPOSPRating")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<POSPRatingResponseModel>> InsertPOSPRating(InsertPOSPRatingCommand insertPOSPRatingCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(insertPOSPRatingCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP Rating Create failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Upload Certificate Template -
    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="banneruploadcommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost()]
    [Route("UploadCertificateTemplate")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UploadCertificateTemplate(IFormFile files, [FromForm] ExamCertificateUploadCommand examcertificateuploadcommand, CancellationToken cancellationToken)
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Services/Files");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        var fileNameWithPath = Path.Combine(path, files.FileName);
        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
        {
            files.CopyTo(stream);
        }
        byte[] byteimage = System.IO.File.ReadAllBytes(fileNameWithPath);

        //var html = System.IO.File.ReadAllText("D:/hero/Hero.Insurance.API/Hero.Insurance.API/POSP/POSP.API/Pages/Certificate.html");
        //byte[] bytes = Encoding.ASCII.GetBytes(html);
        System.IO.File.Delete(fileNameWithPath);
        var dname = Path.GetDirectoryName(fileNameWithPath);
        Directory.Delete(dname, true);
        examcertificateuploadcommand.BannnerImage = byteimage;
        //examcertificateuploadcommand.BannnerImage = bytes;
        examcertificateuploadcommand.BannerFileName = files.FileName;
        //banneruploadcommand.BannerStoragePath = fileNameWithPath;
        var result = await _mediatr.Send(examcertificateuploadcommand, cancellationToken);
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
    #endregion

    #region - Sign POSP Agreement -
    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="banneruploadcommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost()]
    [Route("SignPOSPAgreement")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> SignPOSPAgreement(IFormFile files, [FromForm] AgreementUploadCommand agreementuploadcommand, CancellationToken cancellationToken)
    {

        InsertAgreementIdCommand insertAgreementIdCommand = new InsertAgreementIdCommand();
        var getPOSPAgreementDocumentQuery = new GetPOSPAgreementDocumentQuery
        {
            UserId = agreementuploadcommand.UserId
        };
        var documentQuery = await _mediatr.Send(getPOSPAgreementDocumentQuery, cancellationToken);
        var documentQuery_userdata = documentQuery.Result.FirstOrDefault();

        if (documentQuery_userdata.IsRevoked == 1)
        {
            var problemDetails = Result.CreateNotFoundError("Agreement is Revoked.");
            return NotFound(problemDetails);
        }
        if (files != null)
        {
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "Services/Files");
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            var fileNameWithPath = Path.Combine(tempPath, files.FileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                files.CopyTo(stream);
            }
            byte[] byteimage = System.IO.File.ReadAllBytes(fileNameWithPath);

            System.IO.File.Delete(fileNameWithPath);
            var dname = Path.GetDirectoryName(fileNameWithPath);
            Directory.Delete(dname, true);
            agreementuploadcommand.SignatureImage = byteimage;

        }
        var result = await _mediatr.Send(agreementuploadcommand, cancellationToken);
        var htmls = string.Empty;
        documentQuery = await _mediatr.Send(getPOSPAgreementDocumentQuery, cancellationToken);
        documentQuery_userdata = documentQuery.Result.FirstOrDefault();
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), @"Pages\");
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        string[] OldFiles = Directory.GetFiles(directoryPath);

        // Print the file names
        foreach (string oldTempFiles in OldFiles)
        {
            string[] resultArray = oldTempFiles.Split(".");
            if (resultArray.Length > 0 && resultArray[resultArray.Length - 1].ToLower() == "pdf")
            {
                DateTime creationTime = System.IO.File.GetCreationTimeUtc(oldTempFiles);
                TimeSpan timeDiff = DateTime.Now - creationTime;
                if (timeDiff.TotalMinutes > 3600)
                {
                    try
                    {
                        System.IO.File.Delete(oldTempFiles);
                    }
                    catch (Exception exC)
                    {
                        _logger.LogInformation("Error In Deleting Temp File:-", oldTempFiles.ToString());
                    }
                }
            }
        }


        var path = directoryPath + Guid.NewGuid() + "-Agreement" + ".pdf";
        string HTMLTemplate = GetRowAgreementTemplate(directoryPath);  // Get HTML String
        int age = 0;
        string userSign = await _mongodbService.MongoAgreementSignatureDownload(documentQuery_userdata.SignatureDocumentId);

        if (!string.IsNullOrEmpty(documentQuery_userdata.DateofBirth))
        {
            DateTime dt = _customUtility.ConvertToDateTime(documentQuery_userdata.DateofBirth);
            age = new DateTime(DateTime.Now.Subtract(dt).Ticks).Year - 1;
        }
        string fatherName = string.Empty;
        if (!string.IsNullOrWhiteSpace(documentQuery_userdata.FatherName) && !string.IsNullOrWhiteSpace(documentQuery_userdata.Gender) && documentQuery_userdata.Gender.ToLower() == "male")
            fatherName = "Son Of " + documentQuery_userdata.FatherName;
        else if (!string.IsNullOrWhiteSpace(documentQuery_userdata.FatherName) && !string.IsNullOrWhiteSpace(documentQuery_userdata.Gender) && documentQuery_userdata.Gender.ToLower() == "female")
            fatherName = "Daughter Of" + documentQuery_userdata.FatherName;
        string aadharNumber = string.Empty;
        if (!string.IsNullOrWhiteSpace(documentQuery_userdata.AadhaarNumber))
        {
            aadharNumber = documentQuery_userdata.AadhaarNumber.Substring(0, 4) + '-' + documentQuery_userdata.AadhaarNumber.Substring(4, 4) + '-' + documentQuery_userdata.AadhaarNumber.Substring(8, 4);
        }
        string addressLine1 = !string.IsNullOrWhiteSpace(documentQuery_userdata.AddressLine1) ? documentQuery_userdata.AddressLine1.ToUpper() : string.Empty;
        string addressLine2 = !string.IsNullOrWhiteSpace(documentQuery_userdata.AddressLine2) ? documentQuery_userdata.AddressLine2.ToUpper() : string.Empty;
        string cityName = !string.IsNullOrWhiteSpace(documentQuery_userdata.CityName) ? documentQuery_userdata.CityName.ToUpper() : string.Empty;
        string stateName = !string.IsNullOrWhiteSpace(documentQuery_userdata.StateName) ? documentQuery_userdata.StateName.ToUpper() : string.Empty;

        HTMLTemplate = HTMLTemplate.Replace("{POSPSTAMPNUMBER}", !string.IsNullOrWhiteSpace(documentQuery_userdata.StampNumber) ? documentQuery_userdata.StampNumber : string.Empty);
        HTMLTemplate = HTMLTemplate.Replace("{PANNUMBER}", documentQuery_userdata.PAN);
        HTMLTemplate = HTMLTemplate.Replace("{USERSIGN}", userSign);
        HTMLTemplate = HTMLTemplate.Replace("{DD/MM/YYYY}", DateTime.Now.ToString("dd/MM/yyyy"));
        HTMLTemplate = HTMLTemplate.Replace("{AADHAARNUMBER}", aadharNumber);
        HTMLTemplate = HTMLTemplate.Replace("{ADDRESS}", addressLine1 + ", " + addressLine2 + ", " +
            cityName + " - " + documentQuery_userdata.PinCode + ", " + stateName);
        HTMLTemplate = HTMLTemplate.Replace("{Age}", age == 0 ? string.Empty : Convert.ToString(age));
        HTMLTemplate = HTMLTemplate.Replace("{POSP_Name}", documentQuery_userdata.POSP_Name.ToUpper());
        HTMLTemplate = HTMLTemplate.Replace("{FatherName}", !string.IsNullOrWhiteSpace(fatherName) ? fatherName.ToUpper() : string.Empty); // Father Name
        HTMLTemplate = HTMLTemplate.Replace("{POSPIdentificationNumber}", documentQuery_userdata.POSPId); //POSPID


        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = DinkToPdf.Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Left = 17 },
            DocumentTitle = "",
            Out = path
        };
        var objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = HTMLTemplate,
            WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "styles.css") },
            FooterSettings = { FontName = "Arial", FontSize = 1, Line = false, Right = "", Spacing = 0 }
            // HeaderSettings = { FontName = "Arial", FontSize = 10, Line = false },
            //  FooterSettings = { FontName = "Arial", FontSize = 10, Line = true, Right = "Page [page] of [toPage]", Spacing = 5 }
        };
        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };
        _converter.Convert(pdf);

        string mongoDbId;
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        FileStream to_stream = new FileStream(path, FileMode.Open);
        mongoDbId = await _mongodbService.MongoAgreementUpload("Agreement", to_stream, bytes);

        insertAgreementIdCommand.UserId = agreementuploadcommand.UserId;
        insertAgreementIdCommand.AgreementId = mongoDbId; //id;
        insertAgreementIdCommand.ProcessType = "Agreement";
        var certificatedetail = await _mediatr.Send(insertAgreementIdCommand, cancellationToken);


        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP agreement Create failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #endregion

    #region - DELETE Methods -

    #region - Delete Training Instructions Detail -
    /// <summary>
    /// DeleteTrainingInstructionsDetail
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("DeleteTrainingInstructionsDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> DeleteTrainingInstructionsDetail(string Id, CancellationToken cancellationToken)
    {
        var req = new DeleteTrainingInstructionsDetailQuery
        {
            Id = Id
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Training Instructions Detail deleted failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Delete Exam Instructions Detail - 
    /// <summary>
    /// DeleteExamInstructionsDetail
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("DeleteExamInstructionsDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> DeleteExamInstructionsDetail(string Id, CancellationToken cancellationToken)
    {
        var req = new DeleteExamInstructionsDetailQuery
        {
            Id = Id
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

    #region - Delete Training Material Detail -
    /// <summary>
    /// DeleteTrainingMaterialDetail
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("DeleteTrainingMaterialDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> DeleteTrainingMaterialDetail(string Id, CancellationToken cancellationToken)
    {
        var req = new DeleteTrainingMaterialDetailQuery
        {
            Id = Id
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Training Material Detail deleted failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #endregion

    #region - Extra Methods -

    #region - Get Row Agreement HTML Template -
    /// <summary>
    /// Get Row Agreement HTML Template
    /// </summary>
    /// <returns></returns>
    private string GetRowAgreementTemplate(string directoryPath)
    {
        directoryPath += @"assets\";
        string logoB64String = GetBase64StringOfPath(directoryPath + "sign.png");
        var sb = new StringBuilder();
        sb.Append(@"<!DOCTYPE html>
                    <html lang='en'>
                    <head>
	                    <style>
		                    * {
                        padding: 0;
                        margin: 0;
                        font-family: 'Rubik', sans-serif;
                        font-size: 18px;
                    }
                    .textRed {
                        color: #cc0202;
                    }
                    .textMaroon {
                        color: #800000;
                    }
                    .underline {
                        text-decoration: underline;
                    }
                    .text500 {
                        font-weight: 500;
                    }
                    .textJustify {
                        text-align: justify;
                    }
                    .textBold {
                        font-weight: bold;
                    }
                    .lineHeight4 {
                        line-height: 4;
                    }
                    .borderBottom {
                        border-bottom: 2px solid;
                    }
                    .textCenter {
                        text-align: center;
                    }
                    .marginTop24 {
                        margin-top: 24px;
                    }
                    .containerDiv {
                        width: 780px;
                        padding: 50px 40px;
                        margin: auto;
                    }
                    .stampNumber {
                        text-align: end;
                    }
                    .posTitle {
                        text-align: center;
                        margin-top: 20px;
                    }
                    .nameAndPan {
                        min-width: 100px;
                        border-color: #cc0202;
                        display: inline-block;
                        line-height: 0.8;
                        padding-bottom: 5px;
                    }
                    .name {
                        min-width: 260px;
                        border-color: #cc0202;
                        display: inline-block;
                        line-height: 0.8;
                        padding-bottom: 5px;
                    }
                    .ageAndMore {
                        min-width: 145px;
                        border-color: #cc0202;
                        display: inline-block;
                        line-height: 0.8;
                        padding-bottom: 5px;
                    }
                    .fatherName {
                        min-width: 400px;
                        border-color: #cc0202;
                        display: inline-block;
                        line-height: 0.8;
                        padding-bottom: 5px;
                    }
                    .address {
                        display: inline-block;
                        min-width: 500px;
                        border-color: #cc0202;
                        line-height: 0.8;
                        padding-bottom: 5px;
                    }
                    .whereas ol {
                        padding-left: 20px;
                    }
                    .whereas ol li {
                        padding-bottom: 20px;
                        text-align: justify;
                        padding-left: 25px;
                        padding-top: 15px;
                    }
                    .whereas ol li:last-child {
                        padding-bottom: 0px;
                    }

                    .page2 {
                        padding-top: 30px;
                    }

                    ol li p {
                        margin-top: 20px;
                        text-align: justify;
                    }
                    .marginRight15 {
                        margin-right: 20px;
                    }
                    .itemContent {
                        margin-left: -20px;
                        line-height: 1.3;
                    }
                    .itemContent p {
                        margin-bottom: 20px;
                        text-align: justify;
                    }
                    .itemContentDouble {
                        margin-left: -30px;
                        line-height: 1.3;
                    }
                    .itemContentDouble p {
                        text-align: justify;
                    }
                    .itemContentFib {
                        border-bottom: 2px solid #cc0202;
                        display: inline-block;
                        min-width: 260px;
                        line-height: 0.8;
                        padding-bottom: 5px;
                    }
                    .itemContentDate {
                        display: inline-block;
                        min-width: 250px;
                        border-bottom: 2px solid #cc0202;
                        line-height: 0.8;
                        padding-bottom: 5px;
                    }
                    .seventhSubItem ol li {
                        padding-top: 0px;
                        padding-left: 60px;
                    }
                    .seventhSubItem ol li:last-child {
                        padding-bottom: 20px;
                    }
                    .borderClass{
                        border: 2px solid #000;
                        /* height: 200px; */
                        width: 90%;
                    }
                    .annexure{
                        margin-top: 30px;
                    }
                    .annex p{
                        padding-bottom: 10px;
                    }
                    .annex p:last-child{
                        padding-bottom: 0;
                    }
                    .annexure ol li{
                        padding-bottom: 10px;
                        padding-left: 20px;
                    }
                    .annex ol li:last-child{
                        padding-bottom: 0;
                    }
                    th,
                            td {
                                text-align: left;
                                padding: 8px;
                                border-bottom: 2px solid black;
                                }
                    .textJustify{
                    text-align:justify;
                            }
	                    </style>
                        <meta charset='UTF-8'>
                        <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <link
                            href='https://fonts.googleapis.com/css2?family=Rubik:ital,wght@0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap'
                            rel='stylesheet'>
                    </head>
                    <body>
                        <div class='containerDiv'>
                            <!-- Page 1 content -->
                            <div class='page1'>
                                <p class='textMaroon stampNumber text500'>{POSPSTAMPNUMBER}</p>
                                <p class='underline text500 posTitle'><b>POINT OF SALE AGREEMENT</b></p>
                                <p class='marginTop24'>This Point of Sale Agreement (hereinafter referred to as the “Agreement”) is executed
                                    as on</p>
                                <p class='textMaroon text500 marginTop24'>{DD/MM/YYYY}</p>
                                <p class='text500 marginTop24 textCenter'><b>BY AND BETWEEN</b></p>
                                <p class='marginTop24 lineHeight4 textJustify'>
                                <span class='borderBottom nameAndPan textCenter textMaroon'>{POSP_Name}</span>
                                <span> Name</span>
                                <span class='borderBottom nameAndPan textCenter textMaroon'>{PANNUMBER}</span>
                                <span> ,PAN No</span>  
                                <span class='borderBottom nameAndPan textCenter textMaroon'>{AADHAARNUMBER}</span>
                                <span> /AADHAR Card No</span>
                                <span class='borderBottom ageAndMore textCenter textMaroon'>{Age}</span>
                                <span> aged ,</span>
                                <span class='borderBottom fatherName textCenter textMaroon'>{FatherName}</span>.
                                <span>  having place of residence at</span> 
                                <span class='borderBottom textMaroon address textCenter'>{ADDRESS}</span>
                                </p>

                                <p class='marginTop24 textJustify'>
                                    (hereinafter referred to as the '<span class='text500'>POS</span>', which expression shall unless
                                    repugnant or contrary to the
                                    context or meaning thereof shall be deemed to mean and include its heirs, successors and permitted
                                    assigns) of the <span class='text500'>ONE PART</span>.
                                </p>
                                <p class='marginTop24 text500 textCenter'>AND</p>
                                <p class='marginTop24 textJustify'>
                                    <span class='text500'>HERO INSURANCE BROKING INDIA PRIVATE LIMITED</span>,
                                    a company incorporated under the provisions of the Companies Act, 1956 having Registered Office at 264,
                                    Okhla
                                    Industrial Estate, Phase III, New Delhi – 110020, India (hereinafter referred to as the
                                    '<span class='text500'>Company</span>', which
                                    expression shall unless repugnant or contrary to the context or meaning thereof shall be deemed to mean
                                    and include its executors and permitted assigns) of the <span class='text500'>OTHER PART</span>.
                                </p>
                                <p class='marginTop24'>POS and Company under this Agreement hereinafter individually referred to as the
                                    '<span class='text500'>Party</span>' and collectively
                                    referred to as the '<span class='text500'>Parties</span>'.
                                </p>
                                <p class='marginTop24 text500 textJustify'>WHEREAS</p>
                                <div class='whereas marginTop24'>
                                    <ol>
                                        <li>The Company is registered with the Insurance Regulatory and Development Authority of India to
                                            act as Direct Insurance Broker. </li>
                                        <li>The POS is desirous of being associated with the Company and has approached the Company to be
                                            appointed as a Point-Of-Sale Person within the meaning of Guidelines on Point Of Salesperson –
                                            Life, Non-Life and Health Insurance for soliciting and marketing pre underwritten products
                                            approved by IRDAI as more specifically described under Annexure-1 to this agreement. </li>
                                    </ol>
                                </div>
                            </div>

                            <!-- Page 2 content -->
                            <div class='page2'>
                                <p class='text500 textJustify'>NOW THIS AGREEMENT WITNESSETH AND IT IS HEREBY AGREED BY AND BETWEEN THE
                                    PARTIES HERETO AS FOLLOW</p>
                                <div class='whereas marginTop24'>
                                    <ol style='list-style-type: decimal;'>
                                        <li>
                                            <span class='text500'>DEFINITION</span>
                                        </li>
                                        <div class='itemContent'>
                                            <div style='display: flex;'>
                                                <p>
                                                    1.1. '<span class='text500'>Applicable Laws</span>' means any statute, regulations,
                                                    guidelines,
                                                    rules, notification and
                                                    circular prescribed by IRDAI from time to time including but not limited to the
                                                    Insurance
                                                    Act, 1938 as amended by Insurance Laws( Amendment) Act 2015, the Insurance Rules, 1939,
                                                    the
                                                    Insurance Regulatory and Development Authority Act, 1999 and any other Indian Laws as
                                                    may be
                                                    applicable.
                                                </p>
                                            </div>

                                            <div style='display: flex;'>
                                                <p>
                                                    1.2. '<span class='text500'>Guidelines</span>' means Guidelines on Point of Sales Person and
                                                    any
                                                    subsequent modifications issued by IRDAI.
                                                </p>
                                            </div>

                                            <div style='display: flex;'>
                                                <p>
                                                    1.3. '<span class='text500'>NIELIT</span>' means National Institute of Electronics and
                                                    Information Technology which is an autonomous scientific society of Department of
                                                    Electronics and Information Technology, Government of India and is designated as the
                                                    training and examination body for the “Point of Sales Persons”.
                                                </p>
                                            </div>

                                            <div style='display: flex;'>
                                                <p>
                                                    1.4. '<span class='text500'>IRDAI</span>' mmeans the Insurance Regulatory and Development
                                                    Authority of India established under Section 3 of the IRDA Act, 1999.
                                                </p>
                                            </div>

                                            <div style='display: flex;'>
                                                <p>
                                                    '<span class='text500'>1.5. Point of Sales Person</span>'
                                                    means an individual who possesses the minimum
                                                    qualifications as defined under the Guidelines and has undergone training and has passed
                                                    the
                                                    examination as specified in the Guidelines and is authorized to solicit and market the
                                                    pre –
                                                    underwritten products approved by IRDAI and as specified in Annexure 1.
                                                </p>
                                            </div>
                                        </div>

                                        <li>
                                            <span class='text500'>APPOINTMENT OF POS AS A POINT OF SALES PERSON</span>
                                        </li>
                                        <div class='itemContent'>
                                            <div style='display: flex;'>
                                                <p>
                                                    2.1. The Company hereby appoints <span class='textMaroon itemContentFib textCenter'>{POSP_Name}</span> as its POS and
                                                    <span class='textMaroon itemContentFib textCenter'>{POSP_Name}</span> hereby accepts the
                                                    appointment offered by the Company to act as the POS of the Company to solicit and
                                                    market
                                                    pre underwritten products as approved by IRDAI and specifically mentioned under Annexure
                                                    – 1
                                                    to this Agreement.
                                                </p>
                                            </div>

                                            <div style='display: flex;'>
                                                <p>
                                                    2.2. This appointment of <span class='textMaroon itemContentFib textCenter'>{POSP_Name}</span> shall be without
                                                    prejudice
                                                    to
                                                    the right of the Company to appoint other POS.
                                                </p>
                                            </div>
                                        </div>

                                        <li>
                                            <span class='text500'>EFFECTIVE DATE AND TERM</span>
                                        </li>
                                        <div class='itemContent'>
                                            <div style='display: flex;'>
                                                <p>
                                                    3.1. This Agreement shall be effective from <span class='itemContentDate textMaroon textCenter'>{DD/MM/YYYY}</span> and shall
                                                    be valid unless
                                                    terminated by the Company in accordance with the terms and conditions contained in
                                                    this Agreement.
                                                </p>
                                            </div>
                                        </div>

                                        <li>
                                            <span class='text500'>CONSIDERATION</span>
                                        </li>
                                        <div class='itemContent'>
                                            <div style='display: flex;'>
                                                <p>
                                                    4.1. The Company shall pay consideration to the POS.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    4.2. The Commission shall be subject to deduction of applicable taxes including cess,
                                                    surcharge or similar taxes thereon under the applicable laws in force from time to time.
                                                </p>
                                            </div>
                                        </div>

                                        <li>
                                            <span class='text500'>REPRESENTATIONS AND WARRANTIES OF POINT OF SALE PERSON (POS)</span>
                                        </li>
                                        <div class='itemContent'>
                                            <div style='display: flex;'>
                                                <p>
                                                    5.1. POS warrants that the POS shall adhere to the Guidelines and Anti Money Laundering
                                                    Guidelines issued by IRDAI and any subsequent amendments thereof as issued by IRDAI from
                                                    time to time and the any other Applicable laws.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    5.2. POS warrants that the POS shall comply with Code of Conduct as mentioned under
                                                    <span class='text500'>Annexure – 2</span> of this Agreement and as revised by the
                                                    Company from time to
                                                    time.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    5.3. POS warrants that the POS holds a valid PAN Card and / or Aadhaar card as issued by the
                                                    government authorities.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    5.4. POS shall use only such approved marketing/ sales material provided to him by the
                                                    Company.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    5.5. POS warrants that the POS shall not obtain/ seek/ provide/give undue favors or gift
                                                    from/ to any employee of the Company in connection with any matter or dealing concerning
                                                    the Company or otherwise.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    5.6. POS shall adhere to Section 64VB of the Insurance Act and shall deposit with, or
                                                    dispatch by post to the Company, the premium collected by the POS in full and without
                                                    deduction of any commission within twenty four hours of the collection excluding bank
                                                    and postal holidays.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    5.7. POS warrants that the POS is not inducted as a POS by any other Insurance Company or
                                                    Insurance Intermediary and shall be associated with the Company on an exclusive basis.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    5.8. POS warrants that the POS is not directly or indirectly associated with any other
                                                    Insurance Entity in any capacity.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    5.9. POS warrants that the POS possess the prerequisite minimum qualifications as prescribed
                                                    under the Guidelines and further does not suffer from any disqualifications as mentioned
                                                    in the Section 42(3) of the IRDA Act, 1999.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    5.10. POS warrants that the POS has successfully completed the training and cleared the
                                                    examination conducted by authorized agency and holds a valid pass certificate / letter
                                                    issued by the company/IRDAI recognized institute certifying the POS to be a Point of
                                                    Sales Person.
                                                </p>
                                            </div>
                                        </div>
                                        <li>
                                            <span class='text500'>CONFIDENTIAL INFORMATION</span>
                                        </li>
                                        <div class='itemContent'>
                                            <div style='display: flex;'>
                                                <p>
                                                    6.1. All confidential and proprietary information, whether oral or written made available to
                                                    the POS during the term of this Agreement, shall be received in confidence
                                                    (“Confidential Information”). If any Confidential Information is disclosed orally, then
                                                    the same shall be reduced in writing within 7(Seven) days of such disclosure. Unless
                                                    otherwise specified, Confidential Information shall include all nonpublic information
                                                    furnished, disclosed or transmitted regardless of its form of disclosure.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    6.2. The POS may disclose the Confidential Information on a need to know basis after
                                                    obtaining prior written notice from the Company. The POS shall not disclose or use the
                                                    same for any other purpose, except for complying with its obligations under this
                                                    Agreement.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    6.3. Upon termination of this Agreement or on a demand from the Company, the POS shall
                                                    promptly return to the Company, all correspondences, documents and all materials or
                                                    items belonging to the Company.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    6.4. In addition to the obligations of confidentiality provided hereunder, the Company may
                                                    require the POS to execute a Non-Disclosure Agreement (“NDA”). On the execution of such
                                                    NDA, the obligations of confidentiality and non-disclosure therein shall be in addition
                                                    to the obligations hereunder.
                                                </p>
                                            </div>
                                        </div>
                                        <li>
                                            <span class='text500'>INDEMNITY AND PENALTY</span>
                                        </li>
                                        <div class='itemContent'>
                                            <div style='display: flex;'>
                                                <p>
                                                    7.1. The POS shall be subject to such penalty as prescribed under Section 102 of the
                                                    Insurance Act or any other Applicable Laws for any misconduct or any act or omissions on
                                                    the part of the POS.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    7.2. POS agrees to indemnify, defend and hold harmless the Company against any losses,
                                                    damages, costs and loss of reputation suffered by the Company due to.
                                                </p>
                                            </div>
                                            <div class='seventhSubItem'>
                                                <ol style='list-style-type: lower-alpha;'>
                                                    <li>breach of any obligation or term under this Agreement or of any applicable laws;
                                                    </li>
                                                    <li>the acts, errors, representations, misrepresentations, willful misconduct or
                                                        negligence by the POS in performance of the obligation under this Agreement;</li>
                                                    <li>Misrepresentation and unfair practices made by the POS;</li>
                                                    <li>acts and omissions of the POS and violation of Code of Conduct specified under the
                                                        Guidelines;</li>
                                                </ol>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    7.3. The Company shall not hold any liability to indemnify the POS on non-compliance of
                                                    rules, regulations, Guidelines, Circulars issued by IRDAI from time to time.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    7.4. All the indemnities shall survive the termination or expiry of this Agreement.
                                                </p>
                                            </div>
                                        </div>
                                        <li>
                                            <span class='text500'>TERMINATION</span>
                                        </li>
                                        <div class='itemContent'>
                                            <div style='display: flex;'>
                                                <p>
                                                    8.1. The Parties may terminate this Agreement by serving a prior written notice of 30 days
                                                    without assigning any reason on the other Party.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    8.2. This Agreement will terminate automatically upon the occurrence of any of the following
                                                    events by POSP, and upon such occurrence the parties shall be obligated to make only
                                                    those payments the right to which accrued to the date of termination: • Failure of the
                                                    POSP to attend the in-house training session as conducted by the Company; • Failure of
                                                    the POSP to clear the examination as conducted by Company; • Conviction of a felony by
                                                    POSP; • Misappropriation (or failure to remit) any funds or property due the Company
                                                    from POSP; • Determination that POSP is not in compliance with Company underwriting
                                                    guidelines or the terms of this Agreement and POSP has failed to correct the problem
                                                    within 10 days of the Company providing written notice of same; • In the event of fraud
                                                    or material breach of any of the conditions or provisions of this Agreement on the part
                                                    of either party, the other party may terminate the Agreement immediately upon written
                                                    notice. • Fails to comply with directions of the Company. • Furnish wrong information or
                                                    conceals the information or fails to disclose the material facts of the policy to the
                                                    policy holder. • Fails to resolve complaints, unless the circumstances are beyond his
                                                    control, emanating from the business procured by him and persons he deals with •
                                                    Indulges in inducement in cash or kind with client or any other insurance
                                                    intermediary/agent/insurer. • Fails to pay any penalty levied on his account. • Fails to
                                                    carry out his obligations as prescribed in the agreement and in the provisions 5 of:
                                                    Act/regulations/circulars or guidelines by IRDAI from time to time. • Acts in a manner
                                                    prejudicial to the interest of the company or the client • Acts in a manner that amounts
                                                    to diverting funds of his Group/Affiliates or associates rather than engaging in the
                                                    activity of soliciting and servicing insurance business • Is found guilty of fraud or is
                                                    charged or convicted in any criminal act. • Indulges in any other misconduct. e)
                                                    Agreement shall automatically terminate if the POSP acquires a license as or becomes
                                                    related to, an insurance company, insurance agent, corporate agent, a micro-insurance
                                                    agent, TPA, Surveyor, Referral partner or loss assessor. Upon contravention of this
                                                    Clause 5(e) by the POSP, the POSP shall be liable to indemnify the Company to the extent
                                                    of such losses as may be incurred by the Company.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <p>
                                                    8.3. The Company may,in the following events,after giving due written notice of 30 days,
                                                    suspend the appointment of the POS and terminate this Agreement immediately:
                                                </p>
                                            </div>
                                            <div class='seventhSubItem'>
                                                <ol style='list-style-type: lower-alpha;'>
                                                    <li>Failure or breach by the POS to perform any obligation under this Agreement;
                                                    </li>
                                                    <li>Attracts disqualification under Section 42(3), 42(d) of the Act;</li>
                                                    <li>Fails to comply with the code of conduct as issued from time to time;</li>
                                                    <li>Such other act, omission or commission as mentioned in the Guidelines.</li>
                                                </ol>
                                            </div>
                                        </div>
                                        <li>
                                            <span class='text500'>EFFECT OF TERMINATION</span>
                                        </li>
                                        <div class='itemContent'>
                                            <div style='display: flex;'>
                                                <p>
                                                    9.1. On termination/ expiry of this Agreement, POS shall cease to act and function as the
                                                    Point of Sales Person from the date of termination.
                                                </p>
                                            </div>
                                            <div style='display: flex;'>
                                                <span class='marginRight15'></span>
                                                <p>
                                                    9.2. POS shall hand over to the Company all documents, materials and property belonging to
                                                    the Company.
                                                </p>
                                            </div>
                                        </div>
                                        <li>
                                            <span class='text500'>GOVERNING LAW AND JURISDICTION</span>
                                        </li>
                                        <div class='itemContentDouble'>
                                            <div style='display: flex;'>
                                                <span class='marginRight15'></span>
                                                <p>
                                                    10.1. This Agreement shall be construed, interpreted and governed in accordance with the laws
                                                    of India and courts in New Delhi shall have exclusive jurisdiction.
                                                </p>
                                            </div>
                                        </div>
                                    </ol>
                                </div>
                                <p class='marginTop24 textJustify'>IN WITNESS WHEREOF, this Agreement has been signed and delivered by and between the parties on the date first set
                                </p>");
        sb.Append(@"<div class='marginTop24' style='display: flex; justify-content: space-between;'>
	                 <table style='border-collapse: collapse;width: 100%;' class='borderClass'>
		                <tr>
			                <td style='width:48%'>
				                <div style=' padding: 0 10px 10px;' class='textJustify'>
					                <p>Signed and Delivered by <span class='text500'>Hero Insurance Broking India Private Limited</span>, the within named The Company, by the hands of its Authorized Signatory
					                </p>
				                </div>
			                </td>
			                <td style='width:48%'>
				                <div style=' padding: 0 10px 10px; height: 85.33px;' class='textJustify'>
					                <p>Signed and Delivered by the named POS</p>
				                </div>
			                </td>
		                </tr>
		                <tr>
			                <td style='width:48%'>
				                <div style=' padding: 0 10px 10px;'>
					                <p style='padding-bottom: 10px;' class='text500'>Karan Chopra</p>
					                <p style='padding-bottom: 10px;'>(CEO)</p>
					                <p style='padding-bottom: 10px;' class='text500'>Nochiketa Dixit</p>
					                <p>(PO)</p>
				                </div>
			                </td>");

        sb.Append(@"<td style='width:48%'>
				            <div style=' padding: 0 10px 10px; height: 117.33px;' class='textJustify'>
					            <p class='text500'>NAME</p>
					            <p class='text500 textMaroon'>{POSP_Name}</p>
				            </div>
			            </td>
		            </tr>
		            <tr style='width:100%'>
			            <td style='width:48%'>
				            <div style='padding: 0 10px 10px;'>
					            <p>Sign- </p>");
        sb.AppendFormat(@"<img style='height:150px;width:200px;' src='data:image/png;base64, {0}' alt='sign'>", logoB64String);
        sb.Append(@"<p>Sign-Nochiketa Dixit</p>");
        sb.AppendFormat(@"<img style='height:150px;width:200px;' src='data:image/png;base64, {0}' alt='sign1'>", logoB64String);
        sb.Append(@" </div>
			            </td>
			            <td style='width:48%'>
				            <div style='padding: 0 10px 10px;' class='textJustify'>
					            <p class='text500'>Sign-</p>
                                <img style='height:150px;width:200px;' src='data:image/png;base64, {USERSIGN}' alt='sign'>
				            </div>
			            </td>
		            </tr>
	            </table>
            </div>");

        sb.Append(@"<div class='annexure'>
                                    <p class='text500 textCenter' style='text-decoration: underline;'>Annexure I</p>
                                    <p class='marginTop24'>The POS shall solicit and sell only the following pre underwritten products as
                                        approved by the IRDAI from time to time in respective category.</p>
                                    <p class='marginTop24' style='text-decoration: underline;'>Non-Life:</p>
                                    <div class='marginTop24 annex'>
                                        <p class='textJustify'>
                                            1. Motor Comprehensive Insurance Package Policy for Two – wheeler, private car and commercial
                                            vehicles
                                        </p>
                                        <p class='textJustify'>2. Third party liability (Act only) Policy for Two – wheeler, private car and
                                            commercial vehicles
                                        </p>
                                        <p class='textJustify'>
                                            3. Personal Accident Policy
                                        </p>
                                        <p class='textJustify'>
                                            4. Travel Insurance Policy
                                        </p>
                                        <p class='textJustify'>
                                            5. Home Insurance Policy and
                                        </p>
                                        <p class='textJustify'>
                                            6. Any other Policy specifically approved by IRDAI
                                        </p>
                                    </div>
                                    <p class='marginTop24' style='text-decoration: underline;'>Life:</p>
                                    <div class='marginTop24 annex'>
                                        <p class='textJustify'>
                                            1. Pure term Insurance product with or without return of premium
                                        </p>
                                        <p class='textJustify'>
                                            2. Non Linked (Non Participating) Endowment product (Money back feature not allowed)
                                        </p>
                                        <p class='textJustify'>
                                            3. Immediate Annuity Product
                                        </p>
                                        <p class='textJustify'>
                                            4. Any other product/ product category, if permitted by the authority
                                        </p>
                                    </div>
                                </div>

                                <div class='annexure'>
                                    <p class='text500 textCenter' style='text-decoration: underline;'>Annexure II</p>
                                    <p class='marginTop24 text500'><span class='marginRight15'>1.</span> CODE OF CONDUCT</p>
                                    <p class='marginTop24'>The POS shall adhere to the Code of Conduct as specified below:-</p>
                                    <ol class='marginTop24' style='padding-left: 20px;'>
                                        <li>
                                            Identify that the POS is associated with the Company;
                                        </li>
                                        <li>
                                            Disseminate the requisite information in respect of insurance products offered for sale by the
                                            Company and take into account the needs of the prospective customer while recommending a
                                            specific insurance plan;
                                        </li>
                                        <li>
                                            Indicate the applicable premium to be charged by the insurer for the insurance product offered
                                            for sale;
                                        </li>
                                        <li>
                                            Explain to the prospective customer nature of information required in the proposal form by the
                                            insurer, and also the importance of disclosure of material information in the purchase of an
                                            insurance contract and provide customer with a choice of products and force any particular
                                            product of any particular insurer
                                        </li>
                                        <li>
                                            Bring to the notice of the Company every fact about the prospective customer which are relevant
                                            to insurance underwriting, including any adverse habits or income inconsistency of the customer,
                                            within the knowledge of the POS;
                                        </li>
                                        <li>
                                            Obtain the requisite documents at the time of filing the proposal form with the Company; and any
                                            other documents which may be subsequently asked by the insurer for completion of the proposal;
                                        </li>
                                        <li>
                                            Advise every prospective customer to effect nomination under the policy;
                                        </li>
                                        <li>
                                            Inform promptly to the prospective customer about the acceptance or rejection of the proposal by
                                            the insurer;
                                        </li>
                                        <li>
                                            Render necessary assistance and advice to every policyholder on all policy servicing matters
                                            including assignment of policy, change of address or exercise of options under the policy or any
                                            other policy service, wherever necessary;
                                        </li>
                                        <li>
                                            Render necessary assistance to the policyholders or claimants or beneficiaries in complying with
                                            the requirements for settlement of claims by the insurer;
                                        </li>
                                        <li>
                                            Adhere to the code of conducts/functions of a broker to the extent of POS.
                                        </li>
                                    </ol>
                                </div>
                                <div class='annexure'>
                                    <p class='text500'><span class=''>To, </span><span class='textMaroon'>{POSP_Name}</span></p>
                                    <p class='text500 marginTop24'><span class=''>Address: </span><span class='textMaroon'>{ADDRESS}</span></p>
                                    <p class='text500 annexure'><span class=''>Date: </span><span class='textMaroon'>{DD/MM/YYYY}</span></p>
                                    <div class='annexure'>
                                        <p class='text500 textCenter' style='text-decoration: underline;'>Appointment Letter</p>
                                        <p class='textJustify marginTop24'>This is in reference to the application made by you for enrolling
                                            yourself to act as point-of-Sale Person.</p>
                                        <p class='textJustify marginTop24'>This is to confirm you that you have successfully completed the
                                            prescribed training and have also passed the examination specified for Point of Sales
                                            examination conducted By <span class='text500'>Hero Insurance Broking India Pvt. Ltd</span>.
                                            (IRDAI Registration Number 649)
                                            under the IRDAI Guideline on Point-of-Sale Person.
                                        </p>
                                        <p class='marginTop24'>Your personal details are as under.</p>
                                        <p class='marginTop24 text500'>PAN Number : <span class='textMaroon'>{PANNUMBER} </span></p>
                                        <p class='marginTop24 text500'>POSP Identification Number : <span class='textMaroon'>{POSPIdentificationNumber}</span></p>
                                        <p class='marginTop24'>This letter authorizes you to act as Point-of-Sale Person for <span class='text500'>Hero Insurance Broking India Pvt.
                                            Ltd. (IRDAI Registration No.649)</span> to market products categorized and identified under the POSP
                                            Guidelines only. In case you wish to work for another company, you required to obtain a fresh
                                            letter from the new insurance/insurance intermediary in order to act as Point-of-Sale Person for
                                            that entity.
                                        </p>
                                        <p class='marginTop24'>Yours truly, </p>
                                        <p class='marginTop24 text500'>For Hero Insurance Broking India Pvt. Ltd.</p>
                                        <div class='marginTop24' style='display: flex; height: 100px;'>");

        sb.AppendFormat(@"<img style='height:70px;width:250px;' src='data:image/png;base64, {0}' alt=''>", logoB64String);
        sb.AppendFormat(@"<img style='height:70px;width:250px;'src='data:image/png;base64, {0}' alt=''>", logoB64String);
        sb.Append(@"           
                                        </div>
                                        <p class='marginTop24 text500'>Karan Chopra & Nochiketa Dixit</p>
                                        <p style='margin-top: 5px;'>(CEO & PO)</p>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </body>
                                    </html>");
        string HTMLTemplate = sb.ToString();
        return HTMLTemplate;
    }
    #endregion

    #region - Get Row Certificate Template -
    /// <summary>
    /// Get Row Certificate Template
    /// </summary>
    /// <returns></returns>
    private string GetRowCertificateTemplate(string directoryPath)
    {
        directoryPath += @"assets\";
        var sb = new StringBuilder();
        sb.Append(@"<!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <link
                            href='https://fonts.googleapis.com/css2?family=Rubik:ital,wght@0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap'
                            rel='stylesheet'>");
        sb.Append(@"<style>
                        * {
                            padding: 0;
                            margin: 0;
                            font-family: 'Rubik', sans-serif;
                        }");

        string certBorderB64String = GetBase64StringOfPath(directoryPath + "cert.border.png");
        sb.Append(".mainContainer {display: flex;flex-direction: column;align-items: center;width: auto;margin: 0 auto;background: url('data:image/png;base64, " + certBorderB64String + "');background-size: 100% 100%;}");

        sb.Append(@"body{
                display: flex;
                justify-content: center;
            } .innerContent {
                            position: relative;
                        }

                        .certificateBG {
                            width: auto;
                        }

                        .mainContent {
                            position: relative;
                            padding: 1rem;
                            position: relative;
                        }

                        .logo {
                            text-align: center;
                        }

                        .logo img {
                            width: 200px;
                            height: 95px;
                        }

                        .lineHeight {
                            line-height: 1.5;
                        }

                        .lineHeight1 {
                            line-height: 1;
                        }

                        .title {
                            margin: 0;
                            font-size: 65px;
                            text-transform: uppercase;
                            font-weight: 500;
                            color: #eb3139;
                        }

                        .title2 {
                            margin: 0;
                            font-size: 40px;
                            text-transform: uppercase;
                            font-weight: 400;
                            color: #eb3139;
                        }

                        .title3 {
                            font-size: 25px;
                            text-transform: uppercase;
                            margin-top: 20px;
                            color: #555c59;
                            font-weight: 300;
                        }

                        .content {
                            text-align: justify;
                            font-size: 20px;
                            margin-top: 26px;
                            line-height: 2;
                            padding: 0 90px;
                        }

                        .contentContain {
                            text-align: justify;
                        }

                        .content2 {
                            text-align: justify;
                            font-size: 20px;
                            margin-top: 26px;
                            line-height: 1.5;

                        }

                        .userName {
                            border-bottom: 2px solid;
                            min-width: 300px;
                            display: inline-block;
                            line-height: 0.8;
                            text-align: center;
                        }

                        .state {
                            display: inline-block;
                            line-height: 0.8;
                            border-bottom: 2px solid;
                            min-width: 155px;
                            text-align: center;
                        }

                        .platform {
                            display: inline-block;
                            line-height: 0.8;
                            text-align: center;
                        }

                        .platform2 {
                            display: inline-block;
                            line-height: 0.8;
                            text-align: left;
                        }

                        .date {
                            display: inline-block;
                            text-align: left;
                        }

                        .duration {
                            font-weight: 500;
                        }

                        .totalTime {
                            margin-bottom: 0;
                            font-weight: 500;
                            margin-top: 26px;
                        }

                        .signature {
                            margin-top: 24px;
                            display: flex;
                            flex-direction: column;
                            align-items: center;
                            text-align: center;
                        }

                        .ceoName {
                            font-size: 24px;
                            text-align: center;
                            margin-top: 6px;
                            line-height: 1.5;
                        }

                        .ceo {
                            font-size: 24px;
                            font-weight: 300;
                            line-height: 1;
                            text-align: center;
                        }
                    </style></head>");

        sb.Append(@"<body>
                    <div style='width: 1000px;'>
                    <div class='mainContainer'>
                    <div class='mainContent'>
                    <div class='innerContent'>
                    <div class='logo'>
                    <p><br /><br /></p>");
        string logoB64String = GetBase64StringOfPath(directoryPath + "HeroInsurance_Logo.png");
        sb.AppendFormat(@"<img style='width:250px;height:100px;' src='data:image/png;base64, {0}' alt='logo' />", logoB64String);
        sb.Append(@"</div>
                <div style='margin-top: 15px; text-align: center;'>
                    <p class='title'>Certificate</p>
                    <p class='title2'>Of appreciation</p>
                    <p class='title3 lineHeight'>for online insurance training & examination </p>
                </div>
                <div class='content'>
                    <p style='margin: 0;'>
                        This is to certify that <span class='userName'>{Name}</span> from
                        <span class='state'>{State}</span> having login id <span class='state'>{POSPId}</span> was
                        sponsored
                        for Online Point of Sale – General Insurance & Life Insurance
                        by Hero Insurance Broking India Pvt. Ltd. The training was
                        conducted online at <span class='platform'>{partners.heroinsruance.com}</span> from
                        <span class='date'>{Start Date}</span> - <span class='date'>{End Date}.</span>
                    </p>
                    <div class='content2 contentContain'>
                        <p style='margin: 0;'>
                            Congratulations on completing <span class='duration'>
                                15 hours of training for General
                                Insurance and 15 hours of training for Life Insurance
                            </span> as stipulated under IRDAI POS-P Guidelines.
                        </p>
                        <p class='totalTime'>Total Training Time: 30 hours</p>
                    </div>
                    <div class='content2'>
                        <p style='margin: 0;'>
                            The candidate appeared for the online examination at
                            <span class='platform2'>{partners.heroinsruance.com}</span> on <span
                                class='dateRange'>{Date}</span>
                            and has successfully
                            passed the examination.<br /><br />
                        </p>
                    </div>
                    <div class='signature'>
                        <span class='state'>");
        string signB64String = GetBase64StringOfPath(directoryPath + "sign.png");
        sb.AppendFormat(@"<img style='height:70px;width:250px;' src='data:image/png;base64, {0}' alt='signature' />", signB64String);
        sb.Append(@"</span>
                        <p class='ceoName'>Karan Chopra</p>
                        <p class='ceo'>Chief Executive Officer</p>
                        <br /><br /><br />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    </div>
                </body>
                </html>");
        string HTMLTemplate = sb.ToString();
        return HTMLTemplate;
    }

    private static string GetBase64StringOfPath(string directoryPath)
    {
        string base64String = "";
        try
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(directoryPath);
            base64String = Convert.ToBase64String(fileBytes);
            Console.WriteLine(base64String);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return base64String;
    }

    #endregion

    #endregion

    #region - Get PospLastLogIn Details -
    /// <summary>
    /// GetPospLastLogInDetails
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPospLastLogInDetails")]
    [ProducesResponseType(typeof(IEnumerable<GetPospLastLogInDetailsVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPospLastLogInDetailsVm>>> GetPospLastLogInDetails(string UserId, CancellationToken cancellationToken)
    {
        var req = new GetPospLastLogInDetailsQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("POSP not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - InsertReferralNewUserDetails - 
    /// <summary>
    /// InsertReferralNewUserDetails
    /// </summary>
    /// <param name="insertExamInstructionsDetail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("InsertReferralNewUserDetails")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ReferralNewUserDetailsModel>> InsertReferralNewUserDetails(string referralUserId, string referralMode, string userName, string emailId, string phoneNumber, string environment, CancellationToken cancellationToken)
    {
        var cmd = new InsertReferralNewUserDetailsCommand
        {
            ReferralMode = referralMode,
            UserName = userName,
            EmailId = emailId,
            PhoneNumber = phoneNumber,
            Environment = environment,
            ReferralUserId = referralUserId
        };
        var result = await _mediatr.Send(cmd, cancellationToken);
        if (result.Result != null && !string.IsNullOrEmpty(result.Result.ErrorMessage))
        {
            var problemDetails = Result.CreateNotFoundError(result.Result.ErrorMessage);
            return NotFound(problemDetails);
        }
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Referral detail create failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - InsertUserDeviceDetails - 
    /// <summary>
    /// InsertUserDeviceDetails
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("InsertUserDeviceDetails")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> InsertUserDeviceDetails(InsertUserDeviceDetailsCommand insertUserDeviceDetailsCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(insertUserDeviceDetailsCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Training instructions Detail Create failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion


    #region - GetPanRejectionReasons - 
    /// <summary>
    /// GetPanRejectionReasons
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("GetPanRejectionReasons")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> GetPanRejectionReasons(GetPanRejectionReasonsQuery getPanRejectionReasonsQuery, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(getPanRejectionReasonsQuery, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError(result.Messages);
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion
}
