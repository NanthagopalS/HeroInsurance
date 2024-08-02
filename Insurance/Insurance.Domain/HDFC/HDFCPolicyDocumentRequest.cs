namespace Insurance.Domain.HDFC;

public class HDFCPolicyDocumentRequest
{
    public string TransactionID { get; set; }
    public ReqPolicyDocument Req_Policy_Document { get; set; }
}
public class ReqPolicyDocument
{
    public string Policy_Number { get; set; }
}
