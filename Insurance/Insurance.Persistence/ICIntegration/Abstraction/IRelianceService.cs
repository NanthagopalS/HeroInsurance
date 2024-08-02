using Insurance.Core.Features.Reliance.Command.CKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.Reliance;

namespace Insurance.Persistence.ICIntegration.Abstraction;


public interface IRelianceService
{
    Task<RelianceCoverageResponseDto> GetCoverageDetails(QuoteQueryModel quoteQueryModel, string vehicleNumber, string prevPolicyStartDate, string prevPolicyEndDate,bool manufacturerfullybuild, CancellationToken cancellationToken);
    Task<QuoteResponseModelGeneric> GetQuote(QuoteQueryModel quoteQueryModel, CancellationToken cancellationToken);

    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);

    Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(RelianceCKYCCommand relianceCKYCCommand, CancellationToken cancellationToken);

    Task<ProposalResponseModel> CreateProposal(RelianceRequestDto proposalQuery, RelianceProposalRequest proposalRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken);
    Task<RelianceUploadDocumentResponseModel> GetCKYCPOAResponse(string transactionId, string kycId, CancellationToken cancellationToken);
    Task<ReliancePaymentWrapperModel> GetDocumentPDFBase64(string leadId, string policyNumber, string documentLink, CancellationToken cancellationToken);
}
