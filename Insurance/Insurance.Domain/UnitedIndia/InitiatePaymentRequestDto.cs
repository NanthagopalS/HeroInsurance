namespace Insurance.Domain.UnitedIndia
{
    public class InitiatePaymentRequestDto
    {
        public string LeadId { get; set; }
        public string QuoteTransactionId { get; set; }
        public string transaction_id { get; set; }
        public string orderId { get; set; }
        public string txnAmount { get; set; }
        public Userinfo userInfo { get; set; }
        public string num_reference_number { get; set; }
    }
}
