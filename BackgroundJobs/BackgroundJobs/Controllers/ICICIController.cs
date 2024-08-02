using BackgroundJobs.Models;
using BackgroundJobs.Repository.Repository.Abstraction;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class ICICIController : ControllerBase
{
    private readonly IICICIRepository _iCICIRepository;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly HeroConfig _heroConfig;

    public ICICIController(IICICIRepository iCICIRepository,
                             IRecurringJobManager recurringJobManager,
                             IOptions<HeroConfig> options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _iCICIRepository = iCICIRepository ?? throw new ArgumentNullException(nameof(iCICIRepository));
        _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
        _heroConfig = options.Value;
    }

    [HttpGet("/GetICICIBreakInStatus")]    
    public ActionResult GetICICIBreakInStatus()
    {
        _recurringJobManager.AddOrUpdate("GetICICIBreakInStatus", () => _iCICIRepository.GetPolicyStatus(), _heroConfig.CronExpForICICIBreakInStatus);
        return Ok();
    }

    [HttpGet("/GetICICIPaymentStatus")]   
    public ActionResult GetICICIPaymentStatus()
    {
        _recurringJobManager.AddOrUpdate("GetICICIPaymentStatus", () => _iCICIRepository.GetPaymentStatus(), _heroConfig.CronExpForICICIPaymentStatus);
        return Ok();
    }

    [HttpPost("/CreateICICIIMBroker")]
    public ActionResult CreateICICIIMBroker()
    {
        _recurringJobManager.AddOrUpdate("CreateICICIIMBroker", () => _iCICIRepository.CreateIMBroker(), _heroConfig.CronExpForICICICreateIMBroker);
        return Ok();
    }
}
