using Insurance.Core.Features.GoDigit.Command.CKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Persistence.ICIntegration.Abstraction;
public interface IGodigitService
{
    Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken);
    Task<Tuple<QuoteResponseModel, string, string>> CreateProposal(GoDigitProposalDto proposalQuery, ProposalRequest proposalRequest, GodigitCKYCRequest godigitCKYCRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken);
    Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Tuple<string, string, SaveCKYCResponse, CreateLeadModel> GetCKYCResponse(GoDigitCKYCCommand goDigitCKYCCommand, CancellationToken cancellationToken);
    Task<GoDigitPaymentURLResponseDto> GetPaymentLink(string leadId, string applicationId, string cancelReturnUrl, string successReturnUrl, CancellationToken cancellationToken);
    Task<PaymentCKCYResponseModel> GetPaymentDetails(GodigitPaymentCKYCReqModel godigitPaymentCKYCReqModel, CancellationToken cancellationToken);
    Task<PaymentCKCYResponseModel> GetCKYCDetails(GodigitPaymentCKYCReqModel godigitPaymentCKYCReqModel, CancellationToken cancellationToken);
    Task<GodigitPolicyDocumentResponseDto> GetPolicyDocumentPDF(string leadId, string applicationId, CancellationToken cancellationToken);
    Task<byte[]> GetDocumentPDFBase64(string documentLink, CancellationToken cancellationToken);
    Task<QuoteResponseModel> GetPolicyStatus(string leadId, string policyNumber, CancellationToken cancellationToken);
}
