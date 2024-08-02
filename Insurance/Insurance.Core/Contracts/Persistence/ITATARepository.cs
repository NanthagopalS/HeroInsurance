using Insurance.Core.Features.TATA.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;

namespace Insurance.Core.Contracts.Persistence;
public interface ITATARepository
{
    Task<QuoteResponseModel> GetQuote(GetTATAQuoteQuery request, CancellationToken cancellationToken);
    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> CreateProposal(QuoteTransactionRequest quoteDetails, QuoteConfirmDetailsModel quoteConfirmDetails, TATAProposalRequest proposalRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken);
    Task<TATACKYCStatusResponseModel> PanCKYCVerification(TATACKYCRequestModel tataCKYCRequestModel, CancellationToken cancellationToken);
    Task<TATACKYCStatusResponseModel> POACKYCVerification(TATACKYCRequestModel tataCKYCRequestModel, CancellationToken cancellationToken);
    Task<TATACKYCStatusResponseModel> POAAadharOTPSubmit(POAAadharOTPSubmitRequestModel poaAadharOTPRequest, CancellationToken cancellationToken);
    Task<TATACKYCStatusResponseModel> POADocumentUpdload(POADocumentUploadRequestModel poaDocumentUploadRequest, CancellationToken cancellationToken);
	Task<TATAPaymentResponseDataDto> GetPaymentLink(TATAPaymentRequestModel tATAPaymentRequestModel, CancellationToken cancellationToken);
	Task<TATABreakInResponseModel> VerifyBreakIn(TATABreakInPaymentRequestModel tataBreakInRequestModel, CancellationToken cancellationToken);
    Task<TATAVerifyPaymentStatusResponseDto> VerifyPaymentDetails(string vehicleTypeId, string paymentId, string leadId, CancellationToken cancellationToken);
    Task<TATAPolicyDocumentResponseModel> GetPolicyDocument(string encriptedPolicyId, string vehicleTypeId, string leadId, CancellationToken cancellationToken);
    Task<VariantAndRTOIdCheckModel> DoesTATAVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, CancellationToken cancellationToken);
    Task<TATARequestModelForPaymentVerify> GetLeadIdAndPaymentId(string QuoteTransactionId, CancellationToken cancellationToken);
    Task<CreateLeadModel> GetDetailsForKYCAfterProposal(string quoteTransactionId, CancellationToken cancellationToken);
    Task<TATAQuoteResponseDuringProposal> GetQuoteToAppendPincode(string vehicleTypeId, string requestBody, string pincode, string leadId, CancellationToken cancellationToken);
}

