namespace ThirdPartyUtilities.Models.SMS;

public class SendICNotificationModel
{
    public IEnumerable<SMS> SMS { get; set; }
    public IEnumerable<Email> Email { get; set; }

}
public class SMS
{
    public string Type { get; set; }
    public string Body { get; set; }
}
public class Email
{
    public string Type { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}