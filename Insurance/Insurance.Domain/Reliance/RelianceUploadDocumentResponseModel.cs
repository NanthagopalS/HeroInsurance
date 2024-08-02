using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Domain.Reliance
{
    public class RelianceUploadDocumentResponseModel
    {
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public UploadCKYCDocumentResponse uploadCKYCDocumentResponse { get; set; }
        public CreateLeadModel createLeadModel { get; set; }
    }
}
