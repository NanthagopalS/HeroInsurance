using System.Net.Mail;
using ThirdPartyUtilities.Models.Email;
using ThirdPartyUtilities.Models.ManualPolicy;
using ThirdPartyUtilities.Models.PanRejectionModels;
using ThirdPartyUtilities.Models.SMS;

namespace ThirdPartyUtilities.Abstraction;
public interface IEmailService
{
    Task<string> SendVerificationEmail(string toEmailId, string userId, CancellationToken cancellationToken);
    Task<bool> SendWelcomeMail(string UserId, string EmailId, string UserName, string POSPId, string MobileNo, CancellationToken cancellationToken);
    Task<string> ResetPasswordEmail(string emailId, string userId, CancellationToken cancellationToken);
    Task<bool> SendUserCredentialMail(string emailId, string password, CancellationToken cancellationToken);
    Task<bool> SendEmailToReferralUser(string emailId, string referralId, string userName, string POSPName, CancellationToken cancellationToken);

    Task<bool> SendEmailForNOC(EmailForNOCModel emailForNOCModel, CancellationToken ctoken);

    Task<bool> SendEmailForRenewalsNotification(string toEmailId, string insuranceType, string customerName, string contactSupportNumber, string policyNumber, CancellationToken cancellationToken);
    Task<bool> SendBuyJourneyNotification(SendNotificationRequestModel sendNotificationRequest, CancellationToken cancellationToken);
    Task<bool> SendEmailForCompletedPOSP(string Username, string EmailId, string MobileNo, string POSPId, CancellationToken cancellationToken);
    Task<string> SendPasswordResetEmail(string userId, string EmailId, string password, CancellationToken cancellationToken);
    Task<bool> SendManualPolicyReport(ManualPolicyEmailRequest manualPolicyEmailRequest, List<Attachment> attachmentPaths, CancellationToken cancellationToken);
    Task<bool> SendPanVerificationFailedEmail(ResetUserDetailsForPanVerificationEmailModel resetUserDetailsForPanVerification, CancellationToken cancellationToken);
}
