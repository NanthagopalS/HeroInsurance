namespace Insurance.Web.Models
{
    public class IFFCOSubmitPaymentModel
    {
        public string ProposalRequest { get; set; }
        public string UniqId { get; set; }
        public string ReturnURL { get; set;}
        public string PaymentURL { get; set; }
        public string PartnerCode { get; set; }
    }
}
