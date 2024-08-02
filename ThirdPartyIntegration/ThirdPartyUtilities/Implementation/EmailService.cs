using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;
using ThirdPartyUtilities.Models.Email;
using ThirdPartyUtilities.Models.ManualPolicy;
using ThirdPartyUtilities.Models.PanRejectionModels;
using ThirdPartyUtilities.Models.SMS;

namespace ThirdPartyUtilities.Implementation;
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailConfig _emailConfig;
    private readonly HttpClient _client;
    private readonly IConfiguration _config;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="logger"></param>
    public EmailService(ILogger<EmailService> logger, HttpClient client, IOptions<EmailConfig> options, IConfiguration iConfig)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _emailConfig = options.Value;
        _config = iConfig;
    }

    public async Task<string> SendVerificationEmail(string toEmailId, string userId, CancellationToken cancellationToken)
    {
        Guid guId = Guid.NewGuid();
        try
        {
            string prefixUrl = _config.GetSection("WebUrl").GetSection("prefixUrl").Value;
            string LinkURL = prefixUrl + $"#/verify-user?userId=" + userId + "&guId=" + guId;

            string bodyMessage = $"Dear Partner,<br /><br />Thank you for your interest in the Hero Insurance Broking POSP program. We look forward to having you onboard. Please use this link <a target=_blank href={LinkURL}>Click Here</a> and verify your email id to continue your registration process.<br /><br />Regards,<br /><b>Hero Insurance Broking India Private Limited.</b>";

            await SendEmail(toEmailId, "Email Verification", bodyMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Email Send Error {Message}", ex.Message);
        }
        return guId.ToString();
    }

    public async Task<bool> SendWelcomeMail(string UserId, string EmailId, string UserName, string POSPId, string MobileNo, CancellationToken cancellationToken)
    {
        bool mailStatus = false;
        try
        {
            string prefixUrl = _config.GetSection("WebUrl").GetSection("prefixUrl").Value.ToString();

            string LinkURL = prefixUrl + "#";

            string bodyMessage = $"Dear {UserName},<br /><br />Thank You for choosing to become a POSP partner with Hero Insurance Broking. Your unique code is {POSPId}. Please login to <a target=_blank href={LinkURL}>Dashboard</a> to complete your onboarding journey.<br />Portal Link :<a target=_blank href={LinkURL}>Click Here</a> <br /> Login ID: {POSPId} <br />Mobile Number: {MobileNo} <br /><br />Regards,<br /><b>Hero Insurance Broking India Private Limited.</b>";
            string subject = "Welcome Email";
            mailStatus = await SendEmail(EmailId, subject, bodyMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Email Send Error {Message}", ex.Message);
        }
        return mailStatus;
    }

    private async Task<bool> SendEmail(string _Email, string _Subject, string _Body, CancellationToken cancellationToken)
    {
        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            MailMessage mail = new MailMessage();
            mail.To.Add(_Email);
            mail.From = new MailAddress(_emailConfig.FromEmail);
            mail.Subject = _Subject;
            mail.Body = _Body;
            mail.IsBodyHtml = true;

            System.Net.Mail.SmtpClient smtp = new()
            {
                Host = _emailConfig.Host,
                Port = _emailConfig.Port,
                Credentials = new NetworkCredential(_emailConfig.Username, _emailConfig.Password)
            };
            await smtp.SendMailAsync(mail, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Email sent failed: {_Email} - {_Subject} - {error Message}  ", _Email, _Subject, ex.Message.ToString());
            return false;
        }
        return true;
    }

    public async Task<string> ResetPasswordEmail(string emailId, string userId, CancellationToken cancellationToken)
    {
        Guid guId = Guid.NewGuid();
        try
        {
            string prefixUrl = _config.GetSection("WebUrl").GetSection("prefixUrl").Value;
            string LinkURL = prefixUrl + $"admin/#/reset-password?userId=" + userId + "&guId=" + guId;

            string bodyMessage = $"Dear User,<br /><br />Please use this link <a target=_blank href={LinkURL}>Click Here</a> to reset your Password. <br /><br />Regards,<br /><b>Hero Insurance Broking India Private Limited.</b>";
            string subject = "Reset Password";
            await SendEmail(emailId, subject, bodyMessage, cancellationToken);
        }
        catch (Exception)
        {
            return null;
        }
        return guId.ToString();
    }

    public async Task<bool> SendUserCredentialMail(string emailId, string password, CancellationToken cancellationToken)
    {
        bool mailStatus = false;
        try
        {
            string bodyMessage = $"Dear User,<br /><br />Please check your Credentials details to login into your account as mentioned below:<br />EmailId : {emailId} <br />Password: {password} <br /><br />Regards,<br /><b>Hero Insurance Broking India Private Limited.</b>";
            string subject = "User Credentials";
            mailStatus = await SendEmail(emailId, subject, bodyMessage, cancellationToken);
        }
        catch (Exception)
        {
            return mailStatus;
        }
        return true;
    }

    public async Task<bool> SendEmailToReferralUser(string EmailId, string referralId, string userName, string POSPName, CancellationToken cancellationToken)
    {
        bool mailStatus = false;
        try
        {
            string prefixUrl = _config.GetSection("WebUrl").GetSection("prefixUrl").Value;

            string LinkURL = prefixUrl + "#?tab=reg&referId=" + referralId;

            string bodyMessage = $"Dear {userName},<br /><br />{POSPName} has referred you to become POSP with Hero Insurance Broking India Pvt Ltd. Click  <a target=_blank href={LinkURL}>Link</a> to register and start earning with HIBIPL's POSP program.<br />For any query, please reach out to us at email ID: support@heroibil.com or call: 1800-XXX-XXXX";

            string subject = "Welcome Email";
            mailStatus = await SendEmail(EmailId, subject, bodyMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Email Send Error {Message}", ex.Message);
            return mailStatus;
        }
        return mailStatus;
    }
    public async Task<bool> SendEmailForNOC(EmailForNOCModel emailForNOCModel, CancellationToken cancellationToken)
    {
        bool mailStatus = false;
        DateTime jDate = DateTimeExtensions.ConvertToDateTime(emailForNOCModel.JoiningDate);
        string joiningDD = jDate.Day + "/" + jDate.Month + "/" + jDate.Year;
        string realivingDD = emailForNOCModel.ReleavingData.Day + "/" + emailForNOCModel.ReleavingData.Month + "/" + emailForNOCModel.ReleavingData.Year;
        string emailDD = emailForNOCModel.EmailDate.Day + "/" + emailForNOCModel.EmailDate.Month + "/" + emailForNOCModel.EmailDate.Year;

        try
        {
            string bodyMessage =
        $"<center><p style='display: flex; margin-left:38%; margin-bottom: 0px; text-decoration: underline; color: #426092; font-weight: bold;'>TO WHOMSOEVER IT MAY CONCERN</p></center>" +
        $"<p style='font-size: larger;  margin: 8px 0;'>This is to Certify that <span style='color: #4472C7;  font-weight: bold;'>{emailForNOCModel.Name}</span> " +
        $"bearing Permanent Account Number <span style='color: #4472C7;  font-weight: bold;'>{emailForNOCModel.PanNumber}</span> " +
        $"resident of <span style='color: #4472C7;  font-weight: bold;'>{emailForNOCModel.Address}</span> was POS (General Insurance and Life Insurance) from " +
        $"<span style='color: #4472C7; font-weight: bold;'>{joiningDD}</span> to " +
        $"<span style='color: #4472C7 ; font-weight: bold;'>{realivingDD}.</span></p><p style='font-size: larger;  margin: 8px 0;'>" +
        $" Further, We have no objection to <span style='color: #4472C7;  font-weight: bold;'>{emailForNOCModel.Name}</span> entering any other business relationship.</p>" +
        $"<p style='margin-bottom: 5px; font-size: larger;  margin: 8px 0;'>The POS Code with Hero Insurance Broking India Private Limited has " +
        $"been terminated with effect from <span style='color: #4472C7;  font-weight: bold;'>{emailDD}</span>.</p>" +
        $"<p style='font-size: larger;  margin: 8px 0; font-weight: bold;'> From Hero Insurance Broking India Private Limited</p>";

            string subject = $"Confirmation of No Objection and Release of PAN – {emailForNOCModel.Name} - {emailForNOCModel.PospId}";
            mailStatus = await SendEmail(emailForNOCModel.ToEmailId, subject, bodyMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Email Send Error {Message}", ex.Message);
            return mailStatus;
        }
        return mailStatus;
    }

    public async Task<bool> SendEmailForRenewalsNotification(string toEmailId, string insuranceType, string customerName, string contactSupportNumber, string policyNumber, CancellationToken cancellationToken)
    {
        bool mailStatus = false;
        try
        {
            string bodyMessage =
        $"<p style='display: flex; margin-bottom: 0px; font-weight: bold;'>Dear {customerName}</p><p style='font-size: larger;  margin: 8px 0;'>This is a friendly reminder that your insurance policy {policyNumber} is expiring soon. It's important to keep your policy current to ensure that you're protected against any unforeseen circumstances.You can contact us at <a href='tel:{contactSupportNumber}'>{contactSupportNumber}</a> and our customer service agent will renew your policy in simple steps.Thank you for choosing us and we're looking forward to serving you further.</p><p style='font-size: larger;  margin: 8px 0;'>Best Regards,</p><p style='margin-bottom: 5px; font-size: larger;  margin: 8px 0;'><b>Team Hero Insurance Broking</b></p>";
            string _subject = $"{insuranceType} Policy Expiring Soon | Renew NOW!";
            mailStatus = await SendEmail(toEmailId, _subject, bodyMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Email Send Error {Message}", ex.Message);
            return mailStatus;
        }
        return mailStatus;
    }

    public async Task<bool> SendEmailForCompletedPOSP(string Username, string EmailId, string MobileNo, string POSPId, CancellationToken cancellationToken)
    {
        bool mailStatus = false;
        try
        {
            string prefixUrl = _config.GetSection("WebUrl").GetSection("prefixUrl").Value.ToString();

            string LinkURL = prefixUrl + "#";

            string bodyMessage = $"Dear {Username},<br /><br />Thank You for choosing to become a POSP partner with Hero Insurance Broking. Your unique code is {POSPId}. Please login to <a target=_blank href={LinkURL}>Dashboard</a> with your registered mobile number to complete your onboarding journey.<br />Portal Link :<a target=_blank href={LinkURL}>Click Here</a> <br /> <br />Regards,<br /><b>Hero Insurance Broking India Private Limited.</b>";
            string subject = "Welcome Email";
            mailStatus = await SendEmail(EmailId, subject, bodyMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Email Send Error {Message}", ex.Message);
        }
        return mailStatus;
    }

    public async Task<bool> SendBuyJourneyNotification(SendNotificationRequestModel sendNotificationRequest, CancellationToken cancellationToken)
    {
        bool mailStatus = false;
        var emailBody = string.Empty;
        var emailSubject = string.Empty;
        try
        {
            var notificationList = new SendICNotificationModel();
            string path = Path.GetFullPath("SendNotification.json");
            using (var streamReader = new StreamReader(path))
            {
                string json = streamReader.ReadToEnd();
                notificationList = JsonConvert.DeserializeObject<SendICNotificationModel>(json);
            }
            if (notificationList != null)
            {
                var email = notificationList.Email?.Where(x => x.Type.Equals(sendNotificationRequest.Type, StringComparison.Ordinal)).Select(d => d.Body).FirstOrDefault();
                var subjectFraming = notificationList.Email?.Where(x => x.Type.Equals(sendNotificationRequest.Type)).Select(d => d.Subject).FirstOrDefault();
                var mfgYear = Convert.ToDateTime(sendNotificationRequest.ManufacturingYear).ToString("yyyy-MM-dd").Split("-");
                switch (sendNotificationRequest.Type)
                {
                    case "Quote":
                        emailBody = email.Replace("#{CustomerName}", sendNotificationRequest.CustomerName).Replace("#{Make}", sendNotificationRequest.MakeName)
                            .Replace("#{Model}", sendNotificationRequest.ModelName).Replace("#{Variant}", sendNotificationRequest.VariantName)
                            .Replace("#{ManufacturingYear}", mfgYear[0]).Replace("#{Link}", sendNotificationRequest.Link);
                        emailSubject = subjectFraming.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName);
                        break;
                    case "QuoteConfirm" or "KYC":
                        emailBody = email.Replace("#{CustomerName}", sendNotificationRequest.CustomerName).Replace("#{Make}", sendNotificationRequest.MakeName)
                            .Replace("#{Model}", sendNotificationRequest.ModelName).Replace("#{Variant}", sendNotificationRequest.VariantName)
                            .Replace("#{ManufacturingYear}", mfgYear[0]).Replace("#{Link}", sendNotificationRequest.Link).Replace("#{Insurer}", sendNotificationRequest.InsurerName);
                        emailSubject = subjectFraming.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName)
                            .Replace("#{Insurer}", sendNotificationRequest.InsurerName);
                        break;
                    case "ProposalSummary":
                        emailBody = email.Replace("#{CustomerName}", sendNotificationRequest.CustomerName).Replace("#{Link}", sendNotificationRequest.Link);
                        emailSubject = subjectFraming;
                        break;
                    case "Payment":
                        emailBody = email.Replace("#{CustomerName}", sendNotificationRequest.CustomerName);
                        emailSubject = subjectFraming.Replace("#{CustomerName}", sendNotificationRequest.CustomerName).Replace("#{Insurer}", sendNotificationRequest.InsurerName);
                        break;
                    case "RenewalReminder":
                        emailBody = email.Replace("#{CustomerName}", sendNotificationRequest.CustomerName).Replace("#{Make}", sendNotificationRequest.MakeName)
                            .Replace("#{Model}", sendNotificationRequest.ModelName).Replace("#{Variant}", sendNotificationRequest.VariantName)
                            .Replace("#{ManufacturingYear}", mfgYear[0])
                            .Replace("#{PolicyNumber}", sendNotificationRequest.PolicyNumber);
                        emailSubject = subjectFraming.Replace("#{Make}", sendNotificationRequest.MakeName).Replace("#{Model}", sendNotificationRequest.ModelName)
                            .Replace("#{Insurer}", sendNotificationRequest.InsurerName);
                        break;
                    default:
                        break;
                }

                string bodyMessage = emailBody;
                string subject = emailSubject;
                mailStatus = await SendICEmail(sendNotificationRequest.EmailId, subject, bodyMessage, sendNotificationRequest.PolicyPDF, sendNotificationRequest.Type, cancellationToken);
                return mailStatus;
            }
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError("Email Send Error {Message}", ex.Message);
            return mailStatus;
        }
    }

    private async Task<bool> SendICEmail(string _Email, string _Subject, string _Body, byte[] PolicyArray, string type, CancellationToken cancellationToken)
    {
        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            MailMessage mail = new MailMessage();
            mail.To.Add(_Email);
            mail.From = new MailAddress(_emailConfig.FromEmail);
            mail.Subject = _Subject;
            mail.Body = _Body;
            mail.IsBodyHtml = true;
            if (type.Equals("Payment", StringComparison.OrdinalIgnoreCase))
            {
                mail.Attachments.Add(new Attachment(new MemoryStream(PolicyArray), "Policy.pdf"));
            }

            SmtpClient smtp = new()
            {
                Host = _emailConfig.Host,
                Port = _emailConfig.Port,
                Credentials = new NetworkCredential(_emailConfig.Username, _emailConfig.Password)
            };
            await smtp.SendMailAsync(mail, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Email sent failed: {_Email} - {_Subject} - {error Message}  ", _Email, _Subject, ex.Message.ToString());
            return false;
        }
        return true;
    }

    public async Task<string> SendPasswordResetEmail(string userId, string emailId, string password, CancellationToken cancellationToken)
    {
        Guid guId = Guid.NewGuid();
        try
        {
            string prefixUrl = _config.GetSection("WebUrl").GetSection("prefixUrl").Value;
            string LinkURL = prefixUrl + $"admin";

            string bodyMessage = $"Dear User,<br /><br />Please use this link <a target=_blank href={LinkURL}>Click Here</a> to Login to the portal and Please reset your Password.<br /> EmailId = {emailId} <br /> Password = {password} <br /><br />Regards,<br /><b>Hero Insurance Broking India Private Limited.</b>";
            string subject = "Reset Password";
            await SendEmail(emailId, subject, bodyMessage, cancellationToken);
        }
        catch (Exception)
        {
            return null;
        }
        return guId.ToString();
    }

    public async Task<bool> SendManualPolicyReport(ManualPolicyEmailRequest manualPolicyEmailRequest, List<Attachment> attachmentPaths, CancellationToken cancellationToken)
    {
        try
        {
            string bodyMessage = $"Dear {manualPolicyEmailRequest.UserName},<br /><br />Your request for Bulk Policy Upload has been processed successfully on {manualPolicyEmailRequest.dateAndTime}.<br /><br />Please find attached the Status Report." +
                $"<br /><br />Summery:<br />" +
                $"<table style='border-collapse:collapse;'>" +
                $"<tbody>" +
                $"<tr><td style='border: 1px solid;min-width: 200px;'>Policies Uploaded Successfully</td><td style='border: 1px solid;min-width: 150px;'>{manualPolicyEmailRequest.policyuploadedsuccessful}</td></tr>" +
                $"<tr><td style='border: 1px solid;'>Policies Failed</td><td style='border: 1px solid;'>{manualPolicyEmailRequest.policyfailed}</td></tr>" +
                $"<tr><td style='border: 1px solid;'>Total Policies Uploaded</td><td style='border: 1px solid;'>{manualPolicyEmailRequest.totalpolicy}</td></tr>" +
                $"</tbody>" +
                $"</table>" +
                               "<br/>Thanks,<br />Hero Insurance Broking India Private Limited.";

            string subject = "Manual Policy Upload Status";
            await SendEmail(manualPolicyEmailRequest.EmailId, subject, bodyMessage, attachmentPaths, cancellationToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SendPanVerificationFailedEmail(ResetUserDetailsForPanVerificationEmailModel resetUserDetailsForPanVerification, CancellationToken cancellationToken)
    {
        try
        {
            string bodyMessage = "Dear Partner,<br/>Your PAN Number has been found registered with another insurance entity.<br/> Kindly get your PAN deregistered and then re - apply to become POSP with Hero Insurance Broking India Pvt.Ltd.<br/><br/>For re-applying please login using your registered mobile number to complete your onboarding journey.<br/>Portal Link : <a href='" + _config.GetSection("WebUrl").GetSection("prefixUrl").Value + "' >Hero Insurance</a><br/>Registered Mobile Number: " + resetUserDetailsForPanVerification.MobileNo + "<br/><br/>It is an important step as per the guidelines of IRDA for a smooth process further.This will make sure that there will be no hindrance to your business in the future.<br/><br/>Regards,<br/>Team Hero Insurance Broking";
            string subject = "Preliminary PAN Verification failed on IRDAI's IIB Portal";
            await SendEmail(resetUserDetailsForPanVerification.EmailId, subject, bodyMessage, cancellationToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> SendEmail(string _Email, string _Subject, string _Body, List<Attachment> attachmentPaths, CancellationToken cancellationToken)
    {
        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            MailMessage mail = new MailMessage();
            mail.To.Add(_Email);
            mail.From = new MailAddress(_emailConfig.FromEmail);
            mail.Subject = _Subject;
            mail.Body = _Body;
            mail.IsBodyHtml = true;
            foreach (var attachment in attachmentPaths)
            {
                mail.Attachments.Add(attachment);
            }
            System.Net.Mail.SmtpClient smtp = new()
            {
                Host = _emailConfig.Host,
                Port = _emailConfig.Port,
                Credentials = new NetworkCredential(_emailConfig.Username, _emailConfig.Password)
            };
            await smtp.SendMailAsync(mail, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Email sent failed: {_Email} - {_Subject} - {error Message}  ", _Email, _Subject, ex.Message.ToString());
            return false;
        }
        return true;
    }

}