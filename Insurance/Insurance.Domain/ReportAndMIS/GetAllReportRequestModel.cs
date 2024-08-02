using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.ReportAndMIS
{
    public class GetAllReportRequestModel
    {
        public string EnquirId { get; set; }
        public string ProductType { get; set; }
        public string Insurertype { get; set; }
        public string Stage { get; set; }
        public string Startdate { get; set; }
        public string EndDate { get; set; }
        public int CurrentPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public string? CreatedBy { get; set; }
    }
}
