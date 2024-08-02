namespace Insurance.Domain.Quote;

public class UploadCKYCDocumentResponse
{
    public string InsurerId { get; set; }
    public string LeadID { get; set; }
    public string TransactionId { get; set; }
    public string KYCId { get; set; }
    public string CKYCNumber { get; set; }
    public string CKYCStatus { get; set; }
    public string Message { get; set; }
    public string InsurerName { get; set; }
    public string Name { get; set; }
    public string DOB { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
}

public class PolicyDocumentResponseModel
{
    public RespPolicyDocument Resp_Policy_Document { get; set; }
}

public class RespPolicyDocument
{
    public string PDF_BYTES { get; set; }
}