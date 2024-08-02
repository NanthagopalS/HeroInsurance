using Insurance.Web.Models;

namespace Insurance.Web.Abstraction;

public interface IInsuranceService
{
    Task<PaymentDetailsVm> SavePaymentStatus(string applicationId, string userId, string transactionNumber, CancellationToken cancellationToken);
    Task<PaymentDetailsVm> SaveBajajPaymentStatus(BajajPaymentResponseModel bajajPaymentResponseModel, CancellationToken cancellationToken);
    Task<PaymentDetailsVm> SaveBajajTPPaymentStatus( BajajPaymentResponseModel bajajPaymentResponseModel,
        CancellationToken cancellationToken);
    Task<PaymentDetailsVm> SaveICICIPaymentStatus(string userId, string correlationId, string transactionId);
    Task<CKYCPOAResponseModel> GetHDFCPOAStatus(HDFCPOAResponse hdfcPOAResponse, CancellationToken cancellationToken);
    Task<HeroResult> SaveHDFCPaymentStatus(string quoteTransactionId, string userId, HDFCPaymentResponseModel hdfcPaymentModel, CancellationToken cancellationToken);
    Task<PaymentDetailsVm> SaveCholaPaymentStatus(string quoteTransactionId, string userId, CholaPaymentResponseModel cholaPaymentModel, CancellationToken cancellationToken);
    Task<CKYCPOAResponseModel> GetCholaCKYCDetails(CholaKYCResponse cholaKYCResponse, CancellationToken cancellationToken);
    Task<CKYCPOAResponseModel> GetRelianceCKYCDetails(RelianceKYCResponse cholaKYCResponse, CancellationToken cancellationToken);

    Task<PaymentDetailsVm> SaveReliancePaymentStatus(string quoteTransactionId, string userId, ReliancePaymentResponseModel reliancePaymentModel, CancellationToken cancellationToken);
    Task<ICICIHeroResult> SaveIFFCOPaymentStatus(IFFCOPaymentResponseModel iFFCOPaymentResponseModel, CancellationToken cancellationToken);
    Task<HeroResult> GetProposalDetails(string insurerId, string quotransactionId, string userId, CancellationToken cancellationToken);
    Task<string> GetUserIdDetails(string proposalNumber, CancellationToken cancellationToken);
    Task<ICICIHeroResult> SaveTATAPaymentStatus(string userId, TATAPaymentResponseModel tATAPaymentResponse, CancellationToken cancellationToken);
    Task<ICICIHeroResult> SaveUnitedIndiaPaymentStatus(string userId, UnitedIndiaPaymentResponseModel unitedIndiaPaymentResponse, CancellationToken cancellationToken);
    Task<LeadDetails> SaveUnitedIndiaCKYCStatus(UnitedIndiaCKYCResponseModel unitedIndiaCKYCResponseModel, CancellationToken cancellationToken);
}
