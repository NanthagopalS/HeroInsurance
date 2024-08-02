namespace Insurance.Domain.ReportAndMIS
{
    public class GetAllReportResponseModel
    {
        public IEnumerable<GetAllReportModel> GetAllReportModel { get; set; }
        public int TotalRecords { get; set; }
    }
    public record GetAllReportModel
    {
        public string EnquiryId { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string GeneratedOn { get; set; }
        public string InsuranceCompany { get; set; }
        public string Product { get; set; }
        public string Stage { get; set; }
        public string Premium { get; set; }
        public int TotalRecord { get; set; }

    }
}
