namespace Insurance.Domain.Quote
{
    public class KYCDetailsModel
    {
        public string LeadId { get; set; }
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string PhotoId { get; set; }
        public string Stage { get; set; }
        public string KYCId { get; set; }
        public string CKYCNumber { get; set; }
        public string CKYCStatus { get; set; }
    }
}
