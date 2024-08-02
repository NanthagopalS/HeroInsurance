namespace Insurance.Web.Models
{
    public class UnitedIndiaPaymentRequestModel
    {
        public string HOST { get; set; }
        public string MID { get; set; }
        public string orderId { get; set; }
        public string token { get; set; }
        public string amount { get; set; }
    }
}