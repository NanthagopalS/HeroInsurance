using Insurance.Core.Features.HDFC.Command.CKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.ICICI;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Persistence.ICIntegration.Abstraction;
public interface IHDFCService
{
    Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel query, CancellationToken cancellationToken);
    public Task<(string Token, string TransactionId,string ProductCode)> GetToken(string vehicleTypeId, string policyTypeId, string stage, string categoryId, string leadId, CancellationToken cancellationToken);
    public Task<IDVResponseModel> GetIDV(QuoteQueryModel query, CancellationToken cancellationToken);
    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(HDFCCKYCCommand hdfcCKYCCommand, CancellationToken cancellationToken);
    Task<HDFCUploadDocumentResponseModel> GetCKYCPOAResponse(string transactionId, string kycId,string LeadId, CancellationToken cancellationToken);
    Task<HDFCCkycPOAStatusResponseModel> GetCKYCPOAStatus(string kycId, string leadId, CancellationToken cancellationToken);
    Task<HDFCSubmitPOSPCertificateResponse> CreatePOSP(HDFCCreateIMBrokerRequestDto hDFCCreateIMBrokerRequest, CancellationToken cancellationToken);
    Task<ProposalResponseModel> CreateProposal(HDFCServiceRequestModel proposalQuery, HDFCProposalRequest proposalRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken);
    Task<string> GeneratePaymentCheckSum(string transactionId, string amount, string redirectionURL,string leadId, CancellationToken cancellationToken);
    Task<HDFCPolicyDocumentResponseModel> GetPolicyDocument(HDFCPolicyRequestModel paymentFieldModel, CancellationToken cancellationToken);
}
