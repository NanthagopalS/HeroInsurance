using Insurance.Web.Abstraction;
using Insurance.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace Insurance.Web.Controllers
{
    public class UnitedIndiaController : Controller
    {
        private readonly IInsuranceService _insuranceService;
        private readonly HeroConfig _heroConfig;
        private readonly ILogger<UnitedIndiaController> _logger;
        public UnitedIndiaController(IInsuranceService insuranceService, IOptions<HeroConfig> heroConfig, ILogger<UnitedIndiaController> logger)
        {
            _insuranceService = insuranceService ?? throw new ArgumentNullException(nameof(insuranceService));
            _heroConfig = heroConfig.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> SavePaymentStatus([FromRoute] string id, [FromRoute] string userId,[FromRoute] string refnumber, CancellationToken cancellationToken)
        {
            UnitedIndiaPaymentResponseModel unitedIndiaPaymentResponse = new UnitedIndiaPaymentResponseModel()
            {
                QuoteTransactionId = id,
                orderId = HttpContext?.Request?.Form?["ORDERID"][0],
                num_reference_number = refnumber
            };

            _logger.LogInformation("UnitedIndia SavePaymentStatus ICResponse {response}", JsonConvert.SerializeObject(unitedIndiaPaymentResponse));
            var response = await _insuranceService.SaveUnitedIndiaPaymentStatus(userId, unitedIndiaPaymentResponse, cancellationToken);
            return Redirect($"{_heroConfig.PGPaymentURL}/{response?.data?.InsurerId}/{response?.data?.QuoteTransactionId}");
        }
        public IActionResult InvokePayment([FromQuery] string orderId, [FromQuery] string token, [FromQuery] string amount, CancellationToken cancellationToken)
        {
            UnitedIndiaPaymentRequestModel unitedIndiaPaymentRequest = new UnitedIndiaPaymentRequestModel()
            {
                HOST = _heroConfig.UnitedIndiaPaymentBaseURL,
                MID = _heroConfig.UnitedIndiaMID,
                orderId = orderId,
                token = token,
                amount = amount,
            };
            _logger.LogInformation("UnitedIndia InvokePayment HTMLView {request}", JsonConvert.SerializeObject(unitedIndiaPaymentRequest));
            return View(unitedIndiaPaymentRequest);
        }

        public async Task<IActionResult> GetCKYCStatus([FromRoute] string id, [FromRoute] string userId, [FromQuery] string transactionId, [FromQuery] string status, CancellationToken cancellationToken)
        {
            UnitedIndiaCKYCResponseModel unitedIndiaCKYCResponseModel = new UnitedIndiaCKYCResponseModel()
            {
                QuoteTransactionId = id,
                UserId = userId,
                TrannsactionId = transactionId,
                Status=  status
            };

            _logger.LogInformation("UnitedIndia CKYC ICResponse {response}", JsonConvert.SerializeObject(unitedIndiaCKYCResponseModel));
            var response = await _insuranceService.SaveUnitedIndiaCKYCStatus(unitedIndiaCKYCResponseModel, cancellationToken);
            if (response != null)
            {
                return Redirect($"{_heroConfig.UIICCKYCRedirectionURL}?InsurerId={_heroConfig.UIICInsurerId}&QuotetransactionId={response.QuoteTransactionID}&VehicleNumber={response.VehicleNumber}&VariantId={response.VariantId}&CKYCStatus={response.CKYCstatus}&InsurerName={_heroConfig.UIICInsurerName}");
            }
            else
            {
                _logger.LogInformation("UIIC SavCKYC POA Response Failed {response}", JsonConvert.SerializeObject(response));
                return View("Something Went Wrong.");
            }
        }
    }
}
