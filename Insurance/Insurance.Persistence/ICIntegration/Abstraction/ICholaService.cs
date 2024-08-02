using Insurance.Core.Features.Chola.Command.CKYC;
using Insurance.Core.Features.Chola.Queries.GetBreakinStatus;
using Insurance.Core.Features.Chola.Queries.GetCKYCStatus;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Persistence.ICIntegration.Abstraction;

public interface ICholaService
{
    Task<(string Token, string Expiry)> GetToken(string leadId, string stage);
    Task<CholaIDVResponseModel> GetIDV(QuoteQueryModel quoteQueryModel, CancellationToken cancellationToken);
    Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQueryModel, CancellationToken cancellationToken);
    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<CholaPaymentWrapperModel> GetPaymentDetails(CholaPaymentTaggingRequestModel requestModel, CancellationToken cancellationToken);
    Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(CholaCKYCCommand cholaCKYCCommand, CancellationToken cancellationToken);
    Task<ProposalResponseModel> CreateProposal(CholaServiceRequestModel proposalQuery, CholaProposalRequest proposalRequest, CreateLeadModel createLeadModel, CholaCKYCRequestModel ckycRequestModel, CancellationToken cancellationToken);

    Task<CholaCKYCStatusReponseModel> GetCKYCStatusResponse(GetCholaCKYCStatusQuery cholaCKYCStatusQuery, CancellationToken cancellationToken);
    Task<CholaBreakInResponseModel> GetBreakInStatus(GetBreakinStatusQuery getBreakinStatusQuery, CancellationToken cancellationToken);
    Task<Tuple<string, string, string, string, string>> CreateBreakIn(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken);
    Task<byte[]> GetDocumentPDFBase64(string leadId, string documentLink, string policyId, CancellationToken cancellationToken);
}
