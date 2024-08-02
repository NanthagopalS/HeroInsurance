namespace Insurance.Domain.IFFCO;

public class IFFCOPolicyDocumentResponse
{
    public string uniqueReferenceNo { get; set; }
    public string policyDownloadLink { get; set; }
    public string statusMessage { get; set; }
    public Error[] error { get; set; }
}

public class Error
{
    public string errorField { get; set; }
    public string errorMessage { get; set; }
}


