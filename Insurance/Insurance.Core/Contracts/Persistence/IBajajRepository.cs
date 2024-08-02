using Insurance.Core.Features.Bajaj.Command.CKYC;
using Insurance.Core.Features.Bajaj.Command.UploadCKYCDocument;
using Insurance.Core.Features.Bajaj.Queries.GetQuote;
using Insurance.Domain.Bajaj;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Core.Contracts.Persistence;
public interface IBajajRepository
{
    Task<QuoteResponseModel> GetQuote(GetBajajQuoteQuery request, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> CreateProposal(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken);
    Task<Tuple<byte[]>> GetPolicy(string leadId, string policyNumber, bool IsTP);
    Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<Tuple<BajajBreakinStatusCheckResponseModel, CreateLeadModel>> GetBreakinPinStatus(string leadId, string vehicleNumber, CancellationToken cancellationToken);
    Task<SaveCKYCResponse> SaveCKYC(BajajCKYCCommand bajajCKYCCommand, CancellationToken cancellationToken);
    Task <UploadCKYCDocumentResponse> UploadCKYCDocument(UploadBajajCKYCDocumentCommand uploadBajajCKYC, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> GenerateBreakinPin(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken);
    Task<string> GetQuoteTransactionId(string quoteTransactionId, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> GetPaymentLink(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken);
    Task<VariantAndRTOIdCheckModel> DoesBajajVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber,CancellationToken cancellationToken);
}
