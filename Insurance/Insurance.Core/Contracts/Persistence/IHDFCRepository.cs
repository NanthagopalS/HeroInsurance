using Insurance.Core.Features.HDFC.Command.CKYC;
using Insurance.Core.Features.HDFC.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.ICICI;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Core.Contracts.Persistence;
public interface IHDFCRepository
{
    Task<QuoteResponseModel> GetQuote(GetHdfcQuoteQuery request, CancellationToken cancellationToken);
    Task<HDFCCkycPOAStatusResponseModel> GetCKYCPOAStatus(string kycId, string leadId, CancellationToken cancellationToken);
    Task<HDFCUploadDocumentResponseModel> GetCKYCPOAResponse(string quoteTransactionId, string kycId,string leadId, CancellationToken cancellationToken);
    Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> SaveCKYC(HDFCCKYCCommand hdfcCKYCCommand, CancellationToken cancellationToken);
    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> CreateProposal(HDFCServiceRequestModel proposalQuery, HDFCProposalRequest proposalRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken);
    Task<string> GeneratePaymentCheckSum(string transactionId, string amount, string redirectionURL,string leadId,CancellationToken cancellationToken);
    Task<HDFCPolicyDocumentResponseModel> GetPolicyDocument(HDFCPolicyRequestModel hdfcPolicyReqModel, CancellationToken cancellationToken);
    Task<bool> CreatePOSP(HDFCCreateIMBrokerRequestDto model, CancellationToken cancellationToken);
    public Task<HDFCPolicyRequestModel> GetPaymentFields(string applicationId, CancellationToken cancellationToken);
    Task<string> GetPaymentLink(BreakInPaymentDetailsDBModel breakInPaymentDetailsDBModel, string QuoteTransactionId, CancellationToken cancellationToken);
    Task<VariantAndRTOIdCheckModel> DoesHDFCVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber,string vehicleTypeId, CancellationToken cancellationToken);
}
