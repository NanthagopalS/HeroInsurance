namespace ThirdPartyUtilities.Models.Signzy;
public class PANVerificationRequestModel
{
    public string service { get; set; }
    public string itemId { get; set; }
    public string task { get; set; }
    public string accessToken { get; set; }
    public PANVerificationEssentials essentials { get; set; }
}
public class PANVerificationEssentials
{
    public string number { get; set; }
}


public class Instance
{
    public string id { get; set; }
    public string callbackUrl { get; set; }
}

public class PANVerificationInstance
{
    public string number { get; set; }
    public int id { get; set; }
    public Instance instance { get; set; }
    public PanVerificationResult result { get; set; }
}

public class PanVerificationResult
{
    public string number { get; set; }
    public string name { get; set; }
    public string fatherName { get; set; }
    public string dob { get; set; }
}

public class PANVerificationResponse
{
    public string service { get; set; }
    public string itemId { get; set; }
    public string task { get; set; }
    public PANVerificationEssentials essentials { get; set; }
    public string accessToken { get; set; }
    public string id { get; set; }
    public PANVerificationInstance response { get; set; }
}