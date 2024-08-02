using Insurance.Core.Features.ICICI.Command.CKYC;
using Insurance.Core.Features.ICICI.Command.UploadICICICKYCDocument;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
using Insurance.Domain.ICICI.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Persistence.ICIntegration.Abstraction;

public interface IICICIService
{
    Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQueryModel, CancellationToken cancellationToken);
    Task<string> GetToken(string leadId, bool quoteToken, bool idvToken, bool ckycToken, bool paymentToken, bool policyGeneration, string stage);
    public Task<ICICIIdvResponseModel> GetIDV(QuoteQueryModel query, CancellationToken cancellationToken);
    Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(ICICICKYCCommand iCICICKYCCommand, CancellationToken cancellationToken);
    Task<Tuple<string, string, UploadCKYCDocumentResponse, CreateLeadModel>> UploadCKYCDocument(UploadICICICKYCDocumentCommand uploadICICICKYCDocument, CancellationToken cancellationToken);
    Task<Tuple<QuoteResponseModel, string, string>> CreateProposal(ICICIProposalRequestDto proposalQuery, ICICIProposalRequest proposalRequest, ICICICKYCRequest iCICICKYCRequest, string vehicleTypeId, CreateLeadModel createLeadModel, CancellationToken cancellationToken);
    Task<Tuple<string, string>> CreatePaymentURL(string leadId, string proposalRequest, string proposalResponse, CancellationToken cancellation);
    Task<ICICIPaymentEnquiryResponse> TransactionEnquiry(string transactionId, string leadId, CancellationToken cancellation);
    Task<ICICIPaymentTaggingResponse> PaymentTagging(ICICIPaymentEnquiryResponse paymentEnquiryResponse, ICICIResponseDto iCICIResponseDto, string dealId, QuoteTransactionRequest quoteTransactionRequest, ICICIPOSPDetails iCICIPOSPDetails, string ProductCode = "");
    Task<byte[]> GeneratePolicy(ICICIPaymentTaggingResponseDto iCICIPaymentTaggingResponseDto, ICICIPOSPDetails iCICIPOSPDetails);
    Task<ICICISubmitPOSPCertificateResponse> SubmitPOSPCertificate(ICICICreateIMBrokerModel iCICICreateIMBrokerModel, CancellationToken cancellationToken);
    Task<ICICICreateIMBrokerResponse> CreateBroker(ICICICreateIMBrokerModel iCICICreateIMBrokerModel, CancellationToken cancellationToken);
    Task<Tuple<string, string, string>> CreateBreakinId(ICICIProposalRequestDto iCICIProposalRequestDto, ICICIResponseDto iCICIResponseDto, string city, string state, string vehicleTypeId, string categoryId, CancellationToken cancellationToken);
    Task<ICICIGetBreakinStatusResponseModel> GetBreakinStatus(CancellationToken cancellationToken);
    Task<ICICIClearInspectionStatusResponse> ClearInspectionStatus(ICICIGetBreakinDetails iCICIGetBreakinDetails, ICICIPOSPDetails iCICIPOSPDetails, CancellationToken cancellationToken);
    #region CV
    Task<Tuple<QuoteResponseModel, string, string>> GetCVQuote(QuoteQueryModel quoteQueryModel, CancellationToken cancellationToken);
    Task<Tuple<QuoteResponseModel, string, string>> CreateCvProposal(ICICICVProposalRequestDto proposalQuery, ICICIProposalRequest proposalRequest, ICICICKYCRequest iCICICKYCRequest, string vehicleTypeId, CreateLeadModel createLeadModel, CancellationToken cancellationToken);
    #endregion
}
