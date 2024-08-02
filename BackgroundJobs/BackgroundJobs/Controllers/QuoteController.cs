using BackgroundJobs.Models;
using BackgroundJobs.Repository.Repository.Abstraction;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly HeroConfig _heroConfig;
        private readonly IQuoteRepository _quoteRepo;

        public QuoteController(IRecurringJobManager recurringJobManager, 
            IOptions<HeroConfig> options,
            IQuoteRepository quoteRepo)
        {
            _recurringJobManager = recurringJobManager;
            _heroConfig = options.Value;
            _quoteRepo = quoteRepo;
        }
        [HttpPost("/InsertQuoteTrascationLogs")]
        public ActionResult InsertQuoteTrascationLogs()
        {
            _recurringJobManager.AddOrUpdate("InsertQuoteTrascationLogs", () => _quoteRepo.MoveQuoteDataToArchive(), Cron.Monthly);
            // _quoteRepo.MoveQuoteDataToArchive();
            return Ok();
        }

    }
}
