using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCUploadDocumentResponseModel
    {
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public UploadCKYCDocumentResponse uploadCKYCDocumentResponse { get; set; }
        public CreateLeadModel createLeadModel { get; set; }
    }
}
