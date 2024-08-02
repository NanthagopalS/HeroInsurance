using Insurance.Domain.InsuranceMaster;

namespace Insurance.Domain.Quote
{
    public class QuoteTransactionDbModel
    {
        public QuoteTransactionRequest QuoteTransactionRequest { get; set; }
        public QuoteConfirmDetailsModel QuoteConfirmDetailsModel { get; set; }
        public CreateLeadModel LeadDetail { get; set; }
        public string ProposalRequestBody { get; set; }
        public string CKYCRequestBody { get; set; }
    }

    public class QuoteTransactionRequest
    {
        public string QuoteTransactionId { get; set; }
        public string TransactionId { get; set; }
        public string InsurerId { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string CommonResponse { get; set; }
        public string LeadId { get; set; }
        public string CKYCStatus { get; set; }
        public string VehicleTypeId { get; set; }
        public string UserId { get; set; }
        public string ProposalId { get; set; }
        public string PolicyTypeId { get; set; }
        public string PolicyId { get; set; }
        public string PreviousSATPInsurerName { get; set; }
        public string FinancierName { get; set; }
        public string FinancierBranch { get; set; }
        public string FinancierAddress { get; set; }
    }
    public class QuoteConfirmDetailsModel
    {
        public string Chassis { get; set; }
        public string Engine { get; set; }
        public string CurrentPolicyType { get; set; }
        public string PreviousSAODInsurerName { get; set; }
        public string PreviousSATPInsurerName { get; set; }
        public string NCBValue { get; set; }
        public string SATPInsurerCode { get; set; }
        public string SAODInsurerCode { get; set; }
        public string NCBName { get; set; }
    }
}
