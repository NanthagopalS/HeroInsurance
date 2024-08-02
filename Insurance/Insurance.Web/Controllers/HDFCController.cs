using Insurance.Web.Abstraction;
using Insurance.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Web.Controllers;

public class HDFCController : Controller
{
    private readonly IInsuranceService _insuranceService;
    private readonly HeroConfig _heroConfig;
    private readonly ILogger<HDFCController> _logger;
    public HDFCController(IInsuranceService insuranceService, IOptions<HeroConfig> heroConfig, ILogger<HDFCController> logger)
    {
        _insuranceService = insuranceService ?? throw new ArgumentNullException(nameof(insuranceService));
        _heroConfig = heroConfig.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task<IActionResult> GetCKCYPOAStatus([FromRoute] string id,[FromRoute] string userId, [FromQuery] string txnId, [FromQuery] string status, [FromQuery] string kyc_id, CancellationToken cancellationToken)
    {
        HDFCPOAResponse hdfcResponse = new HDFCPOAResponse
        {
            id = id,
            userId = userId,
            TxnId = txnId,
            Status = status,
            KYCId = kyc_id
        };
        _logger.LogInformation("HDFC GetCKCYPOAStatus ICResponse {response}", JsonConvert.SerializeObject(hdfcResponse));
        var response = await _insuranceService.GetHDFCPOAStatus(hdfcResponse, cancellationToken);
        return Redirect($"{_heroConfig.CKYCRedirectionURL}/{response.data.insurerId}/{response.data.transactionId}");
    }

    public async Task<IActionResult> SavePaymentStatus([FromRoute] string id, [FromRoute] string userId, [FromForm] string hdnmsg, CancellationToken cancellationToken)
    {
        HDFCPaymentResponseModel hdfcPaymentResponse = new HDFCPaymentResponseModel();
        if(hdnmsg!= null)
        {
            string[] data = hdnmsg.Split('|');
            if (data != null && data.Any() && data.Length >= 13)
            {
                hdfcPaymentResponse.MerchantId = data[0];
                hdfcPaymentResponse.TransactionNo = data[1];
                hdfcPaymentResponse.TransctionRefNo = data[2];
                hdfcPaymentResponse.BankReferenceNo = data[3];
                hdfcPaymentResponse.TxnAmount = data[4];
                hdfcPaymentResponse.BankCode = data[5];
                hdfcPaymentResponse.IsSIOpted = data[6];
                hdfcPaymentResponse.PaymentMode = data[7];
                hdfcPaymentResponse.PG_Remarks = data[8];
                hdfcPaymentResponse.PaymentStatus = data[9];
                hdfcPaymentResponse.TransactionDate = data[10].Split(" ")[0];
                hdfcPaymentResponse.AppID= data[11];
                hdfcPaymentResponse.CheckSum= data[12];
            }
        }
        _logger.LogInformation("HDFC SavePaymentStatus ICResponse {response}", JsonConvert.SerializeObject(hdfcPaymentResponse));
        var response = await _insuranceService.SaveHDFCPaymentStatus(id,userId,hdfcPaymentResponse, cancellationToken);
        return Redirect($"{_heroConfig.PGPaymentURL}/{response.data.result.insurerId}/{response.data.result.quoteTransactionId}");
    }
    public IActionResult SubmitPayment([FromRoute] string id, [FromRoute] string userId, [FromQuery] string Trnsno, [FromQuery] string Amt, [FromQuery] string Chksum, CancellationToken cancellationToken)
    {
        HDFCPaymentRequestModel hdfcPaymentRequest = new HDFCPaymentRequestModel();
        hdfcPaymentRequest.Trnsno = Trnsno;
        hdfcPaymentRequest.Amt = Amt;
        hdfcPaymentRequest.Chksum = Chksum;
        hdfcPaymentRequest.Appid = _heroConfig.HDFCPaymentAppId;
        hdfcPaymentRequest.Subid = _heroConfig.HDFCPaymentSubcriptionId;
        hdfcPaymentRequest.StatusURL = $"{_heroConfig.HDFCPGStatusRedirectionURL}{id}/{userId}";
        hdfcPaymentRequest.CheckSumURL = _heroConfig.HDFCCheckSumURL;

        _logger.LogInformation("HDFC SubmitPayment CheckSumView {request}", JsonConvert.SerializeObject(hdfcPaymentRequest));
        return View(hdfcPaymentRequest);
    }
}
