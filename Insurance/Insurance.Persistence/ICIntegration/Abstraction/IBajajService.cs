using Insurance.Core.Features.Bajaj.Command.CKYC;
using Insurance.Core.Features.Bajaj.Command.UploadCKYCDocument;
using Insurance.Domain.Bajaj;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Persistence.ICIntegration.Abstraction;
public interface IBajajService
{
    Task<Tuple<QuoteResponseModel, string, string, string>> GetQuote(QuoteQueryModel query, CancellationToken cancellationToken);
    Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<Tuple<string, string, UploadCKYCDocumentResponse, CreateLeadModel>> UploadCKYCDocument(UploadBajajCKYCDocumentCommand uploadBajajCKYCDocument, CancellationToken cancellationToken);
    Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(BajajCKYCCommand cKYCModel, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> CreateProposal(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken);
    Task<byte[]> GeneratePolicy(string leadId, string policyNumber, bool IsTP);
    Task<Tuple<string,string,string, string>> BreakInPinGeneration(string leadId, string transactionId, string vehicleNumber, string mobileNumber, CancellationToken cancellationToken);
    Task<BajajBreakinStatusCheckResponseModel> GetBreakinPinStatus(string leadId, string vehicleNumber, CancellationToken cancellationToken);
}
