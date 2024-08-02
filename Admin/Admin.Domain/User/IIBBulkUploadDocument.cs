using Org.BouncyCastle.Crypto.Agreement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class IIBBulkUploadDocument
    {
        public string? PAN { get; set; }
        public string? UserId { get; set; }
        public string? POSPName { get; set; }
        public string? DOB { get; set; }
        public string? City { get; set; }
        public string? PIN { get; set; }
        public string? AppointmentDate { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? IIBStatus { get; set; }
        public string? IIBUploadStatus { get; set; }
        public string? POSPId { get; set; }
    }
    
}
