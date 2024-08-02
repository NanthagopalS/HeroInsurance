using Identity.API.Models;
using Identity.Core.Features.Benefits.Commands.BenefitsDetail;
using Identity.Core.Features.User;
using Identity.Core.Features.User.Commands.DeleteBenifitsDetail;
using Identity.Core.Features.User.Commands.InsertBenifitsDetail;
using Identity.Core.Features.User.Commands.ReuploadDocument;
using Identity.Core.Features.User.Commands.SendCompletedRegisterationMail;
using Identity.Core.Features.User.Commands.UserAddressDetail;
using Identity.Core.Features.User.Commands.UserBankDetail;
using Identity.Core.Features.User.Commands.UserDocumentUpload;
using Identity.Core.Features.User.Commands.UserEmailId;
using Identity.Core.Features.User.Commands.UserInquiryDetail;
using Identity.Core.Features.User.Commands.UserPersonalDetail;
using Identity.Core.Features.User.Commands.UserProfilePicture;
using Identity.Core.Features.User.Commands.VerifyEmail;
using Identity.Core.Features.User.Queries.GetBenefitsDetail;
using Identity.Core.Features.User.Queries.GetErrorCode;
using Identity.Core.Features.User.Queries.GetPOSPConfigurationDetail;
using Identity.Core.Features.User.Queries.GetPOSPSourceType;
using Identity.Core.Features.User.Queries.GetRejectedDocument;
using Identity.Core.Features.User.Queries.GetSourcedByUserList;
using Identity.Core.Features.User.Queries.GetStateCitybyPincode;
using Identity.Core.Features.User.Queries.GetUserBreadcrumStatusDetail;
using Identity.Core.Features.User.Queries.GetUserDetail;
using Identity.Core.Features.User.Queries.GetUserListForDepartmentTagging;
using Identity.Core.Features.User.Queries.GetUserPersonalVerificationDetail;
using Identity.Core.Features.User.Queries.GetUserProfileDetail;
using Identity.Core.Features.User.Queries.GetUserProfilePictureDetail;
using Identity.Core.Features.User.Queries.PanVerificationDetails;
using Identity.Core.Features.User.Queries.UserDocument;
using Identity.Core.Features.User.Querries.GetMasterType;
using Identity.Core.Helpers;
using Identity.Domain.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Helpers;

namespace Identity.API.Controllers.v1;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[ApiController]
[ServiceFilter(typeof(ResponseCaptureFilter))]
public class UserController : ControllerBase
{
    private readonly IMediator _mediatr;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public UserController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    #region - GET Methods -

