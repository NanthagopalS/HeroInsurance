using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Bajaj
{
    public class BajajCKYCDocumentUploadRequestDto
    {
        public string appType { get; set; }
        public string fieldType { get; set; }
        public string fieldValue { get; set; }
        public string kycDocumentType { get; set; }
        public string kycDocumentCategory { get; set; }
        public string documentNumber { get; set; }
        public string documentExtension { get; set; }
        public string documentArray { get; set; }
    }
}
