using Insurance.Domain.ICICI.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.ICICI
{
    public class ICICICVProposalResponseDto
    {
        public cvRiskdetails riskDetails { get; set; }
        public Generalinformation generalInformation { get; set; }
        public Premiumdetails premiumDetails { get; set; }
        public string deviationMessage { get; set; }
        public bool isQuoteDeviation { get; set; }
        public bool breakingFlag { get; set; }
        public bool isApprovalRequired { get; set; }
        public string proposalStatus { get; set; }
        //public object garageCashDetails { get; set; }
        //public int earlyPayRate { get; set; }
        public float systemDiscount { get; set; }
        public Taxdetails taxDetails { get; set; }
        //public object riskCoverRates { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public string statusMessage { get; set; }
        public string correlationId { get; set; }
    }

}
