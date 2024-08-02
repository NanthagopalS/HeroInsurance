using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Bajaj
{
    public class BajajCKYCDocumentUploadResponseDto
    {
        public string errorCode { get; set; }
        public string errMsg { get; set; }
        public Documentuploadstatus documentUploadStatus { get; set; }
    }

    public class Documentuploadstatus
    {
        public string documentNumber { get; set; }
        public string poaDocUploadDtatus { get; set; }
        public string poiDocUploadStatus { get; set; }
        public string bagicKycStatus { get; set; }
        public string bagicKycId { get; set; }
    }

}
