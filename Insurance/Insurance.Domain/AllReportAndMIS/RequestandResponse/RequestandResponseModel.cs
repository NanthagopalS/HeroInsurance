namespace Insurance.Domain.AllReportAndMIS.BusinessSummerym
{
    public class RequestandResponseModel
    {
        public IEnumerable<RequestandResponseRecord> RequestandResponseRecord { get; set; }
        public int TotalRecords { get; set; }
        public string FileName { get; set; }

    }

    public class RequestandResponseRecord
    {
        public int TotalRecords { get; set; }
        public int Sno { get; set; }
        public string LeadId { get; set; }
        public string InsurerName { get; set; }
        public string StageID { get; set; }
        public string Product { get; set; }
        public string ApiURL { get; set; }
        public string RequestBody { get; set; }
        public string RequestTime { get; set; }
        public string ResponseBody { get; set; }
        public string ResponseTime { get; set; }
        public string Responnsestatuscode { get; set; }
        public string ResponseError { get; set; }
        public string StageName { get; set; }
    }
}
