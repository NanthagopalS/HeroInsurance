using Insurance.Core.Features.UnitedIndia.Command;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.UnitedIndia.Payment;
using Insurance.Domain.UnitedIndia;

namespace Insurance.Persistence.ICIntegration.Abstraction;

public interface IUnitedIndiaService
{
    Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken);
    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(CKYCModel cKYCModel, UnitedProposalDynamicDetail unitedProposalDynamicDetail, CancellationToken cancellationToken);
    Task<PaymentstausResponse> GetPaymentStatus(InitiatePaymentRequestDto requestDto, CancellationToken cancellationToken);
    Task<string> InitiatePayment(InitiatePaymentRequestDto InitiatePaymentRequest, CancellationToken cancellationToken);
    Task<UnitedIndiaQuoteResponseDuringProposal> UpdateUserDetailsInQuotation(QuoteTransactionRequest quoteTransactionRequest, CreateLeadModel createLeadModel, UnitedProposalDynamicDetail proposalDynamicDetails, CancellationToken cancellationToken);
    Task<ProposalResponseModel> CreateProposal(string requestBody, string transactionId, string leadId, CancellationToken cancellationToken);
    Task<UnitedIndiaPaymentInfoResponseEnvelope> GetPaymentInfo(UIICPaymentInfoHEADER requestDto, string LeadId, CancellationToken cancellationToken);
}
