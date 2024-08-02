using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATACKYCRequestModel
    {
        public string IDType { get; set; }
        public string ProposalNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string LeadId { get; set; }
        public string IdNumber { get; set; }
        public string Name { get; set; }
        public string documentPDF { get; set; }
        public string ReqId { get; set; }
        public bool IsCompany { get; set; }
    }
}
