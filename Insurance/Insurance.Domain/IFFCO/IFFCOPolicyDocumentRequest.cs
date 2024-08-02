namespace Insurance.Domain.IFFCO;
public class IFFCOPolicyDocumentRequest
{
    public string uniqueReferenceNo { get; set; }
    public string contractType { get; set; }
    public string policyDownloadNo { get; set; }
    public Partnerdetail partnerDetail { get; set; }
}

public class Partnerdetail
{
    public string partnerCode { get; set; }
}

