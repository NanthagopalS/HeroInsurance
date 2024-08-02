namespace ThirdPartyUtilities.Models.SMS;
public class SMSRsults
{
    public string id { get; set; }
    public string phone { get; set; }
    public string details { get; set; }
    public string status { get; set; }
    public int OTP { get; set; }
}

public class SMSResponse
{
    public SMSRsults response { get; set; }
}
