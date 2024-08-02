using BackgroundJobs.Models;
using BackgroundJobs.Repository.Repository.Abstraction;
using BackgroundJobs.Repository.Repository.Implementation;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class BajajController : ControllerBase
{
    private readonly IBajajRepository _iBajajRepository;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly HeroConfig _heroConfig;

    public BajajController(IBajajRepository bajajRepository,
                             IRecurringJobManager recurringJobManager,
                             IOptions<HeroConfig> options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _iBajajRepository = bajajRepository ?? throw new ArgumentNullException(nameof(bajajRepository));
        _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
        _heroConfig = options.Value;
    }

    [HttpGet("/GetBajajBreakInStatus")]    
    public ActionResult GetBajajBreakInStatus()
    {
        _recurringJobManager.AddOrUpdate("GetBajajBreakInStatus", () => _iBajajRepository.GetBreakinPinStatus(), _heroConfig.CronExpForBajajBreakInStatus);
        return Ok();
    }
    
}
