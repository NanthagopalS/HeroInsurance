using ThirdPartyUtilities.Models.SMS;

namespace ThirdPartyUtilities.Abstraction;
public interface ISmsService
{
    Task<SMSResponse> SendSMS(string mobileNumber, CancellationToken cancellationToken);
    Task<SMSReferralResponse> SendSMSToReferralUser(string mobileNumber, string userName, string pOSPName, string referralId, CancellationToken cancellationToken);

    Task<SMSResponse> SendSMSForRenewalNotification(RenewalSMSModel renewalSMSModel, CancellationToken cancellationToken);
    Task<SMSResponse> SendICNotification(SendNotificationRequestModel sendNotificationRequest, CancellationToken cancellationToken);
    Task<SMSResponse> SendCholaBreakinNotification(string mobileNumber, string url, string breakinId, string insurerName, CancellationToken cancellationToken);
}
