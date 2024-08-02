namespace Insurance.Web.Models
{
    public class ReliancePaymentResponseModel
    {
        public string StatusID { get; set; }
        public string PoliCyNumber { get; set; }
        public string TransactionNumber { get; set; }
        public string OptionalValue { get; set; }
        public string GatewayName { get; set; }
        public string ProposalNumber { get; set; }
        public string TransactionStatus { get; set; }
        public string ProductCode { get; set; }
        
    }

    public class ReliancePaymentRequestModel
    {
        public string Trnsno { get; set; }
        public string Amt { get; set; }
        public string PanNumber { get; set; }
        public string Subid { get; set; }
        public string PaymentId { get; set; }
        public string Url { get; set; }
        public string RedirectionUrl { get; set; }
        public string UserId { get; set; }
        public string CKYCNumber { get; set; }
        public string StatusURL { get; set; }
    }
    
}
