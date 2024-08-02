using Insurance.Core.Features.Quote.Command.SaveUpdateLead;
using Insurance.Core.Features.Quote.Query.GetPreviousCoverMaster;
using Insurance.Core.Features.Quote.Query.GetProposalDetails;
using Insurance.Core.Models;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Core.Contracts.Persistence
{
    public interface IQuoteRepository
    {
        Task<bool> AddLeadAddress(CreateLeadModel leadDetail, CancellationToken cancellationToken);
        Task<bool> AddLeadNominee(CreateLeadModel leadDetail, CancellationToken cancellationToken);
        Task<IEnumerable<ProposalFieldMasterModel>> GetProposalSummary(string insurerId, string variantId, string vehicleNumber, string quoteTransactionId, CancellationToken cancellationToken);
        Task<IEnumerable<ProposalFieldMasterModel>> GetProposalFields(string InsurerID, string quotetransactionId, CancellationToken cancellationToken);
        Task<SaveUpdateLeadVm> ProposalRequest(ProposalRequestModel proposalRequestModel, CancellationToken cancellationToken);
        Task<QuoteTransactionDbModel> GetQuoteTransactionDetails(string TransactionID, CancellationToken cancellationToken);
        Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> GetQuoteConfirmDetails(string transactionID, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
        Task<IEnumerable<ProposalFieldMasterModel>> GetCKYCFields(string InsurerID, bool isPOI, bool isCompany, bool isDocumentUpload, CancellationToken cancellationToken);
        Task<IEnumerable<ProposalFieldMasterModel>> GetCKYCDocumenFields(string insurerId, string documentName, CancellationToken cancellationToken);
        Task<PaymentCKCYResponseModel> InsertPaymentTransaction(QuoteResponseModel proposalResponse, CancellationToken cancellationToken);
        Task<PaymentCKCYResponseModel> GetPaymentStatus(PaymentStatusRequestModel paymentStatusRequest, CancellationToken cancellationToken);
        Task<string> UploadPolicyDocumentMangoDB(byte[] policyDocumentBase64, CancellationToken cancellationToken);
        Task<string> DownloadPolicyDocumentMangoDB(string documentId, CancellationToken cancellationToken);
        Task<string> GetPolicyDocumentData(PaymentStatusRequestModel policyDocumentRequestModel, CancellationToken cancellationToken);
        Task<QuotConfirmDataResponseModel> QuoteConfirmTransaction(QuoteConfirmDataModel quoteConfirmDataModel,CancellationToken cancellationToken);
        Task<QuoteTransactionDbModel> GetQuoteConfirmDetailsDB(string transactionID, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
        Task SaveQuoteTransaction(SaveQuoteTransactionModel saveQuoteTransactionModel, CancellationToken cancellationToken);
        Task<GetLeadDetailsModel> GetLeadDetails(string leadId, string stageId, CancellationToken cancellationToken);
        Task<SaveCKYCResponse> SaveLeadDetails(string insurerId, string quoteTransactionId, string kycRequest, string kycResponse, string stage, CreateLeadModel createLeadModel,CancellationToken cancellationToken);
        Task UpdateLeadDetails(CreateLeadModel createLeadModel, CancellationToken cancellationToken);
        Task<CKYCStatusModel> GetCKYCStstus(string insurerID, string quoteTransactionId, CancellationToken cancellationToken);
        Task<ICICIPOSPDetails> GetPOPSDetais(string userId);
        Task<BreakInPaymentDetailsDBModel> GetBreakInPaymentDetails(PaymentStatusRequestModel policyRequest, CancellationToken cancellationToken);
        Task<string> UpdateLeadPaymentLink(string insurerId, string quoteTransactionId, string paymentLink, string paymentCorrelationId, CancellationToken cancellationToken);
        Task<GetProposalDetailsForPaymentResponceModel> GetProposalDetailsForPayment(string insurerId, string quoteTransactionId, CancellationToken cancellationToken);
        Task<string> GetUserIddDetails(string insurerId, string proposalNumber, CancellationToken cancellationToken);
		Task<GetPreviousCoverResponseModel> GetPreviousCoverMaster(string insurerID, string vehicalTypeId, string policyTypeId, CancellationToken cancellationToken);
	    Task<CreateLeadModel> InsertBreakInDetails(InsertBreakInDetailsModel breakInDetailsModel, CancellationToken cancellationToken);
        Task<CreateLeadModel> GetLeadDetailsByApplicationIdOrQuoteTransactionId(GetLeadDetailsByApplicationOrQuoteTransactionIdModel request, CancellationToken cancellationToken);
        Task InsertQuoteRequest(string request, string leadId, string stage, string quoteTransactionId, CancellationToken cancellationToken);
        Task<int> InsertICLogs(string insurerId, string requestBody, string responseBody, string leadId, string api, string token, string header, string stage);
        Task<List<string>> VehicleNumberSplit(string vehicleNumber);
        Task<CKYCStatusModel> InsertKYCDetailsAfterProposal(KYCDetailsModel kycDetailsModel, CancellationToken cancellationToken);
    }
}
