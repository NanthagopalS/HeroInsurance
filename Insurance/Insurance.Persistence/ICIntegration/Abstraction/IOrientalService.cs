using Insurance.Core.Features.Oriental.Command.CKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Oriental;
using Insurance.Domain.Quote;

namespace Insurance.Persistence.ICIntegration.Abstraction
{
    public interface IOrientalService
    {
        Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken);
        Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
        Task<Tuple<QuoteResponseModel, string, string>> GetProposal(OrientalEnvelope quoteRequest,
            CreateLeadModel createLeadModel, OrientalProposalDynamicDetail proposalDynamicDetails, QuoteResponseModel commonResponse, CancellationToken cancellationToken);
        Task<OrientalCKYCStatusResponseModel> GetCKYCResponse(OrientalCKYCDetailsModel orientalCKYCDetailsModel, CancellationToken cancellationToken);
        Task<OrientalUploadCKYCStatusResponseModel> UploadCKYCDocument(OrientalCKYCCommand orientalCKYCCommand, CreateLeadModel createLeadModel, CancellationToken cancellationToken);

    }
}
