namespace Insurance.Domain.Quote;

public class SaveCKYCResponse
{
    public int InsurerStatusCode { get; set; }
    public string LeadID { get; set; }
    public string KYCId { get; set; }
    public string CKYCNumber { get; set; }
    public string KYC_Status { get; set; }
    public string Message { get; set; }
    public string redirect_link { get; set; }
    public string PhotoId { get; set; }
    public bool IsKYCRequired { get; set; }
    public string Name { get; set; }
    public string DOB { get; set; }
    public string Age { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public string InsurerName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string ClientId { get; set; }
    public string AadharNumber { get; set; }
    public string ProposalId { get; set; }
    public bool IsDocumentUpload { get; set; }
    //public List<ProposalFieldMasterModel> proposalFieldMasterModels { get; set; }
}
