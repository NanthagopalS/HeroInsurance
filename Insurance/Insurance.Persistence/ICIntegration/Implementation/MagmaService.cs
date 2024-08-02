using Insurance.Domain.Bajaj;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.HDFC;
using Insurance.Domain.Magma;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Persistence.ICIntegration.Implementation;

public class MagmaService : IMagmaService
{
    private readonly HttpClient _client;
    private readonly ILogger<MagmaService> _logger;
    private readonly MagmaConfig _magmaConfig;
    public MagmaService(ILogger<MagmaService> logger, HttpClient client, IOptions<MagmaConfig> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _magmaConfig = options.Value;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="quoteQuery"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
    {
        var quoteVm = new QuoteResponseModel();
        string requestBody = string.Empty;
        var responseBody = string.Empty;
        bool isVehicleNew = false;
        try
        {
            quoteVm.InsurerName = "Bajaj";
            var dateTime = DateTime.Today;
            string policyStartDate = string.Empty;
            if (quoteQuery.PreviousPolicyDetails != null && !string.IsNullOrEmpty(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD))
            {
                DateTime previosPolicyEndDate = Convert.ToDateTime(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD);
                if (previosPolicyEndDate.CompareTo(dateTime) < 0)
                {
                    policyStartDate = Convert.ToDateTime(dateTime).ToString("dd-MMM-yyyy");
                }
                else
                {
                    policyStartDate = Convert.ToDateTime(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD).AddDays(1).ToString("dd-MMM-yyyy");
                }
            }
            else
            {
                isVehicleNew = true;
                policyStartDate = dateTime.ToString("dd-MMM-yyyy");
            }
            string policyEndDate = Convert.ToDateTime(policyStartDate).AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
            var firstDay = new DateTime(dateTime.Year, dateTime.Month, 1);
            string registrationDate = $"{firstDay:dd}-{dateTime:MMM}-{quoteQuery.RegistrationYear}";
            string manYear = quoteQuery.RegistrationYear;

            var magmarequest = new MagmaServiceRequestModel()
            {

            };

            requestBody = JsonConvert.SerializeObject(magmarequest);

            var responseMessage = await _client.PostAsJsonAsync(_magmaConfig.QuoteURL, magmarequest)
        ;

            if (!responseMessage.IsSuccessStatusCode)
            {
                responseBody = responseMessage.ReasonPhrase;
                quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogError("Data not found {responseBody}", responseBody);
            }
            else
            {
                var stream = await responseMessage.Content.ReadAsStreamAsync();
                var result = stream.DeserializeFromJson<MagmaServiceResponseModel>();
                responseBody = JsonConvert.SerializeObject(result);
                _logger.LogInformation(responseBody);

                quoteVm.PolicyStartDate = Convert.ToDateTime(policyStartDate).ToString("dd-MMM-yyyy");
                quoteVm.Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + (quoteQuery.VehicleODTenure).ToString() + " TP";
                quoteVm.PlanType = quoteQuery.PlanType;

            }

        }
        catch (Exception ex)
        {

            _logger.LogError("Magma Error {exception}", ex.Message);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return Tuple.Create(quoteVm, requestBody, responseBody);
        }
        return Tuple.Create(quoteVm, requestBody, responseBody);
    }
}

