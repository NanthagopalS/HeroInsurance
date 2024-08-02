using BackgroundJobs.Models;
using BackgroundJobs.Repository.Repository.Abstraction;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class HDFCController : ControllerBase
{
    private readonly IHDFCRepository _iHDFCRepository;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly HeroConfig _heroConfig;

    public HDFCController(IHDFCRepository hdfcRepository,
                             IRecurringJobManager recurringJobManager,
                             IOptions<HeroConfig> options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _iHDFCRepository = hdfcRepository ?? throw new ArgumentNullException(nameof(hdfcRepository));
        _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
        _heroConfig = options.Value;
    }

    [HttpGet("/GetCKYCStatus")]    
    public ActionResult HDFCCKCYPOAStatus()
    {
        _recurringJobManager.AddOrUpdate("GetCKYCStatus", () => _iHDFCRepository.GetCKYCStatus(), _heroConfig.CronExpForHDFCCKYCPOAStatus);
        return Ok();
    }

    [HttpPost("/CreatePOSP")]
    public ActionResult CreatePOSP()
    {
        _recurringJobManager.AddOrUpdate("CreatePOSP", () => _iHDFCRepository.CreatePOSP(), _heroConfig.CronExpForHDFCCreatePOSP);
        return Ok();
    }
}
