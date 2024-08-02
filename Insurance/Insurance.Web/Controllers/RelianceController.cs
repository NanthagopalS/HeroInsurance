using Insurance.Web.Abstraction;
using Insurance.Web.Implementation;
using Insurance.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Web.Controllers;

public class RelianceController : Controller
{
    private readonly IInsuranceService _insuranceService;
    private readonly HeroConfig _heroConfig;
    private readonly ILogger<InsuranceService> _logger;
    public RelianceController(IInsuranceService insuranceService, IOptions<HeroConfig> heroConfig, ILogger<InsuranceService> logger)
    {
        _insuranceService = insuranceService ?? throw new ArgumentNullException(nameof(insuranceService));
        _heroConfig = heroConfig.Value;
        _logger = logger;
    }

    public async Task<IActionResult> GetCKYCPOAStatus([FromRoute] string id, [FromRoute] string userId, CancellationToken cancellationToken)
    {
        RelianceKYCResponse relianceKYCResponse = new RelianceKYCResponse
        {
            QuoteTransactionId = id,
            UserId = userId,
            UniqueId = Request.Form["unique_id"],
            RegisteredName = Request.Form["registered_name"],
            FirstName = Request.Form["first_name"],
            MiddleName = Request.Form["middle_name"],
            LastName = Request.Form["last_name"],
            DOB = Request.Form["dob"],
            Gender = Request.Form["gender"],
            Mobile = Request.Form["mobile"],
            Email = Request.Form["email"],
            KYCNumber = Request.Form["ckyc_number"],
            CorrAddressLine1 = Request.Form["corr_address_line1"],
            CorrAddressLine2 = Request.Form["corr_address_line2"],
            CorrCity = Request.Form["corr_address_city"],
            CorrState = Request.Form["corr_address_state"],
            CorrCountry = Request.Form["corr_address_country"],
            CorrPinCode = Request.Form["corr_address_pincode"],
            PerAddressLine1 = Request.Form["per_address_line1"],
            PerAddressLine2 = Request.Form["per_address_line2"],
            PerCity = Request.Form["per_address_city"],
            PerState = Request.Form["per_address_state"],
            PerCountry = Request.Form["per_address_country"],
            PerPinCode = Request.Form["per_address_pincode"],
            ProposalId = Request.Form["proposal_id"],
            KYCVerified = Request.Form["kyc_verified"],
            VerifiedAt = Request.Form["verified_at"],
            KYCProcess = Request.Form["kyc_process"],
            UDP1 = Request.Form["UDP1"],
            UDP2 = Request.Form["UDP2"],
            UDP3 = Request.Form["UDP3"],
            UDP4 = Request.Form["UDP4"],
            UDP5 = Request.Form["UDP5"]
        };
        var response = await _insuranceService.GetRelianceCKYCDetails(relianceKYCResponse, cancellationToken);
        return Redirect($"{_heroConfig.CKYCRedirectionURL}/{response.data.insurerId}/{response.data.transactionId}");
    }


    public async Task<IActionResult> SavePaymentStatus([FromRoute] string id, [FromRoute] string userId, CancellationToken cancellationToken)
    {
        var productCode = TempData["ProductCode"] != null ? TempData["ProductCode"].ToString() : "2311";
        ReliancePaymentResponseModel reliancePaymentResponseModel = new ReliancePaymentResponseModel();
        _logger.LogInformation("Reliance SavePaymentStatus Response {Response}", Request.QueryString.Value);
        if (Request.QueryString.Value != null)
        {
            string[] data = Request.QueryString.Value.Split('|');
            if (data != null && data.Any())
            {
                reliancePaymentResponseModel.StatusID = data[0].Replace("?Output=", "");
                reliancePaymentResponseModel.PoliCyNumber = data[1];
                reliancePaymentResponseModel.TransactionNumber = data[2];
                reliancePaymentResponseModel.OptionalValue = data[3];
                reliancePaymentResponseModel.GatewayName = data[4];
                reliancePaymentResponseModel.ProposalNumber = data[5];
                reliancePaymentResponseModel.TransactionStatus = data[6];
            }
        }
        reliancePaymentResponseModel.ProductCode = productCode;
        var response = await _insuranceService.SaveReliancePaymentStatus(id, userId, reliancePaymentResponseModel, cancellationToken);
        return Redirect($"{_heroConfig.PGPaymentURL}/{response.InsurerId}/{response.QuoteTransactionId}");
    }

    public IActionResult SubmitPayment([FromRoute] string id, [FromRoute] string userId, [FromQuery] string ProposalNumber, [FromQuery] string Amt, [FromQuery] string Url, [FromQuery] string Pan, [FromQuery] string ckyc, [FromQuery] string productCode)
    {
        var redirectionUrl = _heroConfig.ReliancePartnerRedirectionURL + ProposalNumber + "&userID=" + _heroConfig.ReliancePartnerCode + "&ProposalAmount=" + Amt + "&PaymentType=" + _heroConfig.ReliancePaymentType + "&Responseurl=" + Url;

        ReliancePaymentRequestModel reliancePaymentRequest = new ReliancePaymentRequestModel();
        reliancePaymentRequest.Trnsno = ProposalNumber;
        reliancePaymentRequest.Amt = Amt;
        reliancePaymentRequest.Url = _heroConfig.RelianceBillDeskURL;
        reliancePaymentRequest.RedirectionUrl = Url;
        reliancePaymentRequest.PaymentId = ProposalNumber;
        reliancePaymentRequest.PanNumber = Pan;
        reliancePaymentRequest.UserId = _heroConfig.ReliancePartnerCode;
        reliancePaymentRequest.CKYCNumber = ckyc;
        TempData["ProductCode"] = productCode;
        reliancePaymentRequest.StatusURL = $"{_heroConfig.ReliancePGStatusRedirectionURL}{id}/{userId}";
        _logger.LogInformation("Reliance SubmitPayment Request {request}", JsonConvert.SerializeObject(reliancePaymentRequest));
        return View(reliancePaymentRequest);
    }
}
