using Insurance.Core.Features.Chola.Command.CKYC;
using Insurance.Core.Features.IFFCO.Command.CKYC;
using Insurance.Core.Features.IFFCO.Command.CreateCKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Persistence.ICIntegration.Abstraction;
public interface IIFFCOService
{
    Task<IFFCOIDVResponseModel> GetIDV(QuoteQueryModel quoteQueryModel, CancellationToken cancellation);
    Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken);
    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(IFFCOCKYCCommand iffcoCKYCCommand, CancellationToken cancellationToken);
    Task<Tuple<string, string, UploadCKYCDocumentResponse, CreateLeadModel>> UploadCKYCDocument(CreateIFFCOCKYCCommand createIFFCOCKYCCommand, CancellationToken cancellationToken);
    Task<Tuple<QuoteResponseModel, string>> GetProposal(IFFCOEnvelope quoteRequest, IFFCOEnvelope quoteResponse,
        CreateLeadModel createLeadModel, IFFCOProposalDynamicDetails proposalDynamicDetails, QuoteResponseModel commonResponse,IFFCOPreviousPolicyDetailsModel iFFCOPreviousPolicyDetailsModel,CancellationToken cancellationToken);
    Task<IFFCOPolicyDocumentResponse> GetPolicyDownloadURL(IFFCOPaymentResponseModel iFFCOPaymentResponseModel, CancellationToken cancellationToken);
    Task<byte[]> PolicyDownload(string leadId, string url, CancellationToken cancellationToken);
    Task<Tuple<string, string, string, string>> GenerateBreakin(IFFCOProposalDynamicDetails iFFCOProposalDynamicDetails, CreateLeadModel createLeadModel, CancellationToken cancellationToken);
    Task<IFFCOBreakinStatusResponseModel> GetBreakinStatus(string leadId, string breakinId, CancellationToken cancellationToken);
    Task<IFFCOIDVResponseModel> GetCVIDV(QuoteQueryModel quoteQueryModel, CancellationToken cancellation);
}
