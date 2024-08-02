using Insurance.Web.Abstraction;
using Insurance.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Insurance.Web.Controllers;
public class ICICIController : Controller
{
    private readonly IInsuranceService _insuranceService;
    private readonly HeroConfig _heroConfig;

    public ICICIController(IInsuranceService insuranceService, IOptions<HeroConfig> options)
    {
        _insuranceService = insuranceService ?? throw new ArgumentNullException(nameof(insuranceService));
        _heroConfig = options.Value;
    }

    public async Task<IActionResult> SavePaymentStatus([FromRoute] string id,
        [FromRoute] string userId)
    {
        var transactionId = HttpContext.Request.Form["TransactionId"][0];
        var response = await _insuranceService.SaveICICIPaymentStatus(userId, id, transactionId);

        return Redirect($"{_heroConfig.PGPaymentURL}/{response.InsurerId}/{response.QuoteTransactionId}");
    }
}
