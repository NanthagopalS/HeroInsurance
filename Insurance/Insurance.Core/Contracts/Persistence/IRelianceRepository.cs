using Insurance.Core.Features.Reliance.Command.CKYC;
using Insurance.Core.Features.Reliance.Command.GetQuote;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.Reliance;

namespace Insurance.Core.Contracts.Persistence;

public interface IRelianceRepository
{
    Task<QuoteResponseModel> GetQuote(GetRelianceQuery query, CancellationToken cancellationToken);
    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<SaveCKYCResponse> GetCKYCDetails(RelianceCKYCCommand relianceCKYCCommand, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> CreateProposal(RelianceRequestDto proposalQuery, RelianceProposalRequest proposalRequest, CreateLeadModel createLeadModel, RelianceCKYCRequestModel ckycRequestModel, CancellationToken cancellationToken);
    Task<RelianceUploadDocumentResponseModel> GetCKYCPOAResponse(string quoteTransactionId, string kycId, CancellationToken cancellationToken);

    string PaymentURLGeneration(string paymentId, string amount, string quoteTransactionId, string panNumber, string proposalNumber, string ckycNumber, string productCode);

    Task<VariantAndRTOIdCheckModel> DoesRelianceVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber,string vehicleTypeId, CancellationToken cancellationToken);
    Task<ReliancePaymentWrapperModel> GetPaymentDetails(string leadId,string policyNumber, string productCode, CancellationToken cancellationToken);
}
