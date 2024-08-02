using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.AllReportAndMIS
{
    public class AllReportAndMISResponseModel
    {
        public IEnumerable<AllReportAndMISModel> AllReportAndMISModels { get; set; }
        public int TotalRecord { get; set; }
    }
    public class AllReportAndMISModel
    {
        public string Id { get; set; }
        public string EnquiryId { get; set; }
        public string LeadName { get; set; }
        public string PhoneNumber { get; set; }
        public string GeneratedOn { get; set; }
        public string InsuranceCompany { get; set; }
        public string Product { get; set; }
        public string Stage { get; set; }
        public string Premium { get; set; }
        public int TotalRecord { get; set; }
    }
}
