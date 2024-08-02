namespace Insurance.Core.Features.Chola.Queries.GetPaymentStatus
{
    public class CholaPaymentDetailsVm
    {
        public int InsurerStatusCode { get; set; }
        public string PaymentTransactionId { get; set; }
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
        public string ApplicationId { get; set; }
        public string LeadId { get; set; }
        public string LeadName { get; set; }
        public string ProposalNumber { get; set; }
        public string PaymentTransactionNumber { get; set; }
        public string Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string CKYCStatus { get; set; }
        public string CKYCLink { get; set; }
        public string CKYCFailedReason { get; set; }
        public string PolicyType { get; set; }
        public string Logo { get; set; }
        public string PolicyDocumentLink { get; set; }
        public string DocumentId { get; set; }
        public string PolicyNumber { get; set; }
        public string CustomerId { get; set; }
        public string PolicyStatus { get; set; }

    }
}
