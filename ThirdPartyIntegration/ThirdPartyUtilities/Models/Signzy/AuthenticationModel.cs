namespace ThirdPartyUtilities.Models.Signzy;
public class AuthenticationRequest
{
    public string username { get; set; }
    public string password { get; set; }
}

public class AuthenticationResponse
{
    public string id { get; set; }
    public long ttl { get; set; }
    public DateTime created { get; set; }
    public string userId { get; set; }
}