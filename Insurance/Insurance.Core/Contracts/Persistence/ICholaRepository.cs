using Insurance.Core.Features.Chola.Command.CKYC;
using Insurance.Core.Features.Chola.Queries.GetBreakinStatus;
using Insurance.Core.Features.Chola.Queries.GetCKYCStatus;
using Insurance.Core.Features.Chola.Queries.GetPOAStatus;
using Insurance.Core.Features.Chola.Queries.GetQuote;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Core.Contracts.Persistence;

public interface ICholaRepository
{
    Task<QuoteResponseModel> GetQuote(GetCholaQuery query, CancellationToken cancellationToken);
    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<CholaPaymentWrapperModel> GetPaymentDetails(CholaPaymentTaggingRequestModel model, CancellationToken cancellationToken);
    Task<SaveCKYCResponse> GetCKYCDetails(CholaCKYCCommand cholaCKYCCommand, CancellationToken cancellationToken);
    Task<CholaCKYCStatusReponseModel> GetCKYCStatus(GetCholaCKYCStatusQuery cholaCKYCStatusQuery, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> CreateBreakIn(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> CreateProposal(CholaServiceRequestModel proposalQuery, CholaProposalRequest proposalRequest, CreateLeadModel createLeadModel, CholaCKYCRequestModel ckycRequestModel, CancellationToken cancellationToken);
    Task<CholaBreakInResponseModel> GetBreakInStatus(GetBreakinStatusQuery getBreakinStatusQuery,CancellationToken cancellationToken);
    Task UpdatedIsBreakinApproved(string breakingId, bool isBreakinApproved, string stage, string inspectionDate, string inspectionAgency,CancellationToken cancellationToken);

    string PaymentURLGeneration(string paymentId, string amount, string quoteTransactionId);
    Task<GetCholaCKYCStatusQuery> GetKycPOA(GetPOAStatusQuery getPOAStatusQuery,CancellationToken cancellationToken);
    Task<CreateLeadModel> GetBreakinDetails(string quoteTransactionId, CancellationToken cancellationToken);
    Task<string> SendBreakinNotification(string breakinId, string mobileNumber, string inspectionURL, CancellationToken cancellationToken);
    Task<VariantAndRTOIdCheckModel> DoesCholaVariantAndRTOExists(string variantId, string rtoId, string policyTypeId, string vehicleNumber, CancellationToken cancellationToken);
}
