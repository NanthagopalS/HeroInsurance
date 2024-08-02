using Insurance.Core.Features.TATA.Command.CKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.TATA;
using Insurance.Domain.Quote;
using Insurance.Domain.Chola;
using Insurance.Domain.HDFC;

namespace Insurance.Persistence.ICIntegration.Abstraction;
public interface ITATAService
{
	Task<QuoteResponseModelGeneric> GetQuote(QuoteQueryModel query, CancellationToken cancellationToken);
    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<ProposalResponseModel> CreateProposal(QuoteTransactionRequest quoteDetails, QuoteConfirmDetailsModel quoteConfirmDetails, TATAProposalRequest proposalRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken);
    Task<TATACKYCStatusResponseModel> PanCKYCVerification(TATACKYCRequestModel tataCKYCRequestModel, CancellationToken cancellationToken);
    Task<TATACKYCStatusResponseModel> POACKYCVerification(TATACKYCRequestModel tataCKYCRequestModel, CancellationToken cancellationToken);
    Task<TATACKYCStatusResponseModel> POAAadharOTPSubmit(POAAadharOTPSubmitRequestModel poaAadharOTPRequest, CancellationToken cancellationToken);
	Task<TATACKYCStatusResponseModel> POADocumentUpdload(POADocumentUploadRequestModel poaDocumentUploadRequest, CancellationToken cancellationToken);
	Task<TATAPaymentResponseDataDto> GetPaymentLink(TATAPaymentRequestModel requestModel, CancellationToken cancellationToken);
	Task<TATAPolicyDocumentResponseModel> GetPolicyDocument(string encriptedPolicyId, string vehicleTypeId, string leadId, CancellationToken cancellationToken);
	Task<TATABreakInResponseModel> VerifyBreakIn(TATABreakInPaymentRequestModel tataBreakInRequestModel, CancellationToken cancellationToken);
    Task<TATAVerifyPaymentStatusResponseDto> VerifyPaymentDetails(string vehicleTypeId, string paymentId, string leadId, CancellationToken cancellationToken);
    Task<TATAQuoteResponseDuringProposal> GetQuoteToAppendPincode(string vehicleTypeId, string requestBody, string pincode, string leadId, CancellationToken cancellationToken);
}
