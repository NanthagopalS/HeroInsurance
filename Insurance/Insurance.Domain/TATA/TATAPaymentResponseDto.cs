namespace Insurance.Domain.TATA
{
    public class TATAPaymentResponseDataDto
    {
        public int status { get; set; }
        public string message_txt { get; set; }
        public PaymentData data { get; set; }
    }

    public class PaymentData
    {
        public string orderID { get; set; }
        public string uniqueIdentifier { get; set; }
        public string paymentLink_web { get; set; }
    }

}
