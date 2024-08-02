using BackgroundJobs.Models;
using BackgroundJobs.Repository.Repository.Abstraction;
using BackgroundJobs.Repository.Repository.Implementation;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class TATAController : ControllerBase
{
    private readonly ITATARepository _iTATARepository;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly HeroConfig _heroConfig;

    public TATAController(ITATARepository tATARepository,
                             IRecurringJobManager recurringJobManager,
                             IOptions<HeroConfig> options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _iTATARepository = tATARepository ?? throw new ArgumentNullException(nameof(tATARepository));
        _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
        _heroConfig = options.Value;
    }

	[HttpGet("/GetTATABreakInStatus")]
	public ActionResult BreakInStatus()
	{
		_recurringJobManager.AddOrUpdate("BreakInStatus", () => _iTATARepository.GetBreakInStatus(), _heroConfig.CronExpForTATABreakinStatus);
		return Ok();
	}

    [HttpGet("/GetTATAPaymentStatus")]
    public ActionResult PaymentStatus()
    {
        _recurringJobManager.AddOrUpdate("PaymentStatus", () => _iTATARepository.GetPaymentStatus(), _heroConfig.CronExpForTATAPaymentStatus);
        return Ok();
    }
}
