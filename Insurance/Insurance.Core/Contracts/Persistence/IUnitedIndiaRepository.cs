using Insurance.Core.Features.UnitedIndia.Command;
using Insurance.Core.Features.UnitedIndia.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.UnitedIndia.Payment;
using Insurance.Domain.UnitedIndia;
using Insurance.Core.Features.UnitedIndia.Queries;
using Insurance.Domain.TATA;
using Insurance.Core.Features.UnitedIndia.Queries.GetCKYCPOAStatus;

namespace Insurance.Core.Contracts.Persistence;

public interface IUnitedIndiaRepository
{
    Task<QuoteResponseModel> GetQuote(GetUnitedIndiaQuoteQuery query, CancellationToken cancellationToken);
    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> SaveCKYC(CKYCModel cKYCModel, UnitedProposalDynamicDetail unitedProposalDynamicDetail, CancellationToken cancellationToken);
    Task<VariantAndRTOIdCheckModel> DoesUnitedIndiaVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, CancellationToken cancellationToken);
    Task<PaymentstausResponse> GetPaymentStatus(InitiatePaymentRequestDto requestDto, CancellationToken cancellationToken);
    Task<string> InitiatePayment(InitiatePaymentRequestDto initiatePaymentRequestDto, CancellationToken cancellationToken);
    Task<TATARequestModelForPaymentVerify> GetLeadIdAndPaymentId(string QuoteTransactionId, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> CreateProposal(string requestBody, string transactionId, string leadId, CancellationToken cancellationToken);
    Task<UnitedIndiaQuoteResponseDuringProposal> GetQuoteUpdate(QuoteTransactionRequest request, CreateLeadModel createLeadModel, UnitedProposalDynamicDetail unitedProposalDynamicDetail, CancellationToken cancellationToken);
    Task<UnitedIndiaPaymentInfoResponseEnvelope> GetPaymentInfo(UIICPaymentInfoHEADER requestDto,string LeadId, CancellationToken cancellationToken);
    Task<IEnumerable<NameValueModel>> GetFinancierBranchDetails(string financierCode, CancellationToken cancellationToken);
    Task<CreateLeadModel> UpdateCKYCPOAStatus(GetUnitedIndiaCKYCPOAQuery getUnitedIndiaCKYCPOAQuery, CancellationToken cancellationToken);
}
