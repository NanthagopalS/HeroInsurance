using Identity.Core.Features.Banner.Commands.InsertBanner;
using Identity.Core.Features.Banner.Queries.BannerDetail;
using Identity.Core.Helpers;
using Identity.Domain.Banner;
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
public class BannerController : ControllerBase
{
    private readonly IMediator _mediatr;

    /// <summary>
    /// Initialize and set the dependencies
    /// </summary>
    /// <param name="mediatr"></param>
    public BannerController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    #region - GET Methods -

    #region - Get Banner Details -
    /// <summary>
    /// GetBannerDetails
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet()]
    [Route("GetBannerDetails")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<BannerDetailModel>>> GetBannerDetails(CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(new GetBannerDetailQuery(), cancellationToken);
        var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
        return Ok(res);
    }
    #endregion

    #endregion

    #region - POST Methods -

    #region - Upload Banner -
    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="banneruploadcommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost()]
    [Route("UploadBanner")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<bool>> UploadBanner(IFormFile files, [FromForm] BannerUploadCommand banneruploadcommand, CancellationToken cancellationToken)
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

    #endregion
    
}





