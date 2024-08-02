using Identity.Core.Features.Authenticate.Commands.ResetPasswordAdmin;
using Identity.Domain.Authentication;
using Identity.Domain.UserLogin;

namespace Identity.Core.Contracts.Persistence;
public interface IAuthenticateRepository
{
    Task<AuthenticationResponse> AuthenticateUser(string userId, string OTP, CancellationToken cancellationToken);
    Task<AuthenticationAdminResponse> AuthenticateAdminUser(string emailId, string Password ,CancellationToken cancellationToken);
    Task<UserLoginResponseModel> SendSMS(string mobileNo);
    Task<VerifyOTPResponse> VerifyOTP(string otp, string userId);
    string CreateJWTTokenForPOSP(POSPDetailsModel pOSPDetailsModel);
    Task<ResetPasswordAdminVm> ResetPasswordAdmin(string userId, CancellationToken cancellationToken);

}
