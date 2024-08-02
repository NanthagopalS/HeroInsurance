namespace ThirdPartyUtilities.Models.Email;
public class EmailRsult
{
    public string id { get; set; }
    public string emailId { get; set; }
    public string details { get; set; }
    public string status { get; set; }
    public string LinkURL { get; set; }
}

public class EmailResponse
{
    public EmailRsult response { get; set; }
}
