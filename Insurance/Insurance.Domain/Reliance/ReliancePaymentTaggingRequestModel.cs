namespace Insurance.Domain.Reliance
{
    public class ReliancePaymentTaggingRequestModel
    {
        public string PaymentId { get; set; }
        public string Amount  { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionReferenceNumber { get; set; }
    }
}
