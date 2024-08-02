namespace Insurance.Domain.ICICI;

public class HDFCCreateIMBrokerResponseArray
{
    public ICICICreateIMBrokerResponse[] iCICICreateIMBrokerResponses { get; set; }
}
public class HDFCCreateIMBrokerResponse
{
    public string imid { get; set; }
    public string intermediaryStatus { get; set; }
    public string status { get; set; }
    public string statusDesc { get; set; }
    public string correlationID { get; set; }
    public string POSPID { get; set; }
}


