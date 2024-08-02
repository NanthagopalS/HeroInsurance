using Insurance.Web.Abstraction;
using Insurance.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Security.Cryptography.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace Insurance.Web.Controllers;

public class BajajController : Controller
{
    private readonly IInsuranceService _insuranceService;
    private readonly HeroConfig _heroConfig;
    public BajajController(IInsuranceService insuranceService, IOptions<HeroConfig> heroConfig)
    {
        _insuranceService = insuranceService ?? throw new ArgumentNullException(nameof(insuranceService));
        _heroConfig = heroConfig.Value;
    }
     

    public async Task<IActionResult> SavePaymentStatus(
        [FromRoute] string id,
        [FromRoute] string userId,
        [FromQuery] string p_policy_ref,
        [FromQuery] string vg_payStatus,
        [FromQuery] string p_pay_status,
        [FromQuery] string policyref,
        [FromQuery] string requestId,
        [FromQuery] string custName,
        CancellationToken cancellationToken)
    {
        BajajPaymentResponseModel bajajPaymentResponseModel = new BajajPaymentResponseModel()
        {
            id = id,
            userId = userId,
            p_policy_ref = p_policy_ref,
            vg_payStatus = vg_payStatus,
            p_pay_status = p_pay_status,
            policyref = policyref.ToLower().Equals("null") ? null : policyref,
            requestId = requestId,
            custName = custName,
        };
        var response = await _insuranceService.SaveBajajPaymentStatus(bajajPaymentResponseModel, cancellationToken);

        return Redirect($"{_heroConfig.PGPaymentURL}/{response.InsurerId}/{response.QuoteTransactionId}/");
    }

    public async Task<IActionResult> SaveTPPaymentStatus(
        [FromRoute] string id,
        [FromRoute] string userId,
        [FromQuery] string status,
        [FromQuery] string amt,
        [FromQuery] string txn,
        [FromQuery] string referenceno,
        [FromQuery] string quoteno,
        CancellationToken cancellationToken)
    {
        BajajPaymentResponseModel bajajPaymentResponseModel = new BajajPaymentResponseModel()
        {
            id = id,
            userId = userId,
            status = status,
            amt = amt,
            txn = txn,
            referenceno = referenceno.ToLower().Equals("null") ? null : referenceno,
            quoteno = quoteno,
        };
        var response = await _insuranceService.SaveBajajTPPaymentStatus(bajajPaymentResponseModel, cancellationToken);

        return Redirect($"{_heroConfig.PGPaymentURL}/{response.InsurerId}/{response.QuoteTransactionId}/");
    }
}
