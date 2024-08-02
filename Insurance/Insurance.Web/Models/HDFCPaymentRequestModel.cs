namespace Insurance.Web.Models
{
    public class HDFCPaymentRequestModel
    {
        public string Trnsno { get; set; }
        public string Amt { get; set; }
        public string Appid { get; set; }
        public string Subid { get; set; }
        public string StatusURL { get; set; }
        public string Chksum { get; set; }
        public string CheckSumURL { get; set; }
    }
}
