namespace Insurance.Domain.Chola
{

    public class CholaIDVResponseModel
    {
        public int StatusCode { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string RequestId { get; set; }
        public string RequestDateTime { get; set; }
        public IDVValues Data { get; set; }
    }

    public class IDVValues
    {
        public int idv_1 { get; set; }
        public int idv_2 { get; set; }
        public int idv_3 { get; set; }
        public int idv_4 { get; set; }
    }

}
