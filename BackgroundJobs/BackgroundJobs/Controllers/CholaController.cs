using BackgroundJobs.Models;
using BackgroundJobs.Repository.Repository.Abstraction;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
    public class CholaController : ControllerBase
    {
        private readonly ICholaRepository _icholaRepository;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly HeroConfig _heroConfig;

        public CholaController(ICholaRepository cholaRepository,IRecurringJobManager recurringJobManager,IOptions<HeroConfig> options)
        {
            if (options is null)
            { 
                throw new ArgumentNullException(nameof(options));
            }
            _icholaRepository = cholaRepository??throw new ArgumentException(nameof(options));
            _recurringJobManager = recurringJobManager??throw new ArgumentException(nameof(options));
            _heroConfig = options.Value;
        }
        [HttpGet("/GetCholaCKYCStatus")]
        public ActionResult GetCKYCStatus()
        {
            _recurringJobManager.AddOrUpdate("GetCKYCStatus", () => _icholaRepository.GetCKYCStatus(), _heroConfig.CronExpForCholaCKYCStatus);
            return Ok();
        }
    [HttpGet("/GetCholaBreakInStatus")]
    public ActionResult BreakInStatus()
    {
        _recurringJobManager.AddOrUpdate("BreakInStatus",()=> _icholaRepository.GetBreakInStatus(),_heroConfig.CronExpForCholaBreakInStatus);
        return Ok();
    }
 

    [HttpGet("/GetPaymentStatus")]
    public ActionResult GetPaymentStatus()
    {
        _recurringJobManager.AddOrUpdate("GetPaymentStatus", () => _icholaRepository.GetPaymentStatus(), _heroConfig.CronExpForCholaPaymentStatus);
        return Ok();
    }

}
