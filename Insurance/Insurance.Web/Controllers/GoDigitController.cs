using Insurance.Web.Abstraction;
using Insurance.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Insurance.Web.Controllers;
public class GoDigitController : Controller
{
    private readonly IInsuranceService _insuranceService;
    private readonly HeroConfig _heroConfig;

    public GoDigitController(IInsuranceService insuranceService, IOptions<HeroConfig> options)
    {
        _insuranceService = insuranceService ?? throw new ArgumentNullException(nameof(insuranceService));
        _heroConfig = options.Value;
    }

    public async Task<IActionResult> SavePaymentStatus([FromRoute] string id, [FromRoute] string userId,
                                                       [FromQuery] string transactionNumber,
                                                       CancellationToken cancellationToken)
    {
        var response = await _insuranceService.SavePaymentStatus(id, userId, transactionNumber, cancellationToken);
         
        return Redirect($"{_heroConfig.PGPaymentURL}/{response.InsurerId}/{response.QuoteTransactionId}");
    }
}
