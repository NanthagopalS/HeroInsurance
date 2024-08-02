using Insurance.Core.Features.GoDigit.Command.CKYC;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.GoDigit.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Core.Contracts.Persistence;
public interface IGoDigitRepository
{
    Task<QuoteResponseModel> GetQuote(GetGoDigitQuery query,CancellationToken cancellationToken);
    Task QuoteTransaction(QuoteResponseModel quoteResponseModel, string requestBody, string responseModel, string stage, string insurerId, string leadId, decimal MaxIDV, decimal MinIDV, decimal RecommendedIDV, string transactionId);
    Task<InputValidation> ValidationCheck(GetGoDigitQuery query, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> CreateProposal(ProposalRequestModel proposalRequestModel, CancellationToken cancellationToken);
    Task<GoDigitPaymentURLResponseDto> CreatePaymentLink(string leadId, string applicationId, string cancelReturnUrl, string successReturnUrl, CancellationToken cancellationToken);
    Task<PaymentCKCYResponseModel> GetPaymentDetails(GodigitPaymentCKYCReqModel godigitPaymentCKYCReqModel, CancellationToken cancellationToken);
    Task<PaymentCKCYResponseModel> GetCKYCDetails(GodigitPaymentCKYCReqModel godigitPaymentCKYCReqModel, CancellationToken cancellationToken);
    Task<GodigitPolicyDocumentResponseDto> GetPolicyDocumentPDF(string leadId, string applicationId, CancellationToken cancellationToken);
    Task<byte[]> GetDocumentPDFBase64(string documentLink, CancellationToken cancellationToken);
    Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<QuoteResponseModel> GetPolicyStatus(string leadId, string quoteTransactionId, CancellationToken cancellationToken);
    Task<SaveCKYCResponse> SaveCKYC(GoDigitCKYCCommand goDigitCKYCCommand, CancellationToken cancellationToken);
    Task<GoDigitPaymentURLResponseDto> GetPaymentLink(string leadId, string applicationId, CancellationToken cancellationToken);
    Task<VariantAndRTOIdCheckModel> DoesGoDigitVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, CancellationToken cancellationToken);
}
