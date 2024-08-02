using Insurance.Web.Abstraction;
using Insurance.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Web.Controllers;

public class IFFCOController : Controller
{

    private readonly IInsuranceService _insuranceService;
    private readonly HeroConfig _heroConfig;
    private readonly ILogger<IFFCOController> _logger;


    public IFFCOController(IInsuranceService insuranceService, IOptions<HeroConfig> options, ILogger<IFFCOController> logger)
    {
        _insuranceService = insuranceService ?? throw new ArgumentNullException(nameof(insuranceService));
        _heroConfig = options.Value;
        _logger = logger;
    }

    public async Task<IActionResult> SubmitIFFCOPayment([FromRoute] string id, [FromRoute] string userId, [FromQuery] string uniqId)
    {
        var response = await _insuranceService.GetProposalDetails(_heroConfig.IFFCOInsurerId, id, userId, new CancellationToken());
        if (response.statusCode == "200")
        {
            var result = response.data.result;
            IFFCOSubmitPaymentModel iFFCOSubmitPayment = new IFFCOSubmitPaymentModel();
            iFFCOSubmitPayment.ProposalRequest = result.requestBody ;
            iFFCOSubmitPayment.UniqId = uniqId;
            iFFCOSubmitPayment.ReturnURL = _heroConfig.IFFCOPaymentResponseURL;
            iFFCOSubmitPayment.PaymentURL = result.vehicleTypeId != "88a807b3-90e4-484b-b5d2-65059a8e1a91" ? _heroConfig.IFFCOPaymentURL : _heroConfig.IFFCOPaymentURLForCv;
            iFFCOSubmitPayment.PartnerCode = result.vehicleTypeId != "88a807b3-90e4-484b-b5d2-65059a8e1a91" ? _heroConfig.IFFCOPartnerCode : _heroConfig.IFFCOCVPartnerCode;

            _logger.LogInformation("IFFCO SubmitPayment {request}", JsonConvert.SerializeObject(iFFCOSubmitPayment));
            return View(iFFCOSubmitPayment);

        }
        else
        {
            _logger.LogInformation("IFFCO SubmitPayment {request} failed", JsonConvert.SerializeObject(response));
            return View("Something Went Wrong.");
        }
    }

    public async Task<IActionResult> GetPaymentStatus(CancellationToken cancellationToken)
    {
        var response = (dynamic)null;
        IFFCOPaymentResponseModel iFFCOPaymentResponseModel = new IFFCOPaymentResponseModel();
        if (Request.QueryString.Value != null)
        {
            string[] data = Request.QueryString.Value.Split('|');
            if (data != null && data.Any())
            {
                var product = data[0]?.Split('=')[1];
                iFFCOPaymentResponseModel.Product = product;
                iFFCOPaymentResponseModel.TransactionId = data[1];
                iFFCOPaymentResponseModel.PolicyNumber = data[2];
                iFFCOPaymentResponseModel.PremiumPayable = data[3];
                iFFCOPaymentResponseModel.StatusMessage = data[4];
                iFFCOPaymentResponseModel.ProposalNumber = data[5];
            }
            _logger.LogInformation("IFFCO Payment Response {response}", Request.QueryString.Value);
        }
        var userId = await _insuranceService.GetUserIdDetails(iFFCOPaymentResponseModel.ProposalNumber, cancellationToken);
        if (userId != null)
        {
            iFFCOPaymentResponseModel.UserId = userId;
            response = await _insuranceService.SaveIFFCOPaymentStatus(iFFCOPaymentResponseModel, cancellationToken);
        }
        return Redirect($"{_heroConfig.PGPaymentURL}/{response.data.InsurerId}/{response.data.QuoteTransactionId}");
    }
}
