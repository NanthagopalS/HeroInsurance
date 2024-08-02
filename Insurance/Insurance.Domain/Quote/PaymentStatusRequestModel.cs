namespace Insurance.Domain.Quote
{
    public  class PaymentStatusRequestModel
    {
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
        public string ApplicationId { get; set; }
    }
}
