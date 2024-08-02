using ThirdPartyUtilities.Models.Signzy;

namespace ThirdPartyUtilities.Abstraction;
public interface ISignzyService
{
    Task<AuthenticationResponse> AuthenticateSignzy(CancellationToken cancellationToken);
    Task<BankVerificationResponse> GetBankVerification(string beneficiaryMobile, string beneficiaryAccount, string beneficiaryName, string beneficiaryIFSC, CancellationToken cancellationToken);
    Task<PANVerificationResponse> GetPANDetails(string panNumber, CancellationToken cancellationToken);
    Task<VehicleRegistrationResponse> GetVehicleRegistrationDetails(string vehicleNumber, CancellationToken cancellationToken);
}
