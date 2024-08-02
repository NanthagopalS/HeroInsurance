namespace ThirdPartyUtilities.Models.Signzy;
public class IdentitiesRequestModel
{
    public string type { get; set; }
    public string email { get; set; }
    public string callbackUrl { get; set; }
    public List<byte[]> images { get; set; }
}

public class IdentitiesResponseModel
{
    public string type { get; set; }
    public string email { get; set; }
    public string callbackUrl { get; set; }
    public List<object> images { get; set; }
    public string accessToken { get; set; }
    public List<object> autoRecognition { get; set; }
    public List<object> verification { get; set; }
    public List<object> forgeryCheck { get; set; }
    public string id { get; set; }
    public string patronId { get; set; }
}