    #region - Get Insurane Type -
    /// <summary>
    /// GetInsuraneType
    /// </summary>
    /// <param name="insuranceType">InsuranceType: MOTOR/HEALTH/TERM</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetPOSPUserMaster")]
    [ProducesResponseType(typeof(MasterTypeVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<MasterTypeVm>> GetPOSPUserType(CancellationToken cancellationToken)
    {
        var req = new GetPOSPUserMasterQuery { };
        var result = await _mediatr.Send(req, cancellationToken);

        if (result == null)
        {
            var problemDetails = Result.CreateNotFoundError("Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get User Profile Picture Detail -
    /// <summary>
    /// GetUserProfilePictureDetail
    /// </summary>
    /// <param name="userId"></param>   
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetUserProfilePictureDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetUserProfilePictureDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetUserProfilePictureDetailVm>>> GetUserProfilePictureDetail(string UserId, CancellationToken cancellationToken)
    {
        var req = new GetUserProfilePictureDetailQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("User profile picture Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - GetUser Profile Detail -
    /// <summary>
    /// GetUserProfileDetail
    /// </summary>
    /// <param name="UserId">
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetUserProfileDetail")]
    [ProducesResponseType(typeof(GetUserProfileDetailVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<GetUserProfileDetailVm>> GetUserProfileDetail(string UserId, CancellationToken cancellationToken)
    {
        var req = new GetUserProfileDetailQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);

        if (result == null)
        {
            var problemDetails = Result.CreateNotFoundError("Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get UserDetail -
    /// <summary>
    /// Get UserDetail
    /// </summary>
    /// <param name="userId"></param>   
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetPOSPConfigurationDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetPOSPConfigurationDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPOSPConfigurationDetailVm>>> GetPOSPConfigurationDetail(CancellationToken cancellationToken)
    {
        var req = new GetPOSPConfigurationDetailQuery { };
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

    #region - Get User Document Detail
    /// <summary>
    /// Get UserDocumentDetail
    /// </summary>
    /// <param name="userId"></param>   
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetUserDocumentDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetUserDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserDocumentDetailModel>>> GetUserDocumentDetail(string UserId, CancellationToken cancellationToken)
    {
        var req = new GetRejectedDocumentQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("User Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Pan Verification Details -
    /// <summary>
    /// GetPanVerificationDetails
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("VerifyPAN/{userId}/{panNumber}")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PanVerificationVM>> VerifyPAN(string userId, string panNumber, CancellationToken cancellationToken)
    {
        var req = new GetPanVerificationQuery
        {
            UserId = userId,
            PanNumber = panNumber
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Result != null && result.Result.IsUserExists && !string.IsNullOrEmpty(result.Result.InvalidPanMsg))
        {
            var problemDetails = Result.CreateNotFoundError(result.Result.InvalidPanMsg);
            return NotFound(problemDetails);
        }
        else if (result.Result != null && result.Result.IsUserExists)
        {
            var problemDetails = Result.CreateNotFoundError("The PAN you have entered is already exist");
            return NotFound(problemDetails);
        }
        else if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("PAN Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Verify EmailId -
    [AllowAnonymous]
    [HttpGet("VerifyEmailId/{userId}/{guId}")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PanVerificationVM>> VerifyEmailId(string userId, string guId, CancellationToken cancellationToken)
    {
        var req = new GetVerifyEmailQuery
        {
            UserId = userId,
            GuId = guId
        };
        var result = await _mediatr.Send(req, cancellationToken);

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Verify Admin EmailId -
    [AllowAnonymous]
    [HttpGet]
    [Route("VerifyAdminEmailId")]
    [ProducesResponseType(typeof(VerifyEmailResponseModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    //public async Task<ActionResult<VerifyEmailResponseModel>> VerifyAdminEmailId(string UserId, string EmailId, CancellationToken cancellationToken)
    public async Task<ActionResult<VerifyEmailResponseModel>> VerifyAdminEmailId(string UserId, string EmailId, CancellationToken cancellationToken)
    {
        var req = new VerifyEmailCommand
        {
            UserId = UserId,
            EmailId = EmailId
        };

        var result = await _mediatr.Send(req, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateValidationError(new List<string> { "Failed to Admin " });
            return BadRequest(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);

    }
    #endregion

    #region - Get Document Type -
    /// <summary>
    /// GetDocumentType With Id 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet()]
    [Route("GetUserDocumentType")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserDocumentTypeModel>>> GetDocumentType(string UserId, CancellationToken cancellationToken)
    {
        var req = new GetDocumentTypeQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    #endregion

    #region - Get User Breadcrum Status Detail -
    /// <summary>
    /// GetUserBreadcrumStatusDetail
    /// </summary>
    /// <param name="userId"></param>   
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetUserBreadcrumStatusDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetUserBreadcrumStatusDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetUserBreadcrumStatusDetailVm>>> GetUserBreadcrumStatusDetail(string UserId, CancellationToken cancellationToken)
    {
        var req = new GetUserBreadcrumStatusDetailQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("User BreadScrum Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get UserDetail -
    /// <summary>
    /// Get UserDetail
    /// </summary>
    /// <param name="userId"></param>   
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetUserDetail")]
    [ProducesResponseType(typeof(IEnumerable<GetUserDetailVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetUserDetailVm>>> GetUserDetail(string UserId, CancellationToken cancellationToken)
    {
        var req = new GetUserDetailQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("User Detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Benefit Detail -
    [HttpGet]
    [Route("GetBenefitsDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> GetBenefitsDetail(CancellationToken cancellationToken)
    {
        var req = new GetBenefitsDetailQuery();

        var result = await _mediatr.Send(req, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Benefit Detail Get failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);

    }
    #endregion

    #region - Get User Personal Verification Detail -
    /// <summary>
    /// GetUserPersonalVerificationDetail
    /// </summary>
    /// <param name="UserId">
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetUserPersonalVerificationDetail")]
    [ProducesResponseType(typeof(GetUserPersonalVerificationDetailVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<GetUserPersonalVerificationDetailVm>> GetUserPersonalVerificationDetail(string UserId, CancellationToken cancellationToken)
    {
        var req = new GetUserPersonalVerificationDetailQuery
        {
            UserId = UserId
        };
        var result = await _mediatr.Send(req, cancellationToken);

        if (result == null)
        {
            var problemDetails = Result.CreateNotFoundError("User personal verification detail not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get Error Code -
    /// <summary>
    /// GetErrorCode
    /// </summary>
    /// <param name="ErrorType">
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetErrorCode")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<GetErrorCodeVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetErrorCodeVm>>> GetErrorCode(string ErrorType, CancellationToken cancellationToken)
    {
        var req = new GetErrorCodeQuery
        {
            ErrorType = ErrorType
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Error codes not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Get State City by Pin code -
    /// <summary>
    /// GetStateCitybyPincode
    /// </summary>
    /// <param name="Pincode">
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetStateCitybyPincode")]
    [ProducesResponseType(typeof(GetStateCitybyPincodeVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<GetStateCitybyPincodeVm>> GetStateCitybyPincode(string Pincode, CancellationToken cancellationToken)
    {
        var req = new GetStateCitybyPincodeQuery
        {
            Pincode = Pincode
        };
        var result = await _mediatr.Send(req, cancellationToken);

        if (result == null)
        {
            var problemDetails = Result.CreateNotFoundError("Data not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - GetUserListForDepartmentTagging -
    /// <summary>
    /// GetUserListForDepartmentTagging
    /// </summary>
    /// <param name="ErrorType">
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpGet("GetUserListForDepartmentTagging")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<GetUserListForDepartmentTaggingVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseMessage), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetUserListForDepartmentTaggingVm>>> GetUserListForDepartmentTagging(string TaggingType, CancellationToken cancellationToken)
    {
        var req = new GetUserListForDepartmentTaggingQuery
        {
            TaggingType = TaggingType
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Error codes not found");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #endregion

    #region - POST Method -

    #region - User Inquiry Detail -
    [HttpPost]
    [AllowAnonymous]
    [Route("CreateUserInquiry")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> CreateUserInquiryDetail(UserInquiryDetailCommand userInquiryDetailCommand, CancellationToken cancellationToken)
    {

        //string fileName = "D:/hero/Hero.Insurance.API/Hero.Insurance.API/ThirdPartyIntegration/ThirdPartyUtilities/Helpers/ErrorCode.json";
        //string jsonData = System.IO.File.ReadAllText(fileName);
        ////jsonconvert 
        ////ErrorCodeDetail ob = JsonSerializer.Deserialize<ErrorCodeDetail>(jsonData);
        //ErrorCodeDetail ob = System.Text.Json.JsonSerializer.Deserialize<ErrorCodeDetail>(jsonData);


        var result = await _mediatr.Send(userInquiryDetailCommand, cancellationToken);

        if (result.Failed)
        {
            //string fileName = "D:/hero/Hero.Insurance.API/Hero.Insurance.API/ThirdPartyIntegration/ThirdPartyUtilities/ErrorCode'ErrorCode.json";
            //string jsonData = System.IO.File.ReadAllText(fileName);
            //ErrorCodeDetail ob = System.Text.Json.JsonSerializer.Deserialize<ErrorCodeDetail>(jsonData);

            //var code = "E-100";
            //var message = "";

            //for (int i = 0; i < ob.Information_error.Length; i++)
            //{
            //    if (ob.Information_error[i].error_code == code)
            //    {
            //        message = ob.Information_error[i].error_msg;
            //    }
            //}
            var problemDetails = Result.CreateValidationError(new List<string> { "Failed to create Inquiry" });
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Upload User Documents -
    /// <summary>
    /// Userdocumentupload
    /// </summary>
    /// <param name="userdocumentuploadcommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    [HttpPost]
    [Route("UploadUserDocuments")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UploadUserDocuments(IFormFile files, [FromForm] UserDocumentUploadCommand userdocumentuploadcommand, CancellationToken cancellationToken)
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
            userdocumentuploadcommand.DocumentFileName = files.FileName;
            userdocumentuploadcommand.ImageStream = byteimage;
            userdocumentuploadcommand.FileSize = fileSizeibMbs.ToString();
            var result = await _mediatr.Send(userdocumentuploadcommand, cancellationToken);
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
    #endregion

    #region - Create Benefits Detail -
    [HttpPost]
    [Route("CreateBenefitsDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> CreateBenefitsDetail(InsertBenefitsDetailCommand insertbenefitsdetailcommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(insertbenefitsdetailcommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Benefit Detail Create failed");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion CreateBenefitDetail

    #region - User Profile Picture Upload - 
    /// <summary>
    /// UserProfilePictureUpload
    /// </summary>
    /// <param name="insuranceType">
    /// <param name="cancellationToken"></param>
    /// <returns>Dashboard data</returns>
    [HttpPost]
    [Route("UserProfilePictureUpload")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UserProfilePictureUpload(IFormFile files, [FromForm] UserProfilePictureCommand userProfilePictureCommand, CancellationToken cancellationToken)
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
        userProfilePictureCommand.ImageStream = byteimage;
        userProfilePictureCommand.ProfilePictureStoragePath = fileNameWithPath;
        userProfilePictureCommand.ProfilePictureFileName = files.FileName;
        var result = await _mediatr.Send(userProfilePictureCommand, cancellationToken);
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

    #region - ReUpload User Documents -
    /// <summary>
    /// ReUploadUserDocuments
    /// </summary>
    /// <param name="reuploaddocumentcommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("ReUploadDocument")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> ReUploadUserDocument(IFormFile files, [FromForm] ReUploadDocumentCommand reuploaddocumentcommand, CancellationToken cancellationToken)
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

            System.IO.File.Delete(fileNameWithPath);
            var dname = Path.GetDirectoryName(fileNameWithPath);
            Directory.Delete(dname, true);
            reuploaddocumentcommand.DocumentFileName = files.FileName;
            reuploaddocumentcommand.ImageStream = byteimage;
            var result = await _mediatr.Send(reuploaddocumentcommand, cancellationToken);
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
    #endregion

    #endregion

    #region - PUT Methods -

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
    public async Task<ActionResult<bool>> UpdateUserPersonalDetail( [FromForm] UserPersonalDetailCommand userPersonalDetailCommand, CancellationToken cancellationToken)
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

    #region - UpdateUserBankDetail -
    [HttpPut]
    [Route("UpdateUserBankDetail")]
    [ProducesResponseType(typeof(ResultModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<UserBankDetailUpdateResponce>> UpdateUserBankDetail(UserBankDetailCommand userBankDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(userBankDetailCommand, cancellationToken);

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region - Update User EmailId - 
    /// <summary>
    /// UpdateUserEmailId
    /// </summary>
    /// <param name="userEmailIdCommand"></param>   
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("UpdateUserEmailId")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<string>> UpdateEmailId(UserEmailIdCommand userEmailIdCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(userEmailIdCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("User Email Id Update failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #region  - Update User Address Detail -

    [HttpPut]
    [Route("UpdateUserAddressDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UpdateUserAddressDetail(UserAddressDetailCommand userAddressDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(userAddressDetailCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("User address Detail Update failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    #endregion

    #region  - Edit Benefit Detail -
    [HttpPut]
    [Route("UpdateBenefitsDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UpdateBenefitsDetail(BenefitsDetailCommand benefitsDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(benefitsDetailCommand, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Benefit Detail Update failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    #endregion EditBenefitDetail

    #endregion

    #region - DELETE Methods -

    #region - Delete Benefit Detail -
    [HttpDelete]
    [Route("DeleteBenefitsDetail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> DeleteBenefitsDetail(string Id, CancellationToken cancellationToken)
    {
        var req = new DeleteBenefitsDetailQuery
        {
            Id = Id
        };
        var result = await _mediatr.Send(req, cancellationToken);
        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Benefit Detail deleted failed");
            return NotFound(problemDetails);
        }

        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }

    #endregion DeleteBenefitDetail

    #endregion

    #region - Get POSPSource Type -
    /// <summary>
    /// Get POSPSource Type
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetPOSPSourceType")]
    [ProducesResponseType(typeof(GetPOSPSourceTypeVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<GetPOSPSourceTypeVm>> GetPOSPSourceType(CancellationToken cancellationToken)
    {
        var req = new GetPOSPSourceTypeQuery() { };
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

    #region - GetSourcedByUserList -
    /// <summary>
    /// GetSourcedByUserList
    /// </summary>        
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetSourcedByUserList")]
    [ProducesResponseType(typeof(GetSourcedByUserListVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<GetSourcedByUserListVm>> GetSourcedByUserList(string? BUId, CancellationToken cancellationToken)
    {
        var req = new GetSourcedByUserListQuery
        {
            BUId = BUId
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

    #region - Send Email for Registeration -
    /// <summary>
    /// Send Email for Registeration
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("SendCompletedRegisterationMail")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> SendCompletedRegisterationMail(string UserId, CancellationToken cancellationToken)
    {
        var req = new SendCompletedRegisterationMailQuery
        {
            UserId = UserId,
        };
        var result = await _mediatr.Send(req, cancellationToken);

        if (result.Failed)
        {
            var problemDetails = Result.CreateNotFoundError("Mail not send or Not completed the journey");
            return NotFound(problemDetails);
        }
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion
}

