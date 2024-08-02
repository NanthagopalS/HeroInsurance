using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;
using ThirdPartyUtilities.Models.Signzy;
using ThirdPartyUtilities.Models.SMS;

namespace ThirdPartyUtilities.Implementation;
public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;
    private readonly HttpClient _client;
    private readonly SMSConfig _smsConfig;
    private readonly IConfiguration _config;
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="logger"></param>
    public SmsService(ILogger<SmsService> logger,
        HttpClient client,
        IOptions<SMSConfig> options,
        IConfiguration iConfig)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _smsConfig = options.Value;
        _config = iConfig;
    }

    public async Task<SMSResponse> SendSMS(string mobileNumber, CancellationToken cancellationToken)
    {
        // A random number (OTP) within a range 1000 to 9999
        var randomGenerator = new Random();
        var OTP = randomGenerator.Next(1000, 9999);
        var param = new Dictionary<string, string>
        {
            { "method", _smsConfig.method },
            { "send_to", mobileNumber },
            { "msg", $"OTP is {OTP}. Hero Insurance Broking" },
            { "msg_type", _smsConfig.msg_type },
            { "userid", _smsConfig.userid },
            { "auth_scheme", _smsConfig.auth_scheme },
            { "password", _smsConfig.password },
            { "format", _smsConfig.format },
            { "v", _smsConfig.v },
        };

        var responseMessage = await _client.PostAsync("GatewayAPI/rest", new FormUrlEncodedContent(param), cancellationToken)
            .ConfigureAwait(false);

        if (!responseMessage.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send SMS");
        }
        else
        {
            var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            var sMSResponse = stream.DeserializeFromJson<SMSResponse>();
            if (sMSResponse != null)
            {
                sMSResponse.response.OTP = OTP;
                return sMSResponse;
            }
        }
        return default;
    }

    public async Task<SMSReferralResponse> SendSMSToReferralUser(string mobileNumber, string userName, string pOSPName, string referralId, CancellationToken cancellationToken)
    {

        string prefixUrl = _config.GetSection("WebUrl").GetSection("prefixUrl").Value;
        string LinkURL = prefixUrl + "#?tab=reg&referId=" + referralId;

        var param = new Dictionary<string, string>
        {
            { "method", _smsConfig.method },
            { "send_to", mobileNumber },
            { "msg", $"Dear {userName}, {pOSPName} thinks you'd make a great POSP for Hero Insurance Broking India Pvt Ltd. Click {LinkURL} to register. Team Hero Insurance Broking" },
            { "msg_type", _smsConfig.msg_type },
            { "userid", _smsConfig.userid },
            { "auth_scheme", _smsConfig.auth_scheme },
            { "password", _smsConfig.password },
            { "format", _smsConfig.format },
            { "v", _smsConfig.v },
        };
        var responseMessage = await _client.PostAsync("GatewayAPI/rest", new FormUrlEncodedContent(param), cancellationToken)
            .ConfigureAwait(false);

        if (!responseMessage.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send SMS");
        }
        else
        {
            var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            var sMSResponse = stream.DeserializeFromJson<SMSReferralResponse>();
            if (sMSResponse != null)
            {
                return sMSResponse;
            }
        }
        return default;
    }

    public async Task<SMSResponse> SendSMSForRenewalNotification(RenewalSMSModel renewalSMSModel, CancellationToken cancellationToken)
    {

        string prefixUrl = _config.GetSection("WebUrl").GetSection("prefixUrl").Value + renewalSMSModel.renewalURL;

        var param = new Dictionary<string, string>
        {
            { "method", _smsConfig.method },
            { "send_to", renewalSMSModel.mobileNumber },
            //{ "msg", $"Dear {renewalSMSModel.leadName}, your {renewalSMSModel.insuranceName} policy for {renewalSMSModel.vechicleMaker} {renewalSMSModel.vechicleModel} is expiring in {renewalSMSModel.daysLeft}! Click here {prefixUrl} to renew now. Team Hero Insurance Broking" },
            { "msg", $"Dear Customer, your insurance policy for Vehicle {renewalSMSModel.vechicleMaker} {renewalSMSModel.vechicleModel} {renewalSMSModel.vechicleVariant} is expiring soon! Click {prefixUrl} to renew now.\nTeam Hero Insurance Broking" },
            { "msg_type", _smsConfig.msg_type },
            { "userid", _smsConfig.userid },
            { "auth_scheme", _smsConfig.auth_scheme },
            { "password", _smsConfig.password },
            { "format", _smsConfig.format },
            { "v", _smsConfig.v },
        };
        var responseMessage = await _client.PostAsync("GatewayAPI/rest", new FormUrlEncodedContent(param), cancellationToken)
            .ConfigureAwait(false);

        if (!responseMessage.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send SMS");
        }
        else
        {
            var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            var sMSResponse = stream.DeserializeFromJson<SMSResponse>();
            if (sMSResponse != null)
            {
                return sMSResponse;
            }
        }
        return default;
    }
    public async Task<SMSResponse> SendICNotification(SendNotificationRequestModel sendNotificationRequest, CancellationToken cancellationToken)
    {
        var notificationList = new SendICNotificationModel();
        var smsMessage = string.Empty;
        try
        {
            string path = Path.GetFullPath("SendNotification.json");
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                notificationList = JsonConvert.DeserializeObject<SendICNotificationModel>(json);
            }
            var sms = notificationList.SMS?.Where(x => x.Type.Equals(sendNotificationRequest.Type, StringComparison.Ordinal)).Select(d => d.Body).FirstOrDefault();

            if (sms != null)
            {
                switch (sendNotificationRequest.Type)
                {
                    case "Quote":
                        smsMessage = sms.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName)
                                        .Replace("#{Link}", sendNotificationRequest.Link);
                        break;
                    case "QuoteConfirm":
                        smsMessage = sms.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName)
                                        .Replace("#{Insurer}", sendNotificationRequest.InsurerName).Replace("#{Link}", sendNotificationRequest.Link);
                        break;
                    case "KYC":
                        smsMessage = sms.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName)
                                        .Replace("#{Insurer}", sendNotificationRequest.InsurerName).Replace("#{Link}", sendNotificationRequest.Link);
                        break;
                    case "ProposalSummary":
                        smsMessage = sms.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName)
                                        .Replace("#{Premium}", sendNotificationRequest.GrossPremium).Replace("#{Link}", sendNotificationRequest.Link);
                        break;
                    case "Payment":
                        smsMessage = sms.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName)
                                        .Replace("#{Insurer}", sendNotificationRequest.InsurerName).Replace("#{Link}", sendNotificationRequest.Link);
                        break;
                    case "RenewalReminder":
                        smsMessage = sms.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName)
                                        .Replace("#{Variant}", sendNotificationRequest.VariantName).Replace("#{Link}", sendNotificationRequest.Link);
                        break;
                    case "BreakInSuccess":
                        smsMessage = sms.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName)
                                        .Replace("#{Insurer}", sendNotificationRequest.InsurerName).Replace("#{Link}", sendNotificationRequest.Link);
                        break;
                    case "BreakInFailure":
                        smsMessage = sms.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName)
                                        .Replace("#{Insurer}", sendNotificationRequest.InsurerName).Replace("#{BreakinId}", sendNotificationRequest.BreakinId)
                                        .Replace("#{POSPName}", sendNotificationRequest.POSPName).Replace("#{POSPMobile}", sendNotificationRequest.POSPMobile);
                        break;
                    case "BreakInInitiation":
                        smsMessage = sms.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName)
                                        .Replace("#{Insurer}", sendNotificationRequest.InsurerName).Replace("#{BreakinId}", sendNotificationRequest.BreakinId);
                        break;
                    default:
                        break;
                }


                var param = new Dictionary<string, string>
            {
                { "method", _smsConfig.method },
                { "send_to", sendNotificationRequest.SMSPhoneNumber },
                { "msg", smsMessage },
                { "msg_type", _smsConfig.msg_type },
                { "userid", _smsConfig.userid },
                { "auth_scheme", _smsConfig.auth_scheme },
                { "password", _smsConfig.password },
                { "format", _smsConfig.format },
                { "v", _smsConfig.v },
            };

                var responseMessage = await _client.PostAsync("GatewayAPI/rest", new FormUrlEncodedContent(param), cancellationToken)
                    .ConfigureAwait(false);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to send SMS");
                }
                else
                {
                    var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                    var sMSResponse = stream.DeserializeFromJson<SMSResponse>();
                    if (sMSResponse != null && sMSResponse.response.status.ToLower().Equals("success"))
                    {
                        return sMSResponse;
                    }
                }
            }
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError("SendICNotification Error {exception}", ex.Message);
            return default;
        }
    }
    public async Task<SMSResponse> SendCholaBreakinNotification(string mobileNumber, string url, string breakinId, string insurerName, CancellationToken cancellationToken)
    {
        var smsMessage = $"Dear Customer, Please click {url} to proceed for inspection of your vehicle with {insurerName}. Break-in ID No {breakinId}. Follow insurer's instructions to finish the process.\nTeam HIBIPL";
        var param = new Dictionary<string, string>
        {
            { "method", _smsConfig.method },
            { "send_to", mobileNumber },
            { "msg", smsMessage },
            { "msg_type", _smsConfig.msg_type },
            { "userid", _smsConfig.userid },
            { "auth_scheme", _smsConfig.auth_scheme },
            { "password", _smsConfig.password },
            { "format", _smsConfig.format },
            { "v", _smsConfig.v },
        };

        var responseMessage = await _client.PostAsync("GatewayAPI/rest", new FormUrlEncodedContent(param), cancellationToken)
            .ConfigureAwait(false);

        if (!responseMessage.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send SMS");
        }
        else
        {
            var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            var sMSResponse = stream.DeserializeFromJson<SMSResponse>();
            if (sMSResponse != null)
            {
                return sMSResponse;
            }
        }
        return default;
    }
}
