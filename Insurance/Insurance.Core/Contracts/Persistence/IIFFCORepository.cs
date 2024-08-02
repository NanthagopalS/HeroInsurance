using Insurance.Core.Features.IFFCO.Command.CKYC;
using Insurance.Core.Features.IFFCO.Command.CreateCKYC;
using Insurance.Core.Features.IFFCO.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Contracts.Persistence
{
    public interface IIFFCORepository
    {
        Task<QuoteResponseModel> GetQuote(GetIFFCOQuery query, CancellationToken cancellationToken);
        Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
        Task<SaveCKYCResponse> GetCKYCDetails(IFFCOCKYCCommand request, CancellationToken cancellationToken);
        Task<UploadCKYCDocumentResponse> UploadCKYCDocument(CreateIFFCOCKYCCommand createIFFCOCKYCCommand, CancellationToken cancellationToken);
        Task<SaveQuoteTransactionModel> CreateProposal(QuoteTransactionDbModel quoteDetails, CancellationToken cancellationToken);
        Task<IFFCOPolicyDocumentResponse> GetPolicyDocumentURL(IFFCOPaymentResponseModel iFFCOPaymentResponseModel, CancellationToken cancellationToken);
        Task<byte[]> GetPolicyDocument(string leadId, string url, CancellationToken cancellationToken);
        Task<string> GetProposalQuotetransactionId(string proposalNumber, CancellationToken cancellationToken);
        Task<SaveQuoteTransactionModel> GenerateBreakin(QuoteTransactionDbModel quoteDetails, CancellationToken cancellationToken);
        Task<IFFCOBreakinStatusResponseModel> BreakinInspectionStatus(string leadId, string breakinId, CancellationToken cancellationToken);
        Task<Tuple<string, string, string>> GetPaymentLink(string proposalRequest, bool isCorporate, CancellationToken cancellationToken);
        Task<CreateLeadModel> GetBreakinDetails(string quoteTransactionId, CancellationToken cancellationToken);
        Task<string> UpdateLeadPaymentLink(string insurerId, string quoteTransactionId, string paymentLink, string uniqId, string proposalRequest, string oldProposalNumber, CancellationToken cancellationToken);
        Task UpdateIFFCOProposalId(string quoteTransactionId, string proposalNumber, CancellationToken cancellationToken);
        Task<VariantAndRTOIdCheckModel> DoesIFFCOVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber,CancellationToken cancellationToken);
    }
}
