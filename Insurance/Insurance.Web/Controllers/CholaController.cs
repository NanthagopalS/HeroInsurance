using Insurance.Web.Abstraction;
using Insurance.Web.Implementation;
using Insurance.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Insurance.Web.Controllers;

public class CholaController : Controller
{
    private readonly IInsuranceService _insuranceService;
    private readonly HeroConfig _heroConfig;
    private readonly ILogger<InsuranceService> _logger;
    public CholaController(IInsuranceService insuranceService, IOptions<HeroConfig> heroConfig, ILogger<InsuranceService> logger)
    {
        _insuranceService = insuranceService ?? throw new ArgumentNullException(nameof(insuranceService));
        _heroConfig = heroConfig.Value;
        _logger = logger;
    }

    public async Task<IActionResult> SavePaymentStatus([FromRoute] string id, [FromRoute] string userId, [FromForm] string msg,  CancellationToken cancellationToken)
    {
        CholaPaymentResponseModel cholaPaymentResponseModel = new CholaPaymentResponseModel();
        _logger.LogInformation("Chola SavePaymentStatus Response {Response}", msg);
        StringBuilder checkSum = new StringBuilder();
        string checkSumValidate = string.Empty;
        if (msg != null)
        {
            string[] data = msg.Split('|');
            if (data != null && data.Any() && data.Length >= 25)
            {
                cholaPaymentResponseModel.MerchantID = data[0];
                cholaPaymentResponseModel.UniqueTxnID = data[1];
                cholaPaymentResponseModel.TxnReferenceNo = data[2];
                cholaPaymentResponseModel.BankReferenceNo = data[3];
                cholaPaymentResponseModel.TxnAmount = data[4];
                cholaPaymentResponseModel.BankID = data[5];
                cholaPaymentResponseModel.BIN = data[6];
                cholaPaymentResponseModel.TxnType = data[7];
                cholaPaymentResponseModel.CurrencyName = data[8];
                cholaPaymentResponseModel.ItemCode = data[9];
                cholaPaymentResponseModel.SecurityType = data[10];
                cholaPaymentResponseModel.SecurityID = data[11];
                cholaPaymentResponseModel.SecurityPassword = data[12];
                cholaPaymentResponseModel.TxnDate = data[13];
                cholaPaymentResponseModel.AuthStatus = data[14];
                cholaPaymentResponseModel.SettlementType = data[15];
                cholaPaymentResponseModel.AdditionalInfo1 = data[16];
                cholaPaymentResponseModel.AdditionalInfo2 = data[17];
                cholaPaymentResponseModel.AdditionalInfo3 = data[18];
                cholaPaymentResponseModel.AdditionalInfo4 = data[19];
                cholaPaymentResponseModel.AdditionalInfo5 = data[20];
                cholaPaymentResponseModel.AdditionalInfo6 = data[21];
                cholaPaymentResponseModel.AdditionalInfo7 = data[22];
                cholaPaymentResponseModel.ErrorStatus = data[23];
                cholaPaymentResponseModel.ErrorDescription = data[24];
                cholaPaymentResponseModel.CheckSum = data[25];
            }
            for (int i = 0; i < data.Length - 1 ; i++)
            {
                checkSum.Append(data[i]).Append("|");
                checkSumValidate = checkSum.ToString().TrimEnd('|');
            }

            // Calling Validate Checksum 
            var result = PaymentURLGeneration(checkSumValidate, cholaPaymentResponseModel.CheckSum);
        }
        var response = await _insuranceService.SaveCholaPaymentStatus(id,userId, cholaPaymentResponseModel, cancellationToken);
        return Redirect($"{_heroConfig.PGPaymentURL}/{response.InsurerId}/{response.QuoteTransactionId}");
    }

    private string PaymentURLGeneration(string checkSumValidate, string checksumId)
    {
        byte[] message = Encoding.UTF8.GetBytes(checkSumValidate);
        byte[] hashValue;
        byte[] key = Encoding.UTF8.GetBytes("G3eAmyVkAzKp8jFq0fqPEqxF4agynvtJ");
        HMACSHA256 hashString = new HMACSHA256(key);
        string hex = "";
        hashValue = hashString.ComputeHash(message);
        foreach (byte x in hashValue)
        {
            hex += String.Format("{0:x2}", x);
        }
        return hex.ToUpper();
    }

    public async Task<IActionResult> GetCKYCStatus([FromRoute] string id, [FromRoute] string userId, [FromQuery] string transactionId, [FromQuery] string status, CancellationToken cancellationToken)
    {
        CholaKYCResponse cholaKYCResponse = new CholaKYCResponse
        { 
            quoteTransactionId = id,
            userId = userId,
            transactionId = transactionId,
            status = status
        };
        var response = await _insuranceService.GetCholaCKYCDetails(cholaKYCResponse, cancellationToken);
        return Redirect($"{_heroConfig.CKYCRedirectionURL}/{response.data.insurerId}/{response.data.transactionId}");
    }
}
