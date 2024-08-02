using Insurance.Web.Abstraction;
using Insurance.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Web.Controllers
{
    public class TATAController : Controller
    {
        private readonly IInsuranceService _insuranceService;
        private readonly HeroConfig _heroConfig;
        private readonly ILogger<TATAController> _logger;
        public TATAController(IInsuranceService insuranceService, IOptions<HeroConfig> heroConfig, ILogger<TATAController> logger)
        {
            _insuranceService = insuranceService ?? throw new ArgumentNullException(nameof(insuranceService));
            _heroConfig = heroConfig.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<IActionResult> SavePaymentStatus([FromRoute] string id, [FromRoute] string userId, [FromQuery] string proposal_no, [FromQuery] string policy_no, CancellationToken cancellationToken)
        {
            TATAPaymentResponseModel tataPaymentResponse = new TATAPaymentResponseModel()
            {
                QuoteTransactionId = id,
                ProposalNumber = proposal_no,
                PolicyNumber = policy_no
            };
            
            _logger.LogInformation("TATA SavePaymentStatus ICResponse {response}", JsonConvert.SerializeObject(tataPaymentResponse));
            var response = await _insuranceService.SaveTATAPaymentStatus(userId, tataPaymentResponse, cancellationToken);
            return Redirect($"{_heroConfig.PGPaymentURL}/{response.data.InsurerId}/{response.data.QuoteTransactionId}");
        }
        public IActionResult SubmitPayment([FromQuery] string url, [FromQuery] string pgiRequest,  CancellationToken cancellationToken)
        {
            TATAPaymentRequestModel tataPaymentRequest = new TATAPaymentRequestModel()
            {
                url = url,
                pgiRequest = pgiRequest,
            };

            _logger.LogInformation("TATA SubmitPayment HTMLView {request}", JsonConvert.SerializeObject(tataPaymentRequest));
            return View(tataPaymentRequest);
        }
    }
}
