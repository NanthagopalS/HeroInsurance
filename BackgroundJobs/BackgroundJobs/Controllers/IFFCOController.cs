using BackgroundJobs.Models;
using BackgroundJobs.Repository.Repository.Abstraction;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class IFFCOController : ControllerBase
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly HeroConfig _heroConfig;
    private readonly IIFFCORepository _iFFCORepository;

    public IFFCOController(IRecurringJobManager recurringJobManager, IOptions<HeroConfig> options, IIFFCORepository iFFCORepository)
    {
        _recurringJobManager = recurringJobManager;
        _heroConfig = options.Value;
        _iFFCORepository = iFFCORepository;
    }
    [HttpGet("/GetIFFCOBreakInStatus")]
    public ActionResult GetIFFCOBreakInStatus()
    {
        _recurringJobManager.AddOrUpdate("GetIFFCOBreakInStatus", () => _iFFCORepository.GetBreakinPinStatus(), _heroConfig.CronExpForIFFCOBreakinStatus);
        return Ok();
    }
}
