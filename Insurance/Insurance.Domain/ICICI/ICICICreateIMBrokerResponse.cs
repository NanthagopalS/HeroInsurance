namespace Insurance.Domain.ICICI;

public class ICICICreateIMBrokerResponseArray
{
    public ICICICreateIMBrokerResponse[] iCICICreateIMBrokerResponses { get; set; }
}
public class ICICICreateIMBrokerResponse
{
    public string imid { get; set; }
    public string intermediaryStatus { get; set; }
    public string status { get; set; }
    public string statusDesc { get; set; }
    public string correlationID { get; set; }
}


