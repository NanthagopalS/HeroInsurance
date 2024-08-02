using BackgroundJobs.Models;
using BackgroundJobs.Repository.Repository.Abstraction;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class GoDigitController : ControllerBase
{
    private readonly IGoDigitRepository _goDigitRepository;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly HeroConfig _heroConfig;

    public GoDigitController(IGoDigitRepository goDigitRepository,
                             IRecurringJobManager recurringJobManager,
                             IOptions<HeroConfig> options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _goDigitRepository = goDigitRepository ?? throw new ArgumentNullException(nameof(goDigitRepository));
        _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
        _heroConfig = options.Value;
    }

    [HttpGet("/GetBreakInStatus")]
    public ActionResult GetBreakInStatus()
    {
        _recurringJobManager.AddOrUpdate("GetBreakInStatus", () => _goDigitRepository.GetPolicyStatus(), _heroConfig.CronExpForGoDigitBreakInStatus);
        return Ok();
    }

    [HttpGet("/GetCKYCPaymentStatus")]
    public ActionResult GetCKYCPaymentStatus()
    {
        _recurringJobManager.AddOrUpdate("GetCKYCPaymentStatus", () => _goDigitRepository.GetCKYCPaymentStatus(), _heroConfig.CronExpForGoDigitPaymentStatus);
        return Ok();
    }
}
