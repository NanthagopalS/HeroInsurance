using Insurance.Core.Features.ICICI.Command.CKYC;
using Insurance.Core.Features.ICICI.Command.UploadICICICKYCDocument;
using Insurance.Core.Features.ICICI.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Core.Contracts.Persistence
{
    public interface IICICIRepository
    {
        Task<QuoteResponseModel> GetQuote(GetIciciQuoteQuery query, CancellationToken cancellationToken);
        Task<SaveQuoteTransactionModel> CreateProposal(ProposalRequestModel proposalRequestModel, CancellationToken cancellationToken);
        Task<ICICIPaymentEnquiryResponse> GetPaymentEnquiry(string transactionId, string leadId, CancellationToken cancellationToken);
        Task<Tuple<ICICIPaymentTaggingResponseDto, ICICIPOSPDetails>> GetPaymentTagging(ICICIPaymentEnquiryResponse paymentEnquiryResponse, string transactionId, string correlationId);
        Task<byte[]> GetPloicy(ICICIPaymentTaggingResponseDto iCICIPaymentTaggingResponseDto, ICICIPOSPDetails iCICIPOSPDetails);
        Task<string> CreateIMBroker(ICICICreateIMBrokerModel iCICICreateIMBrokerModel, CancellationToken cancellationToken);
        Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
        Task<Tuple<string, string, string>> CreateBreakinId(string leadId, string proposalRequest, SaveQuoteTransactionModel proposalResponse,string vehicleTypeId, CancellationToken cancellationToken);
        Task<ICICIGetBreakinStatusResponseModel> BreakinInspectionStatus(CancellationToken cancellationToken);
        Task<string> ClearBreakinInspectionStatus(string breakinId, bool isBreakInApproved, string correlationId, CancellationToken cancellationToken);
        Task<Tuple<string, string>> CreatePaymentLink(string leadId, string proposalRequest, string proposalResponse, CancellationToken cancellationToken);
        Task<QuoteTransactionRequest> GetProposalDetails(string transactionID);
        Task UpdateBreakinStatus(CreateLeadModel createLeadModel, CancellationToken cancellationToken);
        Task<QuoteTransactionRequest> GetBreakInPaymentDetails(string transactionId, string breakinId, CancellationToken cancellationToken);
        Task<SaveCKYCResponse> SaveCKYC(ICICICKYCCommand iCICICKYCCommand, CancellationToken cancellationToken);
        Task<UploadCKYCDocumentResponse> UploadCKYCDocument(UploadICICICKYCDocumentCommand iCICICKYCCommand, CancellationToken cancellationToken);
        Task<Tuple<string, string>> GetPaymentLink(BreakInPaymentDetailsDBModel breakInPaymentDetailsDBModel, CancellationToken cancellationToken);
        Task<VariantAndRTOIdCheckModel> DoesICICIVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, CancellationToken cancellationToken);

        #region CV
        Task<QuoteResponseModel> GetCVQuote(GetCommercialICICIQuoteQuery query, CancellationToken cancellationToken);
        Task<SaveQuoteTransactionModel> CreateCvProposal(ProposalRequestModel proposalRequestModel, CancellationToken cancellationToken);
        #endregion
    }
}
