using Insurance.Core.Features.IFFCO.Command.CKYC;
using Insurance.Core.Features.Oriental.Command;
using Insurance.Core.Features.Oriental.Command.CKYC;
using Insurance.Core.Features.Oriental.Queries.GetQuote;
using Insurance.Core.Features.TATA.Command.CKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Oriental;
using Insurance.Domain.Quote;

namespace Insurance.Core.Contracts.Persistence;

public interface IOrientalRepository
{
    Task<QuoteResponseModel> GetQuote(GetOrientalQuoteQuery query, CancellationToken cancellationToken);
    Task<VariantAndRTOIdCheckModel> DoesOrientalVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, CancellationToken cancellationToken);
    Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken);
    Task<SaveQuoteTransactionModel> CreateProposal(QuoteTransactionDbModel quoteDetails, CancellationToken cancellationToken);
    Task<OrientalCKYCStatusResponseModel> GetCKYCDetails(string proposalDynamicDetail, string proposalNumber, CreateLeadModel createLeadModel, string quoteTransactionId, CancellationToken cancellationToken);
    Task<OrientalUploadCKYCStatusResponseModel> UploadCKYC(OrientalCKYCCommand orientalCKYCCommand, CreateLeadModel createLeadModel, CancellationToken cancellationToken);
    Task<CreateLeadModel> GetDetailsForKYCUpload(string quoteTransactionId, CancellationToken cancellationToken);

}
